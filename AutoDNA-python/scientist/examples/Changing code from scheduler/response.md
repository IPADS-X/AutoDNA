```python
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
        robot.moveContainer(dst=thermal_cycler.location, container=pooled_barcoded_sample)
        thermal_cycler.start_isothermal(temp=37, time=120)
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
        robot.moveContainer(dst=thermal_cycler.location, container=adapter_ligation_tube)
        thermal_cycler.start_isothermal(temp=37, time=120)
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
```