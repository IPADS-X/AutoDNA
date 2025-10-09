from langchain.tools import tool
from llm.model import model, reasoner_model, workflow_model
from langchain.prompts import PromptTemplate
from config import output_path, settings, input_manual_mappings_path, manual_path, protocol_mode
from prompts.agents.Protocol.prompt import *
import os
from agents.constants import AgentOutputPrefix, AgentName
from loguru import logger
import re
import json
from enum import Enum
from tools.utils import extract_text_from_pdfs, get_current_user_prompt, extract_all_from_pdfs, get_inventory, format_reagents_json, get_current_user_requirement
from tools.file_manager import file_manager

from tools.utils import WorkflowCache, getLatestExperimentWorkflowByCache


class WorkflowType(Enum):
    INITIAL = 0
    REFINEMENT = 1
    ADJUSTMENT = 2
    OPTIMIZING = 3
    ALGORITHM = 4
    ERROR = 5

def choose_mode_by_LLM(file_ids: list[str], manual_ids: list[str]) -> WorkflowType:
    # if not file_ids and not manual_ids:
    #     return WorkflowType.ERROR
    
    rules = f"""
    0. The output of each agent has a corresponding prefix in the file ID. 
    {AgentName.Literature} output has prefix {AgentOutputPrefix.LRA_OUTPUT}, {AgentName.Protocol} output has prefix {AgentOutputPrefix.PGA_OUTPUT}, {AgentName.Reagent} output has prefix {AgentOutputPrefix.RMA_OUTPUT}, {AgentName.Hypothesis} output has prefix {AgentOutputPrefix.HPA_OUTPUT}, {AgentName.Hardware} output has prefix {AgentOutputPrefix.HEVA_OUTPUT}.
    1. If there is no previous {AgentOutputPrefix.PGA_OUTPUT} output in the file IDs, then it's the initial procedure generation. Output "Initial"(without quotes). Otherwise, continue to the next step.
    2. If there is a new manual, then it's the refinement mode. Output "Refinement"(without quotes). Otherwise, continue to the next step.
    3. If there is {AgentOutputPrefix.HPA_OUTPUT} in the file IDs or in the history input IDs, then it's the optimizing mode. Output "Optimizing"(without quotes). Otherwise, continue to the next step.
    4. If there is {AgentOutputPrefix.RMA_OUTPUT} in the file IDs, then it's the adjustment mode. Output "Adjustment"(without quotes). Otherwise, continue to the next step.
    5. It's the error mode. Output "Error"(without quotes).
    """

    file_ids_str = ', '.join(file_ids)
    new_manual_ids = filter_ids(manual_ids, True)
    new_manual_ids_str = ', '.join(new_manual_ids)
    history_input_ids = file_manager.get_agent_input_history(AgentName.Protocol)
    history_input_ids_str = ', '.join(history_input_ids)
    choice_template = PromptTemplate.from_template(
    """
    Determine the mode of the Protocol agent based on the provided file IDs and manual IDs. Output only one word.
    You must follow the rules below:
    {rules}
    The provided file IDs are: {file_ids}
    The provided new manual IDs are: {manual_ids}
    The provided history input IDs are: {history_input_ids}
    """
    )
    final_prompt = choice_template.format(
        rules=rules,
        file_ids=file_ids_str,
        manual_ids=new_manual_ids_str,
        history_input_ids=history_input_ids_str
    )
    res = workflow_model.invoke(final_prompt).content.strip()
    logger.debug(f"the input is {final_prompt}")
    logger.info(f"the response is {res}")
    if res == "Initial":
        return WorkflowType.INITIAL
    if res == "Refinement":
        return WorkflowType.REFINEMENT
    if res == "Optimizing":
        return WorkflowType.OPTIMIZING
    if res == "Adjustment":
        return WorkflowType.ADJUSTMENT
    return WorkflowType.ERROR

def filter_ids(ids: list[str], is_compulsory: bool = False) -> list[str]:
    if settings.no_filtering and not is_compulsory:
        # if no_filtering tag is set, return as is
        return ids
    # get past ids from file manager
    past_ids = file_manager.get_agent_input_history(AgentName.Protocol)
    # filter the ids that are not in the past_ids
    filtered_ids = []
    for id in ids:
        if id not in past_ids:
            filtered_ids.append(id)
    return filtered_ids

    


@tool
def Protocol(experiment_name: str, pure_software = False, retry = False, file_ids: list[str] = [], manuals: list[str] = []):
    """Generates, adjusts, modifies, validates an experimental procedure or develops an algorithm based on provided information and user instructions. 

    This agent has four primary uses depending on the stage of the experiment:
    1.  Initial Procedure Generation: To create a new procedure from scratch.
    2.  Procedure Refinement: To adjust an existing procedure based on new information or feedback. Include previous procedure in the input for refinement.
    3.  Procedure Validation/Filtering: To validate a previous procedure against available materials. Include previous procedure in the input for validation/filtering.
    4.  Procedure Modification: To modify an existing procedure based on optimization hypothesis. Include previous procedure in the input for modification.

    Args:
        experiment_name (str): The name or high-level goal of the experiment.
        pure_software (bool, optional): A flag indicating if the task only involves software.
        retry (bool, optional): If too many parts of the previous procedure fail, set it to True to regenerate a possible procedure based on new resources.
        file_ids (list[str], optional): An optional list of file IDs containing relevant documents like previous procedure, experimental information, reagent check information, etc.
        manuals (list[str], optional): An optional list of file IDs for instrument or reagent manuals to be incorporated into the procedure.

    Returns:
        str: A formatted string containing the generated or modified experimental procedure.

    """



    model = workflow_model
    text = ""
    res = "default response" 
    user_prompt = get_current_user_prompt()


    prompt0 = PromptTemplate.from_template(
"""
You are a helpful assistant that can help generate experiment procedures. Your goal is to develop a structured experiment process for {experiment_name}.
"""
+ f"\n{PGA_prompt_option_mode0}" + f"\n{PGA_prompt_intermediate_reagents}"  
+ f"\n{workflow_solution_preparation_guidelines2}" +
"""
The information about the experiment is (may contain redundant even inaccurate information):
{text}
--------------------------------
"""
+ "Critical Note: The procedure you design must be for {experiment_name}."
    )

    prompt1 = PromptTemplate.from_template(
"""
You are a helpful assistant that can help filter out invalid options from an experiment procedure. 
"""
+ f"\n{PGA_prompt_filter}" 
"""
The information about the experiment is:
{text}
Keep all the reaction conditions. Modify the volume information according to reagent notes if any.
If too many parts fail, just output "Too Many parts failed" only.
"""
    )

    prompt2 = PromptTemplate.from_template(
"""
You are an optimization assistant that can help modify experiment procedure based on the given optimizing advice about {experiment_name}.
"""
+ f"\n{PGA_prompt_optimize1}" + f"\n{PGA_prompt_optimize_new_reagent}"
"""
The original procedure is:
{procedure}
Reaction condition like temperature, repeat times should be included in the action.
"""
+ "The optimizing advice is: {advice}"
    )

    prompt_addition = PromptTemplate.from_template(
"""
You are a helpful assistant that can help modify experiment procedure for {experiment_name}.
"""
+ f"\n{PGA_prompt_option_mode1}" +  f"\n{workflow_additional_instructions_BW}" 
+ f"\n{workflow_general_guidelines}"
"""
The information about the experiment is:
{text}
"""
    )

    prompt_filter = PromptTemplate.from_template(
"""
You are a helpful assistant that can help filter experiment procedure for {experiment_name}.
"""
+ f"\n{adjust_workflow_filtering_prompt_new}" + f"\n{PGA_prompt_do_not_prepare_existing_material}"
"""
The information about the experiment is:
{text}
"""
+ """ If too many parts fail, just output "Too Many parts failed" only. 
Otherwise, You must generate a reagent check list for and ONLY for those reagents that are not queried. 
""" + f"\n{PGA_prompt_intermediate_reagents}" 
    )
    # choose mode
    if pure_software:
        category = WorkflowType.ALGORITHM
    elif retry:
        category = WorkflowType.INITIAL
    else:
        category = choose_mode_by_LLM(file_ids, manuals)
    
    if category == WorkflowType.ALGORITHM:
        logger.info("WorkflowType.ALGORITHM")
        context = ""

        for file_id in file_ids:
            file_content = file_manager.get_file_content(file_id)
            if file_content:
                context = context + file_content

        final_prompt = prompt_generate_scheme.format(user_prompt=user_prompt, text=context)
        with open(os.path.join(output_path, 'answers','last_workflow_input.md'), 'w', encoding='utf-8') as f:
            f.write(final_prompt)
        res = model.invoke(final_prompt).content

    if category == WorkflowType.INITIAL:
        protocol_mode.mode = "Procedure Design"
        logger.info("WorkflowType.INITIAL")
        # clear previous agent input informations
        file_manager.clear_agent_inputs(agent_name=AgentName.Protocol)
        context = ""
        reagent_context = ""

        for file_id in file_ids:
            if AgentOutputPrefix.LRA_OUTPUT in file_id:
                file_content = file_manager.get_file_content(file_id)
                if file_content:
                    context = context + file_content
            if AgentOutputPrefix.RMA_OUTPUT in file_id:
                file_content = file_manager.get_file_content(file_id)
                if file_content:
                    reagent_context = reagent_context + file_content

        with open(os.path.join(input_manual_mappings_path , "manual_map.json"), "r") as f:
            kv = json.load(f)
        manual_paths = []
        for manual in manuals:
            if manual in kv:
                manual_paths.append(os.path.join(manual_path, kv[manual]))
        manual_text = extract_all_from_pdfs(manual_paths)
        if manual_text:
            context = context + "\n" + manual_text
            logger.debug(manual_text)

        if retry:
            logger.info("In retry mode")
            final_prompt = prompt_retry.format(user_prompt=user_prompt, reagents=reagent_context, manuals=manual_text if manual_text else "No additional information provided.")
        # todo: think if synthesis, purification, and storage can fit in this
        elif settings.rpa:
            final_prompt = prompt_generate_from_scratch.format(experiment_name=experiment_name, user_prompt=user_prompt, text=context)
        else:
            final_prompt = prompt0.format(experiment_name=experiment_name, text=context)
            # add specific requirement if there is any
            # requirement = get_current_user_requirement()
            # if requirement:
            #     final_prompt = final_prompt + prompt_additional_requirement.format(additional_requirement=requirement)

        with open(os.path.join(output_path, 'answers','last_workflow_input.md'), 'w', encoding='utf-8') as f:
            f.write(final_prompt)
        res = model.invoke(final_prompt).content


    if category == WorkflowType.REFINEMENT:
        protocol_mode.mode = "Procedure Refinement"
        logger.info("WorkflowType.REFINEMENT")
        file_ids = filter_ids(file_ids)

        def _mode2_stage1() -> str:
            context = ""
            for file_id in file_ids:
            # read the file content
                if AgentOutputPrefix.RMA_OUTPUT in file_id:
                    continue
                file_content = file_manager.get_file_content(file_id)
                if file_content:
                    context = context + file_content

            # Then read the manuals
            with open(os.path.join(input_manual_mappings_path , "manual_map.json"), "r") as f:
                kv = json.load(f)
            manual_paths = []
            for manual in manuals:
                if manual in kv:
                    manual_paths.append(os.path.join(manual_path, kv[manual]))
            manual_text = extract_all_from_pdfs(manual_paths)
            if manual_text:
                context = context + "\n" + manual_text
                logger.debug(manual_text)

            final_prompt = prompt_addition.format(experiment_name=experiment_name, text=context)
            with open(os.path.join(output_path, 'answers','last_workflow_input.md'), 'w', encoding='utf-8') as f:
                f.write(final_prompt)
            return model.invoke(final_prompt).content
        
        def _mode2_stage2(intermediate_result: str) -> str:
            # This is the second stage of the refinement workflow
            context = intermediate_result
            for file_id in file_ids:
                # read the file content
                if AgentOutputPrefix.RMA_OUTPUT in file_id:
                    file_content = file_manager.get_file_content(file_id)
                    if file_content:
                        context = context + os.linesep + file_content

            final_prompt = prompt_filter.format(experiment_name=experiment_name, text=context)
            with open(os.path.join(output_path, 'answers','last_workflow_input.md'), 'w', encoding='utf-8') as f:
                f.write(final_prompt)
            return model.invoke(final_prompt).content

        res = file_manager.run_staged_process(process_name="Refinement", stage1_func=_mode2_stage1, stage2_func=_mode2_stage2)


    if category == WorkflowType.ADJUSTMENT:
        protocol_mode.mode = "Procedure Revision"
        logger.info("WorkflowType.ADJUSTMENT")
        file_ids = filter_ids(file_ids)

        context = ""
        for file_id in file_ids:
            # read the file content
            if "Literature" in file_id:
                continue
            file_content = file_manager.get_file_content(file_id)
            if file_content:
                context = context + file_content

        final_prompt = prompt1.format(experiment_name=experiment_name, text=context)
        with open(os.path.join(output_path, 'answers','last_workflow_input.md'), 'w', encoding='utf-8') as f:
            f.write(final_prompt)
        res = model.invoke(final_prompt).content

    if category == WorkflowType.OPTIMIZING:
        protocol_mode.mode = "Procedure Optimization"
        logger.info("WorkflowType.OPTIMIZING")
        advice = ""
        procedure = ""
        RMA_return = ""
        file_ids = filter_ids(file_ids)

        for file_id in file_ids:
            if AgentOutputPrefix.HPA_OUTPUT in file_id:
                file_content = file_manager.get_file_content(file_id)
                if file_content:
                    advice = file_content
            if AgentOutputPrefix.PGA_OUTPUT in file_id:
                file_content = file_manager.get_file_content(file_id)
                if file_content:
                    procedure = file_content
            if AgentOutputPrefix.RMA_OUTPUT in file_id:
                file_content = file_manager.get_file_content(file_id)
                if file_content:
                    RMA_return = file_content

        final_prompt = prompt2.format(experiment_name=experiment_name, procedure=procedure, advice=advice)
        if RMA_return:
            final_prompt = PGA_prompt_optimize_validate.format(experiment_name=experiment_name, procedure=procedure) + PGA_prompt_optimize_check_RMA_return.format(reagent_check_result=RMA_return)
        with open(os.path.join(output_path, 'answers','last_workflow_input.md'), 'w', encoding='utf-8') as f:
            f.write(final_prompt)
        res = model.invoke(final_prompt).content
            

    # save input files 
    file_manager.log_agent_inputs(agent_name=AgentName.Protocol, inputs=file_ids+manuals)
    # save to shared_files
    f_id = file_manager.add_file(
        agent_name="Protocol",
        content=res,
        original_filename=f"{experiment_name}-procedure.md",
        description="Generated procedure for the experiment"
    )

    EOF_line = ""
    if  category == WorkflowType.INITIAL:
        EOF_line = "\n The procedure has been saved to the file: " + f_id
    if  category == WorkflowType.REFINEMENT:
        EOF_line = "\n The procedure with reagent check list has been saved to the file: " + f_id 
    if category == WorkflowType.ADJUSTMENT:
        EOF_line = "\n The validated procedure has been saved to the file: " + f_id 
    if category == WorkflowType.OPTIMIZING:
        EOF_line = "\n The procedure has been saved to the file: " + f_id 
    if category == WorkflowType.ALGORITHM:
        EOF_line = "\n The algorithm has been saved to the file: " + f_id
    
    return res + EOF_line
