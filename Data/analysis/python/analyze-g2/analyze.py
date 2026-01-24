import os
import re
import pandas as pd
import argparse
from Bio import Align
import difflib

from typing import List, Optional, Dict, Tuple


def read_csv_files(dir_path):
    #aggregate all the csv files in the directory, and return only one dataframe:
    df_list = []
    for file in os.listdir(dir_path):
        if file.endswith(".csv"):
            # the first row is not the header, we should keep it
            df = pd.read_csv(os.path.join(dir_path, file), header=None)
            df = df.iloc[:, 0]
            print(df.count())
            df_list.append(df)
    result = pd.concat(df_list)
    return result

def read_all_files(dir_path):
    file_content_list = []
    for file in os.listdir(dir_path):
        # read content of the file
        if not file.endswith(".fastq"):
            continue
        with open(os.path.join(dir_path, file), 'r') as f:
            content = f.read()
            # find all lines
            for line in content.split('\n'):
                file_content_list.append(line)
    result = pd.Series(file_content_list)
    return result

def extract_synthesized_sequence(result):
    result = result[result.str[:30].str.contains('AAAAAAAA', regex=True)]

    # remove :8 is AAAAAA
    result = result[~result.str[:8].str.contains('AAAAAAAA', regex=True)]
    
    # get :16
    result = result.str[:16]
    result = result.value_counts()
    return result

class TrieNode:
    """前缀树节点"""
    def __init__(self):
        self.children: Dict[str, TrieNode] = {}
        self.is_end_of_word: bool = False
        self.full_sequence: Optional[str] = None

class Trie:
    """前缀树结构，用于高效地模拟逐字符比较逻辑"""
    def __init__(self):
        self.root = TrieNode()

    def insert(self, sequence: str):
        """向前缀树中插入一个完整的8位序列"""
        node = self.root
        index_key = sequence[:4]
        for char in index_key:
            if char not in node.children:
                node.children[char] = TrieNode()
            node = node.children[char]
        node.is_end_of_word = True
        node.full_sequence = sequence
        
    def _get_all_sequences_from_node(self, node: TrieNode) -> List[str]:
        """递归地从一个节点获取其下所有完整的序列"""
        sequences = []
        if node.is_end_of_word and node.full_sequence:
            sequences.append(node.full_sequence)
        for child in node.children.values():
            sequences.extend(self._get_all_sequences_from_node(child))
        return sequences

    def find_best_match(self, input_sequence: str) -> Optional[str]:
        """
        模拟原始算法，逐字符在前缀树上搜索，并在失配时进行局部相似度比较
        """
        input_index = input_sequence[:8]
        node = self.root

        # 1. 逐字符精确匹配
        for i, char in enumerate(input_index):
            if char in node.children:
                node = node.children[char]
            else:
                # 2. 发生失配，触发局部相似度仲裁
                # 收集当前节点下的所有可能性
                candidate_sequences = self._get_all_sequences_from_node(node)
                
                if not candidate_sequences:
                    return None # 没有候选序列

                # 在这些少数候选中，通过difflib找到最相似的一个
                best_match_seq = ""
                highest_similarity = -1.0
                for seq in candidate_sequences:
                    similarity = difflib.SequenceMatcher(None, input_index, seq[:8]).ratio()
                    if similarity > highest_similarity:
                        highest_similarity = similarity
                        best_match_seq = seq
                return best_match_seq

        # 3. 如果8个字符完全匹配，直接返回结果
        # 如果该节点本身就是一个词的结尾，则优先返回它
        if node.is_end_of_word and node.full_sequence:
            return node.full_sequence
        
        # 如果它只是一个前缀（例如输入'CGA'，但库里只有'CGAT'），
        # 则返回其下的第一个子序列作为最可能的匹配
        all_sequences = self._get_all_sequences_from_node(node)
        return all_sequences[0] if all_sequences else None
    
    
# https://biopython.org/docs/dev/Tutorial/chapter_pairwise.html#substitution-scores
aligner = Align.PairwiseAligner()
aligner.match_score = 1.0
aligner.mismatch_score = -2.0
aligner.gap_score = -2.5
quick_ratio_lower_bound = 0.55
target_sequence = "ACTCTGAT"
record_existed = True

all_dna_strands_ = [
    "ATATATCT", "ATACATCG", "ATAGTACT", "ATCATGCG", "ATCTATGT", "ATCGATCT",
    "ATGATACA", "ATGTACAC", "ATGCATCG", "ACATGATA", "ACACGAGA", "ACAGAGAG",
    "ACTATACA", "ACTCTGAT", "ACTGAGCG", "ACGAGTGC", "ACGTAGCG", "ACGCGATC",
    "AGATCTAC", "AGACTATC", "AGAGACAG", "AGTATGAG", "AGTCATGA", "AGTGCGCG",
    "AGCACGAC", "AGCTCGAG", "AGCGATCT", "TATACATG", "TATCTCAC", "TATGATAT",
    "TACAGAGA", "TACTATAT", "TACGTATG", "TAGATCGA", "TAGTGATA", "TAGCTCAC",
    "TCATGCGC", "TCACTAGA", "TCAGATAC", "TCTACTGT", "TCTCGAGC", "TCTGTAGC",
    "TCGACTGC", "TCGTAGTG", "TCGCGTGC", "TGATCGCG", "TGACTCAT", "TGAGAGTG",
    "TGTAGCAG", "TGTCGCTC", "TGTGCGTG", "TGCATATG", "TGCTCAGA", "TGCGTCGC",
    "CATAGTAG", "CATCGTCT", "CATGCGAG", "CACAGATG", "CACTAGAT", "CACGATCA",
    "CAGATAGA", "CAGTACTC", "CAGCGCAC", "CTATAGCT", "CTACATAC", "CTAGATGT",
    "CTCAGCTA", "CTCTACAT", "CTCGTACG", "CTGACATA", "CTGTACAC", "CTGCGTGA",
    "CGATGATA", "CGACGACG", "CGAGTCTA", "CGTAGAGA", "CGTCACAC", "CGTGTAGT"
]

trie = Trie()
for strand in all_dna_strands_:
    trie.insert(strand)

def analyze_row_not_empty(row):
    # an array of 1, 2, 3, ..., with length equal to the length of the target sequence
    deletion_list = [i+1 for i in range(len(target_sequence))]
    row['deletion_list'] = deletion_list
    row['insertion_list'] = []
    row['substitution_list'] = []
    row['success_list'] = []
    row['quick_ratio'] = 0
    row['guessed_sequence'] = ''
    return row

def guess_target_sequence(origin_sequence):
    # just get most likely sequence from all_dna_strands_
    
    if len(origin_sequence) < 8:
        print("错误：输入序列的长度至少应为8。")
        return None
    return trie.find_best_match(origin_sequence)
    

def analyze_row(row):
    if row.sequence == '':
        return analyze_row_not_empty(row)
    
    guessed_seq = guess_target_sequence(row.sequence)
    if guessed_seq is None:
        return analyze_row_not_empty(row)
    
    matcher = difflib.SequenceMatcher(None, guessed_seq, row.sequence)
    quick_ratio = matcher.quick_ratio()
    opcodes = matcher.get_opcodes()
    
    deletion_list = []
    insertion_list = []
    substitution_list = []
    success_list = []

    pre = 'equal'
    for tag, i1, i2, j1, j2 in opcodes:
        # print(f"{tag}: seq1[{i1}:{i2}] -> seq2[{j1}:{j2}] | {ideal_seq_dict[key][i1:i2]} -> {tmp_seq[j1:j2]}")
        
        if tag == 'equal':
            if pre == 'insert':
                for i in range(i1+1,i2):
                    success_list.append(i)
                    # sub_error_dict['Right'][i] += 1
            else:
                for i in range(i1,i2):
                    success_list.append(i)
                    # sub_error_dict['Right'][i] += 1
        elif tag == 'delete':
            if i1 == i2:
                deletion_list.append(i1)
            else:
                for i in range(i1, i2):
                    deletion_list.append(i)
        elif tag == 'insert':
            if i1 < 8:
                if i1 == i2:
                    insertion_list.append(i1)
                else:
                    for i in range(i1,i2):
                        insertion_list.append(i)
        elif tag == 'replace':
            if i1 < 8:
                if i1 == i2:
                    substitution_list.append(i1)
                else:
                    for i in range(i1,i2):
                        substitution_list.append(i)

        pre = tag
    
    row['deletion_list'] = deletion_list
    row['insertion_list'] = insertion_list
    row['substitution_list'] = substitution_list
    row['success_list'] = success_list
    row['quick_ratio'] = quick_ratio
    row['guessed_sequence'] = guessed_seq
    # wait for command line input
    # input("Press Enter to continue...")
    return row

def main(args, target_index):    
    # create the output directory if it does not exist
    if not os.path.exists(args.output_dir):
        os.makedirs(args.output_dir)

    result_file = args.output_dir + "/" + args.intermediate + ".csv"
    result_file_1 = args.output_dir + "/" + args.intermediate + "1.csv"
    result_file_2 = args.output_dir + "/" + args.intermediate + "2_" + str(target_index) + ".csv"
    result_file_3 = args.output_dir + "/" + args.intermediate + "3_" + str(target_index) + ".csv"
    result_file_4 = args.output_dir + "/" + args.intermediate + "4_" + str(target_index) + ".csv"

    result = None
    # if result_file does not exist:
    if not os.path.exists(result_file):
        # result = read_csv_files(args.input_dir)
        result = read_all_files(args.input_dir)
        result = extract_synthesized_sequence(result)
        result.to_csv(result_file, header=False)
    result = pd.read_csv(result_file, header=None)
    result = result.fillna('')


    # if result_file_1 does not exist:
    if not os.path.exists(result_file_1) or True:
        result.columns = ["sequence", "count"]
        total_seq_count = result['count'].sum()
        result["percentage"] = result['count'] * 100 / total_seq_count
        
        result = result.apply(analyze_row, axis=1)
        result.to_csv(result_file_1, index=False)   
    result = pd.read_csv(result_file_1, index_col=False)
    
    # get only guess_target_sequence is target_sequence
    result = result[result['guessed_sequence'] == target_sequence]
    
    if not os.path.exists(result_file_2) or True:
        result = result[result['quick_ratio'] > quick_ratio_lower_bound]

        result.to_csv(result_file_2, index=False)
    result = pd.read_csv(result_file_2, index_col=False)

    total_seq_count = result['count'].sum()
    total_base_count = total_seq_count * len(target_sequence)
    
    if total_seq_count == 0 or total_base_count == 0:
        print("No valid sequences found for target sequence index: {0}, target sequence: {1}".format(target_index, target_sequence))
        return
    
    success_seq_count = result['count'].where(
        (result['deletion_list'].apply(eval).apply(len) == 0) 
        & (result['insertion_list'].apply(eval).apply(len) == 0) 
        & (result['substitution_list'].apply(eval).apply(len) == 0), 
        0).sum()
    deletion_seq_count = (result['count'] * (result['deletion_list'].apply(eval).apply(len) > 0)).sum()
    insertion_seq_count = (result['count'] * (result['insertion_list'].apply(eval).apply(len) > 0)).sum()
    substitution_seq_count = (result['count'] * (result['substitution_list'].apply(eval).apply(len) > 0)).sum()
    deletion_base_count = (result['count'] * result['deletion_list'].apply(eval).apply(len)).sum()
    insertion_base_count = (result['count'] * result['insertion_list'].apply(eval).apply(len)).sum()
    substitution_base_count = (result['count'] * result['substitution_list'].apply(eval).apply(len)).sum()
    success_base_count = (result['count'] * result['success_list'].apply(eval).apply(len)).sum()
    # print(deletion_base_count, insertion_base_count, substitution_base_count)
    # print(success_base_count)
    # print(deletion_base_count + insertion_base_count + substitution_base_count + success_base_count)
    # print(total_base_count)

    # print in red color
    print("Target sequence: \033[91m{0}\033[0m".format(target_sequence))
    print("Total number of sequences read: \033[91m{0}\033[0m".format(total_seq_count))
    print(" - Success ratio:\t\033[91m{0}\t/ {1} = {2:.2f}%\033[0m\t(ratio of sequences successfully synthesized)".format(success_seq_count, total_seq_count, success_seq_count * 100 / total_seq_count))
    print(" - Deletion ratio:\t\033[91m{0}\t/ {1} = {2:.2f}%\033[0m\t(ratio of sequences containing deletions)".format(deletion_seq_count, total_seq_count, deletion_seq_count * 100 / total_seq_count))
    print(" - Insertion ratio:\t\033[91m{0}\t/ {1} = {2:.2f}%\033[0m\t(ratio of sequences containing insertions)".format(insertion_seq_count, total_seq_count, insertion_seq_count * 100 / total_seq_count))
    print(" - Substitution ratio:\t\033[91m{0}\t/ {1} = {2:.2f}%\033[0m\t(ratio of sequences containing substitutions)".format(substitution_seq_count, total_seq_count, substitution_seq_count * 100 / total_seq_count))
    print(" - Average yield:\t\033[91m{0:.3f}%\033[0m\t(average yield per base, calculated as (success bases / total bases)^(1/8))".format((success_seq_count / total_seq_count) ** (1 / len(target_sequence)) * 100))
    
    
    print("Total number of bases synthesized: \033[91m{0} * {1} = {2}\033[0m".format(success_seq_count, len(target_sequence), success_base_count))
    print(" - Success ratio:\t\033[91m{0}\t/ {1} = {2:.2f}%\033[0m\t(ratio of bases successfully synthesized)".format(success_base_count, total_base_count, success_base_count * 100 / total_base_count))
    print(" - Deletion ratio:\t\033[91m{0}\t/ {1} = {2:.2f}%\033[0m\t(ratio of deletion errors)".format(deletion_base_count, total_base_count, deletion_base_count * 100 / total_base_count))
    print(" - Insertion ratio:\t\033[91m{0}\t/ {1} = {2:.2f}%\033[0m\t(ratio of insertion errors)".format(insertion_base_count, total_base_count, insertion_base_count * 100 / total_base_count))
    print(" - Substitution ratio:\t\033[91m{0}\t/ {1} = {2:.2f}%\033[0m\t(ratio of substitution errors)".format(substitution_base_count, total_base_count, substitution_base_count * 100 / total_base_count)) 
    
    average_result = pd.DataFrame(columns=['success_ratio', 'deletion_ratio', 'insertion_ratio', 'substitution_ratio', 'average_stepwise_yield'])
    average_result.loc[0] = [
        success_base_count / total_base_count,
        deletion_base_count / total_base_count,
        insertion_base_count / total_base_count,
        substitution_base_count / total_base_count,
        (success_seq_count / total_seq_count) ** (1 / len(target_sequence))
    ]
    average_result.to_csv(result_file_4, index=False)
    
    # new dataframe for stepwise analysis
    stepwise_result = pd.DataFrame(columns=['step', 'deletion_count', 'insertion_count', 'substitution_count', 'success_count'])
    for i in range(len(target_sequence)):
        # index = i + 1
        index = i
        stepwise_result.loc[i] = [str(index), 
            result['count'][result['deletion_list'].apply(eval).apply(lambda x: index in x)].sum(), 
            result['count'][result['insertion_list'].apply(eval).apply(lambda x: index in x)].sum(), 
            result['count'][result['substitution_list'].apply(eval).apply(lambda x: index in x)].sum(),
            result['count'][result['success_list'].apply(eval).apply(lambda x: index in x)].sum()]
    stepwise_result['deletion_ratio'] = stepwise_result['deletion_count'] / total_seq_count
    stepwise_result['insertion_ratio'] = stepwise_result['insertion_count'] / total_seq_count
    stepwise_result['substitution_ratio'] = stepwise_result['substitution_count'] / total_seq_count
    stepwise_result['success_ratio'] = stepwise_result['success_count'] / total_seq_count
        
    # add last row for aggregation
    stepwise_result.loc[str(len(target_sequence ))] = stepwise_result.sum()
    # stepwise_result.loc[str(len(target_sequence))]['step'] = 'Total'
    
    stepwise_result.to_csv(result_file_3, index=False)


if __name__ == "__main__":
    # input the directory of the csv files with command line arguments "-d"
    parser = argparse.ArgumentParser()
    parser.add_argument("-d", "--input_dir", type=str, required=True)
    parser.add_argument("-o", "--output_dir", type=str, required=False, default="./")
    parser.add_argument("-i", "--intermediate", type=str, required=False, default="result")
    args = parser.parse_args()
    
    i = 16
    print("Analyzing target sequence index: {0}".format(i))
    target_sequence = all_dna_strands_[i]
    main(args, i)
        