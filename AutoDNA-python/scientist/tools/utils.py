from config import output_path, prompt_path, settings, input_path, reagent_path
import os
import json
import re
import itertools
from pypdf import PdfReader
from typing import List
from tools.file_manager import file_manager
import pdfplumber
import pandas as pd


class ResponseMock:
    content = ""


def static_init(cls):
    if getattr(cls, "static_init", None):
        cls.static_init()
    return cls


def get_last_task_id_from_file():
    # Get the last task ID from the file
    try:
        # read files from ai_scientist directory
        files = os.listdir(os.path.join(output_path, "ai_scientist"))
        # all files are named as 'taskxxx-stepxxx-tool-coder-input.json', find lastest one
        files = sorted(files, key=lambda x: int(
            re.search(r'task(\d+)', x).group(1)))
        # get the lastest file
        latest_file = files[-1] if files else None
        # get the task id from the file name
        if latest_file:
            task_id = int(re.search(r'task(\d+)', latest_file).group(1))
            return task_id
    except Exception as e:
        return 0

@static_init
class AnswerCache:
    answers = {}
    answer_dir = output_path + "/answers"
    cache_file = answer_dir + "/cache.json"

    @classmethod
    def static_init(cls):
        """
        Initializes the cache by creating the directory and loading from the cache file.
        """
        if not os.path.exists(cls.answer_dir):
            os.makedirs(cls.answer_dir)
        
        if not os.path.exists(cls.cache_file):
            return
            
        try:
            with open(cls.cache_file, "r", encoding="utf-8") as f:
                cls.answers = json.load(f)
        except (json.JSONDecodeError, IOError):
            # If the file is corrupt or unreadable, start with an empty cache
            cls.answers = {}

    @classmethod
    def push_question_and_answer(cls, question, answer):
        """
        Adds a new question-answer pair to the cache and saves it.
        """
        cls.answers[question] = answer
        cls.save()

    @classmethod
    def get_answer(cls, question):
        """
        Retrieves an answer for a given question from the cache.
        Returns None if the question is not found.
        """
        # Use .get() for safer dictionary access to avoid KeyErrors
        return cls.answers.get(question)

    @classmethod
    def save(cls):
        """
        Saves the current state of the answers dictionary to the cache file.
        """
        # Ensure the directory exists before writing
        if not os.path.exists(cls.answer_dir):
            os.makedirs(cls.answer_dir)
        
        # Use a 'with' statement to ensure the file is properly closed
        with open(cls.cache_file, "w", encoding="utf-8") as f:
            json.dump(cls.answers, f, indent=4)


@static_init
class SelfOptimizingCache:
    optimizing_history = []
    result_history = []
    tmp_optimizing_history = []
    tmp_result_history = []
    hypothesis_history = []
    hypothesis_extract_history = []
    hypothesis_paper_extract_history = []
    hypothesis_input_history = []
    optimizing_summary_input_history = []
    finish_signal_history = []
    paper_hypothesis_history = []
    paper_origin_hypothesis_history = []
    paper_hypothesis_input_history = []
    # Keep separate cache dir for initial generation vs corrected code
    cache_dir = os.path.join(output_path, "self_optimizing_cache")
    hypothesis_dir = os.path.join(output_path, "hypothesis_cache")
    finish_signal_dir = os.path.join(output_path, "finish_signal_cache")

    @classmethod
    def static_init(cls):
        if not os.path.exists(cls.cache_dir):
            os.makedirs(cls.cache_dir, exist_ok=True)  # Create dir if needed
        if not os.path.exists(cls.hypothesis_dir):
            os.makedirs(cls.hypothesis_dir, exist_ok=True)
        if not os.path.exists(cls.finish_signal_dir):
            os.makedirs(cls.finish_signal_dir, exist_ok=True)
        try:
            print(f"Loading initial optimizing cache from {cls.cache_dir}...")
            # get all files
            files = os.listdir(cls.cache_dir)
            # sort file name by the number in the file name
            def get_key(file):
                match = re.search(r'(\d+)', file)
                return int(match.group(1)) if match else 0
            files = sorted(files, key=get_key)

            for file in files:
                # open the file and get the content
                with open(os.path.join(cls.cache_dir, file), 'r', encoding='utf-8') as f:
                    content = f.read()
                    if file.find("tmp_optimizing") != -1:
                        cls.tmp_optimizing_history.append(content)
                    elif file.find("tmp_result") != -1:
                        cls.tmp_result_history.append(content)
                    elif file.find("optimizing") != -1:
                        cls.optimizing_history.append(content)
                    elif file.find("result") != -1:
                        cls.result_history.append(content)
            files = os.listdir(cls.hypothesis_dir)
            # sort file name by the number in the file name
            files = sorted(files, key=lambda x: int(
                re.search(r'(\d+)', x).group(1)))
            for file in files:
                # open the file and get the content
                with open(os.path.join(cls.hypothesis_dir, file), 'r', encoding='utf-8') as f:
                    content = f.read()
                    if file.find("optimizing_summary_input") != -1:
                        cls.optimizing_summary_input_history.append(content)
                    elif file.find("paper_hypothesis_input") != -1:
                        cls.paper_hypothesis_input_history.append(content)
                    elif file.find("hypothesis_input") != -1:
                        cls.hypothesis_input_history.append(content)
                    elif file.find("paper_hypothesis_extract") != -1:
                        cls.hypothesis_paper_extract_history.append(content)
                    elif file.find("hypothesis_extract") != -1:
                        cls.hypothesis_extract_history.append(content)
                    elif file.find("paper_origin_hypothesis") != -1:
                        cls.paper_origin_hypothesis_history.append(content)
                    elif file.find("paper_hypothesis") != -1:
                        cls.paper_hypothesis_history.append(content)
                    else:
                        cls.hypothesis_history.append(content)
            files = os.listdir(cls.finish_signal_dir)
            files = sorted(files, key=lambda x: int(
                re.search(r'(\d+)', x).group(1)))
            for file in files:
                # open the file and get the content
                with open(os.path.join(cls.finish_signal_dir, file), 'r', encoding='utf-8') as f:
                    content = f.read()
                    cls.finish_signal_history.append(content)
        except Exception as e:
            print(
                f"Warning: Error loading initial optimizing cache from {cls.cache_dir}: {e}")

    @staticmethod
    def push_optimizing(optimizing):
        SelfOptimizingCache.optimizing_history.append(optimizing)
        SelfOptimizingCache.save()

    @staticmethod
    def push_result(result):
        SelfOptimizingCache.result_history.append(result)
        SelfOptimizingCache.save()
        
    @staticmethod
    def mark_result_rollback(index):
        result = SelfOptimizingCache.result_history[index] if index < len(SelfOptimizingCache.result_history) else ""
        if result.endswith("(Have been rolled back)"):
            return
        SelfOptimizingCache.result_history[index] = f"{result}(Have been rolled back)"
        SelfOptimizingCache.save()
        
    @staticmethod
    def mark_optimizing_rollback(index):
        optimizing = SelfOptimizingCache.optimizing_history[index] if index < len(SelfOptimizingCache.optimizing_history) else ""
        if optimizing.endswith("(Determined to be wrong hypothesis because not effective)"):
            return
        SelfOptimizingCache.optimizing_history[index] = f"{optimizing}(Determined to be wrong hypothesis because not effective)"
        SelfOptimizingCache.save()

    @staticmethod
    def push_finish_signal(finish_signal):
        SelfOptimizingCache.finish_signal_history.append(finish_signal)
        SelfOptimizingCache.save()

    @staticmethod
    def push_optimizing_summary_input(summary):
        SelfOptimizingCache.optimizing_summary_input_history.append(summary)
        SelfOptimizingCache.save()

    @staticmethod
    def clear_tmp_optimizing_history():
        SelfOptimizingCache.tmp_optimizing_history = []
        SelfOptimizingCache.save()

    @staticmethod
    def clear_tmp_result_history():
        SelfOptimizingCache.tmp_result_history = []
        SelfOptimizingCache.save()

    @staticmethod
    def push_tmp_optimizing(optimizing):
        SelfOptimizingCache.tmp_optimizing_history.append(optimizing)
        SelfOptimizingCache.save()

    @staticmethod
    def push_tmp_result(result):
        SelfOptimizingCache.tmp_result_history.append(result)
        SelfOptimizingCache.save()

    @staticmethod
    def push_hypothesis(hypothesis):
        SelfOptimizingCache.hypothesis_history.append(hypothesis)
        SelfOptimizingCache.save()
        
    @staticmethod
    def push_hypothesis_extract(hypothesis_extract):
        SelfOptimizingCache.hypothesis_extract_history.append(hypothesis_extract)
        SelfOptimizingCache.save()
        
    @staticmethod
    def push_hypothesis_paper_extract(hypothesis_extract):
        SelfOptimizingCache.hypothesis_paper_extract_history.append(hypothesis_extract)
        SelfOptimizingCache.save()

    @staticmethod
    def push_hypothesis_input(hypothesis_input):
        SelfOptimizingCache.hypothesis_input_history.append(hypothesis_input)
        SelfOptimizingCache.save()

    @staticmethod
    def push_paper_hypothesis_input(hypothesis_input):
        SelfOptimizingCache.paper_hypothesis_input_history.append(
            hypothesis_input)
        SelfOptimizingCache.save()

    @staticmethod
    def push_paper_hypothesis(hypothesis):
        SelfOptimizingCache.paper_hypothesis_history.append(hypothesis)
        SelfOptimizingCache.save()

    @staticmethod
    def push_paper_origin_hypothesis(hypothesis):
        SelfOptimizingCache.paper_origin_hypothesis_history.append(hypothesis)
        SelfOptimizingCache.save()

    @classmethod
    def save_best_option(cls, option:str):
        if not os.path.exists(SelfOptimizingCache.cache_dir):
            os.makedirs(SelfOptimizingCache.cache_dir, exist_ok=True)
        filepath = os.path.join(cls.cache_dir, "best_option.txt")
        with open(filepath, "w", encoding='utf-8') as f:
            f.write(option)
        
    @staticmethod
    def get_latest():
        max_len = max(len(SelfOptimizingCache.optimizing_history),
                      len(SelfOptimizingCache.result_history))
        last_optimizing = SelfOptimizingCache.optimizing_history[-1] if len(
            SelfOptimizingCache.optimizing_history) > 0 else ""
        last_result = SelfOptimizingCache.result_history[-1] if len(
            SelfOptimizingCache.result_history) > 0 else ""
        return last_optimizing, last_result

    @staticmethod
    def get_exist_hypothesis():
        string = "Here is the hypothesis history:\n"
        for i, hypothesis in enumerate(SelfOptimizingCache.tmp_optimizing_history):
            string += f"Hypothesis {i}: {hypothesis}\n"
        return string

    @staticmethod
    def get_tmp_index(index):
        string = "Here is the optimizing history:\n"
        if index < len(SelfOptimizingCache.tmp_optimizing_history):
            string += f"Optimizing {index}: {SelfOptimizingCache.tmp_optimizing_history[index]}\n"
        string += "And the result of it is:\n"
        if index < len(SelfOptimizingCache.tmp_result_history):
            string += f"Result {index}: {SelfOptimizingCache.tmp_result_history[index]}\n"

        return string

    @staticmethod
    def get_optimizing_history_and_result_history():
        string = "Here is the optimizing history:\n"
        for i, optimizing in enumerate(SelfOptimizingCache.optimizing_history):
            string += f"Hypothesis {i}: {optimizing}\n"
        string += "And the result of them is: (The first one is the baseline result)\n"
        for i, result in enumerate(SelfOptimizingCache.result_history):
            string += f"Result {i}: {result}\n"
        return string

    @staticmethod
    def get_result_history():
        string = "Here is the result history:\n"
        for i, result in enumerate(SelfOptimizingCache.result_history):
            string += f"Result {i}: {result}\n"
        return string

    @staticmethod
    def get_tmp_optimizing_history_and_result_history():
        string = SelfOptimizingCache.get_optimizing_history_and_result_history()
        string += "Here is the tmp optimizing history (These optimizes are not applied for now, note that all result is BASED ON BASELINE, not the previous tmp optimizing history):\n"
        for i, optimizing in enumerate(SelfOptimizingCache.tmp_optimizing_history):
            string += f"(From applied optimizing (BASELINE)) Optimizing {i}: {optimizing}\n"
            string += "And the result of it is:\n"
            if i < len(SelfOptimizingCache.tmp_result_history):
                string += f"Result {i}: {SelfOptimizingCache.tmp_result_history[i]}\n"
            else:
                string += f"Result {i}: None\n"
        return string

    @staticmethod
    def save():
        if not os.path.exists(SelfOptimizingCache.cache_dir):
            os.makedirs(SelfOptimizingCache.cache_dir, exist_ok=True)
        if not os.path.exists(SelfOptimizingCache.hypothesis_dir):
            os.makedirs(SelfOptimizingCache.hypothesis_dir, exist_ok=True)
        if not os.path.exists(SelfOptimizingCache.finish_signal_dir):
            os.makedirs(SelfOptimizingCache.finish_signal_dir, exist_ok=True)
        try:
            for i, optimizing in enumerate(SelfOptimizingCache.optimizing_history):
                with open(os.path.join(SelfOptimizingCache.cache_dir, f"optimizing-{i}.md"), "w", encoding='utf-8') as f:
                    f.write(optimizing)
            for i, result in enumerate(SelfOptimizingCache.result_history):
                with open(os.path.join(SelfOptimizingCache.cache_dir, f"result-{i}.md"), "w", encoding='utf-8') as f:
                    f.write(result)
            for i, optimizing in enumerate(SelfOptimizingCache.tmp_optimizing_history):
                with open(os.path.join(SelfOptimizingCache.cache_dir, f"tmp_optimizing-{i}.txt"), "w", encoding='utf-8') as f:
                    f.write(optimizing)
            for i, result in enumerate(SelfOptimizingCache.tmp_result_history):
                with open(os.path.join(SelfOptimizingCache.cache_dir, f"tmp_result-{i}.txt"), "w", encoding='utf-8') as f:
                    f.write(result)
            for i, hypothesis in enumerate(SelfOptimizingCache.hypothesis_history):
                with open(os.path.join(SelfOptimizingCache.hypothesis_dir, f"hypothesis-{i}.md"), "w", encoding='utf-8') as f:
                    f.write(hypothesis)
            for i, hypothesis_extract in enumerate(SelfOptimizingCache.hypothesis_extract_history):
                with open(os.path.join(SelfOptimizingCache.hypothesis_dir, f"hypothesis_extract-{i}.md"), "w", encoding='utf-8') as f:
                    f.write(hypothesis_extract)
            for i, hypothesis_paper_extract in enumerate(SelfOptimizingCache.hypothesis_paper_extract_history):
                with open(os.path.join(SelfOptimizingCache.hypothesis_dir, f"paper_hypothesis_extract-{i}.md"), "w", encoding='utf-8') as f:
                    f.write(hypothesis_paper_extract)
            for i, hypothesis_input in enumerate(SelfOptimizingCache.hypothesis_input_history):
                with open(os.path.join(SelfOptimizingCache.hypothesis_dir, f"hypothesis_input-{i}.md"), "w", encoding='utf-8') as f:
                    f.write(hypothesis_input)
            for i, finish_signal in enumerate(SelfOptimizingCache.finish_signal_history):
                with open(os.path.join(SelfOptimizingCache.finish_signal_dir, f"finish_signal-{i}.md"), "w", encoding='utf-8') as f:
                    f.write(finish_signal)
            for i, hypothesis in enumerate(SelfOptimizingCache.paper_hypothesis_history):
                with open(os.path.join(SelfOptimizingCache.hypothesis_dir, f"paper_hypothesis-{i}.md"), "w", encoding='utf-8') as f:
                    f.write(hypothesis)
            for i, hypothesis in enumerate(SelfOptimizingCache.paper_origin_hypothesis_history):
                with open(os.path.join(SelfOptimizingCache.hypothesis_dir, f"paper_origin_hypothesis-{i}.md"), "w", encoding='utf-8') as f:
                    f.write(hypothesis)
            for i, hypothesis_input in enumerate(SelfOptimizingCache.paper_hypothesis_input_history):
                with open(os.path.join(SelfOptimizingCache.hypothesis_dir, f"paper_hypothesis_input-{i}.md"), "w", encoding='utf-8') as f:
                    f.write(hypothesis_input)
            for i, summary in enumerate(SelfOptimizingCache.optimizing_summary_input_history):
                with open(os.path.join(SelfOptimizingCache.hypothesis_dir, f"optimizing_summary_input-{i}.md"), "w", encoding='utf-8') as f:
                    f.write(summary)
        except Exception as e:
            print(
                f"Error saving initial code cache to {SelfOptimizingCache.cache_dir}: {e}")
            
    @staticmethod
    def get_ongoing_index():
        min_index = 100
        max_index = -1
        if len(SelfOptimizingCache.paper_hypothesis_input_history) > 0:
            min_index = min(min_index, len(SelfOptimizingCache.paper_hypothesis_input_history) - 1)
            max_index = max(max_index, len(SelfOptimizingCache.paper_hypothesis_input_history) - 1)
        else:
            min_index = -1
            
        if len(SelfOptimizingCache.paper_origin_hypothesis_history) > 0:
            min_index = min(min_index, len(SelfOptimizingCache.paper_origin_hypothesis_history) - 1)
            max_index = max(max_index, len(SelfOptimizingCache.paper_origin_hypothesis_history) - 1)
        else:
            min_index = -1
        
        if len(SelfOptimizingCache.hypothesis_paper_extract_history) > 0:
            min_index = min(min_index, len(SelfOptimizingCache.hypothesis_paper_extract_history) - 1)
            max_index = max(max_index, len(SelfOptimizingCache.hypothesis_paper_extract_history) - 1)
        else:
            min_index = -1
            
        if len(SelfOptimizingCache.paper_hypothesis_history) > 0:
            min_index = min(min_index, len(SelfOptimizingCache.paper_hypothesis_history) - 1)
            max_index = max(max_index, len(SelfOptimizingCache.paper_hypothesis_history) - 1)
        else:
            min_index = -1
            
        if len(SelfOptimizingCache.hypothesis_history) > 0:
            min_index = min(min_index, len(SelfOptimizingCache.hypothesis_history) - 1)
            max_index = max(max_index, len(SelfOptimizingCache.hypothesis_history) - 1)
        else:
            min_index = -1

        if min_index == max_index:
            return max_index + 1
        return max_index
        

@static_init
class WorkflowCache:
    workflow = []
    workflow_dir = os.path.join(output_path, "workflow_cache")

    @classmethod
    def static_init(cls):
        if not os.path.exists(cls.workflow_dir):
            os.makedirs(cls.workflow_dir, exist_ok=True)
        try:
            # get all files
            files = os.listdir(cls.workflow_dir)
            # remove directories from the list
            files = [f for f in files if os.path.isfile(os.path.join(cls.workflow_dir, f))]
            # sort file name by the number in the file name
            files = sorted(files, key=lambda x: int(
                re.search(r'(\d+)', x).group(1)))
            for file in files:
                # open the file and get the content
                with open(os.path.join(cls.workflow_dir, file), 'r', encoding='utf-8') as f:
                    content = f.read()
                    cls.workflow.append(content)
        except Exception as e:
            print(f"Error loading workflow cache from {cls.workflow_dir}: {e}")

    @staticmethod
    def push_workflow(workflow):
        WorkflowCache.workflow.append(workflow)
        WorkflowCache.save()

    @staticmethod
    def save():
        if not os.path.exists(WorkflowCache.workflow_dir):
            os.makedirs(WorkflowCache.workflow_dir, exist_ok=True)
        try:
            for i, workflow in enumerate(WorkflowCache.workflow):
                with open(os.path.join(WorkflowCache.workflow_dir, f"workflow-{i}.md"), "w", encoding='utf-8') as f:
                    f.write(workflow)
        except Exception as e:
            print(
                f"Error saving workflow cache to {WorkflowCache.workflow_dir}: {e}")

    @staticmethod
    def parse_latest_workflow():
        parts = []
        current_part = None
        current_step = None
        current_option = None

        workflow = WorkflowCache.workflow[-1] if WorkflowCache.workflow else ""
        lines = workflow.splitlines()

        started = False
        for line in lines:
            # skip lines invalid
            if not line.startswith('Part') and not started:
                continue
            started = True
            # 解析Part
            if line.find('Part ') != -1:
                # print(f"Parsing line for Part: {line.strip()}")
                part_match = re.match(r'Part (\d+): (.+)', line.strip())
                if part_match:
                    current_part = {
                        'number': part_match.group(1),
                        'title': part_match.group(2),
                        'steps': []
                    }
                    parts.append(current_part)
                    current_step = None
                    current_option = None

            # 解析Step
            elif line.find('Step ') != -1:
                step_match = re.match(
                    r'Step (\d+\.\d+): (.+)', line.strip())
                if step_match and current_part:
                    current_step = {
                        'number': step_match.group(1),
                        'description': step_match.group(2),
                        'options': []
                    }
                    current_part['steps'].append(current_step)
                    current_option = None

            # 解析Option
            elif line.find('Option ') != -1:
                option_match = re.match(
                    r'Option (\d+\.\d+\.\d+): (.+)', line.strip())
                if option_match and current_step:
                    # print(f"Found option: {option_match.group(1)} in step {current_step['number']}")
                    current_option = {
                        'number': option_match.group(1),
                        'header': line.strip(),
                        'content': [line]
                    }
                    current_step['options'].append(current_option)

            # 收集Option内容
            elif current_option and line.startswith('        '):
                current_option['content'].append(line)

        return parts

    @staticmethod
    def extract_latest_workflow_options(save=False, max_files=10):
        parts = WorkflowCache.parse_latest_workflow()
        # 收集所有需要选择的Step（有多个Option的）
        selectable_steps = []
        for part in parts:
            for step in part['steps']:
                if len(step['options']) > 0:  # 只处理有Option的Step
                    selectable_steps.append(step)
                    # print(len(step['options']), " options found in step ", step['number'])

        # 生成所有Option组合
        option_combinations = list(itertools.product(
            *[range(len(step['options'])) for step in selectable_steps]
        ))
        
        print(f"Found {len(selectable_steps)} selectable steps with {len(option_combinations)} combinations.")

        workflows = []
        for i, combination in enumerate(option_combinations):
            workflow = ""
            # 写入所有Part和Step结构
            for part in parts:
                workflow += f"Part {part['number']}: {part['title']}\n"

                # 写入该Part下的Step
                for step in part['steps']:
                    # 跳过没有Option的Step
                    if not step['options']:
                        continue

                    workflow += f"    Step {step['number']}: {step['description']}\n"

                    # 查找当前Step在selectable_steps中的位置
                    step_idx = next(
                        (idx for idx, s in enumerate(selectable_steps)
                         if s['number'] == step['number']),
                        None
                    )

                    if step_idx is not None:
                        # 获取当前组合选择的Option
                        option_idx = combination[step_idx]
                        selected_option = step['options'][option_idx]

                        # 写入选择的Option内容
                        for line in selected_option['content']:
                            workflow += f"        {line}\n"

            workflows.append(workflow.strip())
            # break
            if len(workflows) >= max_files:
                print(f"Reached max_files limit of {max_files}, stopping generation.")
                break
            
        print(len(workflows), " workflows generated from the latest workflow.")

        if save:
            # 保存所有生成的workflow到文件
            directory = os.path.join(
                WorkflowCache.workflow_dir, "extract_workflows")
            if not os.path.exists(directory):
                os.makedirs(directory, exist_ok=True)
            # 清空之前的文件
            for file in os.listdir(directory):
                os.remove(os.path.join(directory, file))
            # 写入每个workflow到单独的文件
            for i, workflow in enumerate(workflows):
                with open(os.path.join(directory, f"workflow-output-{i}.md"), "w", encoding='utf-8') as f:
                    f.write(workflow)
        return workflows
    
# --- Initial Code Cache Class ---
@static_init
class InitialCodeCache:
    codes = {}
    lastest_code = ""
    # Keep separate cache dir for initial generation vs corrected code
    cache_dir = os.path.join(output_path, "initial_code_cache")
    cache_file = os.path.join(cache_dir, "initial_code_cache.json")
    lastest_code_file = os.path.join(cache_dir, "last_initial_code.py")

    @classmethod
    def static_init(cls):
        if not os.path.exists(cls.cache_dir):
            os.makedirs(cls.cache_dir, exist_ok=True) # Create dir if needed
        if not os.path.exists(cls.cache_file):
            return
        if not os.path.isfile(cls.lastest_code_file):
            return
        try:
            with open(cls.cache_file, "r", encoding='utf-8') as f:
                cls.codes = json.load(f)
            with open(cls.lastest_code_file, "r", encoding='utf-8') as f:
                cls.lastest_code = f.read().strip()
        except json.JSONDecodeError:
            print(f"Warning: Could not decode JSON from {cls.cache_file}. Initializing empty cache.")
            cls.codes = {}
        except Exception as e:
            print(f"Warning: Error loading initial code cache from {cls.cache_file}: {e}")
            cls.codes = {}

    @staticmethod
    def push_code(request, code):
        InitialCodeCache.codes[request] = code
        InitialCodeCache.lastest_code = code # Update lastest code
        InitialCodeCache.save()
        
    @staticmethod
    def get_code_option(index):
        code = ""
        try:
            with open(os.path.join(InitialCodeCache.cache_dir, f'code_option_{index}.py'), 'r', encoding='utf-8') as f:
                code = f.read().strip()
        except FileNotFoundError:
            print(f"Warning: Code option {index} file not found in {InitialCodeCache.cache_dir}.")
        except Exception as e:
            print(f"Error reading code option {index} file: {e}")
        return code

    @staticmethod
    def get_code(request):
        return InitialCodeCache.codes.get(request) # Use .get for safer access

    @staticmethod
    def save():
        if not os.path.exists(InitialCodeCache.cache_dir):
            os.makedirs(InitialCodeCache.cache_dir, exist_ok=True)
        try:
            with open(InitialCodeCache.cache_file, "w", encoding='utf-8') as f:
                json.dump(InitialCodeCache.codes, f, indent=4) # Add indent for readability
            with open(InitialCodeCache.lastest_code_file, "w", encoding='utf-8') as f:
                f.write(InitialCodeCache.lastest_code.strip())
        except Exception as e:
            print(f"Error saving initial code cache to {cls.cache_file}: {e}")




def getLatestExperimentWorkflowByCache() -> str:
    try:
        return WorkflowCache.workflow[-1]
    except Exception as e:
        print(f"Error getting latest experiment workflow by cache: {e}")
    return ""


def getLatestExperimentWorkflowByAnswers() -> str:
    # with open(os.path.join(output_path, 'answers','last_workflow.md'), 'r', encoding='utf-8') as f:
    #     content = f.read()
    #     return content
    # list all files in the output directory
    # read the file and get the content
    with open(os.path.join(output_path + "/answers", "last_workflow.md"), 'r') as f:
        content = f.read()
    # find the intput of the file
        return content
    return ""


def to_numeric(string_value):
    """
    Converts a string to an integer or float.
    Handles strings representing percentages (e.g., '75%').
    Returns the original string if conversion is not possible.
    """
    if isinstance(string_value, str):
        # Handle percentage strings
        if '%' in string_value:
            return float(string_value.strip('%')) / 100

        # Attempt to convert to an integer
        try:
            return int(string_value)
        except ValueError:
            # If int conversion fails, try float
            try:
                return float(string_value)
            except ValueError:
                return string_value # Return original if all conversions fail
    # Return the value if it's already a number
    return string_value


# def getLatestOptimizingAdvices() -> list:
#     # list all files in the output directory
#     files = os.listdir(output_path + "/ai_scientist")
#     # get all files that contains "coder-input" in the name
#     files = [f for f in files if "advise" in f and 'json' in f]
#     # file is names as 'stepxxx-tool-coder-input.json', find lastest one
#     files = sorted(files, key=lambda x: int(
#         re.search(r'step(\d+)', x).group(1)))
#     # get the lastest file
#     latest_file = files[-1] if files else None
#     # read the file and get the content
#     if latest_file:
#         with open(os.path.join(output_path + "/ai_scientist", latest_file), 'r') as f:
#             content = f.read()
#         # find the intput of the file
#         # match the content with the pattern 1. 2. 3.
#         pattern = r'\d+\.\s+'
#         matches = re.findall(pattern, content)

# the Cache for code and its corresponding workflow
@static_init
class CoflowCache:
    batch_count = 0
    coflow_dir = os.path.join(output_path, "coflow_cache")

    @classmethod
    def static_init(cls):
        if not os.path.exists(cls.coflow_dir):
            os.makedirs(cls.coflow_dir, exist_ok=True)
        try:
            # get all files
            files = os.listdir(cls.coflow_dir)
            # sort file name by the number in the file name
            files = sorted(files, key=lambda x: int(
                re.search(r'(\d+)', x).group(1)))
            cls.batch_count = len(files)
        except Exception as e:
            print(f"Error loading coflow cache from {cls.coflow_dir}: {e}")
    
    @classmethod
    def save_batch(cls, workflow_list, code_list):
        # first, check if their length is the same
        if len(workflow_list) != len(code_list):
            raise ValueError("Workflow and code lists must have the same length.")

        # The file is named code-{batch_count}-{i}.py for code and workflow-{batch_count}-{i}.md for workflow
        for i, (workflow, code) in enumerate(zip(workflow_list, code_list)):
            with open(os.path.join(CoflowCache.coflow_dir, f"workflow-{CoflowCache.batch_count}-{i}.md"), "w", encoding='utf-8') as f:
                f.write(workflow)
            with open(os.path.join(CoflowCache.coflow_dir, f"code-{CoflowCache.batch_count}-{i}.py"), "w", encoding='utf-8') as f:
                f.write(code)

        CoflowCache.batch_count += 1

    @classmethod
    def get_latest_batch_count(cls) -> int:
    # the latest batch number. For example, if the largest file is code-2-3.py, then the latest batch number is 2.
        files = os.listdir(cls.coflow_dir)
        ans = 0
        for filename in files:
            match = re.match(r'code-(\d+)-\d+\.py', filename)
            if match:
                batch_num = int(match.group(1))
                ans = max(ans, batch_num)
        return ans

    @classmethod
    def get_latest_options(cls) -> int:
        files = os.listdir(cls.coflow_dir)
        # example filename code-0-0.py, code-0-1.py, code-1-0.py, code-1-1.py
        
        batch_groups = {}
        for filename in files:
            match = re.match(r'code-(\d+)-\d+\.py', filename)
            if match:
                batch_num = int(match.group(1))
                batch_groups[batch_num] = batch_groups.get(batch_num, 0) + 1
        
        if not batch_groups:
            raise ValueError("No valid coflow files found.")
        
        latest_batch_num = max(batch_groups.keys())
        return batch_groups[latest_batch_num]
        
    @classmethod
    def get_latest_workflow(cls, choice = 0) -> str:
        batch_num = cls.get_latest_batch_count()
        filename = f"workflow-{batch_num}-{choice}.md"
        if not os.path.exists(os.path.join(cls.coflow_dir, filename)):
            raise ValueError(f"Workflow file {filename} not found.")
        with open(os.path.join(cls.coflow_dir, filename), 'r', encoding='utf-8') as f:
            content = f.read()
        return content

    @classmethod
    def get_cache_dir(cls) -> str:
        return cls.coflow_dir
    
def get_inventory():
    # Todo: get things from the real database
    # return a json
    if settings.rpa:
        # temp method
        # load json from file
        with open(os.path.join(reagent_path, "reagent_RPA.json"), "r", encoding='utf-8') as f:
            inventory = json.load(f)
        return inventory

    if settings.rna:
        with open(os.path.join(reagent_path, "reagent_RNA.json"), "r", encoding='utf-8') as f:
            inventory = json.load(f)
        return inventory
    
    if settings.storage:
        with open(os.path.join(reagent_path, "reagent_storage.json"), "r", encoding='utf-8') as f:
            inventory = json.load(f)
        return inventory
    if settings.amplification:
        with open(os.path.join(reagent_path, "reagent_amplification.json"), "r", encoding='utf-8') as f:
            inventory = json.load(f)
        return inventory
    if settings.write:
        with open(os.path.join(reagent_path, "reagent_write.json"), "r", encoding='utf-8') as f:
            inventory = json.load(f)
        return inventory
    if settings.read:
        with open(os.path.join(reagent_path, "reagent_read.json"), "r", encoding='utf-8') as f:
            inventory = json.load(f)
        return inventory
    if settings.polya:
        with open(os.path.join(reagent_path, "reagent_polyA.json"), "r", encoding='utf-8') as f:
            inventory = json.load(f)
        return inventory
    # Default return synthesis
    with open(os.path.join(reagent_path, "reagent_inventory.json"), "r", encoding='utf-8') as f:
        inventory = json.load(f)
    return inventory

def format_reagents_json(reagents_json, no_doc=False, no_notes=False):
    formatted_lines = []

    for reagent in reagents_json:
        # Handle names and aliases
        names = [reagent["name"]] + reagent.get("aliases", [])
        name_str = " or ".join(f'"{name}"' for name in names)
        
        # Basic properties
        line = f"{name_str}: "
        
        # Add concentration if exists
        if "concentration" in reagent.get("properties", {}):
            conc = reagent["properties"]["concentration"]
            line += f"{conc['value']}{conc['unit']}, "
        
        # Add pH if exists
        if "ph" in reagent.get("properties", {}):
            line += f"pH {reagent['properties']['ph']}, "
        
        # Add form information
        line += f"{reagent['form'].lower()}, "
        
        # Add components if buffer
        if "components" in reagent:
            components = []
            for comp in reagent["components"]:
                if isinstance(comp, dict):
                    # Check if 'concentration' exists before accessing it
                    if 'concentration' in comp:
                        components.append(f"{comp['name']} ({comp['concentration']})")
                    else:
                        components.append(comp['name'])
                else:
                    components.append(comp)
            line += f"components: {', '.join(components)}, "
        
        # Add documentation (conditionally)
        if not no_doc and "documentation" in reagent:
            line += f"doc: {', '.join(reagent['documentation'])}, "
        
        # Add notes (conditionally)
        if not no_notes and "notes" in reagent:
            line += f"notes: {reagent['notes']}, "
        
        # Clean up trailing comma
        line = line.rstrip(", ")
        formatted_lines.append(line)
    
    return "\n".join(formatted_lines)

def remove_empty_lines(text: str) -> str:
    """
    Removes empty lines from the given text.

    Args:
        text: The input text from which to remove empty lines.

    Returns:
        The text with all empty lines removed.
    """
    # remove the lines that has spaces or newline characters only
    lines = text.splitlines()
    lines = [line for line in lines if line.strip()]
    return "\n".join(lines)


def extract_all_from_pdfs(file_paths: List[str], extract_tables: bool = True) -> str:
    """
    Extracts and concatenates text and tables from a list of PDF files.

    Args:
        file_paths: A list of string paths to the PDF files.
        extract_tables: Whether to extract tables separately (default: True).

    Returns:
        A single string containing all the extracted text and tables, with
        separators between documents.
    """
    full_text = ""
    
    for path in file_paths:
        full_text += f"\n\n--- START OF DOCUMENT: {path} ---\n\n"
        
        try:
            with pdfplumber.open(path) as pdf:
                for page_num, page in enumerate(pdf.pages, 1):
                    full_text += f"\n--- Page {page_num} ---\n"
                    
                    # Extract tables if requested
                    if extract_tables:
                        tables = page.extract_tables()
                        if tables:
                            for table_num, table in enumerate(tables, 1):
                                full_text += f"\n[TABLE {table_num}]\n"
                                # Convert table to pandas DataFrame for better formatting
                                try:
                                    df = pd.DataFrame(table[1:], columns=table[0] if table else None)
                                    # Clean up None values
                                    df = df.fillna('')
                                    full_text += df.to_string(index=False) + "\n"
                                except:
                                    # Fallback: print table as-is if DataFrame conversion fails
                                    for row in table:
                                        full_text += " | ".join(str(cell) if cell else '' for cell in row) + "\n"
                                full_text += "[END TABLE]\n\n"
                    
                    # Extract regular text
                    extracted_text = page.extract_text()
                    if extracted_text:
                        # Remove empty lines if you have that function
                        extracted_text = remove_empty_lines(extracted_text)
                        full_text += extracted_text + "\n"
                        
        except Exception as e:
            full_text += f"\n[ERROR processing {path}: {str(e)}]\n"
            
        full_text += f"\n\n--- END OF DOCUMENT: {path} ---\n\n"

    return full_text

def extract_text_from_pdfs(file_paths: List[str]) -> str:
    """
    Extracts and concatenates text from a list of PDF files.

    Args:
        file_paths: A list of string paths to the PDF files.

    Returns:
        A single string containing all the extracted text, with
        separators between documents.
    """
    full_text = ""
    for path in file_paths:
        full_text += f"\n\n--- START OF DOCUMENT: {path} ---\n\n"
        reader = PdfReader(path)
        for page in reader.pages:
            # Add text from the page if any exists
            extracted_text = page.extract_text()
            if extracted_text:
                extracted_text = remove_empty_lines(extracted_text)
                full_text += extracted_text + "\n"
        full_text += f"\n\n--- END OF DOCUMENT: {path} ---\n\n"

    return full_text

def get_current_user_prompt() -> str:
    # check this first 
    current_prompt = file_manager.load_from_cache(settings.CURRENT_USER_PROMPT_FILE)
    if current_prompt:
        return current_prompt

    # default to be enzymatic de novo synthesis prompt
    user_prompt_path = os.path.join(prompt_path, "user_prompt_full.md")
    if settings.rpa:
        user_prompt_path = os.path.join(prompt_path, "user_prompt_rpa.md")
    if settings.test:
        user_prompt_path = os.path.join(prompt_path, "user_prompt_test.md")
    if settings.amplification:
        user_prompt_path = os.path.join(prompt_path, "user_prompt_amplification.md")
    with open(user_prompt_path, 'r', encoding='utf-8') as f:
        return f.read()
    
def get_current_user_requirement() -> str:
    current_requirement = file_manager.load_from_cache(settings.CURRENT_USER_REQUIREMENT_FILE)
    if current_requirement:
        return current_requirement

    return ""