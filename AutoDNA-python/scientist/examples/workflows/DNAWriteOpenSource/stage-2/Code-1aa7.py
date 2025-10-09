from lab_modules import (
    container_manager,
    pipette,
    robot,
    heater_shaker,
    mag_rack_200uL,
    ContainerType,
    DEFAULT_CONTAINERHOLDER_LOCATION
)

def wash_beads(beads_tube, supernatant_volume, wash_buffer_1x, waste_tube, wash_volume=100.0, repeats=3):
    """
    Performs magnetic bead washing cycles.
    First, removes the supernatant from the previous reaction.
    Then, performs a specified number of wash cycles.
    """
    # Move to magnet to pellet beads
    robot.move_container(beads_tube, mag_rack_200uL.get_location())
    mag_rack_200uL.separate(duration_seconds=60)
    
    # Remove previous supernatant
    if beads_tube.volume > 0:
        pipette.transfer(supernatant_volume, beads_tube, waste_tube)
    
    # Perform wash cycles
    for _ in range(repeats):
        # Move off magnet to resuspend
        robot.move_container(beads_tube, DEFAULT_CONTAINERHOLDER_LOCATION)
        pipette.transfer(wash_volume, wash_buffer_1x, beads_tube)
        pipette.mix(volume=wash_volume * 0.8, location=beads_tube, repetitions=15)

        # Move back to magnet to separate
        robot.move_container(beads_tube, mag_rack_200uL.get_location())
        mag_rack_200uL.separate(duration_seconds=60)
        pipette.transfer(wash_volume, beads_tube, waste_tube)

    # Move tube off magnet for the next protocol step
    robot.move_container(beads_tube, DEFAULT_CONTAINERHOLDER_LOCATION)

def synthesize_dna_sequence(sequence: str, reagents: dict, wash_buffer_1x, waste_tube):
    """
    Synthesizes a given DNA sequence on magnetic beads.
    """
    reaction_tube = container_manager.newContainer(
        label=f"synthesis_{sequence}",
        cap=ContainerType.P200
    )
    
    # 1. Prepare beads: Dispense and wash to remove storage buffer
    initial_bead_volume = 20.0
    pipette.transfer(initial_bead_volume, reagents['beads'], reaction_tube)
    wash_beads(
        beads_tube=reaction_tube,
        supernatant_volume=initial_bead_volume,
        wash_buffer_1x=wash_buffer_1x,
        waste_tube=waste_tube,
        repeats=2
    )

    # 2. Main synthesis loop for each nucleotide in the sequence
    for nucleotide in sequence:
        # Step 1.1: Coupling Reaction
        dntp_container = reagents['dntps'][nucleotide]
        
        pipette.transfer(10.0, reagents['reaction_buffer_10x'], reaction_tube)
        pipette.transfer(4.0, reagents['zatdt'], reaction_tube)
        pipette.transfer(10.0, dntp_container, reaction_tube)
        pipette.transfer(20.0, reagents['cocl2'], reaction_tube)
        pipette.transfer(56.0, reagents['water'], reaction_tube)
        pipette.mix(volume=80.0, location=reaction_tube, repetitions=15)
        
        robot.move_container(reaction_tube, heater_shaker.get_location())
        heater_shaker.incubate(
            target_temperature_celsius=30.0,
            target_speed_rpm=1300,
            duration_seconds=300
        )
        robot.move_container(reaction_tube, DEFAULT_CONTAINERHOLDER_LOCATION)

        # Step 1.2: Post-Coupling Wash
        wash_beads(
            beads_tube=reaction_tube,
            supernatant_volume=100.0,
            wash_buffer_1x=wash_buffer_1x,
            waste_tube=waste_tube,
            repeats=3
        )

        # Step 1.3: Deblocking Reaction
        pipette.transfer(100.0, reagents['deblocking_buffer'], reaction_tube)
        pipette.mix(volume=80.0, location=reaction_tube, repetitions=15)

        robot.move_container(reaction_tube, heater_shaker.get_location())
        heater_shaker.incubate(
            target_temperature_celsius=25.0,
            target_speed_rpm=1300,
            duration_seconds=300
        )
        robot.move_container(reaction_tube, DEFAULT_CONTAINERHOLDER_LOCATION)
        
        # Step 1.4: Post-Deblocking Wash
        wash_beads(
            beads_tube=reaction_tube,
            supernatant_volume=100.0,
            wash_buffer_1x=wash_buffer_1x,
            waste_tube=waste_tube,
            repeats=3
        )
        
    return reaction_tube

def main():
    TARGET_SEQUENCES = [
        "ATATATCT", "ATACATCG", "ATAGTACT", "ATCATGCG", "ATCTATGT", "ATCGATCT", "ATGATACA", "ATGTACAC", 
        "ATGCATCG", "ACATGATA", "ACACGAGA", "ACAGAGAG", "ACTATACA", "ACTCTGAT", "ACTGAGCG", "ACGAGTGC", 
        "ACGTAGCG", "ACGCGATC", "AGATCTAC", "AGACTATC", "AGAGACAG", "AGTATGAG", "AGTCATGA", "AGTGCGCG", 
        "AGCACGAC", "AGCTCGAG", "AGCGATCT", "TATACATG", "TATCTCAC", "TATGATAT", "TACAGAGA", "TACTATAT", 
        "TACGTATG", "TAGATCGA", "TAGTGATA", "TAGCTCAC", "TCATGCGC", "TCACTAGA", "TCAGATAC", "TCTACTGT", 
        "TCTCGAGC", "TCTGTAGC", "TCGACTGC", "TCGTAGTG", "TCGCGTGC", "TGATCGCG", "TGACTCAT", "TGAGAGTG", 
        "TGTAGCAG", "TGTCGCTC", "TGTGCGTG", "TGCATATG", "TGCTCAGA", "TGCGTCGC", "CATAGTAG", "CATCGTCT", 
        "CATGCGAG", "CACAGATG", "CACTAGAT", "CACGATCA", "CAGATAGA", "CAGTACTC", "CAGCGCAC", "CTATAGCT", 
        "CTACATAC", "CTAGATGT", "CTCAGCTA", "CTCTACAT", "CTCGTACG", "CTGACATA", "CTGTACAC", "CTGCGTGA", 
        "CGATGATA", "CGACGACG", "CGAGTCTA", "CGTAGAGA", "CGTCACAC", "CGTGTAGT"
    ]

    reagents = {
        'beads': container_manager.getContainerForReplenish("Initiator-immobilized magnetic beads"),
        'zatdt': container_manager.getContainerForReplenish("ZaTdT-R335L-K337G"),
        'cocl2': container_manager.getContainerForReplenish("Cobalt Chloride"),
        'reaction_buffer_10x': container_manager.getContainerForReplenish("Reaction Buffer"),
        'deblocking_buffer': container_manager.getContainerForReplenish("Deblocking Buffer"),
        'bw_buffer_2x': container_manager.getContainerForReplenish("B&W Buffer"),
        'water': container_manager.getContainerForReplenish("Nuclease-free water"),
        'dntps': {
            'A': container_manager.getContainerForReplenish("3'-ONH2-dATP"),
            'C': container_manager.getContainerForReplenish("3'-ONH2-dCTP"),
            'G': container_manager.getContainerForReplenish("3'-ONH2-dGTP"),
            'T': container_manager.getContainerForReplenish("3'-ONH2-dTTP"),
        }
    }

    waste_tube = container_manager.newContainer("waste_tube", cap=ContainerType.P50K)

    final_products = []
    for sequence in TARGET_SEQUENCES:
        # Prepare a fresh, sufficient batch of wash buffer for each synthesis run
        bw_buffer_1x = container_manager.newContainer("1X B&W Buffer", cap=ContainerType.P50K)
        pipette.transfer(2500.0, reagents['bw_buffer_2x'], bw_buffer_1x)
        pipette.transfer(2500.0, reagents['water'], bw_buffer_1x)
        pipette.mix(volume=4000.0, location=bw_buffer_1x, repetitions=10)

        final_product_tube = synthesize_dna_sequence(sequence, reagents, bw_buffer_1x, waste_tube)
        final_products.append(final_product_tube)
    
    print("DNA Synthesis Protocol Complete. Final Products:")
    for tube in final_products:
        print(f"Container Label: {tube.label}, Final Volume: {tube.volume}uL")

if __name__ == "__main__":
    main()