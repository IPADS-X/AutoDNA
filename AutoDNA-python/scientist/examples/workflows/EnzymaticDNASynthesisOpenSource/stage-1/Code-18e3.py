#
# Path Description: Path: Option 1.1.1, Option 1.2.1, Option 1.3.1, Option 1.4.1, Option 1.5.1
from lab_modules import (
    container_manager,
    pipette,
    robot,
    mag_rack_200uL,
    heater_shaker,
    Container,
    ContainerType,
    DEFAULT_CONTAINERHOLDER_LOCATION,
)

def wash_beads(
    bead_container: Container,
    wash_buffer: Container,
    waste_container: Container,
    repetitions: int,
    wash_volume: float,
):
    """
    Performs a specified number of wash cycles on magnetic beads.
    """
    for _ in range(repetitions):
        robot.move_container(bead_container, mag_rack_200uL.get_location())
        mag_rack_200uL.separate(duration_seconds=60)
        
        if bead_container.volume > 0:
            pipette.transfer(bead_container.volume, bead_container, waste_container, touch_tip=False)

        robot.move_container(bead_container, DEFAULT_CONTAINERHOLDER_LOCATION)
        pipette.transfer(wash_volume, wash_buffer, bead_container)
        pipette.mix(volume=wash_volume * 0.9, location=bead_container, repetitions=15)

def run_synthesis_protocol():
    """
    Executes the full iterative synthesis of the ACTCTGAT oligonucleotide.
    """
    # 1. Reagent and Container Setup
    h2o = container_manager.getContainerForReplenish("Nuclease-Free Water", required_volume=2000)
    b_and_w_buffer_2x = container_manager.getContainerForReplenish("B&W Buffer", required_volume=2000)
    tween_1_percent = container_manager.getContainerForReplenish("Tween", required_volume=400)
    cleavage_buffer = container_manager.getContainerForReplenish("Cleavage/Deblocking Buffer", required_volume=1000)
    reaction_buffer_10x = container_manager.getContainerForReplenish("Reaction Buffer", required_volume=100)
    cocl2 = container_manager.getContainerForReplenish("Cobalt Chloride", required_volume=400)
    tdt = container_manager.getContainerForReplenish("Terminal Deoxynucleotidyl Transferase", required_volume=50)

    nucleotide_reagents = {
        "A": container_manager.getContainerForReplenish("3'-ONH2-dATP", required_volume=40),
        "C": container_manager.getContainerForReplenish("3'-ONH2-dCTP", required_volume=10),
        "G": container_manager.getContainerForReplenish("3'-ONH2-dGTP", required_volume=10),
        "T": container_manager.getContainerForReplenish("3'-ONH2-dTTP", required_volume=20),
    }
    
    beads_container = container_manager.getContainerForReplenish("Initiator-immobilized magnetic beads", required_volume=100)
    waste_container = container_manager.newContainer(label="waste_container", cap=ContainerType.P50K)

    # 2. Prepare 1X B&W Buffer with Tween
    b_and_w_buffer_1x = container_manager.newContainer(label="1X_B&W_Tween_Buffer", cap=ContainerType.P50K)
    pipette.transfer(2000, b_and_w_buffer_2x, b_and_w_buffer_1x)
    pipette.transfer(400, tween_1_percent, b_and_w_buffer_1x)
    pipette.transfer(1600, h2o, b_and_w_buffer_1x)

    # 3. Initial Bead Preparation (Remove storage buffer)
    robot.move_container(beads_container, mag_rack_200uL.get_location())
    mag_rack_200uL.separate(duration_seconds=60)
    pipette.transfer(beads_container.volume, beads_container, waste_container)
    robot.move_container(beads_container, DEFAULT_CONTAINERHOLDER_LOCATION)

    # 4. Synthesis Loop
    sequence = "ACTCTGAT"
    coupling_reaction_volume = 100.0
    wash_volume = 100.0
    deblocking_volume = 100.0

    for i, nucleotide in enumerate(sequence):
        dntp_container = nucleotide_reagents[nucleotide]
        
        # Step 1.1: Coupling Reaction
        # Pellet beads from previous wash
        robot.move_container(beads_container, mag_rack_200uL.get_location())
        mag_rack_200uL.separate(duration_seconds=60)
        if beads_container.volume > 0:
             pipette.transfer(beads_container.volume, beads_container, waste_container)
        robot.move_container(beads_container, DEFAULT_CONTAINERHOLDER_LOCATION)
        
        # Prepare and add coupling mix
        coupling_mix_tube = container_manager.newContainer(label=f"coupling_mix_tube_{i+1}", cap=ContainerType.P200)
        pipette.transfer(36.0, h2o, coupling_mix_tube)
        pipette.transfer(10.0, reaction_buffer_10x, coupling_mix_tube)
        pipette.transfer(40.0, cocl2, coupling_mix_tube)
        pipette.transfer(4.0, tdt, coupling_mix_tube)
        pipette.transfer(10.0, dntp_container, coupling_mix_tube)
        
        pipette.transfer(coupling_reaction_volume, coupling_mix_tube, beads_container)
        
        # Incubate
        robot.move_container(beads_container, heater_shaker.get_location())
        heater_shaker.incubate(target_temperature_celsius=30, target_speed_rpm=1000, duration_seconds=300) # 5 min
        robot.move_container(beads_container, DEFAULT_CONTAINERHOLDER_LOCATION)

        # Step 1.2: Post-Coupling Wash
        wash_beads(beads_container, b_and_w_buffer_1x, waste_container, repetitions=2, wash_volume=wash_volume)
        
        # Step 1.3: Deblocking Reaction
        robot.move_container(beads_container, mag_rack_200uL.get_location())
        mag_rack_200uL.separate(duration_seconds=60)
        pipette.transfer(beads_container.volume, beads_container, waste_container)
        robot.move_container(beads_container, DEFAULT_CONTAINERHOLDER_LOCATION)
        
        pipette.transfer(deblocking_volume, cleavage_buffer, beads_container)
        
        # Incubate at room temp with rotation
        robot.move_container(beads_container, heater_shaker.get_location())
        heater_shaker.incubate(target_speed_rpm=1000, duration_seconds=300) # 5 min
        robot.move_container(beads_container, DEFAULT_CONTAINERHOLDER_LOCATION)
        
        # Step 1.4: Post-Deblocking Wash
        wash_beads(beads_container, b_and_w_buffer_1x, waste_container, repetitions=2, wash_volume=wash_volume)

    # 5. Final Product Preparation
    robot.move_container(beads_container, mag_rack_200uL.get_location())
    mag_rack_200uL.separate(duration_seconds=60)
    if beads_container.volume > 0:
        pipette.transfer(beads_container.volume, beads_container, waste_container)
    robot.move_container(beads_container, DEFAULT_CONTAINERHOLDER_LOCATION)
    
    # Resuspend in a final volume of wash buffer
    final_resuspension_volume = 50.0
    pipette.transfer(final_resuspension_volume, b_and_w_buffer_1x, beads_container)

    print(f"Synthesis of '{sequence}' complete.")
    print(f"Final product is in container: '{beads_container.label}' with volume {beads_container.volume} uL.")

run_synthesis_protocol()