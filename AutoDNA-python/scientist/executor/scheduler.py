from typing import List, Optional, Any, Dict

# Configure logging to print messages to the console, making it easy to see the flow of commands.
def call_machine_command(command_name: str, params: Dict[str, Any]) -> Optional[Any]:
    """
    A mock implementation of the function that sends commands to lab machines.
    
    This function logs the command and its parameters, and provides mock return
    values where necessary for the simulation to proceed. This allows for testing
    and debugging of protocols without needing physical hardware.
    """
    

    # The fluorometer is the only instrument that needs a specific mock return value
    # for the calling code to function correctly.
    if command_name == "fluorometer_measure":
        print(f"Below are Just Mockup results. Ignore them.")
        # The number of containers is passed in the params from the instrument module.
        num_containers = params.get("num_containers", 0)
        # print(f"Fluorometer measured {num_containers} containers. Returning mock data.")
        # Return a list of -1s, with the length matching the number of containers.
        return [-1] * num_containers
    
    # For all other commands, no specific return value is needed for the protocol to run.
    # Returning None is a safe default.
    return None