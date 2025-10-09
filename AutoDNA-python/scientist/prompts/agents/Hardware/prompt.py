from langchain.prompts import PromptTemplate

hypothesis_prompt = '''
You are a helpful assistant that provides hypotheses explaining why a target is not being achieved.
Context:
    Last procedure executed:
    =====================WORKFLOW BEGIN=====================
    {last_workflow}
    =====================WORKFLOW END=====================
    Outdated optimization history (Do not generate similar hypotheses as below):
    {text}
    Optimization target (all hypotheses must be based on this):
    {optimize_target}
    IMPORTANT THINGS MUST BE NOTED:
    {note}
Response Requirements:
    1. Carefully consider multiple factors AS MUCH AS POSSIBLE (contains physical, chemical, biological, and other factors).
        Present hypotheses you have thought in a numbered list format.
        Avoid using similar hypotheses have been listed before.
    2. Choose 1 from above at last.
        You should give the hypotheses you think best
    Each entry must contain:
    a) One clear hypotheses for the unmet target
'''

        # Avoid using similar hypotheses have been listed before.


paper_prompt = '''
Give answer in a json format. Don't include any other information.
Keys:
    "is_valid" : "true" or "false", shows whether you can answer this question.
    "answers" : a list of answers, each answer is a string.
Notes:
    1. Carefully consider multiple suggestions AS MUCH AS POSSIBLE.
        When answers, list methods in list format.
    2. If can not answer, just return "is_valid" as "false" and "answers" as an empty list.
'''

self_answer_prompt = '''
You are a helpful assistant that provides a suggestion to improve the target based on the hypothesis.
Context:
    Last workflow executed (may contain irrelevant experiment steps):
    =====================WORKFLOW BEGIN=====================
    {last_workflow}
    =====================WORKFLOW END=====================
    Optimization target (all hypothesis must be based on this):
    {optimize_target}
    Hypothesis:
    {hypothesis}
Response Requirements:
    1. Generate a suggestion to improve the target based on the hypothesis.
    2. The suggestion should be a clear and concise action that only changes one thing in the workflow.
'''

    # Question:
    # {question}
    # Response Requirements:
        # 1. Generate a suggestion to improve the target based on the hypothesis and the question.
        # 2. The suggestion should be a clear and concise action that only changes one thing in the workflow.


    # 1. Carefully consider multiple suggestions AS MUCH AS POSSIBLE.
    #     Present suggestions you have thought in a numbered list format.
    #     Avoid using similar suggestions have been listed before.
    # 2. Generate the most **feasible and brute-force** suggestion to improve the target based on the hypothesis.
    #     this suggestion may be benefical to other aspects of the workflow.
    # 3. The suggestion should be a clear and concise action that only changes one thing in the workflow.


solution_prompt = PromptTemplate.from_template(
'''
You are a helpful assistant that provides solutions to improve the target based on the hypothesis.
Context:
    Last procedure executed :
    {last_procedure}
    The hypothesis is:
    {text}

Response Requirements:
1. Give your answers to the hypothesis above. You must provide an optimizing advice to improve the target based on the hypothesis. The advice must be a specific and actionable step that could be taken into original experiment procedure. You don't need to provide a modified procedure, just give the optimizing advice only.
2. The advice should be a clear and concise action that only changes one thing in the procedure.

Information you can refer to (may contain redundant information):
{context}
'''
)