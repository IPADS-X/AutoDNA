
export const step0_EPA_chat = [
    {
        role: "user",
        content: "Design and implement a nucleic acid test experiment based on RPA isothermal amplification. The experiment requires a total of six fluorescence readings per sample, taking one reading every 5 minutes, with the initial reading starting at time zero. A sample is considered positive if its fluorescence value exceeds three times the fluorescence value of the NTC (No Template Control).\nThis experiment involves 8 batches to be individually tested in a sequential manner, with each group prepared in triplicate that requires simultaneous testing. The batches are: \n- Negative Control: NC-1, NC-2, NC-3\n- No Template Control: NTC-1, NTC-2, NTC-3\n- Sample1: Sample1-1, Sample1-2, Sample1-3\n- Sample2: Sample2-1, Sample2-2, Sample2-3\n- Sample3: Sample3-1, Sample3-2, Sample3-3\n- Sample4: Sample4-1, Sample4-2, Sample4-3\n- Sample5: Sample5-1, Sample5-2, Sample5-3\n- Sample6: Sample6-1, Sample6-2, Sample6-3\nEach test in the batches comprises 3 individual pre-made components: a sample replicate, its RPA reagent buffer, and its Reaction Initiator, stored in uncapped and low-retention tubes initially.",
    },
    {
        role: "assistant",
        agent: "EPA",
        content: `
I have analyzed the user's request and generated the following task analysis:
**Experiment name:** RPA isothermal amplification nucleic acid test
**Complexity:** Simple
`,
    }
]

export const step0_EPA_step = "Planner Agent: Task Analysis";