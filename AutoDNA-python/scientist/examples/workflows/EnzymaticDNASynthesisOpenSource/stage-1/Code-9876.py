from lab_modules import (
    container_manager,
    pipette,
    robot,
    heater_shaker,
    mag_rack_200uL,
    Container,
    ContainerType,
    DEFAULT_CONTAINERHOLDER_LOCATION
)

def perform_wash(reaction_tube: Container, wash_buffer: Container, waste_container: Container, wash_cycles: int):
    """
    Performs magnetic bead separation and washing cycles.
    """
    # Remove supernatant from the previous step
    robot.move_container(reaction_tube, mag_rack_200uL.get_location())
    mag_rack_200uL.separate(duration_seconds=120)
    if reaction_tube.volume > 0:
        pipette.transfer(reaction_tube.volume, reaction_tube, waste_container)
    robot.move_container(reaction_tube, DEFAULT_CONTAINERHOLDER_LOCATION)

    # Perform wash cycles
    for _ in range(wash_cycles):
        # Resuspend
        pipette.transfer(100, wash_buffer, reaction_tube)
        pipette.mix(75, reaction_tube, repetitions=10)

        # Separate and discard
        robot.move_container(reaction_tube, mag_rack_200uL.get_location())
        mag_rack_200uL.separate(duration_seconds=120)
        pipette.transfer(reaction_tube.volume, reaction_tube, waste_container)
        robot.move_container(reaction_tube, DEFAULT_CONTAINERHOLDER_LOCATION)

def run_synthesis_protocol():
    """
    Executes the iterative synthesis of the DNA sequence ACTCTGAT.
    """
    # 1. Reagent and Labware Setup
    h2o = container_manager.getContainerForReplenish("Nuclease-Free Water")
    cocl2 = container_manager.getContainerForReplenish("Cobalt Chloride")
    tdt_enzyme = container_manager.getContainerForReplenish("Terminal Deoxynucleotidyl Transferase")
    reaction_buffer_10x = container_manager.getContainerForReplenish("Reaction Buffer")
    b_and_w_buffer_2x = container_manager.getContainerForReplenish("B&W Buffer")
    cleavage_buffer = container_manager.getContainerForReplenish("Cleavage/Deblocking Buffer")

    nucleotide_reagents = {
        'A': container_manager.getContainerForReplenish("3'-ONH2-dATP"),
        'C': container_manager.getContainerForReplenish("3'-ONH2-dCTP"),
        'G': container_manager.getContainerForReplenish("3'-ONH2-dGTP"),
        'T': container_manager.getContainerForReplenish("3'-ONH2-dTTP"),
    }

    beads_stock = container_manager.getContainerForReplenish("Initiator-immobilized magnetic beads")
    waste_container = container_manager.newContainer("waste_container", cap=ContainerType.P50K)
    bw_buffer_1x = container_manager.newContainer("1X B&W Buffer", cap=ContainerType.P50K)
    
    # Prepare 1X B&W Buffer
    pipette.transfer(2500, b_and_w_buffer_2x, bw_buffer_1x)
    pipette.transfer(2500, h2o, bw_buffer_1x)
    pipette.mix(1000, bw_buffer_1x, repetitions=10)

    # 2. Prepare Beads for Synthesis
    reaction_tube = beads_stock
    
    # Remove storage buffer
    robot.move_container(reaction_tube, mag_rack_200uL.get_location())
    mag_rack_200uL.separate(duration_seconds=120)
    if reaction_tube.volume > 0:
        pipette.transfer(reaction_tube.volume, reaction_tube, waste_container)
    robot.move_container(reaction_tube, DEFAULT_CONTAINERHOLDER_LOCATION)
    
    # 3. Iterative Synthesis Cycles
    target_sequence = "ACTCTGAT"

    for nucleotide_char in target_sequence:
        current_nucleotide = nucleotide_reagents[nucleotide_char]

        # Step 1.1: Coupling Reaction
        # Add reagents to the bead pellet for a 100 uL final volume
        pipette.transfer(43, h2o, reaction_tube)
        pipette.transfer(10, reaction_buffer_10x, reaction_tube)
        pipette.transfer(40, cocl2, reaction_tube)
        pipette.transfer(5, current_nucleotide, reaction_tube)
        pipette.transfer(2, tdt_enzyme, reaction_tube)
        
        # Mix to resuspend beads and incubate
        pipette.mix(75, reaction_tube, repetitions=10)
        robot.move_container(reaction_tube, heater_shaker.get_location())
        heater_shaker.incubate(
            target_temperature_celsius=30,
            target_speed_rpm=800, # Gentle rotation
            duration_seconds=600  # 10 minutes
        )
        robot.move_container(reaction_tube, DEFAULT_CONTAINERHOLDER_LOCATION)

        # Step 1.2: Post-Coupling Wash
        perform_wash(reaction_tube, bw_buffer_1x, waste_container, wash_cycles=3)

        # Step 1.3: Deblocking Reaction
        pipette.transfer(100, cleavage_buffer, reaction_tube)
        pipette.mix(75, reaction_tube, repetitions=10)
        robot.move_container(reaction_tube, heater_shaker.get_location())
        heater_shaker.incubate(
            target_speed_rpm=800, # Gentle rotation at room temp
            duration_seconds=300 # 5 minutes
        )
        robot.move_container(reaction_tube, DEFAULT_CONTAINERHOLDER_LOCATION)

        # Step 1.4: Post-Deblocking Wash
        perform_wash(reaction_tube, bw_buffer_1x, waste_container, wash_cycles=3)

    # Final product is in reaction_tube as a bead pellet.
    # For completion, resuspend in a storage buffer.
    pipette.transfer(50, bw_buffer_1x, reaction_tube)
    pipette.mix(30, reaction_tube, repetitions=10)
    
    print(f"Final Product Container: {reaction_tube.label}, Final Volume: {reaction_tube.volume} uL")

if __name__ == "__main__":
    run_synthesis_protocol()