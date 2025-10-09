import re
import os
import json
from loguru import logger
from config import output_path, pharmacy_prompt_output_format, settings, input_path
from langchain.tools import tool
from llm.model import model, reasoner_model, pharmacy_model
from langchain_core.prompts import PromptTemplate
from tools.reagent_manager import reagent_manager
from tools.file_manager import file_manager
from tools.utils import get_inventory


pharmacy_output_path = os.path.join(output_path, "answers", "pharmacy.txt")

pharmacy_description = (
"""
Checks the availability of reagents from a requested list against the lab's inventory or gathers a summary of all reagents in the inventory.

Args:
    reagent_query_id (str): The file ID of the file that contains the list of reagents to be checked.
    gather_mode(bool, optional): If set to True, the tool will return a summary of all reagents in the lab's inventory. Defaults to False.
Returns:
    str: A string detailing the availability of the requested reagents and manuals that may contain additional information about the experiment.
"""
)

def getCheckListStr(reagent_query_id: str) -> str:
    workflow = file_manager.get_file_content(reagent_query_id)
    start_phrase = r"Reagent Check List:"
    end_phrase = r"additional_kwargs"
    reagents_section = workflow.split(start_phrase)[1]
    reagents_section = reagents_section.split("---")[1]  # Split by the separator
    reagents_section = reagents_section.split(end_phrase)[0]

    reagents_section = reagents_section.replace("\\n", ", ").replace("\\", "").replace("'", "")
    return reagents_section


def format_reagents_json(reagents_json, list_manuals : bool = False) -> str:
    formatted_lines = []
    manuals = []
    manual_line = "<manuals> "
    
    for reagent in reagents_json:
        # Handle names and aliases
        names = [reagent["name"]] + reagent.get("aliases", [])
        name_str = " or ".join(f'"{name}"' for name in names)
        
        # Basic properties
        line = f"{name_str}: "
        
        # Add concentration if exists
        if "concentration" in reagent.get("properties", {}):
            conc = reagent["properties"]["concentration"]
            line += f"{conc['value']}{conc['unit']}, "
        
        # Add pH if exists
        if "ph" in reagent.get("properties", {}):
            line += f"pH {reagent['properties']['ph']}, "
        
        # Add form information
        line += f"{reagent['form'].lower()}, "
        
        # Add components if buffer
        if "components" in reagent:
            components = []
            for comp in reagent["components"]:
                if isinstance(comp, dict):
                    # check if 'concentration' exists
                    if 'concentration' in comp:
                        components.append(f"{comp['name']} ({comp['concentration']})")
                    else:
                        components.append(comp['name'])
                else:
                    components.append(comp)
            line += f"components: {', '.join(components)}, "
        
        # Add documentation
        if "documentation" in reagent:
            line += f"doc: {', '.join(reagent['documentation'])}, "
            if list_manuals:
                for manual in reagent['documentation']:
                    if manual not in manuals:
                        manuals.append(manual)
                        
        # Add notes
        if "notes" in reagent:
            line += f"notes: {reagent['notes']}, "
        
        # Clean up trailing comma
        line = line.rstrip(", ")
        formatted_lines.append(line)
        
    if list_manuals and manuals:
        manual_line += ", ".join(manuals)
        formatted_lines.append(manual_line)
    return "\n".join(formatted_lines) 



@tool(description=pharmacy_description)
def Reagent(reagent_query_id: str = "", gather_mode: bool = False) -> str:

    # reagent_repo = reagent_manager.get_all_reagents()
    reagent_repo = get_inventory()
    reagent_repo_str = format_reagents_json(reagent_repo, gather_mode)
    logger.debug(f"Reagent Repository: {reagent_repo_str}")
    if gather_mode:
        f_id = file_manager.add_file(
            agent_name="Reagent",
            content=reagent_repo_str,
            original_filename="pharmacy.txt",
            description="Pharmacy check result"
        )
        return reagent_repo_str + os.linesep + "The information about all reagents has been saved to the file: " + f_id

    checkList = getCheckListStr(reagent_query_id)
    prompt = PromptTemplate.from_template(
"""
You are a reagent manager responsible for checking a list of requested items against the lab's current inventory.
Given the input check list, provide an output comforming to the following rules and format:
{rules_format}
The information related to current inventory in the lab is as follows:
{reagents_info}
Below is the input check list:
{requested_reagents}
"""
    )

    final_prompt = prompt.format(
        reagents_info=reagent_repo_str,
        rules_format=pharmacy_prompt_output_format,
        requested_reagents=checkList
    )
    response = pharmacy_model.invoke(final_prompt)

    ### for debug 
    pharmacy_input_path = os.path.join(output_path, "answers", "pharmacy_input.md")
    with open(pharmacy_input_path, "w", encoding='utf-8') as f:
        f.write(final_prompt)
    ### for debug end
    # maybe deprecated soon
    with open(pharmacy_output_path, "w", encoding='utf-8') as f:
        f.write(response.content)

    # save the output to file manager
    f_id = file_manager.add_file(
        agent_name="Reagent",
        content=response.content,
        original_filename="pharmacy.txt",
        description="Pharmacy check result"
    )

    EOF_line = "\n The information about requested reagents has been saved to the file: " + f_id 

    return response.content + EOF_line

