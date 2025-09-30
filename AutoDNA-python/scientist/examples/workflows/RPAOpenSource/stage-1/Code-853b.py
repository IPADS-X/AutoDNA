from lab_modules import (
    container_manager,
    pipette,
    robot,
    capper,
    thermal_cycler,
    fluorometer,
    timer,
    Container,
    DEFAULT_CAPPER_LOCATION,
    DEFAULT_THERMAL_CYCLER_LOCATION,
    DEFAULT_CONTAINERHOLDER_LOCATION
)
from typing import List, Dict

def process_batch(
    sample_replicates: List[Container],
    reagent_buffer: Container,
    initiator: Container
) -> Dict[str, List[float]]:
    """
    Processes a batch of three replicates through the RPA protocol.
    This includes reagent addition, mixing, initiation, capping, and fluorescence reading.
    """
    # Step 1: Prepare reaction mixtures
    for tube in sample_replicates:
        # Add RPA Reagent Buffer
        pipette.transfer(42, reagent_buffer, tube)
        # Mix by pipetting
        pipette.mix(volume=23, location=tube, repetitions=5)

    # Step 2: Initiate reactions
    for tube in sample_replicates:
        pipette.transfer(3, initiator, tube)

    # Move tubes to capper and cap them
    for tube in sample_replicates:
        robot.move_container(tube, DEFAULT_CAPPER_LOCATION)
    capper.cap()
    for tube in sample_replicates:
        robot.move_container(tube, DEFAULT_CONTAINERHOLDER_LOCATION)

    # Step 3: Commence isothermal amplification and data acquisition
    batch_readings = {tube.label: [] for tube in sample_replicates}

    # Data Acquisition Schedule: 0, 5, 10, 15, 20, 25 minutes
    time_points_minutes = [0, 5, 10, 15, 20, 25]
    incubation_duration_seconds = 5 * 60

    # Read fluorescence at time 0
    for tube in sample_replicates:
        robot.move_container(tube, fluorometer.location)
    readings = fluorometer.measure_fluorescence()
    for i, tube in enumerate(sample_replicates):
        batch_readings[tube.label].append(readings[i])

    # Run incubation in 5-minute intervals and read fluorescence
    for _ in range(len(time_points_minutes) - 1):
        # Move from fluorometer to thermal cycler for incubation
        thermal_cycler.open_lid()
        for tube in sample_replicates:
            robot.move_container(tube, DEFAULT_THERMAL_CYCLER_LOCATION)
        thermal_cycler.close_lid()

        protocol = [{'temperature_celsius': 39.0, 'duration_seconds': incubation_duration_seconds}]
        thermal_cycler.run_protocol(protocol)
        
        # Move from thermal cycler to fluorometer for reading
        thermal_cycler.open_lid()
        for tube in sample_replicates:
            robot.move_container(tube, fluorometer.location)
        
        readings = fluorometer.measure_fluorescence()
        for i, tube in enumerate(sample_replicates):
            batch_readings[tube.label].append(readings[i])
            
    # After the loop, the tubes are in the fluorometer. Move them back to the holder.
    for tube in sample_replicates:
        robot.move_container(tube, DEFAULT_CONTAINERHOLDER_LOCATION)

    return batch_readings

def run_experiment():
    """
    Main function to execute the entire RPA experiment protocol.
    """
    # Inventory setup
    reagent_buffer = container_manager.getContainerForReplenish(
        "RPA Reagent Buffer", required_volume=42 * 24
    )
    reaction_initiator = container_manager.getContainerForReplenish(
        "Reaction initiator", required_volume=3 * 24
    )

    sample_types = ["NC", "NTC", "Sample1", "Sample2", "Sample3", "Sample4", "Sample5", "Sample6"]
    
    all_sample_tubes = {}
    for sample_type in sample_types:
        replicates = []
        for i in range(1, 4):
            label = f"{sample_type}-{i}"
            # Create a container representing the pre-aliquoted 5uL sample tube
            tube = container_manager.getContainerForReplenish(name=label, required_volume=5)
            tube.volume = 5.0
            replicates.append(tube)
        all_sample_tubes[sample_type] = replicates

    # Run protocol for all batches
    all_results = {}
    for sample_type in sample_types:
        replicates = all_sample_tubes[sample_type]
        results = process_batch(replicates, reagent_buffer, reaction_initiator)
        all_results[sample_type] = results

    # Part 9: Data Analysis and Interpretation
    
    # Step 9.1: Calculate Positivity Threshold from NTC batch
    ntc_results = all_results["NTC"]
    f_ntc1_25min = ntc_results["NTC-1"][-1]
    f_ntc2_25min = ntc_results["NTC-2"][-1]
    f_ntc3_25min = ntc_results["NTC-3"][-1]

    average_ntc_25min = (f_ntc1_25min + f_ntc2_25min + f_ntc3_25min) / 3
    positivity_threshold = average_ntc_25min * 3

    # Step 9.2: Classify all experimental samples
    classifications = {}
    sample_keys_to_classify = ["Sample1", "Sample2", "Sample3", "Sample4", "Sample5", "Sample6"]

    for sample_key in sample_keys_to_classify:
        sample_results = all_results[sample_key]
        classifications[sample_key] = {}
        for replicate_label, readings in sample_results.items():
            final_fluorescence = readings[-1]
            if final_fluorescence > positivity_threshold:
                classifications[sample_key][replicate_label] = "Positive"
            else:
                classifications[sample_key][replicate_label] = "Negative"
    
    # Final Output
    print(f"Positivity Threshold: {positivity_threshold:.2f}")
    print("\nSample Classifications:")
    for sample_key, replicate_classifications in classifications.items():
        print(f"  {sample_key}:")
        for replicate_label, result in replicate_classifications.items():
            print(f"    {replicate_label}: {result}")

# Execute the experiment
run_experiment()