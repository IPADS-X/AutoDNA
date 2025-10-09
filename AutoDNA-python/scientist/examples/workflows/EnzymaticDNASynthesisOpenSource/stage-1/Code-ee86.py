#
# Path Description: Path: Option 1.1.1, Option 1.2.1, Option 1.3.1, Option 1.4.1, Option 1.5.1
from lab_modules import (
    container_manager,
    pipette,
    robot,
    heater_shaker,
    mag_rack_200uL,
    Container,
    ContainerType,
    Location,
)

def synthesis_cycle(
    reaction_tube: Container,
    nucleotide_container: Container,
    reaction_buffer_10x: Container,
    cocl2: Container,
    tdt: Container,
    h2o: Container,
    deblocking_reagent: Container,
    wash_buffer: Container,
    waste: Container,
):
    """
    Performs one full cycle of nucleotide addition: coupling, washing, deblocking, and final washing.
    """

    # Step 1.1: Coupling Reaction
    # Add reagents to the bead pellet for a 100 µL final volume
    pipette.transfer(volume=10, source=reaction_buffer_10x, destination=reaction_tube)
    pipette.transfer(volume=40, source=cocl2, destination=reaction_tube)
    pipette.transfer(volume=4, source=tdt, destination=reaction_tube)
    pipette.transfer(volume=10, source=nucleotide_container, destination=reaction_tube)
    pipette.transfer(volume=26, source=h2o, destination=reaction_tube)

    # Mix the reaction
    pipette.mix(volume=80, location=reaction_tube, repetitions=15)

    # Incubate
    robot.move_container(reaction_tube, heater_shaker.get_location())
    heater_shaker.incubate(
        target_temperature_celsius=30,
        duration_seconds=300,  # 5 minutes
        target_speed_rpm=500,  # Gentle rotation
    )
    robot.move_container(reaction_tube, container_manager._nextContainerLocation)

    # Step 1.2: Post-Coupling Wash
    wash(reaction_tube, wash_buffer, waste)

    # Step 1.3: Deblocking Reaction
    # Resuspend beads in cleavage reagent
    pipette.transfer(volume=100, source=deblocking_reagent, destination=reaction_tube)
    pipette.mix(volume=90, location=reaction_tube, repetitions=15)

    # Incubate at room temperature with rotation
    robot.move_container(reaction_tube, heater_shaker.get_location())
    heater_shaker.incubate(
        duration_seconds=300,  # 5 minutes
        target_speed_rpm=500,  # Gentle rotation
    )
    robot.move_container(reaction_tube, container_manager._nextContainerLocation)

    # Step 1.4: Post-Deblocking Wash
    wash(reaction_tube, wash_buffer, waste)


def wash(reaction_tube: Container, wash_buffer: Container, waste: Container):
    """
    Performs 3 wash cycles on the beads in the reaction tube.
    """
    wash_volume = 150
    for _ in range(3):
        # Separate beads
        robot.move_container(reaction_tube, mag_rack_200uL.get_location())
        mag_rack_200uL.separate(duration_seconds=60)

        # Remove supernatant
        pipette.transfer(
            volume=reaction_tube.volume,
            source=reaction_tube,
            destination=waste,
            air_gap_volume=10,
        )
        robot.move_container(reaction_tube, container_manager._nextContainerLocation)

        # Add wash buffer and resuspend
        pipette.transfer(volume=wash_volume, source=wash_buffer, destination=reaction_tube)
        pipette.mix(volume=100, location=reaction_tube, repetitions=15)

    # Final separation and supernatant removal
    robot.move_container(reaction_tube, mag_rack_200uL.get_location())
    mag_rack_200uL.separate(duration_seconds=60)
    pipette.transfer(
        volume=reaction_tube.volume,
        source=reaction_tube,
        destination=waste,
        air_gap_volume=10,
    )
    robot.move_container(reaction_tube, container_manager._nextContainerLocation)


# --- Main Experiment ---

# 1. Initialize containers for reagents and working tubes
# Reagents
beads_stock = container_manager.getContainerForReplenish(
    "Initiator-immobilized magnetic beads", required_volume=10
)
tdt_stock = container_manager.getContainerForReplenish(
    "Terminal Deoxynucleotidyl Transferase", required_volume=32
)
datp_stock = container_manager.getContainerForReplenish(
    "3'-ONH2-dATP", required_volume=40
)
dctp_stock = container_manager.getContainerForReplenish(
    "3'-ONH2-dCTP", required_volume=10
)
dgtp_stock = container_manager.getContainerForReplenish(
    "3'-ONH2-dGTP", required_volume=10
)
dttp_stock = container_manager.getContainerForReplenish(
    "3'-ONH2-dTTP", required_volume=20
)
cocl2_stock = container_manager.getContainerForReplenish(
    "Cobalt Chloride", required_volume=320
)
reaction_buffer_10x_stock = container_manager.getContainerForReplenish(
    "Reaction Buffer", required_volume=80
)
deblocking_reagent_stock = container_manager.getContainerForReplenish(
    "Cleavage/Deblocking Buffer", required_volume=800
)
bw_buffer_2x_stock = container_manager.getContainerForReplenish(
    "B&W Buffer", required_volume=2400
)
tween_1_percent_stock = container_manager.getContainerForReplenish(
    "Tween-20", required_volume=48
)
h2o_stock = container_manager.getContainerForReplenish(
    "Nuclease-Free Water", required_volume=208 + 2352
)

# Working tubes
reaction_tube = container_manager.newContainer(
    "synthesis_reaction_tube", cap=ContainerType.P200
)
waste_container = container_manager.newContainer("waste", cap=ContainerType.P1500)
wash_buffer_1x = container_manager.newContainer(
    "1x_B&W_wash_buffer", cap=ContainerType.P50K
)

# 2. Prepare 1X B&W Wash Buffer
# Total volume needed: 150 uL per wash * 3 washes per step * 2 steps per cycle * 8 cycles = 7200 uL.
# We will prepare 8000 uL.
# 1X B&W Buffer Recipe: Dilute 2X stock, add Tween-20 to 0.1% final concentration
pipette.transfer(
    volume=4000, source=bw_buffer_2x_stock, destination=wash_buffer_1x
)  # 50%
pipette.transfer(
    volume=800, source=tween_1_percent_stock, destination=wash_buffer_1x
)  # 10% of 1% -> 0.1%
pipette.transfer(
    volume=3200, source=h2o_stock, destination=wash_buffer_1x
)  # 40%
pipette.mix(volume=1000, location=wash_buffer_1x, repetitions=20, reuse_tip=True)

# 3. Prepare beads by removing storage buffer
pipette.transfer(volume=10, source=beads_stock, destination=reaction_tube)
robot.move_container(reaction_tube, mag_rack_200uL.get_location())
mag_rack_200uL.separate(duration_seconds=60)
pipette.transfer(
    volume=reaction_tube.volume, source=reaction_tube, destination=waste_container
)
robot.move_container(reaction_tube, container_manager._nextContainerLocation)

# 4. Perform iterative synthesis for the sequence ACTCTGAT
target_sequence = "ACTCTGAT"
nucleotide_map = {
    "A": datp_stock,
    "C": dctp_stock,
    "T": dttp_stock,
    "G": dgtp_stock,
}

for nucleotide_char in target_sequence:
    nucleotide_reagent = nucleotide_map[nucleotide_char]
    synthesis_cycle(
        reaction_tube=reaction_tube,
        nucleotide_container=nucleotide_reagent,
        reaction_buffer_10x=reaction_buffer_10x_stock,
        cocl2=cocl2_stock,
        tdt=tdt_stock,
        h2o=h2o_stock,
        deblocking_reagent=deblocking_reagent_stock,
        wash_buffer=wash_buffer_1x,
        waste=waste_container,
    )

# 5. Final product is the bead pellet in the reaction tube. Resuspend in 50 uL water for storage.
pipette.transfer(volume=50, source=h2o_stock, destination=reaction_tube)
pipette.mix(volume=40, location=reaction_tube, repetitions=10)

print(
    f"Final product container: {reaction_tube.label}, Final volume: {reaction_tube.volume} uL"
)