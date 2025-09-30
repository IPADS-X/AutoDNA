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
- **RPA (Rapid PCR Assay)**: Rapid diagnostic testing protocols
- **RNA Experiments**: RNA-related experimental procedures
- **DNA Storage**: Information storage and retrieval using DNA
- **Amplification**: DNA amplification protocols
- **PolyA Tailing**: RNA polyadenylation procedures
- **Sequencing**: DNA/RNA sequencing workflows
- **More**: ...

### Advanced Capabilities
- **Smart Caching**: Intelligent caching system for experiment results and intermediate steps
- **Error Correction**: Automated error detection and correction mechanisms
- **Hardware Integration**: Direct integration with laboratory automation systems
- **Literature Integration**: Real-time access to scientific literature and protocols

## 🚀 Quick Start

### Prerequisites

- Python 3.8+
- Google Gemini API key (for AI models)
- Laboratory hardware setup (for live mode)

### Installation

1. Clone the repository:
```bash
git clone <repository-url>
cd AutoDNA-python
```

2. Install dependencies:
```bash
pip install -r requirements.txt  # Create this file with your dependencies
```

3. Set up environment variables:
```bash
export GEMINI_API_KEY="your-gemini-api-key"
```

### Basic Usage

#### Command Line Interface

Run experiments using the command line interface:

```bash
# DNA synthesis experiment
python scientist/ai_scientist.py --synthesis

# RPA (Rapid PCR Assay) experiment
python scientist/ai_scientist.py --rpa

# DNA storage experiment
python scientist/ai_scientist.py --storage

# Test mode (for development)
python scientist/ai_scientist.py --test
```

#### Web Server Interface

Start the FastAPI server for web-based interaction:

```bash
python scientist/server.py
```

The server will be available at `http://localhost:8081`

**API Endpoints:**
- `POST /prompt`: Submit experimental prompts
- `GET /heartbeat`: Check system status

#### Example API Usage

```bash
# Submit an experiment prompt
curl -X POST "http://localhost:8081/prompt" \
     -H "Content-Type: application/json" \
     -d '{"user_prompt": "Design a DNA synthesis protocol for creating a 100bp fragment"}'

# Check system status
curl "http://localhost:8081/heartbeat"
```

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
├── system/                  # System components
│   └── reagent_system/      # Reagent management system
├── sequence_analysis/       # Sequence analysis tools
└── atomic_service/          # Atomic service components
```

## ⚙️ Configuration

### Experiment Modes

Configure experiment types using command-line flags:

| Flag | Description |
|------|-------------|
| `--synthesis` | DNA synthesis experiments |
| `--rpa` | RPA (Rapid PCR Assay) experiments |
| `--rna` | RNA-related experiments |
| `--storage` | DNA storage experiments |
| `--amplification` | DNA amplification experiments |
| `--write` | DNA storage write operations |
| `--read` | DNA storage read operations |
| `--polya` | PolyA tailing experiments |
| `--test` | Test mode for development |

### System Options

| Flag | Description |
|------|-------------|
| `--no_filtering` | Disable protocol input filtering |
| `--multiple` | Enable parallel model calls |

### Environment Configuration

Key configuration settings in `scientist/config.py`:

```python
# Model configurations
model_tag = "gemini-2.5-pro"
temperature = 1.0

# Path configurations
base_path = "."
output_path = "output"
input_path = "input"

# Hardware configurations
scheduler_address = "localhost:8080"
```

## 🔧 Development

### Adding New Agents

1. Create a new agent file in `scientist/agents/`
2. Implement the agent using the `@tool` decorator
3. Add the agent to the tools list in `ai_scientist.py`
4. Create corresponding prompts in `scientist/prompts/agents/`

### Custom Experiment Types

1. Add new experiment type to the `ExperimentType` enum
2. Implement experiment-specific logic in `choose_toolset()` and `choose_user_prompt()`
3. Create experiment-specific prompts
4. Add command-line argument handling

### Testing

Run tests using the test mode:

```bash
python scientist/ai_scientist.py --test --mock_mode
```

## 📊 Output and Results

The system generates comprehensive outputs including:

- **Experiment Protocols**: Detailed step-by-step procedures
- **Generated Code**: Executable Python code for laboratory automation
- **Reagent Lists**: Required reagents with availability status
- **Hardware Commands**: Equipment-specific instructions
- **Literature References**: Relevant scientific papers and protocols
- **Execution Logs**: Detailed logs of agent interactions and decisions

Results are saved in the `output/` directory with structured caching for efficient re-execution.

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests for new functionality
5. Submit a pull request

## 📄 License

This project is licensed under the terms specified in the LICENSE file.

## 🆘 Support

For questions, issues, or contributions, please refer to the project documentation or create an issue in the repository.

## 🔬 Scientific Applications

AutoDNA Python is designed for:

- **Research Laboratories**: Automated protocol design and execution
- **Biotechnology Companies**: Streamlined experimental workflows
- **Educational Institutions**: Teaching and learning molecular biology
- **Diagnostic Labs**: Rapid assay development and testing
- **Synthetic Biology**: Automated DNA synthesis and manipulation

---

**Note**: This system is designed for research and educational purposes. Always follow proper laboratory safety protocols and regulations when using automated systems.
