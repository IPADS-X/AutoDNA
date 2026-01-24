from langchain_core.prompts import PromptTemplate

EPA_guidance_prompt = """
Upon an agent calling, you must TELL the user with detailed explanation of your reasoning for choosing it. You must give your reason on why you fill in the param of that agent the way you do( even you leave the param empty ). This reasoning part must be super detailed. 
If you want to pass information from one agent to another, you MUST fill in the file_id(or file_ids) param of the agent with the id of the file that contains the information you want to pass.
If you think it's time to stop, you must output in a format shown below:
### workflow ### 
(Summarize the workflow you have done)
### final_result ### 
(The final result of the executions)
If the execution fails, judge whether the error is caused by the current execution or if the root cause lies in previous stages. If you determine it is rooted in previous stages, you must stop. Otherwise, you should keep trying.
------------------------------------
A hypothesis must be considered invalidated if neither of its requested reagents is available.
"""  

EPA_enzymatic_synthesis_prompt = EPA_guidance_prompt

EPA_storage_prompt = """
You must decompose the target task into several stages. You must invoke the tools you have to conquer these stages ONE BY ONE. Solve one subtask at a time. Finally, you must integrate the results from each stage to form a complete solution.
""" 

judge_interruptibility_prompt = """
You are a configuration assistant.
Determine if the user specifically requests the experiment process to be "uninterruptible", "continuous", "without stopping", "atomic", or other similar terms.
Default behavior is interruptible.
If the user mentions anything implying the process should NOT be interrupted, output "NO".
Otherwise (default), output "YES".
Output ONLY "YES" or "NO".
----------------------------------------------
The user prompt:
"""

all_protocols_summary_prompt = PromptTemplate.from_template(
"""
{protocol_contents}
----------------------------------------------
Above is all the protocol executed. 
Now there are next steps that requires the result of the protcols above, you must summarize the Key Characteristics of the product that went through the protocols above. The answer must be concise without mentioning the initial input.
"""
)

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

plan_summarization_prompt = """
experiment_name: The goal of the experiment with a short description (no more than 10 words).
Generate the experiment_name parameter for the following experiment description(output only the parameter value):
------------------------------
"""