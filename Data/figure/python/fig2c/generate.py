import matplotlib.pyplot as plt
import numpy as np
import pandas as pd
from scipy import stats
import os
from matplotlib.ticker import ScalarFormatter
import matplotlib.patches as mpatches
import csv

data_path = './nucleic-acid-test_6with2_2.csv'
data_auto_path = './nucleic-acid-test_6with2_auto_2.csv'

# output_path = '../img/nucleic-acid-test.png'
# output_path = '../img/nucleic-acid-test.svg'
output_path = './nucleic-acid-test_V2.svg'
output_split_path = './nucleic-acid-test-split_V2.svg'
output_path_substracted = './nucleic-acid-test-substracted_V2.svg'
p_value_path = './nucleic-acid-test_p_values.csv'
categories = []

error_mode = '3' # 'p' or '3'

# Data Index,Water,Plasmid,2.5*10^1,2.5*10^2,2.5*10^3,2.5*10^4,2.5*10^5,2.5*10^6
# 1-1,1187,706,581,685,735,675,799,699
# 1-2,1044,696,586,737,1072,3778,7629,10861
def extract_data(file_path):
    data = pd.read_csv(file_path, header=None)
    return data

def group_data(data):
    original_data = {}
    # first column is like 1-1, the first before '-' is group ID, the rest is cycle ID
    max_cycle = 0
    max_group = 0
    global categories
    categories = []
    for row in data.iterrows():
        if row[1][0].find('-') == -1:
            for i in range(1, len(row[1])):
                categories.append(row[1][i])
            continue
        group_id = int(row[1][0].split('-')[0])
        max_group = max(max_group, group_id)
        cycle_id = int(row[1][0].split('-')[1])
        max_cycle = max(max_cycle, cycle_id)
        # convert to columns
        fluorescence = row[1][1:].astype(float)  # Average fluorescence across all columns for this cycle
        original_data[group_id] = original_data.get(group_id, {})
        original_data[group_id][cycle_id] = fluorescence
    return original_data, max_group, max_cycle    
    
def analyze_data(data):
    original_data, max_group, max_cycle = group_data(data)
        
    average_fluorescence = []
    stdevalues = []
    p_values = []
    for cycle in range(1, max_cycle + 1):
        cycle_fluorescence = []
        for group in range(1, max_group + 1):
            if group in original_data and cycle in original_data[group]:
                cycle_fluorescence.append(original_data[group][cycle])
        if cycle_fluorescence:
            # all samples get average fluorescence and standard deviation
            average_fluorescence.append(np.mean(cycle_fluorescence, axis=0).tolist())
            # 2 digits after decimal point
            average_fluorescence[-1] = [round(x, 2) for x in average_fluorescence[-1]]
            # standard deviation
            stdevalues.append(np.std(cycle_fluorescence, axis=0).tolist())
            # 2 digits after decimal point
            stdevalues[-1] = [round(x, 2) for x in stdevalues[-1]]
            # p-values
            p_values.append([])
            for i in range(len(cycle_fluorescence[0])):
                # perform t-test for each cycle fluorescence value
                t_stat, p_value = stats.ttest_ind([x.tolist()[0] for x in cycle_fluorescence], [x.tolist()[i] for x in cycle_fluorescence], equal_var=False)
                p_values[-1].append(round(p_value, 4))
        else:
            average_fluorescence.append([0] * len(cycle_fluorescence[0]))
            stdevalues.append([0] * len(cycle_fluorescence[0]))
    return average_fluorescence, stdevalues, p_values

def prepare_data(substract_ntc=False, data_path=data_path):
    data = extract_data(data_path)
    average_fluorescence, stdevalues, p_values = analyze_data(data)

    # substract from NTC
    if substract_ntc:
        # NTC is the first column, so we substract the first column from all other columns
        for i in range(len(average_fluorescence)):
            average_fluorescence[i] = [max(0, x - average_fluorescence[i][0]) for j, x in enumerate(average_fluorescence[i])]
            stdevalues[i] = [0 if average_fluorescence[i][j] == 0 else x for j, x in enumerate(stdevalues[i])]

    return average_fluorescence[len(average_fluorescence) - 1], stdevalues[len(stdevalues) - 1], p_values[len(p_values) - 1]

def categories_to_cp(categories):
    """Convert categories to cp values."""
    cp_mapping = {
        'Water': '0',
        'Plasmid': '0',
        '2.5': '2.5',
        '2.5*10^1': '2.5',
        '2.5*10^2': '25',
        '2.5*10^3': '250',
        '2.5*10^4': '2500',
        '2.5*10^5': '25000',
        '2.5*10^6': '250000',
    }
    return [cp_mapping.get(cat, cat) for cat in categories]

def categories_to_label(categories):
    """Convert categories to label values."""
    cp_mapping = {
        'Water': 'NTC',
        'Plasmid': 'Negative',
        '2.5': '2.5',
        '2.5*10^1': '2.5',
        '2.5*10^2': '25',
        '2.5*10^3': '250',
        '2.5*10^4': '2500',
        '2.5*10^5': '25000',
        '2.5*10^6': '250000',
    }
    return [cp_mapping.get(cat, cat) for cat in categories]

def generate_plot(average_fluorescence, stdevalues, p_values, autoDNA_average_fluorescence, autoDNA_stdevalues, autoDNA_p_values, substract_ntc=False, error_mode='3'):
    x_positions = np.arange(len(categories))

    # 定义每个柱子的高度 (荧光值)，根据原图估算
    fluorescence_values = average_fluorescence

    # 定义误差线的值 (例如标准差)，根据原图估算
    errors = stdevalues

    # 定义X轴下方的附加标签
    zikv_labels = categories_to_cp(categories)
    origin_labels = average_fluorescence

    # --- 2. 创建图表 (Create the Plot) ---
    # 创建一个画布(figure)和一个坐标系(axes)，设置图表大小
    fig, ax = plt.subplots(figsize=(7, 6))
    
    # 设置字体
    plt.rcParams['font.sans-serif'] = ['Arial Unicode MS']  # 设置无衬线字体
    plt.rcParams['font.family'] = 'sans-serif'

    # 绘制条形图，并添加误差线
    first_bar_position = x_positions - 0.2  # 调整第一个柱子的位置
    second_bar_position = x_positions + 0.2  # 调整第二个柱
    # 误差只包含上面
    
    def draw_error_bars(ax, x_positions, fluorescence_values, errors):
        """绘制误差线"""
        for i in range(len(x_positions)):
            # 误差线的垂直范围
            y_start = fluorescence_values[i] - errors[i]
            y_end = fluorescence_values[i] + errors[i]
            # 误差线的水平位置
            x = x_positions[i]

            # 绘制垂直线 (误差线主体)
            ax.vlines(x, y_start, y_end, color='black', linewidth=0.5, zorder=10)

            # 绘制水平线 (顶帽)
            cap_width = 0.15 # 调整顶帽的宽度
            ax.hlines(y_end, x - cap_width, x + cap_width, color='black', linewidth=0.5, zorder=10)
            ax.hlines(y_start, x - cap_width, x + cap_width, color='black', linewidth=0.5, zorder=10)

    
    bars = ax.bar(first_bar_position, fluorescence_values,
        color='orange',  # 第一组柱子颜色
        alpha=0.7,          # 设置透明度
        width=0.4,          # 设置柱子的宽度
        capsize=5,
        # 设置误差线下横线为0
        label='Human Average Fluorescence with Error')  # 添加图例标签
    draw_error_bars(ax, first_bar_position, fluorescence_values, errors)
    
    bars2 = ax.bar(second_bar_position, autoDNA_average_fluorescence,
        color='steelblue',  # 第二组柱子颜色
         alpha=0.7,          # 设置透明度
        width=0.4,          # 设置柱子的宽度
        capsize=5,
        label='AutoDNA Average Fluorescence with Error')  # 添加图例标签
    draw_error_bars(ax, second_bar_position, autoDNA_average_fluorescence, autoDNA_stdevalues)

    # --- 3. 自定义坐标轴和标签 (Customize Axes and Labels) ---
    # 设置标题和Y轴标签
    # ax.set_title('Sample', fontsize=16, pad=15)
    if substract_ntc:
        ax.set_ylabel('Background substracted\nFluorescence', fontsize=14)
    else:
        ax.set_ylabel('Fluorescence', fontsize=14)

    # 设置Y轴的范围和刻度格式
    if substract_ntc:
        ax.set_ylim(0, 30000)  # 设置Y轴范围为0到30000
    else:
        ax.set_ylim(0, 35000)  # 设置Y轴范围为0到35000
    
    # 将Y轴刻度格式化为科学记数法，例如 20000 -> 2 x 10^4
    ax.ticklabel_format(style='sci', axis='y', scilimits=(4,4), useMathText=True)
    ax.yaxis.get_offset_text().set_fontsize(12) # 调整 10^4 字体大小

    # 设置X轴刻度和标签
    ax.set_xticks(x_positions)
    ax.set_xticklabels([i+1 for i in range(len(categories))], rotation=0, ha='center', fontsize=12)
    # ax.set_xlabel('sample', fontsize=14, labelpad=10)

    # 移除顶部和右侧的边框线，使图表更简洁
    ax.spines['top'].set_visible(False)
    ax.spines['right'].set_visible(False)

    # 在X轴下方添加多行文本标签
    # 首先调整图表底部边缘，为文本留出空间
    plt.subplots_adjust(bottom=0.2)

    # 添加行标题
    ax.text(-0.07, -0.05, 'Sample', transform=ax.transAxes, fontsize=12, ha='left')
    ax.text(-0.07, -0.12, 'cp/µL', transform=ax.transAxes, fontsize=12, ha='left', va='center')
    # ax.text(-0.2, -0.22, 'Sample origin', transform=ax.transAxes, fontsize=12, ha='left')

    # 循环添加每列的文本
    y_offset1 = -0.12 # 第一行文本的Y坐标 (相对于坐标系)
    y_offset2 = -0.22 # 第二行文本的Y坐标
    for i, (zikv, origin) in enumerate(zip(zikv_labels, origin_labels)):
        # 使用 `ax.get_xaxis().get_major_ticks()[i].get_loc()` 获取精确的X位置
        center_x = x_positions[i]
        # 修正X位置，使其与柱子对齐
        ax.text(center_x, y_offset1, zikv, transform=ax.get_xaxis_transform(),
                ha='center', va='center', fontsize=10)
        # ax.text(center_x, y_offset2, origin, transform=ax.get_xaxis_transform(),
        #         ha='center', va='top', fontsize=8)

    # 添加统计显著性标记 (****)
    def draw_significance_bar(x1, x2, y, text, h=1000):
        """一个用于绘制显著性标记的辅助函数"""
        # 绘制横线和两端的垂直短线
        ax.plot([x1, x1, x2, x2], [y, y + h, y + h, y], lw=0.5, c='k')
        # 在横线上方添加文本 (例如 ****)
        ax.text((x1 + x2) * 0.5, y + h, text, ha='center', va='bottom', fontsize=14)

    # 根据原图，调用函数绘制四个显著性标记
    # 注意：y坐标是手动选择的，以避免重叠并保持美观
    # draw_significance_bar(x_positions[2], x_positions[3], 3.0e4, '****')
    # draw_significance_bar(x_positions[1], x_positions[3], 5.5e4, '****')
    # draw_significance_bar(x_positions[0], x_positions[3], 9.5e4, '****')
    # draw_significance_bar(x_positions[0], x_positions[1], 9.2e4, '****')
    upper = 1000
    for i in range(len(x_positions)):
        if i < 1:
            continue
        text = 'p=' + str(p_values[i]) + '\n'
        if p_values[i] < 0.0001:
            text += '****'
        elif p_values[i] < 0.001:
            text += '***'
        elif p_values[i] < 0.01:
            text += '**'
        elif p_values[i] < 0.05:
            text += '*'
        draw_significance_bar(x_positions[0], x_positions[i], fluorescence_values[i] + upper, text)
    
    if error_mode == '3':
        err_line_value = 3 * x_positions[0]
        ax.axhline(y=err_line_value, color='red', linestyle='--', linewidth=0.5)

    # --- 5. 调整并显示图表 (Final Adjustments and Display) ---
    # 自动调整布局，防止标签重叠
    plt.tight_layout(rect=[0, 0.05, 1, 1]) # 调整布局区域，为底部文本留出更多空间

    # create directory if it does not exist
    os.makedirs(os.path.dirname(output_path), exist_ok=True)
    if substract_ntc:
        plt.savefig(output_path_substracted, dpi=300, bbox_inches='tight')
    else:
        plt.savefig(output_path, dpi=300, bbox_inches='tight')  

def generate_plot_4(average_fluorescences, stdevalueses, p_valueses, substract_ntc=False, error_mode='3'):
    x_positions = np.arange(len(categories))

    # 定义每个柱子的高度 (荧光值)，根据原图估算
    fluorescence_values = average_fluorescences

    # 定义误差线的值 (例如标准差)，根据原图估算
    errors = stdevalueses

    # 定义X轴下方的附加标签
    zikv_labels = categories_to_cp(categories)

    # --- 2. 创建图表 (Create the Plot) ---
    # 创建一个画布(figure)和一个坐标系(axes)，设置图表大小
    fig, ax = plt.subplots(figsize=(7, 6))
    
    # 设置字体
    plt.rcParams['font.sans-serif'] = ['Arial Unicode MS']  # 设置无衬线字体
    plt.rcParams['font.family'] = 'sans-serif'

    # 绘制条形图，并添加误差线
    first_bar_position = x_positions - 0.3  # 调整第一个柱子的位置
    second_bar_position = x_positions - 0.1  # 调整第二个柱
    third_bar_position = x_positions + 0.1  # 调整第三个柱子的位置
    fourth_bar_position = x_positions + 0.3  # 调整第四个柱子的位置
    # 误差只包含上面
    
    def draw_error_bars(ax, x_positions, fluorescence_values, errors):
        """绘制误差线"""
        for i in range(len(x_positions)):
            # 误差线的垂直范围
            y_start = fluorescence_values[i]
            y_end = fluorescence_values[i] + errors[i]
            # 误差线的水平位置
            x = x_positions[i]

            # 绘制垂直线 (误差线主体)
            ax.vlines(x, y_start, y_end, color='black', linewidth=0.5, zorder=10)

            # 绘制水平线 (顶帽)
            cap_width = 0.15 # 调整顶帽的宽度
            ax.hlines(y_end, x - cap_width, x + cap_width, color='black', linewidth=0.5, zorder=10)

    
    bars = ax.bar(first_bar_position, fluorescence_values[0],
        color='orange',  # 第一组柱子颜色
        alpha=0.7,          # 设置透明度
        width=0.2,          # 设置柱子的宽度
        capsize=5,
        # 设置误差线下横线为0
        label='7.23 Human')  # 添加图例标签
    draw_error_bars(ax, first_bar_position, fluorescence_values[0], errors[0])
    
    bars2 = ax.bar(second_bar_position, fluorescence_values[1],
        color='steelblue',  # 第二组柱子颜色
         alpha=0.7,          # 设置透明度
        width=0.2,          # 设置柱子的宽度
        capsize=5,
        label='7.28 AutoDNA')  # 添加图例标签
    draw_error_bars(ax, second_bar_position, fluorescence_values[1], errors[1])
    
    bars3 = ax.bar(third_bar_position, fluorescence_values[2],
        color='green',  # 第三组柱子颜色
         alpha=0.7,          # 设置透明度
        width=0.2,          # 设置柱子的宽度
        capsize=5,
        label='8.4 Human')  # 添加图例标签
    draw_error_bars(ax, third_bar_position, fluorescence_values[2], errors[2])
    
    bars4 = ax.bar(fourth_bar_position, fluorescence_values[3],
        color='purple',  # 第四组柱子颜色
         alpha=0.7,          # 设置透明度
        width=0.2,          # 设置柱子的宽度
        capsize=5,
        label='8.4 AutoDNA')  # 添加图例标签
    draw_error_bars(ax, fourth_bar_position, fluorescence_values[3], errors[3])

    # --- 3. 自定义坐标轴和标签 (Customize Axes and Labels) ---
    # 设置标题和Y轴标签
    # ax.set_title('Sample', fontsize=16, pad=15)
    if substract_ntc:
        ax.set_ylabel('Background substracted\nFluorescence', fontsize=14)
    else:
        ax.set_ylabel('Fluorescence', fontsize=14)

    # 设置Y轴的范围和刻度格式
    if substract_ntc:
        ax.set_ylim(0, 30000)  # 设置Y轴范围为0到30000
    else:
        ax.set_ylim(0, 35000)  # 设置Y轴范围为0到35000
    
    # 将Y轴刻度格式化为科学记数法，例如 20000 -> 2 x 10^4
    ax.ticklabel_format(style='sci', axis='y', scilimits=(4,4), useMathText=True)
    ax.yaxis.get_offset_text().set_fontsize(12) # 调整 10^4 字体大小

    # 设置X轴刻度和标签
    ax.set_xticks(x_positions)
    ax.set_xticklabels([i+1 for i in range(len(categories))], rotation=0, ha='center', fontsize=12)
    # ax.set_xlabel('sample', fontsize=14, labelpad=10)

    # 移除顶部和右侧的边框线，使图表更简洁
    ax.spines['top'].set_visible(False)
    ax.spines['right'].set_visible(False)

    # 在X轴下方添加多行文本标签
    # 首先调整图表底部边缘，为文本留出空间
    plt.subplots_adjust(bottom=0.2)

    # 添加行标题
    ax.text(-0.07, -0.05, 'Sample', transform=ax.transAxes, fontsize=12, ha='left')
    ax.text(-0.07, -0.12, 'cp/µL', transform=ax.transAxes, fontsize=12, ha='left', va='center')
    # ax.text(-0.2, -0.22, 'Sample origin', transform=ax.transAxes, fontsize=12, ha='left')

    # 循环添加每列的文本
    y_offset1 = -0.12 # 第一行文本的Y坐标 (相对于坐标系)
    y_offset2 = -0.22 # 第二行文本的Y坐标
    for i, (zikv) in enumerate(zikv_labels):
        # 使用 `ax.get_xaxis().get_major_ticks()[i].get_loc()` 获取精确的X位置
        center_x = x_positions[i]
        # 修正X位置，使其与柱子对齐
        ax.text(center_x, y_offset1, zikv, transform=ax.get_xaxis_transform(),
                ha='center', va='center', fontsize=10)
        # ax.text(center_x, y_offset2, origin, transform=ax.get_xaxis_transform(),
        #         ha='center', va='top', fontsize=8)

    # 添加统计显著性标记 (****)
    def draw_significance_bar(x1, x2, y, text, h=1000):
        """一个用于绘制显著性标记的辅助函数"""
        # 绘制横线和两端的垂直短线
        ax.plot([x1, x1, x2, x2], [y, y + h, y + h, y], lw=0.5, c='k')
        # 在横线上方添加文本 (例如 ****)
        ax.text((x1 + x2) * 0.5, y + h, text, ha='center', va='bottom', fontsize=14)

    # 根据原图，调用函数绘制四个显著性标记
    # 注意：y坐标是手动选择的，以避免重叠并保持美观
    # draw_significance_bar(x_positions[2], x_positions[3], 3.0e4, '****')
    # draw_significance_bar(x_positions[1], x_positions[3], 5.5e4, '****')
    # draw_significance_bar(x_positions[0], x_positions[3], 9.5e4, '****')
    # draw_significance_bar(x_positions[0], x_positions[1], 9.2e4, '****')
    upper = 1000
    p_values = p_valueses[0]
    for i in range(len(x_positions)):
        if i < 1:
            continue
        text = 'p=' + str(p_values[i]) + '\n'
        if p_values[i] < 0.0001:
            text += '****'
        elif p_values[i] < 0.001:
            text += '***'
        elif p_values[i] < 0.01:
            text += '**'
        elif p_values[i] < 0.05:
            text += '*'
        # draw_significance_bar(x_positions[0], x_positions[i], fluorescence_values[0][i] + upper, text)
    
    if error_mode == '3':
        err_line_value = 3 * x_positions[0]
        ax.axhline(y=err_line_value, color='red', linestyle='--', linewidth=0.5)

    # --- 5. 调整并显示图表 (Final Adjustments and Display) ---
    # 自动调整布局，防止标签重叠
    plt.tight_layout(rect=[0, 0.05, 1, 1]) # 调整布局区域，为底部文本留出更多空间
    
    # 绘制图例
    ax.legend(loc='upper left', fontsize=12, frameon=False)

    # create directory if it does not exist
    os.makedirs(os.path.dirname(output_path), exist_ok=True)
    if substract_ntc:
        plt.savefig(output_path_substracted, dpi=300, bbox_inches='tight')
    else:
        plt.savefig(output_path, dpi=300, bbox_inches='tight')  


def generate_plot_split(average_fluorescence, stdevalues, p_values, autoDNA_average_fluorescence, autoDNA_stdevalues, autoDNA_p_values, substract_ntc=False, error_mode='3'):
    x_positions = np.arange(len(categories))

    # 定义每个柱子的高度 (荧光值)，根据原图估算
    fluorescence_values = average_fluorescence

    # 定义误差线的值 (例如标准差)，根据原图估算
    errors = stdevalues

    # 定义X轴下方的附加标签
    zikv_labels = categories_to_cp(categories)
    origin_labels = average_fluorescence

    # --- 2. 创建图表 (Create the Plot) ---
    # 创建一个画布(figure)和一个坐标系(axes)，设置图表大小
    fig, (ax1, ax2) = plt.subplots(1, 2, figsize=(15, 7))
    
    # 设置字体
    plt.rcParams['font.sans-serif'] = ['Arial Unicode MS']  # 设置无衬线字体
    plt.rcParams['font.family'] = 'sans-serif'
    
    def draw_error_bars(ax, x_positions, fluorescence_values, errors):
        """绘制误差线"""
        for i in range(len(x_positions)):
            # 误差线的垂直范围
            y_start = fluorescence_values[i]
            y_end = fluorescence_values[i] + errors[i]
            # 误差线的水平位置
            x = x_positions[i]

            # 绘制垂直线 (误差线主体)
            ax.vlines(x, y_start, y_end, color='black', linewidth=0.5, zorder=10)

            # 绘制水平线 (顶帽)
            cap_width = 0.15 # 调整顶帽的宽度
            ax.hlines(y_end, x - cap_width, x + cap_width, color='black', linewidth=0.5, zorder=10)

    def draw_significance_bar(ax, x1, x2, y, text, h=1000):
        """在指定的坐标系(ax)上绘制显著性标记"""
        ax.plot([x1, x1, x2, x2], [y, y + h, y + h, y], lw=0.5, c='k')
        ax.text((x1 + x2) * 0.5, y + h, text, ha='center', va='bottom', fontsize=14)


    def setup_axes(ax, title):
        """统一设置每个子图的坐标轴和标签"""
        ax.set_title(title, fontsize=16, pad=15)
        
        if title == 'AutoDNA':
            ax.set_ylim(0, 25000)
        else:
            if substract_ntc:
                ax.set_ylim(0, 30000)
            else:
                ax.set_ylim(0, 35000)

        ax.ticklabel_format(style='sci', axis='y', scilimits=(4,4), useMathText=True)
        ax.yaxis.get_offset_text().set_fontsize(12)

        ax.set_xticks(x_positions)
        ax.set_xticklabels([i+1 for i in range(len(categories))], rotation=0, ha='center', fontsize=12)
        ax.spines['top'].set_visible(False)
        ax.spines['right'].set_visible(False)
        
        # 添加X轴下方的文本标签
        ax.text(-0.07, -0.05, 'Sample', transform=ax.transAxes, fontsize=12, ha='left')
        ax.text(-0.07, -0.12, 'cp/µL', transform=ax.transAxes, fontsize=12, ha='left', va='center')

        zikv_labels = categories_to_cp(categories)
        y_offset1 = -0.12
        for i, zikv in enumerate(zikv_labels):
            center_x = x_positions[i]
            ax.text(center_x, y_offset1, zikv, transform=ax.get_xaxis_transform(),
                    ha='center', va='center', fontsize=10)
            
    def setup_significance(ax, x_positions, fluorescence_values, p_values):
        """在指定的坐标系(ax)上添加显著性标记"""
            
        upper = 1000
        for i in range(len(x_positions)):
            if i < 3:
                continue
            text = 'p=' + str(p_values[i]) + '    '
            if p_values[i] < 0.0001:
                text += '****'
            elif p_values[i] < 0.001:
                text += '***'
            elif p_values[i] < 0.01:
                text += '**'
            elif p_values[i] < 0.05:
                text += '*'
            draw_significance_bar(ax, x_positions[0], x_positions[i], fluorescence_values[i] + upper, text)
        
        if error_mode == '3':
            err_line_value = 3 * fluorescence_values[0]
            ax.axhline(y=err_line_value, color='red', linestyle='--', linewidth=0.5)
        
    ax1.bar(x_positions, average_fluorescence,
            color='orange', alpha=0.7, width=0.8,
            capsize=5, edgecolor='black', linewidth=0.5,
            label='Human Average Fluorescence')
    draw_error_bars(ax1, x_positions, average_fluorescence, stdevalues)
    setup_axes(ax1, 'Human')
    
    ax2.bar(x_positions, autoDNA_average_fluorescence,
            color='steelblue', alpha=0.7, width=0.8,
            capsize=5, edgecolor='black', linewidth=0.5,
            label='AutoDNA Average Fluorescence')
    draw_error_bars(ax2, x_positions, autoDNA_average_fluorescence, autoDNA_stdevalues)
    setup_axes(ax2, 'AutoDNA')

    # --- 3. 自定义坐标轴和标签 (Customize Axes and Labels) ---
    # 设置标题和Y轴标签
    # ax.set_title('Sample', fontsize=16, pad=15)
    if substract_ntc:
        ax1.set_ylabel('Background substracted\nFluorescence', fontsize=14)
    else:
        ax1.set_ylabel('Fluorescence', fontsize=14)

    # 在X轴下方添加多行文本标签
    # 首先调整图表底部边缘，为文本留出空间
    plt.subplots_adjust(bottom=0.2)


    # 添加统计显著性标记 (****)


    # 根据原图，调用函数绘制四个显著性标记
    # 注意：y坐标是手动选择的，以避免重叠并保持美观
    # draw_significance_bar(x_positions[2], x_positions[3], 3.0e4, '****')
    # draw_significance_bar(x_positions[1], x_positions[3], 5.5e4, '****')
    # draw_significance_bar(x_positions[0], x_positions[3], 9.5e4, '****')
    # draw_significance_bar(x_positions[0], x_positions[1], 9.2e4, '****')
    setup_significance(ax1, x_positions, fluorescence_values, p_values)
    setup_significance(ax2, x_positions, autoDNA_average_fluorescence, autoDNA_p_values)

    # --- 5. 调整并显示图表 (Final Adjustments and Display) ---
    # 自动调整布局，防止标签重叠
    plt.tight_layout(rect=[0, 0.05, 1, 1]) # 调整布局区域，为底部文本留出更多空间

    # create directory if it does not exist
    os.makedirs(os.path.dirname(output_path), exist_ok=True)
    if substract_ntc:
        plt.savefig(output_path_substracted, dpi=300, bbox_inches='tight')
    else:
        plt.savefig(output_path, dpi=300, bbox_inches='tight')

def generate_plot_split_2(average_fluorescence, stdevalues, p_values, autoDNA_average_fluorescence, autoDNA_stdevalues, autoDNA_p_values, substract_ntc=False, error_mode='3'):
    x_positions = np.arange(len(categories))

    # 定义每个柱子的高度 (荧光值)，根据原图估算
    fluorescence_values = average_fluorescence

    # 定义误差线的值 (例如标准差)，根据原图估算
    errors = stdevalues

    # 定义X轴下方的附加标签
    zikv_labels = categories_to_cp(categories)
    origin_labels = average_fluorescence

    # --- 2. 创建图表 (Create the Plot) ---
    # 创建一个画布(figure)和一个坐标系(axes)，设置图表大小
    fig, ax = plt.subplots(figsize=(11, 7))
    
    # 设置字体
    plt.rcParams['font.sans-serif'] = ['Arial Unicode MS']  # 设置无衬线字体
    plt.rcParams['font.family'] = 'sans-serif'

    # 绘制条形图，并添加误差线
    first_bar_position = x_positions
    second_bar_position = x_positions + x_positions[-1] + 3
    # 误差只包含上面
    
    def draw_error_bars(ax, x_positions, fluorescence_values, errors):
        """绘制误差线"""
        for i in range(len(x_positions)):
            # 误差线的垂直范围
            y_start = fluorescence_values[i]
            y_end = fluorescence_values[i] + errors[i]
            # 误差线的水平位置
            x = x_positions[i]

            # 绘制垂直线 (误差线主体)
            ax.vlines(x, y_start, y_end, color='black', linewidth=0.5, zorder=10)

            # 绘制水平线 (顶帽)
            cap_width = 0.15 # 调整顶帽的宽度
            ax.hlines(y_end, x - cap_width, x + cap_width, color='black', linewidth=0.5, zorder=10)

    
    # 加黑色边框
    bars = ax.bar(first_bar_position, fluorescence_values,
        color='orange',  # 第一组柱子颜色
        alpha=0.7,          # 设置透明度
        width=0.8,          # 设置柱子的宽度
        capsize=5,
        edgecolor='black',  # 添加黑色边框
        linewidth=0.5,  # 设置边框线宽度
        # 设置误差线下横线为0
        label='Human Average Fluorescence with Error')  # 添加图例标签
    draw_error_bars(ax, first_bar_position, fluorescence_values, errors)
    
    bars2 = ax.bar(second_bar_position, autoDNA_average_fluorescence,
        color='steelblue',  # 第二组柱子颜色
         alpha=0.7,          # 设置透明度
        width=0.8,          # 设置柱子的宽度
        capsize=5,
        linewidth=0.5,  # 设置边框线宽度
        edgecolor='black',  # 添加黑色边框
        label='AutoDNA Average Fluorescence with Error')  # 添加图例标签
    draw_error_bars(ax, second_bar_position, autoDNA_average_fluorescence, autoDNA_stdevalues)

    # --- 3. 自定义坐标轴和标签 (Customize Axes and Labels) ---
    # 设置标题和Y轴标签
    # ax.set_title('Sample', fontsize=16, pad=15)
    if substract_ntc:
        ax.set_ylabel('Background substracted\nFluorescence', fontsize=14)
    else:
        ax.set_ylabel('Fluorescence', fontsize=14)

    # 设置Y轴的范围和刻度格式
    if substract_ntc:
        ax.set_ylim(0, 30000)  # 设置Y轴范围为0到30000
    else:
        ax.set_ylim(0, 35000)  # 设置Y轴范围为0到35000
    
    # 将Y轴刻度格式化为科学记数法，例如 20000 -> 2 x 10^4
    ax.ticklabel_format(style='sci', axis='y', scilimits=(4,4), useMathText=True)
    ax.yaxis.get_offset_text().set_fontsize(12) # 调整 10^4 字体大小

    # 设置X轴刻度和标签
    positions = first_bar_position.tolist() + second_bar_position.tolist()
    ax.set_xticks(positions)
    labels = []
    for i in range(len(categories)):
        labels.append(str(i+1))
    for i in range(len(categories)):
        labels.append(str(i+1))
    ax.set_xticklabels(labels, rotation=0, ha='center', fontsize=12)
    # ax.set_xlabel('sample', fontsize=14, labelpad=10)

    # 移除顶部和右侧的边框线，使图表更简洁
    ax.spines['top'].set_visible(False)
    ax.spines['right'].set_visible(False)

    # 在X轴下方添加多行文本标签
    # 首先调整图表底部边缘，为文本留出空间
    plt.subplots_adjust(bottom=0.2)

    # 添加行标题
    ax.text(-0.01, -0.05, 'Sample', transform=ax.transAxes, fontsize=12, ha='left')
    ax.text(-0.01, -0.12, 'cp/µL', transform=ax.transAxes, fontsize=12, ha='left', va='center')
    # ax.text(-0.2, -0.22, 'Sample origin', transform=ax.transAxes, fontsize=12, ha='left')

    # 循环添加每列的文本
    y_offset1 = -0.12 # 第一行文本的Y坐标 (相对于坐标系)
    y_offset2 = -0.22 # 第二行文本的Y坐标
    for i, (zikv, origin) in enumerate(zip(zikv_labels, origin_labels)):
        # 使用 `ax.get_xaxis().get_major_ticks()[i].get_loc()` 获取精确的X位置
        center_x = first_bar_position[i]
        center_x2 = second_bar_position[i]
        # 修正X位置，使其与柱子对齐
        ax.text(center_x, y_offset1, zikv, transform=ax.get_xaxis_transform(),
                ha='center', va='center', fontsize=10)
        ax.text(center_x2, y_offset1, zikv, transform=ax.get_xaxis_transform(),
                ha='center', va='center', fontsize=10)
        # ax.text(center_x, y_offset2, origin, transform=ax.get_xaxis_transform(),
        #         ha='center', va='top', fontsize=8)

    # 添加统计显著性标记 (****)
    def draw_significance_bar(x1, x2, y, text, h=1000):
        """一个用于绘制显著性标记的辅助函数"""
        # 绘制横线和两端的垂直短线
        ax.plot([x1, x1, x2, x2], [y, y + h, y + h, y], lw=0.5, c='k')
        # 在横线上方添加文本 (例如 ****)
        ax.text((x1 + x2) * 0.5, y + h, text, ha='center', va='bottom', fontsize=14)

    # 根据原图，调用函数绘制四个显著性标记
    # 注意：y坐标是手动选择的，以避免重叠并保持美观
    # draw_significance_bar(x_positions[2], x_positions[3], 3.0e4, '****')
    # draw_significance_bar(x_positions[1], x_positions[3], 5.5e4, '****')
    # draw_significance_bar(x_positions[0], x_positions[3], 9.5e4, '****')
    # draw_significance_bar(x_positions[0], x_positions[1], 9.2e4, '****')
    upper = 1000
    for i in range(len(x_positions)):
        if i < 3:
            continue
        text = 'p=' + str(p_values[i]) + '    '
        if p_values[i] < 0.0001:
            text += '****'
        elif p_values[i] < 0.001:
            text += '***'
        elif p_values[i] < 0.01:
            text += '**'
        elif p_values[i] < 0.05:
            text += '*'
        draw_significance_bar(x_positions[0], x_positions[i], fluorescence_values[i] + upper, text)
        
        text = 'p=' + str(autoDNA_p_values[i]) + '    '
        if autoDNA_p_values[i] < 0.0001:
            text += '****'
        elif autoDNA_p_values[i] < 0.001:
            text += '***'
        elif autoDNA_p_values[i] < 0.01:
            text += '**'
        elif autoDNA_p_values[i] < 0.05:
            text += '*'
        draw_significance_bar(second_bar_position[0], second_bar_position[i], autoDNA_average_fluorescence[i] + upper, text)
    
    if error_mode == '3':
        err_line_value = 3 * fluorescence_values[0]
        ax.axhline(y=err_line_value, color='red', linestyle='--', linewidth=0.5)

    # --- 5. 调整并显示图表 (Final Adjustments and Display) ---
    # 自动调整布局，防止标签重叠
    plt.tight_layout(rect=[0, 0.05, 1, 1]) # 调整布局区域，为底部文本留出更多空间

    # create directory if it does not exist
    os.makedirs(os.path.dirname(output_path), exist_ok=True)
    if substract_ntc:
        plt.savefig(output_path_substracted, dpi=300, bbox_inches='tight')
    else:
        plt.savefig(output_path, dpi=300, bbox_inches='tight')  


def V2_generate_plot_split(average_fluorescence, stdevalues, p_values, autoDNA_average_fluorescence, autoDNA_stdevalues, autoDNA_p_values, substract_ntc=False, error_mode='3'):
    def draw_error_bars(ax, x_positions, fluorescence_values, errors):
        """绘制误差线"""
        for i in range(len(x_positions)):
            # 误差线的垂直范围
            y_start = fluorescence_values[i]
            y_end = fluorescence_values[i] + errors[i]
            # 误差线的水平位置
            x = x_positions[i]

            # 绘制垂直线 (误差线主体)
            ax.vlines(x, y_start, y_end, color='black', linewidth=0.75, zorder=10)

            # 绘制水平线 (顶帽)
            cap_width = 0.15 # 调整顶帽的宽度
            ax.hlines(y_end, x - cap_width, x + cap_width, color='black', linewidth=0.75, zorder=10)

    
    # reverse all labels
    global categories
    categories = categories[::-1]
    average_fluorescence = average_fluorescence[::-1]
    stdevalues = stdevalues[::-1]
    autoDNA_average_fluorescence = autoDNA_average_fluorescence[::-1]
    autoDNA_stdevalues = autoDNA_stdevalues[::-1]
    
    # --- Plotting ---
    # Setup the plot style
    plt.style.use('default')
    plt.rcParams['font.family'] = 'sans-serif'
    plt.rcParams['font.sans-serif'] = ['Arial', 'Helvetica', 'DejaVu Sans']
    plt.rcParams['axes.linewidth'] = 1.5

    x_positions = np.arange(len(categories))

    # Create a single figure and axes
    fig, ax = plt.subplots(figsize=(12, 7))

    # --- Define X-axis positions for two separate groups on one axis ---
    bar_width = 0.6
    # Positions for the first group (REVERSE)
    x_reverse = np.arange(len(categories))
    # Positions for the second group (SHINE), with a gap in between
    gap = 1
    x_shine = np.arange(len(categories)) + len(categories) + gap
    # Combine all positions for setting ticks
    all_x_positions = np.concatenate([x_reverse, x_shine])
    # Combine all labels for setting tick labels
    all_categories = categories * 2

    # --- Plotting on the single axis ---
    # Plot REVERSE group
    ax.bar(x_reverse, average_fluorescence, width=bar_width, color='steelblue', alpha=0.7,
        edgecolor='black', capsize=4, linewidth=0.75)
    draw_error_bars(ax, x_reverse, average_fluorescence, stdevalues)
    # for i, points in enumerate(reverse_points):
    #     jitter = np.random.uniform(-bar_width/4, bar_width/4, len(points))
    #     ax.scatter(x_reverse[i] + jitter, points, color='black', zorder=5, s=15)

    # Plot SHINE group
    ax.bar(x_shine, autoDNA_average_fluorescence, width=bar_width, color='orange', alpha=0.7,
        edgecolor='black', capsize=4, linewidth=0.75)
    draw_error_bars(ax, x_shine, autoDNA_average_fluorescence, autoDNA_stdevalues)
    # for i, points in enumerate(shine_points):
    #     jitter = np.random.uniform(-bar_width/4, bar_width/4, len(points))
    #     ax.scatter(x_shine[i] + jitter, points, color='black', zorder=5, s=15)

    # --- Customization ---
    # Title
    ax.set_title("RPA", fontsize=25, pad=10)

    # Axis labels
    ax.set_xlabel(r'cp $\mu$l$^{-1}$', fontsize=20, labelpad=10)
    ax.set_ylabel('Fluorescence (a.u.)', fontsize=20, labelpad=10)

    # X-axis ticks and labels
    ax.set_xlim(-0.8, len(categories) + len(categories) - 0.5 + gap)
    ax.set_xticks(all_x_positions)
    ax.set_xticklabels(categories_to_label(all_categories), fontsize=20, rotation='vertical')

    # Y-axis formatting
    ax.set_ylim(0, 3.5e4)
    yticks = np.arange(0, 3.6e4, 5e3)
    yticklabels = ['0'] + [f'${tick/1e4} \\times 10^4$' for tick in yticks[1:]]
    ax.set_yticks(yticks)
    ax.set_yticklabels(yticklabels, fontsize=20)
    ax.tick_params(axis='both', which='major', direction='out', length=6, width=1.5, labelsize=12)

    # Horizontal dashed line
    ax.axhline(y=(autoDNA_average_fluorescence[-1]+average_fluorescence[-1])/2*3, color='black', linestyle='--', linewidth=1.5)
    # ax.axhline(y=autoDNA_average_fluorescence[-1]*3, color='black', linestyle='--', linewidth=1.5)

    # Legend
    reverse_patch = mpatches.Patch(edgecolor='black', facecolor='steelblue', label='Human', alpha=0.7)
    shine_patch = mpatches.Patch(edgecolor='black', facecolor='orange', label='AutoDNA', alpha=0.7)
    ax.legend(handles=[reverse_patch, shine_patch], frameon=False, loc='upper right', bbox_to_anchor=(0.98, 0.95), fontsize=20)

    # Subplot label 'e'
    # ax.text(-0.08, 1.05, 'e', transform=ax.transAxes, fontsize=24, fontweight='bold')

    # Spines
    ax.spines['top'].set_visible(False)
    ax.spines['right'].set_visible(False)

    # Layout
    plt.tight_layout(rect=[0, 0, 1, 0.95])
    plt.savefig(output_split_path, dpi=300, bbox_inches='tight')

def V2_generate_plot(average_fluorescence, stdevalues, p_values, autoDNA_average_fluorescence, autoDNA_stdevalues, autoDNA_p_values, substract_ntc=False, error_mode='3'):
    def draw_error_bars(ax, x_positions, fluorescence_values, errors):
        """绘制误差线"""
        for i in range(len(x_positions)):
            # 误差线的垂直范围
            y_start = fluorescence_values[i]
            y_end = fluorescence_values[i] + errors[i]
            # 误差线的水平位置
            x = x_positions[i]

            # 绘制垂直线 (误差线主体)
            ax.vlines(x, y_start, y_end, color='black', linewidth=0.75, zorder=10)

            # 绘制水平线 (顶帽)
            cap_width = 0.15 # 调整顶帽的宽度
            ax.hlines(y_end, x - cap_width, x + cap_width, color='black', linewidth=0.75, zorder=10)
            # ax.hlines(y_start, x - cap_width, x + cap_width, color='black', linewidth=0.75, zorder=10)
    
    # reverse all labels
    global categories
    # categories = categories[::-1]
    average_fluorescence = average_fluorescence[::-1]
    stdevalues = stdevalues[::-1]
    autoDNA_average_fluorescence = autoDNA_average_fluorescence[::-1]
    autoDNA_stdevalues = autoDNA_stdevalues[::-1]
    
    # --- Plotting ---
    # Setup the plot style
    # plt.style.use('default')
    plt.rcParams['font.family'] = 'sans-serif'
    plt.rcParams['font.sans-serif'] = ['Arial', 'Helvetica', 'DejaVu Sans']
    plt.rcParams['axes.linewidth'] = 1.5

    x_positions = np.arange(len(categories))

    # Create a single figure and axes
    fig, ax = plt.subplots(figsize=(12, 7))

    # --- Define X-axis positions for two separate groups on one axis ---
    bar_width = 0.4
    # Positions for the first group (REVERSE)
    x_reverse = np.arange(len(categories)) - bar_width / 2
    # Positions for the second group (SHINE), with a gap in between
    gap = 1
    x_shine = np.arange(len(categories)) + bar_width / 2
    # Combine all positions for setting ticks
    # Combine all labels for setting tick labels
    all_categories = categories

    # --- Plotting on the single axis ---
    # Plot REVERSE group
    ax.bar(x_reverse, average_fluorescence, width=bar_width, color='steelblue', alpha=0.7, 
        edgecolor='black', capsize=4, linewidth=0.75)
    draw_error_bars(ax, x_reverse, average_fluorescence, stdevalues)
    # for i, points in enumerate(reverse_points):
    #     jitter = np.random.uniform(-bar_width/4, bar_width/4, len(points))
    #     ax.scatter(x_reverse[i] + jitter, points, color='black', zorder=5, s=15)

    # Plot SHINE group
    ax.bar(x_shine, autoDNA_average_fluorescence, width=bar_width, color='orange', alpha=0.7,
        edgecolor='black', capsize=4, linewidth=0.75)
    draw_error_bars(ax, x_shine, autoDNA_average_fluorescence, autoDNA_stdevalues)
    # for i, points in enumerate(shine_points):
    #     jitter = np.random.uniform(-bar_width/4, bar_width/4, len(points))
    #     ax.scatter(x_shine[i] + jitter, points, color='black', zorder=5, s=15)

    # --- Customization ---
    # Title
    ax.set_title("RPA", fontsize=25, pad=10)

    # Axis labels
    ax.set_xlabel(r'cp $\mu$l$^{-1}$', fontsize=20, labelpad=10)
    ax.set_ylabel('Fluorescence (a.u.)', fontsize=20, labelpad=10)

    # X-axis ticks and labels
    ax.set_xlim(-0.8, len(categories))
    ax.set_xticks(x_positions)
    ax.set_xticklabels(categories_to_label(all_categories), fontsize=20, rotation='vertical')

    # Y-axis formatting
    ax.set_ylim(0, 3.5e4)
    yticks = np.arange(0, 3.6e4, 5e3)
    yticklabels = ['0'] + [f'${tick/1e4} \\times 10^4$' for tick in yticks[1:]]
    ax.set_yticks(yticks)
    ax.set_yticklabels(yticklabels, fontsize=20)
    ax.tick_params(axis='both', which='major', direction='out', length=6, width=1.5, labelsize=12)

    # Horizontal dashed line
    ax.axhline(y=(autoDNA_average_fluorescence[-1]+average_fluorescence[-1])/2*3, color='black', linestyle='--', linewidth=1.5)
    # ax.axhline(y=autoDNA_average_fluorescence[-1]*3, color='black', linestyle='--', linewidth=1.5)

    # Legend
    reverse_patch = mpatches.Patch(edgecolor='black', facecolor='steelblue', label='Human', alpha=0.7)
    shine_patch = mpatches.Patch(edgecolor='black', facecolor='orange', label='AutoDNA', alpha=0.7)
    ax.legend(handles=[reverse_patch, shine_patch], frameon=False, loc='upper right', bbox_to_anchor=(0.98, 0.95), fontsize=20)

    # Subplot label 'e'
    # ax.text(-0.08, 1.05, 'e', transform=ax.transAxes, fontsize=24, fontweight='bold')

    # Spines
    ax.spines['top'].set_visible(False)
    ax.spines['right'].set_visible(False)

    # Layout
    plt.tight_layout(rect=[0, 0, 1, 0.95])
    plt.savefig(output_path, dpi=300, bbox_inches='tight')
    
def caculate_p_and_significance(data_path, data_auto_path):
    data = extract_data(data_path)
    original_data, max_group, max_cycle = group_data(data)
    data_auto = extract_data(data_auto_path)
    original_data_auto, max_group_auto, max_cycle_auto = group_data(data_auto)
    
    
    p_values = []
    human_stdevalues = []
    auto_stdevalues = []
    for cycle in range(1, max_cycle + 1):
        human_cycle_fluorescence = []
        auto_cycle_fluorescence = []
        for group in range(1, max_group + 1):
            if group in original_data and cycle in original_data[group]:
                human_cycle_fluorescence.append(original_data[group][cycle])
            if group in original_data_auto and cycle in original_data_auto[group]:
                auto_cycle_fluorescence.append(original_data_auto[group][cycle])
        if human_cycle_fluorescence:
            # standard deviation
            human_stdevalues.append(np.std(human_cycle_fluorescence, axis=0).tolist())
            human_stdevalues[-1] = [round(x, 4) for x in human_stdevalues[-1]]
            auto_stdevalues.append(np.std(auto_cycle_fluorescence, axis=0).tolist())
            auto_stdevalues[-1] = [round(x, 4) for x in auto_stdevalues[-1]]
            # p-values
            p_values.append([])
            for i in range(len(human_cycle_fluorescence[0])):
                # perform t-test for each cycle fluorescence value
                t_stat, p_value = stats.ttest_ind([x.tolist()[i] for x in human_cycle_fluorescence], [x.tolist()[i] for x in auto_cycle_fluorescence], equal_var=False)
                p_values[-1].append(round(p_value, 4))
                
    # save to csv
    with open(p_value_path, 'w', newline='') as csvfile:
        writer = csv.writer(csvfile)
        header = ['Data'] + [category for category in categories]
        writer.writerow(header)
        for cycle_index, p_value_row in enumerate(p_values):
            writer.writerow(['P values ' + str(cycle_index + 1)] + p_value_row)
            
        for cycle_index, stde_row in enumerate(human_stdevalues):
            writer.writerow(['Human Std ' + str(cycle_index + 1)] + stde_row)
        for cycle_index, stde_row in enumerate(auto_stdevalues):
            writer.writerow(['AutoDNA Std ' + str(cycle_index + 1)] + stde_row)
    return p_values

def main():
    # caculate_p_and_significance(data_path, data_auto_path)
    substract_ntc = False  # Change to True if you want to substract NTC
    average_fluorescence, stdevalues, p_values = prepare_data(substract_ntc, data_path)
    autoDNA_average_fluorescence, autoDNA_stdevalues, autoDNA_p_values = prepare_data(substract_ntc, data_auto_path)
    # V2_generate_plot_split(average_fluorescence, stdevalues, p_values, autoDNA_average_fluorescence, autoDNA_stdevalues, autoDNA_p_values, substract_ntc, error_mode)
    V2_generate_plot(average_fluorescence, stdevalues, p_values, autoDNA_average_fluorescence, autoDNA_stdevalues, autoDNA_p_values, substract_ntc, error_mode)
    
    
if __name__ == "__main__":
    main()


# # --- 1. 准备数据 (Data Preparation) ---
# # 定义X轴的类别
