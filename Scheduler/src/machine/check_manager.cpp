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

// TODO: ADD MOVE LIQUID TO OTHER TUBES
bool CheckManager::checkConsumable(Reality& reality, std::shared_ptr<MachineManager> mac_manager_,
                                   std::shared_ptr<Action> action, CheckType check_type,
                                   SubmitFunc func) {

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
                                     std::shared_ptr<Action> action, CheckType check_type,
                                     SubmitFunc func, bool is_again) {
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
                                 std::shared_ptr<Action> action, CheckType check_type,
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
                                 std::shared_ptr<Action> action, CheckType check_type,
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

                logger->warn("Step {} can not execute, reason: \n"
                              "Reagent {}\n(Tube {} index {} / Carrier {} / Machine {} Location {})\n"
                              "Need to add volume, "
                              "Require volume: {} uL / Now volume: {} uL",
                              action->getStep()->getId(),
                              TubeManager::getTubeReagents(tube)[index]->getName(), tube_id, index,
                              TubeManager::getTubeCarrier(tube)->getName(),
                              magic_enum::enum_name(pos.machine_type),
                              MachineManager::fromAreaId(pos.machine_type, pos.area_id),
                              volume/100, tube ? tube->getVolume(index)/100 : 0);

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
                                  std::shared_ptr<Action> action, CheckType check_type,
                                  SubmitFunc func) {
    auto machine = mac_manager_->getMachine<Machine>(action->getStep()->getMachineType());
    if (!machine) {
        // dummy can always execute
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
            target_machine->allocEquipment(req.second, check_type, action->getStep(), container);
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
            target_machine->releaseEquipment(req.second, action->getStep(), container);
        }
        break;
    }
    }

    return true;
}