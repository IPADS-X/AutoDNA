import uvicorn
from fastapi import FastAPI, BackgroundTasks, HTTPException
from fastapi.middleware.cors import CORSMiddleware
from fastapi.responses import FileResponse
from pydantic import BaseModel
from config import output_path, current_output_file_path, prompt_path
from loguru import logger
import json
import os
import sys

# Import the main function from your other script
# Make sure ai_scientist.py is in the same directory
try:
    from ai_scientist import main as ai_scientist_main
except ImportError:
    # Create a dummy function if the file doesn't exist yet
    # This allows the server to start even if ai_scientist.py is not created.
    logger.error("Warning: 'ai_scientist.py' not found. Using a placeholder function.")
    def ai_scientist_main():
        logger.error("ai_scientist.main() was called, but the file is missing.")


# --- Pydantic Model for Request Body ---
# This defines the expected structure of the POST request data.
class UserPrompt(BaseModel):
    user_prompt: str

# --- FastAPI App Initialization ---
app = FastAPI()


origins = ["*"] 

app.add_middleware(
    CORSMiddleware,
    allow_origins=origins,        
    allow_credentials=True,       
    allow_methods=["*"],          
    allow_headers=["*"],        
)
# --- File Paths ---
# Define paths to ensure files are found correctly.
HEARTBEAT_FILE = current_output_file_path
PROMPT_OUTPUT_FILE = os.path.join(prompt_path, "user_prompt_test.md")


# --- Helper Function to run the AI Scientist ---
def run_ai_task(prompt: str):
    """
    Writes the prompt to a file and then calls the main function
    from the ai_scientist script.
    """
    logger.info(f"Received prompt: {prompt}")

    # 1. Write the received prompt to a file.
    try:
        with open(PROMPT_OUTPUT_FILE, "w") as f:
            f.write(prompt)
        logger.success(f"Successfully wrote prompt to '{PROMPT_OUTPUT_FILE}'")
    except IOError as e:
        logger.error(f"Error writing prompt to file: {e}")
        # Update status to reflect the error

    
    # Store original sys.argv to avoid interfering with the server
    original_argv = sys.argv
    
    try:
        # Set the arguments for the ai_scientist script. The first element is the script name.
        sys.argv = ['ai_scientist.py', '--no_filtering']
        
        # Call the target function
        if "RPA" in prompt or "rpa" in prompt:
            sys.argv.append('--rpa')
        else:
            sys.argv.append('--test')
        ai_scientist_main()

        logger.info("'ai_scientist.main()' function finished.")
    finally:
        # Restore the original sys.argv to ensure the server continues to run correctly
        sys.argv = original_argv


# --- API Endpoints ---

@app.post("/prompt")
async def handle_user_prompt(prompt: UserPrompt, background_tasks: BackgroundTasks):
    """
    Receives a user prompt via POST request.
    It writes the prompt to a file and triggers the ai_scientist script
    as a background task, allowing the server to respond immediately.
    """
    background_tasks.add_task(run_ai_task, prompt.user_prompt)
    return {"message": "Prompt received and task started in the background."}


@app.get("/heartbeat")
async def get_heartbeat():
    """
    Receives a GET request for a heartbeat.
    Returns the content of the 'current.json' file.
    """
    if not os.path.exists(HEARTBEAT_FILE):
        raise HTTPException(status_code=404, detail=f"'{HEARTBEAT_FILE}' not found. Please create it.")

    return FileResponse(HEARTBEAT_FILE)


# --- Main Entry Point ---
if __name__ == "__main__":
    # Initialize the status file if it doesn't exist
    if not os.path.exists(HEARTBEAT_FILE):
        logger.info(f"'{HEARTBEAT_FILE}' not found, creating a default one.")
        with open(HEARTBEAT_FILE, "w") as f:
            json.dump({}, f)

    # Run the server
    logger.info("Starting FastAPI server on port 8081...")
    uvicorn.run(app, host="0.0.0.0", port=8081, log_level="warning")
