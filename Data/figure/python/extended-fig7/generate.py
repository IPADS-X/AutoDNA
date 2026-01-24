import csv
import matplotlib.pyplot as plt
from matplotlib.patches import Rectangle
from matplotlib.colors import ListedColormap
import matplotlib.lines as mlines
from matplotlib.ticker import MultipleLocator, FuncFormatter
import pandas as pd

# 操作类型与颜色映射
# OPERATION_COLORS = {
#     'PuriMoveTube': 'blue',
#     'PuriPipette': 'green',
#     'PuriTime': 'red',
#     'PuriShake': 'purple'
# }

OPERATION_NAME = {
    'PuriMoveTube': 'move tube',
    'PuriPipette': 'pipette',
    'PuriTime': 'magnetic time',
    'PuriShake': 'heat&shake'
}

OPERATION_COLORS = {
    'PuriMoveTube': '#A5D8FF',  # 柔和的浅蓝色
    'PuriPipette': '#C1FFC1',   # 柔和的浅绿色
    'PuriTime': '#FFC8C8',      # 柔和的浅粉色
    'PuriShake': '#E6CCFF'      # 柔和的浅紫色
}

concurrent_num = 4
# seconds
origin_data_step = 10
step = 240

def parse_csv(filename):
    """解析CSV文件，返回操作记录和时间轴长度"""
    timeline = []  # 存储每个时间点的操作状态
    
    with open(filename, 'r') as f:
        reader = csv.reader(f)
        now_step = 0
        for row in reader:
            if now_step < step:
                now_step += origin_data_step
                continue
            now_step = 0
            # jump first column
            row = row[1:]  # 跳过第一列（时间戳）
            # 确保每行有8列（对应8个实验）
            if len(row) < concurrent_num:
                row += [''] * (concurrent_num - len(row))
            timeline.append(row)
    
    return timeline[1:]

def create_timeline_plot_multi(timeline):
    # extract timeline to two lists
    concurrent_datas = [[] for i in range(concurrent_num)]
    sequence_data = []
    for i in range(concurrent_num):
        for idx, row in enumerate(timeline):
            if i == 0:
                for j in range(len(row)):
                    if row[j] != ' ':
                        concurrent_datas[j].append({'step': idx, 'operations': row[j], 'type': 'concurrent-' + str(j + 1)})
            if row[0] != ' ':
                sequence_data.append({'step': len(sequence_data), 'operations': row[0], 'type': 'sequence'})
    
    # draw all datas
    # 2. Prepare Data for Plotting
    df_seq = pd.DataFrame(sequence_data)
    # df_seq['type'] = 'sequence'

    df_cons = [pd.DataFrame(concurrent_data) for concurrent_data in concurrent_datas]
    # df_con['type'] = 'concurrent'

    df = pd.concat(df_cons + [df_seq], ignore_index=True)
    
    
    # 5.1 定义每个通道的布局参数
    h_parallel_bar = 0.6 / 4       # 并行条块的高度
    h_parallel_channel = 0.2     # 每个并行通道的总高度
    h_serial_bar = 0.6           # 串行条块的高度
    h_serial_channel = 0.8       # 串行通道的总高度
    padding_between_groups = 0.3 # 串行与并行组之间的间距

    # 5.2 计算每个通道的精确Y坐标
    y_coords_map = {}
    y_tick_labels = []
    y_tick_positions = []
    current_y = 0

    # 计算并行通道的位置 (从下往上)
    for i in range(concurrent_num):
        type_name = f'concurrent-{i + 1}'
        center_y = current_y + h_parallel_channel / 2
        y_coords_map[type_name] = center_y
        y_tick_labels.append(type_name)
        y_tick_positions.append(center_y)
        current_y += h_parallel_channel

    # 增加组间距
    current_y += padding_between_groups

    # 计算串行通道的位置
    type_name = 'sequence'
    center_y = current_y + h_serial_channel / 2
    y_coords_map[type_name] = center_y
    y_tick_labels.append(type_name)
    y_tick_positions.append(center_y)

    # 3. Create the Plot
    # 5. 创建图表
    fig, ax = plt.subplots(figsize=(15, 5))

    # 6. 使用 barh 绘制方块
    # 遍历每一行数据，并为每个步骤绘制一个单位宽度的水平条
    for _, row in df.iterrows():
        ax.barh(
            y=y_coords_map[row['type']],  # 使用预先计算的Y坐标
            width=step,                 # 方块宽度，代表一个时间步
            left=row['step']*step,        # 方块的起始位置（X轴）
            height=0.6/4 if 'concurrent' in row['type'] else 0.6,  # 并行操作的高度为0.6，串行操作的高度为0.8
            color=OPERATION_COLORS[row['operations']], # 根据操作类型填充颜色
            edgecolor="black",       # 添加黑色边框以区分相邻方块
            linewidth=0.1,
            zorder=2  # 确保方块在上层
        )
        
    end_time_concurrent = df_cons[0]['step'].max() * step + step
    end_time_sequence = df_seq['step'].max() * step + step

    # 7.2 绘制垂直虚线
    ax.axvline(x=end_time_concurrent, color='dimgray', linestyle='--', linewidth=1.5, zorder=1) # zorder=1 将虚线置于下层
    ax.axvline(x=end_time_sequence, color='dimgray', linestyle='--', linewidth=1.5, zorder=1)
    
    ax.text(end_time_concurrent, -0.1, f'All Concurrent Ops End\n{end_time_concurrent:.0f}s', 
        ha='center', va='top', color='dimgray', fontsize=10)
    ax.text(end_time_sequence, -0.1, f'All Sequential Ops End\n{end_time_sequence:.0f}s', 
            ha='center', va='top', color='dimgray', fontsize=10)
    
    for i in range(concurrent_num):
        ax.axvline(end_time_sequence/concurrent_num * (i + 1), color='dimgray', linestyle='--', linewidth=0.5, zorder=1)
        # ax.text(end_time_sequence/concurrent_num * (i + 1), -0.6, f'Seq: {i+1} End', 
        #         ha='center', va='top', color='dimgray', fontsize=10)
        ax.annotate(f'Seq: {i+1} End\n{end_time_sequence/concurrent_num * (i + 1):.0f}s',
                    xy=(end_time_sequence/concurrent_num * (i + 1), 1.2),
                    xytext=(end_time_sequence/concurrent_num * (i + 1), 0.9),
                    ha='center', va='bottom', color='dimgray', fontsize=10,
                    arrowprops=dict(arrowstyle='->', color='dimgray', lw=0.5))

    # 7. 格式化图表
    ax.set_xlabel('Time', fontsize=12)
    ax.set_ylabel('Sequence or Concurrent', fontsize=12)
    ax.set_title('Time of Concurrent vs Sequence', fontsize=14)

    # 调整X轴范围，留出一些边距
    ax.set_xlim(-1, len(sequence_data)*step + step * 3)  # 根据数据长度设置X轴范围

    # Y轴标签顺序可能需要手动调整以确保 '串行' 在上
    
    ax.set_yticks(y_tick_positions)
    ax.set_yticklabels(y_tick_labels)
    ax.set_ylim(0, current_y + h_serial_channel) # 调整Y轴范围以适应新布局
    
    # y_labels = sorted(df['type'].unique(), reverse=False)
    # ax.set_yticklabels(y_labels)
    # ax.set_yticks(range(len(y_labels)))


    ax.grid(axis='x', linestyle='--', alpha=0.6)

    # 8. 创建自定义图例
    legend_handles = [mlines.Line2D([], [], color=color, marker='s', linestyle='None',
                                    markersize=10, label=OPERATION_NAME[op_type], markeredgecolor='black')
                    for op_type, color in OPERATION_COLORS.items()]

    ax.legend(handles=legend_handles, title="Operation Type", bbox_to_anchor=(1.01, 1), loc='upper left')

    # 调整布局以确保图例不会被裁剪
    plt.tight_layout(rect=[0, 0, 0.88, 1])

    # 显示并保存图表
    plt.savefig('concurrent_vs_sequence_plot_blocks.svg', dpi=300, bbox_inches='tight', format = 'svg')
    # plt.savefig('concurrent_vs_sequence_plot_blocks.png', dpi=300, bbox_inches='tight')

    # plt.show()
    
def create_vertical_timeline_plot_multi(timeline):
    """
    生成一个垂直的时间线图，其中X轴代表不同的执行通道，Y轴代表时间。
    """
    # 1. 提取时间线数据到列表中
    concurrent_datas = [[] for _ in range(concurrent_num)]
    sequence_data = []
    for i in range(concurrent_num):
        for idx, row in enumerate(timeline):
            if i == 0:
                for j in range(len(row)):
                    if row[j] != ' ':
                        concurrent_datas[j].append({'step': idx, 'operations': row[j], 'type': 'concurrent-' + str(j + 1)})
            if row[0] != ' ':
                sequence_data.append({'step': len(sequence_data), 'operations': row[0], 'type': 'sequence'})
    
    # 2. 准备用于绘图的DataFrame
    df_seq = pd.DataFrame(sequence_data)
    df_cons = [pd.DataFrame(concurrent_data) for concurrent_data in concurrent_datas]
    df = pd.concat(df_cons + [df_seq], ignore_index=True)
    
    # 3. 定义每个通道在X轴上的布局参数 (原为h_, 表示height)
    w_parallel_bar = 0.6 / 4      # 并行条块的宽度
    w_parallel_channel = 0.2      # 每个并行通道的总宽度
    w_serial_bar = 0.6            # 串行条块的宽度
    w_serial_channel = 0.8        # 串行通道的总宽度
    padding_between_groups = 0.3  # 串行与并行组之间的间距

    # 4. 计算每个通道在X轴上的精确坐标
    x_coords_map = {}
    x_tick_labels = []
    x_tick_positions = []
    current_x = 0

    # 计算并行通道的位置 (从左到右)
    for i in range(concurrent_num):
        type_name = f'concurrent-{i + 1}'
        center_x = current_x + w_parallel_channel / 2
        x_coords_map[type_name] = center_x
        x_tick_labels.append(type_name)
        x_tick_positions.append(center_x)
        current_x += w_parallel_channel

    # 增加组间距
    current_x += padding_between_groups

    # 计算串行通道的位置
    type_name = 'sequence'
    center_x = current_x + w_serial_channel / 2
    x_coords_map[type_name] = center_x
    x_tick_labels.append(type_name)
    x_tick_positions.append(center_x)

    # 5. 创建图表 (交换了figsize的宽高，使其变为竖向)
    fig, ax = plt.subplots(figsize=(15, 15))

    # 6. 使用 ax.bar (垂直条形图) 绘制方块
    for _, row in df.iterrows():
        ax.bar(
            x=x_coords_map[row['type']],         # 【改动】方块的X轴中心位置
            height=step,                         # 【改动】方块的高度，代表一个时间步
            bottom=row['step'] * step,           # 【改动】方块的起始位置 (Y轴)
            width=w_parallel_bar if 'concurrent' in row['type'] else w_serial_bar, # 【改动】方块的宽度
            color=OPERATION_COLORS.get(row['operations'], 'gray'), # 根据操作类型填充颜色
            edgecolor="black",                   # 添加黑色边框以区分相邻方块
            linewidth=0.1,
            zorder=2                             # 确保方块在上层
        )
        
    end_time_concurrent = df_cons[0]['step'].max() * step + step
    end_time_sequence = df_seq['step'].max() * step + step

    # 7. 绘制水平虚线 (原为ax.axvline)
    ax.axhline(y=end_time_concurrent, color='dimgray', linestyle='--', linewidth=1.5, zorder=1)
    ax.axhline(y=end_time_sequence, color='dimgray', linestyle='--', linewidth=1.5, zorder=1)
    
    # 调整文本位置 (交换X, Y坐标，并调整对齐方式)
    ax.text(ax.get_xlim()[0] - 0.1, end_time_concurrent, f'All Concurrent Ops End:\n{end_time_concurrent:.0f}s', 
            ha='center', va='center', color='dimgray', fontsize=10)
    ax.text(ax.get_xlim()[0] - 0.1, end_time_sequence, f'All Sequential Ops End:\n{end_time_sequence:.0f}s', 
            ha='center', va='center', color='dimgray', fontsize=10)
    
    for i in range(concurrent_num):
        y_pos = end_time_sequence / concurrent_num * (i + 1)
        ax.axhline(y=y_pos, color='dimgray', linestyle='--', linewidth=0.5, zorder=1)
        # 调整箭头标注 (交换X, Y坐标)
        ax.annotate(f'Seq: {i+1} End\n{y_pos:.0f}s',
                    xy=(x_coords_map['sequence'] - 0.3, y_pos),
                    xytext=(x_coords_map['sequence'] - 0.5, y_pos),
                    ha='center', va='center', color='dimgray', fontsize=10,
                    arrowprops=dict(arrowstyle='->', color='dimgray', lw=0.5))

    # 8. 格式化图表 (交换X, Y轴标签)
    ax.set_xlabel('Sequence or Concurrent', fontsize=12)
    ax.set_ylabel('Time (seconds)', fontsize=12)
    ax.set_title('Time of Concurrent vs Sequence', fontsize=14)

    # 调整Y轴范围 (原为X轴)
    ax.set_ylim(-1, len(sequence_data) * step + step * 5)
    ax.set_xlim(0, current_x + w_serial_channel / 2 + 0.2) # 调整X轴范围以适应新布局

    # 设置X轴刻度和标签 (原为Y轴)
    ax.set_xticks(x_tick_positions)
    ax.set_xticklabels(x_tick_labels, rotation=45, ha="right") # 旋转标签以防重叠

    # 更改网格线方向
    ax.grid(axis='y', linestyle='--', alpha=0.6)

    # 9. 创建自定义图例 (这部分无需改动)
    legend_handles = [mlines.Line2D([], [], color=color, marker='s', linestyle='None',
                                      markersize=10, label=op_name, markeredgecolor='black')
                      for op_type, color in OPERATION_COLORS.items() if (op_name := OPERATION_NAME.get(op_type)) is not None]

    ax.legend(handles=legend_handles, title="Operation Type", bbox_to_anchor=(1.02, 1), loc='upper left')

    # 调整布局以确保图例和标签不会被裁剪
    plt.tight_layout(rect=[0, 0, 0.88, 1])

    # 显示并保存图表
    plt.savefig('concurrent_vs_sequence_vertical_plot.png', dpi=300, bbox_inches='tight', format = 'png')
    plt.savefig('concurrent_vs_sequence_vertical_plot.svg', dpi=300, bbox_inches='tight', format = 'svg')
    # plt.show()

def create_timeline_plot(timeline):
    """创建时序图"""
    fig, ax = plt.subplots(figsize=(10, 100))
    fig, ax = plt.subplots(figsize=(10, len(timeline) / 5))  # 根据时间轴长度调整图形高度
    
    # 设置坐标轴和标签
    ax.set_xlabel('Experiment ID')
    ax.set_ylabel('Time (minutes)')
    ax.set_title('Experiment Operation Timeline')
    ax.set_xticks(range(1, concurrent_num + 1))
    # ax.set_xticks(range(1, 5))
    
    # y_ticks = [i/2 for i in range(0, len(timeline), step)]
    # ax.set_yticks(y_ticks)
    # 自定义Y轴格式化函数
    def scale_y_axis(y, pos):
        """将Y轴数值除以2"""
        return f'{step*y/2:.1f}' if y % 2 != 0 else f'{int(y/2)}'

    # 应用格式化函数
    ax.yaxis.set_major_formatter(FuncFormatter(scale_y_axis))
    ax.yaxis.set_major_locator(MultipleLocator(step))
    # ax.set_yticks(range(0, len(timeline), step))
    # ax.grid(True, axis='y', linestyle='--', alpha=0.7)
    ax.set_xlim(0.5, concurrent_num + 0.5)
    # ax.set_xlim(0.5, 4.5)
    ax.set_ylim(0, len(timeline))
    
    # 创建图例
    legend_handles = []
    for op, color in OPERATION_COLORS.items():
        legend_handles.append(Rectangle((0, 0), 1, 1, color=color, label=OPERATION_NAME[op]))
    ax.legend(handles=legend_handles, loc='upper right')
    
    # 绘制每个时间点的操作
    for time_idx, row in enumerate(timeline):
        for exp_idx in range(concurrent_num):
        # for exp_idx in range(4):
            operation = row[exp_idx].strip()
            if operation in OPERATION_COLORS:
                # 绘制操作方块
                ax.add_patch(Rectangle(
                    (exp_idx + 0.6, time_idx),  # 位置 (x, y)
                    0.8, 0.8,                         # 宽度和高度
                    color=OPERATION_COLORS[operation],
                    alpha=0.7
                ))
                # 添加操作名称缩写
                # ax.text(
                #     exp_idx + 1, time_idx,
                #     operation[4:6],  # 取操作名的缩写 (e.g., "Mo" from "PuriMoveTube")
                #     ha='center', va='center',
                #     fontsize=8, color='white'
                # )
    
    plt.tight_layout()
    plt.savefig('experiment_timeline.png', dpi=300, bbox_inches='tight')
    # plt.show()
    
    

# 使用示例
if __name__ == "__main__":
    # 生成示例CSV文件
    # sample_data = [
    #     ['', '', '', '', 'PuriPipette', 'PuriPipette', 'PuriPipette', 'PuriPipette'],
    #     ['', '', '', 'PuriMoveTube', 'PuriShake', 'PuriShake', 'PuriTime', 'PuriTime'],
    #     ['PuriMoveTube', '', 'PuriTime', 'PuriMoveTube', 'PuriMoveTube', 'PuriShake', 'PuriTime', ''],
    #     ['PuriMoveTube', 'PuriShake', 'PuriTime', '', 'PuriPipette', 'PuriShake', 'PuriShake', 'PuriMoveTube']
    # ]
    
    # # 写入示例CSV文件
    # with open('experiment_timeline.csv', 'w', newline='') as f:
    #     writer = csv.writer(f)
    #     writer.writerows(sample_data)
    
    # 解析并绘图
    timeline_data = parse_csv('./concurrent_info_4.csv')
    create_timeline_plot_multi(timeline_data)