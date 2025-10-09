#pragma once

#include "carrier.hpp"
// #include "reality/container/tube.hpp"

#include "common/dbug_logging.h"

template <uint32_t NUM>
class Tube;

class StripTubeCarrier : public Carrier {
    static constexpr uint32_t M = 12;

public:
    StripTubeCarrier(std::string name) : Carrier(name, TubeType::STRIP_TUBE) {
        for (int i = 0; i < M; i++) {
            tube_[i] = nullptr;
        }
    }

    bool setTube(Tube<8>* tube) {
        for (int i = 0; i < M; i++) {
            if (tube_[i] == nullptr) {
                tube_[i] = tube;
                return true;
            }
        }
        return false;
    }

    int getTubeIndex(Container* tube) const;

    int getOrAllocTubeIndex(Container* tube);

    bool forceAllocTubeIndex(Container* tube, int index);

    bool releaseTube(Container* tube);

    std::vector<uint16_t> getBitmap() const override;

    void markOtherIndexConsumer();

    void renewConsumer(MachineTypeId machine_type, AreaId area_id);

    std::vector<Container*> getContainers();

private:
    Tube<8>* tube_[M];
};