#include "machine/common.hpp"
#include "magic_enum.hpp"
#include "reality/container/container.hpp"

class Step;

class Equipment {
public:
    Equipment(EquipmentType type, int slot_num = 1)
        : equipment_type_(type), max_slot_num_(slot_num) {
        containers_.resize(max_slot_num_);
    }

    static std::shared_ptr<Equipment> build(EquipmentType type, int slot_num = 1) {
        return std::make_shared<Equipment>(type, slot_num);
    }

    virtual ~Equipment() = default;

    EquipmentType getType() const { return (EquipmentType)equipment_type_; }
    std::string   getName() const { return std::string(magic_enum::enum_name(equipment_type_)); }

    bool isAvailable() const { return alloc_slot_num_ < max_slot_num_; }

    IndexId alloc(std::shared_ptr<Container> container = nullptr,
                  std::shared_ptr<Step>      step      = nullptr) {
        if (!isAvailable()) {
            return (IndexId)CommonIndexId::NOT_SUCCESS;
        }
        auto it = std::find(containers_.begin(), containers_.end(), nullptr);
        if (it != containers_.end()) {
            *it = container;
            if (step) {
                equipment_steps_[container] = step;
            }
            alloc_slot_num_++;
            return (IndexId)(it - containers_.begin());
        }

        // if max_num is 1, don't need alloc to tube
        if (max_slot_num_ == 1 && alloc_slot_num_ == 0) {
            alloc_slot_num_++;
            return 0;
        }
        return (IndexId)CommonIndexId::NOT_SUCCESS;
    }

    bool release(std::shared_ptr<Container> container = nullptr) {
        auto it = std::find(containers_.begin(), containers_.end(), container);
        if (it != containers_.end()) {
            *it = nullptr;
            equipment_steps_.erase(container);
            alloc_slot_num_--;
            return true;
        }

        // if max_num is 1, don't need alloc to tube
        if (max_slot_num_ == 1 && alloc_slot_num_ > 0) {
            alloc_slot_num_--;
            return true;
        }
        return false;
    }

    bool release(Container* container = nullptr) {
        for (auto it = containers_.begin(); it != containers_.end(); ++it) {
            if (it->get() == container) {
                equipment_steps_.erase(*it);
                *it = nullptr;
                alloc_slot_num_--;
                return true;
            }
        }

        // if max_num is 1, don't need alloc to tube
        if (max_slot_num_ == 1 && alloc_slot_num_ > 0) {
            alloc_slot_num_--;
            return true;
        }
        return false;
    }

    IndexId getContainerId(std::shared_ptr<Container> container = nullptr) const {
        auto it = std::find(containers_.begin(), containers_.end(), container);
        if (it != containers_.end()) {
            return (IndexId)(it - containers_.begin());
        }
        return (IndexId)CommonIndexId::NOT_SUCCESS;
    }

    std::vector<std::shared_ptr<Step>> getSteps() const {
        std::vector<std::shared_ptr<Step>> steps;
        for (const auto& [container, step] : equipment_steps_) {
            steps.push_back(step);
        }
        return steps;
    }

private:
    EquipmentType                           equipment_type_;
    int                                     max_slot_num_;
    int                                     alloc_slot_num_;
    std::vector<std::shared_ptr<Container>> containers_;

    std::map<std::shared_ptr<Container>, std::shared_ptr<Step>> equipment_steps_;
};