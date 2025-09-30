**Role:**
You are an expert laboratory designer specializing in biology facilities. Your expertise is in creating efficient layouts for **nucleic acid research**.

**Objective:**
Optimizing workstation layout to enhance the workflow efficiency.

**Context:**

**1. Workstation Capacities (The "Bins"):**
* Workstation 1: **3 slots**
* Workstation 2: **5 slots**
* Workstation 3: **6 slots**

**2. Equipment to Place (The "Items"):**
- `Centrifuge for 200µL Tubes`: **2 slots** (Quantity: **2**)
- `Centrifuge for 1.5mL Tubes`: **2 slots** (Quantity: **1**)
- `ThermalCycler for 200µL Tubes`: **1 slot** (Quantity: **1**)
- `Heater for 1.5mL Tubes`: **1 slot** (Quantity: **1**)
- `Magnetic Rack for 1.5mL Tubes`: **1 slot** (Quantity: **1**)
- `Magnetic Rack for 200µL Tubes`: **1 slot** (Quantity: **1**)
- `Heater-Shaker for 200µL Tubes`: **1 slot** (Quantity: **1**)
- `Fluorometer for 200µL Tubes`: **1 slot** (Quantity: **1**)
- `Capper for 200µL Tubes`: **2 slots** (Quantity: **1**)

**3. Example Workflows:**
1. **Library Preparation Workflow:**
### SCRIPT START ###
# Path Description: Path: Option 2.3.1, Option 2.4.1, Option 3.2.1, Option 3.3.2, Option 3.4.2, Option 4.2.1, Option 4.3.2, Option 5.2.2, Option 5.4.1
from lab_modules_new import *

def run_protocol():
    # Instantiate all required lab modules
    pipette = Pipette()
    robot = Robot()
    containerManager = ContainerManager()
    thermal_cycler = ThermalCycler()
    centrifuge_p200 = Centrifuge_P200()
    centrifuge_p1500 = Centrifuge_P1500()
    mag_rack = MagRack()
    heater = Heater()
    timer = Timer()

    # General use containers
    waste_container = containerManager.newContainer(label="Waste Tube", cap=ContainerType.P50K)

    # Part 2: Library Preparation: DNA Repair and End-Prep

    # Step 2.1: Reagent Preparation (Simulated by getting containers)
    # print("Part 2: Library Preparation: DNA Repair and End-Prep")
    axp_beads_reagent = containerManager.getContainerForReplenish("AMPure XP Beads", required_volume=1000)
    dcs_reagent = containerManager.getContainerForReplenish("DNA Control Sample", required_volume=100)
    ffpe_repair_mix = containerManager.getContainerForReplenish("NEBNext FFPE DNA Repair Mix", required_volume=100)
    ultra_ii_enzyme_mix = containerManager.getContainerForReplenish("Ultra II End-prep Enzyme Mix", required_volume=100)
    eb_reagent = containerManager.getContainerForReplenish("Elution Buffer", required_volume=1000)
    ffpe_repair_buffer = containerManager.getContainerForReplenish("NEBNext FFPE DNA Repair Buffer", required_volume=100)
    ultra_ii_buffer = containerManager.getContainerForReplenish("Ultra II End-prep Reaction Buffer", required_volume=100)
    water = containerManager.getContainerForReplenish("nuclease-free water", required_volume=1000)

    # Step 2.2: DNA Control Sample (DCS) Dilution
    diluted_dcs_tube = containerManager.newContainer(label="diluted_DCS", cap=ContainerType.P200)
    pipette.move(dst_container=diluted_dcs_tube, src_container=dcs_reagent, volume=10) # Assume one tube of DCS
    pipette.move(dst_container=diluted_dcs_tube, src_container=eb_reagent, volume=105)
    pipette.mix(diluted_dcs_tube, time=15)
    robot.moveContainer(dst=centrifuge_p200.location, container=diluted_dcs_tube)
    centrifuge_p200.start(time=10)
    robot.moveContainer(dst=DEFAULT_CONTAINERHOLDER_LOCATION, container=diluted_dcs_tube)

    # Step 2.3.1: DNA Sample Preparation
    gdna_reagent = containerManager.getContainerForReplenish("gDNA", required_volume=40) # 400 ng in 10 uL
    dna_sample_tube = containerManager.newContainer(label="gDNA_sample_pcr_tube", cap=ContainerType.P200)
    pipette.move(dst_container=dna_sample_tube, src_container=gdna_reagent, volume=10) # 400ng
    pipette.move(dst_container=dna_sample_tube, src_container=water, volume=1) # to 11 uL
    pipette.mix(dna_sample_tube)
    robot.moveContainer(dst=centrifuge_p200.location, container=dna_sample_tube)
    centrifuge_p200.start(time=10)
    robot.moveContainer(dst=DEFAULT_CONTAINERHOLDER_LOCATION, container=dna_sample_tube)

    # Step 2.4.1: DNA Repair and End-Prep Reaction (with DCS)
    pipette.move(dst_container=dna_sample_tube, src_container=diluted_dcs_tube, volume=1)
    pipette.move(dst_container=dna_sample_tube, src_container=ffpe_repair_buffer, volume=0.875)
    pipette.move(dst_container=dna_sample_tube, src_container=ultra_ii_buffer, volume=0.875)
    pipette.mix(dna_sample_tube, time=15)
    pipette.move(dst_container=dna_sample_tube, src_container=ffpe_repair_mix, volume=0.5)
    pipette.move(dst_container=dna_sample_tube, src_container=ultra_ii_enzyme_mix, volume=0.75)
    pipette.mix(dna_sample_tube, time=15)
    robot.moveContainer(dst=centrifuge_p200.location, container=dna_sample_tube)
    centrifuge_p200.start(time=10)
    robot.moveContainer(dst=DEFAULT_CONTAINERHOLDER_LOCATION, container=dna_sample_tube)

    # Step 2.5: Thermal Cycling for End-Prep
    robot.moveContainer(dst=thermal_cycler.location, container=dna_sample_tube)
    thermal_cycler.start_thermocycling(cycles=1, steps=[(20, 300), (65, 300)])
    robot.moveContainer(dst=DEFAULT_CONTAINERHOLDER_LOCATION, container=dna_sample_tube)

    # # Step 2.6: Cleanup of End-Prepped DNA
    end_prepped_cleanup_tube = containerManager.newContainer(label="end_prepped_cleanup", cap=ContainerType.P1500)
    pipette.move(dst_container=end_prepped_cleanup_tube, src_container=dna_sample_tube, volume=15)
    
    pipette.mix(axp_beads_reagent, time=30)
    pipette.move(dst_container=end_prepped_cleanup_tube, src_container=axp_beads_reagent, volume=15)
    pipette.mix(end_prepped_cleanup_tube, time=10)
    
    timer.wait(300)

    robot.moveContainer(dst=centrifuge_p1500.location, container=end_prepped_cleanup_tube)
    centrifuge_p1500.start(time=10)
    robot.moveContainer(dst=DEFAULT_CONTAINERHOLDER_LOCATION, container=end_prepped_cleanup_tube)
    
    robot.moveContainer(dst=mag_rack.location, container=end_prepped_cleanup_tube)
    mag_rack.wait(time=120)
    pipette.move(dst_container=waste_container, src_container=end_prepped_cleanup_tube, volume=end_prepped_cleanup_tube.volume)
    
    ethanol_80 = containerManager.getContainerForReplenish("80% ethanol", required_volume=400)
    for _ in range(2):
        robot.moveContainer(dst=DEFAULT_CONTAINERHOLDER_LOCATION, container=end_prepped_cleanup_tube)
        pipette.move(dst_container=end_prepped_cleanup_tube, src_container=ethanol_80, volume=200)
        robot.moveContainer(dst=mag_rack.location, container=end_prepped_cleanup_tube)
        mag_rack.wait(time=30)
        pipette.move(dst_container=waste_container, src_container=end_prepped_cleanup_tube, volume=200)

    robot.moveContainer(dst=centrifuge_p1500.location, container=end_prepped_cleanup_tube)
    centrifuge_p1500.start(time=10)
    robot.moveContainer(dst=mag_rack.location, container=end_prepped_cleanup_tube)
    pipette.move(dst_container=waste_container, src_container=end_prepped_cleanup_tube, volume=end_prepped_cleanup_tube.volume)
    mag_rack.wait(time=30)

    robot.moveContainer(dst=DEFAULT_CONTAINERHOLDER_LOCATION, container=end_prepped_cleanup_tube)
    pipette.move(dst_container=end_prepped_cleanup_tube, src_container=water, volume=10)
    pipette.mix(end_prepped_cleanup_tube)
    
    timer.wait(time=120) # Letting it sit for 2 minutes to elute
    robot.moveContainer(dst=mag_rack.location, container=end_prepped_cleanup_tube)
    mag_rack.wait(time=120)

    end_prepped_dna = containerManager.newContainer(label="End-prepped DNA", cap=ContainerType.P1500)
    pipette.move(dst_container=end_prepped_dna, src_container=end_prepped_cleanup_tube, volume=10)
    robot.moveContainer(dst=DEFAULT_CONTAINERHOLDER_LOCATION, container=end_prepped_dna)

    # Part 3: Library Preparation: Native Barcode Ligation
    print("Part 3: Library Preparation: Native Barcode Ligation")
    
    # Step 3.1: Reagent Preparation
    nb01 = containerManager.getContainerForReplenish("NB01", required_volume=10)
    blunt_ta_ligase_mix = containerManager.getContainerForReplenish("Blunt/TA Ligase Master Mix", required_volume=20)
    edta = containerManager.getContainerForReplenish("EDTA (blue cap)", required_volume=10)

    # Step 3.2.1: Native Barcode Ligation
    ligation_tube = containerManager.newContainer(label="barcode_ligation", cap=ContainerType.P200)
    pipette.move(dst_container=ligation_tube, src_container=end_prepped_dna, volume=7.5)
    pipette.move(dst_container=ligation_tube, src_container=nb01, volume=2.5)
    pipette.mix(ligation_tube, time=15)
    pipette.move(dst_container=ligation_tube, src_container=blunt_ta_ligase_mix, volume=10)
    pipette.mix(ligation_tube, time=15)
    robot.moveContainer(dst=centrifuge_p200.location, container=ligation_tube)
    centrifuge_p200.start(time=10)
    
    timer.wait(time=1200) # Incubating for 10 minutes
    robot.moveContainer(dst=DEFAULT_CONTAINERHOLDER_LOCATION, container=ligation_tube)

    # Step 3.3.2: Ligation Reaction Termination and Sample Pooling
    pipette.move(dst_container=ligation_tube, src_container=edta, volume=4)
    pipette.mix(ligation_tube)
    robot.moveContainer(dst=centrifuge_p200.location, container=ligation_tube)
    centrifuge_p200.start(time=10)
    robot.moveContainer(dst=DEFAULT_CONTAINERHOLDER_LOCATION, container=ligation_tube)
    
    pooled_barcoded_sample = containerManager.newContainer(label="pooled_barcoded_sample", cap=ContainerType.P1500)
    pipette.move(dst_container=pooled_barcoded_sample, src_container=ligation_tube, volume=24)

    # Step 3.4.2: Cleanup of Pooled Barcoded Library
    pipette.mix(axp_beads_reagent, time=30)
    pipette.move(dst_container=pooled_barcoded_sample, src_container=axp_beads_reagent, volume=10)
    pipette.mix(pooled_barcoded_sample)
    
    timer.wait(time=600) # Letting it sit for 5 minutes

    robot.moveContainer(dst=DEFAULT_CONTAINERHOLDER_LOCATION, container=pooled_barcoded_sample)

    robot.moveContainer(dst=centrifuge_p1500.location, container=pooled_barcoded_sample)
    centrifuge_p1500.start(time=10)
    robot.moveContainer(dst=mag_rack.location, container=pooled_barcoded_sample)
    mag_rack.wait(time=300)
    pipette.move(dst_container=waste_container, src_container=pooled_barcoded_sample, volume=pooled_barcoded_sample.volume)
    
    ethanol_80_part3 = containerManager.getContainerForReplenish("80% ethanol", required_volume=1400)
    for _ in range(2):
        robot.moveContainer(dst=DEFAULT_CONTAINERHOLDER_LOCATION, container=pooled_barcoded_sample)
        pipette.move(dst_container=pooled_barcoded_sample, src_container=ethanol_80_part3, volume=700)
        robot.moveContainer(dst=mag_rack.location, container=pooled_barcoded_sample)
        mag_rack.wait(time=30)
        pipette.move(dst_container=waste_container, src_container=pooled_barcoded_sample, volume=700)

    robot.moveContainer(dst=centrifuge_p1500.location, container=pooled_barcoded_sample)
    centrifuge_p1500.start(time=10)
    robot.moveContainer(dst=mag_rack.location, container=pooled_barcoded_sample)
    pipette.move(dst_container=waste_container, src_container=pooled_barcoded_sample, volume=pooled_barcoded_sample.volume)
    mag_rack.wait(time=30)
    
    robot.moveContainer(dst=DEFAULT_CONTAINERHOLDER_LOCATION, container=pooled_barcoded_sample)
    pipette.move(dst_container=pooled_barcoded_sample, src_container=water, volume=35)
    pipette.mix(pooled_barcoded_sample, time=10)
    
    for i in range(5):
        robot.moveContainer(dst=heater.location, container=pooled_barcoded_sample)
        heater.set_temp(37)
        heater.start(time=120) 
        robot.moveContainer(dst=DEFAULT_CONTAINERHOLDER_LOCATION, container=pooled_barcoded_sample)
        pipette.mix(pooled_barcoded_sample, time=10)
    robot.moveContainer(dst=DEFAULT_CONTAINERHOLDER_LOCATION, container=pooled_barcoded_sample)

    robot.moveContainer(dst=mag_rack.location, container=pooled_barcoded_sample)
    mag_rack.wait(time=120)
    
    barcoded_dna_library = containerManager.newContainer(label="barcoded_DNA_library", cap=ContainerType.P1500)
    pipette.move(dst_container=barcoded_dna_library, src_container=pooled_barcoded_sample, volume=35)
    robot.moveContainer(dst=DEFAULT_CONTAINERHOLDER_LOCATION, container=barcoded_dna_library)

    # Part 4: Library Preparation: Adapter Ligation and Final Cleanup
    print("Part 4: Library Preparation: Adapter Ligation and Final Cleanup")
    
    # Step 4.1: Reagent Preparation
    native_adapter = containerManager.getContainerForReplenish("Native Adapter", required_volume=10)
    ligation_buffer = containerManager.getContainerForReplenish("NEBNext Quick Ligation Reaction Buffer", required_volume=20)
    t4_ligase = containerManager.getContainerForReplenish("Quick T4 DNA Ligase", required_volume=10)

    # Step 4.2.1: Adapter Ligation
    adapter_ligation_tube = containerManager.newContainer(label="adapter_ligation_tube", cap=ContainerType.P1500)
    pipette.move(dst_container=adapter_ligation_tube, src_container=barcoded_dna_library, volume=30)
    pipette.move(dst_container=adapter_ligation_tube, src_container=native_adapter, volume=5)
    pipette.mix(adapter_ligation_tube)
    pipette.move(dst_container=adapter_ligation_tube, src_container=ligation_buffer, volume=10)
    pipette.mix(adapter_ligation_tube)
    pipette.move(dst_container=adapter_ligation_tube, src_container=t4_ligase, volume=5)
    pipette.mix(adapter_ligation_tube)
    robot.moveContainer(dst=centrifuge_p1500.location, container=adapter_ligation_tube)
    centrifuge_p1500.start(time=10)
    timer.wait(time=1200) # Incubating for 10 minutes
    robot.moveContainer(dst=DEFAULT_CONTAINERHOLDER_LOCATION, container=adapter_ligation_tube)

    # Step 4.3.2: Cleanup of Adapter-Ligated Library using SFB
    pipette.mix(axp_beads_reagent, time=30)
    pipette.move(dst_container=adapter_ligation_tube, src_container=axp_beads_reagent, volume=20)
    pipette.mix(adapter_ligation_tube)
    timer.wait(time=600) # Letting it sit for 5 minutes
    robot.moveContainer(dst=centrifuge_p1500.location, container=adapter_ligation_tube)
    centrifuge_p1500.start(time=10)
    robot.moveContainer(dst=mag_rack.location, container=adapter_ligation_tube)
    mag_rack.wait(time=120)
    pipette.move(dst_container=waste_container, src_container=adapter_ligation_tube, volume=adapter_ligation_tube.volume)
    
    sfb = containerManager.getContainerForReplenish("Short Fragment Buffer", required_volume=250)
    for _ in range(2):
        robot.moveContainer(dst=DEFAULT_CONTAINERHOLDER_LOCATION, container=adapter_ligation_tube)
        pipette.move(dst_container=adapter_ligation_tube, src_container=sfb, volume=125)
        pipette.mix(adapter_ligation_tube)
        robot.moveContainer(dst=centrifuge_p1500.location, container=adapter_ligation_tube)
        centrifuge_p1500.start(time=10)
        robot.moveContainer(dst=mag_rack.location, container=adapter_ligation_tube)
        mag_rack.wait(time=120)
        pipette.move(dst_container=waste_container, src_container=adapter_ligation_tube, volume=125)
    
    robot.moveContainer(dst=centrifuge_p1500.location, container=adapter_ligation_tube)
    centrifuge_p1500.start(time=10)
    robot.moveContainer(dst=mag_rack.location, container=adapter_ligation_tube)
    pipette.move(dst_container=waste_container, src_container=adapter_ligation_tube, volume=adapter_ligation_tube.volume)
    
    robot.moveContainer(dst=DEFAULT_CONTAINERHOLDER_LOCATION, container=adapter_ligation_tube)
    pipette.move(dst_container=adapter_ligation_tube, src_container=eb_reagent, volume=15)
    for i in range(5):
        robot.moveContainer(dst=heater.location, container=adapter_ligation_tube)
        heater.set_temp(37)
        heater.start(time=120) 
        robot.moveContainer(dst=DEFAULT_CONTAINERHOLDER_LOCATION, container=adapter_ligation_tube)
        pipette.mix(adapter_ligation_tube, time=10)
    robot.moveContainer(dst=DEFAULT_CONTAINERHOLDER_LOCATION, container=adapter_ligation_tube)
    
    robot.moveContainer(dst=mag_rack.location, container=adapter_ligation_tube)
    mag_rack.wait(time=120)
    
    final_dna_library = containerManager.newContainer(label="final_DNA_library", cap=ContainerType.P1500)
    pipette.move(dst_container=final_dna_library, src_container=adapter_ligation_tube, volume=15)
    robot.moveContainer(dst=DEFAULT_CONTAINERHOLDER_LOCATION, container=final_dna_library)

if __name__ == "__main__":
    run_protocol()

### SCRIPT END ###

2. **RNA Synthesis Workflow:**
### SCRIPT START ###
### SCRIPT START ###
# Path Description: Path: Option 1.1.1, Option 2.1.1, Option 3.1.2, Option 4.1.3
from lab_modules_new import *

def run_protocol():
    # Instantiate all necessary lab equipment
    pipette = Pipette()
    robot = Robot()
    thermal_cycler = ThermalCycler()
    heatershaker = HeaterShaker()
    centrifuge = Centrifuge_P200()
    mag_rack = MagRack()
    timer = Timer()

    # Get reagent containers from inventory
    annealing_buffer = containerManager.getContainerForReplenish("Annealing Buffer", required_volume=2)
    oligo_1 = containerManager.getContainerForReplenish("Oligo-1", required_volume=2)
    oligo_2 = containerManager.getContainerForReplenish("Oligo-2", required_volume=2)
    nuclease_free_water = containerManager.getContainerForReplenish("Nuclease-Free Water", required_volume=1000)
    transcription_buffer = containerManager.getContainerForReplenish("Transcription Buffer", required_volume=2)
    atp_solution = containerManager.getContainerForReplenish("ATP Solution", required_volume=2)
    gtp_solution = containerManager.getContainerForReplenish("GTP Solution", required_volume=2)
    ctp_solution = containerManager.getContainerForReplenish("CTP Solution", required_volume=2)
    utp_solution = containerManager.getContainerForReplenish("UTP Solution", required_volume=2)
    rnase_inhibitor = containerManager.getContainerForReplenish("RNase Inhibitor", required_volume=0.5)
    t7_rna_polymerase = containerManager.getContainerForReplenish("T7 RNA Polymerase", required_volume=2)
    dnase_I = containerManager.getContainerForReplenish("DNase I", required_volume=5)
    dna_digestion_buffer = containerManager.getContainerForReplenish("DNA Digestion Buffer", required_volume=5)
    rna_magbinding_buffer = containerManager.getContainerForReplenish("RNA MagBinding Buffer", required_volume=150)
    magbinding_beads = containerManager.getContainerForReplenish("MagBinding Beads", required_volume=15)
    isopropanol = containerManager.getContainerForReplenish("Isopropanol", required_volume=250)
    rna_prep_buffer = containerManager.getContainerForReplenish("RNA Prep Buffer", required_volume=500)
    ethanol = containerManager.getContainerForReplenish("Ethanol", required_volume=1000)

    # --- Part 1: Template DNA Preparation ---

    # Step 1.1: Annealing reaction
    print("Part 1: Preparing double-stranded Oligo DNA template.")
    annealing_tube = containerManager.newContainer(ContainerType.P200, label="annealing_reaction")
    
    pipette.move(annealing_tube, annealing_buffer, 2)
    pipette.move(annealing_tube, oligo_1, 2)
    pipette.move(annealing_tube, oligo_2, 2)
    pipette.move(annealing_tube, nuclease_free_water, 14)
    
    robot.moveContainer(thermal_cycler.location, annealing_tube)
    
    # Heat at 95C for 2 mins, cool to 25C over 45 mins, hold at 25C for 10 mins
    # Ramp rate calculation: (95-25)C / (45*60)s = 70/2700 C/s ~ 0.026 C/s
    thermal_cycler.start_thermocycling(
        initial_denaturation=(95, 120),
        cycles=1,
        steps=[(25, 600, 0.026)], # (temp, hold_time, ramp_rate)
    )
    
    dsDNA_template_tube = annealing_tube
    robot.moveContainer(DEFAULT_CONTAINERHOLDER_LOCATION, dsDNA_template_tube)
    print("dsDNA template prepared.")

    # --- Part 2: Single-Strand RNA In Vitro Transcription ---
    
    # Step 2.1: Assemble transcription reaction mix
    print("Part 2: Assembling and performing in vitro transcription.")
    transcription_tube = containerManager.newContainer(ContainerType.P200, label="transcription_reaction")
    
    pipette.move(transcription_tube, transcription_buffer, 2)
    pipette.move(transcription_tube, atp_solution, 2)
    pipette.move(transcription_tube, gtp_solution, 2)
    pipette.move(transcription_tube, ctp_solution, 2)
    pipette.move(transcription_tube, utp_solution, 2)
    pipette.move(transcription_tube, rnase_inhibitor, 0.5)
    pipette.move(transcription_tube, t7_rna_polymerase, 2)
    # Add nuclease-free water to reach final volume before template
    # Final volume 20 ul. Current volume = 12.5 ul. Template is 1 ul. Water = 20 - 12.5 - 1 = 6.5 ul
    pipette.move(transcription_tube, nuclease_free_water, 6.5)
    
    # Add template DNA last
    pipette.move(transcription_tube, dsDNA_template_tube, 1)

    pipette.mix(transcription_tube)
    
    robot.moveContainer(centrifuge.location, transcription_tube)
    centrifuge.start(30) # Brief centrifugation
    robot.moveContainer(DEFAULT_CONTAINERHOLDER_LOCATION, transcription_tube)

    # Step 2.2: Perform transcription reaction
    robot.moveContainer(thermal_cycler.location, transcription_tube)
    thermal_cycler.start_isothermal(temp=42, time=7200) # 42C for 2 hours
    robot.moveContainer(DEFAULT_CONTAINERHOLDER_LOCATION, transcription_tube)
    print("Transcription reaction complete.")

    # --- Part 3: Post-Transcription Nuclease Treatment ---
    
    # Step 3.1: DNase I Digestion
    print("Part 3: Performing DNase I digestion.")
    dnase_tube = containerManager.newContainer(ContainerType.P200, label="dnase_digestion")
    
    # Move RNA sample and adjust volume to 40 ul
    pipette.move(dnase_tube, transcription_tube, 20)
    pipette.move(dnase_tube, nuclease_free_water, 20)
    
    # Add digestion buffer and DNase I
    pipette.move(dnase_tube, dna_digestion_buffer, 5)
    pipette.move(dnase_tube, dnase_I, 5)

    pipette.mix(dnase_tube, time=5) # Gentle mix

    # robot.moveContainer(heatershaker.location, dnase_tube)
    # heatershaker.set_temp(25) # Room temperature
    # heatershaker.set_shake_speed(0)
    # heatershaker.start(900) # 15 minutes
    # robot.moveContainer(DEFAULT_CONTAINERHOLDER_LOCATION, dnase_tube)
    timer.wait(900) # Wait for 15 minutes
    print("DNase I digestion complete.")

    # --- Part 4: Single-Strand RNA Purification ---
    print("Part 4: Purifying RNA with magnetic beads.")
    purification_tube = containerManager.newContainer(ContainerType.P1500, label="purification")
    waste_container = containerManager.newContainer(ContainerType.P1500, label="waste")

    # Binding
    pipette.move(purification_tube, dnase_tube, 40)
    pipette.move(purification_tube, rna_magbinding_buffer, 150)
    pipette.mix(purification_tube)
    pipette.move(purification_tube, magbinding_beads, 15)
    pipette.mix(purification_tube)
    pipette.move(purification_tube, isopropanol, 250)
    
    pipette.mix(purification_tube, 900)
    # robot.moveContainer(heatershaker.location, purification_tube)
    # heatershaker.set_shake_speed(1300)
    # heatershaker.start(900) # Mix for 15 mins
    
    # First Pellet
    robot.moveContainer(mag_rack.location, purification_tube)
    mag_rack.wait(120)
    pipette.move(waste_container, purification_tube, purification_tube.volume)
    robot.moveContainer(DEFAULT_CONTAINERHOLDER_LOCATION, purification_tube)

    # RNA Prep Buffer Wash
    pipette.move(purification_tube, rna_prep_buffer, 500)
    pipette.mix(purification_tube)
    robot.moveContainer(mag_rack.location, purification_tube)
    mag_rack.wait(120)
    pipette.move(waste_container, purification_tube, 500)
    robot.moveContainer(DEFAULT_CONTAINERHOLDER_LOCATION, purification_tube)

    # Ethanol Wash 1
    pipette.move(purification_tube, ethanol, 500)
    pipette.mix(purification_tube)
    robot.moveContainer(mag_rack.location, purification_tube)
    mag_rack.wait(120)
    pipette.move(waste_container, purification_tube, 500)
    robot.moveContainer(DEFAULT_CONTAINERHOLDER_LOCATION, purification_tube)

    # Ethanol Wash 2 with tube transfer
    new_purification_tube = containerManager.newContainer(ContainerType.P1500, label="purification_tube_2")
    pipette.move(purification_tube, ethanol, 500)
    pipette.mix(purification_tube)
    pipette.move(new_purification_tube, purification_tube, 500) # Transfer beads and liquid
    
    robot.moveContainer(mag_rack.location, new_purification_tube)
    mag_rack.wait(120)
    pipette.move(waste_container, new_purification_tube, 500)
    robot.moveContainer(DEFAULT_CONTAINERHOLDER_LOCATION, new_purification_tube)

    # Dry beads
    # robot.moveContainer(heatershaker.location, new_purification_tube)
    # heatershaker.set_shake_speed(0)
    # heatershaker.start(600) # Dry for 10 mins
    # robot.moveContainer(DEFAULT_CONTAINERHOLDER_LOCATION, new_purification_tube)
    
    timer.wait(600) # Wait for 10 minutes

    # Elute RNA
    pipette.move(new_purification_tube, nuclease_free_water, 15)
    # robot.moveContainer(heatershaker.location, new_purification_tube)
    pipette.mix(new_purification_tube, 300)
    # heatershaker.set_shake_speed(1300)
    # heatershaker.start(300) # Mix for 5 mins
    
    robot.moveContainer(mag_rack.location, new_purification_tube)
    mag_rack.wait(120)
    
    # Collect supernatant
    final_rna_product_tube = containerManager.newContainer(ContainerType.P200, label="final_rna_product")
    pipette.move(final_rna_product_tube, new_purification_tube, 15)
    
    print("RNA purification complete.")
    print(f"Final purified RNA is in container: {final_rna_product_tube.label} (Index: {final_rna_product_tube.index})")

if __name__ == "__main__":
    run_protocol()
### SCRIPT END ###

3. **DNA Synthesis Workflow:**
### SCRIPT START ###

# Path Description: This path consistently uses Lysis Buffer for all pre-cleavage (x.2) and post-deblocking (x.4) wash steps.
from lab_modules import Tube, TubeType, TubeManager, Pipette, Robot, MagRack, HeaterShaker, Location, DEFAULT_TUBEHOLDER_LOCATION

def wash_beads(beads_tube: Tube, wash_buffer_tube: Tube, waste_tube: Tube, instruments: dict, num_washes: int = 3, wash_volume: float = 200.0):
    """
    Performs a series of washes on magnetic beads.
    Leaves the beads pelleted and ready for the next step.
    """
    pipette = instruments['pipette']
    robot = instruments['robot']
    magrack = instruments['magrack']

    print(f"Starting wash process with {num_washes} washes.")
    for i in range(num_washes):
        # Move to magnet and wait for separation
        robot.moveTube(magrack.location, beads_tube)
        magrack.wait(60)

        # Remove supernatant
        supernatant_volume = beads_tube.volume
        if supernatant_volume > 0:
            pipette.move(dst_tube=waste_tube, src_tube=beads_tube, volume=supernatant_volume)

        # Move off magnet and resuspend in wash buffer
        robot.moveTube(DEFAULT_TUBEHOLDER_LOCATION, beads_tube)
        pipette.move(dst_tube=beads_tube, src_tube=wash_buffer_tube, volume=wash_volume)
        pipette.mix(beads_tube, time=30)
        print(f"  Wash cycle {i+1}/{num_washes} complete.")

    # Final separation to remove the last wash buffer
    robot.moveTube(magrack.location, beads_tube)
    magrack.wait(60)
    supernatant_volume = beads_tube.volume
    if supernatant_volume > 0:
        pipette.move(dst_tube=waste_tube, src_tube=beads_tube, volume=supernatant_volume)
    robot.moveTube(DEFAULT_TUBEHOLDER_LOCATION, beads_tube)
    print("Bead washing complete. Beads are pelleted.")

def synthesis_cycle(beads_tube: Tube, dntp_tube: Tube, cycle_name: str, reagents: dict, instruments: dict):
    """
    Executes one full synthesis cycle: extension, wash, deblocking, wash.
    """
    pipette = instruments['pipette']
    robot = instruments['robot']
    heatershaker = instruments['heatershaker']

    print(f"--- Starting Synthesis Cycle: {cycle_name} ---")

    # Step 1: TdT-mutant mediated extension (Coupling)
    print(f"Step 1.1: Extension")
    # Add reagents to the pelleted beads tube for a 200 uL final reaction volume
    pipette.move(dst_tube=beads_tube, src_tube=reagents['water'], volume=106.0)
    pipette.move(dst_tube=beads_tube, src_tube=reagents['phosphate_buffer'], volume=20.0)
    pipette.move(dst_tube=beads_tube, src_tube=reagents['nacl'], volume=20.0)
    pipette.move(dst_tube=beads_tube, src_tube=reagents['cocl2'], volume=40.0)
    pipette.move(dst_tube=beads_tube, src_tube=dntp_tube, volume=10.0)
    pipette.mix(beads_tube, time=15)

    # Initiate reaction by adding enzyme
    pipette.move(dst_tube=beads_tube, src_tube=reagents['tdt_enzyme'], volume=4.0)
    pipette.mix(beads_tube, time=15)

    # Incubate
    robot.moveTube(heatershaker.location, beads_tube)
    heatershaker.set_temp(30)
    heatershaker.set_shake_speed(1300)
    heatershaker.start(10 * 60)  # 10 minutes
    robot.moveTube(DEFAULT_TUBEHOLDER_LOCATION, beads_tube)

    # Step 2: Pre-cleavage wash
    print(f"Step 1.2: Pre-cleavage wash")
    wash_beads(beads_tube, reagents['lysis_buffer'], reagents['waste'], instruments, num_washes=3)

    # Step 3: Deblocking
    print(f"Step 1.3: Deblocking")
    pipette.move(dst_tube=beads_tube, src_tube=reagents['cleavage_buffer'], volume=200.0)
    pipette.mix(beads_tube, time=15)

    # Incubate
    robot.moveTube(heatershaker.location, beads_tube)
    heatershaker.set_temp(25)  # Room temperature
    heatershaker.set_shake_speed(1300)
    heatershaker.start(5 * 60)  # 5 minutes
    robot.moveTube(DEFAULT_TUBEHOLDER_LOCATION, beads_tube)

    # Step 4: Post-deblocking wash
    print(f"Step 1.4: Post-deblocking wash")
    wash_beads(beads_tube, reagents['lysis_buffer'], reagents['waste'], instruments, num_washes=3)

    print(f"--- Cycle {cycle_name} complete ---")

def main():
    """
    Main function to run the DNA synthesis experiment.
    """
    # 1. Initialize instruments and tube manager
    tubeManager = TubeManager()
    pipette = Pipette()
    robot = Robot()
    heatershaker = HeaterShaker()
    magrack = MagRack()

    instruments = {
        'pipette': pipette,
        'robot': robot,
        'heatershaker': heatershaker,
        'magrack': magrack
    }

    # 2. Set up reagent and consumable tubes
    print("Setting up reagents...")
    reagents = {
        'beads': tubeManager.getReagentTube(name="initiator-beads", required_volume=50.0),
        'phosphate_buffer': tubeManager.getReagentTube(name="phosphate-buffer-stock", required_volume=1000.0),
        'cocl2': tubeManager.getReagentTube(name="cobalt-chloride", required_volume=1000.0),
        'nacl': tubeManager.getReagentTube(name="sodium-chloride", required_volume=1000.0),
        'tdt_enzyme': tubeManager.getReagentTube(name="tdt-enzyme", required_volume=500.0),
        'lysis_buffer': tubeManager.getReagentTube(name="lysis-buffer", required_volume=10000.0),
        'cleavage_buffer': tubeManager.getReagentTube(name="cleavage-deblocking-buffer", required_volume=5000.0),
        'water': tubeManager.getReagentTube(name="water", required_volume=10000.0),
        'waste': tubeManager.getNewTube(cap=TubeType.P50K, label="waste_tube")
    }

    # Create individual tubes for each dNTP type
    dntp_tubes = {
        'T': tubeManager.getReagentTube(name="onh2-dntp", required_volume=500.0),
        'A': tubeManager.getReagentTube(name="onh2-dntp", required_volume=500.0),
        'C': tubeManager.getReagentTube(name="onh2-dntp", required_volume=500.0),
        'G': tubeManager.getReagentTube(name="onh2-dntp", required_volume=500.0)
    }

    # 3. Define the synthesis sequence
    sequence_to_synthesize = ['T', 'A', 'C', 'T', 'A', 'T', 'G', 'G']
    
    # The starting beads are in suspension, so we pellet them and remove the storage buffer first.
    print("Preparing initial beads...")
    wash_beads(reagents['beads'], reagents['lysis_buffer'], reagents['waste'], instruments, num_washes=1)


    # 4. Run the synthesis cycles
    for i, base in enumerate(sequence_to_synthesize):
        cycle_number = i + 1
        dntp_tube = dntp_tubes[base]
        cycle_name = f"Part {cycle_number}: Addition of {base}"
        synthesis_cycle(reagents['beads'], dntp_tube, cycle_name, reagents, instruments)

    print("\nSynthesis of sequence TACTATGG is complete.")
    print(f"Final product is on the beads in tube: {reagents['beads'].index}")

if __name__ == "__main__":
    main()
### SCRIPT END ###