export const step5_EPA_chat = [
    {
        role: "assistant",
        agent: "EPA",
        content: `
I will use the \`Protocol\` agent to adjust the experimental procedure based on the information gathered from the \`Reagent\` agent.

My reasoning for selecting the \`Protocol\` agent is as follows:
- The previous tool output from the \`Reagent\` agent revealed a critical piece of information: "Magnesium Acetate Solution" is not available as a separate component, but the "Reaction Initiator," which contains it, is available. This means the Reaction Initiator is a pre-made solution.
- The initial protocol listed "Magnesium Acetate Solution" as a sub-component of the Reaction Initiator, which could imply it needs to be mixed. To avoid confusion and ensure the procedure is executable with the available materials, I need to refine the protocol to clarify that the Reaction Initiator is a single, pre-aliquoted solution.
- The \`Protocol\` agent's "Procedure Refinement" capability is perfectly suited for this task.

Here is the breakdown of the parameters I will use for the \`Protocol\` agent:
- \`experiment_name\`: I will keep this as "Nucleic acid test based on RPA isothermal amplification" to maintain consistency with the experimental goal.
- \`file_ids\`: I will provide the file IDs of both the original protocol (\`Protocol-d12e\`) and the reagent check result (\`Reagent-23a5\`). This is crucial because it gives the \`Protocol\` agent the full context: the original plan and the inventory constraints that necessitate the adjustment. By comparing the two, the agent can understand that the reference to the sub-component "Magnesium Acetate Solution" should be removed in favor of the available pre-mixed "Reaction Initiator".
- \`manuals\`: I am leaving this parameter empty as no specific manuals were provided.
- \`pure_software\`: I am leaving this parameter with its default value.
- \`retry\`: I am leaving this parameter with its default value.
`,
    }
]

export const step5_EPA_step = "Planner Agent: Tool Selection ";