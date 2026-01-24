#pragma once

#include "procedure/stage.hpp"
#include "process/dummy.hpp"

class PuriMoveCarrier {
public:
    enum { kOutput = 0 };
    inline static const std::string Name     = "PuriMoveCarrier";
    inline static const std::string StartPos = "StartPos";
    inline static const std::string EndPos   = "EndPos";
    inline static const std::string Carrier  = "Carrier"; // Carrier SN
    inline static const std::string Tube     = Step::Tube;
};

class PuriMoveCarrierStep : public CRTPStep<PuriMoveCarrierStep> {
public:
    PuriMoveCarrierStep(std::string name, Variables&& user_input = Variables())
        : CRTPStep<PuriMoveCarrierStep>(MachineType::PURIFICATION, name, std::move(user_input)) {
        step_funcs_ = {std::bind(&PuriMoveCarrierStep::phase0, this, std::placeholders::_1,
                                 std::placeholders::_2, std::placeholders::_3),
                       std::bind(&PuriMoveCarrierStep::phase1, this, std::placeholders::_1,
                                 std::placeholders::_2, std::placeholders::_3)};
    }

    std::vector<ExecutionResult>
    phase0(Reality& reality, std::shared_ptr<MachineManager> mac_manager, ActionId action_id) {
        std::vector<ExecutionResult> results;
        auto machine = mac_manager->getMachine<PurificationModbusMachine>(machine_type_);

        auto start_pos = user_input_[PuriMoveCarrier::StartPos].get<uint16_t>();
        auto end_pos   = user_input_[PuriMoveCarrier::EndPos].get<uint16_t>();

        if (start_pos == (uint16_t)PurificationArea::AUTO ||
            end_pos == (uint16_t)PurificationArea::AUTO) {
            Carrier* carrier;
            std::shared_ptr<Carrier> carrier_ptr;
            if (user_input_.find(PuriMoveCarrier::Tube) != user_input_.end()) {
                auto tube     = user_input_[PuriMoveCarrier::Tube].get<TubeId>();
                auto tube_ptr = reality.getTube(tube);
                carrier       = TubeManager::getTubeCarrier(tube_ptr);
                carrier_ptr   = reality.getCarrier(carrier->getName());
            } else {
                auto carrierSN = user_input_[PuriMoveCarrier::Carrier].get<std::string>();
                carrier        = reality.getCarrier(carrierSN).get();
                carrier_ptr    = reality.getCarrier(carrier->getName());
            }

            if (start_pos == (uint16_t)PurificationArea::AUTO) {
                start_pos = carrier->getAreaId();
                machine->releasePlaceArea(start_pos);
            }

            if (end_pos == (uint16_t)PurificationArea::AUTO) {
                auto alloc = machine->allocNewArea(carrier_ptr);
                if (alloc.first) {
                    end_pos = alloc.second;
                }
            }

            if (end_pos == (uint16_t)PurificationArea::AUTO) {
                end_pos = (uint16_t)PurificationArea::SAMPLE_AREA;
            }

            if (start_pos == (uint16_t)-1) {
                start_pos = (uint16_t)PurificationArea::SAMPLE_AREA;
                carrier->setMachineType((MachineTypeId)MachineType::PURIFICATION);
            }

            if (end_pos == (uint16_t)-1) {
                end_pos = (uint16_t)PurificationArea::SAMPLE_AREA;
                carrier->setMachineType((MachineTypeId)MachineType::PURIFICATION);
            }

            carrier->setMachineType((MachineTypeId)MachineType::PURIFICATION);
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
            ExecutionResult{next_steps_[PuriMoveCarrier::kOutput], Variables(), false});
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
        auto end_pos = user_input_[PuriMoveCarrier::EndPos].get<uint16_t>();
        if (end_pos == (uint16_t)PurificationArea::EXIT_AREA) {
            locked_equipment.push_back({machine_type_, EquipmentType::EXIT_POS});
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
        auto start_pos = user_input_[PuriMoveCarrier::StartPos].get<uint16_t>();
        if (start_pos == (uint16_t)PurificationArea::ENTER_AREA) {
            unlocked_equipment.push_back({machine_type_, EquipmentType::ENTER_POS});
        }
        return unlocked_equipment;
    }

    std::vector<std::shared_ptr<Step>>
    getTubeTypeSupportsSteps(Reality& reality, std::map<TubeId, TubeId>& tube_map) {
        auto new_params = user_input_;
        if (user_input_.find(PuriMoveCarrier::Tube) == user_input_.end()) {
            return {std::make_shared<PuriMoveCarrierStep>(name_, std::move(new_params))};
        }
        auto tube_id  = user_input_[PuriMoveCarrier::Tube].get<TubeId>();
        auto tube_ptr = reality.getTube(tube_id);

        if (TubeManager::getTubeType(tube_ptr) == TubeType::PCR_TUBE) {
            // change according to tube_map
            if (tube_map.find(tube_id) != tube_map.end()) {
                tube_id = tube_map[tube_id];
            }
            new_params[PuriMoveCarrier::Tube] = tube_id;
        }
        return {std::make_shared<PuriMoveCarrierStep>(name_, std::move(new_params))};
    }

    static std::shared_ptr<Step> fromDummy(TubeId tube_id, AreaId area_id, AreaId from_area_id) {
        Variables user_input;
        user_input[PuriMoveCarrier::Tube] = tube_id;
        if (from_area_id == (AreaId)CommonAreaId::ENTER_AREA) {
            user_input[PuriMoveCarrier::StartPos] = PurificationArea::ENTER_AREA;
        } else if (from_area_id == (AreaId)CommonAreaId::EXIT_AREA) {
            user_input[PuriMoveCarrier::StartPos] = PurificationArea::EXIT_AREA;
        } else if (from_area_id == (AreaId)CommonAreaId::AUTO) {
            user_input[PuriMoveCarrier::StartPos] = PurificationArea::AUTO;
        } else {
            user_input[PuriMoveCarrier::StartPos] = from_area_id;
        }
        if (area_id == (AreaId)CommonAreaId::ENTER_AREA) {
            user_input[PuriMoveCarrier::EndPos] = PurificationArea::ENTER_AREA;
        } else if (area_id == (AreaId)CommonAreaId::EXIT_AREA) {
            user_input[PuriMoveCarrier::EndPos] = PurificationArea::EXIT_AREA;
        } else if (area_id == (AreaId)CommonAreaId::AUTO) {
            user_input[PuriMoveCarrier::EndPos] = PurificationArea::AUTO;
        } else {
            user_input[PuriMoveCarrier::EndPos] = area_id;
        }

        return std::make_shared<PuriMoveCarrierStep>(PuriMoveCarrier::Name, std::move(user_input));
    }
};

class PuriMoveCarrierStage : public TemplatedStage<PuriMoveCarrier> {
public:
    PuriMoveCarrierStage(std::string id, Variables&& input) : TemplatedStage(std::move(id), true) {
        my_step_ = std::make_shared<PuriMoveCarrierStep>(name_, std::move(input));
    }

    void generateWorkflowHelper(Workflow& workflow) {
        if (next_stages_.size() != 1) {
            return;
        }
        genWorkflowForNextStage(workflow, PuriMoveCarrier::kOutput);
    }
};