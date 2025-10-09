import os

base_path = "."
output_path = os.path.join(base_path, "output")
input_path = os.path.join(base_path, "input")
reagent_db_path = os.path.join(base_path, "input")
reagent_path = os.path.join(input_path, "reagents")
input_manual_mappings_path = os.path.join(input_path, "manuals")
paper_path = os.path.join(base_path, "papers")
test_path = os.path.join(base_path, "tests")
prompt_path = os.path.join(base_path, "prompts")
agent_prompt_path = os.path.join(prompt_path, "agents")
manual_path = os.path.join(paper_path, "manuals")
shared_files_path = os.path.join(output_path, "shared_files")
prompt_path = os.path.join(base_path, "prompts")
executor_path = os.path.join(base_path, "executor")
corrector_path = os.path.join(executor_path, "corrector")
scheduler_path = os.path.join(executor_path, "scheduler")
tools_path = os.path.join(base_path, "tools")
hardware_abstractions_path = os.path.join(tools_path, "lab_modules.py")
current_output_file_path = os.path.join(output_path, "current.json")

scheduler_address = "localhost:8080"
scheduler_config_path = "/home/ethereal/vm5/code/RAG/productionLine/ProductionLineScheduler/config"

tool_max_length_output = 5000000


qa_prompt_suffix = " You should answer the question as detailed as possible. For example, If you make a buffer, you should explicitly list the concentration of each reagents and their volume portion. If you do something more than once, you should explicitly tell how many times are suitable."
qa_prompt_option = """ 
If there are multiple methods to do the experiment or multiple options for the same method, you should list all the methods or options. You must find all the information with all the details.
"""
qa_prompt_general_guidelines = """
You should find and list the general guidelines or principles if relevant to the experiment.
"""

pharmacy_prompt_output_format = """
    MANDATORY RULES AND OUTPUT FORMAT:
    a. Availability of a pre-mixed buffer does not imply the availability of its individual chemical components.
    b. For solution, if its recipe is either not specified in the request or not specified in the repository, then it is considered "available" if an item with the same functional name exists in the repository. Otherwise, apply rule c. 
    c. If the concentration of its recipe is either not specified in the request or not specified in the repository, then it is considered "available" if one item in repository has the same chemical components (regardless of concentration). Otherwise, a requested solution is only considered "available" if the repository lists an item with the same chemical components and component ratios, at a concentration equal to or greater than the concentration requested.
    d. Availability is not implied by the presence of its raw chemical components or by any in situ preparation.

    * For each requested substance, provide the following information in a line:
        Reagent Name
        Availability Status (e.g., "available", "not available")
        If available, 
        - if liquid, provide the concentration(if it has) and pH(if it has)
        - if solid, output "available, solid"
        Notes(if it has) and Related manual name (if it has)
        If not available, output "not available" first. Then if there's any buffer with similar function or same name, output its related information.
    * Example Output:
        xxx: available, 50mM, pH 7.0
        yyy: not available
        zzz: available, 100mM, manual name zzz-manual
        aaa: available, solid
    * Just output the information in the format above. Do not add any other information. 
    * At the end of the file, output a line starts with <manuals> 
    * This line contains all of the manuals that are related to the reagents in the output. The manuals are separated by commas and deduplicated. Must not output anything other than manual name. If there are no manuals, OMIT THE LINE. 
"""


class AppConfig:
    """A simple configuration holder."""
    def __init__(self):
        self.mock_mode = False
        self.no_filtering = False
        self.synthesis = False
        self.storage = False
        self.test = False
        self.rpa = False
        self.rna = False
        self.amplification = False
        self.write = False # DNA storage, write
        self.read = False # DNA storage, read
        self.polya = False

        self.variants = False # if set, code agent may generate multiple codes for multiple procedures; Otherwise, just one code for the best procedure

        self.questions = False # if set, then Literature will use the questions to retrive experiment information

        self.CURRENT_USER_PROMPT_FILE = "current_user_prompt.md"
        self.CURRENT_USER_REQUIREMENT_FILE = "current_user_requirement.md"

class ProtocolMode:

    def __init__(self):
        self.mode = "Procedure Design"

# Create a single, shared instance that the rest of the app can import.
settings = AppConfig()
protocol_mode = ProtocolMode()


