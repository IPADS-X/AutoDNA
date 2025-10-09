#pragma once

#include "procedure/stage.hpp"
#include "process/dummy.hpp"

class LibAspirateMix {
public:
    enum { kOutput = 0 };
    inline static const std::string Name           = "LibAspirateMix";
    inline static const std::string Pos            = "Pos";
    inline static const std::string Index          = "Index";
    inline static const std::string Volume         = "Volume";
    inline static const std::string Total          = "Total";
    inline static const std::string PipetteNum     = "PipetteNum";
    inline static const std::string PipetteTrIndex = "PipetteTrIndex";
    inline static const std::string MixSpeed       = "MixSpeed";
    inline static const std::string Tube           = "Tube";

    static nlohmann::json fromDummy(const DummyStep& dummy_step) {
        nlohmann::json input        = nlohmann::json::object();
        auto           dummy_params = dummy_step.getParams();
        input[Tube] = dummy_params[std::to_string(static_cast<int>(Dummy::ParamType::TUBE_INDEX))];

        if (dummy_params.contains(std::to_string(static_cast<int>(Dummy::ParamType::POSITION)))) {
            input[Pos] = dummy_params[std::to_string(static_cast<int>(Dummy::ParamType::POSITION))];
        } else {
            input[Pos] = (uint16_t)LibraryArea::AUTO;
        }
        if (dummy_params.contains(std::to_string(static_cast<int>(Dummy::ParamType::INDEX)))) {
            input[Index] = dummy_params[std::to_string(static_cast<int>(Dummy::ParamType::INDEX))];
        } else {
            input[Index] = (uint16_t)CommonIndexId::AUTO;
        }
        if (dummy_params.contains(std::to_string(static_cast<int>(Dummy::ParamType::VOLUME)))) {
            input[Volume] =
                dummy_params[std::to_string(static_cast<int>(Dummy::ParamType::VOLUME))];
        } else {
            input[Volume] = (uint16_t)CommonVolumeId::AUTO;
        }
        if (dummy_params.contains(
                std::to_string(static_cast<int>(Dummy::ParamType::TOTAL_MIX_TIMES)))) {
            input[Total] =
                dummy_params[std::to_string(static_cast<int>(Dummy::ParamType::TOTAL_MIX_TIMES))];

        } else {
            input[Total] = 10;
        }

        if (dummy_params.contains(std::to_string(static_cast<int>(Dummy::ParamType::DURATION)))) {
            input[Total] =
                int(dummy_params[std::to_string(static_cast<int>(Dummy::ParamType::DURATION))]) /
                3000;
        }

        if (dummy_params.contains(
                std::to_string(static_cast<int>(Dummy::ParamType::PIPETTE_NUM)))) {
            input[PipetteNum] =
                dummy_params[std::to_string(static_cast<int>(Dummy::ParamType::PIPETTE_NUM))];

        } else {
            input[PipetteNum] = 1;
        }
        if (dummy_params.contains(
                std::to_string(static_cast<int>(Dummy::ParamType::PIPETTE_TR_INDEX)))) {
            input[PipetteTrIndex] =
                dummy_params[std::to_string(static_cast<int>(Dummy::ParamType::PIPETTE_TR_INDEX))];
        } else {
            input[PipetteTrIndex] = PipetteTrType::UL_200;
        }
        if (dummy_params.contains(std::to_string(static_cast<int>(Dummy::ParamType::MIX_SPEED)))) {
            input[MixSpeed] =
                dummy_params[std::to_string(static_cast<int>(Dummy::ParamType::MIX_SPEED))];
        } else {
            input[MixSpeed] = 100;
        }

        return input;
    }
};

class LibAspirateMixStep : public CRTPStep<LibAspirateMixStep> {
public:
    LibAspirateMixStep(std::string name, Variables&& user_input = Variables())
        : CRTPStep<LibAspirateMixStep>(MachineType::LIBRARY, name, std::move(user_input)) {
        step_funcs_ = {std::bind(&LibAspirateMixStep::phase0, this, std::placeholders::_1,
                                 std::placeholders::_2, std::placeholders::_3),
                       std::bind(&LibAspirateMixStep::phase1, this, std::placeholders::_1,
                                 std::placeholders::_2, std::placeholders::_3)};
    }

    std::vector<ExecutionResult>
    phase0(Reality& reality, std::shared_ptr<MachineManager> mac_manager, ActionId action_id) {
        std::vector<ExecutionResult> results;
        auto machine = mac_manager->getMachine<LibraryModbusMachine>(machine_type_);

        auto pos              = user_input_[LibAspirateMix::Pos].get<uint16_t>();
        auto index            = user_input_[LibAspirateMix::Index].get<uint16_t>();
        auto volume           = user_input_[LibAspirateMix::Volume].get<uint16_t>();
        auto total            = user_input_[LibAspirateMix::Total].get<uint16_t>();
        auto num              = user_input_[LibAspirateMix::PipetteNum].get<uint16_t>();
        auto pipette_tr_index = user_input_[LibAspirateMix::PipetteTrIndex].get<uint16_t>();

        if (pipette_tr_index == (uint16_t)PipetteTrType::UL_50 && volume > 4500) {
            volume = 4500;
        }
        // volume = volume / 2 > 5000 ? 5000 : volume / 2;
        // pipette_tr_index = (uint16_t)PipetteTrType::UL_50;

        uint16_t mix_speed = 0;
        if (user_input_.find(LibAspirateMix::MixSpeed) != user_input_.end()) {
            mix_speed = user_input_[LibAspirateMix::MixSpeed].get<uint16_t>();
        } else {
            mix_speed = 100;
        }

        if (pos == (uint16_t)LibraryArea::AUTO) {
            auto tube     = user_input_[LibAspirateMix::Tube].get<TubeId>();
            auto tube_pos = reality.getTubePosition(tube, TubePositionType::SINGLE_TUBE);

            pos   = tube_pos.area_id;
            index = tube_pos.index;

            if (pos == (uint16_t)CommonAreaId::NOT_SUCCESS) {
                pos = (uint16_t)LibraryArea::SAMPLE_AREA;
            }
        }

        logger->info(
            "LibAspirateMixStep, phase0: pos: {}, index: {}, volume: {}, total: {}, num: {}", pos,
            index, volume, total, num);
        machine->aspirate_mix(pos, index, volume, total, num, pipette_tr_index, mix_speed,
                              action_id);

        results.push_back(ExecutionResult());
        return results;
    }

    std::vector<ExecutionResult>
    phase1(Reality& reality, std::shared_ptr<MachineManager> mac_manager, ActionId action_id) {
        std::vector<ExecutionResult> results;

        results.push_back(
            ExecutionResult{next_steps_[LibAspirateMix::kOutput], Variables(), false});
        return results;
    }

    std::vector<std::pair<MachineType, EquipmentType>>
    getNeedLockEquipment(Reality& reality, std::shared_ptr<MachineManager> mac_manager_,
                         std::shared_ptr<Action> action) const {
        if (action->getCurrentPhase() != 0) {
            return {};
        }

        return {std::make_pair(machine_type_, EquipmentType::ROBOT_ARM),
                std::make_pair(machine_type_, EquipmentType::PIPETEE_GUN)};
    }

    std::vector<std::pair<MachineType, EquipmentType>>
    getNeedUnlockEquipment(Reality& reality, std::shared_ptr<MachineManager> mac_manager_,
                           std::shared_ptr<Action> action) const {
        if (!action->isDone()) {
            return {};
        }

        return {std::make_pair(machine_type_, EquipmentType::ROBOT_ARM),
                std::make_pair(machine_type_, EquipmentType::PIPETEE_GUN)};
    }

    std::vector<std::shared_ptr<Step>>
    getTubeTypeSupportsSteps(Reality& reality, std::map<TubeId, TubeId>& tube_map) {
        auto new_params = user_input_;
        if (user_input_.find(LibAspirateMix::Tube) == user_input_.end()) {
            return {std::make_shared<LibAspirateMixStep>(name_, std::move(new_params))};
        }
        auto tube_id  = user_input_[LibAspirateMix::Tube].get<TubeId>();
        auto tube_ptr = reality.getTube(tube_id);

        // change according to tube_map
        if (tube_map.find(tube_id) != tube_map.end()) {
            tube_id = tube_map[tube_id];
        }
        new_params[LibAspirateMix::Tube] = tube_id;
        return {std::make_shared<LibAspirateMixStep>(name_, std::move(new_params))};
    }

    static std::shared_ptr<LibAspirateMixStep> fromDummy(DummyStep& dummy_step) {
        if (dummy_step.getType() == Dummy::DummyType::ASPIRATE_MIX) {
            return std::make_shared<LibAspirateMixStep>(LibAspirateMix::Name,
                                                        LibAspirateMix::fromDummy(dummy_step));
        }
        return nullptr;
    }
};

class LibAspirateMixStage : public TemplatedStage<LibAspirateMix> {
public:
    LibAspirateMixStage(std::string id, Variables&& input) : TemplatedStage(std::move(id), true) {
        my_step_ = std::make_shared<LibAspirateMixStep>(name_, std::move(input));
    }

    void generateWorkflowHelper(Workflow& workflow) {
        if (next_stages_.size() != 1) {
            return;
        }
        genWorkflowForNextStage(workflow, LibAspirateMix::kOutput);
    }
};