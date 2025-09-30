from langchain.prompts import PromptTemplate

procedure_format = """
    **MANDATORY OUTPUT STRUCTURE:**
    You **MUST** format the entire workflow strictly as follows. Do not deviate from this structure:

    Part [Part Number]: [Descriptive Title of this Part of the Experiment]

        Step [Part Number].[Step Number]: [description of the action to be performed in this step]

            Option [Part Number].[Step Number].[Option Number]: [Description of this specific option.]
            (Example: Option 1.1.1: xyz 
                        - Component A: 
                            - Recipe (remove this line if component itself does not have a recipe)
                        - Component B: 
                            - Recipe (remove this line if component itself does not have a recipe)
                        - Component C: 
                            - Recipe (remove this line if component itself does not have a recipe)
                        - Component D: 
                            - Recipe (remove this line if component itself does not have a recipe)
                        (operational details, if any)
                        (total volume is flexible and not part of this option's definition.)
            )

            Option [Part Number].[Step Number].[Option Number]: [Description of an alternative option, if chemically viable and different in composition, using available reagents.]
            (Example: Option 1.1.2: xyz 
                        - Component A: 
                            - Recipe (remove this line if component itself does not have a recipe)
                        - Component B: 
                            - Recipe (remove this line if component itself does not have a recipe)
                        - Component C: 
                            - Recipe (remove this line if component itself does not have a recipe)
                        (operational details, if any)
            )
            (List further distinct compositional options if possible)

        Step [Part Number].[Step Number]: [Next step in this part]


    Part [Part Number]: [Next Major Part of the Experiment]
        Step 2.1: 
            Option 2.1.1: ...
            ... and so on for all parts, steps, and options.
"""

procedure_format_without_details = """
    **MANDATORY OUTPUT STRUCTURE:**
    You **MUST** format the entire workflow strictly as follows. Do not deviate from this structure:

    Part [Part Number]: [Descriptive Title of this Part of the Experiment]

        Step [Part Number].[Step Number]: [description of the action to be performed in this step]

            Option [Part Number].[Step Number].[Option Number]: [Description of this specific option.]
            (Example: Option 1.1.1: xyz 
                        - Component A: 
                            - Recipe (remove this line if component's recipe isn't specified)
                        - Component B: 
                            - Recipe (remove this line if component's recipe isn't specified)
                        - Component C: 
                            - Recipe (remove this line if component's recipe isn't specified)
                        - Component D: 
                            - Recipe (remove this line if component's recipe isn't specified)
            )

            Option [Part Number].[Step Number].[Option Number]: [Description of an alternative option, if chemically viable and different in composition, using available reagents.]
            (Example: Option 1.1.2: xyz 
                        - Component A: 
                            - Recipe (remove this line if component's recipe isn't specified)
                        - Component B: 
                            - Recipe (remove this line if component's recipe isn't specified)
                        - Component C: 
                            - Recipe (remove this line if component's recipe isn't specified)
            )

        Step [Part Number].[Step Number]: [Next step in this part]


    Part [Part Number]: [Next Major Part of the Experiment]
        Step 2.1: 
            Option 2.1.1: ...
            ... and so on for all parts, steps, and options.
"""

PGA_prompt_option_new = f"""
Generate a comprehensive workflow that meticulously follows the specified output structure and explores all viable procedural options at each step, especially concerning reagent and buffer compositions.

    **MANDATORY RULE:**
    You **MUST NOT** use related information other than the input.
    {procedure_format}

    **KEY INSTRUCTIONS FOR GENERATING WORKFLOW DETAILS:**

    1.  **Adherence to Structure:** The "Part X: -> Step X.Y: -> Option X.Y.Z:" hierarchy is non-negotiable. DO NOT ADD SUBOPTIONS.
    2.  **Exhaustive Compositional Options:**
        *   For every step involving the preparation or use of a solution/buffer, you **MUST** identify and list **ALL DISTINCT AND CHEMICALLY VIABLE OPTIONS** for its composition
        *   A "distinct option" means a different set of chemical components or significantly different concentrations of the same components that achieve the required function .
        *   options should NOT simply be different total volumes of the exact same buffer recipe. The focus is on *variations in chemical composition or component configuration*, not the quantity of buffer produced.
"""

PGA_prompt_option_mode0 = f"""
Generate a comprehensive experiment procedure that meticulously follows the specified output structure and explores all viable procedural options at each step. Include reaction conditions and operational details in the procedure.

    MANDATORY RULE:
    You MUST NOT use any reagent not mentioned in the input.
    {procedure_format}
    KEY INSTRUCTIONS FOR GENERATING PROCEDURE DETAILS:
    1.  Adherence to Structure: The "Part X: -> Step X.Y: -> Option X.Y.Z:" hierarchy is non-negotiable. DO NOT ADD SUBOPTIONS.
    2.  Exhaustive Compositional Options:
        *   Options should NOT simply be different total volumes of the exact same buffer recipe. The focus is not on the quantity of buffer produced or differences in concentration.
    3.  Component name: If a primary designed function name is mentioned in the input, you MUST use that name as the component name. If no such name is mentioned, you should use its application name if mentioned in the input. If no such name is mentioned, you should use its chemical name.
"""

PGA_prompt_option_mode1 = """
Modify a comprehensive procedure that meticulously follows the specified output structure and explores all viable procedural options at each step, especially concerning reagent and buffer compositions. New options should be based on provided information, and you can add or remove steps to reflect this.

    MANDATORY OUTPUT STRUCTURE:
    You must format the entire workflow strictly as follows. Do not deviate from this structure:

    Part [Part Number]: [Descriptive Title of this Part of the Experiment]

        Step [Part Number].[Step Number]: [description of the action to be performed in this step]

            Option [Part Number].[Step Number].[Option Number]: [Description of this specific option.]
            (Example: Option 1.1.1: xyz 
                        - Component A: 
                            - Recipe (remove this line if component itself does not have a recipe)
                        - Component B: 
                            - Recipe (remove this line if component itself does not have a recipe)
                        - Component C: 
                            - Recipe (remove this line if component itself does not have a recipe)
                        - Component D: 
                            - Recipe (remove this line if component itself does not have a recipe)
            )

            Option [Part Number].[Step Number].[Option Number]: [Description of an alternative option, if chemically viable and different in composition, using available reagents.]
            (Example: Option 1.1.2: xyz 
                        - Component A: 
                            - Recipe (remove this line if component itself does not have a recipe)
                        - Component B: 
                            - Recipe (remove this line if component itself does not have a recipe)
                        - Component C: 
                            - Recipe (remove this line if component itself does not have a recipe)
            )
            (List further distinct compositional options if possible)

        Step [Part Number].[Step Number]: [Next step in this part]


    Part [Part Number]: [Next Major Part of the Experiment]
        Step 2.1: 
            Option 2.1.1: ...
            ... and so on for all parts, steps, and options.

KEY INSTRUCTIONS FOR GENERATING WORKFLOW DETAILS

    1. Adherence to Structure The "Part X: -> Step X.Y: -> Option X.Y.Z:" hierarchy is non-negotiable. DO NOT ADD SUBOPTIONS.
    2. Options should NOT simply be different total volumes of the exact same buffer recipe. The focus is not on the quantity of buffer produced or differences in concentration.
    3. You must check the newly input documents for any additional options that can be added to the workflow. Add details to the procedure if you find anything related. PAY ATTENTION: YOU ARE ALLOWED TO CHANGE the procedure to reflect the new options.
    4. If there are multi-cycles in the procedure, you must ensure the consistency between the cycles.

"""

PGA_prompt_list_reagents = """

    **POST-WORKFLOW OUTPUT: REAGENT CHECK LIST**

    After generating the complete workflow as detailed above, you **MUST** output a section titled:
    "---
    MANDOTORY: YOU MUST CHECK EXACTLY THE SAME REAGENTS AS LISTED BELOW, NO LESS NO MORE.
    Reagent Check List:
    ---"

    Under this title, provide a comprehensive, alphabetized, and deduplicated list of **ALL** individual chemical reagents and components mentioned anywhere in the "Option" descriptions of the generated workflow.
    * Each reagent should be listed on a new line.
    * Ensure this list is complete and accurately reflects all reagents required by the workflow.
    * This list will be used by a ReAct agent to query a pharmacy tool for reagent availability.

    Example of Reagent Check List:
    ---
    Reagent Check List:
    ---
    xxx
    yyy
    zzz
"""

workflow_additional_instructions = """
**ADDITIONAL CRITICAL DIRECTIVES:**

*   **Information Sourcing:** Your *only* sources of information for generating this workflow are the "Experimental Information" and the "Available Reagents list" provided below. You **MUST NOT** invent steps, reagents, concentrations, or procedures. Do not infer common lab practices unless they are directly supported by the provided text except that the input text is insufficient to detail a step or buffer. If the exceptions occur, you must EXPLICITLY state that and explain why you take a new approach.
"""


workflow_additional_instructions_BW = """
ADDITIONAL CRITICAL DIRECTIVES:

**Comprehensive Option Generation:**
    * When detailing procedural steps, especially those involving solutions , you **MUST** meticulously examine *all provided source documents* to identify potential components and compositions.
    * If there are various buffers used for functionally similar purposes, or if there are solutions with similar names, or if there are recommended buffers in the manual serving similar functions, you **MUST** list each such described solution as a potential distinct option for **any step with similar functionality** in ANY PART of the experiment procedure. Any solution mentioned within the full context of the experimental descriptions could be considered a candidate across various stages. These options are to be treated as generic possibilities. DO NOT JUDGE IF THEY ARE REASONABLE. LIST THEM ALL.
    * For steps with similar functions or purposes, their options should be consistent and complete.
"""

workflow_solution_preparation_guidelines = """

**CRITICAL GUIDELINES FOR HANDLING SOLUTIONS AND BUFFERS (Adhere strictly to these rules in addition to all other mandatory instructions):**

1.  **Regarding Pre-Prepared Stock Solutions (from 'Available Reagents' List):**
    *   If the 'Available Reagents' list for the current task describes a reagent with a concentration , you **MUST** treat this item as already formulated and ready for direct use. You **MUST DELETE** any steps that would involve preparing this stock solution. If the 'Available Reagents' list for the current task describes a buffer with a concentration , you **MUST** treat this item as an already formulated buffer and ready for direct use. You **MUST MODIFY** any steps that would involve preparing the buffer, replacing them with "Directly use xxxx stock".
    *   Under no circumstances should you generate any step in the workflow to *prepare this specific stock solution itself*.
    *   Your workflow steps should only describe how to *add it directly* to a reaction mixture using the specified available form.

2.  **Regarding Preparation of Other Working Buffers and Solutions:**
    *   **For Single-Use Buffers/Solutions:** If a specific working buffer or solution (which is not itself a pre-prepared stock from the reagent list) is required for only *one individual procedural step* in the entire experimental workflow:
        *   You **MUST NOT** create a separate, advance preparation step for it.
        *   Instead, within the `Option [Part.Step.OptNum]:` of the single step where this solution is actually used, you **MUST** list all its individual chemical components directly. For each component, specify its source and its target final concentration in the context of that specific reaction or procedural setup.
    *   **For Multi-Use Buffers/Solutions:** If a working buffer or solution (which is not itself a pre-prepared stock from the reagent list) will be used ONLY on its own, then you may prepare it. OTHERWISE, DO NOT PREPARE IT BEFORE USE BECAUSE WHEN IT'S ADDED TO THE REACTION MIXTURE, IT WILL NOT REACH FINAL CONCENTRATION EXPECTED.

"""

workflow_general_guidelines = """
**Application of General Guidelines and Principles:**
    * You MUST identify any instructions in the source that acts like general principles or guidelines for the experiment or solution or material you use.
    * For each such general guideline you find, you should incorporate it into the description of subsequent steps in the workflow where that guideline is relevant and reasonable. Do not list them separately. Do not list them if you assume it's not relevant.
    * Do not assume a general guideline applies only to the section where it is first mentioned. Propagate these foundational principles across the appropriate steps to ensure the experimental condition is good.
"""

# todo: 格式调整
PGA_prompt_intermediate_reagents = """

    POST-PROCEDURE OUTPUT SECTIONS

    After generating the complete procedure as detailed above, you must output a section titled:
    "---
    A list containing reagents whose availability has yet to be verified.
    Reagent Check List:
    ---"
    Under this title, provide a comprehensive and deduplicated list of all individual chemical reagents and components mentioned anywhere in the "Option" descriptions of the generated procedure. Besides, you must also include intermediate reagents, material, solutions that are explicitly mentioned in the procedure as being ready for use at a later stage of the protocol. These materials may be produced in earlier steps or parts of the procedure. Put them in the list so that if they are already prepared, they can be used directly in the next steps of the procedure. All buffers that have explict components must be included in the list. Their components should be also listed, respectively. 
    Each reagent should be listed on a new line. If a buffer comes with its components, you should list the buffer together with its components in the parenthesis.
    Do not include any physical containers or disposable items. If there's any part documenting only technique without any reagent, include its possible reagents in the list.
    This list is intended for another agent to verify reagent availability.

    Example of Reagent Check List:
    ---
    A list containing reagents whose availability has yet to be verified.
    Reagent Check List:
    ---
    xxx
    yyy
    zzz
    intermediate_material_a
    intermediate_solution_b(components: xxx, yyy)
"""

PGA_prompt_additional_reagent = """
    **POST-WORKFLOW OUTPUT SECTIONS**

    After generating the complete workflow as detailed above, You MUST compare the new workflow with the old workflow in the input. If there's any new reagent or intermediate that is not listed in the old workflow, you **MUST** output a section titled(this section is not for you, it's for ReAct agent):
    "---
    **MANDATORY**: YOU **MUST** CHECK EXACTLY THE SAME REAGENTS AND INTERMEDIATES AS LISTED BELOW.
    Reagent Check List:
    ---"

    Under this title, provide a comprehensive, alphabetized, and deduplicated list of new chemical reagents and components that doesn't exist in old workflow. Besides, you 
    ** MUST ** also include intermediate reagents, material, solutions that are explicitly mentioned in the new workflow but not in the old workflow.
    * Each reagent should be listed on a new line.
    * This list will be used by a ReAct agent to query a pharmacy tool for reagent availability.

    Example of Reagent Check List:
    ---
    Reagent Check List:
    ---
    xxx
    intermediate_material_a
    intermediate_solution_b

    If there are no new reagents or intermediates, you MUST output the following:
    ---
    Workflow is good. No new reagents or intermediates to check. Proceed to the next step.
    ---
"""

PGA_prompt_simplification = """
You do not need to list the origin of the reagents, intermediates or buffers. Remove their origins.
"""

PGA_prompt_reducing_repeats = """
**MANDATORY SIMPLIFICATION INSTRUCTION:**
YOU MUST try to simple the experiment by checking experiment conditions like repeat times. 
First, You must categorize each repetition instruction into one of two types:
Type A (Explicitly Quantified): Instructions that specify a precise number or a numerical range for the repetition.
Type B (Described or Suggested): Instructions that suggest, advise, or vaguely describe repetition.
Then Apply the simplification Logic: When you regenerate the workflow, you must apply the following logic based on the category:
For Type A instructions: LEAVE THEM COMPLETELY UNCHANGED. The regenerated workflow must contain the exact same number or numerical range instruction.
For Type B instructions: IGNORE THEM AND REDUCE THE REPEAT TIMES TO THE MINIMUM, A SINGLE EXECUTION IF POSSIBLE. 
"""

adjust_workflow_filtering_prompt_new = f"""
Your Critical Task: Filter and Validate an Existing procedure Based on Reagent Availability, Adhering to a Specific Output Structure

You will be provided with two key pieces of information:
1.  An 'Original procedure Document': This is a comprehensive experimental procedure, previously generated.
2.  A 'Reagent Availability List': This is the list of the status of the reagents that have been requested, along with their specifications. You MUST generate a NEW REAGENT CHECK LIST for those reagents not in the list. You can assume those reagents are available for now, but you should check them again.

Your objective is to produce a 'Revised Procedure'. 
{procedure_format}

KEY INSTRUCTIONS FOR FILTERING AND VALIDATION:

1.  Input Analysis:
    a. Use the 'Reagent Availability List' as the absolute source of truth for reagent availability.
    b. Reagents not on the list are considered available for now. You should check them later on.

2.  Validation of Each Original Option:
    a.  For every Option, verify if any one of its components is listed as unavailable in the 'Reagent Availability List'. If they are not on the list, you can assume they are available for now. For buffers or solutions, if they are not listed in the 'Reagent Availability List' with their specific recipe, you can assume they are available for now.
    b.  If an Option from the 'Original Workflow Document' requires any reagent that is reported "unavailable" in the 'Reagent Availability List' and the reagent is not prepared in preceding steps, then that Option is INVALIDATED. You MUST remove them directly and give no reason.
    c. If a Step has no valid Options left after filtering, the entire Step is removed. 
    d. If a Part has no valid Steps left after filtering, the entire Part is removed.

3.   Critical Exceptions:
    a. A buffer MUST be considered available if provided as available under its functional/common name, irrespective of the availability of its individual constituents.
    b. If a reagent is optional, then you can proceed without it.
    
4.  Final Output:
    * The 'Revised Procedure' should be a clean document containing only performable Parts, Steps, and Options, strictly formatted. All remaining Parts and Steps must be renumbered sequentially. Any invalidated part must not be included. All the details of the validated steps and options must be kept.
    
"""

PGA_prompt_do_not_prepare_existing_material = """
You MUST simplify the procedure based on the provided list of available materials. If the final product of any step or sequence of steps is already listed as an available starting material, you MUST eliminate all steps dedicated to its preparation. Use pre-existing material directly.
"""

PGA_prompt_filter = f"""
Filter and Validate an Existing procedure Based on Reagent Availability, Adhering to a Specific Output Structure.

You will be provided with two key pieces of information:
1.  An 'Original procedure Document': This is a comprehensive experimental procedure, previously generated, which details Parts, Steps, and various Options for each step. 
2.  A 'Reagent Availability List': This is the list of the status of the reagents that have been requested, along with their specifications, including individual and intermediate materials and solutions. Reagents not mentioned in the list are available by default.

Your objective is to produce a 'Revised Validated Procedure'. This revised procedure **MUST**:
    a.  Strictly adhere to the "MANDATORY OUTPUT STRUCTURE" for all included elements.
    b.  ONLY include Parts, Steps, and specific Options from the 'Original procedure Document' that are performable.
{procedure_format}
KEY INSTRUCTIONS FOR FILTERING AND VALIDATION:
1.  Input Analysis:
    a.  Use the 'Reagent Availability List' as the absolute source of truth for reagent availability.
    b.  Reagents not on the list are considered available.
2.  Validation of Each Original Option:
    a.  For every Option described in the 'Original Workflow Document':
        i.  Identify ALL chemical components and their specific concentrations required by that original Option.
        ii. Verify if ANY ONE of these components is listed as unavailable in the 'Reagent Availability List'. If they are not on the list, you can assume they are available for now. 
    b.  If an Option from the 'Original Workflow Document' requires any reagent that is reported "unavailable" in the 'Reagent Availability List' and the reagent is not prepared in preceding steps, then that Option is INVALIDATED. You MUST remove them directly and give no reason.
    c.  If a Step has no valid Options left after filtering, the entire Step is removed. 
    d.  If a Part has no valid Steps left after filtering, the entire Part is removed.
3.   Critical Exceptions:
    a. A buffer must be considered available if provided as available under its functional/common name, irrespective of the availability of its individual constituents.
4.  Final Output:
    a.  The 'Revised Validated Workflow' should be a clean document containing only performable Parts, Steps, and Options, strictly formatted. All remaining Parts and Steps must be renumbered sequentially. Any invalidated part MUST NOT BE included. All the details of the validated steps and options must be kept.

"""

workflow_solution_preparation_guidelines2 = """
DO NOT prepare ANY solutions or buffers.
"""

# json
PGA_prompt_summary = """
At the end, you must change a new line and start a section with title <Protocol Summary>.
Under this title, summarize the output in one sentence and give one recommendation.
"""

PGA_prompt_optimize1 = f"""
Update an original workflow based on the provided optimizing advice.
    {procedure_format}
    KEY INSTRUCTIONS FOR GENERATING PROCEDURE DETAILS:
    1.  Adherence to Structure: The "Part X: -> Step X.Y: -> Option X.Y.Z:" hierarchy is non-negotiable.
    2.  Determinism: If given an optimizing advice containing a specific range, you must select the value that provides the maximum possible effect.

"""

PGA_prompt_optimize2 = f"""
Validate an orginal workflow that meticulously follows the specified output structure. Do not modify any validated part. If there are still multiple options passing validation in one step, keep the best option only. For all options that stay, keep them as they are.
    {procedure_format}
    KEY INSTRUCTIONS FOR GENERATING PROCEDURE DETAILS:
    1.  Adherence to Structure: The "Part X: -> Step X.Y: -> Option X.Y.Z:" hierarchy is non-negotiable.

"""

PGA_prompt_optimize_new_reagent = """
You must figure out if there is any new reagent or buffer in the advice. Reagents and buffers that exist in the provided procedure must not be counted. However, if there is any component change(removal, or concentration adjustment) of a buffer, it must be counted as a new buffer.
If so, at the end of the output, write down a section titled:     
    "---
    A list containing reagents whose availability has yet to be verified.
    Reagent Check List:
    ---"
Under this title, provide all new reagents and buffers found in the advice(excluding those in the original procedure), line by line.
"""

PGA_prompt_optimize_validate = PromptTemplate.from_template(
        """
        You are an validation assistant that can help validate experiment procedure based on the given optimizing advice about {experiment_name}. 
        **Your Task:**
        """
        + f"\n{PGA_prompt_optimize2}"
        """
        The procedure is:
        {procedure}
        """
)
PGA_prompt_optimize_check_RMA_return = PromptTemplate.from_template(
"""
Below is reagent check result:
{reagent_check_result}
"""
)

PGA_prompt_option_rpa = f"""
Generate a comprehensive experiment procedure that meticulously follows the specified output structure.

    {procedure_format_without_details}
    KEY INSTRUCTIONS FOR GENERATING PROCEDURE DETAILS:

    1.  Adherence to Structure: The "Part X: -> Step X.Y: -> Option X.Y.Z:" hierarchy is non-negotiable. DO NOT ADD SUBOPTIONS.
    2.  Automation: The generated protocol is to be used by automated instruments and systems.
    3.  Readiness: The environment is already cleaned, decontaminated and ready, so as the instruments. Necessary setups and reagentsare already in place. Such jobs should not be included in the procedure.
"""

prompt_generate_scheme = PromptTemplate.from_template(
"""
You are a helpful assistant that can help generate a scheme. Your goal is to develop an algorithm that can be easily translated to code.
User Requirement:
{user_prompt}
The information about the experiment is:
{text}
"""
)

prompt_generate_from_scratch = PromptTemplate.from_template(
"""
You are a helpful assistant that can help generate experiment procedures. Your goal is to develop a structured experiment process for {experiment_name}.
User Requirement:
{user_prompt}
"""
+ f"\n{PGA_prompt_option_rpa}" + f"\n{PGA_prompt_intermediate_reagents}" +
"""
The information about the experiment is (may contain redundant even inaccurate information):
{text}
"""
)

prompt_retry = PromptTemplate.from_template(
"""
You are a helpful assistant that can help generate experiment procedures. 
User Requirement:
{user_prompt}
The reagents you can use is:
{reagents}
Additional information you can refer to:
{manuals}
"""
+ f"\n{PGA_prompt_option_rpa}" + f"\n{PGA_prompt_intermediate_reagents}"
)

prompt_additional_requirement = PromptTemplate.from_template(
"""
---------------------
Additional Requirement:
It's compulsory to ensure {additional_requirement}
"""
)