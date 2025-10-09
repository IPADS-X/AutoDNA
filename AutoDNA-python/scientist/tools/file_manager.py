import os
import shelve
import json
import re
import secrets
from loguru import logger
from datetime import datetime
from config import shared_files_path, output_path
import atexit
from typing import Union, Dict, List

class FileManager:
    _instance = None

    def __new__(cls, db_path=os.path.join(shared_files_path, 'file_manager_db')):
        if cls._instance is None:
            cls._instance = super(FileManager, cls).__new__(cls)
            cls._instance._initialized = False
        return cls._instance

    def __init__(self, db_path=os.path.join(shared_files_path, 'file_manager_db')):
        if self._initialized:
            return
        
        if not os.path.exists(shared_files_path):
            os.makedirs(shared_files_path)
        
        db_dir = os.path.dirname(db_path)
        self.cache_path = os.path.join(db_dir, 'process_cache')
        os.makedirs(self.cache_path, exist_ok=True)

        self.input_history_path = os.path.join(db_dir, 'agent_input_history.json')
        self.records_path = os.path.join(db_dir, 'agent_records')
        os.makedirs(self.records_path, exist_ok=True)
        self.mappings_path = os.path.join(db_dir, 'file_mappings.json')
        self.llm_logs_path = os.path.join(db_dir, 'llm_input_logs')
        os.makedirs(self.llm_logs_path, exist_ok=True)
        
        self.db = shelve.open(db_path, writeback=True)
        
        self._initialized = True
        atexit.register(self.close)

    def close(self):
        """Closes the database connection."""
        self.db.close()
        logger.info("FileManager database connection closed.")

    def _generate_unique_id(self, agent_name: str) -> str:
        """Generates a unique ID with a random 4-digit hexadecimal suffix."""
        while True:
            random_suffix = secrets.token_hex(2)
            file_id = f"{agent_name}-{random_suffix}"
            if file_id not in self.db:
                return file_id

    def _get_next_log_num(self, prefix: str) -> int:
        """Finds the highest number used in a log file for a given prefix and returns the next one."""
        highest_num = -1
        pattern = re.compile(rf"^{re.escape(prefix)}_input_(\d+)\.md$")
        try:
            for filename in os.listdir(self.llm_logs_path):
                match = pattern.match(filename)
                if match:
                    num = int(match.group(1))
                    if num > highest_num:
                        highest_num = num
        except FileNotFoundError:
            logger.warning(f"Log directory not found: {self.llm_logs_path}. Will create and start numbering from 0.")
            os.makedirs(self.llm_logs_path, exist_ok=True)
        return highest_num + 1
    
    def add_file(self, agent_name: str, content: str, original_filename: str = "output.txt", description: str = "", save: bool = True) -> str:
        """Adds a file to the database."""
        file_id = self._generate_unique_id(agent_name)
        
        self.db[file_id] = {
            'content': content,
            'agent_name': agent_name,
            'original_filename': original_filename,
            'description': description
        }
        
        logger.info(f"Data saved to KV store with ID: {file_id}")

        if save:
            output_file_path = os.path.join(output_path, "agent_latest_output.md")
            with open(output_file_path, 'w', encoding='utf-8') as f:
                f.write(content)
        return file_id

    def add_file_by_original_name(self, content: str, original_filename: str, description: str = "") -> str | None:
        """Adds a file using its original name as ID."""
        file_id, _ = os.path.splitext(original_filename)

        if file_id in self.db:
            logger.error(f"Error: A file with ID '{file_id}' already exists. Operation cancelled.")
            return None

        agent_name = file_id.split('-', 1)[0]
        
        self.db[file_id] = {
            'content': content,
            'agent_name': agent_name,
            'original_filename': original_filename,
            'description': description
        }
        
        logger.info(f"Data saved to KV store with ID: {file_id} (Agent: {agent_name})")
        return file_id
    
    def add_batch_files(self, agent_name: str, content_list: List[str], description: str = "") -> List[str]:
        """Adds a batch of files, each with a unique ID."""
        if not content_list:
            return []

        if len(content_list) == 1:
            file_id = self.add_file(
                agent_name=agent_name,
                content=content_list[0],
                description=description
            )
            return [file_id]

        new_file_ids = []
        batch_id = secrets.token_hex(2)
        logger.info(f"Processing batch '{batch_id}' for agent '{agent_name}'.")

        for i, content in enumerate(content_list):
            file_id = self._generate_unique_id(agent_name)
            
            self.db[file_id] = {
                'content': content,
                'agent_name': agent_name,
                'original_filename': f"batch_{batch_id}_file_{i}.txt",
                'description': description
            }
            new_file_ids.append(file_id)

        logger.info(f"Saved a batch of {len(new_file_ids)} files for agent '{agent_name}'.")
        return new_file_ids
    
    def get_file_content(self, file_id: str) -> str | None:
        """Retrieves the content of a file from the database using its ID."""
        if file_id in self.db:
            return self.db[file_id]['content']

        logger.error(f"Error: File ID '{file_id}' not found in the database.")
        return None
    
    def get_latest_files(self, count: int) -> List[str]:
        """
        Gets the names of the most recent files.
        Note: Without timestamps, 'latest' is determined by reverse alphanumeric sorting of file IDs.
        """
        if count <= 0:
            return []
        try:
            sorted_ids = sorted(list(self.db.keys()), reverse=True)
        except Exception as e:
            logger.error(f"Error sorting file IDs: {e}.")
            return []

        return sorted_ids[:count]
    
    def remove_file(self, file_id: str) -> bool:
        """Removes a single record from the database."""
        if file_id in self.db:
            del self.db[file_id]
            logger.info(f"Record removed from database for file ID: {file_id}")
            return True
        else:
            logger.error(f"Error: File ID '{file_id}' not found in database. Cannot remove.")
            return False

    def modify_file(self, file_id: str, new_content: str) -> bool:
        """Modifies the content of an existing file."""
        if file_id in self.db:
            self.db[file_id]['content'] = new_content
            logger.info(f"File ID '{file_id}' has been successfully modified.")
            return True
        else:
            logger.error(f"Error: File ID '{file_id}' not found. Cannot modify.")
            return False

    def log_agent_inputs(self, agent_name: str, inputs: Union[str, List[str]]):
        """Appends a new round of input IDs to an agent's history."""
        history = {}
        if os.path.exists(self.input_history_path):
            with open(self.input_history_path, 'r', encoding='utf-8') as f:
                try:
                    history = json.load(f)
                except json.JSONDecodeError:
                    history = {}
        
        round_to_log = [inputs] if isinstance(inputs, str) else inputs
        
        agent_history_rounds = history.get(agent_name, [])
        agent_history_rounds.append(round_to_log)
        history[agent_name] = agent_history_rounds
        with open(self.input_history_path, 'w', encoding='utf-8') as f:
            json.dump(history, f, indent=4)
        logger.info(f"Logged new input round for agent '{agent_name}'.")
        
    def clear_agent_inputs(self, agent_name: str) -> bool:
        """Clears all input history for a specific agent."""
        history = {}
        if os.path.exists(self.input_history_path):
            with open(self.input_history_path, 'r', encoding='utf-8') as f:
                try:
                    history = json.load(f)
                except json.JSONDecodeError:
                    logger.warning("Could not parse input history file. Creating new one.")
                    history = {}
        
        if agent_name in history:
            del history[agent_name]
            with open(self.input_history_path, 'w', encoding='utf-8') as f:
                json.dump(history, f, indent=4)
            logger.info(f"Cleared all input history for agent '{agent_name}'.")
            return True
        else:
            logger.warning(f"No input history found for agent '{agent_name}'.")
            return False
            
    def _read_input_history_file(self) -> dict:
        """Private helper to safely read and parse the history JSON."""
        if not os.path.exists(self.input_history_path):
            return {}
        with open(self.input_history_path, 'r', encoding='utf-8') as f:
            try:
                return json.load(f)
            except json.JSONDecodeError:
                return {}

    def get_agent_input_history(self, agent_name: str) -> List[str]:
        """Reads the history file and returns a flat list of all unique IDs ever passed to an agent."""
        history = self._read_input_history_file()
        all_rounds = history.get(agent_name, [])
        
        if not all_rounds:
            return []
            
        flat_list = [item for round_list in all_rounds for item in round_list]
        return list(dict.fromkeys(flat_list))
    
    def clear_all_agent_input_history(self) -> bool:
        """Clears the entire input history file."""
        if os.path.exists(self.input_history_path):
            try:
                os.remove(self.input_history_path)
                logger.info("Cleared the entire agent input history.")
                return True
            except OSError as e:
                logger.error(f"Error deleting input history file: {e}")
                return False
        else:
            return True

    def get_last_agent_inputs(self, agent_name: str) -> List[str]:
        """Gets only the most recent round of inputs for an agent."""
        history = self._read_input_history_file()
        all_rounds = history.get(agent_name, [])
        return all_rounds[-1] if all_rounds else []
    
    # --- Staged and Cached Processes ---
    def run_staged_process(self, process_name: str, stage1_func: callable, stage2_func: callable):
        """Runs a generic two-stage process that is restartable, using a normal file for its cache."""
        cache_file_path = os.path.join(self.cache_path, f"{process_name}.cache")
        intermediate_result = None
        
        if os.path.exists(cache_file_path):
            logger.debug(f"FileManager: Found cache file '{cache_file_path}'. Resuming at Stage 2.")
            with open(cache_file_path, 'r', encoding='utf-8') as f:
                intermediate_result = f.read()
        else:
            logger.info(f"FileManager: No cache found for '{process_name}'. Starting Stage 1.")
            intermediate_result = stage1_func()
            
            with open(cache_file_path, 'w', encoding='utf-8') as f:
                f.write(intermediate_result)
            logger.success(f"FileManager: Stage 1 complete. Saved checkpoint to '{cache_file_path}'.")

        # input("Preparing to start Stage 2. Press Enter to continue...")
        logger.info(f"FileManager: Starting Stage 2 for '{process_name}'.")
        final_result = stage2_func(intermediate_result)
        logger.success(f"FileManager: Stage 2 complete.")
        return final_result
    
    def run_multi_stage_process(self, process_name: str, stage_functions: List[callable]):
        """Runs a generic, multi-stage process that is restartable."""
        if not stage_functions:
            logger.error("Error: No stage functions provided.")
            return None

        num_stages = len(stage_functions)
        last_output = None
        start_stage = 0

        for i in range(num_stages - 1, -1, -1):
            cache_file_path = os.path.join(self.cache_path, f"{process_name}_{i}.cache")
            if os.path.exists(cache_file_path):
                logger.debug(f"FileManager: Found cache for completed stage {i}. Resuming from stage {i+1}.")
                with open(cache_file_path, 'r', encoding='utf-8') as f:
                    last_output = f.read()
                start_stage = i + 1
                break
        
        if start_stage == 0:
            logger.info(f"FileManager: No cache found for '{process_name}'. Starting fresh from stage 0.")

        for i in range(start_stage, num_stages):
            current_func = stage_functions[i]
            cache_file_path = os.path.join(self.cache_path, f"{process_name}_{i}.cache")
            logger.info(f"--- Running Stage {i} for '{process_name}' ---")
            current_output = current_func() if i == 0 else current_func(last_output)
            
            with open(cache_file_path, 'w', encoding='utf-8') as f:
                f.write(current_output)
            logger.success(f"--- Stage {i} complete. Checkpoint saved to '{cache_file_path}' ---")
            last_output = current_output
        return last_output
    
    # --- Mappings, Records, and Generic Caching ---
    def add_file_mapping(self, key: str, value: str):
        """Creates or updates a persistent mapping from one file ID (key) to another (value)."""
        mappings = {}
        if os.path.exists(self.mappings_path):
            with open(self.mappings_path, 'r', encoding='utf-8') as f:
                try:
                    mappings = json.load(f)
                except json.JSONDecodeError:
                    mappings = {}
        if key in mappings:
            logger.warning(f"Key '{key}' already exists. Overwriting old value '{mappings[key]}' with new value '{value}'.")
        mappings[key] = value
        with open(self.mappings_path, 'w', encoding='utf-8') as f:
            json.dump(mappings, f, indent=4)
        logger.info(f"Successfully mapped '{key}' -> '{value}'.")

    def get_file_mapping(self, key: str) -> str | None:
        """Retrieves a mapped file ID using its key. Returns None if not found."""
        if not os.path.exists(self.mappings_path):
            return None
        with open(self.mappings_path, 'r', encoding='utf-8') as f:
            try:
                mappings = json.load(f)
                return mappings.get(key)
            except json.JSONDecodeError:
                return None

    def record_agent_data(self, agent_name: str, data: Dict) -> bool:
        """Saves or overwrites a dictionary of data for an agent to a user-editable JSON file."""
        record_file_path = os.path.join(self.records_path, f"{agent_name}_record.json")
        try:
            with open(record_file_path, 'w', encoding='utf-8') as f:
                json.dump(data, f, indent=4)
            logger.info(f"Successfully recorded data for agent '{agent_name}' to {record_file_path}")
            return True
        except Exception as e:
            logger.error(f"An unexpected error occurred while writing agent record for '{agent_name}': {e}")
            return False

    def get_agent_data(self, agent_name: str) -> Union[Dict, bool, None]:
        """Retrieves the recorded data for an agent from its JSON file."""
        record_file_path = os.path.join(self.records_path, f"{agent_name}_record.json")
        if not os.path.exists(record_file_path):
            logger.warning(f"No record file found for agent '{agent_name}' at {record_file_path}")
            return False
        try:
            with open(record_file_path, 'r', encoding='utf-8') as f:
                return json.load(f)
        except Exception as e:
            logger.error(f"An unexpected error occurred while reading agent record for '{agent_name}': {e}")
            return None
            
    def save_to_cache(self, cache_name: str, content: str, folder_path: str = None) -> bool:
        """Saves raw string content to a named file in the cache directory."""
        base_path = folder_path if folder_path is not None else self.cache_path
        os.makedirs(base_path, exist_ok=True)
        cache_file_path = os.path.join(base_path, cache_name)
        try:
            with open(cache_file_path, 'w', encoding='utf-8') as f:
                f.write(content)
            logger.info(f"Successfully saved cache to '{cache_file_path}'")
            return True
        except IOError as e:
            logger.error(f"Failed to save cache to '{cache_file_path}': {e}")
            return False

    def log_LLM_input(self, prefix: str, prompt: str) -> str:
        """Logs the LLM input prompt to a uniquely numbered file."""
        next_num = self._get_next_log_num(prefix)
        filename = f"{prefix}_input_{next_num}.md"
        log_file_path = os.path.join(self.llm_logs_path, filename)
        try:
            with open(log_file_path, 'w', encoding='utf-8') as f:
                f.write(prompt)
            logger.info(f"LLM input logged successfully to '{log_file_path}'")
            return log_file_path
        except IOError as e:
            logger.error(f"Failed to log LLM input to '{log_file_path}': {e}")
            return ""
        
    def load_from_cache(self, cache_name: str, folder_path: str = None) -> str | None:
        """Loads raw string content from a named file in the cache directory."""
        base_path = folder_path if folder_path is not None else self.cache_path
        cache_file_path = os.path.join(base_path, cache_name)
        if not os.path.exists(cache_file_path):
            return None
        try:
            with open(cache_file_path, 'r', encoding='utf-8') as f:
                content = f.read()
            logger.info(f"Successfully loaded cache from '{cache_file_path}'")
            return content
        except IOError as e:
            logger.warning(f"Could not load cache from '{cache_file_path}': {e}")
            return None
        
    def save_to_cache_kv(self, cache_name: str, key: str, value: str) -> bool:
        """
        Saves or updates a key-value pair in a named JSON file in the cache directory.
        If the file exists, it's updated. If not, it's created.
        """
        cache_file_path = os.path.join(self.cache_path, cache_name)
        
        current_data = {}
        # First, try to read existing data if the file already exists.
        if os.path.exists(cache_file_path):
            try:
                with open(cache_file_path, 'r', encoding='utf-8') as f:
                    # Handle empty file case
                    content = f.read()
                    if content:
                        current_data = json.loads(content)
            except (IOError, json.JSONDecodeError) as e:
                logger.error(f"Could not read or parse existing cache file '{cache_file_path}', it will be overwritten. Error: {e}")
                # If the file is corrupted, we'll just overwrite it.
                current_data = {}

        # Now, update the dictionary with the new key-value pair.
        current_data[key] = value
        try:
            # Write the entire, updated dictionary back to the file.
            with open(cache_file_path, 'w', encoding='utf-8') as f:
                json.dump(current_data, f, indent=4)
            logger.info(f"Successfully saved/updated key '{key}' in '{cache_file_path}'")
            return True
        except IOError as e:
            logger.error(f"Failed to save key-value cache to '{cache_file_path}': {e}")
            return False

    def load_from_cache_kv(self, cache_name: str, key: str) -> str | None:
        """Loads a value from a named JSON cache file if the provided key matches."""
        cache_file_path = os.path.join(self.cache_path, cache_name)
        if not os.path.exists(cache_file_path):
            return None
        try:
            with open(cache_file_path, 'r', encoding='utf-8') as f:
                data = json.load(f)
            if key in data:
                logger.info(f"Cache hit for key '{key}' in '{cache_file_path}'.")
                return data[key]
            else:
                logger.warning(f"Cache miss: key '{key}' not found in '{cache_file_path}'.")
                return None
        except (IOError, json.JSONDecodeError) as e:
            logger.warning(f"Could not load or parse key-value cache from '{cache_file_path}': {e}")
            return None
        
    def clear_all_files(self) -> bool:
        """Clears all files from the database after user confirmation."""
        print("\n" + "="*50)
        print("⚠️ WARNING: You are about to delete all files in the database.")
        print("This action is irreversible.")
        print("="*50 + "\n")
        
        confirmation = input("Press Enter to proceed with deleting all files, or type 'cancel' to abort: ")
        
        if confirmation.lower() == 'cancel':
            logger.info("Database clear operation cancelled by user.")
            return False

        try:
            num_files = len(self.db)
            self.db.clear()
            logger.success(f"Successfully cleared all {num_files} files from the database.")
            return True
        except Exception as e:
            logger.error(f"An unexpected error occurred while clearing the database: {e}")
            return False

# Singleton instance
file_manager = FileManager()