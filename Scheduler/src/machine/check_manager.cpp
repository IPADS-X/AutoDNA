#include "machine/check_manager.hpp"
#include "procedure/action.hpp"
#include "procedure/step.hpp"

#include "process/amplification.hpp"
#include "process/amplification/aspirate_mix.hpp"
#include "process/amplification/move_carrier.hpp"
#include "process/amplification/move_tube.hpp"
#include "process/amplification/pcr.hpp"
#include "process/branch.hpp"
#include "process/fluorescence.hpp"
#include "process/fluorescence/aspirate_mix.hpp"
#include "process/fluorescence/captube.hpp"
#include "process/fluorescence/fluo.hpp"
#include "process/fluorescence/move_carrier.hpp"
#include "process/fluorescence/move_tube.hpp"
#include "process/fluorescence/pipette.hpp"
#include "process/forloop.hpp"
#include "process/library/aspirate_mix.hpp"
#include "process/library/centrifuge.hpp"
#include "process/library/heater.hpp"
#include "process/library/move_carrier.hpp"
#include "process/library/move_tube.hpp"
#include "process/library/pipette.hpp"
#include "process/library/timer.hpp"
#include "process/portage.hpp"
#include "process/purification.hpp"
#include "process/purification/aspirate_mix.hpp"
#include "process/purification/centrifugal.hpp"
#include "process/purification/centrifuge.hpp"
#include "process/purification/move_carrier.hpp"
#include "process/purification/move_tube.hpp"
#include "process/purification/pcr.hpp"
#include "process/purification/pipette.hpp"
#include "process/purification/shake.hpp"
#include "process/purification/timer.hpp"
#include "process/refrigerator/picking.hpp"
#include "process/refrigerator/placement.hpp"

#include "process/amplification/aspirate_mix.hpp"
#include "process/amplification/move_carrier.hpp"
#include "process/amplification/move_tube.hpp"
#include "process/amplification/pcr.hpp"
#include "process/fluorescence/aspirate_mix.hpp"
#include "process/fluorescence/move_carrier.hpp"
#include "process/fluorescence/move_tube.hpp"
#include "process/fluorescence/pipette.hpp"
#include "process/fluorescence/timer.hpp"
#include "process/library/aspirate_mix.hpp"
#include "process/library/centrifuge.hpp"
#include "process/library/heater.hpp"
#include "process/library/move_carrier.hpp"
#include "process/library/move_tube.hpp"
#include "process/library/pipette.hpp"
#include "process/library/timer.hpp"
#include "process/purification/aspirate_mix.hpp"
#include "process/purification/centrifuge.hpp"
#include "process/purification/move_carrier.hpp"
#include "process/purification/move_tube.hpp"
#include "process/purification/pcr.hpp"
#include "process/purification/pipette.hpp"
#include "process/purification/shake.hpp"
#include "process/purification/timer.hpp"

std::map<MachineType, CheckManager::CarrierFunc> CheckManager::carrier_funcs_ = {
    {MachineType::PURIFICATION, &PuriMoveCarrierStep::fromDummy},
    {MachineType::FLUORESCENCE, &FluoMoveCarrierStep::fromDummy},
    {MachineType::LIBRARY, &LibMoveCarrierStep::fromDummy},
    {MachineType::AMPLIFICATION, &AmpMoveCarrierStep::fromDummy}};

std::map<MachineType, CheckManager::MoveTubeToCarrierFunc>
    CheckManager::move_tube_to_carrier_funcs_ = {
        {MachineType::PURIFICATION, &PuriMoveTubeStep::fromDummyTube},
        {MachineType::FLUORESCENCE, &FluoMoveTubeStep::fromDummyTube},
        {MachineType::LIBRARY, &LibMoveTubeStep::fromDummyTube},
        {MachineType::AMPLIFICATION, &AmpMoveTubeStep::fromDummyTube}};

std::map<MachineType, std::map<std::string, CheckManager::StepCreator>>
    CheckManager::step_factory_ = {};

// TODO: ADD MOVE LIQUID TO OTHER TUBES
bool CheckManager::checkConsumable(Reality& reality, std::shared_ptr<MachineManager> mac_manager_,
                                   std::shared_ptr<Action>   action,
                                   std::shared_ptr<Workflow> original_workflow,
                                   CheckType check_type, SubmitFunc func) {

    auto step = action->getStep();
    if (check_pipette_tr &&
        (dynamic_cast<PuriPipetteStep*>(step.get()) || dynamic_cast<FluoPipetteStep*>(step.get()) ||
         dynamic_cast<LibPipetteStep*>(step.get()))) {
        // check consumable
        auto machine = mac_manager_->getMachine<Machine>(step->getMachineType());

        if (machine) {
            PipetteTrType tr_type = PipetteTrType::UL_50;
            int           num     = 1;
            step->getParams()[PuriPipette::PipetteTrIndex].get_to(tr_type);
            step->getParams()[PuriPipette::PipetteNum].get_to(num);

            if (!machine->consumePipetteTr(tr_type, num)) {
                logger->debug("Step {} can not execute, machine {} has no available pipette tr {}",
                              step->getId(), machine->getName(), magic_enum::enum_name(tr_type));
                return false;
            } else {
                logger->debug("Step {} consume pipette tr {} on machine {}", step->getId(),
                              magic_enum::enum_name(tr_type), machine->getName());
            }
        }
    }

    auto used_tube_ids = action->getStep()->getUsedTubeIds();
    for (const auto& tube_id : used_tube_ids) {
        if (reality.isAlloced(tube_id)) {
            continue;
        }
        // bind tube_id to the step
        // get machine
        auto tube    = reality.getTube(tube_id);
        auto machine = mac_manager_->getMachine<Machine>(action->getStep()->getMachineType());
        if (machine) {
            auto carrier_pair = machine->allocAConsumeTube(tube);
            if (!carrier_pair.first) {
                logger->debug("Failed to allocate tube {} type {} to machine {}", tube_id,
                              magic_enum::enum_name(TubeManager::getTubeType(tube)),
                              action->getStep()->getMachineType());
                return false;
            }
            TubeManager::setTubeCarrier(tube, carrier_pair.first);
            reality.setTubePosition(tube_id, carrier_pair.second, TubePositionType::WHOLE_TUBE);
        }
    }
    return true;
}

bool CheckManager::checkPortageInner(Reality& reality, std::shared_ptr<MachineManager> mac_manager_,
                                     std::shared_ptr<Action>   action,
                                     std::shared_ptr<Workflow> original_workflow,
                                     CheckType check_type, SubmitFunc func, bool is_again) {
    if (dynamic_cast<PortageStep*>(action->getStep().get()) ||
        dynamic_cast<PuriMoveCarrierStep*>(action->getStep().get()) ||
        dynamic_cast<FluoMoveCarrierStep*>(action->getStep().get()) ||
        dynamic_cast<LibMoveCarrierStep*>(action->getStep().get()) ||
        dynamic_cast<AmpMoveCarrierStep*>(action->getStep().get())) {
        return true;
    }
    auto used_tube_ids = action->getStep()->getUsedTubeIds();
    for (const auto& tube_id : used_tube_ids) {
        if (!is_again && !reality.isAlloced(tube_id)) {
            continue;
        }
        // bind tube_id to the step
        // get machine
        auto tube     = reality.getTube(tube_id);
        auto tube_pos = reality.getTubePosition(tube_id, TubePositionType::WHOLE_TUBE);
        auto machine  = mac_manager_->getMachine<Machine>(action->getStep()->getMachineType());

        if (tube_pos.machine_type == (MachineType)MachineType::TOTAL_NUM) {
            continue;
        }

        if (machine) {
            if (tube_pos.machine_type != (MachineType)machine->getType()) {
                // add portage
                std::shared_ptr<Workflow> portage_workflow = std::make_shared<Workflow>(1);
                if (!TubeManager::isOnCarrier(tube)) {
                    portage_workflow->addStep(
                        move_tube_to_carrier_funcs_[tube_pos.machine_type](tube_id));
                }

                portage_workflow->addStep(carrier_funcs_[tube_pos.machine_type](
                    tube_id, (AreaId)CommonAreaId::EXIT_AREA, (AreaId)CommonAreaId::AUTO));

                portage_workflow->addStep(
                    std::make_shared<PortageStep>(Portage::Name, Variables{{Step::Tube, tube_id}}));

                portage_workflow->addStep(carrier_funcs_[(MachineType)machine->getType()](
                    tube_id, (AreaId)CommonAreaId::AUTO, (AreaId)CommonAreaId::ENTER_AREA));

                func(portage_workflow);

                logger->debug(
                    "Step {} can not execute, tube {} is in machine {}, need portage to {}",
                    action->getStep()->getId(), tube_id,
                    magic_enum::enum_name(tube_pos.machine_type),
                    magic_enum::enum_name((MachineType)machine->getType()));

                return false;
            }

            if (((MachineType)machine->getType() == MachineType::PURIFICATION &&
                 tube_pos.area_id == (AreaId)PurificationArea::ENTER_AREA) ||
                ((MachineType)machine->getType() == MachineType::FLUORESCENCE &&
                 tube_pos.area_id == (AreaId)FluorescenceArea::ENTER_AREA) ||
                ((MachineType)machine->getType() == MachineType::AMPLIFICATION &&
                 tube_pos.area_id == (AreaId)AmplificationArea::ENTER_AREA) ||
                ((MachineType)machine->getType() == MachineType::LIBRARY &&
                 tube_pos.area_id == (AreaId)LibraryArea::ENTER_AREA)) {
                logger->debug("Step {} can not execute, tube {} is in enter area",
                              action->getStep()->getId(), tube_id);
                return false;
            }
        }
    }
    return true;
}

bool CheckManager::checkTubeType(Reality& reality, std::shared_ptr<MachineManager> mac_manager_,
                                 std::shared_ptr<Action>   action,
                                 std::shared_ptr<Workflow> original_workflow, CheckType check_type,
                                 SubmitFunc func) {
    // must on library
    if (dynamic_cast<LibMoveTubeStep*>(action->getStep().get()) ||
        dynamic_cast<LibMoveCarrierStep*>(action->getStep().get())) {
        // TODO: implement check for library tube type
        if (!func(nullptr)) {
            return false;
        }
        // until can submit a workflow

        auto used_tube_id = action->getStep()->getUsedTubeIds()[0];

        auto tube = reality.getTube(used_tube_id);

        auto tube_type = TubeManager::getTubeType(tube);

        // if successfully changed, the tube must be new tube, which has same type to target
        // otherwise, submit a change task

        TubeType target_type = TubeType::UNKNOWN;

        if (auto step = dynamic_cast<LibMoveTubeStep*>(action->getStep().get())) {
            target_type = step->getTargetType();
        } else if (auto step = dynamic_cast<LibMoveCarrierStep*>(action->getStep().get())) {
            target_type = step->getTargetType();
        }

        if (target_type == TubeType::UNKNOWN || tube_type == target_type) {
            return true;
        }

        // need submit
        if (target_type == TubeType::PCR_TUBE) {
            auto new_tube = reality.createTube("", TubeType::PCR_TUBE);
            auto new_id   = TubeManager::getTubeId(new_tube);

            std::shared_ptr<Workflow> change_workflow = std::make_shared<Workflow>(1);

            //             auto start_pos        =
            //             user_input_[LibPipette::StartPos].get<uint16_t>();
            // auto start_index      = user_input_[LibPipette::StartIndex].get<uint16_t>();
            // auto volume           = user_input_[LibPipette::Volume].get<uint16_t>();
            // auto end_pos          = user_input_[LibPipette::EndPos].get<uint16_t>();
            // auto end_index        = user_input_[LibPipette::EndIndex].get<uint16_t>();
            // auto num              = user_input_[LibPipette::PipetteNum].get<uint16_t>();
            // auto pipette_tr_index = user_input_[LibPipette::PipetteTrIndex].get<uint16_t>();
            for (int i = 0; i < 8; i++) {
                change_workflow->addStep(std::make_shared<LibPipetteStep>(
                    LibPipette::Name,
                    Variables{{LibPipette::SrcTube, used_tube_id},
                              {LibPipette::DstTube, new_id},
                              {LibPipette::StartIndex, i},
                              {LibPipette::EndIndex, 0},
                              {LibPipette::Volume, CommonVolumeId::AUTO},
                              {LibPipette::StartPos, LibraryArea::AUTO},
                              {LibPipette::EndPos, LibraryArea::AUTO},
                              {LibPipette::PipetteNum, 1},
                              {LibPipette::PipetteTrIndex, PipetteTrType::UL_200}}));
            }

            // submit a new reference
            reality.addTransfer(used_tube_id, new_id);

            func(change_workflow);
        }

        if (target_type == TubeType::STRIP_TUBE) {
            auto new_tube = reality.createTube("", TubeType::STRIP_TUBE);
            auto new_id   = TubeManager::getTubeId(new_tube);

            std::shared_ptr<Workflow> change_workflow = std::make_shared<Workflow>(1);

            for (int i = 0; i < 8; i++) {
                change_workflow->addStep(std::make_shared<LibPipetteStep>(
                    LibPipette::Name,
                    Variables{{LibPipette::SrcTube, used_tube_id},
                              {LibPipette::DstTube, new_id},
                              {LibPipette::StartIndex, 0},
                              {LibPipette::EndIndex, i},
                              {LibPipette::Volume, (int)(CommonVolumeId::AUTO) + (8 - i)},
                              {LibPipette::StartPos, LibraryArea::AUTO},
                              {LibPipette::EndPos, LibraryArea::AUTO},
                              {LibPipette::PipetteNum, 1},
                              {LibPipette::PipetteTrIndex, PipetteTrType::UL_200}}));
            }

            // submit a new reference
            reality.addTransfer(used_tube_id, new_id);

            func(change_workflow);
        }

        return false;
    }
    return true;
}

bool CheckManager::checkReagents(Reality& reality, std::shared_ptr<MachineManager> mac_manager_,
                                 std::shared_ptr<Action>   action,
                                 std::shared_ptr<Workflow> original_workflow, CheckType check_type,
                                 SubmitFunc func) {

    if (dynamic_cast<LibPipetteStep*>(action->getStep().get()) ||
        dynamic_cast<FluoPipetteStep*>(action->getStep().get()) ||
        dynamic_cast<PuriPipetteStep*>(action->getStep().get())) {
        // check reagents
        auto params = action->getStep()->getParams();
        if (params.find(LibPipette::SrcTube) == params.end()) {
            return true;
        }

        auto tube_id = action->getStep()->getParams()[LibPipette::SrcTube].get<TubeId>();
        auto index   = action->getStep()->getParams()[LibPipette::StartIndex].get<uint16_t>();
        auto volume  = action->getStep()->getParams()[LibPipette::Volume].get<uint16_t>();

        if (tube_id != (TubeId)-1) {
            auto tube = reality.getTube(tube_id);
            if (tube && !TubeManager::getTubeIsReagent(tube)) {
                return true;
            }

            if (!tube || tube->getVolume(index) < volume) {
                if (!tube) {
                    logger->warn("Step {} can not execute, tube {} is not available",
                                 action->getStep()->getId(), tube_id);
                }

                auto pos = reality.getTubePosition(tube_id, TubePositionType::WHOLE_TUBE);

                logger->warn(
                    "Step {} can not execute, reason: \n"
                    "Reagent {}\n(Tube {} index {} / Carrier {} / Machine {} Location {})\n"
                    "Need to add volume, "
                    "Require volume: {} uL / Now volume: {} uL",
                    action->getStep()->getId(),
                    TubeManager::getTubeReagents(tube)[index]->getName(), tube_id, index,
                    TubeManager::getTubeCarrier(tube)->getName(),
                    magic_enum::enum_name(pos.machine_type),
                    MachineManager::fromAreaId(pos.machine_type, pos.area_id), volume / 100,
                    tube ? tube->getVolume(index) / 100 : 0);

                // logger->debug("Step {} can not execute, tube {} {} need to add volume, now
                // volume: "
                //               "{}, required volume: {}",
                //               action->getStep()->getId(), tube_id, index,
                //               tube ? tube->getVolume(index) : 0, volume);
                return false;
            }
        }
    }

    return true;
}

bool CheckManager::checkEquipment(Reality& reality, std::shared_ptr<MachineManager> mac_manager_,
                                  std::shared_ptr<Action>   action,
                                  std::shared_ptr<Workflow> original_workflow, CheckType check_type,
                                  SubmitFunc func) {
    auto machine = mac_manager_->getMachine<Machine>(action->getStep()->getMachineType());
    if (!machine) {
        // dummy can always execute
        return true;
    }

    // pre alloc always success
    if (original_workflow->isPreAlloc()) {
        return true;
    }

    switch (check_type) {
    case CheckType::CHECKONLY:
        [[fallthrough]];
    case CheckType::APPLY: {
        for (const auto& req :
             action->getStep()->getNeedLockEquipment(reality, mac_manager_, action)) {
            auto target_machine      = mac_manager_->getMachine<Machine>(req.first);
            auto available_equipment = target_machine->getAvailEquipments();
            if (std::find(available_equipment.begin(), available_equipment.end(), req.second) ==
                available_equipment.end()) {
                logger->debug(
                    "Step {} can not execute, equipment {} in machine {} is not available",
                    action->getStep()->getId(), magic_enum::enum_name(req.second),
                    target_machine->getName());
                return false;
            }
            std::shared_ptr<Container> container;
            auto                       tube_id = action->getStep()->getTubeId();
            if (tube_id != (TubeId)-1) {
                container = reality.getTube(tube_id);
            }
            target_machine->allocEquipment(req.second, check_type, original_workflow,
                                           action->getStep(), container, false);
        }
        break;
    }
    case CheckType::RELEASE: {
        for (const auto& req :
             action->getStep()->getNeedUnlockEquipment(reality, mac_manager_, action)) {
            auto target_machine = mac_manager_->getMachine<Machine>(req.first);
            std::shared_ptr<Container> container;
            auto                       tube_id = action->getStep()->getTubeId();
            if (tube_id != (TubeId)-1) {
                container = reality.getTube(tube_id);
            }
            target_machine->releaseEquipment(req.second, original_workflow, action->getStep(),
                                             container, false);
        }
        break;
    }
    }

    return true;
}

bool CheckManager::checkReplaceEquipment(Reality&                        reality,
                                         std::shared_ptr<MachineManager> mac_manager_,
                                         std::shared_ptr<Action>         action,
                                         std::shared_ptr<Workflow>       original_workflow,
                                         CheckType check_type, SubmitFunc func) {
    auto machine = mac_manager_->getMachine<Machine>(action->getStep()->getMachineType());
    if (!machine) {
        // dummy can always execute
        return true;
    }

    // pre alloc always success
    if (original_workflow->isPreAlloc()) {
        return true;
    }

    switch (check_type) {
    case CheckType::CHECKONLY:
        [[fallthrough]];
    case CheckType::APPLY: {
        for (const auto& req :
             action->getStep()->getNeedLockEquipment(reality, mac_manager_, action)) {
            auto target_machine = mac_manager_->getMachine<Machine>(req.first);
            auto equipment      = target_machine->getEquipment(req.second);
            if (equipment && equipment->isError() &&
                !action->getStep()->canExecuteWithoutEquipment(req.second)) {
                logger->warn("Step {} can not execute, equipment {} in machine {} is in error, try "
                             "to find another machine",
                             action->getStep()->getId(), magic_enum::enum_name(req.second),
                             target_machine->getName());

                std::shared_ptr<Step> new_step = nullptr;
                std::string           op_name  = action->getStep()->getOperationName();
                // Traverse all machines to find a replacement
                for (const auto& [id, machine_ptr] : mac_manager_->getAllMachines()) {
                    auto alt_equipment = machine_ptr->getEquipment(req.second);
                    if (alt_equipment && !alt_equipment->isError()) {
                        // Use factory to create replacement step
                        if (!op_name.empty() && step_factory_.count((MachineType)id) &&
                            step_factory_[(MachineType)id].count(op_name)) {
                            new_step = step_factory_[(MachineType)id][op_name](
                                action->getStep()->getName() + "_changed",
                                Variables(action->getStep()->getParams()));
                        }

                        if (new_step) {
                            logger->info("Found replacement machine {} for step {} (operation: {})",
                                         machine_ptr->getName(), action->getStep()->getId(),
                                         op_name);
                            break;
                        }
                    }
                }

                if (new_step) {
                    new_step->setId(action->getStep()->getId());
                    new_step->setWorkflowId(action->getStep()->getWorkflowId());
                    new_step->copyNextStepsFrom(action->getStep());

                    // When cross-machine fallback happens for a Pipette step, the source
                    // step's StartPos/EndPos carry enum values from the original machine's
                    // area enum (e.g. FluorescenceArea::AUTO=12).  The replacement machine
                    // uses a different enum (e.g. PurificationArea::AUTO=14), so those raw
                    // integers are misinterpreted as wrong physical locations.
                    // Fix: reset both positions to AUTO for the target machine so that
                    // phase0 resolves the actual tube location from `reality`.
                    // Each machine's Area enum assigns a different integer to AUTO.
                    // Remap any position param that equals the source AUTO to the target AUTO.
                    static const std::map<MachineType, uint16_t> kAutoArea = {
                        {MachineType::PURIFICATION, (uint16_t)PurificationArea::AUTO},
                        {MachineType::FLUORESCENCE, (uint16_t)FluorescenceArea::AUTO},
                        {MachineType::LIBRARY, (uint16_t)LibraryArea::AUTO},
                        {MachineType::AMPLIFICATION, (uint16_t)AmplificationArea::AUTO},
                    };
                    auto src_type = (MachineType)action->getStep()->getMachineType();
                    auto dst_type = (MachineType)new_step->getMachineType();
                    auto it_src   = kAutoArea.find(src_type);
                    auto it_dst   = kAutoArea.find(dst_type);
                    if (it_src != kAutoArea.end() && it_dst != kAutoArea.end() &&
                        it_src->second != it_dst->second) {
                        auto params = new_step->getParams();
                        for (const auto& key : {"StartPos", "EndPos", "Pos"}) {
                            if (params.contains(key) &&
                                params[key].get<uint16_t>() == it_src->second) {
                                params[key] = it_dst->second;
                            }
                        }
                        new_step->setParams(std::move(params));
                    }

                    action->setStep(new_step);
                    action->setStepTransfered();
                } else {
                    logger->error("Failed to transfer step {}, no healthy replacement machine "
                                  "found for equipment {}",
                                  action->getStep()->getId(), magic_enum::enum_name(req.second));
                    return false;
                }
            }
        }
        break;
    }
    case CheckType::RELEASE:
        break;
    }

    return true;
}

bool CheckManager::preAllocEquipment(Reality& reality, std::shared_ptr<MachineManager> mac_manager_,
                                     std::shared_ptr<Action>   action,
                                     std::shared_ptr<Workflow> original_workflow,
                                     CheckType check_type, SubmitFunc func) {
    if (!original_workflow->isPreAlloc()) {
        return false;
    }

    switch (check_type) {
    case CheckType::CHECKONLY:
        [[fallthrough]];
    case CheckType::APPLY: {
        bool alloc_failed = false;
        for (const auto& req :
             action->getStep()->getNeedLockEquipment(reality, mac_manager_, action)) {
            auto target_machine = mac_manager_->getMachine<Machine>(req.first);
            auto alloc_all      = target_machine->allocEquipment(
                req.second, check_type, original_workflow, action->getStep(), nullptr, true);
            if (alloc_all == (IndexId)CommonIndexId::NOT_SUCCESS) {
                alloc_failed = true;
            }
        }
        if (alloc_failed) {
            return false;
        }
        break;
    }
    case CheckType::RELEASE: {
        logger->debug("Pre-releasing equipment for step {}", action->getStep()->getId());
        for (const auto& req :
             action->getStep()->getNeedLockEquipment(reality, mac_manager_, action)) {
            auto target_machine = mac_manager_->getMachine<Machine>(req.first);
            target_machine->releaseEquipment(req.second, original_workflow, action->getStep(),
                                             nullptr, true);
        }
        break;
    }
    default:
        break;
    }
    return true;
}

bool CheckManager::moveToWaste(Reality& reality, std::shared_ptr<MachineManager> mac_manager_,
                               std::shared_ptr<Action>   action,
                               std::shared_ptr<Workflow> original_workflow, CheckType check_type,
                               SubmitFunc func) {

    if (!original_workflow->isFinished()) {
        return false;
    }

    // must finished

    auto             steps = original_workflow->getSteps();
    std::set<TubeId> used_tube_ids;
    for (const auto& step : steps) {
        auto step_used_tube_ids = step->getUsedTubeIds();
        used_tube_ids.insert(step_used_tube_ids.begin(), step_used_tube_ids.end());
    }

    // add move to waste step
    std::shared_ptr<Workflow> waste_workflow = std::make_shared<Workflow>(1);
    for (const auto& tube_id : used_tube_ids) {
        auto tube = reality.getTube(tube_id);
        if (!reality.isUselessTube(tube)) {
            continue;
        }

        waste_workflow->addStep(std::make_shared<PuriMoveTubeStep>(
            PuriMoveTube::Name, Variables{{PuriMoveTube::Tube, tube_id},
                                          {PuriMoveTube::StartPos, PurificationArea::AUTO},
                                          {PuriMoveTube::EndPos, PurificationArea::WASTE_AREA}}));
    }

    if (waste_workflow->getSteps().size() > 0) {
        func(waste_workflow);
        logger->debug("Workflow {} move used tubes to waste", original_workflow->getId());
    }
    return true;
}

void CheckManager::registerAllSteps() {
    // Pipette
    CheckManager::registerStep(MachineType::PURIFICATION, "Pipette",
                               [](const std::string& name, Variables&& vars) {
                                   return std::make_shared<PuriPipetteStep>(name, std::move(vars));
                               });
    CheckManager::registerStep(MachineType::LIBRARY, "Pipette",
                               [](const std::string& name, Variables&& vars) {
                                   return std::make_shared<LibPipetteStep>(name, std::move(vars));
                               });
    CheckManager::registerStep(MachineType::FLUORESCENCE, "Pipette",
                               [](const std::string& name, Variables&& vars) {
                                   return std::make_shared<FluoPipetteStep>(name, std::move(vars));
                               });

    // AspirateMix
    CheckManager::registerStep(
        MachineType::PURIFICATION, "AspirateMix", [](const std::string& name, Variables&& vars) {
            return std::make_shared<PuriAspirateMixStep>(name, std::move(vars));
        });
    CheckManager::registerStep(
        MachineType::LIBRARY, "AspirateMix", [](const std::string& name, Variables&& vars) {
            return std::make_shared<LibAspirateMixStep>(name, std::move(vars));
        });
    CheckManager::registerStep(
        MachineType::FLUORESCENCE, "AspirateMix", [](const std::string& name, Variables&& vars) {
            return std::make_shared<FluoAspirateMixStep>(name, std::move(vars));
        });
    CheckManager::registerStep(
        MachineType::AMPLIFICATION, "AspirateMix", [](const std::string& name, Variables&& vars) {
            return std::make_shared<AmpAspirateMixStep>(name, std::move(vars));
        });

    // Time
    CheckManager::registerStep(MachineType::PURIFICATION, "Time",
                               [](const std::string& name, Variables&& vars) {
                                   return std::make_shared<PuriTimeStep>(name, std::move(vars));
                               });
    CheckManager::registerStep(MachineType::LIBRARY, "Time",
                               [](const std::string& name, Variables&& vars) {
                                   return std::make_shared<LibTimeStep>(name, std::move(vars));
                               });
    CheckManager::registerStep(MachineType::FLUORESCENCE, "Time",
                               [](const std::string& name, Variables&& vars) {
                                   return std::make_shared<FluoTimeStep>(name, std::move(vars));
                               });

    // PCR
    CheckManager::registerStep(MachineType::AMPLIFICATION, "PCR",
                               [](const std::string& name, Variables&& vars) {
                                   return std::make_shared<AmpPcrStep>(name, std::move(vars));
                               });
    CheckManager::registerStep(MachineType::PURIFICATION, "PCR",
                               [](const std::string& name, Variables&& vars) {
                                   return std::make_shared<PuriPcrStep>(name, std::move(vars));
                               });

    // MoveTube
    CheckManager::registerStep(MachineType::PURIFICATION, "MoveTube",
                               [](const std::string& name, Variables&& vars) {
                                   return std::make_shared<PuriMoveTubeStep>(name, std::move(vars));
                               });
    CheckManager::registerStep(MachineType::LIBRARY, "MoveTube",
                               [](const std::string& name, Variables&& vars) {
                                   return std::make_shared<LibMoveTubeStep>(name, std::move(vars));
                               });
    CheckManager::registerStep(MachineType::FLUORESCENCE, "MoveTube",
                               [](const std::string& name, Variables&& vars) {
                                   return std::make_shared<FluoMoveTubeStep>(name, std::move(vars));
                               });
    CheckManager::registerStep(MachineType::AMPLIFICATION, "MoveTube",
                               [](const std::string& name, Variables&& vars) {
                                   return std::make_shared<AmpMoveTubeStep>(name, std::move(vars));
                               });

    // MoveCarrier
    CheckManager::registerStep(
        MachineType::PURIFICATION, "MoveCarrier", [](const std::string& name, Variables&& vars) {
            return std::make_shared<PuriMoveCarrierStep>(name, std::move(vars));
        });
    CheckManager::registerStep(
        MachineType::LIBRARY, "MoveCarrier", [](const std::string& name, Variables&& vars) {
            return std::make_shared<LibMoveCarrierStep>(name, std::move(vars));
        });
    CheckManager::registerStep(
        MachineType::FLUORESCENCE, "MoveCarrier", [](const std::string& name, Variables&& vars) {
            return std::make_shared<FluoMoveCarrierStep>(name, std::move(vars));
        });
    CheckManager::registerStep(
        MachineType::AMPLIFICATION, "MoveCarrier", [](const std::string& name, Variables&& vars) {
            return std::make_shared<AmpMoveCarrierStep>(name, std::move(vars));
        });

    // Centrifuge
    CheckManager::registerStep(
        MachineType::PURIFICATION, "Centrifuge", [](const std::string& name, Variables&& vars) {
            return std::make_shared<PuriCentrifugeStep>(name, std::move(vars));
        });
    CheckManager::registerStep(
        MachineType::LIBRARY, "Centrifuge", [](const std::string& name, Variables&& vars) {
            return std::make_shared<LibCentrifugeStep>(name, std::move(vars));
        });

    // Heat
    CheckManager::registerStep(MachineType::LIBRARY, "Heat",
                               [](const std::string& name, Variables&& vars) {
                                   return std::make_shared<LibHeatStep>(name, std::move(vars));
                               });

    // Shake
    CheckManager::registerStep(MachineType::PURIFICATION, "Shake",
                               [](const std::string& name, Variables&& vars) {
                                   return std::make_shared<PuriShakeStep>(name, std::move(vars));
                               });
}