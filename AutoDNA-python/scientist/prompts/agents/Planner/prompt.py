EPA_guidance_prompt = """
Upon an agent calling, you must TELL the user with detailed explanation of your reasoning for choosing it. You must give your reason on why you fill in the param of that agent the way you do( even you leave the param empty ). This reasoning part must be super detailed. 
If you want to pass information from one agent to another, you MUST fill in the file_id(or file_ids) param of the agent with the id of the file that contains the information you want to pass.
If you think it's time to stop, you must output in a format shown below:
### workflow ### 
(Summarize the workflow you have done)
### final_result ### 
(The final result of the executions)
------------------------------------
A hypothesis must be considered invalidated if neither of its requested reagents is available.
"""  

EPA_enzymatic_synthesis_prompt = EPA_guidance_prompt

EPA_storage_prompt = """
You must decompose the target task into several stages. You must invoke the tools you have to conquer these stages ONE BY ONE. Solve one subtask at a time. Finally, you must integrate the results from each stage to form a complete solution.
""" 
comments = """
先跑，
做决策可以考虑一下建议：

看那一步出现偏差，分析原因：

e.g. You MUST check the availability of reagents before code generation.
"""