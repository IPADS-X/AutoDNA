#
# Path Description: Path: Option 1.1.1, Option 2.1.1, Option 2.2.1
from lab_modules import (
    container_manager,
    pipette,
    robot,
    thermal_cycler,
    centrifuge_200uL,
    ContainerType,
    DEFAULT_CONTAINERHOLDER_LOCATION
)

# Part 1: Template DNA Preparation
# Step 1.1: Annealing of DNA oligonucleotides

# Get reagents
annealing_buffer = container_manager.getContainerForReplenish("Annealing Buffer", required_volume=2)
oligo_1 = container_manager.getContainerForReplenish("Oligo A", required_volume=2)
oligo_2 = container_manager.getContainerForReplenish("Oligo B", required_volume=2)
sterile_water = container_manager.getContainerForReplenish("Nuclease-Free Water", required_volume=14 + 6.5)

# Create reaction tube
annealing_tube = container_manager.newContainer(label="annealing_reaction_tube", cap=ContainerType.P200)

# Combine reagents
pipette.transfer(volume=2, source=annealing_buffer, destination=annealing_tube)
pipette.transfer(volume=2, source=oligo_1, destination=annealing_tube)
pipette.transfer(volume=2, source=oligo_2, destination=annealing_tube)
pipette.transfer(volume=14, source=sterile_water, destination=annealing_tube)

# Move tube to thermal cycler
robot.move_container(annealing_tube, thermal_cycler.get_location())

# Define and run the annealing protocol
thermal_cycler.close_lid()
annealing_protocol = [
    {"temperature_celsius": 95.0, "duration_seconds": 120},  # Heat at 95°C for 2 minutes
]

# Approximate gradual cooling from 95°C to 25°C over 45 minutes (2700 seconds)
start_temp = 95.0
end_temp = 25.0
ramp_duration_seconds = 45 * 60
num_steps = 14  # Create 14 steps, each a 5-degree drop
step_duration = int(ramp_duration_seconds / num_steps)
temp_decrement = (start_temp - end_temp) / num_steps
current_temp = start_temp

for _ in range(num_steps):
    current_temp -= temp_decrement
    annealing_protocol.append({
        "temperature_celsius": round(current_temp, 1),
        "duration_seconds": step_duration
    })

# Hold at 25°C for 10 minutes
annealing_protocol.append({"temperature_celsius": 25.0, "duration_seconds": 600})

thermal_cycler.run_protocol(annealing_protocol)
thermal_cycler.open_lid()

# Move tube back and rename for clarity
robot.move_container(annealing_tube, DEFAULT_CONTAINERHOLDER_LOCATION)
annealing_tube.label = "ds_oligo_dna_solution"


# Part 2: In Vitro Transcription for Single-Stranded RNA Synthesis
# Step 2.1: Assembly of the in vitro transcription reaction

# Get reagents
transcription_buffer = container_manager.getContainerForReplenish("Transcription Buffer", required_volume=2)
atp_solution = container_manager.getContainerForReplenish("ATP Solution", required_volume=2)
gtp_solution = container_manager.getContainerForReplenish("GTP Solution", required_volume=2)
ctp_solution = container_manager.getContainerForReplenish("CTP Solution", required_volume=2)
utp_solution = container_manager.getContainerForReplenish("UTP Solution", required_volume=2)
rnase_inhibitor = container_manager.getContainerForReplenish("RNase Inhibitor", required_volume=0.5)
t7_polymerase = container_manager.getContainerForReplenish("T7 RNA Polymerase", required_volume=2)

# Create reaction tube
transcription_tube = container_manager.newContainer(label="transcription_reaction_tube", cap=ContainerType.P200)

# Combine reagents (master mix)
pipette.transfer(volume=2, source=transcription_buffer, destination=transcription_tube)
pipette.transfer(volume=2, source=atp_solution, destination=transcription_tube)
pipette.transfer(volume=2, source=gtp_solution, destination=transcription_tube)
pipette.transfer(volume=2, source=ctp_solution, destination=transcription_tube)
pipette.transfer(volume=2, source=utp_solution, destination=transcription_tube)
pipette.transfer(volume=0.5, source=rnase_inhibitor, destination=transcription_tube)
pipette.transfer(volume=2, source=t7_polymerase, destination=transcription_tube)
pipette.transfer(volume=6.5, source=sterile_water, destination=transcription_tube)

# Mix gently
pipette.mix(volume=15, location=transcription_tube, repetitions=5)

# Add DNA template last
pipette.transfer(volume=1, source=annealing_tube, destination=transcription_tube)

# Mix final solution and centrifuge
pipette.mix(volume=18, location=transcription_tube, repetitions=8)
robot.move_container(transcription_tube, centrifuge_200uL.get_location())
centrifuge_200uL.run()
robot.move_container(transcription_tube, DEFAULT_CONTAINERHOLDER_LOCATION)


# Step 2.2: Incubation for RNA synthesis
# Move tube to thermal cycler for incubation
robot.move_container(transcription_tube, thermal_cycler.get_location())
thermal_cycler.close_lid()

# Incubate at 42°C for 2 hours
incubation_protocol = [{"temperature_celsius": 42.0, "duration_seconds": 2 * 60 * 60}]
thermal_cycler.run_protocol(incubation_protocol)

thermal_cycler.open_lid()
robot.move_container(transcription_tube, DEFAULT_CONTAINERHOLDER_LOCATION)
transcription_tube.label = "synthesized_ssRNA_solution"

# Print final results
print(f"Protocol finished. Final product in container '{transcription_tube.label}' with volume {transcription_tube.volume} µL.")