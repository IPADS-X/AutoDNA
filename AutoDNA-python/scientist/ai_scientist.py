from llm.my_react_agent import create_my_react_agent
from config import base_path, output_path, prompt_path, settings, input_path, current_output_file_path
from prompts.agents.Planner.prompt import EPA_guidance_prompt, EPA_storage_prompt, EPA_enzymatic_synthesis_prompt
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
import argparse
import os
import re
import json
from loguru import logger

FINAL_WRITE_FILE = os.path.join(input_path, "final_write_result.md")
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
    DEFAULT = 9

tools = [Literature, Reagent, Hardware, Code, Protocol, Hypothesis]


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
    if settings.rna or settings.storage or settings.test or settings.amplification or settings.write or settings.read or settings.polya:
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

    summarization_prompt = """
experiment_name: The goal of the experiment with a short description (no more than 10 words).
Generate the experiment_name parameter for the following experiment description(output only the parameter value):
------------------------------
"""
    
    full_prompt = summarization_prompt + initial_prompt
    
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
        plan_system_prompt = """You are a master planner for scientific experiments. Your task is to decompose a complex goal from a user prompt into a sequence of distinct experimental stages.

For each stage, you must identify a noun phrase 'name' starting with 'DNA' for the subtask and a 'user_requirement'. The 'user_requirement' must contain any specific data, parameters, or constraints from the initial prompt that apply ONLY to that stage. If a stage has no specific requirement, the value must be an empty string.

You must output in a JSON format like this:
[
    {
      "name": "Stage 1 Name",
      "user_requirement": "Specific parameter for stage 1."
    },
    {
      "name": "Stage 2 Name",
      "user_requirement": ""
    }
]
"""
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
        
    previous_stage_output_content = ""
    # todo: Is it better to have another param indicating this stage is the continuation of previous stage?
    if settings.read:
        with open(FINAL_WRITE_FILE, "r", encoding="utf-8") as f:
            previous_stage_output_content = f.read()

    # Execute each stage using the new structure.
    for i, stage_info in enumerate(stages):
        stage_name = stage_info.get("name", "Unnamed Stage")
        user_requirement = stage_info.get("user_requirement", "")
        current_stage_num = i + 1
        logger.info(f"🚀 Starting Stage {current_stage_num}/{len(stages)}: '{stage_name}'")

        last_stage_result_prompt = f"""
Result from Previous Stage:
{previous_stage_output_content}
---"""
        current_prompt = f"""
Current Stage Goal: {stage_name}
"""

        if previous_stage_output_content != "":
            current_prompt = last_stage_result_prompt + current_prompt
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

            stage_cache_filename = f"stage-{current_stage_num}_cache.md"
            file_manager.save_to_cache(stage_cache_filename, previous_stage_output_content)
            logger.info(f"Saved stage {current_stage_num} result to '{stage_cache_filename}'.")
        else:
            logger.warning(f"Stage {current_stage_num} did not produce a final message.")
            break

    logger.success("✅ All planned stages have been executed.")
    return previous_stage_output_content

# todo: add a type to toolset
def planner(user_prompt: str, system_prompt: str, toolset, current_stage: int):
    graph = create_my_react_agent(react_model, toolset, name=f"stage-{current_stage}", prompt=EPA_enzymatic_synthesis_prompt if settings.synthesis else system_prompt)
    inputs = {"messages": [("user", user_prompt)]}
    stream = graph.stream(inputs, stream_mode="values", config={'recursion_limit': 100})

    final_message = save_tool_messages_to_file(stream)
    logger.info(f"Final message for stage {current_stage}: {final_message}")
    return final_message

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

    main_routine()



if __name__ == "__main__":
    main()
