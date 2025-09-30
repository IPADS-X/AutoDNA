# AutoDNA - Automated DNA Laboratory System

AutoDNA is a comprehensive multi-component system designed for automated DNA and chemical experiments in laboratory environments. The system combines AI-powered protocol generation, web-based user interfaces, and hardware scheduling to create a complete end-to-end solution for molecular biology automation.

## 🧬 System Overview

AutoDNA consists of three main components working together to provide a seamless laboratory automation experience:

1. **UI** - Web-based frontend interface built on LobeChat framework
2. **AutoDNA-python** - AI-powered multi-agent system for protocol generation and execution
3. **Scheduler** - C++ scheduler for hardware coordination and workflow management

## 🏗️ Architecture

```
┌─────────────────┐    HTTP/WebSocket    ┌─────────────────┐    JSON Protocol    ┌─────────────────┐
│       UI        │ ◄──────────────────► │ AutoDNA-python  │ ◄─────────────────► │   Scheduler     │
│   (Frontend)    │                      │   (AI Engine)   │                    │  (C++ Backend)  │
└─────────────────┘                      └─────────────────┘                    └─────────────────┘
         │                                        │                                       │
         │                                        │                                       │
         ▼                                        ▼                                       ▼
┌─────────────────┐                      ┌─────────────────┐                    ┌─────────────────┐
│   User Interface│                      │   AI Agents     │                    │ Laboratory      │
│   - Workflow Mgmt│                      │   - Protocol    │                    │ Hardware        │
│   - Real-time   │                      │   - Literature  │                    │ - Machines      │
│   - Monitoring  │                      │   - Code Gen    │                    │ - Instruments   │
└─────────────────┘                      │   - Hardware    │                    │ - Reagents      │
                                        └─────────────────┘                    └─────────────────┘
```

## 🚀 Quick Start

### Prerequisites

- **Node.js 18+** (for UI component)
- **Python 3.8+** (for AutoDNA-python)
- **C++17 compatible compiler** (for Scheduler)
- **CMake 3.5+** (for building scheduler)
- **Google Gemini API key** (for AI functionality)

### Installation

#### 1. Clone the Repository
```bash
git clone <repository-url>
cd FORMAL
```

#### 2. Setup UI Component
```bash
cd UI
pnpm install
```

#### 3. Setup AutoDNA-python Component
```bash
cd ../AutoDNA-python
pip install -r requirements.txt
export GEMINI_API_KEY="your-gemini-api-key"
```

#### 4. Setup Scheduler Component
```bash
cd ../Scheduler
mkdir build && cd build
cmake -DCMAKE_BUILD_TYPE=Debug ..
make
```

### Running the System

#### Start the Components (in separate terminals)

1. **Start the Scheduler** (Terminal 1):
```bash
cd Scheduler/build
./bin/main_web ../config
```

2. **Start AutoDNA-python** (Terminal 2):
```bash
cd AutoDNA-python
python scientist/server.py
```

3. **Start the UI** (Terminal 3):
```bash
cd UI
pnpm dev
```

#### Access Points
- **UI Interface**: http://localhost:3010
- **AutoDNA API**: http://localhost:8081
- **Scheduler WebSocket**: http://localhost:8080

## 📋 Component Details

### 🔧 UI Component

The frontend provides an intuitive web interface for managing laboratory workflows.

**Key Features:**
- Multi-agent workflow orchestration
- Real-time status monitoring
- Interactive protocol design
- Result visualization and comparison

**Technology Stack:**
- Built on LobeChat framework
- React/Next.js frontend
- WebSocket communication
- Custom workflow routing system

**Directory Structure:**
```
UI/
├── src/custom/           # Custom AutoDNA components
│   ├── routing.ts        # Main workflow routing
│   ├── synthesis/        # DNA synthesis workflows
│   ├── sequence/         # Sequencing workflows
│   └── nuc_acid_test/    # Nucleic acid testing
├── src/components/       # UI components
└── src/app/             # Next.js app structure
```

### 🤖 AutoDNA-python Component

The AI engine that powers intelligent protocol generation and execution.

**Key Features:**
- Multi-agent AI system with specialized agents
- Literature search and analysis
- Protocol design and optimization
- Code generation for laboratory automation
- Hardware integration and control

**AI Agents:**
- **Literature Agent**: Scientific literature search and analysis
- **Protocol Agent**: Experimental protocol design
- **Reagent Agent**: Reagent inventory management
- **Code Agent**: Python code generation for automation
- **Hardware Agent**: Laboratory equipment interface
- **Hypothesis Agent**: Experimental hypothesis formulation

**Supported Experiment Types:**
- DNA Synthesis
- RPA (Rapid PCR Assay)
- RNA Experiments
- DNA Storage
- Amplification
- PolyA Tailing
- Sequencing
- More...

**Directory Structure:**
```
AutoDNA-python/
├── scientist/            # Main application
│   ├── agents/          # AI agent implementations
│   ├── llm/            # Language model configurations
│   ├── prompts/        # Agent prompts and templates
│   ├── executor/       # Code execution and correction
│   ├── ai_scientist.py # Main entry point
│   └── server.py       # FastAPI web server
├── system/             # System components
└── sequence_analysis/  # Analysis tools
```

### ⚙️ Scheduler Component

The C++ backend that manages laboratory hardware and workflow execution.

**Key Features:**
- Hardware coordination and scheduling
- Reagent allocation and management
- Real-time workflow monitoring
- WebSocket communication
- Modbus protocol support

**Supported Hardware:**
- Fluorescence readers
- PCR machines
- Liquid handling systems
- Temperature controllers
- Various laboratory instruments

**Directory Structure:**
```
Scheduler/
├── src/                # Source code
│   ├── main_scheduler.cpp  # Main entry point
│   ├── server/         # WebSocket and scheduler
│   ├── machine/        # Machine management
│   ├── procedure/      # Workflow definitions
│   └── modbus/         # Hardware communication
├── config/             # Configuration files
└── build/              # Build output
```

## 🔄 Workflow Process

1. **User Input**: User submits experimental requirements through the UI
2. **AI Processing**: AutoDNA-python agents analyze requirements and generate protocols
3. **Protocol Generation**: AI agents collaborate to create detailed experimental protocols
4. **Hardware Scheduling**: Scheduler receives protocol and schedules hardware operations
5. **Execution**: Laboratory hardware executes the protocol under scheduler control
6. **Monitoring**: Real-time status updates flow back through the system
7. **Results**: Experimental results are processed and displayed in the UI

## ⚙️ Configuration

### Environment Variables

```bash
# Required for AutoDNA-python
export GEMINI_API_KEY="your-gemini-api-key"

# Optional configurations
export SCHEDULER_ADDRESS="localhost:8080"
export OUTPUT_PATH="./output"
export INPUT_PATH="./input"
```

### Scheduler Configuration

The scheduler requires JSON configuration files in the `Scheduler/config/` directory:

- `reagents.json` - Reagent definitions and allocations
- `protocol_flow.json` - Protocol execution steps (generated by AutoDNA-python)
- Machine-specific configuration files

### AutoDNA-python Configuration

Key settings in `AutoDNA-python/scientist/config.py`:
```python
model_tag = "gemini-2.5-pro"
temperature = 1.0
scheduler_address = "localhost:8080"
```

## 🧪 Example Workflows

### DNA Synthesis Workflow
```bash
# Through UI: Navigate to synthesis workflow
# Through CLI:
cd AutoDNA-python
python scientist/ai_scientist.py --synthesis
```

### RPA (Rapid PCR Assay) Workflow
```bash
cd AutoDNA-python
python scientist/ai_scientist.py --rpa
```

### Custom Protocol via API
```bash
curl -X POST "http://localhost:8081/prompt" \
     -H "Content-Type: application/json" \
     -d '{"user_prompt": "Design a DNA synthesis protocol for creating a 100bp fragment"}'
```

## 🔧 Development

### Adding New Experiment Types

Just give user-prompts by UI!

### Adding New Agents

1. Create agent implementation in `AutoDNA-python/scientist/agents/`
2. Add agent prompts in `AutoDNA-python/scientist/prompts/`
3. Register agent in the main application
4. Update UI to handle new agent interactions

### Testing

```bash
# Test AutoDNA-python
cd AutoDNA-python
python scientist/ai_scientist.py --test

# Test scheduler locally
cd Scheduler/build
./bin/main_local_web ../config

# Test UI
cd UI
pnpm test
```

## 📊 Output and Results

The system generates comprehensive outputs:

- **Protocols**: Detailed step-by-step experimental procedures
- **Generated Code**: Executable Python code for laboratory automation
- **Reagent Lists**: Required reagents with availability status
- **Hardware Commands**: Equipment-specific instructions
- **Literature References**: Relevant scientific papers
- **Execution Logs**: Detailed logs of agent interactions

Results are saved in structured formats with intelligent caching for efficient re-execution.

## 🛠️ Troubleshooting

### Common Issues

1. **Connection Issues**: Ensure all components are running and ports are available
2. **API Key**: Verify GEMINI_API_KEY is set correctly
3. **Dependencies**: Check that all required libraries are installed
4. **Configuration**: Verify config files are present and properly formatted

### Debug Mode

```bash
# Enable debug logging in AutoDNA-python
python scientist/ai_scientist.py --test --debug

# Enable debug build for scheduler
cd Scheduler/build
cmake -DCMAKE_BUILD_TYPE=Debug ..
make
```

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch for each component
3. Follow existing code structure and conventions
4. Add appropriate tests and documentation
5. Submit pull requests for each component separately

### Code Standards
- **Python**: Follow PEP 8 for AutoDNA-python
- **C++**: Follow project-specific conventions for Scheduler
- **TypeScript/React**: Follow LobeChat conventions for UI

## 📄 License

This project is licensed under the terms specified in the LICENSE file.

## 🔬 Scientific Applications

AutoDNA is designed for:

- **Research Laboratories**: Automated protocol design and execution
- **Biotechnology Companies**: Streamlined experimental workflows
- **Educational Institutions**: Teaching molecular biology concepts
- **Diagnostic Labs**: Rapid assay development and testing
- **Synthetic Biology**: Automated DNA synthesis and manipulation

## 🆘 Support

For questions, issues, or contributions:
- Check component-specific README files
- Review troubleshooting section
- Create issues in the repository
- Refer to scientific documentation

---

**Note**: This system is designed for research and educational purposes. Always follow proper laboratory safety protocols and regulations when using automated systems.
