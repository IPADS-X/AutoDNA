#pragma once

#include "procedure/stage.hpp"
#include "process/dummy.hpp"

class LibMoveTube {
public:
    enum { kOutput = 0 };
    inline static const std::string Name       = "LibMoveTube";
    inline static const std::string StartPos   = "StartPos";
    inline static const std::string StartIndex = "StartIndex";
    inline static const std::string EndPos     = "EndPos";
    inline static const std::string EndIndex   = "EndIndex";
    inline static const std::string Type       = "Type"; // 0: 8-tube, 1: pcr-tube
    inline static const std::string Tube       = Step::Tube;

    using MoveTubeType = TubeType;

    static nlohmann::json fromDummy(const DummyStep& dummy_step) {
        nlohmann::json input        = nlohmann::json::object();
        auto           dummy_params = dummy_step.getParams();
        input[Tube] = dummy_params[std::to_string(static_cast<int>(Dummy::ParamType::TUBE_INDEX))];

        if (dummy_params.contains(
                std::to_string(static_cast<int>(Dummy::ParamType::START_POSITION)))) {
            input[StartPos] =
                dummy_params[std::to_string(static_cast<int>(Dummy::ParamType::START_POSITION))];
        } else {
            input[StartPos] = (uint16_t)LibraryArea::AUTO;
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
            if (pos == Dummy::DummyEquipment::FLUORESCENCE) {
                input[EndPos] = (uint16_t)LibraryArea::FLUOROMETER_AREA;
            } else if (pos == Dummy::DummyEquipment::CENTRIFUGE) {
                input[EndPos] = (uint16_t)LibraryArea::TUBE_CENTRIFUGE_AREA;
            } else if (pos == Dummy::DummyEquipment::METAL_BATH) {
                input[EndPos] = (uint16_t)LibraryArea::HIGH_TEMP_AREA;
            } else if (pos == Dummy::DummyEquipment::MAGNETIC_RACK) {
                input[EndPos] = (uint16_t)LibraryArea::MAGNETIC_RACK_AREA;
            } else if (pos == Dummy::DummyEquipment::MIXER) {
                input[EndPos] = (uint16_t)LibraryArea::MIXER_AREA;
            } else if (pos == Dummy::DummyEquipment::REACTION_POS) {
                input[EndPos] = (uint16_t)CommonAreaId::REACTION;
            } else {
                input[EndPos] = (uint16_t)LibraryArea::AUTO;
            }
        } else {
            input[EndPos] = (uint16_t)LibraryArea::AUTO;
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

class LibMoveTubeStep : public CRTPStep<LibMoveTubeStep> {
public:
    LibMoveTubeStep(std::string name, Variables&& user_input = Variables())
        : CRTPStep<LibMoveTubeStep>(MachineType::LIBRARY, name, std::move(user_input)) {
        step_funcs_ = {std::bind(&LibMoveTubeStep::phase0, this, std::placeholders::_1,
                                 std::placeholders::_2, std::placeholders::_3),
                       std::bind(&LibMoveTubeStep::phase1, this, std::placeholders::_1,
                                 std::placeholders::_2, std::placeholders::_3)};
    }

    long long getTime() const { return 30; }

    std::vector<ExecutionResult>
    phase0(Reality& reality, std::shared_ptr<MachineManager> mac_manager, ActionId action_id) {
        std::vector<ExecutionResult> results;
        auto machine = mac_manager->getMachine<LibraryModbusMachine>(machine_type_);

        auto start_pos   = user_input_[LibMoveTube::StartPos].get<uint16_t>();
        auto start_index = user_input_[LibMoveTube::StartIndex].get<uint16_t>();
        auto end_pos     = user_input_[LibMoveTube::EndPos].get<uint16_t>();
        auto end_index   = user_input_[LibMoveTube::EndIndex].get<uint16_t>();

        if (start_pos == (uint16_t)LibraryArea::AUTO) {
            auto tube     = user_input_[LibMoveTube::Tube].get<TubeId>();
            auto tube_pos = reality.getTubePosition(tube, TubePositionType::WHOLE_TUBE);

            start_pos   = tube_pos.area_id;
            start_index = tube_pos.index;
        }

        if (end_index == (uint16_t)CommonIndexId::AUTO && end_pos != (uint16_t)LibraryArea::AUTO) {
            auto tube_id = user_input_[LibMoveTube::Tube].get<TubeId>();
            auto tube    = reality.getTube(tube_id);
            if (end_pos != (uint16_t)CommonAreaId::REACTION) {
                if (end_pos == (uint16_t)LibraryArea::HIGH_TEMP_AREA) {
                    end_index =
                        machine->getEquipment(EquipmentType::METAL_BATH)->getContainerId(tube);
                }
                if (end_pos == (uint16_t)LibraryArea::FLUOROMETER_AREA) {
                    end_index =
                        machine->getEquipment(EquipmentType::FLUORESCENCE)->getContainerId(tube);
                }
                if (end_pos == (uint16_t)LibraryArea::MAGNETIC_RACK_AREA) {
                    end_index =
                        machine->getEquipment(EquipmentType::MAGNETIC_RACK)->getContainerId(tube);
                }
                if (end_pos == (uint16_t)LibraryArea::MIXER_AREA) {
                    end_index = machine->getEquipment(EquipmentType::MIXER)->getContainerId(tube);
                }
                if (end_pos == (uint16_t)LibraryArea::TUBE_CENTRIFUGE_AREA) {
                    if (TubeManager::getTubeType(tube) == TubeType::PCR_TUBE) {
                        end_pos = (uint16_t)LibraryArea::PCR_STRIP_CENTRIFUGE_AREA;
                    } else {
                        end_index =
                            machine->getEquipment(EquipmentType::CENTRIFUGE)->getContainerId(tube);
                    }
                }
                if (end_pos == (uint16_t)LibraryArea::PCR_STRIP_CENTRIFUGE_AREA) {
                    end_index =
                        machine->getEquipment(EquipmentType::CENTRIFUGE_PCR)->getContainerId(tube);
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

        if (user_input_[LibMoveTube::StartPos].get<uint16_t>() == (uint16_t)LibraryArea::AUTO) {
            auto tube_id = user_input_[LibMoveTube::Tube].get<TubeId>();
            auto tube    = reality.getTube(tube_id);
            if (TubeManager::getTubeType(tube) == TubeType::PCR_TUBE) {
                machine->move_pcr_tube(start_pos, start_index, end_pos, end_index, action_id);
            } else {
                machine->move_tube(start_pos, start_index, end_pos, end_index, action_id);
            }
        } else {
            if (user_input_[LibMoveTube::Type].get<uint16_t>() ==
                (uint16_t)LibMoveTube::MoveTubeType::PCR_TUBE) {
                machine->move_pcr_tube(start_pos, start_index, end_pos, end_index, action_id);
            } else {
                machine->move_tube(start_pos, start_index, end_pos, end_index, action_id);
            }
        }

        results.push_back(ExecutionResult());
        return results;
    }

    std::vector<ExecutionResult>
    phase1(Reality& reality, std::shared_ptr<MachineManager> mac_manager, ActionId action_id) {
        std::vector<ExecutionResult> results;

        results.push_back(ExecutionResult{next_steps_[LibMoveTube::kOutput], Variables(), false});
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
        auto end_pos = user_input_[LibMoveTube::EndPos].get<uint16_t>();
        auto tube_id = user_input_[LibMoveTube::Tube].get<TubeId>();
        auto tube    = reality.getTube(tube_id);
        if (end_pos == (uint16_t)LibraryArea::HIGH_TEMP_AREA) {
            locked_equipment.push_back({machine_type_, EquipmentType::METAL_BATH});
        }
        if (end_pos == (uint16_t)LibraryArea::FLUOROMETER_AREA) {
            locked_equipment.push_back({machine_type_, EquipmentType::FLUORESCENCE});
        }
        if (end_pos == (uint16_t)LibraryArea::MAGNETIC_RACK_AREA) {
            locked_equipment.push_back({machine_type_, EquipmentType::MAGNETIC_RACK});
        }
        if (end_pos == (uint16_t)LibraryArea::MIXER_AREA) {
            locked_equipment.push_back({machine_type_, EquipmentType::MIXER});
        }
        if (end_pos == (uint16_t)LibraryArea::TUBE_CENTRIFUGE_AREA) {
            if (TubeManager::getTubeType(tube) == TubeType::PCR_TUBE) {
                end_pos = (uint16_t)LibraryArea::PCR_STRIP_CENTRIFUGE_AREA;
            } else {
                locked_equipment.push_back({machine_type_, EquipmentType::CENTRIFUGE});
            }
        }
        if (end_pos == (uint16_t)LibraryArea::PCR_STRIP_CENTRIFUGE_AREA) {
            locked_equipment.push_back({machine_type_, EquipmentType::CENTRIFUGE_PCR});
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
        auto start_pos = user_input_[LibMoveTube::StartPos].get<uint16_t>();

        if (start_pos == (uint16_t)LibraryArea::AUTO) {
            auto tube_id = user_input_[LibMoveTube::Tube].get<TubeId>();
            auto tube    = reality.getTube(tube_id);
            start_pos = reality.getTubeLastPosition(tube_id, TubePositionType::WHOLE_TUBE).area_id;
        }
        if (start_pos == (uint16_t)LibraryArea::HIGH_TEMP_AREA) {
            unlocked_equipment.push_back({machine_type_, EquipmentType::METAL_BATH});
        }
        if (start_pos == (uint16_t)LibraryArea::FLUOROMETER_AREA) {
            unlocked_equipment.push_back({machine_type_, EquipmentType::FLUORESCENCE});
        }
        if (start_pos == (uint16_t)LibraryArea::MAGNETIC_RACK_AREA) {
            unlocked_equipment.push_back({machine_type_, EquipmentType::MAGNETIC_RACK});
        }
        if (start_pos == (uint16_t)LibraryArea::MIXER_AREA) {
            unlocked_equipment.push_back({machine_type_, EquipmentType::MIXER});
        }
        if (start_pos == (uint16_t)LibraryArea::TUBE_CENTRIFUGE_AREA) {
            unlocked_equipment.push_back({machine_type_, EquipmentType::CENTRIFUGE});
        }
        if (start_pos == (uint16_t)LibraryArea::PCR_STRIP_CENTRIFUGE_AREA) {
            unlocked_equipment.push_back({machine_type_, EquipmentType::CENTRIFUGE_PCR});
        }
        return unlocked_equipment;
    }

    TubeType getTargetType() {
        auto end_pos = user_input_[LibMoveTube::EndPos].get<uint16_t>();

        if (end_pos == (uint16_t)LibraryArea::HIGH_TEMP_AREA ||
            end_pos == (uint16_t)LibraryArea::MAGNETIC_RACK_AREA) {
            return TubeType::PCR_TUBE;
        }

        if (end_pos == (uint16_t)LibraryArea::FLUOROMETER_AREA ||
            end_pos == (uint16_t)LibraryArea::MIXER_AREA ||
            end_pos == (uint16_t)LibraryArea::SAMPLE_AREA) {
            return TubeType::STRIP_TUBE;
        }

        return TubeType::UNKNOWN;
    }

    std::vector<std::shared_ptr<Step>>
    getTubeTypeSupportsSteps(Reality& reality, std::map<TubeId, TubeId>& tube_map) {
        auto new_params = user_input_;
        if (user_input_.find(LibMoveTube::Tube) == user_input_.end()) {
            return {std::make_shared<LibMoveTubeStep>(name_, std::move(new_params))};
        }
        auto tube_id  = user_input_[LibMoveTube::Tube].get<TubeId>();
        auto tube_ptr = reality.getTube(tube_id);
        auto end_pos  = user_input_[LibMoveTube::EndPos].get<uint16_t>();

        auto tube_changed_id = tube_map.find(tube_id);
        auto real_id         = tube_id;
        if (tube_changed_id != tube_map.end()) {
            real_id = tube_changed_id->second;
        }

        if (TubeManager::getTubeType(tube_ptr) == TubeType::STRIP_TUBE &&
            (end_pos == (uint16_t)LibraryArea::HIGH_TEMP_AREA ||
             end_pos == (uint16_t)LibraryArea::MAGNETIC_RACK_AREA ||
             end_pos == (uint16_t)LibraryArea::TUBE_CENTRIFUGE_AREA)) {
            auto new_tube = reality.createTube("", TubeType::PCR_TUBE);
            auto new_id   = TubeManager::getTubeId(new_tube);

            // move all volume to new tube
            std::vector<std::shared_ptr<Step>> res;

            for (int i = 0; i < 8; i++) {
                res.push_back(std::make_shared<LibPipetteStep>(
                    LibPipette::Name, Variables{{LibPipette::SrcTube, real_id},
                                                {LibPipette::DstTube, new_id},
                                                {LibPipette::StartIndex, i},
                                                {LibPipette::EndIndex, 0},
                                                {LibPipette::Volume, CommonVolumeId::AUTO}}));
            }

            new_params[LibMoveTube::Tube] = new_id;
            res.push_back(
                std::make_shared<LibMoveTubeStep>(LibMoveTube::Name, std::move(new_params)));
            tube_map[tube_id] = new_id;
            return res;
        }

        if (TubeManager::getTubeType(tube_ptr) == TubeType::PCR_TUBE &&
            (end_pos == (uint16_t)LibraryArea::FLUOROMETER_AREA ||
             end_pos == (uint16_t)LibraryArea::MIXER_AREA ||
             end_pos == (uint16_t)LibraryArea::SAMPLE_AREA)) {
            auto new_tube = reality.createTube("", TubeType::STRIP_TUBE);
            auto new_id   = TubeManager::getTubeId(new_tube);

            // move all volume to new tube
            std::vector<std::shared_ptr<Step>> res;

            for (int i = 0; i < 8; i++) {
                res.push_back(std::make_shared<LibPipetteStep>(
                    LibPipette::Name,
                    Variables{{LibPipette::SrcTube, real_id},
                              {LibPipette::DstTube, new_id},
                              {LibPipette::StartIndex, 0},
                              {LibPipette::EndIndex, i},
                              {LibPipette::Volume, (int)(CommonVolumeId::AUTO) + (8 - i)}}));
            }

            new_params[LibMoveTube::Tube] = new_id;
            res.push_back(
                std::make_shared<LibMoveTubeStep>(LibMoveTube::Name, std::move(new_params)));
            tube_map[tube_id] = new_id;
            return res;
        }

        return {std::make_shared<LibMoveTubeStep>(name_, std::move(new_params))};
    }

    static std::shared_ptr<LibMoveTubeStep> fromDummy(DummyStep& dummy_step) {
        if (dummy_step.getType() == Dummy::DummyType::MOVE_TUBE || dummy_step.getType() == Dummy::DummyType::MOVE_PCR_TUBE) {
            auto&& vars = LibMoveTube::fromDummy(dummy_step);
            if (vars[LibMoveTube::EndPos] == (uint16_t)LibraryArea::AUTO) {
                return nullptr;
            }
            return std::make_shared<LibMoveTubeStep>(LibMoveTube::Name, std::move(vars));
        }
        return nullptr;
    }

    static std::shared_ptr<LibMoveTubeStep> fromDummyTube(TubeId tube_id) {
        Variables vars;
        vars[LibMoveTube::Tube]       = tube_id;
        vars[LibMoveTube::StartPos]   = (uint16_t)LibraryArea::AUTO;
        vars[LibMoveTube::StartIndex] = (uint16_t)CommonIndexId::AUTO;
        vars[LibMoveTube::EndPos]     = (uint16_t)CommonAreaId::REACTION;
        vars[LibMoveTube::EndIndex]   = (uint16_t)CommonIndexId::AUTO;
        return std::make_shared<LibMoveTubeStep>(LibMoveTube::Name, std::move(vars));
    }
};

class LibMoveTubeStage : public TemplatedStage<LibMoveTube> {
public:
    LibMoveTubeStage(std::string id, Variables&& input) : TemplatedStage(std::move(id), true) {
        my_step_ = std::make_shared<LibMoveTubeStep>(name_, std::move(input));
    }

    void generateWorkflowHelper(Workflow& workflow) {
        if (next_stages_.size() != 1) {
            return;
        }
        genWorkflowForNextStage(workflow, LibMoveTube::kOutput);
    }
};