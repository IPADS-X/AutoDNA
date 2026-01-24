```python
import abc
from enum import IntEnum
from typing import List, Union

# --- Provided Context ---

class Location:
    """Represents the location of a machine or container."""
    def __init__(self, description: str):
        self.description = description

class BaseMachine(abc.ABC):
    """Abstract base class for any machine in the lab."""
    def __init__(self, location: Location):
        self.location = location

    def get_location(self) -> Location:
        return self.location

class ContainerType(IntEnum):
    """Enumeration for standard container capacities in microliters (uL)."""
    P200 = 200      # Container with 200 uL capacity
    P1500 = 1500    # Container with 1500 uL capacity
    P50K = 50000    # Container with 50,000 uL capacity (50 mL)

class Container:
    """A model for lab containers like wells or tubes."""
    def __init__(self,
                 index: int,
                 capacity: ContainerType = ContainerType.P200,
                 volume: float = 0.0,
                 label: str = "water",
                 location: Location = None):
        self.index = index
        self.volume = volume
        self.capacity = max(capacity.value, volume)
        self.label = label
        self.capped = False  # Containers are uncapped by default
        self.location = location

# --- API Design ---

class Pipette(BaseMachine, abc.ABC):
    """
    An abstract base class defining the interface for a robotic pipette.

    This interface is designed to be container-centric, where actions are
    performed on or between specified containers. Complexities like tip
    management are handled implicitly with sensible defaults to ensure
    a simple and intuitive user experience.
    """

    @abc.abstractmethod
    def transfer(
        self,
        volume: Union[float, List[float]],
        source: Union[Container, List[Container]],
        destination: Union[Container, List[Container]],
        reuse_tip: bool = False,
        touch_tip: bool = True,
        air_gap_volume: float = 0.0
    ) -> None:
        """
        Transfers a specified volume of liquid from a source to a destination.

        This method supports one-to-one, one-to-many (distribute),
        many-to-one (pool), and many-to-many transfers. By default, a new
        sterile tip is used for the entire operation and then discarded.

        Args:
            volume: The volume in microliters (uL) to transfer. Can be a
                    single value for all transfers or a list of volumes
                    corresponding to each source/destination pair.
            source: The container or list of containers to aspirate from.
            destination: The container or list of containers to dispense into.
            reuse_tip: If True, the pipette will not discard the tip after
                       the operation is complete. Defaults to False.
            touch_tip: If True, touches the pipette tip to the side of the
                       destination container to ensure complete delivery.
                       Defaults to True.
            air_gap_volume: Volume of air in uL to aspirate after the liquid
                            to prevent dripping. Defaults to 0.0.
        """
        ...

    @abc.abstractmethod
    def mix(
        self,
        volume: float,
        location: Container,
        repetitions: int,
        rate_per_minute: int = 20,
        reuse_tip: bool = False
    ) -> None:
        """
        Mixes the contents of a container by repeated aspiration and dispensing.

        A new tip is used by default and discarded upon completion.

        Args:
            volume: The volume in uL to aspirate and dispense during mixing.
            location: The container whose contents are to be mixed.
            repetitions: The number of mixing cycles to perform.
            rate_per_minute: The speed of the mix operation, in cycles per
                             minute. Defaults to 20.
            reuse_tip: If True, the pipette will not discard the tip after
                       the operation is complete. Defaults to False.
        """
        ...
```