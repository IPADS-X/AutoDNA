from lab_modules import *

def run_protocol():
    # --- Reagent and General Setup ---
    waste_container = container_manager.newContainer("waste_container", cap=ContainerType.P50K)

    # --- Part 1: DNA Repair and End-Preparation ---

    # Step 1.1: Create a single pooled DNA sample
    sample_names = [
        "final_dna_product_synthesis_ATATATCT", "final_dna_product_synthesis_ATACATCG", "final_dna_product_synthesis_ATAGTACT", 
        "final_dna_product_synthesis_ATCATGCG", "final_dna_product_synthesis_ATCTATGT", "final_dna_product_synthesis_ATCGATCT", 
        "final_dna_product_synthesis_ATGATACA", "final_dna_product_synthesis_ATGTACAC", "final_dna_product_synthesis_ATGCATCG", 
        "final_dna_product_synthesis_ACATGATA", "final_dna_product_synthesis_ACACGAGA", "final_dna_product_synthesis_ACAGAGAG", 
        "final_dna_product_synthesis_ACTATACA", "final_dna_product_synthesis_ACTCTGAT", "final_dna_product_synthesis_ACTGAGCG", 
        "final_dna_product_synthesis_ACGAGTGC", "final_dna_product_synthesis_ACGTAGCG", "final_dna_product_synthesis_ACGCGATC", 
        "final_dna_product_synthesis_AGATCTAC", "final_dna_product_synthesis_AGACTATC", "final_dna_product_synthesis_AGAGACAG", 
        "final_dna_product_synthesis_AGTATGAG", "final_dna_product_synthesis_AGTCATGA", "final_dna_product_synthesis_AGTGCGCG", 
        "final_dna_product_synthesis_AGCACGAC", "final_dna_product_synthesis_AGCTCGAG", "final_dna_product_synthesis_AGCGATCT", 
        "final_dna_product_synthesis_TATACATG", "final_dna_product_synthesis_TATCTCAC", "final_dna_product_synthesis_TATGATAT", 
        "final_dna_product_synthesis_TACAGAGA", "final_dna_product_synthesis_TACTATAT", "final_dna_product_synthesis_TACGTATG", 
        "final_dna_product_synthesis_TAGATCGA", "final_dna_product_synthesis_TAGTGATA", "final_dna_product_synthesis_TAGCTCAC", 
        "final_dna_product_synthesis_TCATGCGC", "final_dna_product_synthesis_TCACTAGA", "final_dna_product_synthesis_TCAGATAC", 
        "final_dna_product_synthesis_TCTACTGT", "final_dna_product_synthesis_TCTCGAGC", "final_dna_product_synthesis_TCTGTAGC", 
        "final_dna_product_synthesis_TCGACTGC", "final_dna_product_synthesis_TCGTAGTG", "final_dna_product_synthesis_TCGCGTGC", 
        "final_dna_product_synthesis_TGATCGCG", "final_dna_product_synthesis_TGACTCAT", "final_dna_product_synthesis_TGAGAGTG", 
        "final_dna_product_synthesis_TGTAGCAG", "final_dna_product_synthesis_TGTCGCTC", "final_dna_product_synthesis_TGTGCGTG", 
        "final_dna_product_synthesis_TGCATATG", "final_dna_product_synthesis_TGCTCAGA", "final_dna_product_synthesis_TGCGTCGC", 
        "final_dna_product_synthesis_CATAGTAG", "final_dna_product_synthesis_CATCGTCT", "final_dna_product_synthesis_CATGCGAG", 
        "final_dna_product_synthesis_CACAGATG", "final_dna_product_synthesis_CACTAGAT", "final_dna_product_synthesis_CACGATCA", 
        "final_dna_product_synthesis_CAGATAGA", "final_dna_product_synthesis_CAGTACTC", "final_dna_product_synthesis_CAGCGCAC", 
        "final_dna_product_synthesis_CTATAGCT", "final_dna_product_synthesis_CTACATAC", "final_dna_product_synthesis_CTAGATGT", 
        "final_dna_product_synthesis_CTCAGCTA", "final_dna_product_synthesis_CTCTACAT", "final_dna_product_synthesis_CTCGTACG", 
        "final_dna_product_synthesis_CTGACATA", "final_dna_product_synthesis_CTGTACAC", "final_dna_product_synthesis_CTGCGTGA", 
        "final_dna_product_synthesis_CGATGATA", "final_dna_product_synthesis_CGACGACG", "final_dna_product_synthesis_CGAGTCTA", 
        "final_dna_product_synthesis_CGTAGAGA", "final_dna_product_synthesis_CGTCACAC", "final_dna_product_synthesis_CGTGTAGT"
    ]
    source_samples = [container_manager.getContainerForReplenish(name, required_volume=1.0) for name in sample_names]
    master_dna_pool = container_manager.newContainer("master_dna_pool", cap=ContainerType.P1500)
    for sample in source_samples:
        pipette.transfer(1.0, sample, master_dna_pool)
    
    pooled_dna_sample_tube = container_manager.newContainer("pooled_dna_sample_tube", cap=ContainerType.P200)
    pipette.transfer(10.0, master_dna_pool, pooled_dna_sample_tube)
    
    h2o = container_manager.getContainerForReplenish("nuclease-free water", required_volume=100)
    pipette.transfer(1.0, h2o, pooled_dna_sample_tube)
    pipette.mix(10.0, pooled_dna_sample_tube, repetitions=10)
    robot.move_container(pooled_dna_sample_tube, centrifuge_200uL.get_location())
    centrifuge_200uL.run()
    robot.move_container(pooled_dna_sample_tube, DEFAULT_CONTAINERHOLDER_LOCATION)
    
    # Step 1.2: Prepare diluted DNA Control Sample (DCS)
    dcs_source = container_manager.getContainerForReplenish("DNA Control Sample", required_volume=35)
    eb_buffer = container_manager.getContainerForReplenish("Elution Buffer", required_volume=105)
    diluted_dcs_tube = container_manager.newContainer("diluted_DCS", cap=ContainerType.P200)
    pipette.transfer(35.0, dcs_source, diluted_dcs_tube)
    pipette.transfer(105.0, eb_buffer, diluted_dcs_tube)
    pipette.mix(100.0, diluted_dcs_tube, repetitions=10)
    robot.move_container(diluted_dcs_tube, centrifuge_200uL.get_location())
    centrifuge_200uL.run()
    robot.move_container(diluted_dcs_tube, DEFAULT_CONTAINERHOLDER_LOCATION)

    # Step 1.3: Assemble the DNA repair and end-prep reaction
    repair_buffer = container_manager.getContainerForReplenish("NEBNext FFPE DNA Repair Buffer", required_volume=0.875)
    end_prep_buffer = container_manager.getContainerForReplenish("Ultra II End-prep Reaction Buffer", required_volume=0.875)
    end_prep_enzyme = container_manager.getContainerForReplenish("Ultra II End-prep Enzyme Mix", required_volume=0.75)
    repair_mix = container_manager.getContainerForReplenish("NEBNext FFPE DNA Repair Mix", required_volume=0.5)

    pipette.transfer(1.0, diluted_dcs_tube, pooled_dna_sample_tube)
    pipette.mix(11.0, pooled_dna_sample_tube, repetitions=10)
    pipette.transfer(0.875, repair_buffer, pooled_dna_sample_tube)
    pipette.mix(12.0, pooled_dna_sample_tube, repetitions=10)
    pipette.transfer(0.875, end_prep_buffer, pooled_dna_sample_tube)
    pipette.mix(13.0, pooled_dna_sample_tube, repetitions=10)
    pipette.transfer(0.75, end_prep_enzyme, pooled_dna_sample_tube)
    pipette.mix(14.0, pooled_dna_sample_tube, repetitions=10)
    pipette.transfer(0.5, repair_mix, pooled_dna_sample_tube)
    pipette.mix(14.5, pooled_dna_sample_tube, repetitions=10)
    end_prep_reaction_mix = pooled_dna_sample_tube
    end_prep_reaction_mix.label = "end_prep_reaction_mix"

    # Step 1.4: Incubate the reaction in a thermal cycler
    robot.move_container(end_prep_reaction_mix, thermal_cycler.get_location())
    thermal_cycler.close_lid()
    protocol = [
        {"temperature_celsius": 20.0, "duration_seconds": 300},
        {"temperature_celsius": 65.0, "duration_seconds": 300},
    ]
    thermal_cycler.run_protocol(protocol)
    thermal_cycler.open_lid()
    robot.move_container(end_prep_reaction_mix, DEFAULT_CONTAINERHOLDER_LOCATION)

    # Step 1.5: Purify the end-prepped DNA
    purification_tube_1 = container_manager.newContainer("end_prep_purification_tube", cap=ContainerType.P1500)
    pipette.transfer(15.0, end_prep_reaction_mix, purification_tube_1)
    axp_beads = container_manager.getContainerForReplenish("AMPure XP Beads", required_volume=15)
    pipette.transfer(15.0, axp_beads, purification_tube_1)
    pipette.mix(25.0, purification_tube_1, repetitions=10)
    timer.wait(300)

    # Step 1.6: Wash the beads with 80% ethanol
    robot.move_container(purification_tube_1, mag_rack_1p5mL.get_location())
    mag_rack_1p5mL.separate(wait_duration_seconds=60)
    pipette.transfer(30.0, purification_tube_1, waste_container)
    
    ethanol_80 = container_manager.getContainerForReplenish("80% ethanol in nuclease-free water", required_volume=400)
    for _ in range(2):
        pipette.transfer(200.0, ethanol_80, purification_tube_1)
        timer.wait(30)
        pipette.transfer(200.0, purification_tube_1, waste_container)

    # Step 1.7: Elute the purified end-prepped DNA
    robot.move_container(purification_tube_1, centrifuge_1p5mL.get_location())
    centrifuge_1p5mL.run(speed_rpm=2000, duration_seconds=10)
    robot.move_container(purification_tube_1, mag_rack_1p5mL.get_location())
    mag_rack_1p5mL.separate(wait_duration_seconds=10)
    pipette.transfer(purification_tube_1.volume, purification_tube_1, waste_container, touch_tip=False)
    
    timer.wait(30) # Air-dry
    robot.move_container(purification_tube_1, DEFAULT_CONTAINERHOLDER_LOCATION)
    
    pipette.transfer(10.0, h2o, purification_tube_1)
    pipette.mix(8.0, purification_tube_1, repetitions=15)
    timer.wait(120)
    
    robot.move_container(purification_tube_1, mag_rack_1p5mL.get_location())
    mag_rack_1p5mL.separate(wait_duration_seconds=120)
    
    end_prepped_dna = container_manager.newContainer("end_prepped_dna", cap=ContainerType.P1500)
    pipette.transfer(10.0, purification_tube_1, end_prepped_dna)

    # --- Part 2: Native Barcode Ligation ---
    
    # Step 2.1: Assemble the barcode ligation reaction
    barcode_ligation_tube = container_manager.newContainer("barcode_ligation_tube", cap=ContainerType.P200)
    nb01 = container_manager.getContainerForReplenish("Native Barcode", required_volume=2.5)
    ligase_master_mix = container_manager.getContainerForReplenish("Blunt/TA Ligase Master Mix", required_volume=10.0)
    
    pipette.transfer(7.5, end_prepped_dna, barcode_ligation_tube)
    pipette.mix(7.0, barcode_ligation_tube, repetitions=10)
    pipette.transfer(2.5, nb01, barcode_ligation_tube)
    pipette.mix(9.0, barcode_ligation_tube, repetitions=10)
    pipette.transfer(10.0, ligase_master_mix, barcode_ligation_tube)
    pipette.mix(18.0, barcode_ligation_tube, repetitions=15)

    # Step 2.2: Incubate the barcode ligation reaction
    robot.move_container(barcode_ligation_tube, centrifuge_200uL.get_location())
    centrifuge_200uL.run()
    robot.move_container(barcode_ligation_tube, DEFAULT_CONTAINERHOLDER_LOCATION)
    timer.wait(1200)

    # Step 2.3: Stop the ligation reaction with EDTA
    edta = container_manager.getContainerForReplenish("EDTA (blue cap)", required_volume=4.0)
    pipette.transfer(4.0, edta, barcode_ligation_tube)
    pipette.mix(22.0, barcode_ligation_tube, repetitions=10)
    robot.move_container(barcode_ligation_tube, centrifuge_200uL.get_location())
    centrifuge_200uL.run()
    robot.move_container(barcode_ligation_tube, DEFAULT_CONTAINERHOLDER_LOCATION)
    stopped_ligation_mix = barcode_ligation_tube
    stopped_ligation_mix.label = "stopped_ligation_mix"

    # Step 2.4: Purify the barcoded DNA
    purification_tube_2 = container_manager.newContainer("barcode_purification_tube", cap=ContainerType.P1500)
    pipette.transfer(24.0, stopped_ligation_mix, purification_tube_2)
    axp_beads_2 = container_manager.getContainerForReplenish("AMPure XP Beads", required_volume=10.0)
    pipette.transfer(10.0, axp_beads_2, purification_tube_2)
    pipette.mix(30.0, purification_tube_2, repetitions=10)
    timer.wait(600)

    # Step 2.5: Wash the beads with 80% ethanol
    robot.move_container(purification_tube_2, mag_rack_1p5mL.get_location())
    mag_rack_1p5mL.separate(wait_duration_seconds=120)
    pipette.transfer(34.0, purification_tube_2, waste_container)
    
    ethanol_80_wash2 = container_manager.getContainerForReplenish("80% ethanol in nuclease-free water", required_volume=1400)
    for _ in range(2):
        pipette.transfer(700.0, ethanol_80_wash2, purification_tube_2)
        timer.wait(30)
        pipette.transfer(700.0, purification_tube_2, waste_container)

    # Step 2.6: Elute the purified barcoded DNA
    robot.move_container(purification_tube_2, centrifuge_1p5mL.get_location())
    centrifuge_1p5mL.run(speed_rpm=2000, duration_seconds=10)
    robot.move_container(purification_tube_2, mag_rack_1p5mL.get_location())
    mag_rack_1p5mL.separate(wait_duration_seconds=10)
    pipette.transfer(purification_tube_2.volume, purification_tube_2, waste_container, touch_tip=False)

    timer.wait(30) # Air-dry
    robot.move_container(purification_tube_2, DEFAULT_CONTAINERHOLDER_LOCATION)
    
    pipette.transfer(35.0, h2o, purification_tube_2)
    pipette.mix(30.0, purification_tube_2, repetitions=15)
    
    robot.move_container(purification_tube_2, heater_1p5mL.get_location())
    heater_1p5mL.start(temperature_celsius=37.0)
    for _ in range(5):
        timer.wait(120)
        pipette.mix(30.0, purification_tube_2, repetitions=5)
    heater_1p5mL.stop()
    robot.move_container(purification_tube_2, DEFAULT_CONTAINERHOLDER_LOCATION)
    
    robot.move_container(purification_tube_2, mag_rack_1p5mL.get_location())
    mag_rack_1p5mL.separate(wait_duration_seconds=120)
    
    barcoded_dna_pool = container_manager.newContainer("barcoded_dna_pool", cap=ContainerType.P1500)
    pipette.transfer(35.0, purification_tube_2, barcoded_dna_pool)
    
    # --- Part 3: Adapter Ligation and Clean-up ---
    
    # Step 3.1: Assemble the adapter ligation reaction
    adapter_ligation_tube = container_manager.newContainer("adapter_ligation_tube", cap=ContainerType.P1500)
    native_adapter = container_manager.getContainerForReplenish("Native Adapter", required_volume=5.0)
    ligation_buffer = container_manager.getContainerForReplenish("NEBNext Quick Ligation Reaction Buffer", required_volume=10.0)
    t4_ligase = container_manager.getContainerForReplenish("Quick T4 DNA Ligase", required_volume=5.0)
    
    pipette.transfer(30.0, barcoded_dna_pool, adapter_ligation_tube)
    pipette.mix(25.0, adapter_ligation_tube, repetitions=10)
    pipette.transfer(5.0, native_adapter, adapter_ligation_tube)
    pipette.mix(33.0, adapter_ligation_tube, repetitions=10)
    pipette.transfer(10.0, ligation_buffer, adapter_ligation_tube)
    pipette.mix(43.0, adapter_ligation_tube, repetitions=10)
    pipette.transfer(5.0, t4_ligase, adapter_ligation_tube)
    pipette.mix(45.0, adapter_ligation_tube, repetitions=15)

    # Step 3.2: Incubate the adapter ligation reaction
    robot.move_container(adapter_ligation_tube, centrifuge_1p5mL.get_location())
    centrifuge_1p5mL.run(speed_rpm=2000, duration_seconds=10)
    robot.move_container(adapter_ligation_tube, DEFAULT_CONTAINERHOLDER_LOCATION)
    timer.wait(1200)

    # Step 3.3: Purify the adapter-ligated library
    axp_beads_3 = container_manager.getContainerForReplenish("AMPure XP Beads", required_volume=20.0)
    pipette.transfer(20.0, axp_beads_3, adapter_ligation_tube)
    pipette.mix(60.0, adapter_ligation_tube, repetitions=10)
    timer.wait(600)
    
    # Step 3.4: Wash the beads with Short Fragment Buffer (SFB)
    robot.move_container(adapter_ligation_tube, mag_rack_1p5mL.get_location())
    mag_rack_1p5mL.separate(wait_duration_seconds=120)
    pipette.transfer(70.0, adapter_ligation_tube, waste_container)
    
    sfb = container_manager.getContainerForReplenish("Short Fragment Buffer", required_volume=250)
    for _ in range(2):
        pipette.transfer(125.0, sfb, adapter_ligation_tube)
        robot.move_container(adapter_ligation_tube, DEFAULT_CONTAINERHOLDER_LOCATION)
        pipette.mix(100.0, adapter_ligation_tube, repetitions=10)
        robot.move_container(adapter_ligation_tube, mag_rack_1p5mL.get_location())
        mag_rack_1p5mL.separate(wait_duration_seconds=120)
        pipette.transfer(125.0, adapter_ligation_tube, waste_container)
        
    # Step 3.5: Elute the final sequencing library
    robot.move_container(adapter_ligation_tube, centrifuge_1p5mL.get_location())
    centrifuge_1p5mL.run(speed_rpm=2000, duration_seconds=10)
    robot.move_container(adapter_ligation_tube, mag_rack_1p5mL.get_location())
    mag_rack_1p5mL.separate(wait_duration_seconds=10)
    pipette.transfer(adapter_ligation_tube.volume, adapter_ligation_tube, waste_container, touch_tip=False)
    robot.move_container(adapter_ligation_tube, DEFAULT_CONTAINERHOLDER_LOCATION)
    
    elution_buffer = container_manager.getContainerForReplenish("Elution Buffer", required_volume=15.0)
    pipette.transfer(15.0, elution_buffer, adapter_ligation_tube)
    pipette.mix(12.0, adapter_ligation_tube, repetitions=15)
    
    robot.move_container(adapter_ligation_tube, heater_1p5mL.get_location())
    heater_1p5mL.start(temperature_celsius=37.0)
    for _ in range(5):
        timer.wait(120)
        pipette.mix(12.0, adapter_ligation_tube, repetitions=5)
    heater_1p5mL.stop()
    robot.move_container(adapter_ligation_tube, DEFAULT_CONTAINERHOLDER_LOCATION)
    
    robot.move_container(adapter_ligation_tube, mag_rack_1p5mL.get_location())
    mag_rack_1p5mL.separate(wait_duration_seconds=120)
    
    final_sequencing_library = container_manager.newContainer("final_sequencing_library", cap=ContainerType.P1500)
    pipette.transfer(15.0, adapter_ligation_tube, final_sequencing_library)
    
    # --- Part 4: Flow Cell Priming and Library Loading ---
    
    # Step 4.1: Prepare the flow cell priming mix
    priming_mix_tube = container_manager.newContainer("flow_cell_priming_mix", cap=ContainerType.P1500)
    fcf = container_manager.getContainerForReplenish("Flow Cell Flush", required_volume=1170.0)
    bsa = container_manager.getContainerForReplenish("Bovine Serum Albumin", required_volume=5.0)
    fct = container_manager.getContainerForReplenish("Flow Cell Tether", required_volume=30.0)
    
    pipette.transfer(1170.0, fcf, priming_mix_tube)
    pipette.transfer(5.0, bsa, priming_mix_tube)
    pipette.transfer(30.0, fct, priming_mix_tube)
    pipette.mix(1000.0, priming_mix_tube, repetitions=10)
    
    # Step 4.2: Prime the MinION flow cell
    priming_port = Port(PortType.PRIMING)
    priming_port_tube = priming_port.get_tube()
    robot.open_port(priming_port)
    pipette.transfer(800.0, priming_mix_tube, priming_port_tube)
    timer.wait(300)
    pipette.transfer(200.0, priming_mix_tube, priming_port_tube)
    
    # Step 4.3: Prepare the DNA library for loading
    load_ready_library = container_manager.newContainer("load_ready_library", cap=ContainerType.P1500)
    sb = container_manager.getContainerForReplenish("Sequencing Buffer", required_volume=37.5)
    lis = container_manager.getContainerForReplenish("Library Solution", required_volume=25.5)
    
    pipette.transfer(37.5, sb, load_ready_library)
    pipette.transfer(25.5, lis, load_ready_library)
    pipette.transfer(12.0, final_sequencing_library, load_ready_library)
    pipette.mix(70.0, load_ready_library, repetitions=10)

    # Step 4.4: Load the library onto the flow cell
    spoton_port = Port(PortType.SPOTON)
    spoton_port_tube = spoton_port.get_tube()
    robot.open_port(spoton_port)
    pipette.transfer(75.0, load_ready_library, spoton_port_tube)

    # Step 4.5: Initiate the sequencing run
    robot.close_port(spoton_port)
    robot.close_port(priming_port)

    # Print final results
    print(f"Final sequencing library: {final_sequencing_library.label}, Volume: {final_sequencing_library.volume} uL")
    print(f"Load-ready library: {load_ready_library.label}, Volume: {load_ready_library.volume} uL")
    print("Protocol finished. Sequencing run can be started.")

if __name__ == "__main__":
    run_protocol()