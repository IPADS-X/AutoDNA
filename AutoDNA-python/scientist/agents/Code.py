import traceback
import re
import os
from loguru import logger
from langchain_core.tools import tool
from llm.model import code_model
from agents.constants import AgentOutputPrefix
from config import output_path, input_path, hardware_abstractions_path, corrector_path, shared_files_path, scheduler_path, scheduler_config_path, current_output_file_path
from tools.file_manager import file_manager
from tools.utils import format_reagents_json, get_inventory, CoflowCache, InitialCodeCache
from tools.reagent_manager import reagent_manager
from prompts.agents.Code.prompt import *
import subprocess
from langchain_core.prompts import PromptTemplate
import sys
import concurrent.futures
from datetime import datetime
import secrets
import json
from tools.web_socket import WebSocketClient

from agents.Hardware import execute_code_to_scheduler


SEPARATE_PATTERN = r"## SCRIPT START ##"
SEPARATE_PATTERN_PROTOCOL = r"### PATH START ###"
MAX_CORRECTION_ATTEMPTS = 3

global_error_message = ""
global_initial_code = ""
global_corrected_code = ""

# --- Static Initializer Decorator (Keep as is) ---
def static_init(cls):
    if getattr(cls, "static_init", None):
        cls.static_init()
    return cls

def extract_python_code(code_string: str) -> str:
    """
    Extracts Python code from a markdown block if it exists.

    Args:
        code_string (str): The string potentially containing a python code block.

    Returns:
        str: The cleaned Python code, stripped of markdown fences.
    """
    cleaned_code = code_string.strip()
    if cleaned_code.startswith("```python") and cleaned_code.endswith("```"):
        # Remove ```python from the start
        cleaned_code = cleaned_code[len("```python"):].strip()
        # Remove ``` from the end
        cleaned_code = cleaned_code[:-len("```")].strip()
    return cleaned_code

def corrector(original_code: str, error_message: str, hardware_abstractions: str, coder_hints: str) -> str:
    """Correct the code based on the error message."""
    final_prompt = prompt_corrector.format(
        original_code=original_code,
        error_message=error_message,
        hardware_abstractions=hardware_abstractions,
        coder_hints=coder_hints
    )

    # for debug, save the final prompt
    with open(os.path.join(InitialCodeCache.cache_dir, 'last_corrector_prompt.md'), 'w', encoding='utf-8') as f:
        f.write(final_prompt)
    response = code_model.invoke(final_prompt)

    if response:
        return extract_python_code(response.content)
    else:
        raise ValueError("No response from corrector.")

def linearize_procedure(input_text: str, LLM_model) -> str:
    """Linearize the procedure from the user input."""
    prompt = PromptTemplate.from_template(
        """
        You are a procedure linearizer that single out individual procedure path from a procedure that has multiple options. 
        """
        + f"\n{extractor_prompt_extract}"
        """
        The original procedure containing multiple options is :
        {procedure}
        """
    )

    procedure = input_text
    
    final_prompt = prompt.format(procedure = procedure)
    # for debug, save the final prompt
    with open(os.path.join(InitialCodeCache.cache_dir, 'last_linearizer_prompt.md'), 'w', encoding='utf-8') as f:
        f.write(final_prompt)
    response = LLM_model.invoke(final_prompt)

    if response:
        return response.content
    else:
        raise ValueError("No response from procedure linearizer.")

def separate_workflows(text_data: str) -> list[str]:
    """
    Separates a block of text into individual workflows using a flexible delimiter.

    This function uses a regular expression to split the input string by SEPARATE_PATTERN.
    Args:
        text_data: A string containing one or more workflows.

    Returns:
        A list of strings, where each string is an individual workflow.
    """

    # splitter.
    split_pattern = SEPARATE_PATTERN_PROTOCOL

    # Split the text using the regex pattern
    workflows = re.split(split_pattern, text_data)

    # Filter out empty strings from the result and strip whitespace
    cleaned_workflows = [workflow.strip() for workflow in workflows if workflow.strip()]

    return cleaned_workflows

def get_formated_inventory():
    # return a json file
    inventory = get_inventory()
    return format_reagents_json(inventory)

def no_instrument_invoke(scheme: str, multiple_inputs: str = "") -> str:
    full_prompt = prompt_no_instrument.format(coder_format=coder_prompt_format, scheme=scheme)
    if multiple_inputs != "":
        full_prompt = full_prompt + prompt_multiple_inputs.format(multi_inputs=multiple_inputs)
    llm_response = code_model.invoke(full_prompt).content
    file_id = file_manager.add_file(AgentOutputPrefix.PDA_OUTPUT, llm_response)
    return "The code has been saved to " + file_id

def execute_code(code: str, lib_path: str = None) -> dict:
    """
    Executes a single string of Python code in a designated environment
    and captures the output.

    Args:
        code (str): The Python code to execute.
        lib_path (str, optional): The designated path to the libraries/modules
                                needed by the code. Defaults to None.

    Returns:
        dict: A dictionary containing the execution status, stdout, and stderr.
    """
    try:
        # 1. Copy the current environment variables
        env = os.environ.copy()

        # 2. If a library path is provided, set it as the PYTHONPATH
        #    This tells the Python interpreter where to look for modules.
        env['PYTHONUTF8'] = '1'
        if lib_path:
            env['PYTHONPATH'] = lib_path

        # 3. Run the code in a subprocess with the modified environment
        result = subprocess.run(
            [sys.executable, "-c", code],
            capture_output=True,
            text=True,
            timeout=30,
            env=env,  # Pass the modified environment here
            encoding='utf-8' 
        )

        status = 'success'
        if result.stderr:
            logger.warning(f"Execution resulted in stderr: {result.stderr.strip()}")
            
        return {"status": status, "stdout": result.stdout, "stderr": result.stderr}

    except subprocess.TimeoutExpired:
        return {"status": "error", "stdout": "", "stderr": "Execution timed out."}
    except Exception as e:
        return {"status": "error", "stdout": "", "stderr": f"An unexpected error occurred: {e}"}

def correct_and_execute_code(code: str, hardware_abstractions: str, coder_hints: str, max_retries: int = MAX_CORRECTION_ATTEMPTS) -> tuple[str, bool]:
    """
    Tries to execute code, and if it fails, corrects it iteratively.
    Logs each correction attempt to a dedicated, human-readable cache folder.

    Args:
        code (str): The initial Python code to execute.
        hardware_abstractions (str): The hardware abstraction layer details.
        coder_hints (str): Hints for the coder LLM.
        max_retries (int): The maximum number of correction attempts.

    Returns:
        tuple[str, bool]: A tuple containing the final version of the code and a boolean indicating success.
    """
    current_code = code

    # --- Correction Cache Setup ---
    correction_cache_dir = os.path.join(shared_files_path, 'correction_cache')
    os.makedirs(correction_cache_dir, exist_ok=True)
    session_id = f"{datetime.now().strftime('%Y%m%d_%H%M%S')}_{secrets.token_hex(4)}"
    session_dir = os.path.join(correction_cache_dir, session_id)
    os.makedirs(session_dir, exist_ok=True)
    logger.info(f"Correction session started. Logs will be saved in: {session_dir}")
    # --- End Correction Cache Setup ---

    for i in range(max_retries):
        exec_result = execute_code(current_code, lib_path=corrector_path)
        # If there is no standard error, the execution is successful
        if not exec_result['stderr'].strip():
            logger.info(f"Code executed successfully on attempt {i+1}.")
            return current_code, True

        error_message = exec_result['stderr']
        global global_error_message
        global_error_message = error_message
        logger.warning(f"Execution failed on attempt {i+1}. Error: {error_message}. Attempting to correct.")
        
        # --- Log Correction Input ---
        input_log_path = os.path.join(session_dir, f'attempt_{i+1}_input.log')
        with open(input_log_path, 'w', encoding='utf-8') as f:
            f.write("--- ORIGINAL CODE ---\n")
            f.write(current_code)
            f.write("\n\n--- ERROR MESSAGE ---\n")
            f.write(error_message)
        # --- End Log Correction Input ---

        corrected_code = corrector(
            original_code=current_code,
            error_message=error_message,
            hardware_abstractions=hardware_abstractions,
            coder_hints=coder_hints
        )
        current_code = corrected_code

        # --- Log Correction Output ---
        output_log_path = os.path.join(session_dir, f'attempt_{i+1}_output.py')
        with open(output_log_path, 'w', encoding='utf-8') as f:
            f.write(corrected_code)
        # --- End Log Correction Output ---

    # A final check after all retries have been exhausted
    final_exec_result = execute_code(current_code, lib_path=corrector_path)
    if not final_exec_result['stderr'].strip():
        logger.info("Code executed successfully after final correction attempt.")
        return current_code, True
    else:
        logger.error(f"Failed to correct code after {max_retries} attempts. Last error: {final_exec_result['stderr']}")
        return current_code, False
    
def change_code_from_scheduler(json_str:str):
    try:
        json_obj = json.loads(json_str)
    except json.JSONDecodeError:
        logger.error("Error: Invalid JSON format.")
        return False

    if json_obj.get("type") != "call_code_agent":
        logger.warning("The request is not for a code agent.")
        return False
    
    workflow_name = json_obj.get("workflow_name")
    error_message = json_obj.get("message")
    repeat_num = json_obj.get("times")
    
    logger.debug("get change code request: ", json_obj)
    
    code_to_be_corrected = file_manager.get_file_content(workflow_name)
    if not code_to_be_corrected:
        logger.error(f"File {workflow_name} does not exist! Something went wrong!")
        return False
    # step1: call corrector, just combine original code and error message
    with open(hardware_abstractions_path, 'r', encoding='utf-8') as file:
        hardware_abstractions = file.read()
    new_code = corrector(code_to_be_corrected, error_message,hardware_abstractions, coder_prompt_hints)
    # save the file to file manager
    file_manager.modify_file(workflow_name, new_code)
    
    # step2: execute code
    try:
        execute_code_to_scheduler([workflow_name], lib_path=os.path.abspath(scheduler_path), times=int(repeat_num))
    except json.JSONDecodeError:
        logger.error("execute code to scheduler failed.")
    

@tool
def Code(code_request_id: str, pure_software: bool = False, multiple_inputs: str = "", inputs_from_previous_stage : str = "") -> str:
    """
    Generates the Python code script for a validated experiment procedure.

    Args:
        code_request_id (str): A file_id containing the experiment procedure to be transformed to code.
        pure_software (bool): A flag indicating if the task is purely software-based.
        multiple_inputs (str): If there are multiple different inputs for the protocol, then fill the parameter with all of the different inputs. Otherwise, leave it empty.
        inputs_from_previous_stage (str): If the outputs from the previous stage has something to do with the current stage and multiple_inputs is empty, then populate this parameter with the information from the previous stage that may serve as inputs.

    Returns:
        str: A string containing the comma separated id list of the generated Python code scripts.
    """
    
    ###### NOTE: REGISTER CALLBACK HERE #################
    try:
        WebSocketClient.get_instance().register_callback("Code Agent", change_code_from_scheduler)
    except Exception as e:
        logger.error(f"Failed to register callback: {e}")
    ###### NOTE: REGISTER CALLBACK HERE #################

    logger.info("!!!CODER INVOKED!!!")
    res = file_manager.get_file_content(code_request_id)


    if pure_software:
        return no_instrument_invoke(res, multiple_inputs if inputs_from_previous_stage == "" else inputs_from_previous_stage)

    # --- Construct the prompt ---
    inventory = get_formated_inventory()
    if not os.path.exists(hardware_abstractions_path):
        raise FileNotFoundError(f"Hardware abstraction file not found at: {hardware_abstractions_path}")
    with open(hardware_abstractions_path, 'r', encoding='utf-8') as file:
        hardware_abstractions = file.read()
    
    if res:
        workflow = res
    else:
        logger.error(f"File with ID {code_request_id} not found.")
        return f"Error: File with ID {code_request_id} not found."
    
    workflow = linearize_procedure(workflow, code_model)

    with open(os.path.join(InitialCodeCache.cache_dir, 'last_linearized_workflow.md'), 'w', encoding='utf-8') as f:
        f.write(workflow)
    workflows = separate_workflows(workflow)

    full_prompt = prompt_code_main.format(
        inventory=inventory,
        hardware_abstractions=hardware_abstractions,
        coder_hints=coder_prompt_hints,
        coder_format=coder_prompt_format,
        workflow=workflow
    )

    if multiple_inputs.strip() != "":
        full_prompt += prompt_multiple_inputs.format(multi_inputs=multiple_inputs)
    elif inputs_from_previous_stage.strip() != "":
        full_prompt += prompt_inputs_from_previous_stage.format(inputs_from_previous_stage=inputs_from_previous_stage)
    
    with open(os.path.join(InitialCodeCache.cache_dir, 'last_initial_prompt.md'), 'w', encoding='utf-8') as f:
        f.write(full_prompt)

    # cache check
    chosen_llm_response = ""
    cache_name = f"{AgentOutputPrefix.PDA_OUTPUT}_cache.json"
    cache_result = file_manager.load_from_cache_kv(cache_name, code_request_id)
    if cache_result:
        logger.info(f"Cache hit for request '{code_request_id}'")
        chosen_llm_response = cache_result
    else:
        # --- Call the LLM for initial code generation ---
        logger.info("Generating initial code from LLM...")
        try:
            response = code_model.invoke(full_prompt)
            if not response or not response.content:
                logger.error("LLM returned an empty response.")
                return "An error occurred: No response received from the code generation LLM."
            
            chosen_llm_response = response.content
            logger.info("Successfully received response from LLM.")
            # save to cache
            file_manager.save_to_cache_kv(cache_name, code_request_id, chosen_llm_response)
        except Exception as e:
            logger.error(f"Error during code generation: {e}\n{traceback.format_exc()}")
            return "An error occurred during the code generation LLM call."

    # --- Extract and Validate the CHOSEN code ---
    parts = chosen_llm_response.split(SEPARATE_PATTERN)
    initial_codes = []
    for (i, part) in enumerate(parts):
        if len(part) < 100:
            continue
        code_content = extract_python_code(part.strip())
        global_initial_code = "```python" + os.linesep + code_content + os.linesep + "```"
        initial_codes.append(code_content)
        with open(os.path.join(InitialCodeCache.cache_dir, f'chosen_code_option_{i}.py'), 'w', encoding='utf-8') as f:
            f.write(code_content)
    
    if len(workflows) != len(initial_codes):
        logger.error(f"Mismatch: {len(workflows)} workflows and {len(initial_codes)} code scripts were generated from the chosen option. Aborting.")
        return "Error: Mismatch between the number of generated workflows and code scripts from the chosen option."

    # --- Correct and Execute Concurrently (for the chosen set of codes) ---
    final_codes = []
    successful_workflows = []

    with open(current_output_file_path, "r", encoding="utf-8") as f:
        all_steps_data = json.load(f)
    step_key = "stepX"
    step_data = [
        [
            [
                {
                    "role": "assistant",
                    "agent": "Code Agent",
                    "content": global_initial_code
                }
            ]
        ],
        # check if tool_calls is just []
        ["Code Agent: Code Generation"]
    ]
    all_steps_data[step_key] = step_data
    # write back to the file
    with open(current_output_file_path, "w", encoding="utf-8") as f:
        json.dump(all_steps_data, f, indent=4)

    with concurrent.futures.ThreadPoolExecutor() as executor:
        future_to_code = {
            executor.submit(
                correct_and_execute_code, 
                code, 
                hardware_abstractions, 
                coder_prompt_hints
            ): (workflows[i]) for i, code in enumerate(initial_codes)
        }
        for future in concurrent.futures.as_completed(future_to_code):
            associated_workflow = future_to_code[future]
            try:
                corrected_code, success = future.result()
                if success:
                    logger.info("A code script has been successfully validated.")
                    final_codes.append(corrected_code)
                    global_corrected_code = "```python" + os.linesep + corrected_code + os.linesep + "```"
                    successful_workflows.append(associated_workflow)
                else:
                    logger.error("A code script failed validation after multiple correction attempts.")
            except Exception as exc:
                logger.error(f'A thread for code correction generated an exception: {exc}')

    if not final_codes:
        return "Error: All generated code scripts failed execution and could not be corrected."

    with open(current_output_file_path, "r", encoding="utf-8") as f:
        all_steps_data = json.load(f)
    step_key = "stepY"
    step_data = [
        [
            [
                {
                    "role": "assistant",
                    "agent": "Code Agent",
                    "content": global_error_message
                },
                {
                    "role": "assistant",
                    "agent": "Code Agent",
                    "new_content": global_initial_code,
                    "content" : global_corrected_code
                }
            ]
        ],
        # check if tool_calls is just []
        ["Code Agent: Code Correction"]
    ]
    all_steps_data[step_key] = step_data
    # write back to the file
    with open(current_output_file_path, "w", encoding="utf-8") as f:
        json.dump(all_steps_data, f, indent=4)
    # --- Save successful codes and workflows ---
    procedure_file_ids = file_manager.add_batch_files(AgentOutputPrefix.PGA_OUTPUT, successful_workflows)
    file_ids = file_manager.add_batch_files(AgentOutputPrefix.PDA_OUTPUT, final_codes)
    
    # Construct file mapping between code and procedure
    for code_id, procedure_id in zip(file_ids, procedure_file_ids):
        file_manager.add_file_mapping(code_id, procedure_id)
        
    file_ids_str = ', '.join(file_ids)

    # --- Cache and Return Success ---
    InitialCodeCache.push_code(workflow, "\n".join(final_codes))
    logger.info("Code generation, correction, and execution completed successfully.")
    return f"result: {file_ids_str}"