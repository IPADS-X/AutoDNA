# AutoDNA Frontend

AutoDNA Frontend is a sophisticated frontend application built on the LobeChat framework that orchestrates multiple AI agents to complete complex chemical tasks. The system provides an interactive interface for managing and monitoring automated chemical workflows.

## Overview

AutoDNA leverages a multi-agent architecture where specialized AI agents collaborate to execute chemical tasks.

## Features

## Getting Started

### Prerequisites
- Node.js 18+ 
- pnpm package manager
- Modern web browser

### Installation

1. Clone the repository:
```bash
git clone <repository-url>
cd lobe-chat
```

2. Install dependencies:
```bash
pnpm install
```

3. Start the development server:
```bash
pnpm dev
```

The application will be available at `http://localhost:3010` (default port is 3010, not 3000).

### Usage

1. Open your browser and navigate to `http://localhost:3010`
2. The interface will display the AutoDNA workflow management system
3. Clear any red alert messages in the bottom-left corner if present
4. Close the input box to begin using the system

## Architecture

### Custom Components (`src/custom/`)

The application extends the base LobeChat framework with custom components:

- **`routing.ts`**: Main routing and state management for all workflows
- **`diffviewer.tsx`**: Custom diff viewer component for comparing results

### Agent Communication

The system uses a sophisticated state management approach:
- Global state tracking for workflow progress
- Event-driven communication between agents
- Real-time status updates via WebSocket connections
- HTTP API integration for backend communication

### Backend Integration

AutoDNA communicates with backend services for:
- Task submission and management
- Real-time status polling
- Result processing and storage
- Agent coordination

## Development

### Project Structure
```
src/custom/
├── routing.ts              # Main workflow routing
├── synthesis/              # Synthesis workflow
│   ├── synthesis_routing.ts
│   ├── pre_synthesis.ts
│   ├── encoding.ts
│   ├── workflow.ts
│   ├── coding.ts
│   └── run_code.ts
├── sequence/               # Sequencing workflow
│   ├── sequence_routing.ts
│   ├── pre_sequence.ts
│   ├── craft.ts
│   ├── decoding.ts
│   ├── coding.ts
│   └── run_code.ts
└── nuc_acid_test/          # Nucleic acid testing sample
```

### Available Scripts

- `pnpm dev`: Start development server
- `pnpm build`: Build for production
- `pnpm start`: Start production server
- `pnpm lint`: Run linting
- `pnpm test`: Run tests

## Configuration

The application can be configured through environment variables and the custom routing system. Key configuration points:

- Backend API endpoints
- Agent communication protocols
- Workflow timing and intervals
- Testing parameters

## Contributing

This is a specialized frontend for AutoDNA chemical task automation. When contributing:

1. Follow the existing code structure in `src/custom/`
2. Maintain consistency with the multi-agent architecture
3. Test workflow integrations thoroughly
4. Update documentation for new agent types or workflows

## License

This project is built on the LobeChat framework and follows the same licensing terms.

## Support

For issues related to AutoDNA functionality, please refer to the custom components in `src/custom/` and the workflow routing system.
