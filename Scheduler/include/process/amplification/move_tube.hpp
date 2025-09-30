#pragma once

#include "procedure/stage.hpp"
#include "process/dummy.hpp"

class AmpMoveTube {
public:
    enum { kOutput = 0 };
    inline static const std::string Name       = "AmpMoveTube";
    inline static const std::string StartPos   = "StartPos";
    inline static const std::string StartIndex = "StartIndex";
    inline static const std::string EndPos     = "EndPos";
    inline static const std::string EndIndex   = "EndIndex";
    inline static const std::string Tube       = Step::Tube;

    static nlohmann::json fromDummy(const DummyStep& dummy_step) {
        nlohmann::json input        = nlohmann::json::object();
        auto           dummy_params = dummy_step.getParams();
        input[Tube] = dummy_params[std::to_string(static_cast<int>(Dummy::ParamType::TUBE_INDEX))];

        if (dummy_params.contains(
                std::to_string(static_cast<int>(Dummy::ParamType::START_POSITION)))) {
            input[StartPos] =
                dummy_params[std::to_string(static_cast<int>(Dummy::ParamType::START_POSITION))];
        } else {
            input[StartPos] = (uint16_t)AmplificationArea::AUTO;
        }

        if (dummy_params.contains(
                std::to_string(static_cast<int>(Dummy::ParamType::START_INDEX)))) {
            input[StartIndex] =
                dummy_params[std::to_string(static_cast<int>(Dummy::ParamType::START_INDEX))];
        } else {
            input[StartIndex] = (uint16_t)CommonIndexId::AUTO;
        }

        if (dummy_params.contains(
                std::to_string(static_cast<int>(Dummy::ParamType::END_POSITION)))) {
            auto pos =
                dummy_params[std::to_string(static_cast<int>(Dummy::ParamType::END_POSITION))];
            if (pos == Dummy::DummyEquipment::PCR) {
                input[EndPos] = (uint16_t)AmplificationArea::THERMAL_CYCLING_AREA;
            } else if (pos == Dummy::DummyEquipment::METAL_BATH) {
                input[EndPos] = (uint16_t)AmplificationArea::HIGH_TEMP_AREA;
            } else if (pos == Dummy::DummyEquipment::REACTION_POS) {
                input[EndPos] = (uint16_t)CommonAreaId::REACTION;
            } else {
                input[EndPos] = (uint16_t)AmplificationArea::AUTO;
            }
        } else {
            input[EndPos] = (uint16_t)AmplificationArea::AUTO;
        }
        if (dummy_params.contains(std::to_string(static_cast<int>(Dummy::ParamType::END_INDEX)))) {
            input[EndIndex] =
                dummy_params[std::to_string(static_cast<int>(Dummy::ParamType::END_INDEX))];
        } else {
            input[EndIndex] = (uint16_t)CommonIndexId::AUTO;
        }

        return input;
    }
};

class AmpMoveTubeStep : public CRTPStep<AmpMoveTubeStep> {
public:
    AmpMoveTubeStep(std::string name, Variables&& user_input = Variables())
        : CRTPStep<AmpMoveTubeStep>(MachineType::AMPLIFICATION, name, std::move(user_input)) {
        step_funcs_ = {std::bind(&AmpMoveTubeStep::phase0, this, std::placeholders::_1,
                                 std::placeholders::_2, std::placeholders::_3),
                       std::bind(&AmpMoveTubeStep::phase1, this, std::placeholders::_1,
                                 std::placeholders::_2, std::placeholders::_3)};
    }

    long long getTime() const { return 30; }

    std::vector<ExecutionResult>
    phase0(Reality& reality, std::shared_ptr<MachineManager> mac_manager, ActionId action_id) {
        std::vector<ExecutionResult> results;
        auto machine = mac_manager->getMachine<AmplificationModbusMachine>(machine_type_);

        auto start_pos   = user_input_[AmpMoveTube::StartPos].get<uint16_t>();
        auto start_index = user_input_[AmpMoveTube::StartIndex].get<uint16_t>();
        auto end_pos     = user_input_[AmpMoveTube::EndPos].get<uint16_t>();
        auto end_index   = user_input_[AmpMoveTube::EndIndex].get<uint16_t>();

        if (start_pos == (uint16_t)AmplificationArea::AUTO) {
            auto tube     = user_input_[AmpMoveTube::Tube].get<TubeId>();
            auto tube_pos = reality.getTubePosition(tube, TubePositionType::WHOLE_TUBE);

            start_pos   = tube_pos.area_id;
            start_index = tube_pos.index;
        }

        if (end_index == (uint16_t)CommonIndexId::AUTO &&
            end_pos != (uint16_t)AmplificationArea::AUTO) {
            auto tube_id = user_input_[AmpMoveTube::Tube].get<TubeId>();
            auto tube    = reality.getTube(tube_id);
            if (end_pos != (uint16_t)CommonAreaId::REACTION) {
                if (end_pos == (uint16_t)AmplificationArea::HIGH_TEMP_AREA) {
                    end_index =
                        machine->getEquipment(EquipmentType::METAL_BATH)->getContainerId(tube);
                }
                if (end_pos == (uint16_t)AmplificationArea::THERMAL_CYCLING_AREA) {
                    end_index = machine->getEquipment(EquipmentType::PCR)->getContainerId(tube);
                }
            } else {
                auto reaction_pos =
                    reality.getTubeReactionPosition(tube_id, TubePositionType::WHOLE_TUBE);
                end_pos   = reaction_pos.area_id;
                end_index = reaction_pos.index;
            }

            reality.setTubePosition(tube_id, {machine_type_, (AreaId)end_pos, (IndexId)end_index},
                                    TubePositionType::WHOLE_TUBE);
        }

        logger->debug("Moving tube from {}:{} to {}:{}", 
            magic_enum::enum_name((AmplificationArea)start_pos), start_index,
            magic_enum::enum_name((AmplificationArea)end_pos), end_index);

        machine->move_tube(start_pos, start_index, end_pos, end_index, action_id);

        results.push_back(ExecutionResult());
        return results;
    }

    std::vector<ExecutionResult>
    phase1(Reality& reality, std::shared_ptr<MachineManager> mac_manager, ActionId action_id) {
        std::vector<ExecutionResult> results;

        results.push_back(ExecutionResult{next_steps_[AmpMoveTube::kOutput], Variables(), false});
        return results;
    }

    std::vector<std::pair<MachineType, EquipmentType>>
    getNeedLockEquipment(Reality& reality, std::shared_ptr<MachineManager> mac_manager_,
                         std::shared_ptr<Action> action) const {
        if (action->getCurrentPhase() != 0) {
            return {};
        }

        std::vector<std::pair<MachineType, EquipmentType>> locked_equipment = {
            std::make_pair(machine_type_, EquipmentType::ROBOT_ARM),
            std::make_pair(machine_type_, EquipmentType::PIPETEE_GUN)};
        auto end_pos = user_input_[AmpMoveTube::EndPos].get<uint16_t>();
        if (end_pos == (uint16_t)AmplificationArea::HIGH_TEMP_AREA) {
            locked_equipment.push_back(std::make_pair(machine_type_, EquipmentType::METAL_BATH));
        }
        if (end_pos == (uint16_t)AmplificationArea::THERMAL_CYCLING_AREA) {
            locked_equipment.push_back(std::make_pair(machine_type_, EquipmentType::PCR));
        }
        return locked_equipment;
    }

    std::vector<std::pair<MachineType, EquipmentType>>
    getNeedUnlockEquipment(Reality& reality, std::shared_ptr<MachineManager> mac_manager_,
                           std::shared_ptr<Action> action) const {
        if (!action->isDone()) {
            return {};
        }

        std::vector<std::pair<MachineType, EquipmentType>> unlocked_equipment = {
            std::make_pair(machine_type_, EquipmentType::ROBOT_ARM),
            std::make_pair(machine_type_, EquipmentType::PIPETEE_GUN)};
        auto start_pos = user_input_[AmpMoveTube::StartPos].get<uint16_t>();

        if (start_pos == (uint16_t)AmplificationArea::AUTO) {
            auto tube_id = user_input_[AmpMoveTube::Tube].get<TubeId>();
            auto tube    = reality.getTube(tube_id);
            start_pos = reality.getTubeLastPosition(tube_id, TubePositionType::WHOLE_TUBE).area_id;
        }
        if (start_pos == (uint16_t)AmplificationArea::HIGH_TEMP_AREA) {
            unlocked_equipment.push_back(std::make_pair(machine_type_, EquipmentType::METAL_BATH));
        }
        if (start_pos == (uint16_t)AmplificationArea::THERMAL_CYCLING_AREA) {
            unlocked_equipment.push_back(std::make_pair(machine_type_, EquipmentType::PCR));
        }
        return unlocked_equipment;
    }

    std::vector<std::shared_ptr<Step>>
    getTubeTypeSupportsSteps(Reality& reality, std::map<TubeId, TubeId>& tube_map) {
        auto new_params = user_input_;
        if (user_input_.find(AmpMoveTube::Tube) == user_input_.end()) {
            return {std::make_shared<AmpMoveTubeStep>(name_, std::move(new_params))};
        }
        auto tube_id  = user_input_[AmpMoveTube::Tube].get<TubeId>();
        auto tube_ptr = reality.getTube(tube_id);

        if (TubeManager::getTubeType(tube_ptr) == TubeType::PCR_TUBE) {
            // change according to tube_map
            if (tube_map.find(tube_id) != tube_map.end()) {
                tube_id = tube_map[tube_id];
            }
            new_params[AmpMoveTube::Tube] = tube_id;
        }
        return {std::make_shared<AmpMoveTubeStep>(name_, std::move(new_params))};
    }

    static std::shared_ptr<AmpMoveTubeStep> fromDummy(DummyStep& dummy_step) {
        if (dummy_step.getType() == Dummy::DummyType::MOVE_TUBE) {
            auto&& vars = AmpMoveTube::fromDummy(dummy_step);
            if (vars[AmpMoveTube::EndPos] == (uint16_t)AmplificationArea::AUTO) {
                return nullptr;
            }
            return std::make_shared<AmpMoveTubeStep>(AmpMoveTube::Name, std::move(vars));
        }
        return nullptr;
    }

    static std::shared_ptr<AmpMoveTubeStep> fromDummyTube(TubeId tube_id) {
        Variables vars;
        vars[AmpMoveTube::Tube]       = tube_id;
        vars[AmpMoveTube::StartPos]   = (uint16_t)AmplificationArea::AUTO;
        vars[AmpMoveTube::StartIndex] = (uint16_t)CommonIndexId::AUTO;
        vars[AmpMoveTube::EndPos]     = (uint16_t)CommonAreaId::REACTION;
        vars[AmpMoveTube::EndIndex]   = (uint16_t)CommonIndexId::AUTO;
        return std::make_shared<AmpMoveTubeStep>(AmpMoveTube::Name, std::move(vars));
    }
};

class AmpMoveTubeStage : public TemplatedStage<AmpMoveTube> {
public:
    AmpMoveTubeStage(std::string id, Variables&& input) : TemplatedStage(std::move(id), true) {
        my_step_ = std::make_shared<AmpMoveTubeStep>(name_, std::move(input));
    }

    void generateWorkflowHelper(Workflow& workflow) {
        if (next_stages_.size() != 1) {
            return;
        }
        genWorkflowForNextStage(workflow, AmpMoveTube::kOutput);
    }
};