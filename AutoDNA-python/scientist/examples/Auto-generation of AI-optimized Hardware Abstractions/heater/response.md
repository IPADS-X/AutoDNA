```python
from abc import ABC, abstractmethod
from enum import IntEnum
from typing import Optional

# Provided context is assumed to be available
# from typing import List, Union
#
# class Location:
#     # ...
#
# class BaseMachine:
#     # ...


class Heater1_5mlTubes(BaseMachine, ABC):
    """
    Abstract base class for a heating incubator designed for 1.5ml sample tubes.

    This interface defines the essential controls for setting temperature and
    running timed or indefinite incubation processes, based on the operating
    steps of the Model TH-1500.
    """

    @abstractmethod
    def start(self, temperature_celsius: float, duration_minutes: Optional[float] = None) -> None:
        """
        Starts the heating process.

        This single method handles both indefinite heating and timed incubations,
        reflecting the core functionalities of "Set Temperature" and "Set Time".

        Args:
            temperature_celsius: The target temperature in degrees Celsius.
            duration_minutes: The incubation time in minutes. If None, the heater
                              will maintain the target temperature indefinitely.
        """
        ...

    @abstractmethod
    def stop(self) -> None:
        """
        Immediately stops any active heating or timed incubation process.
        The heater will begin to cool down passively.
        """
        ...

    @abstractmethod
    def get_current_temperature(self) -> float:
        """
        Returns the current temperature of the heating block in degrees Celsius.

        Returns:
            The current temperature as a float.
        """
        ...
```