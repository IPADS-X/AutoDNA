#include "machine/machine.hpp"
#include "procedure/step.hpp"
#include "procedure/workflow.hpp"

std::shared_ptr<spdlog::logger> Machine::logger = nullptr;

IndexId Machine::allocEquipment(EquipmentType equipment, CheckType checkType,
                                std::shared_ptr<Workflow>  original_workflow,
                                std::shared_ptr<Step>      step      = nullptr,
                                std::shared_ptr<Container> container = nullptr, bool is_all = false) {
    auto it = all_equipments_.find(equipment);
    if (it == all_equipments_.end()) {
        logger->warn("Equipment: {} not found in machine: {}", magic_enum::enum_name(equipment),
                     getName());
        return (IndexId)CommonIndexId::NOT_SUCCESS;
    }
    if (!it->second->isAvailable()) {
        logger->debug("Equipment: {} in machine: {} is not available",
                      magic_enum::enum_name(equipment), getName());
        return (IndexId)CommonIndexId::NOT_SUCCESS;
    }

    if (checkType == CheckType::CHECKONLY) {
        // just return
        return 0;
    }

    // retrieve pre alloc
    if (original_workflow->isPreAlloc()) {
        logger->debug("Allocated equipment: {} of machine: {} to step: {}",
                  magic_enum::enum_name(equipment), getName(), step->getId());
        return it->second->allocAll(original_workflow);
    }
    IndexId index = it->second->alloc(container, step);
    logger->debug("Allocated equipment: {} of machine: {} to step: {}",
                  magic_enum::enum_name(equipment), getName(), step->getId());
    return index;
}

bool Machine::releaseEquipment(EquipmentType equipment, std::shared_ptr<Workflow> original_workflow,
                               std::shared_ptr<Step>      step      = nullptr,
                               std::shared_ptr<Container> container = nullptr, bool is_all = false) {
    auto it = all_equipments_.find(equipment);
    if (it != all_equipments_.end()) {
        logger->debug("Realeased equipment: {} of machine: {}", magic_enum::enum_name(equipment),
                      getName());
        if (original_workflow->isPreAlloc()) {
            if (is_all) {
                return it->second->releaseAll(original_workflow);
            } else {
                return true;
            }
        }
        return it->second->release(container);
    }
    return false;
}
