#
# Path Description: Path: Option 1.1.1, Option 1.2.1, Option 1.3.1, Option 1.4.1, Option 1.5.1
from lab_modules import (
    container_manager,
    pipette,
    robot,
    heater_shaker,
    mag_rack_200uL,
    ContainerType,
    DEFAULT_CONTAINERHOLDER_LOCATION,
    Container
)

def wash_beads(
    reaction_tube: Container,
    wash_buffer: Container,
    waste_container: Container,
    supernatant_volume: float,
    wash_volume: float,
    repetitions: int
):
    """
    Performs magnetic separation and washing of beads.
    """
    # Initial separation and removal of supernatant
    robot.move_container(reaction_tube, mag_rack_200uL.get_location())
    mag_rack_200uL.separate(duration_seconds=60)
    pipette.transfer(supernatant_volume, reaction_tube, waste_container)
    robot.move_container(reaction_tube, DEFAULT_CONTAINERHOLDER_LOCATION)

    # Wash cycles
    for _ in range(repetitions):
        pipette.transfer(wash_volume, wash_buffer, reaction_tube)
        pipette.mix(volume=wash_volume * 0.9, location=reaction_tube, repetitions=10)
        robot.move_container(reaction_tube, mag_rack_200uL.get_location())
        mag_rack_200uL.separate(duration_seconds=60)
        pipette.transfer(wash_volume, reaction_tube, waste_container)
        robot.move_container(reaction_tube, DEFAULT_CONTAINERHOLDER_LOCATION)

def synthesis_cycle(
    reaction_tube: Container,
    dntp_container: Container,
    tdt_enzyme: Container,
    cocl2: Container,
    reaction_buffer: Container,
    water: Container,
    cleavage_buffer: Container,
    wash_buffer: Container,
    waste_container: Container
):
    """
    Performs one full cycle of nucleotide synthesis: coupling, washing, deblocking, washing.
    """
    # Constants for this cycle
    REACTION_VOLUME = 100.0
    WASH_VOLUME = 200.0
    DEBLOCK_VOLUME = 100.0

    # Step 1.1: Coupling Reaction
    pipette.transfer(volume=36.0, source=water, destination=reaction_tube)
    pipette.transfer(volume=10.0, source=reaction_buffer, destination=reaction_tube)
    pipette.transfer(volume=40.0, source=cocl2, destination=reaction_tube)
    pipette.transfer(volume=10.0, source=dntp_container, destination=reaction_tube)
    pipette.transfer(volume=4.0, source=tdt_enzyme, destination=reaction_tube, reuse_tip=False)
    pipette.mix(volume=80.0, location=reaction_tube, repetitions=15)
    
    robot.move_container(reaction_tube, heater_shaker.get_location())
    heater_shaker.incubate(
        target_temperature_celsius=30.0,
        duration_seconds=10 * 60,
        target_speed_rpm=1000  # Gentle rotation
    )
    robot.move_container(reaction_tube, DEFAULT_CONTAINERHOLDER_LOCATION)

    # Step 1.2: Post-Coupling Wash
    wash_beads(reaction_tube, wash_buffer, waste_container, REACTION_VOLUME, WASH_VOLUME, 3)

    # Step 1.3: Deblocking Reaction
    pipette.transfer(volume=DEBLOCK_VOLUME, source=cleavage_buffer, destination=reaction_tube)
    pipette.mix(volume=90.0, location=reaction_tube, repetitions=15)
    
    robot.move_container(reaction_tube, heater_shaker.get_location())
    heater_shaker.incubate(
        duration_seconds=5 * 60,
        target_speed_rpm=1000  # Room temp, gentle rotation
    )
    robot.move_container(reaction_tube, DEFAULT_CONTAINERHOLDER_LOCATION)

    # Step 1.4: Post-Deblocking Wash
    wash_beads(reaction_tube, wash_buffer, waste_container, DEBLOCK_VOLUME, WASH_VOLUME, 3)


def run_protocol():
    # --- Reagent Setup ---
    water = container_manager.getContainerForReplenish("Nuclease-Free Water")
    tdt_enzyme = container_manager.getContainerForReplenish("Terminal Deoxynucleotidyl Transferase")
    cocl2 = container_manager.getContainerForReplenish("Cobalt Chloride")
    reaction_buffer = container_manager.getContainerForReplenish("Reaction Buffer")
    cleavage_buffer = container_manager.getContainerForReplenish("Cleavage/Deblocking Buffer")
    bw_buffer_stock = container_manager.getContainerForReplenish("B&W Buffer")
    tween = container_manager.getContainerForReplenish("Tween")

    dATP = container_manager.getContainerForReplenish("3'-ONH2-dATP")
    dCTP = container_manager.getContainerForReplenish("3'-ONH2-dCTP")
    dGTP = container_manager.getContainerForReplenish("3'-ONH2-dGTP")
    dTTP = container_manager.getContainerForReplenish("3'-ONH2-dTTP")

    nucleotide_map = {
        'A': dATP,
        'C': dCTP,
        'T': dTTP,
        'G': dGTP
    }

    # --- Working Containers Setup ---
    # Prepare 1X B&W Wash Buffer with 0.1% Tween-20
    bw_wash_buffer = container_manager.newContainer("1X B&W Wash Buffer", cap=ContainerType.P50K)
    total_wash_buffer_vol = 10000.0
    pipette.transfer(volume=total_wash_buffer_vol / 2, source=bw_buffer_stock, destination=bw_wash_buffer) # 2X -> 1X
    pipette.transfer(volume=(0.1 / 1.0) * total_wash_buffer_vol, source=tween, destination=bw_wash_buffer) # 1% -> 0.1%
    pipette.transfer(volume=total_wash_buffer_vol - (total_wash_buffer_vol / 2) - ((0.1 / 1.0) * total_wash_buffer_vol), source=water, destination=bw_wash_buffer)

    # Prepare reaction tube with beads
    # Assuming beads come in 100uL storage buffer
    initial_bead_volume = 100.0
    reaction_tube = container_manager.getContainerForReplenish("Initiator-immobilized magnetic beads", required_volume=initial_bead_volume)
    
    waste_container = container_manager.newContainer("Liquid Waste", cap=ContainerType.P50K)

    # Initial wash to remove storage buffer
    robot.move_container(reaction_tube, mag_rack_200uL.get_location())
    mag_rack_200uL.separate(duration_seconds=60)
    pipette.transfer(initial_bead_volume, reaction_tube, waste_container)
    robot.move_container(reaction_tube, DEFAULT_CONTAINERHOLDER_LOCATION)

    # --- Synthesis Execution ---
    sequence = "ACTCTGAT"
    
    for nucleotide_char in sequence:
        dntp_container = nucleotide_map[nucleotide_char]
        synthesis_cycle(
            reaction_tube,
            dntp_container,
            tdt_enzyme,
            cocl2,
            reaction_buffer,
            water,
            cleavage_buffer,
            bw_wash_buffer,
            waste_container
        )
    
    # Final product is in reaction_tube
    # Resuspend in 100uL of water for storage
    pipette.transfer(100, water, reaction_tube)
    pipette.mix(90, reaction_tube, repetitions=10)

    print(f"Final Synthesized Product: {reaction_tube.label}, Volume: {reaction_tube.volume}uL")
    print(f"Waste Container: {waste_container.label}, Volume: {waste_container.volume}uL")

# Run the entire protocol
run_protocol()