import shutil
from llm.my_react_agent import create_my_react_agent
from config import base_path, output_path, prompt_path, settings, input_path, current_output_file_path
from prompts.agents.Planner.prompt import *
from llm.model import react_model, plan_model
from agents.Literature import Literature
from agents.Code import Code
from agents.Protocol import Protocol
from agents.Reagent import Reagent
from agents.Hardware import Hardware
from enum import Enum
from langchain_core.messages import AIMessage, ToolMessage
from agents.Hypothesis import Hypothesis
from tools.file_manager import file_manager
from tools.utils import CoflowCache
import argparse
import os
import re
import json
from loguru import logger

FINAL_WRITE_FILE = os.path.join(input_path, "final_write_result.md")
FINAL_WRITE_SUMMARY = os.path.join(input_path, "final_write_summary.md")
TRUNCATE_LENGTH = 3000

class ExperimentType(Enum):
    SYNTHESIS = 0
    RPA = 1
    RNA = 2
    STORAGE = 3
    TEST = 4
    AMPLIFICATION = 5
    WRITE = 6
    READ = 7
    POLYA = 8
    DETECTION = 9
    DEFAULT = 10

tools = [Literature, Reagent, Hardware, Code, Protocol, Hypothesis]
stage_protocols = []

def get_experiment_type() -> ExperimentType:
    if settings.synthesis:
        return ExperimentType.SYNTHESIS
    if settings.rpa:
        return ExperimentType.RPA
    if settings.rna:
        return ExperimentType.RNA
    if settings.storage:
        return ExperimentType.STORAGE
    if settings.test:
        return ExperimentType.TEST
    if settings.amplification:
        return ExperimentType.AMPLIFICATION
    if settings.detection:
        return ExperimentType.DETECTION
    if settings.write:
        return ExperimentType.WRITE
    if settings.read:
        return ExperimentType.READ
    if settings.polya:
        return ExperimentType.POLYA
    return ExperimentType.DEFAULT

def choose_toolset():
    if settings.rpa:
        return [tool for tool in tools if tool not in [Literature, Hypothesis]]
    if settings.rna or settings.storage or settings.test or settings.amplification or settings.write or settings.read or settings.polya or settings.detection:
        return tools
    return tools

def choose_user_prompt(experiment_type: ExperimentType):
    if experiment_type == ExperimentType.SYNTHESIS:
        user_prompt = open(os.path.join(prompt_path, "user_prompt_full.md"), "r", encoding="utf-8").read()
        return user_prompt
    if experiment_type == ExperimentType.RPA:
        user_prompt = open(os.path.join(prompt_path, "user_prompt_rpa.md"), "r", encoding="utf-8").read()
        return user_prompt  
    if experiment_type == ExperimentType.RNA:
        user_prompt = open(os.path.join(prompt_path, "user_prompt_siRNA.md"), "r", encoding="utf-8").read()
        return user_prompt
    if experiment_type == ExperimentType.STORAGE:
        user_prompt = open(os.path.join(prompt_path, "user_prompt_storage.md"), "r", encoding="utf-8").read()
        return user_prompt
    if experiment_type == ExperimentType.AMPLIFICATION:
        user_prompt = open(os.path.join(prompt_path, "user_prompt_amplification.md"), "r", encoding="utf-8").read()
        return user_prompt
    if experiment_type == ExperimentType.DETECTION:
        user_prompt = open(os.path.join(prompt_path, "user_prompt_detection.md"), "r", encoding="utf-8").read()
        return user_prompt
    if experiment_type == ExperimentType.WRITE:
        user_prompt = open(os.path.join(prompt_path, "user_prompt_write.md"), "r", encoding="utf-8").read()
        return user_prompt
    if experiment_type == ExperimentType.READ:
        user_prompt = open(os.path.join(prompt_path, "user_prompt_read.md"), "r", encoding="utf-8").read()
        return user_prompt
    if experiment_type == ExperimentType.TEST:
        user_prompt = open(os.path.join(prompt_path, "user_prompt_test.md"), "r", encoding="utf-8").read()
        return user_prompt
    if experiment_type == ExperimentType.POLYA:
        user_prompt = open(os.path.join(prompt_path, "user_prompt_polyA.md"), "r", encoding="utf-8").read()
        return user_prompt
    return "Default experiment."

def judge_interruptibility(user_prompt: str) -> bool:
    """
    Judges if the user requirement implies the task must be uninterruptible.
    Default is True (interruptible). If user implies otherwise, returns False.
    """

    judger_prompt = judge_interruptibility_prompt
    full_prompt = judger_prompt + user_prompt
        
    response = plan_model.invoke(full_prompt)
    answer = response.content.strip().upper()
    
    # "YES" means it IS interruptible. "NO" means it is NOT interruptible.
    is_interruptible = "YES" in answer
        
    if is_interruptible:
        logger.info("Task judged as: Interruptible")
    else:
        logger.info("Task judged as: Uninterruptible")
        
    return is_interruptible

def choose_system_prompt(experiment_type: ExperimentType):
    if experiment_type == ExperimentType.STORAGE:
        return EPA_storage_prompt
    return EPA_guidance_prompt  

def _extract_final_result(raw_content: str) -> str:
    """
    Extracts the content following the '### final_result ###' marker.

    Args:
        raw_content: The full string output from a planner stage.

    Returns:
        The stripped content of the final result, or the original content
        if the marker is not found.
    """
    # Split the content by the marker, ignoring case and surrounding whitespace.
    # maxsplit=1 ensures it only splits on the first occurrence.
    parts = re.split(r'###\s*final_result\s*###', raw_content, maxsplit=1, flags=re.IGNORECASE)
    
    # If the marker was found, 'parts' will have two elements.
    if len(parts) > 1:
        return parts[1].strip()
    
    # Otherwise, return the original content as a fallback.
    return raw_content.strip()

def save_tool_messages_to_file(stream):
    tool_counts = {}
    final_message = None      
    for s in stream:
        # Check if 'messages' key exists and is not empty
        if s and "messages" in s and s["messages"]:
            messages = s["messages"]
            message = s["messages"][-1]
            # Check if the latest message is a ToolMessage
            if isinstance(message, ToolMessage):
                logger.debug(f"Saving ToolMessage (ID: {message.tool_call_id}) content to file...")
                # Append the content of the ToolMessage to the file
                output_file_path = os.path.join(base_path, "output", "latest.txt")
                if len(messages) > 1:
                    previous_message = messages[-2]
                    if isinstance(previous_message, AIMessage) and previous_message.tool_calls:
                        for tool_call in previous_message.tool_calls:
                            name = tool_call.get("name", "UnknownTool")
                            tool_counts[name] = tool_counts.get(name, 0) + 1
                            logger.debug(f"Count of Tool {name}: {tool_counts[name]}")
                            output_file_path = os.path.join(base_path, "output", f"lastest_{tool_counts[name]}.txt")
                with open(output_file_path, 'a', encoding='utf-8') as f:
                    # check if the type is str
                    if isinstance(message.content, str):
                        f.write(message.content)
                    f.write("\n---\n") # Add a separator between messages
            else:
                # Optional: Print other message types to console if you still want to see them
                if isinstance(message, AIMessage):
                    content = message.content
                    if content != "":
                        final_message = message
                        logger.info(f"Current AIMessage content: {content}")
    return final_message                            

def summarize_task(initial_prompt: str) -> str:
    """
    Summarizes the initial prompt into a short experiment name using an LLM.

    Args:
        initial_prompt: The initial user prompt describing the experiment.

    Returns:
        A short, descriptive name for the experiment.
    """
    logger.info("Summarizing the main task...")
    
    # Define cache path for stage 0
    stage0_cache_dir = os.path.join(output_path, "stage-0")
    os.makedirs(stage0_cache_dir, exist_ok=True)
    
    input_cache_file = os.path.join(stage0_cache_dir, "summarize_task_input.txt")
    output_cache_file = os.path.join(stage0_cache_dir, "summarize_task_output.txt")

    # Check if the summary is already cached
    if os.path.exists(output_cache_file):
        with open(output_cache_file, 'r', encoding='utf-8') as f:
            summary = f.read().strip()
        logger.success(f"Loaded cached task summary: '{summary}'")
        return summary
    
    full_prompt = plan_summarization_prompt + initial_prompt
    
    # Save the input prompt to the cache
    with open(input_cache_file, 'w', encoding='utf-8') as f:
        f.write(full_prompt)
    
    response = plan_model.invoke(full_prompt)
    summary = response.content.strip()
    
    # Save the output summary to the cache
    with open(output_cache_file, 'w', encoding='utf-8') as f:
        f.write(summary)
        
    logger.success(f"Task summarized as: '{summary}' and cached.")
    return summary

def judge_requirement_relevance(current_stage_name: str, prev_stage_name: str, prev_requirement: str, prev_result: str, current_stage_num: int) -> bool:
    """
    Judges if the user requirement from the previous stage is relevant to the current stage.
    """
    if not prev_requirement:
        return False

    logger.info(f"Judging relevance of previous requirement for Stage {current_stage_num}...")

    # Cache setup
    stage_cache_dir = os.path.join(output_path, f"stage-{current_stage_num}")
    os.makedirs(stage_cache_dir, exist_ok=True)
    cache_file = os.path.join(stage_cache_dir, "judge_req_relevance_cache.json")

    # Check cache
    if os.path.exists(cache_file):
        try:
            with open(cache_file, 'r', encoding='utf-8') as f:
                data = json.load(f)
            if data.get('prev_stage_name') == prev_stage_name and data.get('current_stage_name') == current_stage_name:
                return data.get('is_relevant', False)
        except Exception:
            pass

    # Prompt
    prompt = f"""You are a scientific experiment planner. 
Determine if the specific user requirement from the previous stage should be carried over and applied to the current stage.

Previous Stage: {prev_stage_name}
Previous Requirement: "{prev_requirement}"
Previous Result Summary: {prev_result[:TRUNCATE_LENGTH]}...

Current Stage: {current_stage_name}

Does the Previous Requirement constrain or apply to the Current Stage? 
Output ONLY "YES" or "NO".
"""
    
    response = plan_model.invoke(prompt)
    answer = response.content.strip().upper()
    is_relevant = "YES" in answer

    # Save cache
    with open(cache_file, 'w', encoding='utf-8') as f:
        json.dump({
            'prev_stage_name': prev_stage_name, 
            'current_stage_name': current_stage_name,
            'is_relevant': is_relevant
        }, f)

    if is_relevant:
        logger.success(f"Previous requirement '{prev_requirement}' deemed RELEVANT to Stage {current_stage_num}.")
    else:
        logger.info(f"Previous requirement deemed NOT relevant.")
        
    return is_relevant

def judge_relevant_stages(current_stage_name: str, stage_history: list, current_stage_num: int, initial_user_goal: str) -> list:
    """
    Uses an LLM to judge which historical stage outputs are relevant for the current stage.

    Args:
        current_stage_name: The name of the current stage being executed.
        stage_history: List of dicts with keys 'stage_num', 'stage_name', 'output'.
        current_stage_num: The current stage number.
        initial_user_goal: The original user prompt before stage decomposition.

    Returns:
        List of stage numbers (integers) that are relevant to include.
    """
    if not stage_history:
        return []
    
    logger.info(f"Judging which historical stages are relevant for: '{current_stage_name}'")
    
    # Check cache first
    stage_cache_dir = os.path.join(output_path, f"stage-{current_stage_num}")
    os.makedirs(stage_cache_dir, exist_ok=True)
    cache_file = os.path.join(stage_cache_dir, "judge_relevant_stages_cache.json")
    
    if os.path.exists(cache_file):
        try:
            with open(cache_file, 'r', encoding='utf-8') as f:
                cache_data = json.load(f)
            # Check if stage_name matches
            if cache_data.get('stage_name') == current_stage_name:
                relevant_stages = cache_data.get('relevant_stages', [])
                logger.success(f"Loaded cached relevant stages: {relevant_stages}")
                return relevant_stages
        except (json.JSONDecodeError, IOError) as e:
            logger.warning(f"Failed to load cache: {e}. Regenerating judgment.")
    
    # Build a summary of available stages
    stage_summary = "\n".join([
        f"Stage {s['stage_num']}: {s['stage_name']}"
        for s in stage_history
    ])
    
    judger_prompt = f"""You are a scientific experiment planner. Given the overall experiment goal, the current stage, and a list of previous stages, determine which previous stages have outputs that are relevant and should be included as context for the current stage.

Overall experiment goal:
{initial_user_goal}

Previous stages:
{stage_summary}

Current stage: Stage {current_stage_num}: {current_stage_name}

Output ONLY a JSON list of stage numbers (integers) that are relevant. For example: [1, 3] or []. If no previous stages are relevant, output [].
Consider:
- Direct dependencies (e.g., if current stage needs results from a specific previous stage)
- Indirect dependencies (e.g., if current stage builds upon methods from earlier stages)
- include stages that provide essential context
"""
    
    # save the prompt
    with open(os.path.join(stage_cache_dir, "judge_relevant_stages_prompt.txt"), 'w', encoding='utf-8') as f:
        f.write(judger_prompt)
    response = plan_model.invoke(judger_prompt)
    result_str = response.content.strip()
    
    try:
        # Clean up the response
        result_str = result_str.replace("```json", "").replace("```", "").strip()
        relevant_stages = json.loads(result_str)
        
        # Validate the result
        if not isinstance(relevant_stages, list):
            logger.warning(f"LLM returned non-list result: {result_str}. Defaulting to previous stage only.")
            return [current_stage_num - 1] if current_stage_num > 1 else []
        
        # Filter out invalid stage numbers
        valid_stages = [s for s in relevant_stages if isinstance(s, int) and 1 <= s < current_stage_num]
        
        # Cache the result
        cache_data = {
            'stage_name': current_stage_name,
            'relevant_stages': valid_stages
        }
        with open(cache_file, 'w', encoding='utf-8') as f:
            json.dump(cache_data, f, indent=4)
        
        logger.success(f"Judged relevant stages: {valid_stages} and cached.")
        return valid_stages
        
    except (json.JSONDecodeError, ValueError) as e:
        logger.warning(f"Failed to parse LLM response: {result_str}. Error: {e}. Defaulting to previous stage only.")
        return [current_stage_num - 1] if current_stage_num > 1 else []

def judge_task_complexity(experiment_name: str) -> str:
    """
    Judges if a task is simple or complex based on its name.

    Args:
        experiment_name: A short description of the experiment.

    Returns:
        'simple' or 'complex'.
    """
    logger.info(f"Judging complexity for: '{experiment_name}'")

    stage0_cache_dir = os.path.join(output_path, "stage-0")
    
    input_cache_file = os.path.join(stage0_cache_dir, "judge_task_complexity_input.txt")
    output_cache_file = os.path.join(stage0_cache_dir, "judge_task_complexity_output.txt")

    if os.path.exists(output_cache_file):
        with open(output_cache_file, 'r', encoding='utf-8') as f:
            complexity = f.read().strip()
        logger.success(f"Loaded cached complexity judgement: '{complexity}'")
        return complexity

    judger_prompt = """You are a judger that judges whether an experiment is simple or complex.
If an experiment composing of very different sub-experiments instead of interative loops, then it is a complex one. Otherwise, you should deem it as a simple one. Output only "simple" or "complex"(without quotes).
----------------------------------------------
The experiment:
"""
    
    full_prompt = judger_prompt + experiment_name
    
    with open(input_cache_file, 'w', encoding='utf-8') as f:
        f.write(full_prompt)
        
    response = plan_model.invoke(full_prompt)
    complexity = response.content.strip()
    
    with open(output_cache_file, 'w', encoding='utf-8') as f:
        f.write(complexity)
        
    logger.success(f"Judged task complexity as: '{complexity}' and cached.")
    return complexity

def judge_agent_output_validity(agent_output: str) -> bool:
    """
    find out if "final_result" is in the agent_output
    """
    pattern = r'###\s*final_result\s*'
    match = re.search(pattern, agent_output, re.IGNORECASE)
    return match is not None

def judge_experiment_success(final_result: str, initial_user_goal: str) -> bool:
    """
    Judges if the experiment result indicates success based on the final output.

    Args:
        final_result: The final output from all stages.
        initial_user_goal: The original user prompt describing the experiment.

    Returns:
        True if the experiment is successful, False otherwise.
    """
    logger.info("Judging if the experiment is successful...")

    judger_prompt = f"""You are a scientific experiment evaluator. Based on the experiment final result, determine if the experiment was successful.

Final Result:
{final_result}...

Criteria for success:
- No critical errors or failures were reported
- The output contains meaningful results or conclusions

Output ONLY "YES" if the experiment is successful, or "NO" if it is not successful.
"""

    response = plan_model.invoke(judger_prompt)
    answer = response.content.strip().upper()
    is_successful = "YES" in answer

    if is_successful:
        logger.success("Experiment judged as: SUCCESSFUL")
    else:
        logger.warning("Experiment judged as: NOT SUCCESSFUL")

    return is_successful

def analyze_failure_and_get_retry_stage(stage_history: list, initial_user_goal: str) -> int:
    """
    Analyzes why the experiment failed and determines which stage to retry from.

    Args:
        stage_history: List of dicts with keys 'stage_num', 'stage_name', 'output'.
        initial_user_goal: The original user prompt describing the experiment.

    Returns:
        The stage number to retry from (1-indexed), or -1 if retry is not recommended.
    """
    logger.info("Analyzing experiment failure to determine retry stage...")

    # Combine all stage outputs
    combined_outputs = ""
    for stage_data in stage_history:
        combined_outputs += f"""
--- Stage {stage_data['stage_num']}: {stage_data['stage_name']} ---
{stage_data['output']}
"""

    analysis_prompt = f"""You are a scientific experiment troubleshooter. The following experiment did not succeed. Analyze the outputs from each stage to determine:
1. Why the experiment was not successful
2. Which stage should be retried to fix the issue

Experiment Goal:
{initial_user_goal}

Stage Outputs:
{combined_outputs}

Analyze the failure and determine the earliest stage where the issue originated or where a retry would most likely fix the problem.

Output your analysis in the following format:
```json
{{
    "failure_reason": "Brief explanation of why the experiment failed",
    "retry_stage": <stage_number_to_retry_from>
}}
```

The retry_stage should be an integer representing the stage number (1-indexed). If the failure cannot be fixed by retrying, output retry_stage as -1.
"""

    # Save the analysis prompt for debugging
    analysis_prompt_path = os.path.join(output_path, "failure_analysis_prompt.txt")
    with open(analysis_prompt_path, 'w', encoding='utf-8') as f:
        f.write(analysis_prompt)

    response = plan_model.invoke(analysis_prompt)
    result_str = response.content.strip()

    # Save the analysis result for debugging
    analysis_result_path = os.path.join(output_path, "failure_analysis_result.txt")
    with open(analysis_result_path, 'w', encoding='utf-8') as f:
        f.write(result_str)

    try:
        # Clean up the response
        result_str = result_str.replace("```json", "").replace("```", "").strip()
        analysis_result = json.loads(result_str)

        failure_reason = analysis_result.get("failure_reason", "Unknown reason")
        retry_stage = analysis_result.get("retry_stage", -1)

        logger.info(f"Failure analysis: {failure_reason}")
        logger.info(f"Recommended retry stage: {retry_stage}")

        return int(retry_stage)

    except (json.JSONDecodeError, ValueError, KeyError) as e:
        logger.warning(f"Failed to parse failure analysis response: {e}. Defaulting to retry from stage 1.")
        return 1

def clear_stage_caches_from(start_stage: int, total_stages: int):
    """
    Clears cached stage results from start_stage onwards to allow retry.

    Args:
        start_stage: The stage number to start clearing from (1-indexed).
        total_stages: The total number of stages.
    """
    logger.info(f"Clearing stage caches from stage {start_stage} to {total_stages}...")
    for stage_num in range(start_stage, total_stages + 1):
        stage_cache_filename = f"stage-{stage_num}_cache.md"
        cache_path = os.path.join(output_path, "coflow_cache", stage_cache_filename)
        if os.path.exists(cache_path):
            os.remove(cache_path)
            logger.info(f"Removed cache file: {stage_cache_filename}")
        
        # Also clear the stage output folder
        stage_folder = os.path.join(output_path, f"stage-{stage_num}")
        if os.path.exists(stage_folder):
            shutil.rmtree(stage_folder, ignore_errors=True)
            logger.info(f"Cleared stage folder: stage-{stage_num}")

# todo: add a type to toolset
def planner(user_prompt: str, system_prompt: str, toolset, current_stage: int):
    max_retries = 1
    
    for attempt in range(max_retries):
        if attempt > 0:
            logger.info(f"🔄 Retry attempt {attempt + 1}/{max_retries} for Stage {current_stage}...")
            # clear the output folder 
            shutil.rmtree(os.path.join(output_path, f"stage-{current_stage}"), ignore_errors=True)
            os.makedirs(os.path.join(output_path, f"stage-{current_stage}"), exist_ok=True)

        graph = create_my_react_agent(react_model, toolset, name=f"stage-{current_stage}", prompt=EPA_enzymatic_synthesis_prompt if settings.synthesis else system_prompt)
        inputs = {"messages": [("user", user_prompt)]}
        
        # Create a fresh stream for this attempt
        stream = graph.stream(inputs, stream_mode="values", config={'recursion_limit': 100})

        final_message = save_tool_messages_to_file(stream)
        
        # Validation Logic
        if final_message and final_message.content:
            is_valid = judge_agent_output_validity(final_message.content)
            
            if is_valid:
                logger.debug(f"Best Protocol ID after Stage {current_stage}: {CoflowCache.get_best_protocol()}")
                stage_protocols.append(CoflowCache.get_best_protocol())
                logger.info(f"Final message for stage {current_stage}: {final_message}")
                return final_message
            else:
                logger.warning(f"❌ Stage {current_stage} output validation failed. Discarding result.")
                # The loop will continue, effectively "initializing and starting all over again"
                # because we re-create the graph and stream in the next iteration.
        else:
            logger.warning(f"❌ Stage {current_stage} returned no content. Retrying...")

    logger.error(f"Failed to get valid output for Stage {current_stage} after {max_retries} attempts.")
    return final_message

def planner_plan(initial_prompt: str, system_prompt: str, toolset: list):
    """
    Decomposes a task into structured stages, each with a name and a potential
    user requirement, then executes them sequentially.
    """
    experiment_name = summarize_task(initial_prompt)
    task_complexity = judge_task_complexity(experiment_name)

    # ### MODIFICATION START ###
    # Save the initial analysis to a JSON file as requested.
    assistant_content = (
        "\nI have analyzed the user's request and generated the following task analysis:\n"
        f"**Experiment name:** {experiment_name}\n"
        f"**Complexity:** {task_complexity}\n"
    )
    output_data = {
        "step0": [
            [
                [
                    {
                        "role": "user",
                        "content": initial_prompt
                    },
                    {
                        "role": "assistant",
                        "agent": "Planner Agent",
                        "content": assistant_content
                    }
                ]
            ],
            [
                "Planner Agent: Task Analysis"
            ]
        ]
    }
    with open(current_output_file_path, 'w', encoding='utf-8') as f:
        json.dump(output_data, f, indent=4)
    logger.info(f"Saved initial task analysis to: {current_output_file_path}")
    # ### MODIFICATION END ###

    if "simple" in task_complexity.lower():
        logger.info("Task judged as simple. Proceeding with single-stage execution.")
        final_message = planner(initial_prompt, system_prompt, toolset, current_stage=1)
        return final_message.content if final_message else "Execution failed."

    cache_file_name = "decompose_task_output.json"
    cache_file_path = os.path.join(output_path, "stage-0", cache_file_name)
    stages = None

    # Attempt to load the structured plan from the cache.
    if os.path.exists(cache_file_path):
        try:
            with open(cache_file_path, 'r', encoding='utf-8') as f:
                stages = json.load(f)
            logger.success(f"Loaded structured plan from cache with {len(stages)} stages.")
        except Exception:
            logger.warning("Could not load cache. Generating a new plan.")
            
    # If no cache was loaded, generate a new, structured plan.
    if stages is None:
        logger.info("Generating a new structured experimental plan...")

        plan_messages = plan_system_prompt + initial_prompt
        # for debug, save the plan_messages to a file
        with open(os.path.join(output_path,"decompose_task_input.txt"), 'w', encoding='utf-8') as f:
            f.write(plan_messages)
        response = plan_model.invoke(plan_messages)

        try:
            plan_json_str = response.content.strip().replace("```json", "").replace("```", "").strip()
            stages = json.loads(plan_json_str)
            logger.success(f"Structured plan generated with {len(stages)} stages.")
            
            with open(cache_file_path, 'w', encoding='utf-8') as f:
                json.dump(stages, f, indent=4)
            logger.info(f"Saved new structured plan to cache: {cache_file_path}")

        except (json.JSONDecodeError, KeyError) as e:
            logger.error(f"Failed to parse structured plan from model response: {e}")
            logger.warning("Falling back to single-stage execution.")
            final_message = planner(initial_prompt, system_prompt, toolset, current_stage=1)
            return final_message.content if final_message else "Execution failed."

    if not stages:
        logger.error("Could not generate or load a plan. Aborting.")
        return "Execution failed: No plan was available."
        
    # Track all stage outputs with metadata
    stage_history = []
    
    previous_stage_output_content = ""
    write_summary_content = ""
    # todo: Is it better to have another param indicating this stage is the continuation of previous stage?
    if settings.read:
        with open(FINAL_WRITE_FILE, "r", encoding="utf-8") as f:
            previous_stage_output_content = f.read()
            # Add the write stage output to history if in read mode
        with open(FINAL_WRITE_SUMMARY, "r", encoding="utf-8") as f: 
            write_summary_content = f.read()

            stage_history.append({
                'stage_num': 0,
                'stage_name': 'DNA Storage Write',
                'output': previous_stage_output_content
            })

    # Execute each stage using the new structure.
    for i, stage_info in enumerate(stages):
        stage_name = stage_info.get("name", "Unnamed Stage")
        user_requirement = stage_info.get("user_requirement", "")
        current_stage_num = i + 1
        logger.info(f"🚀 Starting Stage {current_stage_num}/{len(stages)}: '{stage_name}'")

        # Dynamic check for previous stage requirement inheritance
        if i > 0:
            prev_stage_info = stages[i-1]
            prev_req = prev_stage_info.get("user_requirement", "")
            
            # Use the output content from the loop iteration (which represents the previous stage result)
            should_inherit = judge_requirement_relevance(
                current_stage_name=stage_name,
                prev_stage_name=prev_stage_info.get("name", "Unknown"),
                prev_requirement=prev_req,
                prev_result=previous_stage_output_content,
                current_stage_num=current_stage_num
            )
            
            if should_inherit:
                user_requirement += f" {prev_req}"

        # Use LLM to judge which historical stages are relevant
        relevant_stage_nums = judge_relevant_stages(stage_name, stage_history, current_stage_num, initial_prompt)
        
        # Build context from relevant historical stages
        historical_context = ""
        if relevant_stage_nums:
            logger.info(f"Including outputs from stages: {relevant_stage_nums}")
            for stage_num in relevant_stage_nums:
                # Find the stage in history
                matching_stages = [s for s in stage_history if s['stage_num'] == stage_num]
                if matching_stages:
                    stage_data = matching_stages[0]
                    historical_context += f"""
Result from Stage {stage_data['stage_num']}: {stage_data['stage_name']}
{stage_data['output']}
---
"""
        
        current_prompt = f"""
Current Stage Goal: {stage_name}
"""
        if settings.read: # <--- MODIFICATION 3: Inject summary into prompt
            current_prompt = f"Write Stage Summary:{write_summary_content}" + current_prompt

        if historical_context:
            current_prompt = historical_context + current_prompt
        # If this stage has a specific requirement, inject it into the prompt.
        if user_requirement:
            logger.debug(f"Injecting requirement for this stage: '{user_requirement}'")
            requirement_injection = f"""
---
Requirement for this stage:
{user_requirement}
---
"""         # sa
            current_prompt += requirement_injection
        

        current_prompt += "\nPlease focus ONLY on executing this current stage."
        # save current user prompt
        file_manager.save_to_cache(settings.CURRENT_USER_PROMPT_FILE, current_prompt)
        # save current user requirement (even if it's "")
        file_manager.save_to_cache(settings.CURRENT_USER_REQUIREMENT_FILE, user_requirement)
        # check if cached
        stage_cache_filename = f"stage-{current_stage_num}_cache.md"
        cached_stage_result = file_manager.load_from_cache(stage_cache_filename)
        # if cached, process directly to next stage
        if cached_stage_result is not None:
            logger.info(f"Found cached result for stage {current_stage_num}. Using cached result.")
            previous_stage_output_content = cached_stage_result
            # Add cached result to history
            stage_history.append({
                'stage_num': current_stage_num,
                'stage_name': stage_name,
                'output': cached_stage_result
            })
            continue

        final_message_for_stage = planner(
            user_prompt=current_prompt,
            system_prompt=system_prompt,
            toolset=toolset,
            current_stage=current_stage_num
        )

        if final_message_for_stage and final_message_for_stage.content:
            raw_output = final_message_for_stage.content
            previous_stage_output_content = _extract_final_result(raw_output)

            # Add this stage's output to history
            stage_history.append({
                'stage_num': current_stage_num,
                'stage_name': stage_name,
                'output': previous_stage_output_content
            })

            stage_cache_filename = f"stage-{current_stage_num}_cache.md"
            file_manager.save_to_cache(stage_cache_filename, previous_stage_output_content)
            logger.info(f"Saved stage {current_stage_num} result to '{stage_cache_filename}'.")
        else:
            logger.warning(f"Stage {current_stage_num} did not produce a final message.")
            break

    logger.info("All planned stages have been executed.")

    # Judge if the experiment is successful
    max_retry_attempts = 2  # Maximum number of retry attempts
    retry_attempt = 0

    while retry_attempt < max_retry_attempts:
        is_successful = judge_experiment_success(previous_stage_output_content, initial_prompt)
        
        if is_successful:
            logger.success("🎉 Experiment completed successfully!")
            break
        else:
            retry_attempt += 1
            logger.warning(f"❌ Experiment not successful. Retry attempt {retry_attempt}/{max_retry_attempts}")
            
            if retry_attempt >= max_retry_attempts:
                logger.error("Maximum retry attempts reached. Proceeding with current result.")
                break
            
            # Analyze failure and get retry stage
            retry_stage = analyze_failure_and_get_retry_stage(stage_history, initial_prompt)
            
            if retry_stage < 1 or retry_stage > len(stages):
                logger.warning(f"Invalid retry stage {retry_stage}. Skipping retry.")
                break
            
            logger.info(f"🔄 Retrying from Stage {retry_stage}...")
            
            # Clear caches from retry_stage onwards
            clear_stage_caches_from(retry_stage, len(stages))
            
            # Remove stage history from retry_stage onwards
            stage_history = [s for s in stage_history if s['stage_num'] < retry_stage]
            
            # Re-execute stages from retry_stage
            for i in range(retry_stage - 1, len(stages)):
                stage_info = stages[i]
                stage_name = stage_info.get("name", "Unnamed Stage")
                user_requirement = stage_info.get("user_requirement", "")
                current_stage_num = i + 1
                logger.info(f"🚀 Retrying Stage {current_stage_num}/{len(stages)}: '{stage_name}'")

                # Dynamic check for previous stage requirement inheritance
                if i > 0:
                    prev_stage_info = stages[i-1]
                    prev_req = prev_stage_info.get("user_requirement", "")
                    
                    should_inherit = judge_requirement_relevance(
                        current_stage_name=stage_name,
                        prev_stage_name=prev_stage_info.get("name", "Unknown"),
                        prev_requirement=prev_req,
                        prev_result=previous_stage_output_content,
                        current_stage_num=current_stage_num
                    )
                    
                    if should_inherit:
                        user_requirement += f" {prev_req}"

                # Use LLM to judge which historical stages are relevant
                relevant_stage_nums = judge_relevant_stages(stage_name, stage_history, current_stage_num, initial_prompt)
                
                # Build context from relevant historical stages
                historical_context = ""
                if relevant_stage_nums:
                    logger.info(f"Including outputs from stages: {relevant_stage_nums}")
                    for stage_num in relevant_stage_nums:
                        matching_stages = [s for s in stage_history if s['stage_num'] == stage_num]
                        if matching_stages:
                            stage_data = matching_stages[0]
                            historical_context += f"""Result from Stage {stage_data['stage_num']}: {stage_data['stage_name']}
{stage_data['output']}
---
"""
                
                current_prompt = f"""Current Stage Goal: {stage_name}
"""
                if settings.read:
                    current_prompt = f"Write Stage Summary:{write_summary_content}" + current_prompt

                if historical_context:
                    current_prompt = historical_context + current_prompt
                
                if user_requirement:
                    logger.debug(f"Injecting requirement for this stage: '{user_requirement}'")
                    requirement_injection = f"""
---
Requirement for this stage:
{user_requirement}
---
"""
                    current_prompt += requirement_injection
                
                current_prompt += "\nPlease focus ONLY on executing this current stage."
                file_manager.save_to_cache(settings.CURRENT_USER_PROMPT_FILE, current_prompt)
                file_manager.save_to_cache(settings.CURRENT_USER_REQUIREMENT_FILE, user_requirement)

                final_message_for_stage = planner(
                    user_prompt=current_prompt,
                    system_prompt=system_prompt,
                    toolset=toolset,
                    current_stage=current_stage_num
                )

                if final_message_for_stage and final_message_for_stage.content:
                    raw_output = final_message_for_stage.content
                    previous_stage_output_content = _extract_final_result(raw_output)

                    stage_history.append({
                        'stage_num': current_stage_num,
                        'stage_name': stage_name,
                        'output': previous_stage_output_content
                    })

                    stage_cache_filename = f"stage-{current_stage_num}_cache.md"
                    file_manager.save_to_cache(stage_cache_filename, previous_stage_output_content)
                    logger.info(f"Saved stage {current_stage_num} result to '{stage_cache_filename}'.")
                else:
                    logger.warning(f"Stage {current_stage_num} did not produce a final message.")
                    break
            
            logger.success("✅ Retry stages have been executed.")

    complete_routine()

    return previous_stage_output_content

# todo: add a type to toolset
# def planner(user_prompt: str, system_prompt: str, toolset, current_stage: int):
#     graph = create_my_react_agent(react_model, toolset, name=f"stage-{current_stage}", prompt=EPA_enzymatic_synthesis_prompt if settings.synthesis else system_prompt)
#     inputs = {"messages": [("user", user_prompt)]}
#     stream = graph.stream(inputs, stream_mode="values", config={'recursion_limit': 100})

#     final_message = save_tool_messages_to_file(stream)
#     logger.info(f"Final message for stage {current_stage}: {final_message}")
#     return final_message

def complete_routine():
    # Get all the protocols from each stage with their protocol-id
    logger.info(f"Retrieving all protocols from each stage, the protocol IDs are: {stage_protocols}")
    protocol_contents = ""
    for i in range(len(stage_protocols)):
        protocol_id = stage_protocols[i]
        content = file_manager.get_file_content(protocol_id)
        if content:
            protocol_contents += content + "\n"

    summary = all_protocols_summary_prompt.format(protocol_contents=protocol_contents)
    # invoke the LLM to summarize
    response = plan_model.invoke(summary)

    if settings.write:
        final_summary_path = FINAL_WRITE_SUMMARY
        with open(final_summary_path, "w", encoding="utf-8") as f:
            f.write(response.content)
    
    # For now, Other summaries are not saved to files

def main_routine():
    experiment_type = get_experiment_type()
    toolset = choose_toolset()
    system_prompt = choose_system_prompt(experiment_type)
    # graph = create_my_react_agent(react_model, toolset, name="ai_scientist", prompt=system_prompt)

    user_prompt = choose_user_prompt(experiment_type)
    output_file_path = os.path.join(output_path, "latest.txt")
    os.makedirs(os.path.dirname(output_file_path), exist_ok=True)

    logger.info("📝 Conducting planning First.")
    final_result = planner_plan(user_prompt, system_prompt, toolset)
    logger.info(f"Final result after all stages: {final_result}")


def main():
    parser = argparse.ArgumentParser(description="Multi-Agent System with Mock Mode")
    parser.add_argument(
        '-m', '--mock_mode',
        action='store_true',  # Makes it a flag, e.g., presence means True
        help="Run the system in mock mode for debugging."
    )
    parser.add_argument(
        '--no_filtering',
        action='store_true',
        help="Disable Protocol input filtering."
    )

    parser.add_argument(
        '--synthesis',
        action='store_true',
        help="For synthesis experiments."
    )
    parser.add_argument(
        '--rpa',
        action='store_true',
        help="For RPA experiments, without Literature and Hypothesis."
    )
    parser.add_argument(
        '--rna',
        action='store_true',
        help="For RNA experiments, without Hypothesis."
    )
    parser.add_argument(
        '--storage',
        action='store_true',
        help="For DNA storage experiments."
    )
    parser.add_argument(
        '--test',
        action='store_true',
        help="For testing purposes."
    )
    parser.add_argument(
        '--amplification',
        action='store_true',
        help="For DNA amplification experiments."
    )
    parser.add_argument(
        '--detection',
        action='store_true',
        help="For DNA detection experiments."
    )
    parser.add_argument(
        '--write',
        action='store_true',
        help="For DNA storage write experiments."
    )
    parser.add_argument(
        '--read',
        action='store_true',
        help="For DNA storage read experiments."
    )
    parser.add_argument(
        '--polya',
        action='store_true',
        help="For PolyA tailing experiments."
    )

    parser.add_argument(
        '--variants',
        action='store_true',
        help="Enable variants mode."
    )
    parser.add_argument(
        '--questions',
        action='store_true',
        help="Enable questions mode for Literature."
    )
    parser.add_argument(
        '--chat',
        type=str,
        default='gemini',
        help="Specify the chat model series (default: gemini)."
    )

    args = parser.parse_args()

    if args.no_filtering:
        logger.info("🚫 Protocol input filtering is disabled.")
        settings.no_filtering = True

    if args.synthesis:
        logger.info("🧬 Running in SYNTHESIS mode.")
        settings.synthesis = True
    if args.rpa:
        logger.info("🤖 Running in RPA mode, without Literature and Hypothesis.")
        settings.rpa = True
    if args.rna:
        logger.info("🧬 Running in RNA mode, without Hypothesis.")
        settings.rna = True
    if args.storage:
        logger.info("💾 Running in DNA storage mode.")
        settings.storage = True
    if args.test:
        logger.info("🧪 Running in TEST mode.")
        settings.test = True
    if args.amplification:
        logger.info("🔬 Running in DNA amplification mode.")
        settings.amplification = True
    if args.detection:
        logger.info("🧪 Running in DNA detection mode.")
        settings.detection = True
    if args.write:
        logger.info("✍️ Running in DNA storage WRITE mode.")
        settings.write = True
    if args.read:
        logger.info("📖 Running in DNA storage READ mode.")
        settings.read = True
    
    if args.polya:
        logger.info("🧬 Running in PolyA tailing mode.")
        settings.polya = True

    if args.mock_mode:
        logger.info("🚀 Running in MOCK mode.")
        settings.mock_mode = True
    else:
        logger.info("⚡️ Running in LIVE mode.")

    if args.variants:
        logger.info("🔀 Enabling variants mode.")
        settings.variants = True

    if args.chat:
        if args.chat not in ["gemini", "qwen"]:
            logger.warning(f"❌ Invalid chat model series: '{args.chat}'. Use default.")
        else:
            logger.info(f"💬 Using chat model series: {args.chat}")
            settings.chat_series = args.chat

    main_routine()



if __name__ == "__main__":
    main()
