```python
class PortType(Enum):
    """Enumeration for the types of ports a robot can interact with."""
    PRIMING = "Priming-Port"
    SPOTON = "SpotON"

class Port:
    """Represents an accessible port on a piece of lab equipment."""
    def __init__(self, port_type: PortType):
        self.port_type = port_type

# --- Robot Abstract Base Class ---

class Robot(BaseMachine, abc.ABC):
    """
    An abstract base class defining the interface for a laboratory robot arm.

    The API is designed to be "container-centric," meaning actions are
    defined by what they do to a container or a specific labware component,
    rather than by the low-level movements of the robot itself.
    """

    @abc.abstractmethod
    def power_on(self) -> None:
        """Powers on the robot."""
        ...

    @abc.abstractmethod
    def power_off(self) -> None:
        """Powers off the robot and ensures it is in a safe state."""
        ...

    @abc.abstractmethod
    def home(self) -> None:
        """Moves the robot arm to its default, safe "home" position."""
        ...

    @abc.abstractmethod
    def move_container(self, container: Container, destination: Location) -> None:
        """
        Picks up a container from its current location, moves it, and places
        it at the specified destination.

        This method encapsulates the entire pick-and-place sequence. The robot
        infers the container's starting point from `container.location`. Upon
        successful completion, the container's location attribute should be
        updated to the destination.

        Args:
            container: The container object to be moved.
            destination: The target location for the container.
        """
        ...

    @abc.abstractmethod
    def open_port(self, port: Port) -> None:
        """
        Opens a specified port on a machine.

        Args:
            port: The Port object representing the port to be opened.
        """
        ...

    @abc.abstractmethod
    def close_port(self, port: Port) -> None:
        """
        Closes a specified port on a machine.

        Args:
            port: The Port object representing the port to be closed.
        """
        ...
```