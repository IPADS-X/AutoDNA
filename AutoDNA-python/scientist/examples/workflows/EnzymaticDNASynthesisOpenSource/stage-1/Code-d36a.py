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

# --- Protocol Constants ---
SEQUENCE = "ACTCTGAT"
REACTION_VOLUME = 100.0
BEAD_VOLUME = 50.0
WASH_VOLUME = 100.0
NUM_WASHES = 3
DEBLOCKING_VOLUME = 100.0
FINAL_RESUSPENSION_VOLUME = 50.0

# --- Reagent volumes for 100uL coupling reaction ---
VOL_REACTION_BUFFER_10X = 10.0
VOL_COCL2 = 40.0
VOL_DNTP = 10.0
VOL_TDT = 2.0
VOL_H2O = REACTION_VOLUME - (
    VOL_REACTION_BUFFER_10X + VOL_COCL2 + VOL_DNTP + VOL_TDT
)


def wash_beads(
    reaction_tube: Container,
    wash_buffer: Container,
    waste_container: Container,
    num_washes: int,
    wash_volume: float,
):
    """Helper function to perform magnetic bead wash cycles."""
    # First, remove the supernatant from the previous reaction
    robot.move_container(reaction_tube, mag_rack_200uL.get_location())
    mag_rack_200uL.separate(duration_seconds=120)
    if reaction_tube.volume > 0:
        pipette.transfer(reaction_tube.volume, reaction_tube, waste_container)
    robot.move_container(reaction_tube, DEFAULT_CONTAINERHOLDER_LOCATION)

    for _ in range(num_washes):
        # 1. Resuspend beads in wash buffer
        pipette.transfer(wash_volume, wash_buffer, reaction_tube)
        pipette.mix(volume=wash_volume * 0.9, location=reaction_tube, repetitions=15)

        # 2. Separate beads on magnetic rack
        robot.move_container(reaction_tube, mag_rack_200uL.get_location())
        mag_rack_200uL.separate(duration_seconds=120)

        # 3. Aspirate and discard supernatant
        if reaction_tube.volume > 0:
            pipette.transfer(reaction_tube.volume, reaction_tube, waste_container)

        robot.move_container(reaction_tube, DEFAULT_CONTAINERHOLDER_LOCATION)


# --- 1. Reagent and Container Setup ---

# Get reagent containers from inventory
h2o = container_manager.getContainerForReplenish("Nuclease-Free Water")
cocl2 = container_manager.getContainerForReplenish("Cobalt Chloride")
tdt_enzyme = container_manager.getContainerForReplenish("Terminal Deoxynucleotidyl Transferase")
reaction_buffer_10x = container_manager.getContainerForReplenish("Reaction Buffer")
cleavage_buffer = container_manager.getContainerForReplenish("Cleavage/Deblocking Buffer")
b_and_w_buffer_2x = container_manager.getContainerForReplenish("B&W Buffer")
beads = container_manager.getContainerForReplenish("Initiator-immobilized magnetic beads")

# dNTP reagents
dntp_reagents = {
    "A": container_manager.getContainerForReplenish("3'-ONH2-dATP"),
    "C": container_manager.getContainerForReplenish("3'-ONH2-dCTP"),
    "G": container_manager.getContainerForReplenish("3'-ONH2-dGTP"),
    "T": container_manager.getContainerForReplenish("3'-ONH2-dTTP"),
}

# Create new containers for the experiment
synthesis_tube = container_manager.newContainer("SynthesisTube", cap=ContainerType.P200)
waste_container = container_manager.newContainer("WasteContainer", cap=ContainerType.P50K)
b_and_w_buffer_1x = container_manager.newContainer("1X_B&W_Buffer", cap=ContainerType.P50K)

# --- 2. Prepare 1X B&W Buffer ---
# Dilute the 2X stock to a 1X working solution
pipette.transfer(2500, b_and_w_buffer_2x, b_and_w_buffer_1x)
pipette.transfer(2500, h2o, b_and_w_buffer_1x)
pipette.mix(volume=800, location=b_and_w_buffer_1x, repetitions=10)

# --- 3. Initial Bead Preparation ---
# Transfer beads to the synthesis tube and remove storage buffer
pipette.transfer(BEAD_VOLUME, beads, synthesis_tube)
robot.move_container(synthesis_tube, mag_rack_200uL.get_location())
mag_rack_200uL.separate(duration_seconds=120)
pipette.transfer(synthesis_tube.volume, synthesis_tube, waste_container)
robot.move_container(synthesis_tube, DEFAULT_CONTAINERHOLDER_LOCATION)

# --- 4. Iterative Synthesis ---
for nucleotide in SEQUENCE:
    # --- Step 1.1: Coupling Reaction ---
    current_dntp = dntp_reagents[nucleotide]

    # Add reagents to the beads to start the reaction
    pipette.transfer(VOL_REACTION_BUFFER_10X, reaction_buffer_10x, synthesis_tube)
    pipette.transfer(VOL_COCL2, cocl2, synthesis_tube)
    pipette.transfer(VOL_DNTP, current_dntp, synthesis_tube)
    pipette.transfer(VOL_TDT, tdt_enzyme, synthesis_tube)
    pipette.transfer(VOL_H2O, h2o, synthesis_tube)
    pipette.mix(volume=REACTION_VOLUME * 0.8, location=synthesis_tube, repetitions=10)

    # Incubate to allow nucleotide incorporation
    robot.move_container(synthesis_tube, heater_shaker.get_location())
    heater_shaker.incubate(
        target_temperature_celsius=30.0,
        duration_seconds=600,  # 10 minutes
        target_speed_rpm=800,
    )
    robot.move_container(synthesis_tube, DEFAULT_CONTAINERHOLDER_LOCATION)

    # --- Step 1.2: Post-Coupling Wash ---
    wash_beads(
        reaction_tube=synthesis_tube,
        wash_buffer=b_and_w_buffer_1x,
        waste_container=waste_container,
        num_washes=NUM_WASHES,
        wash_volume=WASH_VOLUME,
    )

    # --- Step 1.3: Deblocking Reaction ---
    pipette.transfer(DEBLOCKING_VOLUME, cleavage_buffer, synthesis_tube)
    pipette.mix(volume=DEBLOCKING_VOLUME * 0.9, location=synthesis_tube, repetitions=10)

    # Incubate at room temperature with shaking to deblock the 3' end
    robot.move_container(synthesis_tube, heater_shaker.get_location())
    heater_shaker.incubate(
        duration_seconds=300,  # 5 minutes
        target_speed_rpm=800
    )
    robot.move_container(synthesis_tube, DEFAULT_CONTAINERHOLDER_LOCATION)

    # --- Step 1.4: Post-Deblocking Wash ---
    wash_beads(
        reaction_tube=synthesis_tube,
        wash_buffer=b_and_w_buffer_1x,
        waste_container=waste_container,
        num_washes=NUM_WASHES,
        wash_volume=WASH_VOLUME,
    )

# --- 5. Final Product ---
# Resuspend the final beads with the synthesized DNA in B&W buffer for storage
pipette.transfer(FINAL_RESUSPENSION_VOLUME, b_and_w_buffer_1x, synthesis_tube)
pipette.mix(volume=FINAL_RESUSPENSION_VOLUME * 0.8, location=synthesis_tube, repetitions=10)

print(f"Final product in container '{synthesis_tube.label}' with volume {synthesis_tube.volume}uL.")