export const step6_PGA_chat = [
    {
        role: "assistant",
        agent: "PGA",
        content: `
Part 1: Negative Control (NC) Batch Processing

    Step 1.1: Prepare the three Negative Control (NC) reaction mixtures by combining each NC replicate with its corresponding RPA reagent buffer. For each of the three replicates (NC-1, NC-2, NC-3), transfer the RPA reagent buffer into the tube containing the sample. Mix thoroughly by gentle pipetting.

        Option 1.1.1: Composition for a single Negative Control reaction mix prior to initiation.
            - Component A: Negative Control Sample (NC-1, NC-2, or NC-3)
            - Component B: RPA Reagent Buffer
            (operational details: Use 5uL of Negative Control Sample and 42uL of RPA Reagent Buffer per reaction.)

    Step 1.2: Initiate the three NC reactions. Dispense the Reaction Initiator into each of the three prepared NC reaction mixture tubes. Cap the tubes immediately after addition.

        Option 1.2.1: Composition of the Reaction Initiator.
            - Component A: Reaction Initiator
                - Recipe
                - Magnesium Acetate Solution
            (operational details: Use 3uL of Reaction Initiator per reaction.)

    Step 1.3: Commence isothermal amplification and data acquisition for the NC batch. Immediately load the three capped tubes into the fluorescence reader and start the pre-defined instrument protocol.

        Option 1.3.1: Instrument Protocol Settings.
            - Parameter A: Incubation Temperature: 39 °C
            - Parameter B: Total Run Time: 25 minutes
            - Parameter C: Data Acquisition Schedule: Read fluorescence at 0, 5, 10, 15, 20, and 25 minutes.
            - Parameter D: Fluorescence Channel: Set to the appropriate channel for the probe's fluorophore.

Part 2: No Template Control (NTC) Batch Processing

    Step 2.1: Prepare the three No Template Control (NTC) reaction mixtures by combining each NTC replicate with its corresponding RPA reagent buffer. For each of the three replicates (NTC-1, NTC-2, NTC-3), transfer the RPA reagent buffer into the tube containing the sample. Mix thoroughly by gentle pipetting.

        Option 2.1.1: Composition for a single No Template Control reaction mix prior to initiation.
            - Component A: No Template Control Sample (NTC-1, NTC-2, or NTC-3)
                - Recipe
                - Nuclease-Free Water
            - Component B: RPA Reagent Buffer
            (operational details: Use 5uL of No Template Control Sample and 42uL of RPA Reagent Buffer per reaction.)

    Step 2.2: Initiate the three NTC reactions. Dispense the Reaction Initiator into each of the three prepared NTC reaction mixture tubes. Cap the tubes immediately after addition.

        Option 2.2.1: Composition of the Reaction Initiator.
            - Component A: Reaction Initiator
                - Recipe
                - Magnesium Acetate Solution
            (operational details: Use 3uL of Reaction Initiator per reaction.)

    Step 2.3: Commence isothermal amplification and data acquisition for the NTC batch. Immediately load the three capped tubes into the fluorescence reader and start the pre-defined instrument protocol.

        Option 2.3.1: Instrument Protocol Settings.
            - Parameter A: Incubation Temperature: 39 °C
            - Parameter B: Total Run Time: 25 minutes
            - Parameter C: Data Acquisition Schedule: Read fluorescence at 0, 5, 10, 15, 20, and 25 minutes.
            - Parameter D: Fluorescence Channel: Set to the appropriate channel for the probe's fluorophore.

Part 3: Sample1 Batch Processing

    Step 3.1: Prepare the three Sample1 reaction mixtures by combining each Sample1 replicate with its corresponding RPA reagent buffer. For each of the three replicates (Sample1-1, Sample1-2, Sample1-3), transfer the RPA reagent buffer into the tube containing the sample. Mix thoroughly by gentle pipetting.

        Option 3.1.1: Composition for a single Sample1 reaction mix prior to initiation.
            - Component A: Sample1 Replicate (Sample1-1, Sample1-2, or Sample1-3)
            - Component B: RPA Reagent Buffer
            (operational details: Use 5uL of Sample1 Replicate and 42uL of RPA Reagent Buffer per reaction.)

    Step 3.2: Initiate the three Sample1 reactions. Dispense the Reaction Initiator into each of the three prepared Sample1 reaction mixture tubes. Cap the tubes immediately after addition.

        Option 3.2.1: Composition of the Reaction Initiator.
            - Component A: Reaction Initiator
                - Recipe
                - Magnesium Acetate Solution
            (operational details: Use 3uL of Reaction Initiator per reaction.)

    Step 3.3: Commence isothermal amplification and data acquisition for the Sample1 batch. Immediately load the three capped tubes into the fluorescence reader and start the pre-defined instrument protocol.

        Option 3.3.1: Instrument Protocol Settings.
            - Parameter A: Incubation Temperature: 39 °C
            - Parameter B: Total Run Time: 25 minutes
            - Parameter C: Data Acquisition Schedule: Read fluorescence at 0, 5, 10, 15, 20, and 25 minutes.
            - Parameter D: Fluorescence Channel: Set to the appropriate channel for the probe's fluorophore.

Part 4: Sample2 Batch Processing

    Step 4.1: Prepare the three Sample2 reaction mixtures by combining each Sample2 replicate with its corresponding RPA reagent buffer. For each of the three replicates (Sample2-1, Sample2-2, Sample2-3), transfer the RPA reagent buffer into the tube containing the sample. Mix thoroughly by gentle pipetting.

        Option 4.1.1: Composition for a single Sample2 reaction mix prior to initiation.
            - Component A: Sample2 Replicate (Sample2-1, Sample2-2, or Sample2-3)
            - Component B: RPA Reagent Buffer
            (operational details: Use 5uL of Sample2 Replicate and 42uL of RPA Reagent Buffer per reaction.)

    Step 4.2: Initiate the three Sample2 reactions. Dispense the Reaction Initiator into each of the three prepared Sample2 reaction mixture tubes. Cap the tubes immediately after addition.

        Option 4.2.1: Composition of the Reaction Initiator.
            - Component A: Reaction Initiator
                - Recipe
                - Magnesium Acetate Solution
            (operational details: Use 3uL of Reaction Initiator per reaction.)

    Step 4.3: Commence isothermal amplification and data acquisition for the Sample2 batch. Immediately load the three capped tubes into the fluorescence reader and start the pre-defined instrument protocol.

        Option 4.3.1: Instrument Protocol Settings.
            - Parameter A: Incubation Temperature: 39 °C
            - Parameter B: Total Run Time: 25 minutes
            - Parameter C: Data Acquisition Schedule: Read fluorescence at 0, 5, 10, 15, 20, and 25 minutes.
            - Parameter D: Fluorescence Channel: Set to the appropriate channel for the probe's fluorophore.

Part 5: Sample3 Batch Processing

    Step 5.1: Prepare the three Sample3 reaction mixtures by combining each Sample3 replicate with its corresponding RPA reagent buffer. For each of the three replicates (Sample3-1, Sample3-2, Sample3-3), transfer the RPA reagent buffer into the tube containing the sample. Mix thoroughly by gentle pipetting.

        Option 5.1.1: Composition for a single Sample3 reaction mix prior to initiation.
            - Component A: Sample3 Replicate (Sample3-1, Sample3-2, or Sample3-3)
            - Component B: RPA Reagent Buffer
            (operational details: Use 5uL of Sample3 Replicate and 42uL of RPA Reagent Buffer per reaction.)

    Step 5.2: Initiate the three Sample3 reactions. Dispense the Reaction Initiator into each of the three prepared Sample3 reaction mixture tubes. Cap the tubes immediately after addition.

        Option 5.2.1: Composition of the Reaction Initiator.
            - Component A: Reaction Initiator
                - Recipe
                - Magnesium Acetate Solution
            (operational details: Use 3uL of Reaction Initiator per reaction.)

    Step 5.3: Commence isothermal amplification and data acquisition for the Sample3 batch. Immediately load the three capped tubes into the fluorescence reader and start the pre-defined instrument protocol.

        Option 5.3.1: Instrument Protocol Settings.
            - Parameter A: Incubation Temperature: 39 °C
            - Parameter B: Total Run Time: 25 minutes
            - Parameter C: Data Acquisition Schedule: Read fluorescence at 0, 5, 10, 15, 20, and 25 minutes.
            - Parameter D: Fluorescence Channel: Set to the appropriate channel for the probe's fluorophore.

Part 6: Sample4 Batch Processing

    Step 6.1: Prepare the three Sample4 reaction mixtures by combining each Sample4 replicate with its corresponding RPA reagent buffer. For each of the three replicates (Sample4-1, Sample4-2, Sample4-3), transfer the RPA reagent buffer into the tube containing the sample. Mix thoroughly by gentle pipetting.

        Option 6.1.1: Composition for a single Sample4 reaction mix prior to initiation.
            - Component A: Sample4 Replicate (Sample4-1, Sample4-2, or Sample4-3)
            - Component B: RPA Reagent Buffer
            (operational details: Use 5uL of Sample4 Replicate and 42uL of RPA Reagent Buffer per reaction.)

    Step 6.2: Initiate the three Sample4 reactions. Dispense the Reaction Initiator into each of the three prepared Sample4 reaction mixture tubes. Cap the tubes immediately after addition.

        Option 6.2.1: Composition of the Reaction Initiator.
            - Component A: Reaction Initiator
                - Recipe
                - Magnesium Acetate Solution
            (operational details: Use 3uL of Reaction Initiator per reaction.)

    Step 6.3: Commence isothermal amplification and data acquisition for the Sample4 batch. Immediately load the three capped tubes into the fluorescence reader and start the pre-defined instrument protocol.

        Option 6.3.1: Instrument Protocol Settings.
            - Parameter A: Incubation Temperature: 39 °C
            - Parameter B: Total Run Time: 25 minutes
            - Parameter C: Data Acquisition Schedule: Read fluorescence at 0, 5, 10, 15, 20, and 25 minutes.
            - Parameter D: Fluorescence Channel: Set to the appropriate channel for the probe's fluorophore.

Part 7: Sample5 Batch Processing

    Step 7.1: Prepare the three Sample5 reaction mixtures by combining each Sample5 replicate with its corresponding RPA reagent buffer. For each of the three replicates (Sample5-1, Sample5-2, Sample5-3), transfer the RPA reagent buffer into the tube containing the sample. Mix thoroughly by gentle pipetting.

        Option 7.1.1: Composition for a single Sample5 reaction mix prior to initiation.
            - Component A: Sample5 Replicate (Sample5-1, Sample5-2, or Sample5-3)
            - Component B: RPA Reagent Buffer
            (operational details: Use 5uL of Sample5 Replicate and 42uL of RPA Reagent Buffer per reaction.)

    Step 7.2: Initiate the three Sample5 reactions. Dispense the Reaction Initiator into each of the three prepared Sample5 reaction mixture tubes. Cap the tubes immediately after addition.

        Option 7.2.1: Composition of the Reaction Initiator.
            - Component A: Reaction Initiator
                - Recipe
                - Magnesium Acetate Solution
            (operational details: Use 3uL of Reaction Initiator per reaction.)

    Step 7.3: Commence isothermal amplification and data acquisition for the Sample5 batch. Immediately load the three capped tubes into the fluorescence reader and start the pre-defined instrument protocol.

        Option 7.3.1: Instrument Protocol Settings.
            - Parameter A: Incubation Temperature: 39 °C
            - Parameter B: Total Run Time: 25 minutes
            - Parameter C: Data Acquisition Schedule: Read fluorescence at 0, 5, 10, 15, 20, and 25 minutes.
            - Parameter D: Fluorescence Channel: Set to the appropriate channel for the probe's fluorophore.

Part 8: Sample6 Batch Processing

    Step 8.1: Prepare the three Sample6 reaction mixtures by combining each Sample6 replicate with its corresponding RPA reagent buffer. For each of the three replicates (Sample6-1, Sample6-2, Sample6-3), transfer the RPA reagent buffer into the tube containing the sample. Mix thoroughly by gentle pipetting.

        Option 8.1.1: Composition for a single Sample6 reaction mix prior to initiation.
            - Component A: Sample6 Replicate (Sample6-1, Sample6-2, or Sample6-3)
            - Component B: RPA Reagent Buffer
            (operational details: Use 5uL of Sample6 Replicate and 42uL of RPA Reagent Buffer per reaction.)

    Step 8.2: Initiate the three Sample6 reactions. Dispense the Reaction Initiator into each of the three prepared Sample6 reaction mixture tubes. Cap the tubes immediately after addition.

        Option 8.2.1: Composition of the Reaction Initiator.
            - Component A: Reaction Initiator
                - Recipe
                - Magnesium Acetate Solution
            (operational details: Use 3uL of Reaction Initiator per reaction.)

    Step 8.3: Commence isothermal amplification and data acquisition for the Sample6 batch. Immediately load the three capped tubes into the fluorescence reader and start the pre-defined instrument protocol.

        Option 8.3.1: Instrument Protocol Settings.
            - Parameter A: Incubation Temperature: 39 °C
            - Parameter B: Total Run Time: 25 minutes
            - Parameter C: Data Acquisition Schedule: Read fluorescence at 0, 5, 10, 15, 20, and 25 minutes.
            - Parameter D: Fluorescence Channel: Set to the appropriate channel for the probe's fluorophore.

Part 9: Data Analysis and Interpretation

    Step 9.1: Calculate the positivity threshold from the No Template Control (NTC) batch data. Average the fluorescence readings of the three NTC replicates (NTC-1, NTC-2, NTC-3) at the final time point (25 minutes). Multiply this average value by three.

        Option 9.1.1: Formula for Positivity Threshold Calculation.
            - Variable A: F_NTC1_25min = Fluorescence of NTC-1 at 25 minutes.
            - Variable B: F_NTC2_25min = Fluorescence of NTC-2 at 25 minutes.
            - Variable C: F_NTC3_25min = Fluorescence of NTC-3 at 25 minutes.
            - Calculation: Threshold = ( (F_NTC1_25min + F_NTC2_25min + F_NTC3_25min) / 3 ) * 3

    Step 9.2: Classify all experimental samples (Sample1 to Sample6). For each sample replicate, compare its fluorescence value at the 25-minute time point to the calculated positivity threshold.

        Option 9.2.1: Classification Criteria.
            - Condition A: If a sample replicate's 25-minute fluorescence value is greater than the Positivity Threshold, classify the replicate as 'Positive'.
            - Condition B: If a sample replicate's 25-minute fluorescence value is less than or equal to the Positivity Threshold, classify the replicate as 'Negative'.
 The validated procedure has been saved to the file: Protocol-6131
`        }
]

export const step6_PGA_step = "Protocol Agent: Procedure Revision";