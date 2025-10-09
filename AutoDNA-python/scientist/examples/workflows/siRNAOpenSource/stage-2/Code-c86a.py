### SCRIPT START ###
# Path Description: Path: Option 1.1.1, Option 2.1.1
from lab_modules import *

# Initialize instruments
timer = Timer()
container_manager = ContainerManager()
pipette = Pipette()
robot = Robot()
mag_rack_1p5mL = MagRackP1500()

# --- Reagent and Sample Setup ---
# Input container from the previous stage
synthesized_ssRNA_solution = container_manager.getContainerForReplenish(name="synthesized_ssRNA_solution", required_volume=20.0)

# Reagents for Part 1: DNase Treatment
dnase_i = container_manager.getContainerForReplenish(name="DNase I", required_volume=5.0)
dna_digestion_buffer = container_manager.getContainerForReplenish(name="DNA Digestion Buffer", required_volume=5.0)
nuclease_free_water = container_manager.getContainerForReplenish(name="Nuclease-Free Water", required_volume=35.0) # 20 uL for reaction, 15 uL for elution

# Reagents for Part 2: siRNA Clean-up
rna_magbinding_buffer = container_manager.getContainerForReplenish(name="RNA MagBinding Buffer", required_volume=150.0)
magbinding_beads = container_manager.getContainerForReplenish(name="MagBinding Beads", required_volume=15.0)
isopropanol = container_manager.getContainerForReplenish(name="Isopropanol", required_volume=250.0)
rna_prep_buffer = container_manager.getContainerForReplenish(name="RNA Prep Buffer", required_volume=500.0)
ethanol = container_manager.getContainerForReplenish(name="Ethanol", required_volume=1000.0) # 2 x 500 uL washes

# --- Working Containers ---
# Using a 1.5 mL tube due to large wash volumes in Part 2
cleanup_tube_1 = container_manager.newContainer(label="cleanup_tube_1", cap=ContainerType.P1500)
waste_container = container_manager.newContainer(label="waste", cap=ContainerType.P50K)

# --- Part 1: DNA Template Removal (DNase I treatment) ---

# Prepare the 50 µl DNase I Reaction Mix
pipette.transfer(5.0, dna_digestion_buffer, cleanup_tube_1)
pipette.transfer(5.0, dnase_i, cleanup_tube_1)
pipette.transfer(20.0, synthesized_ssRNA_solution, cleanup_tube_1)
# Adjust volume to 50 µl with Nuclease-Free Water
pipette.transfer(20.0, nuclease_free_water, cleanup_tube_1)

# Mix gently
pipette.mix(volume=30.0, location=cleanup_tube_1, repetitions=5)

# Incubate at room temperature for 15 minutes
timer.wait(900)  # 15 minutes * 60 seconds/minute

# --- Part 2: siRNA Clean-up (Magnetic bead-based) ---

# Add RNA MagBinding Buffer and mix
pipette.transfer(150.0, rna_magbinding_buffer, cleanup_tube_1)
pipette.mix(volume=150.0, location=cleanup_tube_1, repetitions=10)

# Resuspend beads, then add to sample and mix
pipette.mix(volume=15.0, location=magbinding_beads, repetitions=10)
pipette.transfer(15.0, magbinding_beads, cleanup_tube_1)
pipette.mix(volume=180.0, location=cleanup_tube_1, repetitions=10)

# Add isopropanol and mix for 15 minutes
pipette.transfer(250.0, isopropanol, cleanup_tube_1)
pipette.mix(volume=200.0, location=cleanup_tube_1, repetitions=300, rate_per_minute=20) # 15 min * 20 reps/min

# Magnetic separation and first supernatant removal
robot.move_container(cleanup_tube_1, mag_rack_1p5mL.get_location())
mag_rack_1p5mL.separate(wait_duration_seconds=180)
pipette.transfer(cleanup_tube_1.volume, cleanup_tube_1, waste_container)
robot.move_container(cleanup_tube_1, DEFAULT_CONTAINERHOLDER_LOCATION)

# RNA Prep Buffer wash
pipette.transfer(500.0, rna_prep_buffer, cleanup_tube_1)
pipette.mix(volume=400.0, location=cleanup_tube_1, repetitions=15)
robot.move_container(cleanup_tube_1, mag_rack_1p5mL.get_location())
mag_rack_1p5mL.separate(wait_duration_seconds=180)
pipette.transfer(cleanup_tube_1.volume, cleanup_tube_1, waste_container)
robot.move_container(cleanup_tube_1, DEFAULT_CONTAINERHOLDER_LOCATION)

# First Ethanol wash
pipette.transfer(500.0, ethanol, cleanup_tube_1)
pipette.mix(volume=400.0, location=cleanup_tube_1, repetitions=15)
robot.move_container(cleanup_tube_1, mag_rack_1p5mL.get_location())
mag_rack_1p5mL.separate(wait_duration_seconds=180)
pipette.transfer(cleanup_tube_1.volume, cleanup_tube_1, waste_container)
robot.move_container(cleanup_tube_1, DEFAULT_CONTAINERHOLDER_LOCATION)

# Second Ethanol wash
pipette.transfer(500.0, ethanol, cleanup_tube_1)
pipette.mix(volume=400.0, location=cleanup_tube_1, repetitions=15)

# Transfer bead/ethanol slurry to a new tube
cleanup_tube_2 = container_manager.newContainer(label="cleanup_tube_2", cap=ContainerType.P1500)
pipette.transfer(500.0, cleanup_tube_1, cleanup_tube_2)

# Final magnetic separation and supernatant removal
robot.move_container(cleanup_tube_2, mag_rack_1p5mL.get_location())
mag_rack_1p5mL.separate(wait_duration_seconds=180)
pipette.transfer(cleanup_tube_2.volume, cleanup_tube_2, waste_container)
robot.move_container(cleanup_tube_2, DEFAULT_CONTAINERHOLDER_LOCATION)

# Air-dry beads for 10 minutes
timer.wait(600)

# Elute RNA
pipette.transfer(15.0, nuclease_free_water, cleanup_tube_2)
pipette.mix(volume=12.0, location=cleanup_tube_2, repetitions=100, rate_per_minute=20) # Mix for 5 minutes

# Separate beads and collect the final product
robot.move_container(cleanup_tube_2, mag_rack_1p5mL.get_location())
mag_rack_1p5mL.separate(wait_duration_seconds=180)
final_sirna_product = container_manager.newContainer(label="final_sirna_product", cap=ContainerType.P200)
pipette.transfer(15.0, cleanup_tube_2, final_sirna_product)

# --- Final Output ---
print(f"Final product in container '{final_sirna_product.label}' with volume {final_sirna_product.volume} uL.")