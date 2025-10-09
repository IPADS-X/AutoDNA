#
# Path Description: Path: Option 1.1.1
from lab_modules import robot, refrigerator, container_manager, Location

def run_protocol():
    """
    Executes the protocol for short-term DNA storage.
    """
    # Step 1.1: Transfer all final DNA product containers to a designated 4°C refrigerator for storage.
    
    # List of final DNA product container labels to be stored
    dna_product_labels = [
        "final_dna_product_synthesis_ATATATCT", "final_dna_product_synthesis_ATACATCG",
        "final_dna_product_synthesis_ATAGTACT", "final_dna_product_synthesis_ATCATGCG",
        "final_dna_product_synthesis_ATCTATGT", "final_dna_product_synthesis_ATCGATCT",
        "final_dna_product_synthesis_ATGATACA", "final_dna_product_synthesis_ATGTACAC",
        "final_dna_product_synthesis_ATGCATCG", "final_dna_product_synthesis_ACATGATA",
        "final_dna_product_synthesis_ACACGAGA", "final_dna_product_synthesis_ACAGAGAG",
        "final_dna_product_synthesis_ACTATACA", "final_dna_product_synthesis_ACTCTGAT",
        "final_dna_product_synthesis_ACTGAGCG", "final_dna_product_synthesis_ACGAGTGC",
        "final_dna_product_synthesis_ACGTAGCG", "final_dna_product_synthesis_ACGCGATC",
        "final_dna_product_synthesis_AGATCTAC", "final_dna_product_synthesis_AGACTATC",
        "final_dna_product_synthesis_AGAGACAG", "final_dna_product_synthesis_AGTATGAG",
        "final_dna_product_synthesis_AGTCATGA", "final_dna_product_synthesis_AGTGCGCG",
        "final_dna_product_synthesis_AGCACGAC", "final_dna_product_synthesis_AGCTCGAG",
        "final_dna_product_synthesis_AGCGATCT", "final_dna_product_synthesis_TATACATG",
        "final_dna_product_synthesis_TATCTCAC", "final_dna_product_synthesis_TATGATAT",
        "final_dna_product_synthesis_TACAGAGA", "final_dna_product_synthesis_TACTATAT",
        "final_dna_product_synthesis_TACGTATG", "final_dna_product_synthesis_TAGATCGA",
        "final_dna_product_synthesis_TAGTGATA", "final_dna_product_synthesis_TAGCTCAC",
        "final_dna_product_synthesis_TCATGCGC", "final_dna_product_synthesis_TCACTAGA",
        "final_dna_product_synthesis_TCAGATAC", "final_dna_product_synthesis_TCTACTGT",
        "final_dna_product_synthesis_TCTCGAGC", "final_dna_product_synthesis_TCTGTAGC",
        "final_dna_product_synthesis_TCGACTGC", "final_dna_product_synthesis_TCGTAGTG",
        "final_dna_product_synthesis_TCGCGTGC", "final_dna_product_synthesis_TGATCGCG",
        "final_dna_product_synthesis_TGACTCAT", "final_dna_product_synthesis_TGAGAGTG",
        "final_dna_product_synthesis_TGTAGCAG", "final_dna_product_synthesis_TGTCGCTC",
        "final_dna_product_synthesis_TGTGCGTG", "final_dna_product_synthesis_TGCATATG",
        "final_dna_product_synthesis_TGCTCAGA", "final_dna_product_synthesis_TGCGTCGC",
        "final_dna_product_synthesis_CATAGTAG", "final_dna_product_synthesis_CATCGTCT",
        "final_dna_product_synthesis_CATGCGAG", "final_dna_product_synthesis_CACAGATG",
        "final_dna_product_synthesis_CACTAGAT", "final_dna_product_synthesis_CACGATCA",
        "final_dna_product_synthesis_CAGATAGA", "final_dna_product_synthesis_CAGTACTC",
        "final_dna_product_synthesis_CAGCGCAC", "final_dna_product_synthesis_CTATAGCT",
        "final_dna_product_synthesis_CTACATAC", "final_dna_product_synthesis_CTAGATGT",
        "final_dna_product_synthesis_CTCAGCTA", "final_dna_product_synthesis_CTCTACAT",
        "final_dna_product_synthesis_CTCGTACG", "final_dna_product_synthesis_CTGACATA",
        "final_dna_product_synthesis_CTGTACAC", "final_dna_product_synthesis_CTGCGTGA",
        "final_dna_product_synthesis_CGATGATA", "final_dna_product_synthesis_CGACGACG",
        "final_dna_product_synthesis_CGAGTCTA", "final_dna_product_synthesis_CGTAGAGA",
        "final_dna_product_synthesis_CGTCACAC", "final_dna_product_synthesis_CGTGTAGT"
    ]

    # Get container objects from the manager
    dna_product_containers = [
        container_manager.getContainerForReplenish(name=label) for label in dna_product_labels
    ]

    # Set refrigerator temperature to 4°C
    refrigerator.set_target_temperature(4.0)
    refrigerator_location = refrigerator.get_location()

    # Move each container to the refrigerator
    for container in dna_product_containers:
        robot.move_container(container, refrigerator_location)
        
    print("All final DNA products have been transferred to the 4°C refrigerator for storage.")
    for container in dna_product_containers:
        print(f"Container '{container.label}' is now in the refrigerator.")

if __name__ == "__main__":
    run_protocol()