#pragma once

#include "procedure/stage.hpp"

class Dummy {
public:
    enum { kOutput = 0 };
    inline static const std::string Name = "Dummy";
    inline static const std::string Type = "Type";
    enum class DummyType {
        UNKNOWN             = -1,
        PIPETTE             = 0,
        MOVE_TUBE           = 1,
        MOVE_CARRIER        = 2,
        SHAKE               = 3,
        TIME                = 4,
        HEATER              = 5,
        PCR                 = 6,
        CENTRIFUGE          = 7,
        ASPIRATE_MIX        = 8,
        MIX                 = 9,
        FLUO                = 10,
        CAP                 = 11,
        MOVE_PCR_TUBE       = 12,
        CENTRIFUGE_PCR_TUBE = 13,
        PIPETTE_PCR_TUBE    = 14,

        SET_HEATER_TEMP      = 15,
        SET_HEATER_SHAKER_SPEED = 16,
        SET_HEATER_SHAKER_TEMP = 17,

        OPEN_PCR_LID  = 18,
        CLOSE_PCR_LID = 19,

        OPEN_PORT = 20,
        CLOSE_PORT = 21,

        ALLOC_TUBE          = 100,
        GET_TUBE            = 101,
    };

    enum class ParamType {
        POSITION,
        INDEX,
        TUBE_NAME,
        START_POSITION,
        START_INDEX,
        END_POSITION,
        END_INDEX,
        VOLUME,
        TOTAL_MIX_TIMES,
        PIPETTE_NUM,
        PIPETTE_TR_INDEX,
        MIX_SPEED,
        TUBE_INDEX,
        PCR_FILE,
        BEFORE_PIPETTE_MIX_TIMES,
        AFTER_PIPETTE_MIX_TIMES,
        BEFORE_PIPETTE_MIX_VOLUME,
        AFTER_PIPETTE_MIX_VOLUME,
        SRC_TUBE_INDEX,
        DEST_TUBE_INDEX,
        DURATION,
        TEMPERATURE,
        TUBE_TYPE,
        TUBE_LIST
    };

    using DummyMachine = MachineType;

    using DummyEquipment = EquipmentType;
};

class DummyStep : public CRTPStep<DummyStep> {
public:
    DummyStep(std::string name, Variables&& user_input,
              Dummy::DummyType    type    = Dummy::DummyType::UNKNOWN,
              Dummy::DummyMachine machine = Dummy::DummyMachine::PURIFICATION)
        : CRTPStep<DummyStep>(MachineType::TOTAL_NUM, name, std::move(user_input)),
          type_(type),
          machine_(machine) {
        step_funcs_ = {std::bind(&DummyStep::phase0, this, std::placeholders::_1,
                                 std::placeholders::_2, std::placeholders::_3)};
    }

    std::vector<ExecutionResult>
    phase0(Reality& reality, std::shared_ptr<MachineManager> mac_manager, ActionId action_id) {
        std::vector<ExecutionResult> results;
        Variables                    output;
        if (user_input_.empty()) {
            output = step_input_;
        } else {
            output = user_input_;
        }
        results.push_back(ExecutionResult{next_steps_[Dummy::kOutput], std::move(output), false});
        return results;
    }

    bool check(const DummyStep& other) const {
        // 1. check type
        if (type_ != other.type_) {
            // std::cout << "Type mismatch: " << magic_enum::enum_name(type_) << " vs "
            //           << magic_enum::enum_name(other.type_) << "\n";
            return false;
        }

        // 2. check params
        for (auto& single_input : user_input_.items()) {
            if (other.user_input_.find(single_input.key()) == other.user_input_.end()) {
                // std::cout << "Key " << single_input.key() << " not found in other step\n";
                return false;
            }

            if (single_input.key() ==
                    std::to_string(static_cast<int>(Dummy::ParamType::TUBE_NAME)) ||
                single_input.key() ==
                    std::to_string(static_cast<int>(Dummy::ParamType::TUBE_INDEX)) ||
                single_input.key() ==
                    std::to_string(static_cast<int>(Dummy::ParamType::END_INDEX)) ||
                single_input.key() ==
                    std::to_string(static_cast<int>(Dummy::ParamType::INDEX)) ||
                single_input.key() ==
                    std::to_string(static_cast<int>(Dummy::ParamType::SRC_TUBE_INDEX)) ||
                single_input.key() ==
                    std::to_string(static_cast<int>(Dummy::ParamType::DEST_TUBE_INDEX)) ||
                single_input.key() ==
                    std::to_string(static_cast<int>(Dummy::ParamType::TUBE_LIST))) {
                continue;
            }

            auto other_value = other.user_input_.at(single_input.key());

            if (single_input.value() != other_value) {
                // std::cout << "Value mismatch for key " << single_input.key() << ": "
                //           << single_input.value() << " vs " << other_value << "\n";
                return false;
            }
        }

        return true;
    }

    std::vector<TubeId> getUsedTubeIds() {
        std::vector<TubeId> used_tube_ids;
        for (const auto& [key, value] : user_input_.items()) {
            if (key == std::to_string(static_cast<int>(Dummy::ParamType::SRC_TUBE_INDEX)) ||
                key == std::to_string(static_cast<int>(Dummy::ParamType::DEST_TUBE_INDEX)) ||
                key == std::to_string(static_cast<int>(Dummy::ParamType::TUBE_INDEX))) {
                used_tube_ids.push_back(value.get<TubeId>());
            }
        }
        return used_tube_ids;
    }

    Dummy::DummyMachine getMachine() const { return machine_; }

    MachineType getMachineType() const { return machine_; }

    Dummy::DummyType getType() const { return type_; }

    void setType(Dummy::DummyType type) { type_ = type; }

    void setMachine(Dummy::DummyMachine machine) { machine_ = machine; }

    bool setCurrentNum(uint16_t current_num) {
        user_input_[std::to_string(static_cast<int>(Dummy::ParamType::PIPETTE_NUM))] = current_num;
        return true;
    }

    uint16_t getCurrentNum() const {
        if (user_input_.find(std::to_string(static_cast<int>(Dummy::ParamType::PIPETTE_NUM))) !=
            user_input_.end()) {
            return user_input_
                .at(std::to_string(static_cast<int>(Dummy::ParamType::PIPETTE_NUM)))
                .get<uint16_t>();
        }
        return 1;
    }

    bool targetIsEquipment() {
        auto target =
            user_input_.find(std::to_string(static_cast<int>(Dummy::ParamType::END_POSITION)));
        if (target != user_input_.end()) {
            if (target.value() != Dummy::DummyEquipment::REACTION_POS &&
                target.value() != Dummy::DummyEquipment::ENTER_POS &&
                target.value() != Dummy::DummyEquipment::EXIT_POS) {
                return true;
            }
        }
        return false;
    }

    bool nowIsEquipment() {
        if (type_ == Dummy::DummyType::CAP || type_ == Dummy::DummyType::SHAKE ||
            type_ == Dummy::DummyType::TIME || type_ == Dummy::DummyType::HEATER ||
            type_ == Dummy::DummyType::PCR || type_ == Dummy::DummyType::CENTRIFUGE ||
            type_ == Dummy::DummyType::FLUO || type_ == Dummy::DummyType::CENTRIFUGE_PCR_TUBE) {
            return true;
        }
        return false;
    }

    bool srcIsEquipment() {
        auto target =
            user_input_.find(std::to_string(static_cast<int>(Dummy::ParamType::START_POSITION)));
        if (target != user_input_.end()) {
            if (target.value() != Dummy::DummyEquipment::REACTION_POS &&
                target.value() != Dummy::DummyEquipment::ENTER_POS &&
                target.value() != Dummy::DummyEquipment::EXIT_POS) {
                return true;
            }
        }
        return false;
    }

private:
    Dummy::DummyType    type_;
    Dummy::DummyMachine machine_;
};

class DummyStage : public TemplatedStage<Dummy> {
public:
    DummyStage(std::string id, Variables&& input, Dummy::DummyType type = Dummy::DummyType::UNKNOWN,
               Dummy::DummyMachine machine = Dummy::DummyMachine::PURIFICATION)
        : TemplatedStage(std::move(id), false) {
        my_step_ = std::make_shared<DummyStep>(name_, std::move(input), type, machine);
    }

    void generateWorkflowHelper(Workflow& workflow) {
        if (next_stages_.size() != 1) {
            return;
        }
        auto next_stage = next_stages_[Dummy::kOutput];
        auto next_step  = next_stage->getMyStep();
        next_stage->generateWorkflow(workflow);
        my_step_->setNextStep(Dummy::kOutput, next_step);
    }
};
