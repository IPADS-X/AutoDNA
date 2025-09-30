```python
import abc
from abc import ABC, abstractmethod
from enum import IntEnum
from typing import List, Union

# --- Provided Context ---

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

# --- API Design ---

class MagRack(BaseMachine, ABC):
    """
    Abstract base class for a magnetic separation rack for 200ul PCR tubes.

    This device uses high-performance permanent magnets to separate paramagnetic
    beads from a solution. The magnetic field is always active, requiring no
    external power. The primary operation involves placing tubes into the rack
    and waiting for a specified duration for the beads to form a tight pellet
    against the tube wall.
    """

    MODEL: str = "MS-08"
    NUM_SLOTS: int = 8

    @abstractmethod
    def separate(self, duration_seconds: int = 60) -> None:
        """
        Waits for a specified duration to allow magnetic beads to separate.

        This method represents the core procedure of using the rack: allowing
        time for the permanent magnets to attract and immobilize the beads
        against the side of the tubes.

        Args:
            duration_seconds: The time in seconds to wait for complete bead
                              separation. Defaults to 60 seconds.
        """
        ...
```