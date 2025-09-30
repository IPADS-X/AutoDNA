#pragma once

#include "machine/common.hpp"
#include "procedure/stage.hpp"
#include "process/dummy.hpp"
#include "process/library/pipette.hpp"

class LibMoveCarrier {
public:
    enum { kOutput = 0 };
    inline static const std::string Name     = "LibMoveCarrier";
    inline static const std::string StartPos = "StartPos";
    inline static const std::string EndPos   = "EndPos";
    inline static const std::string Carrier  = "Carrier"; // Carrier SN
    inline static const std::string Tube     = Step::Tube;
};

class LibMoveCarrierStep : public CRTPStep<LibMoveCarrierStep> {
public:
    LibMoveCarrierStep(std::string name, Variables&& user_input = Variables())
        : CRTPStep<LibMoveCarrierStep>(MachineType::LIBRARY, name, std::move(user_input)) {
        step_funcs_ = {std::bind(&LibMoveCarrierStep::phase0, this, std::placeholders::_1,
                                 std::placeholders::_2, std::placeholders::_3),
                       std::bind(&LibMoveCarrierStep::phase1, this, std::placeholders::_1,
                                 std::placeholders::_2, std::placeholders::_3)};
    }

    std::vector<ExecutionResult>
    phase0(Reality& reality, std::shared_ptr<MachineManager> mac_manager, ActionId action_id) {
        std::vector<ExecutionResult> results;
        auto machine = mac_manager->getMachine<LibraryModbusMachine>(machine_type_);

        auto start_pos = user_input_[LibMoveCarrier::StartPos].get<uint16_t>();
        auto end_pos   = user_input_[LibMoveCarrier::EndPos].get<uint16_t>();

        if (start_pos == (uint16_t)LibraryArea::AUTO || end_pos == (uint16_t)LibraryArea::AUTO) {
            Carrier*                 carrier;
            std::shared_ptr<Carrier> carrier_ptr;
            if (user_input_.find(LibMoveCarrier::Tube) != user_input_.end()) {
                auto tube     = user_input_[LibMoveCarrier::Tube].get<TubeId>();
                auto tube_ptr = reality.getTube(tube);
                carrier       = TubeManager::getTubeCarrier(tube_ptr);
                carrier_ptr   = reality.getCarrier(carrier->getName());
            } else {
                auto carrierSN = user_input_[LibMoveCarrier::Carrier].get<std::string>();
                carrier        = reality.getCarrier(carrierSN).get();
                carrier_ptr    = reality.getCarrier(carrier->getName());
            }

            if (start_pos == (uint16_t)LibraryArea::AUTO) {
                start_pos = carrier->getAreaId();
                machine->releasePlaceArea(start_pos);
            }

            if (end_pos == (uint16_t)LibraryArea::AUTO) {
                auto alloc = machine->allocNewArea(carrier_ptr);
                if (alloc.first) {
                    end_pos = alloc.second;
                }
            }

            if (end_pos == (uint16_t)LibraryArea::AUTO) {
                end_pos = (uint16_t)LibraryArea::SAMPLE_AREA;
            }

            if (start_pos == (uint16_t)-1) {
                start_pos = (uint16_t)LibraryArea::SAMPLE_AREA;
                carrier->setMachineType((MachineTypeId)MachineType::LIBRARY);
            }

            if (end_pos == (uint16_t)-1) {
                end_pos = (uint16_t)LibraryArea::SAMPLE_AREA;
                carrier->setMachineType((MachineTypeId)MachineType::LIBRARY);
            }

            carrier->setMachineType((MachineTypeId)MachineType::LIBRARY);
            carrier->setAreaId(end_pos);
        }

        machine->move_carrier(start_pos, end_pos, action_id);

        results.push_back(ExecutionResult());
        return results;
    }

    std::vector<ExecutionResult>
    phase1(Reality& reality, std::shared_ptr<MachineManager> mac_manager, ActionId action_id) {
        std::vector<ExecutionResult> results;

        results.push_back(
            ExecutionResult{next_steps_[LibMoveCarrier::kOutput], Variables(), false});
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
        auto end_pos = user_input_[LibMoveCarrier::EndPos].get<uint16_t>();
        if (end_pos == (uint16_t)LibraryArea::EXIT_AREA) {
            locked_equipment.push_back(std::make_pair(machine_type_, EquipmentType::EXIT_POS));
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
        auto start_pos = user_input_[LibMoveCarrier::StartPos].get<uint16_t>();
        if (start_pos == (uint16_t)LibraryArea::ENTER_AREA) {
            unlocked_equipment.push_back(std::make_pair(machine_type_, EquipmentType::ENTER_POS));
        }
        return unlocked_equipment;
    }

    TubeType getTargetType() { return TubeType::STRIP_TUBE; }

    std::vector<std::shared_ptr<Step>>
    getTubeTypeSupportsSteps(Reality& reality, std::map<TubeId, TubeId>& tube_map) {
        auto new_params = user_input_;
        if (user_input_.find(LibMoveCarrier::Tube) == user_input_.end()) {
            return {std::make_shared<LibMoveCarrierStep>(name_, std::move(new_params))};
        }
        auto tube_id  = user_input_[LibMoveCarrier::Tube].get<TubeId>();
        auto tube_ptr = reality.getTube(tube_id);

        auto tube_changed_id = tube_map.find(tube_id);
        auto real_id         = tube_id;
        if (tube_changed_id != tube_map.end()) {
            real_id = tube_changed_id->second;
        }

        if (TubeManager::getTubeType(tube_ptr) == TubeType::PCR_TUBE) {
            // change tube_map
            // alloc a new tmp strip-tube
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

            new_params[LibMoveCarrier::Tube] = new_id;
            res.push_back(
                std::make_shared<LibMoveCarrierStep>(LibMoveCarrier::Name, std::move(new_params)));

            // change according to tube_map
            tube_map[tube_id] = new_id;
            return res;
        }
        return {std::make_shared<LibMoveCarrierStep>(name_, std::move(new_params))};
    }

    static std::shared_ptr<Step> fromDummy(TubeId tube_id, AreaId area_id, AreaId from_area_id) {
        Variables user_input;
        user_input[LibMoveCarrier::Tube] = tube_id;
        if (from_area_id == (AreaId)CommonAreaId::ENTER_AREA) {
            user_input[LibMoveCarrier::StartPos] = LibraryArea::ENTER_AREA;
        } else if (from_area_id == (AreaId)CommonAreaId::EXIT_AREA) {
            user_input[LibMoveCarrier::StartPos] = LibraryArea::EXIT_AREA;
        } else if (from_area_id == (AreaId)CommonAreaId::AUTO) {
            user_input[LibMoveCarrier::StartPos] = LibraryArea::AUTO;
        } else {
            user_input[LibMoveCarrier::StartPos] = from_area_id;
        }
        if (area_id == (AreaId)CommonAreaId::ENTER_AREA) {
            user_input[LibMoveCarrier::EndPos] = LibraryArea::ENTER_AREA;
        } else if (area_id == (AreaId)CommonAreaId::EXIT_AREA) {
            user_input[LibMoveCarrier::EndPos] = LibraryArea::EXIT_AREA;
        } else if (area_id == (AreaId)CommonAreaId::AUTO) {
            user_input[LibMoveCarrier::EndPos] = LibraryArea::AUTO;
        } else {
            user_input[LibMoveCarrier::EndPos] = area_id;
        }

        return std::make_shared<LibMoveCarrierStep>(LibMoveCarrier::Name, std::move(user_input));
    }
};

class LibMoveCarrierStage : public TemplatedStage<LibMoveCarrier> {
public:
    LibMoveCarrierStage(std::string id, Variables&& input) : TemplatedStage(std::move(id), true) {
        my_step_ = std::make_shared<LibMoveCarrierStep>(name_, std::move(input));
    }

    void generateWorkflowHelper(Workflow& workflow) {
        if (next_stages_.size() != 1) {
            return;
        }
        genWorkflowForNextStage(workflow, LibMoveCarrier::kOutput);
    }
};