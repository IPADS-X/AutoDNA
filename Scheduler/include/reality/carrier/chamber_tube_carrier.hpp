#pragma once

#include "carrier.hpp"
#include "reality/container/container.hpp"

#include "common/dbug_logging.h"

template <uint32_t NUM>
class Tube;

class Carrier;

class ChamberTubeCarrier : public Carrier {
    static constexpr uint32_t M = 12;

public:
    ChamberTubeCarrier(std::string name) : Carrier(name, TubeType::CHAMBER) { tube_[0] = nullptr; }

    bool setTube(Tube<1>* tube) { tube_[0] = tube; return true; }

    int getTubeIndex(Container* tube) const;

    int getOrAllocTubeIndex(Container* tube);

    bool forceAllocTubeIndex(Container* tube, int index);

    bool releaseTube(Container* tube);

    std::vector<uint16_t> getBitmap() const override;

    void markOtherIndexConsumer();

    void renewConsumer(MachineTypeId machine_type, AreaId area_id);

    std::vector<Container*> getContainers();

private:
    Tube<1>* tube_[M];
};