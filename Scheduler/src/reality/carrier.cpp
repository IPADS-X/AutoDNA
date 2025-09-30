#include "reality/carrier/chamber_tube_carrier.hpp"
#include "reality/carrier/pcr_tube_carrier.hpp"
#include "reality/carrier/strip_tube_carrier.hpp"
#include "reality/container/tube.hpp"

#include "common/dbug_logging.h"

#include <vector>

int Carrier::getTubeIndex(Container* tube) const {
    if (type_ == TubeType::PCR_TUBE) {
        return static_cast<const PcrTubeCarrier*>(this)->getTubeIndex(tube);
    } else if (type_ == TubeType::CHAMBER) {
        return static_cast<const ChamberTubeCarrier*>(this)->getTubeIndex(tube);
    } else {
        return static_cast<const StripTubeCarrier*>(this)->getTubeIndex(tube);
    }
}

int Carrier::getOrAllocTubeIndex(Container* tube) {
    if (type_ == TubeType::PCR_TUBE) {
        return static_cast<PcrTubeCarrier*>(this)->getOrAllocTubeIndex(tube);
    } else if (type_ == TubeType::CHAMBER) {
        return static_cast<ChamberTubeCarrier*>(this)->getOrAllocTubeIndex(tube);
    } else {
        return static_cast<StripTubeCarrier*>(this)->getOrAllocTubeIndex(tube);
    }
}

bool Carrier::forceAllocTubeIndex(Container* tube, int index) {
    if (type_ == TubeType::PCR_TUBE) {
        return static_cast<PcrTubeCarrier*>(this)->forceAllocTubeIndex(tube, index);
    } else if (type_ == TubeType::CHAMBER) {
        return static_cast<ChamberTubeCarrier*>(this)->forceAllocTubeIndex(tube, index);
    } else {
        return static_cast<StripTubeCarrier*>(this)->forceAllocTubeIndex(tube, index);
    }
}

bool Carrier::releaseTube(Container* tube) {
    if (type_ == TubeType::PCR_TUBE) {
        return static_cast<PcrTubeCarrier*>(this)->releaseTube(tube);
    } else if (type_ == TubeType::CHAMBER) {
        return static_cast<ChamberTubeCarrier*>(this)->releaseTube(tube);
    } else {
        return static_cast<StripTubeCarrier*>(this)->releaseTube(tube);
    }
}

void Carrier::markOtherIndexConsumer() {
    if (type_ == TubeType::PCR_TUBE) {
        static_cast<PcrTubeCarrier*>(this)->markOtherIndexConsumer();
    } else if (type_ == TubeType::CHAMBER) {
        static_cast<ChamberTubeCarrier*>(this)->markOtherIndexConsumer();
    } else {
        static_cast<StripTubeCarrier*>(this)->markOtherIndexConsumer();
    }
}

void Carrier::renewConsumer(MachineTypeId machine_type, AreaId area_id) {
    if (type_ == TubeType::PCR_TUBE) {
        static_cast<PcrTubeCarrier*>(this)->renewConsumer(machine_type, area_id);
    } else if (type_ == TubeType::CHAMBER) {
        static_cast<ChamberTubeCarrier*>(this)->renewConsumer(machine_type, area_id);
    } else {
        static_cast<StripTubeCarrier*>(this)->renewConsumer(machine_type, area_id);
    }
}

std::vector<Container*> Carrier::getContainers(){
    if (type_ == TubeType::PCR_TUBE) {
        return static_cast<PcrTubeCarrier*>(this)->getContainers();
    } else if (type_ == TubeType::CHAMBER) {
        return static_cast<ChamberTubeCarrier*>(this)->getContainers();
    } else {
        return static_cast<StripTubeCarrier*>(this)->getContainers();
    }
}

std::vector<uint16_t> StripTubeCarrier::getBitmap() const {
    uint32_t              len = M * Tube<8>::N;
    std::vector<uint16_t> bitmap(len);
    for (uint32_t i = 0; i < M; i++) {
        auto                  tube = tube_[i];
        std::vector<uint16_t> tube_bitmap;
        if (tube != nullptr) {
            tube_bitmap = tube->getBitmap();
        } else {
            tube_bitmap = std::vector<uint16_t>(Tube<8>::N, 0);
        }

        for (uint32_t j = 0; j < Tube<8>::N; j++) {
            bitmap[i * Tube<8>::N + j] = tube_bitmap[j];
        }
    }
    return bitmap;
}

int StripTubeCarrier::getTubeIndex(Container* tube) const {
    for (int i = 0; i < M; i++) {
        if (dynamic_cast<Tube<8>*>(tube) == tube_[i]) {
            return i;
        }
    }
    return -1;
}

int StripTubeCarrier::getOrAllocTubeIndex(Container* tube) {
    if (TubeManager::getTubeType(tube) != TubeType::STRIP_TUBE) {
        return -1;
    }
    int index = getTubeIndex(tube);
    if (index != -1) {
        return index;
    }

    if (!setTube(dynamic_cast<Tube<8>*>(tube))){
        return -1;
    }

    return getTubeIndex(tube);
}

bool StripTubeCarrier::forceAllocTubeIndex(Container* tube, int index) {
    if (index < 0 || index >= M) {
        return false;
    }

    tube_[index] = dynamic_cast<Tube<8>*>(tube);
    return true;
}

bool StripTubeCarrier::releaseTube(Container* tube) {
    for (int i = 0; i < M; i++) {
        if (tube_[i] == tube) {
            tube_[i] = nullptr;
            return true;
        }
    }
    return false;
}

void StripTubeCarrier::markOtherIndexConsumer() {
    for (int i = 0; i < M; i++) {
        if (tube_[i] == nullptr) {
            consumer_indexs_.push_back(i);
        }
    }
}

void StripTubeCarrier::renewConsumer(MachineTypeId machine_type, AreaId area_id) {
    for (auto i : consumer_indexs_) {
        if (tube_[i] != nullptr) {
            tube_[i]->setTubeCarrier(nullptr);
            tube_[i] = nullptr;
        }
    }

    if (machine_type != (MachineTypeId)MachineType::TOTAL_NUM &&
        area_id != (AreaId)CommonAreaId::NOT_SUCCESS) {
        setMachineType(machine_type);
        setAreaId(area_id);
        is_consumer_ = true;
    }
}

std::vector<Container*> StripTubeCarrier::getContainers(){
    std::vector<Container*> containers;
    for (int i = 0; i < M; i++) {
        if (tube_[i] != nullptr) {
            containers.push_back(tube_[i]);
        }
    }
    return containers;
}

std::vector<uint16_t> ChamberTubeCarrier::getBitmap() const {
    uint32_t              len = M * Tube<1>::N;
    std::vector<uint16_t> bitmap(len);
    for (uint32_t i = 0; i < M; i++) {
        auto                  tube = tube_[i];
        std::vector<uint16_t> tube_bitmap;
        if (tube != nullptr) {
            tube_bitmap = tube->getBitmap();
        } else {
            tube_bitmap = std::vector<uint16_t>(Tube<1>::N, 0);
        }

        for (uint32_t j = 0; j < Tube<1>::N; j++) {
            bitmap[i * Tube<1>::N + j] = tube_bitmap[j];
        }
    }
    return bitmap;
}

int ChamberTubeCarrier::getTubeIndex(Container* tube) const {
    for (int i = 0; i < M; i++) {
        if (dynamic_cast<Tube<1>*>(tube) == tube_[i]) {
            return i;
        }
    }
    return -1;
}

int ChamberTubeCarrier::getOrAllocTubeIndex(Container* tube) {
    if (TubeManager::getTubeType(tube) != TubeType::CHAMBER) {
        return -1;
    }

    int index = getTubeIndex(tube);
    if (index != -1) {
        return index;
    }

    if (!setTube(dynamic_cast<Tube<1>*>(tube))){
        return -1;
    }

    return getTubeIndex(tube);
}

bool ChamberTubeCarrier::forceAllocTubeIndex(Container* tube, int index) {
    if (index < 0 || index >= M) {
        return false;
    }

    tube_[index] = dynamic_cast<Tube<1>*>(tube);
    return true;
}

bool ChamberTubeCarrier::releaseTube(Container* tube) {
    for (int i = 0; i < M; i++) {
        if (tube_[i] == tube) {
            tube_[i] = nullptr;
            return true;
        }
    }
    return false;
}

void ChamberTubeCarrier::markOtherIndexConsumer() {
    for (int i = 0; i < M; i++) {
        if (tube_[i] == nullptr) {
            consumer_indexs_.push_back(i);
        }
    }
}

void ChamberTubeCarrier::renewConsumer(MachineTypeId machine_type, AreaId area_id) {
    for (auto i : consumer_indexs_) {
        if (tube_[i] != nullptr) {
            tube_[i]->setTubeCarrier(nullptr);
            tube_[i] = nullptr;
        }
    }

    if (machine_type != (MachineTypeId)MachineType::TOTAL_NUM &&
        area_id != (AreaId)CommonAreaId::NOT_SUCCESS) {
        setMachineType(machine_type);
        setAreaId(area_id);
        is_consumer_ = true;
    }
}

std::vector<Container*> ChamberTubeCarrier::getContainers(){
    std::vector<Container*> containers;
    for (int i = 0; i < M; i++) {
        if (tube_[i] != nullptr) {
            containers.push_back(tube_[i]);
        }
    }
    return containers;
}

std::vector<uint16_t> PcrTubeCarrier::getBitmap() const {
    uint32_t              len = M * Tube<1>::N;
    std::vector<uint16_t> bitmap(len);
    for (uint32_t i = 0; i < M; i++) {
        auto                  tube = tube_[i];
        std::vector<uint16_t> tube_bitmap;
        if (tube != nullptr) {
            tube_bitmap = tube->getBitmap();
        } else {
            tube_bitmap = std::vector<uint16_t>(Tube<1>::N, 0);
        }

        for (uint32_t j = 0; j < Tube<1>::N; j++) {
            bitmap[i * Tube<1>::N + j] = tube_bitmap[j];
        }
    }
    return bitmap;
}

int PcrTubeCarrier::getTubeIndex(Container* tube) const {
    for (int i = 0; i < M; i++) {
        if (dynamic_cast<Tube<1>*>(tube) == tube_[i]) {
            return i;
        }
    }
    return -1;
}

int PcrTubeCarrier::getOrAllocTubeIndex(Container* tube) {
    if (TubeManager::getTubeType(tube) != TubeType::PCR_TUBE) {
        return -1;
    }
    int index = getTubeIndex(tube);
    if (index != -1) {
        return index;
    }

    if (!setTube(dynamic_cast<Tube<1>*>(tube))){
        return -1;
    }

    return getTubeIndex(tube);
}

bool PcrTubeCarrier::forceAllocTubeIndex(Container* tube, int index) {
    if (index < 0 || index >= M) {
        return false;
    }

    tube_[index] = dynamic_cast<Tube<1>*>(tube);
    return true;
}

bool PcrTubeCarrier::releaseTube(Container* tube) {
    for (int i = 0; i < M; i++) {
        if (tube_[i] == tube) {
            tube_[i] = nullptr;
            return true;
        }
    }
    return false;
}

void PcrTubeCarrier::markOtherIndexConsumer() {
    for (int i = 0; i < M; i++) {
        if (tube_[i] == nullptr) {
            consumer_indexs_.push_back(i);
        }
    }
}

void PcrTubeCarrier::renewConsumer(MachineTypeId machine_type, AreaId area_id) {
    for (auto i : consumer_indexs_) {
        if (tube_[i] != nullptr) {
            tube_[i]->setTubeCarrier(nullptr);
            tube_[i] = nullptr;
        }
    }

    if (machine_type != (MachineTypeId)MachineType::TOTAL_NUM &&
        area_id != (AreaId)CommonAreaId::NOT_SUCCESS) {
        setMachineType(machine_type);
        setAreaId(area_id);
        is_consumer_ = true;
    }
}

std::vector<Container*> PcrTubeCarrier::getContainers(){
    std::vector<Container*> containers;
    for (int i = 0; i < M; i++) {
        if (tube_[i] != nullptr) {
            containers.push_back(tube_[i]);
        }
    }
    return containers;
}