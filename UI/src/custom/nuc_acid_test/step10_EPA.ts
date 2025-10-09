
export const step10_EPA_chat = [
    {
        role: "assistant",
        agent: "EPA",
        content: `
I will use the \`Hardware\` agent to execute the Python code for the experiment.
- **file_ids**: I will set this to \`["Code-853b"]\`. This is the file ID of the Python script generated in the previous step by the \`Code\` agent, which contains the detailed experimental procedure.
- **repeat_num**: I will set this to \`1\`. The prompt describes 8 batches to be tested sequentially, with each group in triplicate. The generated protocol already accounts for the triplicate testing within each batch, and the overall experiment is described as a single run through these 8 batches. Therefore, the entire experimental process, as encoded in \`Code-853b\`, needs to be executed once.
- **pure_software**: I will leave this parameter empty as this experiment involves physical reagents and fluorescence readings, indicating it is not purely software-based.
`,
    }
]

export const step10_EPA_step = "Planner Agent: Tool Selection    ";