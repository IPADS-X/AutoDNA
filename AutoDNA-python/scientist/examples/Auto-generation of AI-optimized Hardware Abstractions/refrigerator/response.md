```python
from abc import ABC, abstractmethod
from enum import IntEnum
from typing import List, Union

# --- Provided Context ---

class Location:
    # Usage: The location of a machine or container
    def __init__(self, description: str):
        self.description = description

class BaseMachine:
    def __init__(self, location: Location):
        self.location = location

    def get_location(self) -> Location:
        return self.location

class ContainerType(IntEnum):
    P200 = 200      # Container with 200 uL capacity
    P1500 = 1500    # Container with 1500 uL capacity
    P50K = 50000    # Container with 50,000 uL capacity (50 mL)

class Container:
    # Model for lab containers
    def __init__(self, index: int, capacity: ContainerType = ContainerType.P200, volume: float = 0.0, label: str = "water", location: Location = None):
        self.index = index
        self.volume = volume
        self.capacity = max(capacity.value, volume)
        self.label = label
        self.capped = False  # Containers are uncapped by default
        self.location = location

# --- API Design ---

class Refrigerator(BaseMachine, ABC):
    """
    Abstract base class representing a simple, low-temperature storage unit.

    This API models the refrigerator as a passive storage environment. In line
    with the "Container-Centric Design" principle, the refrigerator does not
    have "place" or "retrieve" methods. Such actions are performed *on* a
    Container by an external agent (e.g., a robotic arm), which changes the
    container's location to be inside the refrigerator. This class provides
    methods to monitor and control the environment and to query its contents.
    """

    @abstractmethod
    def __init__(self, location: Location, target_temperature: float = 4.0):
        """
        Initializes the Refrigerator.

        Args:
            location: The physical location of the refrigerator.
            target_temperature: The default target temperature in Celsius.
        """
        ...

    @abstractmethod
    def set_target_temperature(self, celsius: float):
        """
        Sets the target temperature for the refrigerator's environment.

        Args:
            celsius: The desired target temperature in Celsius.
        """
        ...

    @abstractmethod
    def get_current_temperature(self) -> float:
        """
        Returns the current internal temperature of the refrigerator in Celsius.
        """
        ...

    @abstractmethod
    def get_contents(self) -> List[Container]:
        """
        Returns a list of all containers currently stored inside the refrigerator.
        """
        ...
```