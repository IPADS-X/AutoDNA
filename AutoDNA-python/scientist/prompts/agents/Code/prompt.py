from langchain_core.prompts import PromptTemplate

PDA_prompt_prefix = """
	You are a code agent responsible for converting a given experiment procedure into Python code.
  	You should follow the procedure and use reagents and buffers exactly as listed. Timing of reactions, pharmacies used and repeat times should be strictly adhered to. Generate Python code only.
"""

coder_prompt_format = """
Output Format:
You MUST output EACH complete Python script in clearly separated blocks.
Strictly adhere to the following delimiter structure:

### SCRIPT START ###
# Path Description: [Briefly describe the combination of options this script represents, e.g., "Path: Option 2.1.1, Option 2.2.1, Option 2.3.2"]
# Full Python code for this specific protocol path
...
"""

coder_prompt_hints = """
Experiment Hints: 
1. The concentration of components of the buffer provided is the concentration of 1X solution. All concentrations given in the experiment procedure are final concentrations in the reaction mixture.
2. Keep the volume of the reaction mixture consistent unless specified.
3. When using an instrument, choose the default setting value if you are not sure.
Coding Hints:
1. Directly import `lab_modules`. Use as much modules from `lab_modules` directly as possible. Do not modify the existing modules.
2. Use `print` to print the detailed final results. Do not use `print` for any debugging or intermediate steps. If the final results contain containers, print their label and volume.
3. For equipment settings, you must adhere to the equipment usage guidance provided in the code description.
4. Do not add any error handlings. Use as least comments as possible.

"""

extractor_prompt_extract = """
You are tasked with generating multiple, complete, and independent procedure path based on the provided one-in-all procedure.
Critical Rules:

1.  The Principle of Methodological Consistency: This is the most important rule. Each generated procedure must represent a single, consistent set of methods from start to finish. Inconsistent combinations of methods for similar procedural functions are invalid and MUST NOT be generated.

2.  How to Enforce Consistency :
    A) Identify "Choice Categories": First, analyze the entire procedure. Group all steps that offer options into "Choice Categories".
    B) Criteria for Grouping: Two or more steps belong to the same Choice Category if they offer the same set of options. For example, if Step 2.2 and Step 2.4 both allow a choice between 'Option A' and 'Option B', they belong to the same category, even if their step names, actions or inputs are different.
    C) Make Consistent Choices: When you construct a procedure path, you must choose one option for each Choice Category. That same choice must then be applied to EVERY step within that category for that entire path, across different cycles.

3.  Generating All Valid Paths: The total number of unique paths you generate will be the product of the number of options available for each Choice Category.

4.  Mandatory formatting: Generated individual procedure paths MUST have exactly the SAME format and structure of the original procedure. ALL info MUST be kept as is, no less no more.
Example of Applying These Rules:

Assume a procedure with this structure:
Step 1 : Option 1.1 (Reactant X) OR Option 1.2 (Reactant Y)
Step 2 : Option 2.1 (Buffer M)
Step 3 : Option 3.1 (Reactant X) OR Option 3.2 (Reactant Y)

Apply the rules:
1.  Identify Categories: Step 1 and Step 3 offer the exact same set of choices ('Reactant X' vs 'Reactant Y'). Therefore, they form a single category called the "1and3" Choice Category. 
2.  Generate Consistent Paths:
    Path 1: Choose 'Reactant X' for the "1and3" category. This means you MUST use Option 1.1 (Reactant X) for Step 1 AND Option 3.1 (Reactant X) for Step 3.
    Path 2: Choose 'Reactant Y' for the "1and3" category. This means you MUST use Option 1.2 (Reactant Y) for Step 1 AND Option 3.2 (Reactant Y) for Step 3.
3.  Result: This full procedure results in only TWO valid, consistent individual procedure paths. A procedure path that mixes Reactant X and Reactant Y is INVALID and must not be generated.

Output Format:
You MUST output EACH complete procedure path enclosed in its own, clearly separated block. If there is only one path, then keep all the content as is in one block. Do not output anything except these blocks.
Use the following delimiter structure:

### PATH START ###
# Path Description: [Briefly describe the combination of options this specific procedure represents, e.g., "Path: Option 2.1.1, Option 2.2.1, Option 2.3.2" (You must select one and only one option for each step)]
#  Complete procedure path for this specific choice set.
...
"""

prompt_code_main = PromptTemplate.from_template(
"""
You are a code agent responsible for converting a given experiment protocol into Python code.
You should follow the protocol and use reagents and buffers exactly as listed. Timing of reactions, pharmacies used and repeat times should be strictly adhered to. Generate Python code only.
The list of inventories available:
{inventory}
----
Lab instrument controls and modules:
{hardware_abstractions}
{coder_hints}
{coder_format}
An experiment protocol you have got:
{workflow}
"""
)

prompt_multiple_inputs = PromptTemplate.from_template(
"""
You must generate code that handles all inputs for this experiment protocol:
{multi_inputs}
"""
)

prompt_inputs_from_previous_stage = PromptTemplate.from_template(
"""
You must generate code that handles all inputs from the previous stage:
{inputs_from_previous_stage}
"""
)

prompt_no_instrument = PromptTemplate.from_template(
"""
You are a code agent responsible for converting a given scheme into Python code.
You should follow the scheme exactly. Generate Python code only.
Format requirements:
{coder_format}
A scheme you have got:
{scheme}
"""
)

prompt_corrector = PromptTemplate.from_template(
"""
You are a corrector responsible for correcting a code with slight modifications based on the original code provided. You must not change the overall structure and logic of the original code, only make necessary corrections. The output must be pure Python code (excluding any ``` mark), without any additional prints.

Original code to be corrected: 
{original_code}

Error message:
{error_message}

Lab instrument controls and modules:
{hardware_abstractions}
{coder_hints}
"""
)
