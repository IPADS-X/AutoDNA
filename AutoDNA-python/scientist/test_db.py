import os
import argparse # Import the argparse library
from tools.file_manager import file_manager
from config import shared_files_path, input_path, output_path

def add_all_files_to_file_manager():
    """Adds all .txt files from the shared path to the file manager."""
    print("Searching for files to add...")
    for filename in os.listdir(shared_files_path):
        # The original code specified .md but the loop checked for .txt. Correcting to .md.
        # Change ".md" to ".txt" if that was your intended file type.
        if filename.endswith(".md"):
            file_path = os.path.join(shared_files_path, filename)
            with open(file_path, 'r', encoding='utf-8') as file:
                content = file.read()
                file_manager.add_file_by_original_name(
                    content=content, 
                    original_filename=filename, 
                    description="Added from shared_files_path"
                )

def check_files():
    """Checks for the existence of specific files and prints their content length."""
    print("\nChecking for specific files...")
    file_check_ids = ["Literature-0", "Protocol-1", "Reagent-3"]
    for file_id in file_check_ids:
        file_content = file_manager.get_file_content(file_id)
        if file_content:
            print(f"File ID: {file_id}, Content Length: {len(file_content)}")
        else:
            # The original code would raise an error on len(None). This is safer.
            print(f"File ID: {file_id} not found.")

def get_file_and_save(file_id: str):
    """Retrieves a file by ID and saves its content to the output directory."""
    print(f"Attempting to retrieve file with ID: '{file_id}'...")
    content = file_manager.get_file_content(file_id)

    if content is None:
        # get_file_content() already prints an error message.
        return

    if "Code" in file_id:
        output_file_path = os.path.join(output_path, f"{file_id}.py")
    else:
        output_file_path = os.path.join(output_path, f"{file_id}.md")
    os.makedirs(output_path, exist_ok=True)
    with open(output_file_path, 'w', encoding='utf-8') as f:
        f.write(content)

    print(f"Success! Content of file '{file_id}' saved to '{output_file_path}'")

def print_latest_file_to_md():
    """Gets the latest file from the repo and prints it to a default file."""
    print("Finding the latest file in the repository...")
    latest_files = file_manager.get_latest_files(1)

    if not latest_files:
        print("No files found in the database.")
        return

    latest_id = latest_files[0]
    content = file_manager.get_file_content(latest_id)

    if content is None:
        print(f"Error: Could not retrieve content for the latest file ID: {latest_id}")
        return

    # Define the output path and ensure the directory exists
    output_file_path = os.path.join(output_path, "agent_latest_output.md")
    os.makedirs(output_path, exist_ok=True)

    with open(output_file_path, 'w', encoding='utf-8') as f:
        f.write(content)

    print(f"Success! Content of the latest file ('{latest_id}') has been written to '{output_file_path}'")

if __name__ == "__main__":
    parser = argparse.ArgumentParser(description="Manage the file_manager database.")
    
    # This group handles destructive actions
    destructive_group = parser.add_mutually_exclusive_group()
    destructive_group.add_argument("--remove", metavar="FILENAME", help="Remove a single file by its ID.")
    destructive_group.add_argument("--add", metavar="FILENAME", help="Add a new file using its original name. Content is read from a default file.")
    destructive_group.add_argument("--modify", metavar="FILENAME", help="Modify the content of an existing file by ID.")
    destructive_group.add_argument("--clear", action="store_true", help="Clear all files from the database.")

    # This argument is for querying, so it's not in the group
    parser.add_argument("--latest", type=int, metavar="N", help="List the N most recent files.")
    parser.add_argument("-g", "--get_file", metavar="FILE_ID", help="Retrieve a file by ID and save its content to a .md file.")
    parser.add_argument("--get_latest", action="store_true", help="Write the single latest file's content to a default .md file.")

    
    args = parser.parse_args()

    # --- Execute logic based on arguments ---
    if args.get_file:
        file_id_to_get = os.path.splitext(args.get_file)[0]
        get_file_and_save(file_id_to_get)
    elif args.get_latest:
        print_latest_file_to_md()
    elif args.latest:
        latest_files = file_manager.get_latest_files(args.latest)
        if latest_files:
            print(f"The {len(latest_files)} most recent files are:")
            for file_id in latest_files:
                print(f"- {file_id}")
        else:
            print("No files found in the database.")
    elif args.add:
        original_filename = args.add
        content_path = os.path.join(input_path, "temp.md")
        with open(content_path, 'r', encoding='utf-8') as file:
                new_content = file.read()
        print(f"Attempting to add file '{original_filename}' with content from '{content_path}'.")
        file_manager.add_file_by_original_name(
            content=new_content,
            original_filename=original_filename,
            description="Added via --add command"
        )
            
    elif args.modify:
        file_id_to_modify = os.path.splitext(args.modify)[0]
        with open(os.path.join(input_path, "temp.md"), 'r', encoding='utf-8') as file:
            new_content = file.read()
        file_manager.modify_file(file_id_to_modify, new_content)   

    elif args.remove:
        file_id_to_remove = os.path.splitext(args.remove)[0]
        file_manager.remove_file(file_id_to_remove)


    
    elif args.clear:
        file_manager.clear_all_files()
        
    else:
        # Default behavior if no flags are provided
        print("No special flags specified. Running default functions.")
        add_all_files_to_file_manager()
        check_files()