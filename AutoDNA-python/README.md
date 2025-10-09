# AutoDNA Python

AutoDNA Python is a sophisticated multi-agent AI system designed for automated DNA and chemical experiments. It leverages specialized AI agents to collaborate and execute complex experimental workflows, from protocol design to code generation and execution.

## 🧬 Overview

AutoDNA Python is built on a multi-agent architecture where different AI agents specialize in various aspects of scientific experimentation:

- **Literature Agent**: Searches and analyzes scientific literature
- **Protocol Agent**: Designs experimental protocols and workflows
- **Reagent Agent**: Manages reagent inventory and availability
- **Code Agent**: Generates executable Python code for laboratory automation
- **Hardware Agent**: Interfaces with laboratory hardware and equipment
- **Hypothesis Agent**: Formulates and tests experimental hypotheses

## ✨ Key Features

### Multi-Agent Collaboration
- **Intelligent Task Decomposition**: Automatically breaks down complex experiments into manageable stages
- **Specialized Agents**: Each agent handles specific aspects of the experimental process
- **Sequential Execution**: Coordinates multi-stage experiments with proper data flow between stages

### Experiment Types
- **DNA Synthesis**: Automated DNA synthesis workflows
- **RPA-based NAT**: Nucleic Acid Test experiments using RPA (Recombinase Polymerase Amplification)
- **RNA Experiments**: RNA-related experimental procedures
- **DNA Storage write operation**: Information storage using DNA
- **DNA Storage read operation**: Retrieval of information stored in DNA
- **PolyA Tailing**: RNA polyadenylation procedures
- **More**: ...


## 🚀 Quick Start

### Prerequisites

- Python 3.8+
- Google Gemini API key
- Scheduler setup 

### Installation

1. Clone the repository:
```bash
git clone <repository-url>
cd AutoDNA-python
```

2. Install dependencies:
```bash
pip install -r requirements.txt
```

3. Set up environment variables:
```bash
export GEMINI_API_KEY="your-gemini-api-key"
```

### Basic Usage

#### Command Line Interface

First, remove `scientist/output/` folder if it exists.
Run experiments using the command line interface:

```bash
cd scientist
# RPA-based NAT experiment
python ai_scientist.py --no_filtering --rpa

# RNA experiment
python ai_scientist.py --no_filtering --rna

# DNA synthesis experiment
python ai_scientist.py --no_filtering --synthesis

# DNA storage write operation
python ai_scientist.py --no_filtering --write

# DNA storage read operation
python ai_scientist.py --no_filtering --read

# Test mode (for customized testing)
python ai_scientist.py --test
```

#### Web Server Interface

First, remove `scientist/output/` folder if it exists.
Start the FastAPI server for web-based interaction:

```bash
cd scientist
python server.py
```

The server will be available at `http://localhost:8081`

**API Endpoints:**
- `POST /prompt`: Submit experimental prompts
- `GET /heartbeat`: Check system status

#### Example API Usage

Refer to `http://localhost:8081/docs` for interactive API documentation once the server is running.

## 📁 Project Structure

```
AutoDNA-python/
├── scientist/                 # Main application directory
│   ├── agents/               # AI agent implementations
│   │   ├── Code.py          # Code generation agent
│   │   ├── Protocol.py      # Protocol design agent
│   │   ├── Literature.py    # Literature search agent
│   │   ├── Reagent.py       # Reagent management agent
│   │   ├── Hardware.py      # Hardware interface agent
│   │   └── Hypothesis.py    # Hypothesis generation agent
│   ├── llm/                 # Language model configurations
│   ├── prompts/             # Agent prompts and templates
│   ├── tools/               # Utility tools and helpers
│   ├── executor/            # Code execution and correction
│   ├── input/               # Input files and configurations
│   ├── output/              # Experiment results and logs
│   ├── ai_scientist.py      # Main application entry point
│   ├── server.py            # FastAPI web server
│   └── config.py            # Configuration settings
└── system/                  # System components
    └── reagent_system/      # Reagent management system
```

## ⚙️ Configuration

### Experiment Modes

Configure experiment types using command-line flags:

| Flag | Description |
|------|-------------|
| `--synthesis` | DNA synthesis experiments |
| `--rpa` | RPA-based NAT experiments |
| `--rna` | RNA-related experiments |
| `--write` | DNA storage write operations |
| `--read` | DNA storage read operations |
| `--polya` | PolyA tailing experiments |
| `--test` | Test mode for customized testing |

### System Options

| Flag | Description |
|------|-------------|
| `--no_filtering` | Disable protocol input filtering. Set this flag if you are not sure |
| `--multiple` | Enable parallel model calls |

### Environment Configuration

Key configuration settings in `scientist/config.py`:

```python
# Model configurations
model_tag = "gemini-2.5-pro"
temperature = 1.0

# Scheduler configurations (you MUST set the scheduler config path accordingly)
scheduler_address = "localhost:8080"
scheduler_config_path = "<your_path_to_scheduler_config>"
```


## 📊 Output and Results

The system generates comprehensive outputs including:

- **Agent outputs**: Detailed outputs from each AI agent
- **Execution Logs**: Detailed logs of agent interactions and decisions

Results are saved in the `output/` directory and most of them will be shown in the web UI if you run the server.


## 🔬 Scientific Applications

AutoDNA Python is designed for:

- **Research Laboratories**: Automated protocol design and execution
- **Biotechnology Companies**: Streamlined experimental workflows
- **Educational Institutions**: Teaching and learning molecular biology
- **Synthetic Biology**: Automated DNA synthesis and manipulation

---

**Note**: This system is designed for research and educational purposes. Always follow proper laboratory safety protocols and regulations when using automated systems.
