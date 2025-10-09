An elegant and intuitive API design for a biological laboratory instrument, as requested.

```python
from abc import ABC, abstractmethod
from typing import List

# --- Provided Context ---

class Location:
    """The location of a machine or container."""
    def __init__(self, description: str):
        self.description = description

class BaseMachine:
    """An abstract representation of a laboratory machine."""
    def __init__(self, location: Location):
        self.location = location

    def get_location(self) -> Location:
        """Returns the physical location of the machine."""
        return self.location

# --- API Design ---

class Fluorometer200ul(BaseMachine, ABC):
    """
    An abstract base class representing a fluorometer designed for up to eight
    concurrent 200µL tubes.

    This interface defines the essential operations for fluorescence-based
    quantification and raw signal measurement, based on a two-point calibration
    system.
    """

    @abstractmethod
    def calibrate(
        self,
        standard1_concentration: float,
        standard2_concentration: float,
        unit: str,
    ) -> None:
        """
        Performs a two-point calibration for concentration measurements.

        The implementation should handle the process of reading the two provided
        standards. The resulting calibration is stored internally for
        subsequent calls to `measure_concentration`.

        Args:
            standard1_concentration: The known concentration of the first standard.
            standard2_concentration: The known concentration of the second standard.
            unit: The unit of concentration (e.g., 'ng/µL'). This unit will be
                  used for the results of `measure_concentration`.

        Raises:
            Exception: If the calibration process fails for any reason (e.g.,
                       hardware error, invalid readings).
        """
        ...

    @abstractmethod
    def measure_concentration(self, sample_volume_ul: int = 10) -> List[float]:
        """
        Measures the concentration of samples in all eight wells.

        A calibration must be successfully performed via the `calibrate` method
        before using this function.

        Args:
            sample_volume_ul: The volume of the initial sample added to the
                              assay tube, in microliters (µL). Defaults to 10.
                              Valid range is typically 1-20 µL.

        Returns:
            A list of eight floats, representing the calculated concentration
            for each corresponding well (1 through 8). The unit is determined
            by the `unit` provided during the last successful calibration.

        Raises:
            Exception: If the instrument has not been calibrated or if the
                       measurement fails.
        """
        ...

    @abstractmethod
    def measure_fluorescence(self) -> List[int]:
        """
        Measures the raw fluorescence signal from all eight wells.

        This method provides the raw Relative Fluorescence Units (RFU) and does
        not require prior calibration.

        Returns:
            A list of eight integers, representing the raw fluorescence
            signal (RFU) for each corresponding well (1 through 8).
        """
        ...

```