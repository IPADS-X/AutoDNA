#include "machine/machine.hpp"
#include "procedure/step.hpp"

std::shared_ptr<spdlog::logger> Machine::logger = nullptr;

IndexId Machine::allocEquipment(EquipmentType equipment, CheckType checkType,
                                std::shared_ptr<Step>      step      = nullptr,
                                std::shared_ptr<Container> container = nullptr) {
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

    IndexId index = it->second->alloc(container, step);
    logger->debug("Allocated equipment: {} of machine: {} to step: {}",
                  magic_enum::enum_name(equipment), getName(), step->getId());
    return index;
}

bool Machine::releaseEquipment(EquipmentType equipment, std::shared_ptr<Step> step = nullptr,
                               std::shared_ptr<Container> container = nullptr) {
    auto it = all_equipments_.find(equipment);
    if (it != all_equipments_.end()) {
        logger->debug("Realeased equipment: {} of machine: {}",
                  magic_enum::enum_name(equipment), getName());
        return it->second->release(container);
    }
    return false;
}
