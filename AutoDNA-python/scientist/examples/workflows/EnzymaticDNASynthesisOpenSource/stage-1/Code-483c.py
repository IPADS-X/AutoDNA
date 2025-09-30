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

def coupling_step(reaction_tube: Container, nucleotide_reagent: Container, tdt_reagent: Container, cocl2_reagent: Container, reaction_buffer_reagent: Container, water_reagent: Container):
    """Performs the TdT-mediated coupling reaction."""
    tdt_vol = 2.0
    dntp_vol = 5.0
    cocl2_vol = 20.0
    buffer_vol = 10.0
    total_vol = 100.0
    h2o_vol = total_vol - tdt_vol - dntp_vol - cocl2_vol - buffer_vol

    pipette.transfer(h2o_vol, water_reagent, reaction_tube)
    pipette.transfer(buffer_vol, reaction_buffer_reagent, reaction_tube)
    pipette.transfer(cocl2_vol, cocl2_reagent, reaction_tube)
    pipette.transfer(dntp_vol, nucleotide_reagent, reaction_tube)
    pipette.transfer(tdt_vol, tdt_reagent, reaction_tube)

    pipette.mix(volume=total_vol * 0.8, location=reaction_tube, repetitions=15)

    robot.move_container(reaction_tube, heater_shaker.get_location())
    heater_shaker.incubate(target_temperature_celsius=37, target_speed_rpm=1300, duration_seconds=600)
    robot.move_container(reaction_tube, DEFAULT_CONTAINERHOLDER_LOCATION)

def deblocking_step(reaction_tube: Container, deblocking_reagent: Container, waste_container: Container, deblock_volume: float = 100.0):
    """Performs the two-step deblocking reaction."""
    pipette.transfer(deblock_volume, deblocking_reagent, reaction_tube)
    robot.move_container(reaction_tube, heater_shaker.get_location())
    heater_shaker.incubate(target_speed_rpm=1300, duration_seconds=300)
    robot.move_container(reaction_tube, DEFAULT_CONTAINERHOLDER_LOCATION)

    robot.move_container(reaction_tube, mag_rack_200uL.get_location())
    mag_rack_200uL.separate(duration_seconds=120)
    pipette.transfer(reaction_tube.volume, reaction_tube, waste_container)
    robot.move_container(reaction_tube, DEFAULT_CONTAINERHOLDER_LOCATION)

    pipette.transfer(deblock_volume, deblocking_reagent, reaction_tube)
    robot.move_container(reaction_tube, heater_shaker.get_location())
    heater_shaker.incubate(target_speed_rpm=1300, duration_seconds=300)
    robot.move_container(reaction_tube, DEFAULT_CONTAINERHOLDER_LOCATION)

def perform_wash(reaction_tube: Container, wash_buffer: Container, waste_container: Container, wash_volume: float = 100.0, repetitions: int = 3):
    """Washes beads by removing supernatant and resuspending in buffer."""
    robot.move_container(reaction_tube, mag_rack_200uL.get_location())
    mag_rack_200uL.separate(duration_seconds=120)
    pipette.transfer(reaction_tube.volume, reaction_tube, waste_container)
    robot.move_container(reaction_tube, DEFAULT_CONTAINERHOLDER_LOCATION)

    for _ in range(repetitions):
        pipette.transfer(wash_volume, wash_buffer, reaction_tube)
        pipette.mix(volume=wash_volume * 0.8, location=reaction_tube, repetitions=10)
        
        robot.move_container(reaction_tube, mag_rack_200uL.get_location())
        mag_rack_200uL.separate(duration_seconds=120)
        pipette.transfer(reaction_tube.volume, reaction_tube, waste_container)
        robot.move_container(reaction_tube, DEFAULT_CONTAINERHOLDER_LOCATION)

TARGET_SEQUENCE = "ACTCTGAT"

tdt_enzyme = container_manager.getContainerForReplenish("Terminal Deoxynucleotidyl Transferase")
cocl2 = container_manager.getContainerForReplenish("Cobalt Chloride")
reaction_buffer_10x = container_manager.getContainerForReplenish("Reaction Buffer")
water = container_manager.getContainerForReplenish("Nuclease-Free Water")
bw_buffer_2x = container_manager.getContainerForReplenish("B&W Buffer")
deblocking_buffer = container_manager.getContainerForReplenish("Cleavage/Deblocking Buffer")
beads_stock = container_manager.getContainerForReplenish("Initiator-immobilized magnetic beads")

nucleotide_map = {
    'A': container_manager.getContainerForReplenish("3'-ONH2-dATP"),
    'C': container_manager.getContainerForReplenish("3'-ONH2-dCTP"),
    'G': container_manager.getContainerForReplenish("3'-ONH2-dGTP"),
    'T': container_manager.getContainerForReplenish("3'-ONH2-dTTP"),
}

synthesis_tube = container_manager.newContainer("synthesis_tube", cap=ContainerType.P200)
waste_container = container_manager.newContainer("waste", cap=ContainerType.P50K)
bw_buffer_1x = container_manager.newContainer("1X_B&W_Buffer", cap=ContainerType.P50K)

pipette.transfer(2500, bw_buffer_2x, bw_buffer_1x)
pipette.transfer(2500, water, bw_buffer_1x)
pipette.mix(volume=1000, location=bw_buffer_1x, repetitions=25)

beads_volume_to_use = 20.0
pipette.transfer(beads_volume_to_use, beads_stock, synthesis_tube)
robot.move_container(synthesis_tube, mag_rack_200uL.get_location())
mag_rack_200uL.separate(duration_seconds=120)
pipette.transfer(synthesis_tube.volume, synthesis_tube, waste_container)
robot.move_container(synthesis_tube, DEFAULT_CONTAINERHOLDER_LOCATION)

for nucleotide_char in TARGET_SEQUENCE:
    current_nucleotide_reagent = nucleotide_map[nucleotide_char]

    coupling_step(
        reaction_tube=synthesis_tube,
        nucleotide_reagent=current_nucleotide_reagent,
        tdt_reagent=tdt_enzyme,
        cocl2_reagent=cocl2,
        reaction_buffer_reagent=reaction_buffer_10x,
        water_reagent=water
    )

    perform_wash(
        reaction_tube=synthesis_tube,
        wash_buffer=bw_buffer_1x,
        waste_container=waste_container,
        repetitions=3
    )

    deblocking_step(
        reaction_tube=synthesis_tube,
        deblocking_reagent=deblocking_buffer,
        waste_container=waste_container
    )

    perform_wash(
        reaction_tube=synthesis_tube,
        wash_buffer=bw_buffer_1x,
        waste_container=waste_container,
        repetitions=3
    )

print(f"Synthesis of sequence '{TARGET_SEQUENCE}' complete.")
print(f"Final product is in container: '{synthesis_tube.label}'")
print(f"Final volume in container: {synthesis_tube.volume} uL")