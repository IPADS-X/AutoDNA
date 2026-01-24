import re
import matplotlib.pyplot as plt
import matplotlib.dates as mdates
import matplotlib.patches as mpatches
from datetime import datetime, timedelta
import pprint # 用于更美观地打印调试信息

log_file = "./concurrent_all.log"

with open(log_file, 'r') as f:
    log_data_full = f.read()
    
# --- 用户配置区 ---
# 请在这里定义 Workflow 和 实验名称 的对应关系
# 格式：'Workflow X': '您的实验名称'
experiment_map = {
    "Workflow 1": "Nucleic Acid Test",
    "Workflow 2": "Library Preparation",
    "Workflow 3": "PolyA",
    "Workflow 4": "Library Preparation",
}

experiment_map2 = {
    "Workflow 1": "PolyA",
    "Workflow 2": "Nucleic Acid Test",
    "Workflow 3": "Library Preparation",
    "Workflow 4": "PolyA",
}

# --- 配置区结束 ---

log_lines = log_data_full.strip().split('\n')

all_periods = []
temp_pcr_start_time = None
temp_fluo_start_time = None
temp_pipette_start_time = [None, None, None, None]
temp_pcr_workflow_id = None
temp_fluo_workflow_id = None
temp_pipette_workflow_id = [None, None, None, None]
temp_centrifuge_start_time = None
temp_centrifuge_workflow_id = None
temp_capper_start_time = None
temp_capper_workflow_id = None

pcr_alloc_pattern = re.compile(r'\[(.*?)\] \[Machine   \] \[debug\] Allocated equipment: PCR')
pcr_release_pattern = re.compile(r'\[(.*?)\] \[Machine   \] \[debug\] Realeased equipment: PCR')
fluo_alloc_pattern = re.compile(r'\[(.*?)\] \[Machine   \] \[debug\] Allocated equipment: FLUORESCENCE')
fluo_release_pattern = re.compile(r'\[(.*?)\] \[Machine   \] \[debug\] Realeased equipment: FLUORESCENCE')
pipette_alloc_pattern = re.compile(r'\[(.*?)\] \[Machine   \] \[debug\] Allocated equipment: PIPETEE_GUN')
pipette_release_pattern = re.compile(r'\[(.*?)\] \[Machine   \] \[debug\] Realeased equipment: PIPETEE_GUN')
centrifuge_alloc_pattern = re.compile(r'\[(.*?)\] \[Machine   \] \[debug\] Allocated equipment: CENTRIFUGE')
centrifuge_release_pattern = re.compile(r'\[(.*?)\] \[Machine   \] \[debug\] Realeased equipment: CENTRIFUGE')
capper_alloc_pattern = re.compile(r'\[(.*?)\] \[Machine   \] \[debug\] Allocated equipment: CAPPER')
capper_release_pattern = re.compile(r'\[(.*?)\] \[Machine   \] \[debug\] Realeased equipment: CAPPER')


# machine: fluorescence to step:
machine_alloc_pattern = re.compile(r'machine: (.*?) to step: (.*?)')
machine_release_pattern = re.compile(r'machine: (.*)')
machine_map = {
    "fluorescence": 0,
    "amplification": 1,
    "library": 2,
    "purification": 3
}

reverse_machine_map = {v: k for k, v in machine_map.items()}

workflow_pattern = re.compile(r'of workflow (\d+)')
# 新的、更宽松的heater匹配规则
heater_pattern = re.compile(r'\[(.*?)\] .*?LibHeat.*?workflow .*?enter phase0.*?"Duration":\s*(\d+)')

print("--- 开始解析日志 ---")
base_time = None
changed = False
for i, line in enumerate(log_lines):
    capper_alloc_match = capper_alloc_pattern.search(line)
    if capper_alloc_match:
        temp_capper_start_time = datetime.strptime(capper_alloc_match.group(1), '%H:%M:%S')
        if base_time == None:
            base_time = temp_capper_start_time
            print("change base"+line)
            temp_capper_start_time = timedelta(seconds=0)
        elif (temp_capper_start_time - base_time).total_seconds() > 10000 and not changed:
            base_time = base_time + timedelta(seconds=5500)
            temp_capper_start_time = temp_capper_start_time - base_time
            experiment_map = experiment_map2
            changed = True
        else:
            temp_capper_start_time = temp_capper_start_time - base_time
        if i + 1 < len(log_lines):
            workflow_match = workflow_pattern.search(log_lines[i+1])
            if workflow_match: temp_capper_workflow_id = f"Workflow {workflow_match.group(1)}"
            else: temp_capper_workflow_id = "Workflow Unknown"
        continue

    capper_release_match = capper_release_pattern.search(line)
    if capper_release_match:
        end_time = datetime.strptime(capper_release_match.group(1), '%H:%M:%S') - base_time
        experiment_type = experiment_map.get(temp_capper_workflow_id, "未知实验")
        all_periods.append({
            'instrument': 'Capper', 'start': temp_capper_start_time.total_seconds(),
            'end': end_time.total_seconds(), 'experiment': experiment_type
        })
        print(f">>> 解析到 CAPPER 周期: {temp_capper_start_time} - {end_time} [{experiment_type}]")
        temp_capper_start_time = None
        temp_capper_workflow_id = None
        continue
            
    if base_time == None:
        continue
    
    
    heater_match = heater_pattern.search(line)
    if heater_match:
        start_time = datetime.strptime(heater_match.group(1), '%H:%M:%S')
        print(int(heater_match.group(2))/1000)
        end_time = start_time + timedelta(seconds=int(heater_match.group(2))/1000)
        if base_time == None:
            base_time = start_time
            start_time = timedelta(seconds=0)
            end_time = start_time + timedelta(seconds=int(heater_match.group(2))/1000)
        elif (start_time - base_time).total_seconds() > 10000 and not changed:
            base_time = base_time + timedelta(seconds=5500)
            start_time = start_time - base_time
            end_time = start_time + timedelta(seconds=int(heater_match.group(2))/1000)
            experiment_map = experiment_map2
            changed = True
        else:
            start_time = start_time - base_time
            end_time = start_time + timedelta(seconds=int(heater_match.group(2))/1000)
        if end_time - start_time < timedelta(seconds=180):
            experiment_type = 'Library Preparation'
        else:
            experiment_type = 'PolyA'
        all_periods.append({
            'instrument': 'Heater', 'start': start_time.total_seconds(),
            'end': end_time.total_seconds(), 'experiment': experiment_type
        })
        print(f">>> 解析到 Heater 周期: {start_time} - {end_time} [{experiment_type}]")
        
    pcr_alloc_match = pcr_alloc_pattern.search(line)
    if pcr_alloc_match:
        temp_pcr_start_time = datetime.strptime(pcr_alloc_match.group(1), '%H:%M:%S')
        if base_time == None:
            base_time = temp_pcr_start_time
            temp_pcr_start_time = timedelta(seconds=0)
        else:
            temp_pcr_start_time = temp_pcr_start_time - base_time
        if i + 1 < len(log_lines):
            workflow_match = workflow_pattern.search(log_lines[i+1])
            if workflow_match: temp_pcr_workflow_id = f"Workflow {workflow_match.group(1)}"
            else: temp_pcr_workflow_id = "Workflow Unknown"
        continue

    pcr_release_match = pcr_release_pattern.search(line)
    if pcr_release_match and temp_pcr_start_time:
        end_time = datetime.strptime(pcr_release_match.group(1), '%H:%M:%S') - base_time
        experiment_type = experiment_map.get(temp_pcr_workflow_id, "未知实验")
        all_periods.append({
            'instrument': 'Thermal Cycler', 'start': temp_pcr_start_time.total_seconds(),
            'end': end_time.total_seconds(), 'experiment': experiment_type
        })
        print(f">>> 解析到 PCR 周期: {temp_pcr_start_time} - {end_time} [{experiment_type}]")
        temp_pcr_start_time = None
        temp_pcr_workflow_id = None
        continue


    fluo_alloc_match = fluo_alloc_pattern.search(line)
    if fluo_alloc_match:
        temp_fluo_start_time = datetime.strptime(fluo_alloc_match.group(1), '%H:%M:%S') - base_time
        if i + 1 < len(log_lines):
            workflow_match = workflow_pattern.search(log_lines[i+1])
            if workflow_match: temp_fluo_workflow_id = f"Workflow {workflow_match.group(1)}"
            else: temp_fluo_workflow_id = "Workflow Unknown"
        continue
    
    fluo_release_match = fluo_release_pattern.search(line)
    if fluo_release_match and temp_fluo_start_time:
        end_time = datetime.strptime(fluo_release_match.group(1), '%H:%M:%S') - base_time
        experiment_type = experiment_map.get(temp_fluo_workflow_id, "未知实验")
        all_periods.append({
            'instrument': 'Fluorometer', 'start': temp_fluo_start_time.total_seconds(),
            'end': end_time.total_seconds(), 'experiment': experiment_type
        })
        print(f">>> 解析到 FLUORESCENCE 周期: {temp_fluo_start_time} - {end_time} [{experiment_type}]")
        temp_fluo_start_time = None
        temp_fluo_workflow_id = None
        continue
    
    pipette_alloc_match = pipette_alloc_pattern.search(line)
    if pipette_alloc_match:
        j = i+1
        machine_match = machine_alloc_pattern.search(line)
        machine_id = machine_map[machine_match.group(1)] if machine_match else 0
        temp_pipette_start_time[machine_id] = datetime.strptime(pipette_alloc_match.group(1), '%H:%M:%S') - base_time
        while j < len(log_lines):
            workflow_match = workflow_pattern.search(log_lines[j])
            if workflow_match: 
                temp_pipette_workflow_id[machine_id] = f"Workflow {workflow_match.group(1)}"
                break
            else: 
                temp_pipette_workflow_id[machine_id] = "Workflow Unknown"
            j += 1
        continue

    pipette_release_match = pipette_release_pattern.search(line)
    if pipette_release_match and temp_pipette_start_time:
        end_time = datetime.strptime(pipette_release_match.group(1), '%H:%M:%S') - base_time
        machine_match = machine_release_pattern.search(line)
        machine_id = machine_map[machine_match.group(1)] if machine_match else 0
        experiment_type = experiment_map.get(temp_pipette_workflow_id[machine_id], "未知实验")
        if temp_pipette_start_time[machine_id] == None:
            continue
        all_periods.append({
            'instrument': str(reverse_machine_map[machine_id]).capitalize()+ '\'s\n Pipette & Robot Arm', 'start': temp_pipette_start_time[machine_id].total_seconds(),
            'end': end_time.total_seconds(), 'experiment': experiment_type
        })
        print(f">>> 解析到 PIPETTE_GUN 周期: {temp_pipette_start_time[machine_id]} - {end_time} [{experiment_type}]")
        # temp_pipette_start_time = None
        # temp_pipette_workflow_id = None
        continue
    
    centrifuge_alloc_match = centrifuge_alloc_pattern.search(line)
    if centrifuge_alloc_match:
        temp_centrifuge_start_time = datetime.strptime(centrifuge_alloc_match.group(1), '%H:%M:%S') - base_time
        if i + 1 < len(log_lines):
            workflow_match = workflow_pattern.search(log_lines[i+1])
            if workflow_match: temp_centrifuge_workflow_id = f"Workflow {workflow_match.group(1)}"
            else: temp_centrifuge_workflow_id = "Workflow Unknown"
        continue

    centrifuge_release_match = centrifuge_release_pattern.search(line)
    if centrifuge_release_match and temp_centrifuge_start_time:
        end_time = datetime.strptime(centrifuge_release_match.group(1), '%H:%M:%S') - base_time
        experiment_type = experiment_map.get(temp_centrifuge_workflow_id, "未知实验")
        all_periods.append({
            'instrument': 'Centrifuge', 'start': temp_centrifuge_start_time.total_seconds(),
            'end': end_time.total_seconds(), 'experiment': experiment_type
        })
        print(f">>> 解析到 CENTRIFUGE 周期: {temp_centrifuge_start_time} - {end_time} [{experiment_type}]")
        temp_centrifuge_start_time = None
        temp_centrifuge_workflow_id = None
        continue
print("--- 日志解析完成 ---\n")

if all_periods:
    print("--- 准备绘图数据 ---")
    pprint.pprint(all_periods)
    print("--- 开始绘图 ---")
    
    # 删除未知实验
    all_periods = [p for p in all_periods if p['experiment'] != "未知实验"]

    fig, ax = plt.subplots(figsize=(15, 5))

    experiments = sorted(list(set(p['experiment'] for p in all_periods)))
    # colors = plt.cm.get_cmap('viridis', len(experiments))
    # 使用浅色,带颜色的coler map
    # colors = plt.cm.get_cmap('tab20', len(experiments))
    OPERATION_COLORS = [ '#A5D8FF',  # 柔和的浅蓝色
    '#C1FFC1',   # 柔和的浅绿色
    '#FFC8C8',      # 柔和的浅粉色
    '#E6CCFF'      # 柔和的浅紫色
]
    color_map = {exp: OPERATION_COLORS[i] for i, exp in enumerate(experiments)}
    
    
    # 获取最大end time
    max_end_time = None
    max_polyA_time = None
    max_nuc_time = None
    for period in all_periods:
        end = period['end']
        if max_end_time is None or end > max_end_time:
            max_end_time = end
        if period['experiment'] == "PolyA":
            if max_polyA_time is None or end > max_polyA_time:
                max_polyA_time = end
        elif period['experiment'] == "Nucleic Acid Test":
            if max_nuc_time is None or end > max_nuc_time:
                max_nuc_time = end

    # 获取heater和pcr占用时间
    heater_occupied_polyA_time = 0
    heater_occupied_library_time = 0
    pcr_occupied_nuc_time = 0
    pcr_occupied_library_time = 0
    
    for period in all_periods:
        if period['instrument'] == 'Heater':
            if period['experiment'] == "PolyA":
                heater_occupied_polyA_time += period['end'] - period['start']
            elif period['experiment'] == "Library Preparation":
                heater_occupied_library_time += period['end'] - period['start']
        elif period['instrument'] == 'Thermal Cycler':
            if period['experiment'] == "Nucleic Acid Test":
                pcr_occupied_nuc_time += period['end'] - period['start']
            elif period['experiment'] == "Library Preparation":
                pcr_occupied_library_time += period['end'] - period['start']

    print("总时间：", max_end_time)
    print("PolyA最大时间：", max_polyA_time)
    print("Nuc最大时间：", max_nuc_time)
    print("Heater用于PolyA实验的时间：", heater_occupied_polyA_time)
    print("Heater用于Library实验的时间：", heater_occupied_library_time)
    print("PCR用于Nuc实验的时间：", pcr_occupied_nuc_time)
    print("PCR用于Library实验的时间：", pcr_occupied_library_time)

    # **代码修正点**: 先绘制数据，让matplotlib自动建立分类轴
    for period in all_periods:
        # print(period)
        if period['experiment'] == "未知实验":
            continue
        duration = period['end'] - period['start']
        color = color_map.get(period['experiment'], 'grey')
        ax.barh(period['instrument'], duration, left=period['start'], height=0.6,
                align='center', color=color, edgecolor='black', alpha=0.8)

    # **代码修正点**: 在绘图之后再调整坐标轴
    ax.invert_yaxis() # 翻转Y轴，让PCR在Heater上面

    min_time = min(p['start'] for p in all_periods) - 300
    max_time = max(p['end'] for p in all_periods) + 300
    ax.set_xlim(min_time, max_time)

    # ax.xaxis.set_major_formatter(mdates.DateFormatter('%H:%M:%S'))
    # ax.xaxis.set_major_locator(mdates.AutoDateLocator(minticks=10))
    fig.autofmt_xdate(rotation=30, ha='right')

    legend_patches = [mpatches.Patch(color=color, label=exp) for exp, color in color_map.items()]
    ax.legend(handles=legend_patches, bbox_to_anchor=(1.01, 1), loc='upper left')

    ax.set_xlabel('Time / seconds')
    ax.set_title('Instrument Occupancy by Experiment Type')

    plt.grid(axis='x', linestyle='--', alpha=0.7)
    plt.tight_layout(rect=[0, 0, 1, 1])

    plt.savefig('instrument_occupancy_timeline_corrected.svg')
    print("\n修正后的时间线图已生成并保存为 'instrument_occupancy_timeline_corrected.png'")
else:
    print("无法从日志中解析出任何仪器占用信息。")