```python
import abc
from enum import IntEnum
from typing import List, Union, TypedDict

# --- Provided Context ---

class Location:
    """
    Represents the physical location of a machine or container in the lab.
    This helps robots and scheduling systems find and interact with the hardware.
    """
    def __init__(self, description: str):
        self.description = description

class BaseMachine(abc.ABC):
    """An abstract base class for all laboratory machines."""
    def __init__(self, location: Location):
        self.location = location

    def get_location(self) -> Location:
        """Returns the machine's physical location."""
        return self.location

# --- API Design for ThermoCycler ---

class TemperatureStep(TypedDict):
    """
    Defines a single temperature-hold step in a protocol.
    Example: {"temperature_celsius": 95.0, "duration_seconds": 180}
    """
    temperature_celsius: float
    duration_seconds: int

class Cycle(TypedDict):
    """
    Defines a cycle of multiple temperature steps to be repeated.
    Example: {
        "steps": [
            {"temperature_celsius": 95.0, "duration_seconds": 30},
            {"temperature_celsius": 60.0, "duration_seconds": 45}
        ],
        "count": 30
    }
    """
    steps: List[TemperatureStep]
    count: int

# A protocol is a list of individual steps or entire cycles.
ProtocolStep = Union[TemperatureStep, Cycle]

class ThermoCycler200ul(BaseMachine, abc.ABC):
    """
    An abstract interface for controlling a thermal cycler designed
    for standard 200ul PCR tubes or plates.
    """

    def __init__(self, location: Location):
        """Initializes the ThermoCycler at a specific lab location."""
        super().__init__(location)
        ...

    @abc.abstractmethod
    def open_lid(self) -> None:
        """Opens the heated lid to allow for loading or unloading of plates."""
        ...

    @abc.abstractmethod
    def close_lid(self) -> None:
        """Closes the heated lid."""
        ...

    @abc.abstractmethod
    def run_protocol(self, protocol: List[ProtocolStep]) -> None:
        """
        Starts a thermal cycling protocol.

        The machine will execute the list of steps and cycles in order.
        This method should return immediately, and the run's progress can
        be monitored via `get_status()`.

        Args:
            protocol: A list of TemperatureStep and Cycle dictionaries defining
                      the complete thermal cycling program.
        """
        ...

    @abc.abstractmethod
    def pause(self) -> None:
        """Pauses the currently executing protocol."""
        ...

    @abc.abstractmethod
    def resume(self) -> None:
        """Resumes a paused protocol."""
        ...

    @abc.abstractmethod
    def stop(self) -> None:
        """
        Immediately stops the current protocol and cools the block to a safe
        standby temperature.
        """
        ...
```