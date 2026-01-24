#pragma once

#include "machine/common.hpp"
#include "procedure/stage.hpp"
#include "process/dummy.hpp"

class Portage {
public:
    enum { kOutput = 0 };
    inline static const std::string Name = "Portage";
};

class PortageStep : public CRTPStep<PortageStep> {
public:
    PortageStep(std::string name, Variables&& user_input = Variables())
        : CRTPStep<PortageStep>(MachineType::PORTAGE, name, std::move(user_input)) {
        step_funcs_ = {std::bind(&PortageStep::phase0, this, std::placeholders::_1,
                                 std::placeholders::_2, std::placeholders::_3),
                       std::bind(&PortageStep::phase1, this, std::placeholders::_1,
                                 std::placeholders::_2, std::placeholders::_3),
                       std::bind(&PortageStep::phase2, this, std::placeholders::_1,
                                 std::placeholders::_2, std::placeholders::_3),
                       std::bind(&PortageStep::phase3, this, std::placeholders::_1,
                                 std::placeholders::_2, std::placeholders::_3)};
    }

    std::vector<ExecutionResult>
    phase0(Reality& reality, std::shared_ptr<MachineManager> mac_manager, ActionId action_id) {
        auto          machine = mac_manager->getMachine<PortageModbusMachine>(machine_type_);
        MachineTypeId to      = next_steps_[Portage::kOutput]->getMachineType();
        std::string   carrier_sn;
        if (user_input_.empty()) {
            carrier_sn = step_input_[Carrier::NAME].get<std::string>();
        } else {
            if (user_input_.find(Step::Tube) != user_input_.end()) {
                auto tube     = user_input_[Step::Tube].get<TubeId>();
                auto tube_ptr = reality.getTube(tube);
                auto carrier  = TubeManager::getTubeCarrier(tube_ptr);
                carrier_sn    = carrier->getName();
            } else {
                carrier_sn = user_input_[Carrier::NAME].get<std::string>();
            }
        }

        auto          carrier = reality.getCarrier(carrier_sn);
        MachineTypeId from    = carrier->getMachineType();

        auto from_machine = mac_manager->getMachine<ModbusMachine>(from);

        if (from_machine && from_machine->supportAutomicService()) {
            from_machine->requestPortageTransfer(action_id);
        }

        // carrier->setMachineType(to);

        logger->debug("Portage from {} to {}", MachineNames[from], MachineNames[to]);
        if (to == (MachineTypeId)MachineType::TOTAL_NUM) {
        } else if (from != (MachineTypeId)MachineType::TOTAL_NUM) {
            machine->autoPortage(MachinePortageIds[from], MachinePortageIds[to], action_id);
        } else {
            machine->manualPortage(MachinePortageIds[to], carrier_sn, action_id);
        }

        if (from_machine && from_machine->supportAutomicService()) {
            return {ExecutionResult()};
        } else {
            return {ExecutionResult{true}};
        }

        return {ExecutionResult()};
    }

    std::vector<ExecutionResult>
    phase1(Reality& reality, std::shared_ptr<MachineManager> mac_manager, ActionId action_id) {
        std::vector<ExecutionResult> results;
        MachineTypeId                to = next_steps_[Portage::kOutput]->getMachineType();
        if (to == (MachineTypeId)MachineType::TOTAL_NUM) {
            Variables output = user_input_.empty() ? step_input_ : user_input_;
            results.push_back(
                ExecutionResult{next_steps_[Portage::kOutput], std::move(output), false});
        } else {
            results.push_back(ExecutionResult());
        }

        return results;
    }

    std::vector<ExecutionResult>
    phase2(Reality& reality, std::shared_ptr<MachineManager> mac_manager, ActionId action_id) {
        std::vector<ExecutionResult> results;

        auto manager = std::dynamic_pointer_cast<MachineManager>(mac_manager);
        auto machine = manager->getMachine<PortageModbusMachine>(machine_type_);
        machine->onPortageFinished();

        if (next_steps_.empty()) {
            logger->error("PortageStep has no next step set");
            return {ExecutionResult{true}};
        }

        MachineTypeId to         = next_steps_[Portage::kOutput]->getMachineType();
        auto          to_machine = mac_manager->getMachine<ModbusMachine>(to);

        // auto carrier = reality.getCarrier(carrier_sn);
        // carrier->setMachineType(to);
        if (to_machine && to_machine->supportAutomicService()) {
            to_machine->acceptPortageTransfer(action_id);
            results.push_back(ExecutionResult());
            return results;
        }

        return {ExecutionResult{true}};
    }

    std::vector<ExecutionResult>
    phase3(Reality& reality, std::shared_ptr<MachineManager> mac_manager, ActionId action_id) {
        std::vector<ExecutionResult> results;
        Variables                    output;
        if (user_input_.empty()) {
            output = step_input_;
        } else {
            output = user_input_;
        }
        results.push_back(ExecutionResult{next_steps_[Portage::kOutput], std::move(output), false});
        return results;
    }

    std::vector<std::pair<MachineType, EquipmentType>>
    getNeedLockEquipment(Reality& reality, std::shared_ptr<MachineManager> mac_manager_,
                         std::shared_ptr<Action> action) const {
        MachineTypeId to = next_steps_.at(Portage::kOutput)->getMachineType();

        auto machine_to = mac_manager_->getMachine<Machine>(to);

        std::string carrier_sn;
        if (user_input_.empty()) {
            carrier_sn = step_input_[Carrier::NAME].get<std::string>();
        } else {
            if (user_input_.find(Step::Tube) != user_input_.end()) {
                auto tube     = user_input_[Step::Tube].get<TubeId>();
                auto tube_ptr = reality.getTube(tube);
                auto carrier  = TubeManager::getTubeCarrier(tube_ptr);
                carrier_sn    = carrier->getName();
            } else {
                carrier_sn = user_input_[Carrier::NAME].get<std::string>();
            }
        }
        auto          carrier = reality.getCarrier(carrier_sn);
        MachineTypeId from    = carrier->getMachineType();

        auto machine_from = mac_manager_->getMachine<Machine>(from);

        std::vector<std::pair<MachineType, EquipmentType>> results;

        if (action->getCurrentPhase() == 0) {
            std::shared_ptr<Step> step = std::make_shared<Step>(*this);
            results.push_back(
                std::make_pair((MachineType)machine_to->getType(), EquipmentType::ENTER_POS));

            if (machine_from) {
                results.push_back(
                    std::make_pair((MachineType)machine_from->getType(), EquipmentType::ROBOT_ARM));
                results.push_back(std::make_pair((MachineType)machine_from->getType(),
                                                 EquipmentType::PIPETEE_GUN));
            }

            auto machine_portage = mac_manager_->getMachine<PortageModbusMachine>(machine_type_);
            results.push_back(
                std::make_pair((MachineType)machine_portage->getType(), EquipmentType::REALM));
        }

        if (action->getCurrentPhase() == 2) {
            if (machine_to) {
                std::shared_ptr<Step> step = std::make_shared<Step>(*this);
                // ensure same order to avoid dead lock
                results.push_back(
                    std::make_pair((MachineType)machine_to->getType(), EquipmentType::ROBOT_ARM));
                results.push_back(
                    std::make_pair((MachineType)machine_to->getType(), EquipmentType::PIPETEE_GUN));
            }
        }

        return results;
    }

    std::vector<std::pair<MachineType, EquipmentType>>
    getNeedUnlockEquipment(Reality& reality, std::shared_ptr<MachineManager> mac_manager_,
                           std::shared_ptr<Action> action) const {
        std::string carrier_sn;
        if (user_input_.empty()) {
            carrier_sn = step_input_[Carrier::NAME].get<std::string>();
        } else {
            if (user_input_.find(Step::Tube) != user_input_.end()) {
                auto tube     = user_input_[Step::Tube].get<TubeId>();
                auto tube_ptr = reality.getTube(tube);
                auto carrier  = TubeManager::getTubeCarrier(tube_ptr);
                carrier_sn    = carrier->getName();
            } else {
                carrier_sn = user_input_[Carrier::NAME].get<std::string>();
            }
        }
        auto          carrier = reality.getCarrier(carrier_sn);
        MachineTypeId from    = carrier->getMachineType();

        auto machine_from = mac_manager_->getMachine<Machine>(from);

        auto machine_portage = mac_manager_->getMachine<PortageModbusMachine>(machine_type_);
        auto machine_to =
            mac_manager_->getMachine<Machine>(next_steps_.at(Portage::kOutput)->getMachineType());

        std::vector<std::pair<MachineType, EquipmentType>> results;
        if (action->getCurrentPhase() == 1) { // phase 0 is done
            // do nothing
        } else if (action->getCurrentPhase() == 2) { // phase 1 is done
        } else if (action->getCurrentPhase() == 3) { // phase 2 is done
        }

        if (action->getCurrentPhase() == 3 && action->isDone()) { // phase 3 is done
            if (machine_from) {
                results.push_back(
                    std::make_pair((MachineType)machine_from->getType(), EquipmentType::EXIT_POS));
                results.push_back(
                    std::make_pair((MachineType)machine_from->getType(), EquipmentType::ROBOT_ARM));
                results.push_back(std::make_pair((MachineType)machine_from->getType(),
                                                 EquipmentType::PIPETEE_GUN));
            }
            if (machine_portage) {
                results.push_back(
                    std::make_pair((MachineType)machine_portage->getType(), EquipmentType::REALM));
            }
            if (machine_to) {
                results.push_back(
                    std::make_pair((MachineType)machine_to->getType(), EquipmentType::ROBOT_ARM));
                results.push_back(
                    std::make_pair((MachineType)machine_to->getType(), EquipmentType::PIPETEE_GUN));
            }
        }

        return results;
    }
};

class PortageStage : public TemplatedStage<Portage> {
public:
    PortageStage(std::string id, Variables&& input) : TemplatedStage(std::move(id), true) {
        my_step_ = std::make_shared<PortageStep>(name_, std::move(input));
    }

    void generateWorkflowHelper(Workflow& workflow) {
        if (next_stages_.size() != 1) {
            return;
        }
        auto next_stage = next_stages_[Portage::kOutput];
        auto next_step  = next_stage->getMyStep();
        next_stage->generateWorkflow(workflow);
        my_step_->setNextStep(Portage::kOutput, next_step);
    }
};