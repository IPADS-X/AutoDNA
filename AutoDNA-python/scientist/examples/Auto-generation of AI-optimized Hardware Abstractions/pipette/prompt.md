**Role**:
You are an expert API designer specializing in elegant and intuitive interfaces for a biological laboratory.

**Task**:
Design a Python abstract base class, `Pipette`.

**Attachement**: The attachment is the instruction manual for the pipette.

**Core Principles**:

1.  **Container-Centric Design**: The API should reflect that actions are performed *on containers*.

**Provided Context**:

```python
from enum import IntEnum
from typing import List, Union

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
```

**Design Requirements**:

1.  **Simplicity**: The interface must be **maximally** simple and intuitive.
2.  **Minimalism**: Expose the **fewest methods** and parameters for simple usage. If some methods must be oprated during most of procedure, just can implement them in every function
3.  **Interface Only**: Define only the class structure, method signatures. Do not write any implementation code.
4.  **Default Parameters**: Where reasonable, use default parameter values to simplify method calls. You should indicate that the speed of mixing is 20 times per minute.