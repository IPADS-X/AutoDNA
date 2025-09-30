### SCRIPT START ###
# Path Description: This script follows the single, detailed algorithm provided for encoding a sentence into DNA.
import math

# --- Helper function for base conversion ---
def int_to_base(n, base):
    """Converts a non-negative integer to its string representation in a given base."""
    if n == 0:
        return '0'
    digits = []
    while n > 0:
        digits.append(str(n % base))
        n //= base
    return "".join(reversed(digits))

# --- Input and Constants as per the scheme ---
SENTENCE = "Intelligence is the ability to adapt to change"

DNA_MAP = {
    'A': {'0': 'T', '1': 'C', '2': 'G'},
    'T': {'0': 'A', '1': 'C', '2': 'G'},
    'C': {'0': 'A', '1': 'T', '2': 'G'},
    'G': {'0': 'A', '1': 'T', '2': 'C'}
}

INDEX_TRIT_COUNT = 4
DATA_TRIT_COUNT = 3

# --- Procedure ---

# Part 1: Data Preparation (Text -> Binary -> Ternary)

# 1. Convert Sentence to a Single Binary Stream
binary_stream = ""
for char in SENTENCE:
    # Get ASCII value and convert to 8-bit binary representation
    ascii_val = ord(char)
    binary_char = format(ascii_val, '08b')
    binary_stream += binary_char

# 2. Convert Binary Stream to Ternary Stream
# Treat the entire binary stream as one large number
large_integer_from_binary = int(binary_stream, 2)
# Convert this large integer to its base-3 (ternary) representation
ternary_data_stream = int_to_base(large_integer_from_binary, 3)

# Part 2: Segmentation and Structuring

# 3. Pad the Ternary Data Stream
# Calculate the number of strands needed
num_strands = math.ceil(len(ternary_data_stream) / DATA_TRIT_COUNT)
# Calculate the total length required for the data trits after padding
padded_length = num_strands * DATA_TRIT_COUNT
# Add leading '0's until the stream reaches the required length
padded_ternary_data_stream = ternary_data_stream.zfill(padded_length)

# 4. Generate Source Trit Segments
source_segments = []
for i in range(num_strands):
    # i. Get Data Chunk
    start_index = i * DATA_TRIT_COUNT
    end_index = start_index + DATA_TRIT_COUNT
    data_chunk = padded_ternary_data_stream[start_index:end_index]

    # ii. Get Index Trits
    index_in_ternary = int_to_base(i, 3)
    index_trits = index_in_ternary.zfill(INDEX_TRIT_COUNT)

    # iii. Combine Index and Data to form a 7-trit source segment
    source_segment = index_trits + data_chunk
    
    # iv. Add the segment to the list
    source_segments.append(source_segment)

# Part 3: Error Correction and DNA Conversion

# 5. Generate Final DNA Strands
final_dna_strands = []
for source_segment in source_segments:
    # i. Calculate Error Trit
    sum_of_trits = sum(int(trit) for trit in source_segment)
    error_trit = str(sum_of_trits % 3)

    # ii. Form 8-Trit Strand
    eight_trit_strand = source_segment + error_trit

    # iii. Convert to DNA
    previous_base = 'G'  # Initial base as per the rules
    dna_strand = ""
    for trit in eight_trit_strand:
        current_base = DNA_MAP[previous_base][trit]
        dna_strand += current_base
        previous_base = current_base
        
    # iv. Add the completed DNA strand to the final list
    final_dna_strands.append(dna_strand)

# --- Output ---
# Print the final list of 8-base-long DNA strings
print("Encoded DNA Strands:")
for i, strand in enumerate(final_dna_strands):
    print(f"Strand {i:02d}: {strand}")

# You can also print the list directly
# print(final_dna_strands)
### SCRIPT START ###