#pragma once

#include "container.hpp"
#include "reality/carrier/carrier.hpp"
#include "reality/carrier/strip_tube_carrier.hpp"
#include "reality/reagent.hpp"

#include "common/dbug_logging.h"

#include <cstdint>
#include <memory>
#include <vector>

using TubeId = int32_t;

// class Reagent;

// class Carrier;

struct TubePosition {
    MachineType machine_type;
    AreaId      area_id;
    IndexId     index;
};

enum class TubePositionType {
    SINGLE_TUBE,
    WHOLE_TUBE,
};

using TubeName  = std::string;
using TubeLabel = std::string;

template <uint32_t NUM>
class Tube : public Container {
public:
    inline static const int N = NUM;

    Tube(TubeId id, TubeName name, TubeLabel label) : id_(id), name_(name), label_(label) {
        for (int i = 0; i < N; i++) {
            reagents_[i] = nullptr;
        }
    }

    TubeId getId() const { return id_; }

    TubeName getName() const { return name_; }

    TubeLabel getLabel() const { return label_; }

    void setTubeCarrier(std::shared_ptr<Carrier> carrier) {
        if (carrier == nullptr) {
            carrier_ = nullptr;
            return;
        }
        carrier_ = carrier.get();
        carrier_->getOrAllocTubeIndex(this);
    }

    void setNowTubePosition(TubePosition now_position) {
        last_position_ = getNowTubePosition();
        now_position_  = now_position;
        // releaseCarrierPosition();

        is_on_carrier = now_position_.area_id == carrier_->getAreaId();
    }

    Carrier* getTubeCarrier() const { return carrier_; }

    std::vector<uint16_t> getBitmap() const {
        std::vector<uint16_t> bitmap(NUM);
        for (size_t i = 0; i < NUM; i++) {
            bitmap[i] = reagents_[i] ? 1 : 0;
        }
        return bitmap;
    }

    void setReagent(Reagent* reagent) override {
        for (size_t i = 0; i < NUM; i++) {
            if (reagents_[i] == nullptr) {
                reagents_[i] = reagent;
                return;
            }
        }
        ASSERT(false);
    }

    void setType(TubeType type) { type_ = type; }

    TubeType getType() const { return type_; }

    bool forceSetReagentIndex(Reagent* reagent, size_t index) {
        if (index < NUM) {
            reagents_[index] = reagent;
            return true;
        }
        return false;
    }

    std::vector<Reagent*> getReagents() const {
        std::vector<Reagent*> reagents;
        for (size_t i = 0; i < NUM; i++) {
            if (reagents_[i] != nullptr) {
                reagents.push_back(reagents_[i]);
            }
        }
        return reagents;
    }

    bool setAllReagentVolume(std::shared_ptr<Reagent> reagent, int volume) {
        for (size_t i = 0; i < NUM; i++) {
            if (reagents_[i] == reagent.get()) {
                volume_[i] = volume;
            }
        }
        return true;
    }

    bool setVolume(int volume, int index) {
        if (index < 0 || index >= NUM) {
            return false;
        }
        volume_[index] = volume;
        return true;
    }

    int getVolume(int index) const {
        if (type_ == TubeType::CHAMBER) {
            return volume_[0];
        }
        if (index < 0 || index >= NUM) {
            return 0;
        }
        return volume_[index];
    }

    TubePosition getNowTubePosition() const {
        auto tmp_now_position = now_position_;
        if (is_on_carrier && carrier_) {
            tmp_now_position.machine_type = (MachineType)carrier_->getMachineType();
            tmp_now_position.area_id      = carrier_->getAreaId();
        }
        return tmp_now_position;
    }

    TubePosition getLastTubePosition() const { return last_position_; }

    bool releaseCarrierPosition() {
        if (carrier_ && now_position_.area_id != carrier_->getAreaId()) {
            carrier_->releaseTube(this);
            carrier_ = nullptr;
            return true;
        }
        return false;
    }

    bool getIsReagent() const { return is_reagent; }

    void setIsReagent(bool is_reagent) { this->is_reagent = is_reagent; }

    bool getIsOnCarrier() const { return is_on_carrier; }

private:
    TubeId id_;

    TubeType type_ = TubeType::UNKNOWN;

    TubeName name_ = "";

    TubeLabel label_ = "";

    Reagent* reagents_[NUM];
    int      volume_[NUM];

    bool is_reagent = false;

    bool is_on_carrier = true;

    Carrier* carrier_ = nullptr;

    TubePosition now_position_;

    TubePosition last_position_ = {MachineType::TOTAL_NUM, -1, -1};

    friend class Reagent;
};

class TubeManager {
public:
    TubeManager() = default;

    std::shared_ptr<Tube<8>> allocStripTube(TubeLabel label, TubeName name) {
        auto id = next_id_++;
        if (name.empty()) {
            name = "Tube_" + std::to_string(id);
        }
        if (label.empty()) {
            label = "Label_" + std::to_string(id);
        }
        auto tube = std::make_shared<Tube<8>>(id, name, label);
        tube->setType(TubeType::STRIP_TUBE);
        strip_tubes_[name] = tube;
        tube_names_[id]    = {name, TubeType::STRIP_TUBE};
        return tube;
    }

    std::shared_ptr<Tube<1>> allocChamberTube(TubeLabel label, TubeName name) {
        auto id = next_id_++;
        if (name.empty()) {
            name = "Tube_" + std::to_string(id);
        }
        if (label.empty()) {
            label = "Label_" + std::to_string(id);
        }
        auto tube = std::make_shared<Tube<1>>(id, name, label);
        tube->setType(TubeType::CHAMBER);
        chamber_tubes_[name] = tube;
        tube_names_[id]      = {name, TubeType::CHAMBER};
        return tube;
    }

    std::shared_ptr<Tube<1>> allocPcrTube(TubeLabel label, TubeName name) {
        auto id = next_id_++;
        if (name.empty()) {
            name = "Tube_" + std::to_string(id);
        }
        if (label.empty()) {
            label = "Label_" + std::to_string(id);
        }
        auto tube = std::make_shared<Tube<1>>(id, name, label);
        tube->setType(TubeType::PCR_TUBE);
        pcr_tubes_[name] = tube;
        tube_names_[id]  = {name, TubeType::PCR_TUBE};
        return tube;
    }

    std::shared_ptr<Container> allocTube(const TubeLabel& label, const TubeName& name,
                                         TubeType type) {
        if (type == TubeType::STRIP_TUBE) {
            return allocStripTube(label, name);
        } else if (type == TubeType::PCR_TUBE) {
            return allocPcrTube(label, name);
        } else if (type == TubeType::CHAMBER) {
            return allocChamberTube(label, name);
        }
        throw std::invalid_argument("Invalid tube size");
    }

    static TubeType getTubeType(std::shared_ptr<Container> tube) {
        if (std::dynamic_pointer_cast<Tube<8>>(tube)) {
            return TubeType::STRIP_TUBE;
        } else if (std::dynamic_pointer_cast<Tube<1>>(tube)) {
            return std::dynamic_pointer_cast<Tube<1>>(tube)->getType();
        }
        return TubeType::UNKNOWN;
    }

    static TubeType getTubeType(Container* tube) {
        if (dynamic_cast<Tube<8>*>(tube)) {
            return TubeType::STRIP_TUBE;
        } else if (dynamic_cast<Tube<1>*>(tube)) {
            return dynamic_cast<Tube<1>*>(tube)->getType();
        }
        return TubeType::UNKNOWN;
    }

    static TubeType toTubeType(const std::string& type_str) {
        if (type_str == "STRIP_TUBE") {
            return TubeType::STRIP_TUBE;
        } else if (type_str == "PCR_TUBE") {
            return TubeType::PCR_TUBE;
        } else if (type_str == "CHAMBER") {
            return TubeType::CHAMBER;
        }
        return TubeType::UNKNOWN;
    }

    std::shared_ptr<Container> getTube(const TubeName& name, TubeType type) const {
        if (type == TubeType::STRIP_TUBE) {
            auto it = strip_tubes_.find(name);
            if (it != strip_tubes_.end()) {
                return it->second;
            }
        } else if (type == TubeType::PCR_TUBE) {
            auto it = pcr_tubes_.find(name);
            if (it != pcr_tubes_.end()) {
                return it->second;
            }
        } else if (type == TubeType::CHAMBER) {
            auto it = chamber_tubes_.find(name);
            if (it != chamber_tubes_.end()) {
                return it->second;
            }
        }
        return nullptr;
    }

    std::vector<std::shared_ptr<Container>> getLabelTubes(const TubeLabel& label) const {
        std::vector<std::shared_ptr<Container>> tubes;
        for (const auto& [name, tube] : strip_tubes_) {
            if (tube->getLabel() == label) {
                tubes.push_back(tube);
            }
        }
        for (const auto& [name, tube] : pcr_tubes_) {
            if (tube->getLabel() == label) {
                tubes.push_back(tube);
            }
        }
        for (const auto& [name, tube] : chamber_tubes_) {
            if (tube->getLabel() == label) {
                tubes.push_back(tube);
            }
        }
        return tubes;
    }

    static std::shared_ptr<Tube<8>> toStripTube(std::shared_ptr<Container> tube) {
        return std::dynamic_pointer_cast<Tube<8>>(tube);
    }

    static std::shared_ptr<Tube<1>> toPcrTube(std::shared_ptr<Container> tube) {
        return std::dynamic_pointer_cast<Tube<1>>(tube);
    }

    static std::shared_ptr<Tube<1>> toChamberTube(std::shared_ptr<Container> tube) {
        return std::dynamic_pointer_cast<Tube<1>>(tube);
    }

    std::shared_ptr<Container> getTube(TubeId id) const {
        auto transfer_id = id;
        while (transfer_tubes_.find(transfer_id) != transfer_tubes_.end()) {
            transfer_id = transfer_tubes_.at(transfer_id);
        }

        auto it = tube_names_.find(transfer_id);
        if (it != tube_names_.end()) {
            const auto& [name, type] = it->second;
            return getTube(name, type);
        }
        return nullptr;
    }

    static TubeId getTubeId(std::shared_ptr<Container> tube) {
        if (TubeManager::getTubeType(tube) == TubeType::PCR_TUBE) {
            return TubeManager::toPcrTube(tube)->getId();
        } else if (TubeManager::getTubeType(tube) == TubeType::STRIP_TUBE) {
            return TubeManager::toStripTube(tube)->getId();
        } else {
            return TubeManager::toChamberTube(tube)->getId();
        }
    }

    static std::pair<std::shared_ptr<Container>, int>
    getTubeByReagent(std::shared_ptr<Reagent> reagent) {
        auto tube = reagent->getContainer();
        if (TubeManager::getTubeType(tube) == TubeType::PCR_TUBE) {
            auto pcr_tube = TubeManager::toPcrTube(tube);
            return {tube, 0};
        } else if (TubeManager::getTubeType(tube) == TubeType::CHAMBER) {
            auto chamber_tube = TubeManager::toChamberTube(tube);
            return {tube, 0};
        } else {
            auto strip_tube = TubeManager::toStripTube(tube);
            for (int i = 0; i < strip_tube->getReagents().size(); i++) {
                if (strip_tube->getReagents()[i] == reagent.get()) {
                    return {tube, i};
                }
            }
        }
        return {nullptr, -1};
    }

    static int getTubeReagentsPos(std::shared_ptr<Container> tube, Reagent* reagent) {
        if (TubeManager::getTubeType(tube) == TubeType::PCR_TUBE) {
            return TubeManager::toPcrTube(tube)->getReagents().size();
        } else if (TubeManager::getTubeType(tube) == TubeType::CHAMBER) {
            return TubeManager::toChamberTube(tube)->getReagents().size();
        } else {
            auto strip_tube = TubeManager::toStripTube(tube);
            for (int i = 0; i < strip_tube->getReagents().size(); i++) {
                if (strip_tube->getReagents()[i] == reagent) {
                    return i;
                }
            }
        }
        return -1;
    }

    static bool checkSameReagent(std::shared_ptr<Container> tube, int start_index, int end_index) {
        if (start_index > end_index) {
            std::swap(start_index, end_index);
        }
        if (TubeManager::getTubeType(tube) == TubeType::PCR_TUBE) {
            return false;
        } else if (TubeManager::getTubeType(tube) == TubeType::CHAMBER) {
            return true;
        } else {
            auto strip_tube = TubeManager::toStripTube(tube);
            auto reagents   = strip_tube->getReagents();
            if (start_index >= reagents.size() || end_index >= reagents.size()) {
                return false;
            }
            if (reagents.empty()) {
                return true;
            }
            return reagents[start_index] == reagents[end_index];
        }
        return true;
    }

    static std::vector<Reagent*> getTubeReagents(std::shared_ptr<Container> tube) {
        if (TubeManager::getTubeType(tube) == TubeType::PCR_TUBE) {
            return TubeManager::toPcrTube(tube)->getReagents();
        } else if (TubeManager::getTubeType(tube) == TubeType::CHAMBER) {
            return TubeManager::toChamberTube(tube)->getReagents();
        } else {
            return TubeManager::toStripTube(tube)->getReagents();
        }
    }

    static bool getTubeIsReagent(std::shared_ptr<Container> tube) {
        if (TubeManager::getTubeType(tube) == TubeType::PCR_TUBE) {
            return TubeManager::toPcrTube(tube)->getIsReagent();
        } else if (TubeManager::getTubeType(tube) == TubeType::CHAMBER) {
            return TubeManager::toChamberTube(tube)->getIsReagent();
        } else {
            return TubeManager::toStripTube(tube)->getIsReagent();
        }
    }

    static void setTubeIsReagent(std::shared_ptr<Container> tube, bool is_reagent) {
        if (TubeManager::getTubeType(tube) == TubeType::PCR_TUBE) {
            TubeManager::toPcrTube(tube)->setIsReagent(is_reagent);
        } else if (TubeManager::getTubeType(tube) == TubeType::CHAMBER) {
            TubeManager::toChamberTube(tube)->setIsReagent(is_reagent);
        } else {
            TubeManager::toStripTube(tube)->setIsReagent(is_reagent);
        }
    }

    void setTubePosition(TubeId id, TubePosition tube_position, TubePositionType type) {
        auto tube = getTube(id);
        if (!tube) {
            throw std::runtime_error("Tube not found");
        }
        Carrier* carrier;
        if (TubeManager::getTubeType(tube) == TubeType::PCR_TUBE) {
            TubeManager::toPcrTube(tube)->setNowTubePosition(tube_position);
        } else if (TubeManager::getTubeType(tube) == TubeType::CHAMBER) {
            TubeManager::toChamberTube(tube)->setNowTubePosition(tube_position);
        } else {
            if (type == TubePositionType::WHOLE_TUBE) {
                TubeManager::toStripTube(tube)->setNowTubePosition(tube_position);
            } else {
                TubeManager::toStripTube(tube)->setNowTubePosition(
                    {tube_position.machine_type, tube_position.area_id,
                     (IndexId)(tube_position.index / 8)});
            }
        }
    }

    TubePosition getTubePosition(TubeId id, TubePositionType type, IndexId index = 0) const {
        auto tube = getTube(id);
        if (!tube) {
            throw std::runtime_error("Tube not found");
        }

        TubePosition pos;
        if (TubeManager::getTubeType(tube) == TubeType::PCR_TUBE) {
            return TubeManager::toPcrTube(tube)->getNowTubePosition();
        } else if (TubeManager::getTubeType(tube) == TubeType::CHAMBER) {
            return TubeManager::toChamberTube(tube)->getNowTubePosition();
        } else {
            auto pos = TubeManager::toStripTube(tube)->getNowTubePosition();
            if (type == TubePositionType::WHOLE_TUBE) {
                return pos;
            } else {
                return {pos.machine_type, pos.area_id, (IndexId)(pos.index * 8 + index % 8)};
            }
        }
    }

    TubePosition getTubeLastPosition(TubeId id, TubePositionType type, IndexId index = 0) const {
        auto tube = getTube(id);
        if (!tube) {
            throw std::runtime_error("Tube not found");
        }

        TubePosition pos;
        if (TubeManager::getTubeType(tube) == TubeType::PCR_TUBE) {
            return TubeManager::toPcrTube(tube)->getLastTubePosition();
        } else if (TubeManager::getTubeType(tube) == TubeType::CHAMBER) {
            return TubeManager::toChamberTube(tube)->getLastTubePosition();
        } else {
            auto pos = TubeManager::toStripTube(tube)->getLastTubePosition();
            if (type == TubePositionType::WHOLE_TUBE) {
                return pos;
            } else {
                return {pos.machine_type, pos.area_id, (IndexId)(pos.index * 8 + index % 8)};
            }
        }
    }

    TubePosition getTubeReactionPosition(TubeId id, TubePositionType type, IndexId index) const;

    static Carrier* getTubeCarrier(std::shared_ptr<Container> tube);

    static void setTubeCarrier(std::shared_ptr<Container> tube, std::shared_ptr<Carrier> carrier);

    bool isAlloced(TubeId id) {
        if (id == -1) {
            return false;
        }
        auto tube = getTube(id);
        if (!tube) {
            return false;
        }

        return TubeManager::getTubeCarrier(tube) != nullptr;
    }

    static bool isOnCarrier(std::shared_ptr<Container> tube) {
        if (TubeManager::getTubeType(tube) == TubeType::PCR_TUBE) {
            return TubeManager::toPcrTube(tube)->getIsOnCarrier();
        } else if (TubeManager::getTubeType(tube) == TubeType::CHAMBER) {
            return TubeManager::toChamberTube(tube)->getIsOnCarrier();
        } else {
            return TubeManager::toStripTube(tube)->getIsOnCarrier();
        }
    }

    bool addTransfer(TubeId origin_tube, TubeId new_id) {
        transfer_tubes_[origin_tube] = new_id;
        return true;
    }

    bool markWorkflowTubes(std::string                                       workflow_name,
                           std::map<std::string, std::pair<TubeId, IndexId>> tmp_tubes_relation_) {
        workflow_tubes_relation_[workflow_name] = tmp_tubes_relation_;
        return true;
    }

    std::map<std::string, std::pair<TubeId, IndexId>>
    getWorkflowTubesRelation(std::string workflow_name) {
        if (workflow_tubes_relation_.find(workflow_name) == workflow_tubes_relation_.end()) {
            return {};
        }
        return workflow_tubes_relation_[workflow_name];
    }

private:
    TubeId                                          next_id_ = 1;
    std::map<TubeId, std::pair<TubeName, TubeType>> tube_names_;
    std::map<TubeName, std::shared_ptr<Tube<8>>>    strip_tubes_;
    std::map<TubeName, std::shared_ptr<Tube<1>>>    pcr_tubes_;
    std::map<TubeName, std::shared_ptr<Tube<1>>>    chamber_tubes_;

    std::map<std::string, std::map<std::string, std::pair<TubeId, IndexId>>>
        workflow_tubes_relation_;

    std::map<TubeId, TubeId> transfer_tubes_;
};

// using Tube8 = Tube<8>;