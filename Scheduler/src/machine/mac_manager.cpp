#include "machine/mac_manager.hpp"
#include <memory>

std::shared_ptr<spdlog::logger> MachineManager::logger = nullptr;

void MachineManager::addMachine(std::shared_ptr<Machine> machine) {
    MachineTypeId type = machine->getType();
    if (machines_.find(type) != machines_.end()) {
        throw std::runtime_error("Machine already exists");
    }
    machines_[type] = machine;
    machine->init();
}