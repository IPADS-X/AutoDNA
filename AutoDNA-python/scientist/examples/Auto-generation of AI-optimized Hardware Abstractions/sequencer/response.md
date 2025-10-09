An elegant and intuitive interface for a biological laboratory device should mirror the user's mental model of operating it. For a sequencer, the core interactions are starting a run, checking its progress, and stopping it.

This abstract base class, `Sequencer`, defines that simple, three-method interface. It abstracts away hardware-specific details like flow cell types or connection protocols, focusing only on the essential, high-level commands. This minimalist approach ensures that any concrete implementation, whether for a portable MinION or a high-throughput PromethION, will provide a consistent and easy-to-use programming experience.

```python
import abc
from pathlib import Path
from typing import TypedDict, Literal

class Sequencer(abc.ABC):
    """
    An abstract base class defining a simple, universal interface for
    controlling a DNA/RNA sequencer.

    This interface is designed to be minimal and intuitive, focusing on the
    three core actions of a sequencing workflow: starting, monitoring,
    and stopping a run.
    """

    @abc.abstractmethod
    def start_run(
        self, *, run_name: str, output_directory: Path
    ) -> None:
        """
        Begins a new sequencing run.

        This method assumes the device has been physically prepared with a
        flow cell and a prepared sample library.

        Args:
            run_name: A unique identifier for the sequencing run.
            output_directory: The path to a directory where sequencing
                              data (e.g., FASTQ files) will be written in
                              real-time. The directory will be created if
                              it does not exist.

        Raises:
            RuntimeError: If a run is already in progress or the device
                          is not ready.
        """
        pass

    @abc.abstractmethod
    def stop_run(self) -> None:
        """
        Stops the currently active sequencing run gracefully.

        If a run is in progress, this command will terminate the sequencing
        process and finalize any data output. If no run is active, this
        method does nothing.
        """
        pass

```