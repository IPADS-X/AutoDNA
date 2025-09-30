export const step7_EPA_chat = [
    {
        role: "assistant",
        agent: "EPA",
        content: `
I will use the \`Code\` agent to generate the Python script that will automate the execution of our validated experimental procedure.

My reasoning for selecting the \`Code\` agent is as follows:
- With a finalized and validated protocol in hand (\`Protocol-6131\`), the next logical step toward implementation is to translate this procedure into a machine-executable format.
- The \`Code\` agent is specifically designed to take an experimental protocol as input and produce the corresponding Python code, making it the perfect tool for this stage of the workflow.

Here is the breakdown of the parameters I will use for the \`Code\` agent:
- \`code_request_id\`: I will set this to \`Protocol-6131\`, which is the file ID of our validated procedure. This ensures the generated code will accurately reflect the steps we've carefully defined.
- \`pure_software\`: I am leaving this with its default \`False\` value, as this experiment clearly involves interacting with physical lab hardware (the fluorescence reader).
- \`multiple_inputs\`: I am leaving this parameter empty. The protocol involves processing several different batches of samples, but the core procedure for each batch is identical. The code can handle the different sample names through a list or loop, so we don't need to define them as separate, distinct inputs at this stage.
- \`inputs_from_previous_stage\`: I am leaving this parameter empty as the protocol file itself contains all the necessary information for generating the code. There are no computational results from a prior step that need to be passed into this one.
`,   }
]

export const step7_EPA_step = "Planner Agent: Tool Selection   ";