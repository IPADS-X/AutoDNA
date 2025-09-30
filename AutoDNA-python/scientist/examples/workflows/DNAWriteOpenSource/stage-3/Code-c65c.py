from lab_modules import *

def run_protocol():
    input_dna_labels = [
        "synthesis_ATATATCT", "synthesis_ATACATCG", "synthesis_ATAGTACT", "synthesis_ATCATGCG", "synthesis_ATCTATGT", "synthesis_ATCGATCT", "synthesis_ATGATACA", "synthesis_ATGTACAC", "synthesis_ATGCATCG", "synthesis_ACATGATA",
        "synthesis_ACACGAGA", "synthesis_ACAGAGAG", "synthesis_ACTATACA", "synthesis_ACTCTGAT", "synthesis_ACTGAGCG", "synthesis_ACGAGTGC", "synthesis_ACGTAGCG", "synthesis_ACGCGATC", "synthesis_AGATCTAC", "synthesis_AGACTATC",
        "synthesis_AGAGACAG", "synthesis_AGTATGAG", "synthesis_AGTCATGA", "synthesis_AGTGCGCG", "synthesis_AGCACGAC", "synthesis_AGCTCGAG", "synthesis_AGCGATCT", "synthesis_TATACATG", "synthesis_TATCTCAC", "synthesis_TATGATAT",
        "synthesis_TACAGAGA", "synthesis_TACTATAT", "synthesis_TACGTATG", "synthesis_TAGATCGA", "synthesis_TAGTGATA", "synthesis_TAGCTCAC", "synthesis_TCATGCGC", "synthesis_TCACTAGA", "synthesis_TCAGATAC", "synthesis_TCTACTGT",
        "synthesis_TCTCGAGC", "synthesis_TCTGTAGC", "synthesis_TCGACTGC", "synthesis_TCGTAGTG", "synthesis_TCGCGTGC", "synthesis_TGATCGCG", "synthesis_TGACTCAT", "synthesis_TGAGAGTG", "synthesis_TGTAGCAG", "synthesis_TGTCGCTC",
        "synthesis_TGTGCGTG", "synthesis_TGCATATG", "synthesis_TGCTCAGA", "synthesis_TGCGTCGC", "synthesis_CATAGTAG", "synthesis_CATCGTCT", "synthesis_CATGCGAG", "synthesis_CACAGATG", "synthesis_CACTAGAT", "synthesis_CACGATCA",
        "synthesis_CAGATAGA", "synthesis_CAGTACTC", "synthesis_CAGCGCAC", "synthesis_CTATAGCT", "synthesis_CTACATAC", "synthesis_CTAGATGT", "synthesis_CTCAGCTA", "synthesis_CTCTACAT", "synthesis_CTCGTACG", "synthesis_CTGACATA",
        "synthesis_CTGTACAC", "synthesis_CTGCGTGA", "synthesis_CGATGATA", "synthesis_CGACGACG", "synthesis_CGAGTCTA", "synthesis_CGTAGAGA", "synthesis_CGTCACAC", "synthesis_CGTGTAGT"
    ]

    final_products = []
    
    # Common Reagents and Waste
    waste_container = container_manager.newContainer(label="waste_container", cap=ContainerType.P50K)
    nuclease_free_water = container_manager.getContainerForReplenish("Nuclease-free water")
    tdt_buffer = container_manager.getContainerForReplenish("TdT Reaction Buffer")
    cocl2 = container_manager.getContainerForReplenish("Cobalt Chloride")
    datp = container_manager.getContainerForReplenish("dATP")
    tdt_enzyme = container_manager.getContainerForReplenish("TdT")
    phanta_max_mix = container_manager.getContainerForReplenish("Phanta Max Master Mix")
    polyt_primer = container_manager.getContainerForReplenish("polyT primer")
    p5_primer = container_manager.getContainerForReplenish("P5 primer")
    p7_i23_primer = container_manager.getContainerForReplenish("P7-i23 primer")
    spri_beads = container_manager.getContainerForReplenish("VAHTS DNA Clean Beads")
    ethanol_80 = container_manager.getContainerForReplenish("80% ethanol")

    for i, dna_label in enumerate(input_dna_labels):
        # Part 1: Poly(A) Tailing
        # Step 1.1: Prepare the synthesized DNA for reaction.
        dna_beads_tube = container_manager.getContainerForReplenish(name=dna_label, required_volume=50) # Assuming some initial buffer
        robot.move_container(dna_beads_tube, mag_rack_200uL.get_location())
        mag_rack_200uL.separate(duration_seconds=60)
        pipette.transfer(volume=dna_beads_tube.volume, source=dna_beads_tube, destination=waste_container)
        robot.move_container(dna_beads_tube, DEFAULT_CONTAINERHOLDER_LOCATION)

        # Step 1.2: Prepare the Poly(A) Tailing reaction mixture.
        pipette.transfer(volume=5, source=tdt_buffer, destination=dna_beads_tube)
        pipette.transfer(volume=5, source=cocl2, destination=dna_beads_tube)
        pipette.transfer(volume=0.5, source=datp, destination=dna_beads_tube)
        pipette.transfer(volume=0.5, source=tdt_enzyme, destination=dna_beads_tube)
        pipette.transfer(volume=39, source=nuclease_free_water, destination=dna_beads_tube)

        # Step 1.3: Incubate the Poly(A) Tailing reaction.
        robot.move_container(dna_beads_tube, heater_shaker.get_location())
        heater_shaker.incubate(target_temperature_celsius=37, duration_seconds=30 * 60)
        heater_shaker.incubate(target_temperature_celsius=65, duration_seconds=10 * 60)
        heater_shaker.stop()
        robot.move_container(dna_beads_tube, DEFAULT_CONTAINERHOLDER_LOCATION)
        
        robot.move_container(dna_beads_tube, mag_rack_200uL.get_location())
        mag_rack_200uL.separate(duration_seconds=2 * 60)
        pipette.transfer(volume=dna_beads_tube.volume, source=dna_beads_tube, destination=waste_container)
        robot.move_container(dna_beads_tube, DEFAULT_CONTAINERHOLDER_LOCATION)

        # Step 1.4 & 1.5: Wash the beads post-tailing.
        for _ in range(3):
            pipette.transfer(volume=200, source=nuclease_free_water, destination=dna_beads_tube)
            pipette.mix(volume=180, location=dna_beads_tube, repetitions=10)
            robot.move_container(dna_beads_tube, mag_rack_200uL.get_location())
            mag_rack_200uL.separate(duration_seconds=2 * 60)
            pipette.transfer(volume=dna_beads_tube.volume, source=dna_beads_tube, destination=waste_container)
            robot.move_container(dna_beads_tube, DEFAULT_CONTAINERHOLDER_LOCATION)

        # Part 2: Amplification
        # Step 2.1: Prepare the first PCR reaction mixture.
        pipette.transfer(volume=25, source=phanta_max_mix, destination=dna_beads_tube)
        pipette.transfer(volume=2, source=polyt_primer, destination=dna_beads_tube)
        pipette.transfer(volume=23, source=nuclease_free_water, destination=dna_beads_tube)
        
        # Step 2.2: Perform the first PCR.
        pcr1_protocol = [
            {"temperature_celsius": 95.0, "duration_seconds": 180},
            {
                "steps": [
                    {"temperature_celsius": 95.0, "duration_seconds": 15},
                    {"temperature_celsius": 65.0, "duration_seconds": 15},
                    {"temperature_celsius": 72.0, "duration_seconds": 30},
                ],
                "count": 10,
            },
            {"temperature_celsius": 72.0, "duration_seconds": 300},
            {"temperature_celsius": 4.0, "duration_seconds": 0}, # Hold at 4C
        ]
        robot.move_container(dna_beads_tube, thermal_cycler.get_location())
        thermal_cycler.close_lid()
        thermal_cycler.run_protocol(pcr1_protocol)
        thermal_cycler.open_lid()
        robot.move_container(dna_beads_tube, DEFAULT_CONTAINERHOLDER_LOCATION)
        
        # Step 2.3: Prepare the second PCR reaction mixture.
        robot.move_container(dna_beads_tube, mag_rack_200uL.get_location())
        mag_rack_200uL.separate(duration_seconds=2 * 60)
        
        pcr2_tube = container_manager.newContainer(label=f"pcr2_tube_{i}")
        robot.move_container(pcr2_tube, DEFAULT_CONTAINERHOLDER_LOCATION)
        pipette.transfer(volume=20, source=dna_beads_tube, destination=pcr2_tube)
        robot.move_container(dna_beads_tube, DEFAULT_CONTAINERHOLDER_LOCATION) # Move first tube away

        pipette.transfer(volume=2, source=p5_primer, destination=pcr2_tube)
        pipette.transfer(volume=2, source=p7_i23_primer, destination=pcr2_tube)
        pipette.transfer(volume=26, source=nuclease_free_water, destination=pcr2_tube)

        # Step 2.4: Perform the second PCR.
        pcr2_protocol = [
            {"temperature_celsius": 95.0, "duration_seconds": 180},
            {
                "steps": [
                    {"temperature_celsius": 95.0, "duration_seconds": 15},
                    {"temperature_celsius": 65.0, "duration_seconds": 15},
                    {"temperature_celsius": 72.0, "duration_seconds": 30},
                ],
                "count": 30,
            },
            {"temperature_celsius": 72.0, "duration_seconds": 300},
            {"temperature_celsius": 4.0, "duration_seconds": 0}, # Hold at 4C
        ]
        robot.move_container(pcr2_tube, thermal_cycler.get_location())
        thermal_cycler.close_lid()
        thermal_cycler.run_protocol(pcr2_protocol)
        thermal_cycler.open_lid()
        robot.move_container(pcr2_tube, DEFAULT_CONTAINERHOLDER_LOCATION)

        # Step 2.5: Collect the final PCR product.
        robot.move_container(pcr2_tube, mag_rack_200uL.get_location())
        mag_rack_200uL.separate(duration_seconds=2 * 60)
        
        purification_tube = container_manager.newContainer(label=f"purification_tube_{i}")
        pipette.transfer(volume=pcr2_tube.volume, source=pcr2_tube, destination=purification_tube)
        robot.move_container(purification_tube, DEFAULT_CONTAINERHOLDER_LOCATION)
        
        # Part 3: Purification
        # Step 3.1: Bind DNA to purification beads.
        pipette.transfer(volume=110, source=spri_beads, destination=purification_tube)
        pipette.mix(volume=150, location=purification_tube, repetitions=30)
        
        # Step 3.2: Incubate for DNA binding.
        timer.wait(10 * 60)
        
        # Step 3.3: Separate beads from binding solution.
        robot.move_container(purification_tube, mag_rack_200uL.get_location())
        mag_rack_200uL.separate(duration_seconds=5 * 60)
        pipette.transfer(volume=purification_tube.volume, source=purification_tube, destination=waste_container)
        
        # Step 3.4 & 3.5: Wash the DNA-bound beads.
        for _ in range(2):
            pipette.transfer(volume=200, source=ethanol_80, destination=purification_tube)
            timer.wait(30)
            pipette.transfer(volume=purification_tube.volume, source=purification_tube, destination=waste_container)
            
        # Step 3.6: Dry the beads.
        timer.wait(7 * 60)
        
        # Step 3.7: Elute the purified DNA.
        robot.move_container(purification_tube, DEFAULT_CONTAINERHOLDER_LOCATION)
        pipette.transfer(volume=20, source=nuclease_free_water, destination=purification_tube)
        pipette.mix(volume=18, location=purification_tube, repetitions=15)
        timer.wait(2 * 60)
        robot.move_container(purification_tube, mag_rack_200uL.get_location())
        mag_rack_200uL.separate(duration_seconds=5 * 60)

        final_dna_tube = container_manager.newContainer(label=f"final_dna_product_{dna_label}")
        pipette.transfer(volume=purification_tube.volume, source=purification_tube, destination=final_dna_tube)
        final_products.append(final_dna_tube)

    for product in final_products:
        print(f"Container: {product.label}, Volume: {product.volume}")

run_protocol()