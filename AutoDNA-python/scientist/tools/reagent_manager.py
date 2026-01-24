# reagent_manager.py
import shelve
import os
import atexit
import json
from typing import Dict, List, Any, Optional
from config import reagent_db_path

class ReagentManager:
    """Manages all interactions with the reagent shelve database."""
    _instance = None

    def __new__(cls, db_path: str = os.path.join(reagent_db_path, "reagents.db")):
        if cls._instance is None:
            cls._instance = super(ReagentManager, cls).__new__(cls)
            cls._instance._initialized = False
        return cls._instance

    def __init__(self, db_path: str = os.path.join(reagent_db_path, "reagents_db")):
        self.json_sync_path = os.path.join(reagent_db_path, 'reagent_inventory.json')
        if self._initialized:
            return
        
        if not os.path.exists(reagent_db_path):
            os.makedirs(reagent_db_path)
        
        self.db = shelve.open(db_path, writeback=True)

        # mandatory initialization from json
        self._initialize_from_json()

        self._alias_map = self._build_alias_map()
        self._initialized = True
        atexit.register(self.close)

    def _generate_id(self) -> int:
        """Finds the highest existing integer ID and returns the next one."""
        max_id = 0
        for key in self.db.keys():
            try:
                # Check if the key is a number
                current_id = int(key)
                if current_id > max_id:
                    max_id = current_id
            except ValueError:
                # Ignore keys that are not integers (like old name-based IDs)
                continue
        return max_id + 1

    def add_reagent(self, reagent_data: Dict[str, Any]) -> Optional[int]:
        """
        Adds a new reagent, auto-generating a new numeric ID.
        Returns the new ID on success, None on failure.
        """
        if 'name' not in reagent_data:
            print("Error: Reagent 'name' is a required field.")
            return None
            
        # Generate a new unique numeric ID
        new_id = self._generate_id()
        reagent_data['id'] = new_id
        
        # Shelve keys must be strings, so we convert the number to a string for the key
        self.db[str(new_id)] = reagent_data
        
        self._alias_map = self._build_alias_map()
        self._sync_to_json()
        print(f"Successfully added reagent '{reagent_data['name']}' with new ID: {new_id}")
        return new_id
    
    def _sync_to_json(self):
        """Writes the entire current database content to the JSON sync file."""
        all_reagents = self.get_all_reagents()
        with open(self.json_sync_path, 'w', encoding='utf-8') as f:
            json.dump(all_reagents, f, indent=4)
        print("Database synced to reagent_inventory.json")

    def _initialize_from_json(self):
        """Populates the shelve database from the JSON file."""
        if not os.path.exists(self.json_sync_path):
            print("Warning: reagent_inventory.json not found. Database will start empty.")
            return

        try:
            with open(self.json_sync_path, 'r', encoding='utf-8') as f:
                reagents_data = json.load(f)
            
            for reagent in reagents_data:
                reagent_id = reagent.get('id')
                if reagent_id:
                    self.db[str(reagent_id)] = reagent
            print(f"Initialized database from {self.json_sync_path}")
        except (json.JSONDecodeError, FileNotFoundError) as e:
            print(f"Error initializing from JSON: {e}. Database will start empty.")

    def _build_alias_map(self) -> Dict[str, str]:
        """Builds a fast lookup map from any name/alias to its primary ID."""
        alias_map = {}
        for reagent_id, data in self.db.items():
            alias_map[data['name'].lower()] = reagent_id
            for alias in data.get('aliases', []):
                alias_map[alias.lower()] = reagent_id
        return alias_map

    def close(self):
        """Closes the database connection safely on exit."""
        self.db.close()
        print("ReagentDB closed.")

    def find_reagent(self, name_or_alias: str) -> Optional[Dict[str, Any]]:
        """Finds a reagent by its name or alias."""
        reagent_id = self._alias_map.get(name_or_alias.lower())
        return self.db.get(reagent_id) if reagent_id else None

    def get_reagent_by_id(self, reagent_id: str) -> Optional[Dict[str, Any]]:
        """Retrieves a reagent directly by its unique ID."""
        return self.db.get(reagent_id)

    def get_all_reagents(self) -> List[Dict[str, Any]]:
        """Returns a list of all reagents, sorted by name."""
        return sorted(list(self.db.values()), key=lambda x: x['name'])

    def add_reagent(self, reagent_data: Dict[str, Any]) -> bool:
        """Adds a new reagent to the database. Fails if ID exists."""
        reagent_id = reagent_data.get('id')
        if not reagent_id or reagent_id in self.db:
            print(f"Error: Reagent ID '{reagent_id}' is missing or already exists.")
            return False
        self.db[reagent_id] = reagent_data
        self._alias_map = self._build_alias_map()
        print(f"Successfully added reagent: {reagent_id}")
        return True

    def update_reagent(self, reagent_id: str, data: Dict[str, Any]) -> bool:
        """Updates an existing reagent record."""
        if reagent_id not in self.db:
            print(f"Error: Reagent ID '{reagent_id}' not found.")
            return False
        self.db[reagent_id] = data
        self._alias_map = self._build_alias_map()
        print(f"Successfully updated reagent: {reagent_id}")
        return True

    def delete_reagent(self, reagent_id: str) -> bool:
        """Deletes a reagent from the database."""
        if reagent_id in self.db:
            del self.db[reagent_id]
            self._alias_map = self._build_alias_map()
            print(f"Successfully deleted reagent: {reagent_id}")
            return True
        print(f"Error: Reagent ID '{reagent_id}' not found.")
        return False

# Singleton instance for the application to import
reagent_manager = ReagentManager()
