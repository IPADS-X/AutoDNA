from loguru import logger
from langchain.tools import tool
import subprocess
from tools.utils import SelfOptimizingCache, get_current_user_prompt, insert_intermediate_reagents
from langchain_core.prompts import PromptTemplate
from llm.model import reasoner_model, code_model
from config import base_path, output_path, executor_path, settings, scheduler_path, scheduler_config_path
from agents.Literature import answer_one_question
from agents.constants import AgentName, AgentOutputPrefix
from prompts.agents.Hardware.prompt import intermediate_products_prompt
from tools.file_manager import file_manager
from tools.utils import CoflowCache, to_numeric, WorkflowCache
import os
import sys
from enum import Enum
import asyncio
import json

from tools.web_socket import WebSocketClient

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
            logger.warning(f"Error is {result.stderr.strip()}")
            
        return {"status": status, "stdout": result.stdout, "stderr": result.stderr}

    except subprocess.TimeoutExpired:
        return {"status": "error", "stdout": "", "stderr": "Execution timed out."}
    except Exception as e:
        return {"status": "error", "stdout": "", "stderr": f"An unexpected error occurred: {e}"}

def execute_and_log_files(file_ids: list[str], executor_path: str) -> str:
    """
    Executes a list of Python script files, logs the results, and returns the aggregated stdout.
    
    Args:
        file_ids (list[str]): A list of file IDs to execute.
        executor_path (str): The path to the execution environment libraries.
        
    Returns:
        str: A string containing the combined standard output of all successful runs.
    """
    local_run_result = ""
    for file_id in file_ids:
        code = file_manager.get_file_content(file_id)
        if code is None:
            logger.error(f"Failed to retrieve code for file ID: {file_id}")
            continue
        
        exec_result = execute_code(code, lib_path=os.path.abspath(executor_path))
        
        if exec_result['status'] == 'success':
            logger.info(f"Code execution successful for file ID: {file_id}")
            logger.info(f"Standard Output:\n{exec_result['stdout']}")
            if exec_result['stderr']:
                logger.warning(f"Standard Error:\n{exec_result['stderr']}")
            local_run_result += exec_result['stdout']
        else:
            logger.error(f"Code execution failed for file ID: {file_id}")
            logger.error(f"Error Message:\n{exec_result['stderr']}")
            
    return local_run_result


def _ordered_execution_mode(file_ids: list[str]) -> tuple[str, list[str]]:
    metadata = [file_manager.get_file_metadata(file_id) for file_id in file_ids]
    parallel_metadata = [
        item for item in metadata if item.get("execution_mode") == "parallel_batch"
    ]

    if not parallel_metadata:
        return "optimization", list(file_ids)
    if len(parallel_metadata) != len(file_ids):
        raise ValueError(
            "A parallel batch cannot be mixed with ordinary or optimization code IDs."
        )

    batch_ids = {item.get("batch_id") for item in parallel_metadata}
    batch_sizes = {item.get("batch_size") for item in parallel_metadata}
    batch_indexes = [item.get("batch_index") for item in parallel_metadata]
    if None in batch_ids or len(batch_ids) != 1:
        raise ValueError("Parallel code IDs do not belong to one complete batch.")
    if batch_sizes != {len(file_ids)}:
        raise ValueError(
            "Parallel code IDs are incomplete; Hardware must receive the whole Code batch."
        )
    if set(batch_indexes) != set(range(len(file_ids))):
        raise ValueError("Parallel code IDs have invalid or duplicate batch indexes.")

    ordered_ids = [
        file_id
        for _, file_id in sorted(
            zip(batch_indexes, file_ids), key=lambda indexed_id: indexed_id[0]
        )
    ]
    return "parallel_batch", ordered_ids


def _convert_code_to_scheduler_file(file_id: str, lib_path: str) -> str:
    code = file_manager.get_file_content(file_id)
    if not code:
        raise ValueError(f"No Python code was found for workflow {file_id}.")

    config_dir = os.path.abspath(os.path.expanduser(scheduler_config_path))
    os.makedirs(config_dir, exist_ok=True)
    origin_json_file_path = os.path.join(config_dir, "protocol_flow.json")
    target_json_file_path = os.path.join(
        config_dir,
        f"protocol_flow_{file_id}.json",
    )
    if os.path.exists(origin_json_file_path):
        os.remove(origin_json_file_path)

    env = os.environ.copy()
    env['PYTHONUTF8'] = '1'
    env["SCHEDULER_CONFIG_PATH"] = config_dir
    if lib_path:
        existing_pythonpath = env.get("PYTHONPATH")
        env["PYTHONPATH"] = os.path.abspath(lib_path)
        if existing_pythonpath:
            env["PYTHONPATH"] += os.pathsep + existing_pythonpath

    conversion = subprocess.run(
        [sys.executable, "-c", code],
        capture_output=True,
        text=True,
        timeout=30,
        env=env,
        encoding='utf-8',
    )
    if conversion.returncode != 0:
        details = conversion.stderr.strip() or conversion.stdout.strip() or "no output"
        raise RuntimeError(f"Workflow conversion failed for {file_id}: {details}")
    if not os.path.isfile(origin_json_file_path):
        raise RuntimeError(f"Workflow converter produced no JSON for {file_id}.")

    with open(origin_json_file_path, 'r', encoding='utf-8') as f:
        data = json.load(f)
    if not isinstance(data, dict) or not isinstance(data.get("steps"), list):
        raise ValueError(f"Workflow converter produced invalid JSON for {file_id}.")
    with open(target_json_file_path, 'w', encoding='utf-8') as f:
        json.dump(data, f, indent=4, ensure_ascii=False)
    logger.info(f"Converted workflow {file_id} to {target_json_file_path}")
    return target_json_file_path


# The env path is defined in config file
# The scheduler config path is defined in config file
def execute_code_to_scheduler(
    file_ids: list[str],
    lib_path: str,
    times: int,
    parallel_batch: bool = False,
) -> dict:
    if not file_ids:
        raise ValueError("At least one workflow Code ID is required.")

    workflow_files = [
        _convert_code_to_scheduler_file(file_id, lib_path)
        for file_id in file_ids
    ]
    repeat_count = int(times)
    if repeat_count < 1:
        raise ValueError("Workflow repeat count must be positive.")

    if parallel_batch:
        messages = [{
            "command": "new_workflow",
            "workflow_names": list(file_ids),
            "times": repeat_count,
            "uninterruptible": not settings.interruptible,
        }]
    else:
        messages = [
            {
                "command": "new_workflow",
                "workflow_name": file_id,
                "times": repeat_count,
                "uninterruptible": not settings.interruptible,
            }
            for file_id in file_ids
        ]

    scheduler_client = WebSocketClient.get_instance()
    for message_data in messages:
        message = json.dumps(message_data)
        scheduler_client.send_message(message)
        logger.info(f"Sent message to scheduler: {message}")

    return {
        "workflow_names": list(file_ids),
        "workflow_files": workflow_files,
        "messages": messages,
    }

def judge_info_enough(current_return: str):
    user_prompt = get_current_user_prompt()
    prompt_judge = PromptTemplate.from_template(
        """
        You should judge if the return of current execution is enough to fulfill the goal in the instruction.
        The instruction is:
        {user_prompt}
        Current metrics:
        {current_return} for all in the execution have been thoroughly and correctly recorded.
        Output "Good"(without quotes) unless a piece of information very critical for the objective is missing. Any information that can be derived or deduced from the provided metrics is not considered critical.
        If it's the case, output a key word or a key phrase that indicates the additional metric needed from the result.
        """
    )
    final_prompt = prompt_judge.format(user_prompt=user_prompt, current_return=current_return)
    # for debug, save the prompt to a file
    with open(os.path.join(output_path, "judger_input.md"), "w", encoding="utf-8") as f:
        f.write(final_prompt)
    res = code_model.invoke(final_prompt).content
    logger.info(f"Return of the judge_info_enough: {res}")
    return res

def get_input_set():
    basic_input = "Time"
    current_input = basic_input
    while True:
        res = judge_info_enough(current_input)
        if "Good" in res:
            break
        current_input = current_input + f",{res}"
    return current_input

def get_input(metric: str):
    res = input(f"Please enter the {metric} of the experiment: ")
    return res

def choose_best_result_by_llm(results: dict, optimizing_num: int) -> int:
    """
    Uses an LLM to determine the best result from a set of experiments.

    Args:
        results (dict): A dictionary where keys are indices (0 to n-1) and
                        values are dictionaries of metrics for each experiment.
        optimizing_num (int): The number of experiments to compare.

    Returns:
        int: The index of the best result, or 0 as a fallback.
    """
    user_prompt = get_current_user_prompt()
    prompt_template = PromptTemplate.from_template(
        """
        Given the overall goal and a list of experimental results, identify the best one.
        The overall goal is:
        {user_prompt}

        Here are the {optimizing_num} experimental results:
        {results_str}

        Based on the overall goal, which result is the best?
        Please respond with only the index number of the best result.
        Your response must be a single integer between 0 and {max_index} (inclusive).
        Do not provide any explanation or other text.
        """
    )

    # Format the results into a string for the prompt
    results_str = ""
    for i in range(optimizing_num):
        results_str += f"Index {i}: {results[i]}\n"

    final_prompt = prompt_template.format(
        user_prompt=user_prompt,
        optimizing_num=optimizing_num,
        results_str=results_str,
        max_index=optimizing_num - 1
    )
    
    logger.info("Prompting LLM to choose the best result.")
    # for debug, save the prompt to a file
    with open(os.path.join(output_path, "best_index_judger_input.md"), "w", encoding="utf-8") as f:
        f.write(final_prompt)

    response = reasoner_model.invoke(final_prompt).content
    logger.info(f"LLM response for best index: '{response}'")

    try:
        best_index = int(response.strip())
        if 0 <= best_index < optimizing_num:
            return best_index
        else:
            logger.warning(f"LLM returned an out-of-range index: {best_index}. Defaulting to 0.")
            return 0
    except (ValueError, TypeError):
        logger.error(f"Failed to parse LLM response '{response}' as an integer. Defaulting to 0.")
        return 0

def is_optimizing_job() -> bool:
    """
    Uses an LLM to determine if the user's goal is an iteratively optimizing job.

    Returns:
        bool: True if it is an optimizing job, False otherwise.
    """
    user_prompt = get_current_user_prompt()
    prompt_template = PromptTemplate.from_template(
        """
        Analyze the user's instruction to determine if the goal is to iteratively optimize a result or process.
        An iteratively optimizing job involves repeating a process to find a better result, often by adjusting parameters. Examples include improving yield, reducing time, or maximizing a specific metric over several attempts.
        A non-optimizing job is typically a one-off task, like running a calculation once, retrieving information, or executing a fixed procedure without the goal of improvement through repetition.

        User's instruction:
        {user_prompt}

        Is this an iteratively optimizing job?
        Respond with only "yes" or "no".
        """
    )

    final_prompt = prompt_template.format(user_prompt=user_prompt)
    
    logger.info("Prompting LLM to judge if it is an optimizing job.")
    # for debug, save the prompt to a file
    with open(os.path.join(output_path, "is_optimizing_judger_input.md"), "w", encoding="utf-8") as f:
        f.write(final_prompt)

    response = reasoner_model.invoke(final_prompt).content.strip().lower()
    logger.info(f"LLM response for optimizing job check: '{response}'")

    return "yes" in response

def remove_mockup_metrics_data(metrics: list[str], local_run_result: str) -> str:
    """
    Removes mockup metrics data from the local run result.

    Args:
        metrics (list[str]): A list of metric names to look for.
        local_run_result (str): local run results.

    Returns:
        str: The cleaned local run result without mockup metrics data.
    """

    remove_prompt = PromptTemplate.from_template(
"""
Remove all of the data and results(including all the descriptions for them and all the conclusions derived from them) that are mocked-up related to the metrics: {metrics}. Keep all other parts as is. Your output must be a subset of the input. An empty output is allowed if all the content is related to the mocked-up metrics.
Below is the text to be processed:
{local_run_result}
"""
    )
    final_remove_prompt = remove_prompt.format(metrics=", ".join(metrics), local_run_result=local_run_result)
    # for debug, save the prompt to a file
    with open(os.path.join(output_path, "remove_mockup_metrics_input.md"), "w", encoding="utf-8") as f:
        f.write(final_remove_prompt)
    res = reasoner_model.invoke(final_remove_prompt).content
    return res

def extract_container_names(local_run_result: str) -> list[str]:
    """
    Extracts container names from the local run result using an LLM.

    Args:
        local_run_result (str): The execution result text.

    Returns:
        list[str]: A list of extracted container names.
    """
    prompt_template = intermediate_products_prompt
    
    final_prompt = prompt_template.format(local_run_result=local_run_result)
    
    # for debug, save the prompt to a file
    with open(os.path.join(output_path, "container_extraction_input.md"), "w", encoding="utf-8") as f:
        f.write(final_prompt)
        
    try:
        response = reasoner_model.invoke(final_prompt).content.strip()
        
        # Basic cleanup in case the model wraps code in markdown despite instructions
        if response.startswith("```json"):
            response = response[7:]
        if response.startswith("```"):
            response = response[3:]
        if response.endswith("```"):
            response = response[:-3]
            
        container_list = json.loads(response.strip())
        
        if isinstance(container_list, list):
            logger.info(f"Extracted container names: {container_list}")
            return container_list
        else:
            logger.warning("LLM response for container extraction was not a JSON list.")
            return []
            
    except Exception as e:
        logger.error(f"Failed to extract container names: {e}")
        return []

def extract_and_notify_product_info(result: str):
    """
    Extracts product information from the result using an LLM and broadcasts it via WebSocket.

    Args:
        result (str): The execution result text to analyze.
    """
    prompt_template = PromptTemplate.from_template(
        """
        Analyze the result and extract the name of the final product(s) and its relevant note.
        Output in json format:
        {{
        "name": "xxxxx",
        "notes":"yyyyy"
        }}
        Result:
        {result}
        """
    )
    final_prompt = prompt_template.format(result=result)
    
    # for debug, save the prompt to a file
    with open(os.path.join(output_path, "product_extraction_input.md"), "w", encoding="utf-8") as f:
        f.write(final_prompt)

    try:
        extraction_res = reasoner_model.invoke(final_prompt).content
        logger.info(f"LLM response for product extraction: {extraction_res}")
        
        # Sanitize and parse JSON
        cleaned_res = extraction_res.replace("```json", "").replace("```", "").strip()
        product_info = json.loads(cleaned_res)
        
        # Broadcast using asyncio.run
        asyncio.run(WebSocketClient().send_message("product_generated", product_info))
        
    except Exception as e:
        logger.warning(f"Failed to extract or broadcast product info: {e}")

@tool
def Hardware(file_ids: list[str], repeat_num: int, pure_software: bool = False):
    """Executes experimental Python code and retrieves the results.

    This agent runs a Python script for an experiment and returns the output. It is primarily used to determine the outcome of an experiment and to check if an optimization target has been achieved.

    Args:
        file_ids (list[str]): A list of file IDs for the Python scripts to be executed.
        repeat_num (int): The number of times the experiment is repeated.
        pure_software (bool, optional): A flag indicating if the task only involves software.

    Returns:
        str: A string containing the results from the code execution.
    """
    
    if pure_software:
        # directly execute the code and return
        result = execute_and_log_files(file_ids, executor_path)
        return str(result)

    try:
        resolved_mode, ordered_file_ids = _ordered_execution_mode(file_ids)
        execute_code_to_scheduler(
            ordered_file_ids,
            scheduler_path,
            repeat_num,
            parallel_batch=resolved_mode == "parallel_batch",
        )
    except Exception as exc:
        logger.error(f"Failed to prepare or submit scheduler workflows: {exc}")
        return f"Error: Failed to prepare or submit scheduler workflows: {exc}"
        
    result = {}
    metrics = []
    metric_history = file_manager.get_agent_data(AgentName.Hardware)
    
    if not metric_history:
        input_set = get_input_set()
        data = {'metrics': input_set}
        metrics = input_set.split(",")
        file_manager.record_agent_data(AgentName.Hardware, data)
    else:
        input_set = metric_history.get('metrics', "")
        if input_set:
            metrics = input_set.split(",")
        else:
            logger.warning("No metrics found in the Hardware agent data. Using default metrics.")
            metrics = ["Time"]

    logger.debug(f"Metrics for Hardware: {metrics}")
    optimizing_num = (
        1 if resolved_mode == "parallel_batch" else len(ordered_file_ids)
    )
    logger.info(f"The number of records is: {optimizing_num}")

    for i in range(int(optimizing_num)):
        optimizing_result = {}
        for metric in metrics:
            optimizing_result[metric] = get_input(metric)
        result[i] = optimizing_result

    best_index = 0
    procedure_id = None
    if optimizing_num > 1:
        # Let the LLM choose the best result based on the user's goal
        best_index = choose_best_result_by_llm(result, optimizing_num)
    
    if ordered_file_ids:
        # Determine procedure_id based on the best_index (which is 0 if optimizing_num <= 1)
        file_id = ordered_file_ids[best_index]
        procedure_id = file_manager.get_file_mapping(file_id)
        CoflowCache.update_best_protocol(procedure_id)

    if best_index != -1:
        SelfOptimizingCache.push_result(str(result[best_index]))

    if not settings.mock_mode:
        SelfOptimizingCache.clear_tmp_optimizing_history()
        SelfOptimizingCache.clear_tmp_result_history()

    # Now, execute the codes and get the local run results
    local_run_result = execute_and_log_files(ordered_file_ids, executor_path)
    # for debug, save the local_run_result to a file
    with open(os.path.join(output_path, "local_run_result.txt"), "w", encoding="utf-8") as f:
        f.write(local_run_result)
    results = SelfOptimizingCache.get_result_history()

    # --- New Logic: Extract container names ---
    extracted_containers = extract_container_names(local_run_result)
    insert_intermediate_reagents(extracted_containers)
    # ------------------------------------------

    corrected_local_run_result = remove_mockup_metrics_data(metrics, local_run_result)
    if not is_optimizing_job():
        results = results + os.linesep + corrected_local_run_result
    else:
        # For optimizing jobs, we report the procedure ID corresponding to the best outcome.
        if procedure_id is not None:
            results += f"\nThe procedure corresponding to the latest outcome has ID: {procedure_id}"
    
    return results