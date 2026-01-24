from enum import Enum, StrEnum




class AgentName(StrEnum):
    """
    Enum for agent names.
    """
    Literature = "Literature"
    Planner = "Planner"
    Reagent = "Reagent"
    Hypothesis = "Hypothesis"
    Protocol = "Protocol"
    Code = "Code"
    Hardware = "Hardware"

class AgentOutputPrefix(StrEnum):
    """
    Enum for agent output file name prefix.
    """
    LRA_OUTPUT = "Literature"
    EPA_OUTPUT = "Planner"
    RMA_OUTPUT = "Reagent"
    HPA_OUTPUT = "Hypothesis"
    PGA_OUTPUT = "Protocol"
    PDA_OUTPUT = "Code"
    HEVA_OUTPUT = "Hardware"


