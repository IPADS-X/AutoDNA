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

# --- CONSTANTS ---
REACTION_VOLUME = 100.0
WASH_VOLUME = 100.0
DEBLOCKING_VOLUME = 100.0
WASH_CYCLES = 3
SEQUENCE = "ACTCTGAT"
BEADS_VOLUME = 50.0

# --- HELPER FUNCTIONS ---

def coupling_step(dntp_container: Container, reaction_tube: Container):
    """Performs the TdT-mediated coupling reaction."""
    robot.move_container(reaction_tube, DEFAULT_CONTAINERHOLDER_LOCATION)
    
    # Add reagents to the reaction tube containing pelleted beads
    pipette.transfer(volume=10.0, source=reaction_buffer, destination=reaction_tube)
    pipette.transfer(volume=20.0, source=co_cl2, destination=reaction_tube)
    pipette.transfer(volume=2.0, source=tdt_enzyme, destination=reaction_tube)
    pipette.transfer(volume=5.0, source=dntp_container, destination=reaction_tube)
    pipette.transfer(volume=63.0, source=water, destination=reaction_tube) # Top up to REACTION_VOLUME
    
    pipette.mix(volume=80.0, location=reaction_tube, repetitions=10)
    
    robot.move_container(reaction_tube, heater_shaker.location)
    heater_shaker.incubate(
        target_temperature_celsius=30,
        duration_seconds=600, # 10 minutes
        target_speed_rpm=1300
    )

def wash_step(reaction_tube: Container, wash_buffer: Container, waste_container: Container):
    """Washes the beads after a reaction step."""
    robot.move_container(reaction_tube, mag_rack_200uL.location)
    mag_rack_200uL.separate(duration_seconds=120) # 2 minutes
    
    robot.move_container(reaction_tube, DEFAULT_CONTAINERHOLDER_LOCATION)
    pipette.transfer(volume=REACTION_VOLUME, source=reaction_tube, destination=waste_container)
    
    for _ in range(WASH_CYCLES):
        pipette.transfer(volume=WASH_VOLUME, source=wash_buffer, destination=reaction_tube)
        pipette.mix(volume=80.0, location=reaction_tube, repetitions=10)
        
        robot.move_container(reaction_tube, mag_rack_200uL.location)
        mag_rack_200uL.separate(duration_seconds=120)
        
        robot.move_container(reaction_tube, DEFAULT_CONTAINERHOLDER_LOCATION)
        pipette.transfer(volume=WASH_VOLUME, source=reaction_tube, destination=waste_container)

def deblocking_step(reaction_tube: Container, deblocking_buffer: Container):
    """Performs the deblocking reaction to remove the 3'-ONH2 group."""
    robot.move_container(reaction_tube, DEFAULT_CONTAINERHOLDER_LOCATION)
    pipette.transfer(volume=DEBLOCKING_VOLUME, source=deblocking_buffer, destination=reaction_tube)
    pipette.mix(volume=80.0, location=reaction_tube, repetitions=5)
    
    robot.move_container(reaction_tube, heater_shaker.location)
    heater_shaker.incubate(
        duration_seconds=300, # 5 minutes
        target_speed_rpm=1300
    )

# --- PROTOCOL EXECUTION ---

# 1. Setup and Reagent Preparation
# Get reagent containers from inventory
beads_suspension = container_manager.getContainerForReplenish("Initiator-immobilized magnetic beads")
tdt_enzyme = container_manager.getContainerForReplenish("Terminal Deoxynucleotidyl Transferase")
dATP = container_manager.getContainerForReplenish("3'-ONH2-dATP")
dCTP = container_manager.getContainerForReplenish("3'-ONH2-dCTP")
dGTP = container_manager.getContainerForReplenish("3'-ONH2-dGTP")
dTTP = container_manager.getContainerForReplenish("3'-ONH2-dTTP")
co_cl2 = container_manager.getContainerForReplenish("Cobalt Chloride")
reaction_buffer = container_manager.getContainerForReplenish("Reaction Buffer")
cleavage_buffer = container_manager.getContainerForReplenish("Cleavage/Deblocking Buffer")
lysis_buffer_2x = container_manager.getContainerForReplenish("Lysis Buffer")
water = container_manager.getContainerForReplenish("Nuclease-Free Water")

dntp_map = {'A': dATP, 'C': dCTP, 'T': dTTP, 'G': dGTP}

# Prepare 1X Lysis Buffer
lysis_buffer_1x = container_manager.newContainer("1X Lysis Buffer", cap=ContainerType.P50K)
pipette.transfer(volume=2500.0, source=lysis_buffer_2x, destination=lysis_buffer_1x)
pipette.transfer(volume=2500.0, source=water, destination=lysis_buffer_1x)
pipette.mix(volume=800.0, location=lysis_buffer_1x, repetitions=10)

# Create reaction and waste tubes
reaction_tube = container_manager.newContainer("synthesis_reaction_tube", cap=ContainerType.P200)
waste_container = container_manager.newContainer("waste", cap=ContainerType.P50K)

# 2. Initial Bead Preparation
pipette.transfer(volume=BEADS_VOLUME, source=beads_suspension, destination=reaction_tube)
robot.move_container(reaction_tube, mag_rack_200uL.location)
mag_rack_200uL.separate(duration_seconds=120)
robot.move_container(reaction_tube, DEFAULT_CONTAINERHOLDER_LOCATION)
pipette.transfer(volume=BEADS_VOLUME, source=reaction_tube, destination=waste_container)

# 3. Iterative Synthesis Cycles
for nucleotide in SEQUENCE:
    target_dntp = dntp_map[nucleotide]
    
    # Step 1.1: Coupling Reaction
    coupling_step(target_dntp, reaction_tube)
    
    # Step 1.2: Post-Coupling Wash
    wash_step(reaction_tube, lysis_buffer_1x, waste_container)
    
    # Step 1.3: Deblocking Reaction
    deblocking_step(reaction_tube, cleavage_buffer)
    
    # Step 1.4: Post-Deblocking Wash
    wash_step(reaction_tube, lysis_buffer_1x, waste_container)

# 4. Finalization
# The final synthesized oligonucleotide is on the beads in the reaction_tube.
# Add some buffer for storage.
pipette.transfer(volume=50.0, source=lysis_buffer_1x, destination=reaction_tube)
print(f"Synthesis of sequence {SEQUENCE} complete.")
print(f"Final product is in container: {reaction_tube.label}, Volume: {reaction_tube.volume} uL")