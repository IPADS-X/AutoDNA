```python
from abc import ABC, abstractmethod
from enum import IntEnum
from typing import List, Union

# Provided Context
class Location:
    # Usage: The location of a machine or container
    # A machine named Robot can help move containers to Location
    def __init__(self, description: str):
        self.description = description

class BaseMachine:
    def __init__(self, location: Location):
        self.location = location

    def get_location(self) -> Location:
        return self.location

# API Design
class MagRack(BaseMachine, ABC):
    """
    An abstract base class for a passive magnetic rack for 1.5ml tubes.

    This rack uses permanent magnets to separate paramagnetic beads from a
    solution. Based on the principle of operation, the magnetic field is always
    active. The core operation involves placing a tube in the rack and waiting
    for a pellet to form.
    """

    def __init__(self, location: Location, model: str = "MS-08"):
        """
        Initializes the MagRack.

        Args:
            location: The physical location of the magnetic rack.
            model: The specific model identifier of the rack, e.g., "MS-08".
        """
        super().__init__(location)
        self.model: str = model
        self.capacity: int = 8
        self.compatible_tube_type: str = "1.5ml PCR Tube"

    @abstractmethod
    def separate(self, wait_duration_seconds: int = 60) -> None:
        """
        Performs the magnetic separation by waiting for a specified duration.

        This method simulates the primary function of the rack: holding tubes
        stationary within the magnetic field to allow paramagnetic beads
        to pellet against the tube wall. In a real-world scenario, this would
        be a blocking call that waits for the specified time before proceeding.

        Args:
            wait_duration_seconds: The time in seconds to wait for the beads
                                   to be fully separated and form a tight pellet.
        """
        pass
```