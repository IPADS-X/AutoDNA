```python
from abc import ABC, abstractmethod
from enum import IntEnum
from typing import Optional

# --- Provided Context ---

class Location:
    """Represents the physical location of a machine or container."""
    def __init__(self, description: str):
        self.description = description

class BaseMachine:
    """A base class for all laboratory machines."""
    def __init__(self, location: Location):
        self.location = location

    def get_location(self) -> Location:
        """Returns the machine's location."""
        return self.location

# --- API Design ---

class HeaterShaker(BaseMachine, ABC):
    """
    An abstract base class for a heated shaker, typically used with 200ul tubes.

    This interface provides simple controls for setting temperature and shaking speed
    for a specified duration.
    """

    @abstractmethod
    def incubate(
        self,
        target_temperature_celsius: Optional[float] = None,
        target_speed_rpm: Optional[int] = 1300,
        duration_seconds: Optional[int] = None
    ) -> None:
        """
        Starts a heating and/or shaking protocol.

        At least one of `target_temperature_celsius` or `target_speed_rpm` must
        be provided to start a run.

        Args:
            target_temperature_celsius: The target temperature in Celsius. If None,
                the heater remains off.
            target_speed_rpm: The target shaking speed in revolutions per minute.
                If None, the shaker remains off.
            duration_seconds: The duration of the run in seconds. If None, the
                process runs indefinitely until `stop()` is called.
        """
        ...

    @abstractmethod
    def stop(self) -> None:
        """
        Safely stops all heating and shaking operations.

        This method will immediately turn off the heater and initiate a safe
        deceleration of the shaker.
        """
        ...

    @abstractmethod
    def get_current_temperature_celsius(self) -> float:
        """
        Returns the current temperature of the heating block in Celsius.
        """
        ...

    @abstractmethod
    def get_current_speed_rpm(self) -> int:
        """
        Returns the current shaking speed in revolutions per minute.
        """
        ...
```