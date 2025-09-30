from lab_modules import *

def synthesis_cycle(
    reaction_tube: Container,
    nucleotide_container: Container,
    reaction_buffer: Container,
    cocl2: Container,
    tdt: Container,
    h2o: Container,
    bw_1x_buffer: Container,
    cleavage_buffer: Container,
    waste_container: Container,
):
    """
    Performs one full cycle of nucleotide addition:
    1. Coupling Reaction
    2. Post-Coupling Wash
    3. Deblocking Reaction
    4. Post-Deblocking Wash
    """
    # Step 1.1: Coupling Reaction
    # The reaction tube arrives with a bead pellet, located on the magnetic rack.
    # Move tube to a position for liquid handling (e.g., shaker location).
    robot.move_container(reaction_tube, heater_shaker.get_location())

    # Add reagents for a 100 uL final volume reaction
    pipette.transfer(36, h2o, reaction_tube)
    pipette.transfer(10, reaction_buffer, reaction_tube)
    pipette.transfer(40, cocl2, reaction_tube)
    pipette.transfer(10, nucleotide_container, reaction_tube)
    pipette.transfer(4, tdt, reaction_tube)
    
    # Incubate at 30 C for 10 min with shaking
    heater_shaker.incubate(
        target_temperature_celsius=30,
        target_speed_rpm=1000,
        duration_seconds=600
    )

    # Step 1.2: Post-Coupling Wash
    robot.move_container(reaction_tube, mag_rack_200uL.get_location())
    mag_rack_200uL.separate(duration_seconds=120)
    pipette.transfer(100, reaction_tube, waste_container)
    
    for _ in range(3):
        robot.move_container(reaction_tube, heater_shaker.get_location())
        pipette.transfer(100, bw_1x_buffer, reaction_tube)
        heater_shaker.incubate(target_speed_rpm=1300, duration_seconds=30)
        robot.move_container(reaction_tube, mag_rack_200uL.get_location())
        mag_rack_200uL.separate(duration_seconds=120)
        pipette.transfer(100, reaction_tube, waste_container)

    # Step 1.3: Deblocking Reaction
    robot.move_container(reaction_tube, heater_shaker.get_location())
    pipette.transfer(100, cleavage_buffer, reaction_tube)
    # Incubate at room temperature (25 C) for 5 min with rotation
    heater_shaker.incubate(
        target_temperature_celsius=25,
        target_speed_rpm=1000,
        duration_seconds=300
    )

    # Step 1.4: Post-Deblocking Wash
    robot.move_container(reaction_tube, mag_rack_200uL.get_location())
    mag_rack_200uL.separate(duration_seconds=120)
    pipette.transfer(100, reaction_tube, waste_container)

    for _ in range(3):
        robot.move_container(reaction_tube, heater_shaker.get_location())
        pipette.transfer(100, bw_1x_buffer, reaction_tube)
        heater_shaker.incubate(target_speed_rpm=1300, duration_seconds=30)
        robot.move_container(reaction_tube, mag_rack_200uL.get_location())
        mag_rack_200uL.separate(duration_seconds=120)
        pipette.transfer(100, reaction_tube, waste_container)


def run_protocol():
    # --- Reagent and Container Setup ---
    # Get stock reagents from inventory
    beads_stock = container_manager.getContainerForReplenish("Initiator-immobilized magnetic beads")
    tdt_stock = container_manager.getContainerForReplenish("Terminal Deoxynucleotidyl Transferase")
    dATP_stock = container_manager.getContainerForReplenish("3'-ONH2-dATP")
    dCTP_stock = container_manager.getContainerForReplenish("3'-ONH2-dCTP")
    dGTP_stock = container_manager.getContainerForReplenish("3'-ONH2-dGTP")
    dTTP_stock = container_manager.getContainerForReplenish("3'-ONH2-dTTP")
    cocl2_stock = container_manager.getContainerForReplenish("Cobalt Chloride")
    reaction_buffer_10x = container_manager.getContainerForReplenish("Reaction Buffer")
    bw_buffer_2x = container_manager.getContainerForReplenish("B&W Buffer")
    cleavage_buffer_stock = container_manager.getContainerForReplenish("Cleavage/Deblocking Buffer")
    tween_stock = container_manager.getContainerForReplenish("Tween")
    h2o = container_manager.getContainerForReplenish("Nuclease-Free Water")

    # Create new containers for working solutions and waste
    waste_container = container_manager.newContainer("waste_container", cap=ContainerType.P50K)
    bw_1x_buffer = container_manager.newContainer("1x_bw_buffer_with_tween", cap=ContainerType.P50K)

    # Prepare 1X B&W Buffer with 0.1% Tween-20 (e.g., 5000 uL)
    # Final recipe: 1X B&W, 0.1% Tween
    # From stocks: 2X B&W, 1% Tween, H2O
    pipette.transfer(2500, bw_buffer_2x, bw_1x_buffer) # Dilutes 2X to 1X in 5000uL
    pipette.transfer(500, tween_stock, bw_1x_buffer)  # Dilutes 1% to 0.1% in 5000uL
    pipette.transfer(2000, h2o, bw_1x_buffer)          # Top up to 5000uL
    pipette.mix(1000, bw_1x_buffer, repetitions=10)

    # --- Bead Preparation ---
    synthesis_tube = container_manager.newContainer("synthesis_tube", cap=ContainerType.P200)
    
    # Dispense beads and remove storage buffer
    bead_volume = 10
    pipette.transfer(bead_volume, beads_stock, synthesis_tube)
    robot.move_container(synthesis_tube, mag_rack_200uL.get_location())
    mag_rack_200uL.separate(duration_seconds=120)
    pipette.transfer(bead_volume, synthesis_tube, waste_container)

    # --- Iterative Synthesis ---
    target_sequence = "ACTCTGAT"
    nucleotide_map = {
        'A': dATP_stock,
        'C': dCTP_stock,
        'T': dTTP_stock,
        'G': dGTP_stock
    }

    for nucleotide in target_sequence:
        current_nucleotide_container = nucleotide_map[nucleotide]
        synthesis_cycle(
            reaction_tube=synthesis_tube,
            nucleotide_container=current_nucleotide_container,
            reaction_buffer=reaction_buffer_10x,
            cocl2=cocl2_stock,
            tdt=tdt_stock,
            h2o=h2o,
            bw_1x_buffer=bw_1x_buffer,
            cleavage_buffer=cleavage_buffer_stock,
            waste_container=waste_container,
        )

    # --- Final Product Resuspension ---
    # After the last cycle, the beads are on the magnet. Resuspend in 50uL buffer.
    robot.move_container(synthesis_tube, heater_shaker.get_location())
    pipette.transfer(50, bw_1x_buffer, synthesis_tube)
    heater_shaker.incubate(target_speed_rpm=1300, duration_seconds=30)
    
    print(f"Final product in: {synthesis_tube.label}, Volume: {synthesis_tube.volume}uL")

# Run the entire protocol
run_protocol()