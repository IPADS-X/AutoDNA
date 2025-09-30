from lab_modules import (
    container_manager,
    pipette,
    robot,
    heater_shaker,
    mag_rack_200uL,
    Container,
    ContainerType,
    DEFAULT_CONTAINERHOLDER_LOCATION,
)

def run_protocol():
    # Define constants for reaction volumes
    REACTION_VOLUME = 100.0
    WASH_VOLUME = 100.0
    DEBLOCKING_VOLUME = 100.0
    BEAD_VOLUME = 10.0

    # Get reagents from inventory
    h2o = container_manager.getContainerForReplenish("Nuclease-Free Water")
    cocl2 = container_manager.getContainerForReplenish("Cobalt Chloride")
    reaction_buffer = container_manager.getContainerForReplenish("Reaction Buffer")
    tdt_enzyme = container_manager.getContainerForReplenish("Terminal Deoxynucleotidyl Transferase")
    beads_stock = container_manager.getContainerForReplenish("Initiator-immobilized magnetic beads")
    deblocking_reagent = container_manager.getContainerForReplenish("Cleavage/Deblocking Buffer")
    bw_buffer_stock = container_manager.getContainerForReplenish("B&W Buffer")
    tween = container_manager.getContainerForReplenish("Tween")
    
    # Get nucleotide reagents
    datp = container_manager.getContainerForReplenish("3'-ONH2-dATP")
    dctp = container_manager.getContainerForReplenish("3'-ONH2-dCTP")
    dgtp = container_manager.getContainerForReplenish("3'-ONH2-dGTP")
    dttp = container_manager.getContainerForReplenish("3'-ONH2-dTTP")

    nucleotide_map = {
        'A': datp,
        'C': dctp,
        'T': dttp,
        'G': dgtp,
    }

    # Create new containers for the experiment
    synthesis_tube = container_manager.newContainer("synthesis_tube", cap=ContainerType.P200)
    wash_buffer = container_manager.newContainer("wash_buffer", cap=ContainerType.P50K)
    waste_container = container_manager.newContainer("waste_container", cap=ContainerType.P50K)
    
    # --- Prepare 1X B&W Wash Buffer ---
    # Total volume to prepare: 5mL (enough for 8 cycles * 6 washes/cycle * 100uL/wash + extra)
    total_wash_buffer_vol = 5000.0
    bw_stock_vol = total_wash_buffer_vol / 2.0  # From 2X to 1X
    tween_vol = total_wash_buffer_vol * 0.01 / 1.0 # From 1% to 0.01%
    h2o_for_wash_vol = total_wash_buffer_vol - bw_stock_vol - tween_vol
    
    pipette.transfer(bw_stock_vol, bw_buffer_stock, wash_buffer)
    pipette.transfer(tween_vol, tween, wash_buffer)
    pipette.transfer(h2o_for_wash_vol, h2o, wash_buffer)
    pipette.mix(200, wash_buffer, repetitions=10)

    # --- Initial Bead Preparation ---
    pipette.transfer(BEAD_VOLUME, beads_stock, synthesis_tube)
    robot.move_container(synthesis_tube, mag_rack_200uL.get_location())
    mag_rack_200uL.separate(duration_seconds=120)
    pipette.transfer(BEAD_VOLUME, synthesis_tube, waste_container, touch_tip=False)
    robot.move_container(synthesis_tube, DEFAULT_CONTAINERHOLDER_LOCATION)
    # Perform one wash to condition the beads
    pipette.transfer(WASH_VOLUME, wash_buffer, synthesis_tube)
    pipette.mix(90, synthesis_tube, repetitions=10)
    robot.move_container(synthesis_tube, mag_rack_200uL.get_location())
    mag_rack_200uL.separate(duration_seconds=120)
    pipette.transfer(WASH_VOLUME, synthesis_tube, waste_container)
    robot.move_container(synthesis_tube, DEFAULT_CONTAINERHOLDER_LOCATION)


    # --- Synthesis Cycles ---
    sequence = "ACTCTGAT"
    
    for nucleotide_char in sequence:
        current_nucleotide_reagent = nucleotide_map[nucleotide_char]
        
        # 1. Coupling Reaction
        # Add reagents to the synthesis tube
        pipette.transfer(10.0, reaction_buffer, synthesis_tube)
        pipette.transfer(20.0, cocl2, synthesis_tube)
        pipette.transfer(2.0, tdt_enzyme, synthesis_tube)
        pipette.transfer(5.0, current_nucleotide_reagent, synthesis_tube)
        pipette.transfer(63.0, h2o, synthesis_tube) # Top up to REACTION_VOLUME
        
        pipette.mix(80, synthesis_tube, repetitions=10)
        
        robot.move_container(synthesis_tube, heater_shaker.get_location())
        heater_shaker.incubate(
            target_temperature_celsius=37.0,
            target_speed_rpm=300, # Gentle rotation
            duration_seconds=600  # 10 minutes
        )
        robot.move_container(synthesis_tube, DEFAULT_CONTAINERHOLDER_LOCATION)
        
        # 2. Post-Coupling Wash
        robot.move_container(synthesis_tube, mag_rack_200uL.get_location())
        mag_rack_200uL.separate(duration_seconds=120)
        pipette.transfer(REACTION_VOLUME, synthesis_tube, waste_container)
        robot.move_container(synthesis_tube, DEFAULT_CONTAINERHOLDER_LOCATION)

        for _ in range(3):
            pipette.transfer(WASH_VOLUME, wash_buffer, synthesis_tube)
            pipette.mix(90, synthesis_tube, repetitions=10)
            robot.move_container(synthesis_tube, mag_rack_200uL.get_location())
            mag_rack_200uL.separate(duration_seconds=120)
            pipette.transfer(WASH_VOLUME, synthesis_tube, waste_container)
            robot.move_container(synthesis_tube, DEFAULT_CONTAINERHOLDER_LOCATION)
            
        # 3. Deblocking Reaction
        pipette.transfer(DEBLOCKING_VOLUME, deblocking_reagent, synthesis_tube)
        pipette.mix(90, synthesis_tube, repetitions=10)
        
        robot.move_container(synthesis_tube, heater_shaker.get_location())
        heater_shaker.incubate(
            target_speed_rpm=300, # Gentle rotation at room temp
            duration_seconds=300 # 5 minutes
        )
        robot.move_container(synthesis_tube, DEFAULT_CONTAINERHOLDER_LOCATION)
        
        # 4. Post-Deblocking Wash
        robot.move_container(synthesis_tube, mag_rack_200uL.get_location())
        mag_rack_200uL.separate(duration_seconds=120)
        pipette.transfer(DEBLOCKING_VOLUME, synthesis_tube, waste_container)
        robot.move_container(synthesis_tube, DEFAULT_CONTAINERHOLDER_LOCATION)
        
        for _ in range(3):
            pipette.transfer(WASH_VOLUME, wash_buffer, synthesis_tube)
            pipette.mix(90, synthesis_tube, repetitions=10)
            robot.move_container(synthesis_tube, mag_rack_200uL.get_location())
            mag_rack_200uL.separate(duration_seconds=120)
            pipette.transfer(WASH_VOLUME, synthesis_tube, waste_container)
            robot.move_container(synthesis_tube, DEFAULT_CONTAINERHOLDER_LOCATION)
            
    # Final product is in the synthesis_tube (beads with synthesized DNA)
    print(f"Final container '{synthesis_tube.label}' volume: {synthesis_tube.volume} uL")

run_protocol()