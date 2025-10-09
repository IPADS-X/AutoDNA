from scheduler import call_machine_command, container_id_to_type_map
from abc import ABC, abstractmethod
from enum import IntEnum
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
        self.type = capacity
        self.label = label
        self.capped = False
        self.replenishable = replenishable
        self.location = location

# --- Default Locations ---
DEFAULT_CONTAINERHOLDER_LOCATION = Location("container_holder")
DEFAULT_CAPPER_LOCATION = Location("capper")
DEFAULT_CENTRIFUGE_P1500_LOCATION = Location("centrifuge")
DEFAULT_CENTRIFUGE_P200_LOCATION = Location("centrifuge")
DEFAULT_FLUOROMETER_LOCATION = Location("fluorometer")
DEFAULT_HEATER_LOCATION = Location("heater")
DEFAULT_HEATERSHAKER_LOCATION = Location("heater_shaker")
DEFAULT_MAGRACKP1500_LOCATION = Location("magrack")
DEFAULT_MAGRACKP200_LOCATION = Location("magrack")
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
        if 'waste' in label.lower():
            label = 'Waste Tube'
        container = Container(index = index, capacity = cap, volume = 0, label = label, location = DEFAULT_CONTAINERHOLDER_LOCATION)
        self.containerList.append(container)
        ContainerManager._nextContainerIndex += 1
        container_id_to_type_map[container.index] = container.type
        call_machine_command("container_allocate", {
            "container_index": container.index,
            "container_label": container.label
        })
        return container

    def getContainerForReplenish(self, name: str, required_volume: float = 0) -> Container:
        """Gets a container that can be automatically replenished with a reagent."""
        if 'waste' in name.lower():
            name = 'Waste Tube'
        container = Container(index = ContainerManager._nextContainerIndex, capacity = ContainerType.P200 if required_volume <= 200 else ContainerType.P50K, volume = required_volume, label = name)
        self.containerList.append(container)
        ContainerManager._nextContainerIndex += 1
        container_id_to_type_map[container.index] = container.type
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
        call_machine_command("capper_cap_container", {})

class Centrifuge1p5mL(BaseMachine, abc.ABC):
    """A centrifuge for 1.5ml tubes."""
    def __init__(self, location: Location = DEFAULT_CENTRIFUGE_P1500_LOCATION):
        super().__init__(location)

    def run(self, speed_rpm: int, duration_seconds: int) -> None:
        """Starts a centrifugation cycle."""
        call_machine_command("centrifuge_start", {
            "time": duration_seconds, "speed": speed_rpm, "container_type": ContainerType.P1500
        })

class Centrifuge_200ulTubes(BaseMachine, abc.ABC):
    """A centrifuge for 200ul tubes."""
    def __init__(self, location: Location = DEFAULT_CENTRIFUGE_P200_LOCATION):
        super().__init__(location)

    def run(self, speed_rpm: int = 2500, duration_sec: int = 15) -> None:
        """Starts a centrifugation cycle."""
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
        num_containers = len(self.containers_on_instrument) if self.containers_on_instrument else 0
        value = call_machine_command("fluorometer_measure", {
            "num_containers": num_containers
        })
        return value
        
class Heater1_5mlTubes(BaseMachine, ABC):
    """A heating incubator for 1.5ml tubes."""
    def __init__(self, location: Location = DEFAULT_HEATER_LOCATION):
        super().__init__(location)

    def start(self, temperature_celsius: float, duration_minutes: Optional[float] = None) -> None:
        """Starts the heating process."""
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
        call_machine_command("magrack_wait", {"time": wait_duration_seconds})
        
class MagRackP200(BaseMachine, ABC):
    """A passive magnetic rack for 200ul tubes."""
    def __init__(self, location: Location = DEFAULT_MAGRACKP200_LOCATION):
        super().__init__(location)

    def separate(self, duration_seconds: int = 60) -> None:
        """Performs magnetic separation by waiting."""
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

    def open_lid(self) -> None: pass
    def close_lid(self) -> None: pass
    def set_lid_temperature(self, temperature_celsius: float = 105.0) -> None: pass
    def pause(self) -> None: pass
    def resume(self) -> None: pass
    def stop(self) -> None: pass

    def run_protocol(self, protocol: List[ProtocolStep]) -> None:
        """Starts a thermal cycling protocol."""
        call_machine_command("thermal_cycler_run_program", {"program": protocol})

class Pipette(BaseMachine, abc.ABC):
    """A robotic pipette for liquid handling."""
    def __init__(self, location: Location = DEFAULT_PIPETTE_LOCATION):
        super().__init__(location)

    def transfer(self, volume: float, source: Container, destination: Container, **kwargs) -> None:
        """Transfers liquid between containers."""
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
