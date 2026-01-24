import os
import re
import pandas as pd
import argparse
from Bio import Align

# https://biopython.org/docs/dev/Tutorial/chapter_pairwise.html#substitution-scores
aligner = Align.PairwiseAligner()
aligner.match_score = 1.0
aligner.mismatch_score = -2.0
aligner.gap_score = -2.5
primer_sequence = "AATGATACGGCGACCACCGAGATCTACACTCTTTCCCTACACGACGCTCTTCCGATCT"
target_length = 8
max_length = 10
# sample_ratio  = 0.001
sample_ratio  = 1
indexes = [i for i in range(0, 78)]


# indexes = [i for i in range(0, 5)]

# indexes = [i for i in range(5, 38)]
# indexes.remove(24)
# indexes.remove(33)
# indexes.remove(20)

# indexes = [i for i in range(38, 78)]
# indexes.append(24)  # Add index 24
# indexes.append(20)
# indexes.append(33)  # Add index 33

indexes.sort()


print(len(indexes), "indexes:", indexes)

# 4, 12, 14, 17, 26, 31
# 5, 13, 15, 18, 27, 32

# 12858, 148, 123, 21, 12, 81

all_dna_strands_ = [
    "ATATATCT", "ATACATCG", "ATAGTACT", "ATCATGCG",
    "ATCTATGT", "ATCGATCT", "ATGATACA", "ATGTACAC",
    "ATGCATCG", "ACATGATA", "ACACGAGA", "ACAGAGAG",
    "ACTATACA", "ACTCTGAT", "ACTGAGCG", "ACGAGTGC",
    "ACGTAGCG", "ACGCGATC", "AGATCTAC", "AGACTATC",
    "AGAGACAG", "AGTATGAG", "AGTCATGA", "AGTGCGCG",
    "AGCACGAC", "AGCTCGAG", "AGCGATCT", "TATACATG",
    "TATCTCAC", "TATGATAT", "TACAGAGA", "TACTATAT",
    "TACGTATG", "TAGATCGA", "TAGTGATA", "TAGCTCAC",
    "TCATGCGC", "TCACTAGA", "TCAGATAC", "TCTACTGT",
    "TCTCGAGC", "TCTGTAGC", "TCGACTGC", "TCGTAGTG",
    "TCGCGTGC", "TGATCGCG", "TGACTCAT", "TGAGAGTG",
    "TGTAGCAG", "TGTCGCTC", "TGTGCGTG", "TGCATATG",
    "TGCTCAGA", "TGCGTCGC", "CATAGTAG", "CATCGTCT",
    "CATGCGAG", "CACAGATG", "CACTAGAT", "CACGATCA",
    "CAGATAGA", "CAGTACTC", "CAGCGCAC", "CTATAGCT",
    "CTACATAC", "CTAGATGT", "CTCAGCTA", "CTCTACAT",
    "CTCGTACG", "CTGACATA", "CTGTACAC", "CTGCGTGA",
    "CGATGATA", "CGACGACG", "CGAGTCTA", "CGTAGAGA",
    "CGTCACAC", "CGTGTAGT"
]

right_sequence = 'Intelligence is the ability to adapt to change'


def index_to_sequence_ternary(index):
    digits = []
    for _ in range(4):
        digits.append(index % 3)
        index = index // 3
    # Reverse to get most significant digit first
    digits = digits[::-1]
    return ''.join(map(str, digits))

def third_string_to_binary_value(third_string):
    """
    Convert a string of '0', '1', and '2' to a binary value.
    """
    value = 0
    for char in third_string:
        value = value * 3 + int(char)
    return value

def binary_value_to_original_string(value):
    value_string = ""
    while value > 0:
        value_string = str(value % 2) + value_string
        value //= 2
    
    if len(value_string) % 8 != 0:
        # Pad with leading zeros to make it a multiple of 8 bits
        value_string = value_string.zfill((len(value_string) + 7) // 8 * 8)
    
    original_text = "".join([chr(int(value_string[i:i+8], 2)) for i in range(0, len(value_string), 8)])
    return original_text


def decode_nucleotides(sequence):
    transition = {
        ('A', 'T'): 0, ('A', 'C'): 1, ('A', 'G'): 2,
        ('T', 'A'): 0, ('T', 'C'): 1, ('T', 'G'): 2,
        ('C', 'A'): 0, ('C', 'T'): 1, ('C', 'G'): 2,
        ('G', 'A'): 0, ('G', 'T'): 1, ('G', 'C'): 2,
    }
    digits = []
    prev_nuc = None
    for nuc in sequence:
        if prev_nuc is None:
            # Initial mapping for the first nucleotide
            if nuc == 'A':
                digit = 0
            elif nuc == 'T':
                digit = 1
            else:
                digit = 2
        else:
            digit = transition[(prev_nuc, nuc)]
        digits.append(digit)
        prev_nuc = nuc
    return ''.join(map(str, digits))


def validate_sequence(sequence, index, sum_check=True, check_length=True):
    # all sequences should be 8 characters long
    if check_length and len(sequence) != target_length:
        return False
    # neighboring nucleotides should be different
    for i in range(len(sequence) - 1):
        if sequence[i] == sequence[i + 1]:
            return False
    index_digits = index_to_sequence_ternary(index)
    sequence_digits = decode_nucleotides(sequence)
    # Check if the sequence starts with the index digits
    if not sequence_digits.startswith(index_digits):
        return False
    if sum_check:
        # Check error code, which is sum of digits in front of the sequence
        error_code = sequence_digits[-1]
        sum_digits = sum(int(digit) for digit in sequence_digits)
        sum_digits = (sum_digits - int(error_code)) % 3
        if sum_digits != int(error_code):
            return False
    return True


def extract_sequence_data(sequence):
    sequence_digits = decode_nucleotides(sequence)
    data = sequence_digits[4:7]
    return data


def find_most_possible_sequence(result, index):
    final_sequence_index = -1
    next_not_pass_index = -1
    next_pass_index = -1
    total_count = 0
    for i, row in result.iterrows():
        sequence = row[0]
        count = row[1]
        # check if the sequence is valid
        if validate_sequence(sequence, index):
            # record all the sequences that are valid
            total_count += count
            if final_sequence_index == -1:
                final_sequence_index = i
            elif next_pass_index == -1:
                next_pass_index = i
            # if passed, append True to the result
            result.at[i, 2] = index
            # the real correct sequence
            result.at[i, 3] = result.iloc[final_sequence_index][0]
            result.at[i, 4] = True
        elif validate_sequence(sequence, index, sum_check=False, check_length=False):
            result.at[i, 2] = index
            if next_not_pass_index == -1:
                next_not_pass_index = i
            result.at[i, 3] = all_dna_strands_[index]
            result.at[i, 4] = False
    return final_sequence_index, total_count, next_not_pass_index, next_pass_index


def read_all_files(dir_path):
    file_content_list = []
    for file in os.listdir(dir_path):
        # read content of the file
        with open(os.path.join(dir_path, file), 'r') as f:
            content = f.read()
            # find all lines
            for line in content.split('\n'):
                file_content_list.append(line)
    result = pd.Series(file_content_list)
    return result


def extract_synthesized_sequence(result):
    # remove all the rows that do not match (.*?)AAAAAAAA
    # result = result[result.str.contains(r'.*?' + primer_sequence + r'.*?AAAAAAAA', regex=True)]
    result = result[result.str.contains('AAAA', regex=True)]
    # result = result[result.str.contains('AAAAAAAA', regex=True)]
    result = result[result.str.contains(primer_sequence, regex=True)]
    # remove all caracters before primer_sequence
    result = result.str.replace(r'.*?' + primer_sequence, '', regex=True)
    # remove primer sequence
    result = result.str.replace(primer_sequence, '', regex=True)
    # result = result.str.replace(r'AAAAAAAA', '', regex=True)
    # result = result.str.replace(r'AAAAAAAA' + r'.*', '', regex=True)
    # print(len(result), "sequences after removing primer and AAAAAAAA")

    # # 获取第target_length+1个字符不是A的数目
    # non_A_value = result[result.str[target_length] != 'A']
    # # 获取第target_length个字符是A的数目
    # A_value = result[result.str[target_length - 1] == 'A'].count()

    # find how many until A occurs
    # num={0: 0, 1: 0, 2: 0, 3: 0, 4: 0, 5: 0, 6: 0, 7: 0, 8: 0, 9: 0, 10: 0, 11: 0, 12: 0, 13: 0, 14: 0, 15: 0}
    # print(len(result), "sequences after removing primer and AAAAAAAA")
    # for i in range(len(result)):
    #     sequence = result.iloc[i]
    #     for j in range(len(sequence)):
    #         # if is A and last 3 characters are A, record the index
    #         if len(sequence) > 2 + j and sequence[j] == 'A' and sequence[j+1] == 'A' and sequence[j+2] == 'A':
    #             num[j] += 1
    #             break
    #         if j > 10:
    #             break
    #     if i % 10000 == 0:
    #         print(f"Processed {i} sequences")
    # # print("number of sequences with non-A at target_length:", len(non_A_value))
    # print(num)

    # get first target_length characters
    # result = result.map(lambda x: x[0:target_length])
    # if ends with duplicated 'A', remove until the last 'A'
    def remove_trailing_A(x):
        sequence = x
        # while len(sequence) < target_length:
        #     sequence += 'A'
        # return sequence[:target_length]
            
        for j in range(len(sequence)):
            # if is A and last 2 characters are A, record the index
            if len(sequence) > 2 + j and sequence[j] == 'A' and sequence[j+1] == 'A' and sequence[j+2] == 'A':
                if j < target_length:
                    x = sequence[0:j+1]
                else:
                    x = sequence[0:j]
                break
            if j > max_length:
                x = sequence[0:j]
                break
        return x
    result = result.map(lambda x: remove_trailing_A(x))
    
    # just get first sample_ratio data
    result = result[:int(len(result) * sample_ratio)]

    result = result.value_counts()
    
    return result
    
    # # if sequence ends with A, half the count
    # def adjust_count(x):
    #     if x[0].endswith('A'):
    #         return x[0], 0.5
    #     else:
    #         return x[0], 1.0
        
    # adjusted_result = result.map(adjust_count)
    
    # return adjusted_result


def analyze_row(row):
    target_sequence = all_dna_strands_[row[2]]
    alignments = aligner.align(target_sequence, row[0])
    
    # target_sequence = "ATATATCT"
    # alignments = aligner.align(target_sequence, "ATATGCGA")
    
    best_alignment = alignments[0]
    aligned_target = best_alignment[0]
    aligned_query = best_alignment[1]

    temp_index = []
    target_index = 0
    for i in range(len(aligned_target)):
        if aligned_target[i] != '-':
            target_index += 1

        # to handle the special case:
        # target            0 -TGTGTGTG 8
        #                   0 -|||||||| 9
        # query             0 GTGTGTGTG 9
        append_index = target_index if target_index != 0 else 1
        temp_index.append(append_index)
        
    existed_index = []

    deletion_list = []
    for i in range(len(aligned_query)):
        if aligned_query[i] == '-':
            deletion_list.append(temp_index[i])
            existed_index.append(temp_index[i])

    insertion_list = []
    target_index = 0
    for i in range(len(aligned_target)):
        if aligned_target[i] == '-':
            if (len(insertion_list) == 0 or insertion_list[-1] != temp_index[i]) and temp_index[i] not in existed_index:
                insertion_list.append(temp_index[i])
                existed_index.append(temp_index[i])

    substitution_list = []
    for i, (base1, base2) in enumerate(zip(aligned_target, aligned_query)):
        if base1 != base2 and base1 != '-' and base2 != '-' and  temp_index[i] not in existed_index:
            substitution_list.append(temp_index[i])
            existed_index.append(temp_index[i])

    # count number of indexes not in deletion_list, insertion_list, substitution_list
    success_list = []
    for i in range(len(target_sequence)):
        if i+1 not in deletion_list and i+1 not in insertion_list and i+1 not in substitution_list:
            success_list.append(i+1)

    # print(best_alignment)
    # print("deletion_list: ", deletion_list)
    # print("insertion_list: ", insertion_list)
    # print("substitution_list: ", substitution_list)
    row[5] = deletion_list
    row[6] = insertion_list
    row[7] = substitution_list
    row[8] = success_list
    # wait for command line input
    # input("Press Enter to continue...")
    return row


def analize_index(result, index):
    origin_count = result[1].sum()
    target_sequence = all_dna_strands_[index]
    result = result[result[2] == index]
    total_seq_count = result[1].sum()
    total_base_count = total_seq_count * len(target_sequence)
    success_seq_count = result[1].where(
        (result[5].apply(eval).apply(len) == 0)
        & (result[6].apply(eval).apply(len) == 0)
        & (result[7].apply(eval).apply(len) == 0),
        0).sum()
    deletion_seq_count = (
        result[1] * (result[5].apply(eval).apply(len) > 0)).sum()
    insertion_seq_count = (
        result[1] * (result[6].apply(eval).apply(len) > 0)).sum()
    substitution_seq_count = (
        result[1] * (result[7].apply(eval).apply(len) > 0)).sum()
    deletion_base_count = (result[1] * result[5].apply(eval).apply(len)).sum()
    insertion_base_count = (result[1] * result[6].apply(eval).apply(len)).sum()
    substitution_base_count = (
        result[1] * result[7].apply(eval).apply(len)).sum()
    success_base_count = (result[1] * result[8].apply(eval).apply(len)).sum()
    
    # success_base_count = (result[1] * (8 - result[5].apply(eval).apply(len) -result[6].apply(eval).apply(len) - result[7].apply(eval).apply(len))).sum()

    # total_base_count = deletion_base_count + insertion_base_count + substitution_base_count + success_base_count

    prefix_matched_ratio = total_seq_count / \
        origin_count if origin_count > 0 else 0
    deletion_ratio = deletion_seq_count / \
        total_seq_count if total_seq_count > 0 else 0
    insertion_ratio = insertion_seq_count / \
        total_seq_count if total_seq_count > 0 else 0
    substitution_ratio = substitution_seq_count / \
        total_seq_count if total_seq_count > 0 else 0
    success_ratio = success_seq_count / total_seq_count if total_seq_count > 0 else 0
    deletion_base_ratio = deletion_base_count / \
        total_base_count if total_base_count > 0 else 0
    insertion_base_ratio = insertion_base_count / \
        total_base_count if total_base_count > 0 else 0
    substitution_base_ratio = substitution_base_count / \
        total_base_count if total_base_count > 0 else 0
    success_base_ratio = success_base_count / \
        total_base_count if total_base_count > 0 else 0

    stepwise_result = []
    for i in range(len(target_sequence)):
        index = i + 1
        stepwise_result.append([str(index),
                                result[1][result[5].apply(eval).apply(
                                    lambda x: index in x)].sum() / total_seq_count,
                                result[1][result[6].apply(eval).apply(
                                    lambda x: index in x)].sum() / total_seq_count,
                                result[1][result[7].apply(eval).apply(
                                    lambda x: index in x)].sum() / total_seq_count,
                                result[1][result[8].apply(eval).apply(lambda x: index in x)].sum() / total_seq_count])

    return {
        'total_count': origin_count,
        'total_prefix_match_count': total_seq_count,
        'prefix_matched_ratio': prefix_matched_ratio,
        'deletion_seq_ratio': deletion_ratio,
        'insertion_seq_ratio': insertion_ratio,
        'substitution_seq_ratio': substitution_ratio,
        'success_seq_ratio': success_ratio,
        'deletion_base_ratio': deletion_base_ratio,
        'insertion_base_ratio': insertion_base_ratio,
        'substitution_base_ratio': substitution_base_ratio,
        'success_base_ratio': success_base_ratio,
        'stepwise_result': stepwise_result
    }
    
    
def decode(results):
    # results = all_dna_strands_
    # decode the sequence
    all_dna_strands_data = []
    all_data = ""
    for i in range(len(results)):
        all_dna_strands_data.append([])
        all_dna_strands_data[i] = extract_sequence_data(results[i])
        for j in range(len(all_dna_strands_data[i])):
            all_data += all_dna_strands_data[i][j]
    
    # decode the data
    all_data = third_string_to_binary_value(all_data)
    all_data = binary_value_to_original_string(all_data)
    return all_data


if __name__ == "__main__":
    # input the directory of the csv files with command line arguments "-d"
    parser = argparse.ArgumentParser()
    parser.add_argument("-d", "--input_dir", type=str, required=True)
    parser.add_argument("-o", "--output_dir", type=str,
                        required=False, default="./output/sequence_analysis")
    parser.add_argument("-i", "--intermediate", type=str,
                        required=False, default="result")
    args = parser.parse_args()

    # create the output directory if it does not exist
    if not os.path.exists(args.output_dir):
        os.makedirs(args.output_dir)

    result_file = args.output_dir + "/" + args.intermediate + ".csv"
    index_result_file = args.output_dir + "/" + args.intermediate + "index.csv"
    result_file_1 = args.output_dir + "/" + args.intermediate + "1.csv"
    result_file_2 = args.output_dir + "/" + args.intermediate + "2.csv"
    result_file_3 = args.output_dir + "/" + args.intermediate + "3.csv"

    result = None
    # if result_file does not exist:
    if not os.path.exists(result_file):
        result = read_all_files(args.input_dir)
        result = extract_synthesized_sequence(result)
        result.to_csv(result_file, header=False)
    result = pd.read_csv(result_file, header=None)
    result = result.fillna('')
    result[2] = -1
    result[3] = ''
    result[4] = False

    index_result = None
    if not os.path.exists(index_result_file) or not os.path.exists(result_file_1):
        index_result = pd.DataFrame(columns=['index', 'most_possible_sequence',
                                             'total_count', 'total_prefix_match_count', 'prefix_matched_ratio',
                                    'most_possible_sequence_count', 'pass_error_check_count', 'most_possible_sequence_of_prefix_matched_ratio', 'most_possible_sequence_of_pass_error_check_ratio',
                                             'data', 'right_sequence_count', 'right_sequence', 'if_most_possible_sequence_is_right',
                                             'next_not_pass_num', 'next_not_pass_sequence', 'next_pass_num', 'next_pass_sequence'])
        # index_result = pd.DataFrame(columns=['index', 'most_possible_sequence',
        #                             'count', 'total_valid_count', 'ratio', 'data', 'right_sequence', 'match'])
        for i in indexes:
            print(f"Processing index {i}...")
            # find the most possible sequence
            sequence_index, total_valid_count, next_not_pass_index, next_pass_index = find_most_possible_sequence(
                result, i)
            if sequence_index == -1:
                print(f"No valid sequence found for index {i}.")
                index_result.loc[i] = [i, '',
                                   0, 0, 0,
                                   0, total_valid_count, 0, count /
                                   total_valid_count if total_valid_count > 0 else 0,
                                   '', 0, all_dna_strands_[i], all_dna_strands_[i] == sequence, -1, '', -1, '']
                continue
            sequence = result.iloc[sequence_index][0]
            right_sequence_count = result[result[0] == all_dna_strands_[i]].sum()[1]
            count = result.iloc[sequence_index][1]
            data = extract_sequence_data(sequence)
            index_result.loc[i] = [i, sequence,
                                   0, 0, 0,
                                   count, total_valid_count, 0, count /
                                   total_valid_count if total_valid_count > 0 else 0,
                                   data, right_sequence_count, all_dna_strands_[i], all_dna_strands_[i] == sequence,
                                   result.iloc[next_not_pass_index][1] if next_not_pass_index != -1 else -1, result.iloc[next_not_pass_index][0] if next_not_pass_index != -1 else '',
                                      result.iloc[next_pass_index][1] if next_pass_index != -1 else -1, result.iloc[next_pass_index][0] if next_pass_index != -1 else '']
            # index_result.loc[i] = [i, sequence, count, total_valid_count, count /
            #                        total_valid_count, data, all_dna_strands_[i], all_dna_strands_[i] == sequence]
        index_result.to_csv(index_result_file, index=False)
        result.to_csv(result_file_1, header=False, index=False)
    index_result = pd.read_csv(index_result_file)
    index_result = index_result.fillna('')
    result = pd.read_csv(result_file_1, header=None)
    result = result.fillna('')

    if not os.path.exists(result_file_2):
        # remove result[2] < 0
        result = result[result[2] >= 0]
        result = result[result[3] != '']
        result = result[result[4] == True]
        result.to_csv(result_file_2, header=False, index=False)
    result = pd.read_csv(result_file_2, header=None)
    result = result.fillna('')

    result = pd.read_csv(result_file_1, header=None)
    result = result.fillna('')
    if not os.path.exists(result_file_3):
        # remove result[2] < 0
        result = result[result[2] >= 0]
        result = result[result[3] != '']
        # result = result[result[4] == True]
        result = result.apply(analyze_row, axis=1)
        result.to_csv(result_file_3, header=False, index=False)
    result = pd.read_csv(result_file_3, header=None)
    result = result.fillna('')
    index_result = pd.read_csv(index_result_file)
    index_result = index_result.fillna('')

    for i in indexes:
        analysis = analize_index(result, i)
        index_result.loc[index_result['index'] == i,
                         'total_count'] = analysis['total_count']
        index_result.loc[index_result['index'] == i,
                         'total_prefix_match_count'] = analysis['total_prefix_match_count']
        index_result.loc[index_result['index'] == i,
                         'prefix_matched_ratio'] = analysis['prefix_matched_ratio']
        index_result.loc[index_result['index'] == i,
                         'most_possible_sequence_of_prefix_matched_ratio'] = index_result.loc[index_result['index'] == i,
                                                                                              'most_possible_sequence_count'] / analysis['total_prefix_match_count'] if analysis['total_prefix_match_count'] > 0 else 0
        index_result.loc[index_result['index'] == i,
                         'deletion_seq_ratio'] = analysis['deletion_seq_ratio']
        index_result.loc[index_result['index'] == i,
                         'insertion_seq_ratio'] = analysis['insertion_seq_ratio']
        index_result.loc[index_result['index'] == i,
                         'substitution_seq_ratio'] = analysis['substitution_seq_ratio']
        index_result.loc[index_result['index'] == i,
                         'success_seq_ratio'] = analysis['success_seq_ratio']
        index_result.loc[index_result['index'] == i,
                         'deletion_base_ratio'] = analysis['deletion_base_ratio']
        index_result.loc[index_result['index'] == i,
                         'insertion_base_ratio'] = analysis['insertion_base_ratio']
        index_result.loc[index_result['index'] == i,
                         'substitution_base_ratio'] = analysis['substitution_base_ratio']
        index_result.loc[index_result['index'] == i,
                         'success_base_ratio'] = analysis['success_base_ratio']
        for j, step in enumerate(analysis['stepwise_result']):
            index_result.loc[index_result['index'] == i,
                             f'step_{j+1}_deletion_ratio'] = step[1]
            index_result.loc[index_result['index'] == i,
                             f'step_{j+1}_insertion_ratio'] = step[2]
            index_result.loc[index_result['index'] == i,
                             f'step_{j+1}_substitution_ratio'] = step[3]
            index_result.loc[index_result['index'] == i,
                             f'step_{j+1}_success_ratio'] = step[4]

    index_result.to_csv(index_result_file, index=False)

    print("Index result:")
    print(index_result)
    
    print("Result:")
    results = index_result['most_possible_sequence'].tolist()
    
    decode_result = decode(results)
    
    
    
    print(decode(results))
