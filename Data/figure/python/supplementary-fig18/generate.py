from collections import Counter
import matplotlib.pyplot as plt
import os

from Bio import SeqIO
from math import log

def errs_tab(n):
    """Generate list of error rates for qualities less than equal than n."""
    return [10**(q / -10) for q in range(n+1)]

def ave_qual(quals, qround=False, tab=errs_tab(128)):
    """Calculate average basecall quality of a read.

    Receive the integer quality scores of a read and return the average quality for that read
    First convert Phred scores to probabilities,
    calculate average error probability
    convert average back to Phred scale
    """
    if quals:
        mq = -10 * log(sum([tab[q] for q in quals]) / len(quals), 10)
        if qround:
            return round(mq)
        else:
            return mq
    else:
        return None

def phred_score(char):
    """将质量字符转换为 Q 值（Phred+33 编码）"""
    return ord(char) - 33

def analyze_fastq_quality(fastq_dir):
    q_counter = Counter()
    if os.path.exists("q_distribution.csv"):
        print("Q distribution file already exists. Skipping analysis.")
        # read existing file
        with open("q_distribution.csv", "r") as f:
            next(f)  # skip header
            for line in f:
                q_value, count = map(int, line.strip().split(","))
                q_counter[q_value] += count
        return q_counter
    
    for fastq_file in os.listdir(fastq_dir):
        if not fastq_file.endswith(".fastq") and not fastq_file.endswith(".fq"):
            continue
        print(f"Processing {fastq_file}...")
        with open(os.path.join(fastq_dir, fastq_file), 'r') as f:
            for rec in SeqIO.parse(f, "fastq"):
                q_counter[ave_qual(rec.letter_annotations["phred_quality"], qround=True)] += len(rec)
                # for q in rec.letter_annotations["phred_quality"]:
                #     q_counter[q] += 1
                
                # yield ut.ave_qual(rec.letter_annotations["phred_quality"]), len(rec)
            # line_num = 0
            # for line in f:
            #     line_num += 1
            #     if line_num % 4 == 0:  # 每四行的第四行是质量值
            #         # sum_q = 0
            #         # for char in line.strip():
            #         #     q = phred_score(char)
            #         #     sum_q += q
            #         # avg_q = sum_q / len(line.strip())
            #         # q_counter[avg_q] += len(line.strip())
            #         for char in line.strip():
            #             q = phred_score(char)
            #             q_counter[q] += 1

    # save to csv
    with open("q_distribution.csv", "w") as f:
        f.write("Q_value,Count\n")
        for q_value, count in sorted(q_counter.items()):
            f.write(f"{q_value},{count}\n")
    return q_counter

def plot_q_distribution(q_distribution):
    q_values = sorted(q_distribution.keys())
    counts = [q_distribution[q] for q in q_values]
    
    bar_colors = ['red' if c < 9 else 'skyblue' for c in q_values]
    
    not_successful = sum(counts[q] for q in q_values if q < 9)
    total = sum(counts)
    success_rate = (total - not_successful) / total * 100 if total > 0 else 0
    print(f"Total bases: {total}, Not successful (Q<=9): {not_successful}, Success rate: {success_rate:.2f}%")

    plt.figure(figsize=(10, 6))

    # 使用 bar_colors 列表来设置每个柱子的颜色
    plt.bar(q_values, counts, color=bar_colors, align='edge')

    plt.xlabel('Q')
    plt.ylabel('Bases')
    plt.title('Distribution of Q Scores')
    plt.grid(axis='y', linestyle='--', alpha=0.7)
    plt.tight_layout()
    plt.savefig('q_distribution.png')
    plt.savefig('q_distribution.svg')
    plt.show()
    
def analyze_read_length(fastq_dir):
    length_counter = Counter()
    if os.path.exists("read_length_distribution.csv"):
        print("Read length distribution file already exists. Skipping analysis.")
        # read existing file
        with open("read_length_distribution.csv", "r") as f:
            next(f)  # skip header
            for line in f:
                length, count = map(int, line.strip().split(","))
                length_counter[length] += count
        return length_counter
    
    
    for fastq_file in os.listdir(fastq_dir):
        if not fastq_file.endswith(".fastq") and not fastq_file.endswith(".fq"):
            continue
        print(f"Processing {fastq_file} for read lengths...")
        with open(os.path.join(fastq_dir, fastq_file), 'r') as f:
            line_num = 0
            for line in f:
                line_num += 1
                if line_num % 4 == 2:  # 每四行的第二行是序列
                    read_length = len(line.strip())
                    length_counter[read_length] += len(line.strip())

    # longest 1% is outlier
    total_reads = sum(length_counter.values())
    cumulative = 0
    for length in sorted(length_counter.keys(), reverse=True):
        cumulative += length_counter[length]
        if cumulative / total_reads >= 0.01:
            break
        del length_counter[length]
                    
    # save to csv
    with open("read_length_distribution.csv", "w") as f:
        f.write("Read_Length,Count\n")
        for length, count in sorted(length_counter.items()):
            f.write(f"{length},{count}\n")
    return length_counter

def plot_read_length_distribution(length_distribution):
    lengths = sorted(length_distribution.keys())
    counts = [length_distribution[l] for l in lengths]

    plt.figure(figsize=(10, 6))
    plt.bar(lengths, counts, color='lightgreen')
    plt.xlabel('Read Length')
    plt.ylabel('Nums')
    plt.title('Distribution of Read Lengths')
    plt.grid(axis='y', linestyle='--', alpha=0.7)
    plt.tight_layout()
    plt.savefig('read_length_distribution.png')
    plt.savefig('read_length_distribution.svg')
    plt.show()
    
def analyze_qc_from_plot_file(plot_file):
    q_counter = Counter()
    with open(plot_file, "r") as f:
        next(f)  # skip header
        for line in f:
            parts = line.strip().split("\t")
            if len(parts) < 2:
                continue
            q_value = int(float(parts[0]))
            count = int(parts[1])
            q_counter[q_value] += count
    return q_counter

def main():
    # fastq_dir = "/home/ethereal/vm4/data/DNA/g2/all_new"
    # fastq_dir = "/home/ethereal/vm4/data/DNA/g3/auto"
    # fastq_dir = "/home/ethereal/vm4/data/DNA/g3/0415"
    fastq_dir = "./first200.fastq"
    # plot_file = "/home/ethereal/vm2/RAG/paperQA/code/sequence_analysis/q_score/data/auto.tsv"
    # q_distribution = analyze_qc_from_plot_file(plot_file)
    q_distribution = analyze_fastq_quality(fastq_dir)
    plot_q_distribution(q_distribution)
    length_distribution = analyze_read_length(fastq_dir)
    plot_read_length_distribution(length_distribution)

if __name__ == "__main__":
    main()