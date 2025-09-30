from typing import List, Optional, Any, Dict
import json
import atexit
from datetime import datetime
import os


EXECUTION_LOG = {
    "protocol_start_time": datetime.now().isoformat(),
    "steps": []
}

def save_log_on_exit():
    # open now file location
    path = os.getenv("SCHEDULER_CONFIG_PATH", os.path.dirname(os.path.abspath(__file__)))
    path = os.path.join(path, 'protocol_flow.json')
    with open(path, 'w', encoding='utf-8') as f:
        json.dump(EXECUTION_LOG, f, indent=4, ensure_ascii=False)
    print(f"\n--- Execution log automatically saved to protocol_flow.json ---")

# 注册这个函数，以便在脚本正常退出时被调用
atexit.register(save_log_on_exit)


def _log_step(action, params):
    def safe_repr(obj):
        # 优先判断对象是否为JSON原生支持的类型，如果是，则直接返回
        if isinstance(obj, (list, dict, str, int, float, bool, type(None))):
            return obj
        # 对于其他复杂对象（如自定义类的实例），才使用其字符串表示形式
        return repr(obj)

    step = {
        "id": len(EXECUTION_LOG["steps"]) + 1,
        "action": action,
        "parameters": {k: safe_repr(v) for k, v in params.items()}
    }
    EXECUTION_LOG["steps"].append(step)

container_id_to_type_map = {}
containers_pos_map = {}
def call_machine_command(action, params):
    container_type = None
    container_index = None
    for (k,v) in params.items():
        if ("container_index" in k):
            if v in container_id_to_type_map:
                container_type = container_id_to_type_map[v]
                container_index = v
        if ("container_type" in k):
            container_type = v

    for (k,v) in params.items():
        if ("dst_pos" in k):
            containers_pos_map[container_index] = v
    
    # if machine start, output containers list on it
    if (action == "heatershaker_start"):
        containers = []
        for (k,v) in containers_pos_map.items():
            if ("heater_shaker" in v):
                containers.append(k)
        params["containers_on_machine"] = list(containers)
        params["pos"] = "heater_shaker"
    
    if (action == "heater_start"):
        containers = []
        for (k,v) in containers_pos_map.items():
            if ("heater" in v):
                containers.append(k)
        params["containers_on_machine"] = list(containers)
        params["pos"] = "heater"
        
    if (action == "thermal_cycler_run_program"):
        containers = []
        for (k,v) in containers_pos_map.items():
            if ("thermocycler" in v):
                containers.append(k)
        params["containers_on_machine"] = list(containers)
        params["pos"] = "thermocycler"

    params["container_type"] = container_type.name if container_type else "Unknown"
    _log_step(action, params)
    if action == "fluorometer_measure":
        print(f"Below are Just Mockup results. Ignore them.")
        # The number of containers is passed in the params from the instrument module.
        num_containers = params.get("num_containers", 0)
        # print(f"Fluorometer measured {num_containers} containers. Returning mock data.")
        # Return a list of -1s, with the length matching the number of containers.
        return [-1] * num_containers
    return 0