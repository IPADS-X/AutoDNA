
export const step1_EPA_chat = [
    {
        role: "assistant",
        agent: "EPA",
        content: `
I will use the \`Protocol\` agent to design a detailed experimental procedure for the nucleic acid test.

My reasoning for selecting the \`Protocol\` agent is as follows:
- The user's request is to "Design and implement a nucleic acid test experiment," which directly aligns with the capabilities of the \`Protocol\` agent to generate experimental procedures.
- A well-defined protocol is essential before proceeding to code generation and execution, and this agent is the perfect first step to lay out the experimental plan.

Here is the breakdown of the parameters I will use for the \`Protocol\` agent:
- \`experiment_name\`: I will set this to "Nucleic acid test based on RPA isothermal amplification" to accurately reflect the user's objective.
- \`pure_software\`: I am leaving this parameter with its default value as the experiment involves hardware.
- \`retry\`: I am leaving this parameter with its default value as this is the first attempt and there is no failed procedure to retry.
- \`file_ids\`: I am leaving this parameter empty as there are no previous documents or procedures to reference for this initial design.
- \`manuals\`: I am leaving this parameter empty as no specific instrument or reagent manuals were provided by the user.     
`        ,
    }
]

export const step1_EPA_step = "Planner Agent: Tool Selection";