import argparse
import json
import csv
from tools.reagent_manager import reagent_manager

def _prompt_for_reagent_data(existing_data=None):
    """Interactively prompts the user for reagent information."""
    if existing_data is None:
        existing_data = {}
    
    print("Please enter the reagent details. Press Enter to accept the default value shown in [brackets].")
    
    # ID is now auto-generated, so we don't ask for it.
    # We only show it if we are updating an existing entry.
    if 'id' in existing_data:
        print(f"Updating Reagent with fixed ID: {existing_data['id']}")

    def get_input(prompt_text, current_value):
        user_input = input(f"{prompt_text} [{current_value}]: ")
        return user_input if user_input else current_value

    data = {
        "name": get_input("Name (required)", existing_data.get("name", "")),
        "aliases": get_input("Aliases (pipe-separated)", "|".join(existing_data.get("aliases", []))),
        # ... (rest of the prompts are the same) ...
        "category": get_input("Category (Buffer or Reagent)", existing_data.get("category", "Reagent")),
        "status": get_input("Status", existing_data.get("status", "Available")),
        "form": get_input("Form (Solid, Solution or Suspension)", existing_data.get("form", "Solution")),
        "concentration_value": get_input("Concentration Value", existing_data.get("properties", {}).get("concentration", {}).get("value", "")),
        "concentration_unit": get_input("Concentration Unit", existing_data.get("properties", {}).get("concentration", {}).get("unit", "")),
        "ph": get_input("pH", existing_data.get("properties", {}).get("ph", "")),
        "components": get_input("Components (pipe-separated)", "|".join(existing_data.get("components", []))),
        "documentation": get_input("Documentation (pipe-separated)", "|".join(existing_data.get("documentation", []))),
        "notes": get_input("Notes", existing_data.get("notes", "")),
    }

    if not data["name"]:
        print("Error: Name is a required field.")
        return None
        
    # Add the fixed ID back in for updates
    if 'id' in existing_data:
        data['id'] = existing_data['id']

    return data

def csv_row_to_reagent_dict(row):
    """Converts a dictionary from a CSV row or prompt to the nested reagent format."""
    # The 'id' is now optional in the input row, as it will be generated.
    reagent = {
        "name": row.get("name"),
        "aliases": row.get("aliases", "").split('|') if row.get("aliases") else [],
        "category": row.get("category"),
        # ... (rest of the conversion is the same) ...
        "status": row.get("status"),
        "form": row.get("form"),
        "properties": {},
        "components": row.get("components", "").split('|') if row.get("components") else [],
        "documentation": row.get("documentation", "").split('|') if row.get("documentation") else [],
        "notes": row.get("notes")
    }
    # Add id if it exists (for updates)
    if row.get('id'):
        reagent['id'] = row.get('id')
        
    try:
        if row.get("concentration_value") and row.get("concentration_unit"):
            reagent["properties"]["concentration"] = {
                "value": float(row["concentration_value"]),
                "unit": row["concentration_unit"]
            }
        if row.get("ph"):
            reagent["properties"]["ph"] = float(row["ph"])
    except (ValueError, TypeError):
        print("Warning: Could not parse numeric properties (concentration/pH).")
    return reagent

# ... (handle_list and handle_remove are unchanged) ...
def handle_list(args):
    """Lists all reagents or searches for a specific one."""
    if args.search:
        reagent = reagent_manager.find_reagent(args.search)
        if reagent:
            print(json.dumps(reagent, indent=2))
        else:
            print(f"Reagent '{args.search}' not found.")
    else:
        all_reagents = reagent_manager.get_all_reagents()
        print(f"Found {len(all_reagents)} reagents in the database:")
        for reagent in all_reagents:
            print(f"- {reagent['id']} ({reagent['name']})")

def handle_remove(args):
    """Removes a reagent by its ID."""
    reagent_manager.delete_reagent(args.reagent_id)


def handle_add(args):
    """Adds new reagents from a CSV file or interactively."""
    if args.file_path:
        try:
            with open(args.file_path, 'r', encoding='utf-8') as f:
                reader = csv.DictReader(f)
                for row in reader:
                    new_reagent_data = csv_row_to_reagent_dict(row)
                    reagent_manager.add_reagent(new_reagent_data)
        except FileNotFoundError:
            print(f"Error: The file '{args.file_path}' was not found.")
    else:
        print("--- Add Reagent (Interactive Mode) ---")
        row_data = _prompt_for_reagent_data()
        if row_data:
            new_reagent_data = csv_row_to_reagent_dict(row_data)
            reagent_manager.add_reagent(new_reagent_data)

def handle_update(args):
    """Updates an existing reagent from a file or interactively."""
    existing_reagent = reagent_manager.get_reagent_by_id(args.reagent_id)
    if not existing_reagent:
        print(f"Error: Reagent with ID '{args.reagent_id}' not found.")
        return

    if args.file_path:
        try:
            with open(args.file_path, 'r', encoding='utf-8') as f:
                reader = csv.DictReader(f)
                row = next(reader) 
                updated_data = csv_row_to_reagent_dict(row)
            
            # Ensure the ID from the file matches for safety
            if updated_data.get('id') != args.reagent_id:
                print("Error: The 'id' in the file does not match the reagent_id provided.")
                return
            reagent_manager.update_reagent(args.reagent_id, updated_data)
        except FileNotFoundError:
            print(f"Error: The file '{args.file_path}' was not found.")
    else:
        print(f"--- Update Reagent (Interactive Mode): {args.reagent_id} ---")
        row_data = _prompt_for_reagent_data(existing_reagent)
        if row_data:
            updated_data = csv_row_to_reagent_dict(row_data)
            reagent_manager.update_reagent(args.reagent_id, updated_data)

def main():
    parser = argparse.ArgumentParser(description="A CLI to manage the reagent inventory database.")
    subparsers = parser.add_subparsers(dest="command", required=True, help="Available commands")

    # --- List Command ---
    parser_list = subparsers.add_parser("list", help="List all reagents or search for one.")
    parser_list.add_argument("search", nargs="?", help="Name or alias of the reagent to search for.")
    parser_list.set_defaults(func=handle_list)

    # --- Add Command ---
    parser_add = subparsers.add_parser("add", help="Add new reagents from a file or interactively.")
    parser_add.add_argument("file_path", nargs="?", default=None, help="Path to a CSV file. If omitted, runs in interactive mode.")
    parser_add.set_defaults(func=handle_add)

    # --- Update Command ---
    parser_update = subparsers.add_parser("update", help="Update a reagent from a file or interactively.")
    parser_update.add_argument("reagent_id", help="The unique ID of the reagent to update.")
    parser_update.add_argument("file_path", nargs="?", default=None, help="Path to a CSV file. If omitted, runs in interactive mode.")
    parser_update.set_defaults(func=handle_update)

    # --- Remove Command ---
    parser_remove = subparsers.add_parser("remove", help="Remove a reagent by its ID.")
    parser_remove.add_argument("reagent_id", help="The unique ID of the reagent to remove.")
    parser_remove.set_defaults(func=handle_remove)

    args = parser.parse_args()
    args.func(args)

if __name__ == "__main__":
    main()