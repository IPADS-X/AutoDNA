#include "fstream"
#include "procedure/stage.hpp"
#include "procedure/workflow.hpp"
#include "process/dummy.hpp"
#include "reality/reality.hpp"

#include "transform/common.hpp"

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

class IntervalProcedure {
public:
    using CarrierFunc = std::function<std::shared_ptr<Step>(TubeId, AreaId, AreaId)>;

    static inline std::map<MachineType, CarrierFunc> carrier_funcs_ = {
        {MachineType::PURIFICATION, &PuriMoveCarrierStep::fromDummy},
        {MachineType::FLUORESCENCE, &FluoMoveCarrierStep::fromDummy},
        {MachineType::LIBRARY, &LibMoveCarrierStep::fromDummy},
        {MachineType::AMPLIFICATION, &AmpMoveCarrierStep::fromDummy}};

    static std::vector<std::shared_ptr<Workflow>>
    transferInterval(Reality& reality, std::shared_ptr<MachineManager> mac_manager_,
                     std::vector<std::shared_ptr<Workflow>> sources,
                     std::shared_ptr<spdlog::logger> logger, std::string workflow_name, int jump_from) {

        if (IntervalProcedure::logger_ == nullptr) {
            IntervalProcedure::logger_ = logger;
        }

        std::vector<std::shared_ptr<Workflow>> workflows = combinePipetteMix(
            reality, mac_manager_,
            addPipette(reality, mac_manager_,
                       //    addEquipmentChangeVolume(
                       //        reality, mac_manager_,
                       addUseEquipment(reality, mac_manager_,
                                       addPortage(reality, mac_manager_, sources))));

        workflows = addJump(reality, mac_manager_, workflows, workflow_name, jump_from);

        // std::vector<std::shared_ptr<Workflow>> workflows = combinePipetteMix(
        //     reality, mac_manager_,
        //     addUseEquipment(reality, mac_manager_, addPortage(reality, mac_manager_, sources)));

        for (const auto& workflow : workflows) {
            auto steps = workflow->getSteps();
            for (auto& step : steps) {
                // print each step's name
                logger_->debug("Step Id: {} Step Name: {}, Step params: {}", step->getId(),
                               step->getName(), step->getParams());
            }
        }

        return workflows;
    }

    static std::vector<std::shared_ptr<Workflow>>
    addJump(Reality& reality, std::shared_ptr<MachineManager> mac_manager_,
            std::vector<std::shared_ptr<Workflow>> sources, std::string workflow_name, int jump_from) {

        std::vector<std::shared_ptr<Workflow>> results;
        // #ifdef JUMP_ORIGIN_STEP
        for (const auto& workflow : sources) {
            std::shared_ptr<Workflow> new_workflow = std::make_shared<Workflow>(1);

            auto jump_until_from_data = jump_from;
            if (jump_until_from_data <= 1) {
                jump_until_from_data = 1;
            }

            std::cout << "Jumping to step " << jump_until_from_data << std::endl;

            new_workflow->setBaseStepId(jump_until_from_data);
            auto steps = workflow->getSteps();
            for (int i = 0; i < steps.size(); i++) {
                if (i + 1 >= jump_until_from_data) {
                    new_workflow->addStep(steps[i]);
                }
            }
            results.push_back(new_workflow);
        }
        // #endif

        return results;
    }

    // add move reagents to the

    static std::vector<std::shared_ptr<Workflow>>
    addPortage(Reality& reality, std::shared_ptr<MachineManager> mac_manager_,
               std::vector<std::shared_ptr<Workflow>> sources) {
        return sources;
        // // TODO: CHECK TUBE, NOT SEQUENTIALLY
        // std::vector<std::shared_ptr<Workflow>> results;
        // for (const auto& source : sources) {
        //     // Create a new workflow for each source
        //     auto new_workflow = std::make_shared<Workflow>(1);
        //     auto steps        = source->getSteps();
        //     // MachineType last_machine = MachineType::TOTAL_NUM;
        //     std::map<TubeId, MachineType> tube_last_machine;
        //     for (const auto& step : steps) {
        //         // 1. get used tube
        //         // just need to check firstly occured tubes?
        //         // 2. check tubes now machine is same of real machine
        //         // 3. if not match, add postage

        //         auto used_tubes = step->getUsedTubeIds();

        //         for (const auto& tube_id : used_tubes) {
        //             auto last_machine_it = tube_last_machine.find(tube_id);
        //             auto last_machine    = MachineType::TOTAL_NUM;
        //             if (last_machine_it == tube_last_machine.end()) {
        //                 auto tube = reality.getTube(tube_id);
        //                 if (tube) {
        //                     last_machine =
        //                         reality.getTubePosition(tube_id, TubePositionType::WHOLE_TUBE)
        //                             .machine_type;
        //                 }
        //             }

        //             auto now_machine = step->getMachineType();

        //             logger_->debug("Tube {} last machine: {}, now machine: {}", tube_id,
        //                            magic_enum::enum_name(last_machine),
        //                            magic_enum::enum_name((MachineType)now_machine));

        //             if (last_machine != (MachineType)now_machine) {
        //                 new_workflow->addStep(carrier_funcs_[last_machine](
        //                     tube_id, (AreaId)CommonAreaId::EXIT_AREA,
        //                     (AreaId)CommonAreaId::AUTO));

        //                 new_workflow->addStep(
        //                     std::make_shared<PortageStep>(Portage::Name, Variables{}));

        //                 new_workflow->addStep(carrier_funcs_[(MachineType)step->getMachineType()](
        //                     tube_id, (AreaId)CommonAreaId::AUTO,
        //                     (AreaId)CommonAreaId::ENTER_AREA));

        //                 tube_last_machine[tube_id] = (MachineType)now_machine;
        //             }
        //         }

        //         new_workflow->addStep(step);
        //     }
        //     results.push_back(new_workflow);
        // }
        // return results;

        std::vector<std::shared_ptr<Workflow>> results;
        for (const auto& source : sources) {
            // Create a new workflow for each source
            auto        new_workflow = std::make_shared<Workflow>(1);
            auto        steps        = source->getSteps();
            MachineType last_machine = MachineType::TOTAL_NUM;
            for (const auto& step : steps) {
                if ((MachineType)step->getMachineType() != last_machine &&
                    last_machine != MachineType::TOTAL_NUM) {
                    new_workflow->addStep(carrier_funcs_[last_machine](
                        step->getTubeId(), (AreaId)CommonAreaId::EXIT_AREA,
                        (AreaId)CommonAreaId::AUTO));

                    new_workflow->addStep(
                        std::make_shared<PortageStep>(Portage::Name, Variables{}));

                    new_workflow->addStep(carrier_funcs_[(MachineType)step->getMachineType()](
                        step->getTubeId(), (AreaId)CommonAreaId::AUTO,
                        (AreaId)CommonAreaId::ENTER_AREA));
                }

                new_workflow->addStep(step);
                last_machine = (MachineType)step->getMachineType();
            }
            results.push_back(new_workflow);
        }
        return results;
    }

    static std::vector<std::shared_ptr<Workflow>>
    addUseEquipment(Reality& reality, std::shared_ptr<MachineManager> mac_manager_,
                    std::vector<std::shared_ptr<Workflow>> sources) {
        std::vector<std::shared_ptr<Workflow>> results;
        for (const auto& source : sources) {
            // Create a new workflow for each source
            auto                new_workflow = std::make_shared<Workflow>(1);
            auto                steps        = source->getSteps();
            std::vector<TubeId> on_pcr_tubes = {};
            for (const auto& step : steps) {
                if (dynamic_cast<FluoCapTubeStep*>(step.get()) != nullptr) {
                    new_workflow->addStep(std::make_shared<FluoMoveTubeStep>(
                        FluoMoveTube::Name,
                        Variables{
                            {FluoMoveTube::StartPos, FluorescenceArea::CAPPING_CONSUMABLE_AREA},
                            {FluoMoveTube::EndPos, FluorescenceArea::CAPPER_LID_PLACEMENT_AREA},
                            {FluoMoveTube::StartIndex, 0},
                            {FluoMoveTube::EndIndex, 0}}));
                    new_workflow->addStep(step);
                } else if (auto real_step = dynamic_cast<PuriMoveTubeStep*>(step.get())) {
                    if ((PurificationArea)real_step->getParams()[PuriMoveTube::EndPos]
                            .get<AreaId>() == PurificationArea::THERMAL_CYCLING_AREA) {
                        on_pcr_tubes.push_back(
                            real_step->getParams()[PuriMoveTube::Tube].get<TubeId>());
                        new_workflow->addStep(std::make_shared<PuriPipetteStep>(
                            PuriPipette::Name,
                            Variables{
                                {PuriPipette::StartPos, PurificationArea::AUTO},
                                {PuriPipette::StartIndex, 0},
                                {PuriPipette::SrcTube,
                                 real_step->getParams()[PuriMoveTube::Tube].get<TubeId>()},
                                {PuriPipette::EndPos, PurificationArea::THERMAL_CYCLING_CAP_AREA},
                                {PuriPipette::EndIndex, 0},
                                {PuriPipette::Volume, 20000},
                                {PuriPipette::PipetteNum, 4},
                                {PuriPipette::PipetteTrIndex, PipetteTrType::UL_200}}));
                    } else if ((CommonAreaId)real_step->getParams()[PuriMoveTube::EndPos]
                                   .get<AreaId>() == CommonAreaId::REACTION) {
                        // if tube is on pcr, need to pipette to a tmp tube first
                        if (std::find(on_pcr_tubes.begin(), on_pcr_tubes.end(),
                                      real_step->getParams()[PuriMoveTube::Tube].get<TubeId>()) !=
                            on_pcr_tubes.end()) {
                            auto tube_id = real_step->getParams()[PuriMoveTube::Tube].get<TubeId>();
                            on_pcr_tubes.erase(
                                std::remove(on_pcr_tubes.begin(), on_pcr_tubes.end(), tube_id),
                                on_pcr_tubes.end());

                            new_workflow->addStep(std::make_shared<PuriPipetteStep>(
                                PuriPipette::Name,
                                Variables{{PuriPipette::StartPos,
                                           PurificationArea::THERMAL_CYCLING_CAP_AREA},
                                          {PuriPipette::StartIndex, 0},
                                          {PuriPipette::EndPos, PurificationArea::AUTO},
                                          {PuriPipette::EndIndex, 0},
                                          {PuriPipette::DstTube, tube_id},
                                          {PuriPipette::Volume, 20000},
                                          {PuriPipette::PipetteNum, 4},
                                          {PuriPipette::PipetteTrIndex, PipetteTrType::UL_200}}));
                        } else {
                            new_workflow->addStep(step);
                        }
                    } else {
                        new_workflow->addStep(step);
                    }
                } else if (auto real_step = dynamic_cast<LibMoveTubeStep*>(step.get())) {
                    if ((CommonAreaId)real_step->getParams()[LibMoveTube::EndPos].get<AreaId>() ==
                        CommonAreaId::REACTION) {
                        // if tube is on pcr, need to pipette to a tmp tube first
                        if (std::find(on_pcr_tubes.begin(), on_pcr_tubes.end(),
                                      real_step->getParams()[LibMoveTube::Tube].get<TubeId>()) !=
                            on_pcr_tubes.end()) {
                            auto tube_id = real_step->getParams()[LibMoveTube::Tube].get<TubeId>();
                            on_pcr_tubes.erase(
                                std::remove(on_pcr_tubes.begin(), on_pcr_tubes.end(), tube_id),
                                on_pcr_tubes.end());

                            new_workflow->addStep(std::make_shared<PuriPipetteStep>(
                                PuriPipette::Name,
                                Variables{{PuriPipette::StartPos,
                                           PurificationArea::THERMAL_CYCLING_CAP_AREA},
                                          {PuriPipette::StartIndex, 0},
                                          {PuriPipette::EndPos, PurificationArea::AUTO},
                                          {PuriPipette::EndIndex, 0},
                                          {PuriPipette::DstTube, tube_id},
                                          {PuriPipette::Volume, 20000},
                                          {PuriPipette::PipetteNum, 4},
                                          {PuriPipette::PipetteTrIndex, PipetteTrType::UL_200}}));
                        }
                        new_workflow->addStep(step);
                    } else {
                        new_workflow->addStep(step);
                    }
                } else if (dynamic_cast<PuriPcrStep*>(step.get()) != nullptr) {
                    // after pcr, tube is on pcr
                    new_workflow->addStep(std::make_shared<PuriMoveCarrierStep>(
                        PuriMoveCarrier::Name,
                        Variables{
                            {PuriMoveCarrier::StartPos, PurificationArea::THERMAL_CYCLING_CAP_AREA},
                            {PuriMoveCarrier::EndPos, PurificationArea::THERMAL_CYCLING_AREA}}));
                    new_workflow->addStep(step);

                    new_workflow->addStep(std::make_shared<PuriMoveCarrierStep>(
                        PuriMoveCarrier::Name,
                        Variables{
                            {PuriMoveCarrier::StartPos, PurificationArea::THERMAL_CYCLING_AREA},
                            {PuriMoveCarrier::EndPos,
                             PurificationArea::THERMAL_CYCLING_CAP_AREA}}));
                } else {
                    new_workflow->addStep(step);
                }
            }
            results.push_back(new_workflow);
        }
        return results;
    }

    static std::vector<std::shared_ptr<Workflow>>
    addEquipmentChangeVolume(Reality& reality, std::shared_ptr<MachineManager> mac_manager_,
                             std::vector<std::shared_ptr<Workflow>> sources) {
        // if use 1.5ml tube, transfer to a tmp 200ul tube
        std::vector<std::shared_ptr<Workflow>> results;
        for (const auto& source : sources) {
            // Create a new workflow for each source
            auto                     new_workflow           = std::make_shared<Workflow>(1);
            auto                     steps                  = source->getSteps();
            std::map<TubeId, TubeId> pcr_tube_to_strip_tube = {};
            for (const auto& step : steps) {
                auto new_steps = step->getTubeTypeSupportsSteps(reality, pcr_tube_to_strip_tube);
                for (const auto new_step : new_steps) {
                    new_workflow->addStep(new_step);
                }
            }
            results.push_back(new_workflow);
        }
        return results;
    }

    static std::vector<std::shared_ptr<Workflow>>
    addPipette(Reality& reality, std::shared_ptr<MachineManager> mac_manager_,
               std::vector<std::shared_ptr<Workflow>> sources) {
        // if volume more than 200ul, use multi pipetees
        std::vector<std::shared_ptr<Workflow>> results;
        for (const auto& source : sources) {
            // Create a new workflow for each source
            auto                     new_workflow           = std::make_shared<Workflow>(1);
            auto                     steps                  = source->getSteps();
            std::map<TubeId, TubeId> pcr_tube_to_strip_tube = {};
            for (const auto& step : steps) {
                if (dynamic_cast<LibPipetteStep*>(step.get()) != nullptr ||
                    dynamic_cast<FluoPipetteStep*>(step.get()) != nullptr ||
                    dynamic_cast<PuriPipetteStep*>(step.get()) != nullptr) {
                    auto volume = step->getParams()[LibPipette::Volume].get<uint16_t>();
                    while (volume > 20000) {
                        volume -= 20000;
                        auto new_params                = step->getParams();
                        new_params[LibPipette::Volume] = 20000;
                        new_workflow->addStep(step->buildNewStep(std::move(new_params)));
                    }
                    if (volume > 0) {
                        auto new_params                = step->getParams();
                        new_params[LibPipette::Volume] = volume;
                        new_workflow->addStep(step->buildNewStep(std::move(new_params)));
                    }
                } else {
                    new_workflow->addStep(step);
                }
            }
            results.push_back(new_workflow);
        }
        return results;
    }

    static std::vector<std::shared_ptr<Workflow>>
    combinePipetteMix(Reality& reality, std::shared_ptr<MachineManager> mac_manager_,
                      std::vector<std::shared_ptr<Workflow>> sources) {
        std::vector<std::shared_ptr<Workflow>> results;
        for (const auto& source : sources) {
            // Create a new workflow for each source
            auto                  new_workflow = std::make_shared<Workflow>(1);
            auto                  steps        = source->getSteps();
            std::shared_ptr<Step> last_step    = nullptr;
            bool                  jump         = false;
            for (const auto& step : steps) {
                if (jump) {
                    jump      = false;
                    last_step = step;
                    continue;
                }
                if (last_step && ((dynamic_cast<LibPipetteStep*>(last_step.get()) != nullptr &&
                                   dynamic_cast<LibAspirateMixStep*>(step.get()) != nullptr) ||
                                  (dynamic_cast<FluoPipetteStep*>(last_step.get()) != nullptr &&
                                   dynamic_cast<FluoAspirateMixStep*>(step.get()) != nullptr) ||
                                  (dynamic_cast<PuriPipetteStep*>(last_step.get()) != nullptr &&
                                   dynamic_cast<PuriAspirateMixStep*>(step.get()) != nullptr))) {
                    logger_->debug("combinePipetteMix: combine pipette and mix step: {} and {}",
                                   last_step->getName(), step->getName());
                    // combine pipette and mix step
                    auto last_step_params                       = last_step->getParams();
                    auto new_params                             = step->getParams();
                    last_step_params[LibPipette::AfterMixTimes] = new_params[LibAspirateMix::Total];
                    last_step_params[LibPipette::AfterMixVolume] =
                        new_params[LibAspirateMix::Volume];
                    last_step_params[LibPipette::MixSpeed] = new_params[LibAspirateMix::MixSpeed];

                    new_workflow->addStep(last_step->buildNewStep(std::move(last_step_params)));
                    jump = true;
                } else if (last_step) {
                    new_workflow->addStep(last_step);
                    jump = false;
                }
                last_step = step;
            }
            if (last_step) {
                new_workflow->addStep(last_step);
            }
            results.push_back(new_workflow);
        }
        return results;
    }

private:
    static inline std::shared_ptr<spdlog::logger> logger_;
};