import json
import asyncio
from loguru import logger
from langchain.tools import tool
from llm.my_react_agent import _get_state_value, AgentState
from langchain.prompts import PromptTemplate
from llm.model import model, hypothesis_model
from pydantic import BaseModel, Field
from tools.file_manager import file_manager
from agents.constants import AgentOutputPrefix, AgentName
from langchain_core.pydantic_v1 import BaseModel, Field
from tools.utils import SelfOptimizingCache, extract_all_from_pdfs
import random as rand
from prompts.agents.Hardware.prompt import paper_prompt, hypothesis_prompt, self_answer_prompt, solution_prompt

from config import output_path, input_manual_mappings_path, manual_path
import os
from tools.utils import getLatestExperimentWorkflowByCache

class Optimizing_Advice(BaseModel):
    """A format for the optimizing advice"""
    hypothesis: str = Field(description="The hypothesis of the optimizing")
    verification: list[str] = Field(description="The verification of the optimizing")

    def __str__(self):
        return f"Hypothesis: {self.hypothesis}, Verification: {', '.join(self.verification)}"


async def async_extract_hypothesis_history(text: str) -> str:
    """
    Asynchronously extracts the single best hypothesis from the given text.
    """
    text_with_suffix = text + os.linesep + "You must summarize the best hypothesis into a key phrase (no more than 5 words). It must be your only output."
    # Use ainvoke for asynchronous execution
    response = await hypothesis_model.ainvoke(text_with_suffix)
    return response.content

def get_hypothesis_and_answers_concurrently(optimizing_history, last_workflow, optimize_target, last_code, other_targets, context) -> Optimizing_Advice:
    """
    Orchestrates a three-step sequential generation of a hypothesis, its summary, and a final answer.
    """
    async def main():
        # --- STAGE A: Generate the initial hypothesis ---
        
        # Step 1: Format initial prompt
        initial_prompt = PromptTemplate.from_template(hypothesis_prompt).format(
            text=optimizing_history,
            last_workflow=last_workflow,
            last_code=last_code,
            optimize_target=optimize_target,
            note="None"
        )

        # save the prompt
        with open(os.path.join(output_path, "HPA_Stage_A_Prompt.txt"), "w", encoding='utf-8') as f:
            f.write(initial_prompt)
        # Step 2: Get a single initial hypothesis
        logger.debug("Generating initial hypothesis...")
        result_A = await hypothesis_model.ainvoke(initial_prompt)
        logger.debug("Generation complete.")

        chosen_origin_hypothesis = result_A.content

        # --- STAGE B: Extract the summary ---
        
        # Step 3: Extract a summary from the chosen hypothesis
        logger.debug("Generating summary for the chosen hypothesis...")
        chosen_question_summary = await async_extract_hypothesis_history(chosen_origin_hypothesis)
        logger.debug("Extraction complete.")


        # --- STAGE C: Generate the final answer ---

        # Step 4: Get a final answer based on the summary
        final_solution_prompt = solution_prompt.format(
            last_procedure=last_workflow,
            text=chosen_question_summary,
            context=context,
        )
        # save the prompt
        with open(os.path.join(output_path, "HPA_Stage_C_Prompt.txt"),
                    "w", encoding='utf-8') as f:
                f.write(final_solution_prompt)


        logger.debug("Generating final answer...")
        result_C = await hypothesis_model.ainvoke(final_solution_prompt)
        logger.debug("Generation complete.")

        chosen_final_answer = result_C.content
        
        # --- Caching the complete chosen result path ---
        SelfOptimizingCache.push_paper_hypothesis_input(initial_prompt)
        SelfOptimizingCache.push_paper_origin_hypothesis(chosen_origin_hypothesis)
        SelfOptimizingCache.push_hypothesis_paper_extract(chosen_question_summary)
        SelfOptimizingCache.push_paper_hypothesis(chosen_final_answer)
        SelfOptimizingCache.push_hypothesis(chosen_final_answer)

        return chosen_final_answer

    # Run the async main function from our synchronous tool
    return asyncio.run(main())


@tool
def Hypothesis(current_optimize_target: str, roll_back_num: int, current_procedure_id: str, manuals: list[str] = []) -> str:
    """Generates an optimization advice for an experimental procedure based on its history and a specified optimization target.

    Args:
        current_optimize_target (str): The target to be optimized. Only supports a single target.
        roll_back_num (int): The number of recent, ineffective optimization attempts to discard.
        current_procedure_id (str): The ID of the experimental procedure that is being optimized.
        manuals (list[str], optional): An optional list of file IDs for manuals that were collected during the process, which may inform the optimization advice.

    Returns:
        str: A formatted string containing the new optimization advice and a reference to its saved file ID.
    """
    
    other_targets = []
    
    if roll_back_num > 0:
        last_procedure_id = file_manager.get_last_agent_inputs(AgentName.Hypothesis)[0]
        if not last_procedure_id:
            raise ValueError("No previous procedure ID found to roll back.")

        while len(SelfOptimizingCache.result_history) <= len(SelfOptimizingCache.optimizing_history):
            SelfOptimizingCache.push_result("Not available to verify. Suggest an alternative hypothesis.")
        for i in range(roll_back_num):
            SelfOptimizingCache.mark_result_rollback(len(SelfOptimizingCache.result_history) - 1 - i)
            SelfOptimizingCache.mark_optimizing_rollback(len(SelfOptimizingCache.optimizing_history) - 1 - i)

    SelfOptimizingCache.tmp_optimizing_history.clear()
    
    optimizing_history = SelfOptimizingCache.get_optimizing_history_and_result_history()
    last_procedure = file_manager.get_file_content(current_procedure_id) if current_procedure_id else getLatestExperimentWorkflowByCache()
    last_code = ""

    # get the content of the manuals
    context = ""
    with open(os.path.join(input_manual_mappings_path, "manual_map.json"), "r") as f:
        kv = json.load(f)
    manual_paths = []
    for manual in manuals:
        if manual in kv:
            manual_paths.append(os.path.join(manual_path, kv[manual]))
    manual_text = extract_all_from_pdfs(manual_paths)
    if manual_text:
        context = context + "\n" + manual_text
    # --- Main Change: Call the new sequential function ---
    final_hypothesis = get_hypothesis_and_answers_concurrently(
        optimizing_history, 
        last_procedure, 
        current_optimize_target, 
        last_code, 
        other_targets, 
        context
    )
    
    SelfOptimizingCache.push_optimizing(str(final_hypothesis))

    file_id = file_manager.add_file(AgentOutputPrefix.HPA_OUTPUT, str(final_hypothesis))
    if not roll_back_num > 0:
        file_manager.log_agent_inputs(AgentName.Hypothesis, current_procedure_id)
    
    EOF_line = "\n The optimizing hypothesis is saved to file: " + file_id 
    roll_back_line = ""
    if roll_back_num > 0:
        roll_back_line = f"\n Rolled back to procedure ID: {last_procedure_id}"
        
    res = str(final_hypothesis) + EOF_line + roll_back_line
    return res