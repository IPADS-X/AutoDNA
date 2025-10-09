from scheduler import call_machine_command 
from abc import ABC, abstractmethod
from enum import IntEnum, Enum
from typing import List, Union, Optional, TypedDict
import abc
from enum import Enum

# --- Global Instrument Registry ---
# This registry maps location description strings to their corresponding instrument instances.
# It is used by the Robot to update which containers are on which instruments.
INSTRUMENT_REGISTRY = {}

# --- Core Definitions ---

class Location:
    """Represents a physical location in the lab, such as a machine's slot."""
    def __init__(self, description: str):
        self.description = description

class BaseMachine:
    """A base class for all physical lab instruments that can hold containers."""
    def __init__(self, location: Location):
        self.location = location
        # A list to track container objects currently placed on this machine.
        self.containers_on_instrument: List['Container'] = []
        # Automatically register the instrument instance in the global registry upon creation.
        INSTRUMENT_REGISTRY[location.description] = self

    def get_location(self) -> Location:
        return self.location

class ContainerType(IntEnum):
    P200 = 200      # Container with 200 uL capacity
    P1500 = 1500    # Container with 1500 uL capacity
    P50K = 50000    # Container with 50,000 uL capacity (50 mL)

class Container:
    """Model for lab containers (e.g., tubes, plates)."""
    def __init__(self, label: str, index: int, capacity: ContainerType = ContainerType.P200, volume: float = 0.0, replenishable: bool = False, location: Optional[Location] = None):
        self.index = index
        self.volume = volume
        self.capacity = max(capacity.value, volume)
        self.label = label
        self.capped = False
        self.replenishable = replenishable
        self.location = location

# --- Default Locations ---
DEFAULT_CONTAINERHOLDER_LOCATION = Location("ContainerHolder")
DEFAULT_CAPPER_LOCATION = Location("capper")
DEFAULT_CENTRIFUGE_P1500_LOCATION = Location("centrifuge_P1500")
DEFAULT_CENTRIFUGE_P200_LOCATION = Location("centrifuge_P200")
DEFAULT_FLUOROMETER_LOCATION = Location("fluorometer")
DEFAULT_HEATER_LOCATION = Location("heater")
DEFAULT_HEATERSHAKER_LOCATION = Location("heater_shaker")
DEFAULT_MAGRACKP1500_LOCATION = Location("magrack_P1500")
DEFAULT_MAGRACKP200_LOCATION = Location("magrack_P200")
DEFAULT_THERMAL_CYCLER_LOCATION = Location("thermal_cycler")
DEFAULT_PIPETTE_LOCATION = Location("pipette")
DEFAULT_ROBOT_LOCATION = Location("robot")

class ContainerManager:
    """Manages the allocation and retrieval of new or existing containers."""
    _nextContainerIndex = 1000
    
    def __init__(self):
        self.containerList = []

    def newContainer(self, label: str, cap: ContainerType = ContainerType.P200) -> Container:
        """Allocates a new, empty container."""
        index = ContainerManager._nextContainerIndex
        container = Container(label=label, index=index, capacity=cap, volume=0, location=DEFAULT_CONTAINERHOLDER_LOCATION)
        self.containerList.append(container)
        ContainerManager._nextContainerIndex += 1
        call_machine_command("container_allocate", {
            "container_index": container.index,
            "container_label": container.label
        })
        return container

    def getContainerForReplenish(self, name: str, required_volume: float = 0) -> Container:
        """Gets a container that can be automatically replenished with a reagent."""
        index = ContainerManager._nextContainerIndex
        capacity = ContainerType.P200 if required_volume <= 200 else ContainerType.P50K
        container = Container(label=name, index=index, capacity=capacity, volume=required_volume, location=DEFAULT_CONTAINERHOLDER_LOCATION, replenishable=True)
        self.containerList.append(container)
        ContainerManager._nextContainerIndex += 1
        call_machine_command("container_get", {
            "container_index": container.index,
            "container_label": container.label
        })
        return container
    
class PortType(Enum):
    """Enumeration for the types of ports a robot can interact with."""
    PRIMING = "Priming-Port"
    SPOTON = "SpotON"

class Port:
    """Represents an accessible port on a piece of lab equipment."""
    def __init__(self, port_type: PortType):
        self.port_type = port_type
        
    def get_tube(self):
        container = container_manager.getContainerForReplenish(self.port_type.name)
        container.capacity = ContainerType.P50K
        return container

class PortType(Enum):
    """Enumeration for the types of ports a robot can interact with."""
    PRIMING = "Priming-Port"
    SPOTON = "SpotON"

class Port:
    """Represents an accessible port on a piece of lab equipment."""
    def __init__(self, port_type: PortType):
        self.port_type = port_type

# --- Instrument APIs ---

class Timer:
    """A non-physical machine for pausing execution."""
    def wait(self, time: int):
        call_machine_command("timer_wait", {"time": time})

class Capper_200ul_tubes(BaseMachine, abc.ABC):
    """An automatic capper for 200µL PCR tubes."""
    def __init__(self, location: Location = DEFAULT_CAPPER_LOCATION):
        super().__init__(location)
    
    def cap(self) -> None:
        """Executes a capping cycle."""
        if not self.containers_on_instrument:
            raise Exception("Capper_200ul_tubes cannot be run with empty containers")
        call_machine_command("capper_cap_container", {})

class Centrifuge1p5mL(BaseMachine, abc.ABC):
    """A centrifuge for 1.5ml tubes."""
    def __init__(self, location: Location = DEFAULT_CENTRIFUGE_P1500_LOCATION):
        super().__init__(location)

    def run(self, speed_rpm: int, duration_seconds: int) -> None:
        """Starts a centrifugation cycle."""
        if not self.containers_on_instrument:
            raise Exception("Centrifuge1p5mL cannot be run with empty containers")
        call_machine_command("centrifuge_start", {
            "time": duration_seconds, "speed": speed_rpm, "container_type": ContainerType.P1500
        })

class Centrifuge_200ulTubes(BaseMachine, abc.ABC):
    """A centrifuge for 200ul tubes."""
    def __init__(self, location: Location = DEFAULT_CENTRIFUGE_P200_LOCATION):
        super().__init__(location)

    def run(self, speed_rpm: int = 2500, duration_sec: int = 15) -> None:
        """Starts a centrifugation cycle."""
        if not self.containers_on_instrument:
            raise Exception("Centrifuge_200ulTubes cannot be run with empty containers")
        call_machine_command("centrifuge_start", {
            "time": duration_sec, "speed": speed_rpm, "container_type": ContainerType.P200
        })

class Fluorometer200ul(BaseMachine, ABC):
    """A fluorometer for 200µL tubes."""
    def __init__(self, location: Location = DEFAULT_FLUOROMETER_LOCATION):
        super().__init__(location)

    def calibrate(self, standard1_concentration: float, standard2_concentration: float, unit: str) -> None:
        pass

    def measure_concentration(self, sample_volume_ul: int = 10) -> List[float]:
        pass

    def measure_fluorescence(self) -> List[int]:
        """Measures raw fluorescence, returning a list of mock values."""
        if not self.containers_on_instrument:
            raise Exception("Fluorometer200ul cannot be run with empty containers")
        
        value = call_machine_command("fluorometer_measure", {
            "num_containers": len(self.containers_on_instrument)
        })
        return value
        
class Heater1_5mlTubes(BaseMachine, ABC):
    """A heating incubator for 1.5ml tubes."""
    def __init__(self, location: Location = DEFAULT_HEATER_LOCATION):
        super().__init__(location)

    def start(self, temperature_celsius: float, duration_minutes: Optional[float] = None) -> None:
        """Starts the heating process."""
        if not self.containers_on_instrument:
            raise Exception("Heater1_5mlTubes cannot be run with empty containers")
        call_machine_command("heater_start", {
            "time": duration_minutes * 60 if duration_minutes is not None else None,
            "temperature": temperature_celsius
        })

    def stop(self) -> None:
        pass

    def get_current_temperature(self) -> float:
        return 0.0
    
class HeaterShaker200uL(BaseMachine, ABC):
    """A heated shaker for 200ul tubes."""
    def __init__(self, location: Location = DEFAULT_HEATERSHAKER_LOCATION):
        super().__init__(location)
        
    def incubate(self, target_temperature_celsius: Optional[float] = None, target_speed_rpm: int = 1300, duration_seconds: Optional[int] = None) -> None:
        """Starts a heating and/or shaking protocol."""
        if not self.containers_on_instrument:
            raise Exception("HeaterShaker200uL cannot be run with empty containers")
        call_machine_command("heatershaker_start", {
            "time": duration_seconds, "temperature": target_temperature_celsius, "speed": target_speed_rpm
        })

    def stop(self) -> None:
        pass

    def get_current_temperature_celsius(self) -> float:
        return 0.0

    def get_current_speed_rpm(self) -> int:
        return 0

class MagRackP1500(BaseMachine, ABC):
    """A passive magnetic rack for 1.5ml tubes."""
    def __init__(self, location: Location = DEFAULT_MAGRACKP1500_LOCATION, model: str = "MS-08"):
        super().__init__(location)
        self.model = model

    def separate(self, wait_duration_seconds: int = 60) -> None:
        """Performs magnetic separation by waiting."""
        if not self.containers_on_instrument:
            raise Exception("MagRackP1500 cannot be run with empty containers")
        call_machine_command("magrack_wait", {"time": wait_duration_seconds})
        
class MagRackP200(BaseMachine, ABC):
    """A passive magnetic rack for 200ul tubes."""
    def __init__(self, location: Location = DEFAULT_MAGRACKP200_LOCATION):
        super().__init__(location)

    def separate(self, duration_seconds: int = 60) -> None:
        """Performs magnetic separation by waiting."""
        if not self.containers_on_instrument:
            raise Exception("MagRackP200 cannot be run with empty containers")
        call_machine_command("magrack_wait", {"time": duration_seconds})
        
class TemperatureStep(TypedDict):
    temperature_celsius: float
    duration_seconds: int

class Cycle(TypedDict):
    steps: List[TemperatureStep]
    count: int

ProtocolStep = Union[TemperatureStep, Cycle]

class ThermoCycler200ul(BaseMachine, abc.ABC):
    """A thermal cycler for 200ul PCR tubes."""
    def __init__(self, location: Location = DEFAULT_THERMAL_CYCLER_LOCATION):
        super().__init__(location)

    def pause(self) -> None: pass
    def resume(self) -> None: pass
    def stop(self) -> None: pass

    def open_lid(self) -> None:
        """Opens the heated lid to allow for loading or unloading of plates."""
        call_machine_command("thermal_cycler_open_lid", {
        })

    def close_lid(self) -> None:
        """Closes the heated lid."""
        call_machine_command("thermal_cycler_close_lid", {
        })

    def run_protocol(self, protocol: List[ProtocolStep]) -> None:
        """Starts a thermal cycling protocol."""
        if not self.containers_on_instrument:
            raise Exception("ThermoCycler200ul cannot be run with empty containers")
        call_machine_command("thermal_cycler_run_program", {"program": protocol})

class Pipette(BaseMachine, abc.ABC):
    """A robotic pipette for liquid handling."""
    def __init__(self, location: Location = DEFAULT_PIPETTE_LOCATION):
        super().__init__(location)

    def transfer(self, volume: float, source: Container, destination: Container, **kwargs) -> None:
        """Transfers liquid between containers."""
        if source.volume < volume and not source.replenishable:
            raise ValueError(f"Insufficient volume in source {source.label}. Required: {volume} uL, Available: {source.volume} uL.")
        # Exception: If the destination has "waste" in its label, allow overflow.
        if destination.volume + volume > destination.capacity and not ("waste" in destination.label.lower()):
            raise ValueError(f"Transfer will overflow destination {destination.label}. Current Volume: {destination.volume} uL, Capacity: {destination.capacity} uL.")
        
        source.volume -= volume
        destination.volume += volume

        call_machine_command("pipette_move", {
            "dst_container_index": destination.index, "src_container_index": source.index, "volume": volume
        })

    def mix(self, volume: float, location: Container, repetitions: int, **kwargs) -> None:
        """Mixes the contents of a container."""
        call_machine_command("pipette_mix", {
            "container_index": location.index, "num": repetitions, "volume": volume
        })
    
class Robot(BaseMachine, abc.ABC):
    """A laboratory robot arm for moving containers."""
    def __init__(self, location: Location = DEFAULT_ROBOT_LOCATION):
        super().__init__(location)
        
    def home(self) -> None:
        pass

    def move_container(self, container: Container, destination: Location) -> None:
        """Moves a container, updating the state of the source and destination instruments."""
        # Remove the container from the source machine's list, if the source is a known instrument.
        if container.location and container.location.description in INSTRUMENT_REGISTRY:
            source_machine = INSTRUMENT_REGISTRY[container.location.description]
            if container in source_machine.containers_on_instrument:
                source_machine.containers_on_instrument.remove(container)

        # Add the container to the destination machine's list.
        if destination.description in INSTRUMENT_REGISTRY:
            destination_machine = INSTRUMENT_REGISTRY[destination.description]
            destination_machine.containers_on_instrument.append(container)
        
        # Update the container's own location attribute.
        container.location = destination

        call_machine_command("robot_move_container", {
            "dst_pos": destination.description, "container_index": container.index
        })
        
    def open_port(self, port: Port) -> None:
        """
        Opens a specified port on a machine.

        Args:
            port: The Port object representing the port to be opened.
        """
        call_machine_command("open_port", {
            "port": port.port_type.name
        })

    def close_port(self, port: Port) -> None:
        """
        Closes a specified port on a machine.

        Args:
            port: The Port object representing the port to be closed.
        """
        call_machine_command("close_port", {
            "port": port.port_type.name
        })

            
    def open_port(self, port: Port) -> None:
        """
        Opens a specified port on a machine.

        Args:
            port: The Port object representing the port to be opened.
        """
        call_machine_command("open_port", {
            "port": port.port_type.name
        })

    def close_port(self, port: Port) -> None:
        """
        Closes a specified port on a machine.

        Args:
            port: The Port object representing the port to be closed.
        """
        call_machine_command("close_port", {
            "port": port.port_type.name
        })

DEFAULT_SEQUENCER_LOCATION = Location("sequencer")
class Sequencer(abc.ABC):
    """
    An abstract base class defining a simple, universal interface for
    controlling a DNA/RNA sequencer.

    This interface is designed to be minimal and intuitive, focusing on the
    three core actions of a sequencing workflow: starting, monitoring,
    and stopping a run.
    """
    def __init__(self, location: Location = DEFAULT_SEQUENCER_LOCATION):
        self.location = location

    def start_run(
        self, *, run_name: str, output_duirectory: str
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
        call_machine_command("start_sequencing")

    def stop_run(self) -> None:
        """
        Stops the currently active sequencing run gracefully.

        If a run is in progress, this command will terminate the sequencing
        process and finalize any data output. If no run is active, this
        method does nothing.
        """
        pass

DEFAULT_REFRIGERATOR_LOCATION = Location("refrigerator")
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

    def __init__(self, target_temperature: float = 4.0, location: Location = DEFAULT_REFRIGERATOR_LOCATION):
        """
        Initializes the Refrigerator.

        Args:
            location: The physical location of the refrigerator.
            target_temperature: The default target temperature in Celsius.
        """
        self.location = location
        self.target_temperature = target_temperature

    def get_current_temperature(self) -> float:
        """
        Returns the current internal temperature of the refrigerator in Celsius.
        """
        return self.target_temperature

    def get_contents(self) -> List[Container]:
        """
        Returns a list of all containers currently stored inside the refrigerator.
        """
        pass
    
    def set_target_temperature(self, celsius: float):
        """
        Sets the target temperature for the refrigerator's environment.

        Args:
            celsius: The desired target temperature in Celsius.
        """
        self.target_temperature = celsius
        pass

# --- Instrument Initialization ---
# These singleton instances are used directly in lab protocols.
# The BaseMachine constructor automatically registers them in INSTRUMENT_REGISTRY.
timer = Timer()
container_manager = ContainerManager()
pipette = Pipette()
robot = Robot()
capper = Capper_200ul_tubes()
centrifuge_1p5mL = Centrifuge1p5mL()
centrifuge_200uL = Centrifuge_200ulTubes()
fluorometer = Fluorometer200ul()
heater_1p5mL = Heater1_5mlTubes()
heater_shaker = HeaterShaker200uL()
mag_rack_1p5mL = MagRackP1500()
mag_rack_200uL = MagRackP200()
thermal_cycler = ThermoCycler200ul()
sequencer = Sequencer()
refrigerator = Refrigerator()
