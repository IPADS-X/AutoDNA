#include "reality/container/tube.hpp"
#include "reality/carrier/pcr_tube_carrier.hpp"
#include "reality/carrier/strip_tube_carrier.hpp"

TubePosition TubeManager::getTubeReactionPosition(TubeId id, TubePositionType type,
                                                  IndexId index = 0) const {
    auto tube = getTube(id);
    if (!tube) {
        throw std::runtime_error("Tube not found");
    }

    TubePosition pos;
    if (TubeManager::getTubeType(tube) == TubeType::PCR_TUBE) {
        auto carrier     = TubeManager::toPcrTube(tube)->getTubeCarrier();
        pos.machine_type = (MachineType)carrier->getMachineType();
        pos.area_id      = carrier->getAreaId();
        pos.index        = carrier->getOrAllocTubeIndex(tube.get());
    } else if (TubeManager::getTubeType(tube) == TubeType::CHAMBER) {
        auto chamber_carrier = TubeManager::toChamberTube(tube)->getTubeCarrier();
        pos.machine_type     = (MachineType)chamber_carrier->getMachineType();
        pos.area_id          = chamber_carrier->getAreaId();
        pos.index            = chamber_carrier->getOrAllocTubeIndex(tube.get());
    } else {
        auto carrier     = TubeManager::toStripTube(tube)->getTubeCarrier();
        pos.machine_type = (MachineType)carrier->getMachineType();
        pos.area_id      = carrier->getAreaId();
        if (type == TubePositionType::WHOLE_TUBE) {
            pos.index = carrier->getOrAllocTubeIndex(tube.get());
        } else {
            pos.index = carrier->getOrAllocTubeIndex(tube.get()) * 8 + index % 8;
        }
    }
    return pos;
}

Carrier* TubeManager::getTubeCarrier(std::shared_ptr<Container> tube) {
    if (!tube) {
        throw std::runtime_error("Tube not found");
    }
    if (TubeManager::getTubeType(tube) == TubeType::PCR_TUBE) {
        return TubeManager::toPcrTube(tube)->getTubeCarrier();
    } else if (TubeManager::getTubeType(tube) == TubeType::CHAMBER) {
        return TubeManager::toChamberTube(tube)->getTubeCarrier();
    } else {
        return TubeManager::toStripTube(tube)->getTubeCarrier();
    }
}

void TubeManager::setTubeCarrier(std::shared_ptr<Container> tube,
                                 std::shared_ptr<Carrier>   carrier) {
    if (!tube) {
        throw std::runtime_error("Tube not found");
    }
    if (TubeManager::getTubeType(tube) == TubeType::PCR_TUBE) {
        TubeManager::toPcrTube(tube)->setTubeCarrier(carrier);
    } else if (TubeManager::getTubeType(tube) == TubeType::CHAMBER) {
        TubeManager::toChamberTube(tube)->setTubeCarrier(carrier);
    } else {
        TubeManager::toStripTube(tube)->setTubeCarrier(carrier);
    }
}