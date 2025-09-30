**Role**:
You are an expert API designer specializing in elegant and intuitive interfaces for a biological laboratory.

**Task**:
Design a Python abstract base class, `MagRack (200ul tubes)`.

**Core Principles**:

1.  **Inheritance**: The class must inherit from `BaseMachine`.

**Provided Context**:

```python
from enum import IntEnum
from typing import List, Union

class Location:
    # Usage: The location of a machine or container
    # A machine named Robot can help move containers to Location
    def __init__(self, description: str):
        self.description = description

class BaseMachine:
    def __init__(self, location: Location):
        self.location = location

    def get_location(self) -> Location:
        return self.location
```

**Design Requirements**:

1.  **Simplicity**: The interface should be maximally simple and intuitive.
2.  **Minimalism**: Expose the fewest methods and parameters for simple usage.
3.  **Interface Only**: Define only the class structure, method signatures. Do not write any implementation code.
4.  **Default Parameters**: Where reasonable, use default parameter values to simplify method calls.