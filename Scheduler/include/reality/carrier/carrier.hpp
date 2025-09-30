#pragma once

#include "machine/common.hpp"
#include "reality/container/container.hpp"

#include <string>
#include <vector>

class Carrier {
public:
    inline static const std::string NAME = "CarrierSN";

    Carrier(std::string name, TubeType type)
        : name_(name), type_(type), machine_type_((MachineTypeId)MachineType::TOTAL_NUM) {}

    std::string getName() const { return name_; }

    void setMachineType(MachineTypeId machine_type) { machine_type_ = machine_type; }

    void setAreaId(AreaId area_id) { area_id_ = area_id; }

    MachineTypeId getMachineType() const { return machine_type_; }

    AreaId getAreaId() const { return area_id_; }

    int getTubeIndex(Container* tube) const;

    int getNextAvailableIndex() const;

    int getOrAllocTubeIndex(Container* tube);

    bool forceAllocTubeIndex(Container* tube, int index);

    bool releaseTube(Container* tube);

    virtual std::vector<uint16_t> getBitmap() const = 0;

    bool isConsumer() { return is_consumer_; }

    void setIsConsumer(bool is_consumer) { is_consumer_ = is_consumer; }

    void markOtherIndexConsumer();

    void renewConsumer(MachineTypeId machine_type, AreaId area_id);

    std::vector<Container*> getContainers();
protected:
    std::string   name_;
    MachineTypeId machine_type_;
    TubeType      type_;
    AreaId        area_id_ = -1;

    // can be consumed by tube manager
    bool             is_consumer_     = false;
    std::vector<int> consumer_indexs_ = {};
};