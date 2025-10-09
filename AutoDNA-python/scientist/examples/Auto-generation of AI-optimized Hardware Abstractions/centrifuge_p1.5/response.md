An elegant and intuitive API for laboratory equipment prioritizes safety, clarity, and ease of use for its primary users: scientists and technicians. The design should prevent common errors by requiring explicit parameters for critical operations, directly reflecting the step-by-step nature of the provided user guide.

Here is the abstract base class design for the `Centrifuge (1.5ml tubes)`.

```python
import abc
from enum import IntEnum
from typing import List, Union

# Provided Context
class Location:
    # Usage: The location of a machine or container
    # A robot can move containers to the Location
    def __init__(self, description: str):
        self.description = description

class BaseMachine:
    def __init__(self, location: Location):
        self.location = location

    def get_location(self) -> Location:
        return self.location

# API Design
class Centrifuge1p5mL(BaseMachine, abc.ABC):
    """
    Abstract base class for a 'Centrifuge (1.5ml tubes)'.

    This class defines the standard interface for operating a microcentrifuge
    that is specifically designed for standard 1.5ml centrifuge tubes,
    as described in the Quick Start Guide.
    """

    @abc.abstractmethod
    def run(self, speed_rpm: int, duration_seconds: int) -> None:
        """
        Sets the parameters and starts a centrifugation cycle.

        This method is blocking and will only return after the centrifugation
        run has completed. It combines the 'Set Speed', 'Set Duration', and
        'Start' steps from the user guide into a single, explicit command.

        Args:
            speed_rpm: The desired rotational speed in revolutions per minute (RPM).
            duration_seconds: The desired duration of the run in seconds.
        """
        pass

```