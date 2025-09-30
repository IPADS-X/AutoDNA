#pragma once

#include "procedure/stage.hpp"
#include "process/dummy.hpp"

class PuriMoveTube {
public:
    enum { kOutput = 0 };
    inline static const std::string Name       = "PuriMoveTube";
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
            input[StartPos] = (uint16_t)PurificationArea::AUTO;
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
            if (pos == Dummy::DummyEquipment::MAGNETIC_RACK) {
                input[EndPos] = (uint16_t)PurificationArea::MAGNETIC_AREA;
            } else if (pos == Dummy::DummyEquipment::HEATER_SHAKER) {
                input[EndPos] = (uint16_t)PurificationArea::SHAKING_AREA;
            } else if (pos == Dummy::DummyEquipment::CENTRIFUGE) {
                input[EndPos] = (uint16_t)PurificationArea::CENTRIFUGE_AREA;
            } else if (pos == Dummy::DummyEquipment::PCR) {
                input[EndPos] = (uint16_t)PurificationArea::THERMAL_CYCLING_AREA;
            } else if (pos == Dummy::DummyEquipment::REACTION_POS) {
                input[EndPos] = (uint16_t)CommonAreaId::REACTION;
            } else {
                input[EndPos] = (uint16_t)PurificationArea::AUTO;
            }
        } else {
            input[EndPos] = (uint16_t)PurificationArea::AUTO;
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

class PuriMoveTubeStep : public CRTPStep<PuriMoveTubeStep> {
public:
    PuriMoveTubeStep(std::string name, Variables&& user_input = Variables())
        : CRTPStep<PuriMoveTubeStep>(MachineType::PURIFICATION, name, std::move(user_input)) {
        step_funcs_ = {std::bind(&PuriMoveTubeStep::phase0, this, std::placeholders::_1,
                                 std::placeholders::_2, std::placeholders::_3),
                       std::bind(&PuriMoveTubeStep::phase1, this, std::placeholders::_1,
                                 std::placeholders::_2, std::placeholders::_3)};
    }

    long long getTime() const { return 30; }

    std::vector<ExecutionResult>
    phase0(Reality& reality, std::shared_ptr<MachineManager> mac_manager, ActionId action_id) {
        std::vector<ExecutionResult> results;
        auto machine = mac_manager->getMachine<PurificationModbusMachine>(machine_type_);

        auto start_pos   = user_input_[PuriMoveTube::StartPos].get<uint16_t>();
        auto start_index = user_input_[PuriMoveTube::StartIndex].get<uint16_t>();
        auto end_pos     = user_input_[PuriMoveTube::EndPos].get<uint16_t>();
        auto end_index   = user_input_[PuriMoveTube::EndIndex].get<uint16_t>();

        if (start_pos == (uint16_t)PurificationArea::AUTO) {
            auto tube     = user_input_[PuriMoveTube::Tube].get<TubeId>();
            auto tube_pos = reality.getTubePosition(tube, TubePositionType::WHOLE_TUBE);

            start_pos   = tube_pos.area_id;
            start_index = tube_pos.index;
        }

        if (end_index == (uint16_t)CommonIndexId::AUTO &&
            end_pos != (uint16_t)PurificationArea::AUTO) {
            auto tube_id = user_input_[PuriMoveTube::Tube].get<TubeId>();
            auto tube    = reality.getTube(tube_id);
            if (end_pos != (uint16_t)CommonAreaId::REACTION) {
                if (end_pos == (uint16_t)PurificationArea::CENTRIFUGE_AREA) {
                    end_index =
                        machine->getEquipment(EquipmentType::CENTRIFUGE)->getContainerId(tube);
                }
                if (end_pos == (uint16_t)PurificationArea::MAGNETIC_AREA) {
                    end_index =
                        machine->getEquipment(EquipmentType::MAGNETIC_RACK)->getContainerId(tube);
                }
                if (end_pos == (uint16_t)PurificationArea::SHAKING_AREA) {
                    end_index =
                        machine->getEquipment(EquipmentType::HEATER_SHAKER)->getContainerId(tube);
                }
                if (end_pos == (uint16_t)PurificationArea::THERMAL_CYCLING_AREA) {
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

        logger->info("PuriMoveTubeStep, phase0: start_pos: {}, start_index: {}, end_pos: {}, "
                     "end_index: {}",
                     start_pos, start_index, end_pos, end_index);

        machine->move_tube(start_pos, start_index, end_pos, end_index, action_id);

        results.push_back(ExecutionResult());
        return results;
    }

    std::vector<ExecutionResult>
    phase1(Reality& reality, std::shared_ptr<MachineManager> mac_manager, ActionId action_id) {
        std::vector<ExecutionResult> results;

        results.push_back(ExecutionResult{next_steps_[PuriMoveTube::kOutput], Variables(), false});
        return results;
    }

    std::vector<std::pair<MachineType, EquipmentType>>
    getNeedLockEquipment(Reality& reality, std::shared_ptr<MachineManager> mac_manager_,
                         std::shared_ptr<Action> action) const {
        if (action->getCurrentPhase() != 0) {
            return {};
        }

        std::vector<std::pair<MachineType, EquipmentType>> locked_equipment = {
            {machine_type_, EquipmentType::ROBOT_ARM}, {machine_type_, EquipmentType::PIPETEE_GUN}};
        auto end_pos = user_input_[PuriMoveTube::EndPos].get<uint16_t>();
        if (end_pos == (uint16_t)PurificationArea::CENTRIFUGE_AREA) {
            locked_equipment.push_back({machine_type_, EquipmentType::CENTRIFUGE});
        }
        if (end_pos == (uint16_t)PurificationArea::MAGNETIC_AREA) {
            locked_equipment.push_back({machine_type_, EquipmentType::MAGNETIC_RACK});
        }
        if (end_pos == (uint16_t)PurificationArea::SHAKING_AREA) {
            locked_equipment.push_back({machine_type_, EquipmentType::HEATER_SHAKER});
        }
        if (end_pos == (uint16_t)PurificationArea::THERMAL_CYCLING_AREA) {
            locked_equipment.push_back({machine_type_, EquipmentType::PCR});
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
            {machine_type_, EquipmentType::ROBOT_ARM}, {machine_type_, EquipmentType::PIPETEE_GUN}};
        auto start_pos = user_input_[PuriMoveTube::StartPos].get<uint16_t>();

        if (start_pos == (uint16_t)PurificationArea::AUTO) {
            auto tube_id = user_input_[PuriMoveTube::Tube].get<TubeId>();
            auto tube    = reality.getTube(tube_id);
            start_pos = reality.getTubeLastPosition(tube_id, TubePositionType::WHOLE_TUBE).area_id;
        }

        if (start_pos == (uint16_t)PurificationArea::CENTRIFUGE_AREA) {
            unlocked_equipment.push_back({machine_type_, EquipmentType::CENTRIFUGE});
        }
        if (start_pos == (uint16_t)PurificationArea::MAGNETIC_AREA) {
            unlocked_equipment.push_back({machine_type_, EquipmentType::MAGNETIC_RACK});
        }
        if (start_pos == (uint16_t)PurificationArea::SHAKING_AREA) {
            unlocked_equipment.push_back({machine_type_, EquipmentType::HEATER_SHAKER});
        }
        if (start_pos == (uint16_t)PurificationArea::THERMAL_CYCLING_AREA) {
            unlocked_equipment.push_back({machine_type_, EquipmentType::PCR});
        }

        return unlocked_equipment;
    }

    std::vector<std::shared_ptr<Step>>
    getTubeTypeSupportsSteps(Reality& reality, std::map<TubeId, TubeId>& tube_map) {
        auto new_params = user_input_;
        if (user_input_.find(PuriMoveTube::Tube) == user_input_.end()) {
            return {std::make_shared<PuriMoveTubeStep>(name_, std::move(new_params))};
        }
        auto tube_id  = user_input_[PuriMoveTube::Tube].get<TubeId>();
        auto tube_ptr = reality.getTube(tube_id);

        if (TubeManager::getTubeType(tube_ptr) == TubeType::PCR_TUBE) {
            // change according to tube_map
            if (tube_map.find(tube_id) != tube_map.end()) {
                tube_id = tube_map[tube_id];
            }
            new_params[PuriMoveTube::Tube] = tube_id;
        }
        return {std::make_shared<PuriMoveTubeStep>(name_, std::move(new_params))};
    }

    static std::shared_ptr<PuriMoveTubeStep> fromDummy(DummyStep& dummy_step) {
        if (dummy_step.getType() == Dummy::DummyType::MOVE_TUBE || dummy_step.getType() == Dummy::DummyType::MOVE_PCR_TUBE){
            auto&& vars = PuriMoveTube::fromDummy(dummy_step);
            if (vars[PuriMoveTube::EndPos] == (uint16_t)PurificationArea::AUTO) {
                return nullptr;
            }
            return std::make_shared<PuriMoveTubeStep>(PuriMoveTube::Name, std::move(vars));
        }
        return nullptr;
    }

    static std::shared_ptr<PuriMoveTubeStep> fromDummyTube(TubeId tube_id) {
        Variables vars;
        vars[PuriMoveTube::Tube]       = tube_id;
        vars[PuriMoveTube::StartPos]   = (uint16_t)PurificationArea::AUTO;
        vars[PuriMoveTube::StartIndex] = (uint16_t)CommonIndexId::AUTO;
        vars[PuriMoveTube::EndPos]     = (uint16_t)CommonAreaId::REACTION;
        vars[PuriMoveTube::EndIndex]   = (uint16_t)CommonIndexId::AUTO;
        return std::make_shared<PuriMoveTubeStep>(PuriMoveTube::Name, std::move(vars));
    }
};

class PuriMoveTubeStage : public TemplatedStage<PuriMoveTube> {
public:
    PuriMoveTubeStage(std::string id, Variables&& input) : TemplatedStage(std::move(id), true) {
        my_step_ = std::make_shared<PuriMoveTubeStep>(name_, std::move(input));
    }

    void generateWorkflowHelper(Workflow& workflow) {
        if (next_stages_.size() != 1) {
            return;
        }
        genWorkflowForNextStage(workflow, PuriMoveTube::kOutput);
    }
};