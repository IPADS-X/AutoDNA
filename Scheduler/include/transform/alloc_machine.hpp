#include "fstream"
#include "procedure/stage.hpp"
#include "procedure/workflow.hpp"
#include "process/dummy.hpp"
#include "reality/reality.hpp"

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

class AllocMachine {
    using InstanceFunc = std::function<std::shared_ptr<Step>(DummyStep& dummy_step)>;

    static const inline std::map<std::pair<Dummy::DummyType, MachineType>, InstanceFunc>
        instance_func_map_ = {
            {std::make_pair(Dummy::DummyType::PIPETTE, MachineType::LIBRARY),
             LibPipetteStep::fromDummy},
            {std::make_pair(Dummy::DummyType::PIPETTE, MachineType::PURIFICATION),
             PuriPipetteStep::fromDummy},
            {std::make_pair(Dummy::DummyType::PIPETTE, MachineType::FLUORESCENCE),
             FluoPipetteStep::fromDummy},
            {std::make_pair(Dummy::DummyType::PIPETTE_PCR_TUBE, MachineType::LIBRARY),
             LibPipetteStep::fromDummy},
            {std::make_pair(Dummy::DummyType::MOVE_TUBE, MachineType::AMPLIFICATION),
             AmpMoveTubeStep::fromDummy},
            {std::make_pair(Dummy::DummyType::MOVE_TUBE, MachineType::LIBRARY),
             LibMoveTubeStep::fromDummy},
            {std::make_pair(Dummy::DummyType::MOVE_PCR_TUBE, MachineType::LIBRARY),
             LibMoveTubeStep::fromDummy},
            {std::make_pair(Dummy::DummyType::MOVE_PCR_TUBE, MachineType::PURIFICATION),
             PuriMoveTubeStep::fromDummy},
            {std::make_pair(Dummy::DummyType::MOVE_TUBE, MachineType::PURIFICATION),
             PuriMoveTubeStep::fromDummy},
            {std::make_pair(Dummy::DummyType::MOVE_TUBE, MachineType::FLUORESCENCE),
             FluoMoveTubeStep::fromDummy},
            {std::make_pair(Dummy::DummyType::ASPIRATE_MIX, MachineType::AMPLIFICATION),
             AmpAspirateMixStep::fromDummy},
            {std::make_pair(Dummy::DummyType::ASPIRATE_MIX, MachineType::LIBRARY),
             LibAspirateMixStep::fromDummy},
            {std::make_pair(Dummy::DummyType::ASPIRATE_MIX, MachineType::PURIFICATION),
             PuriAspirateMixStep::fromDummy},
            {std::make_pair(Dummy::DummyType::ASPIRATE_MIX, MachineType::FLUORESCENCE),
             FluoAspirateMixStep::fromDummy},
            {std::make_pair(Dummy::DummyType::PCR, MachineType::PURIFICATION),
             PuriPcrStep::fromDummy},
            {std::make_pair(Dummy::DummyType::PCR, MachineType::AMPLIFICATION),
             AmpPcrStep::fromDummy},
            {std::make_pair(Dummy::DummyType::CAP, MachineType::FLUORESCENCE),
             FluoCapTubeStep::fromDummy},
            {std::make_pair(Dummy::DummyType::FLUO, MachineType::FLUORESCENCE),
             FluoFluoStep::fromDummy},
            {std::make_pair(Dummy::DummyType::HEATER, MachineType::LIBRARY),
             LibHeatStep::fromDummy},
            {std::make_pair(Dummy::DummyType::TIME, MachineType::LIBRARY), LibTimeStep::fromDummy},
            {std::make_pair(Dummy::DummyType::TIME, MachineType::PURIFICATION),
             PuriTimeStep::fromDummy},
            {std::make_pair(Dummy::DummyType::CENTRIFUGE, MachineType::LIBRARY),
             LibCentrifugeStep::fromDummy},
            {std::make_pair(Dummy::DummyType::CENTRIFUGE, MachineType::PURIFICATION),
             PuriCentrifugeStep::fromDummy},
            {std::make_pair(Dummy::DummyType::CENTRIFUGE_PCR_TUBE, MachineType::LIBRARY),
             LibCentrifugeStep::fromDummy},
            {std::make_pair(Dummy::DummyType::SHAKE, MachineType::PURIFICATION),
             PuriShakeStep::fromDummy},
    };

    static const inline std::map<Dummy::DummyType, std::vector<MachineType>> action_sig_map_ = {
        {Dummy::DummyType::SHAKE, {MachineType::PURIFICATION}},
        {Dummy::DummyType::HEATER, {MachineType::LIBRARY}},
        {Dummy::DummyType::PCR, {MachineType::AMPLIFICATION, MachineType::PURIFICATION}},
        {Dummy::DummyType::CENTRIFUGE, {MachineType::PURIFICATION, MachineType::LIBRARY}},
        {Dummy::DummyType::FLUO, {MachineType::FLUORESCENCE}},
        {Dummy::DummyType::CAP, {MachineType::FLUORESCENCE}},
        {Dummy::DummyType::PIPETTE_PCR_TUBE, {MachineType::LIBRARY}},
        {Dummy::DummyType::MOVE_PCR_TUBE, {MachineType::LIBRARY}},
        {Dummy::DummyType::CENTRIFUGE_PCR_TUBE, {MachineType::LIBRARY}},
    };

    static const inline std::map<Dummy::DummyEquipment, std::vector<MachineType>>
        equipment_sig_map_ = {
            {Dummy::DummyEquipment::HEATER_SHAKER, {MachineType::PURIFICATION}},
            {Dummy::DummyEquipment::METAL_BATH, {MachineType::LIBRARY}},
            {Dummy::DummyEquipment::PCR, {MachineType::AMPLIFICATION, MachineType::PURIFICATION}},
            {Dummy::DummyEquipment::CENTRIFUGE, {MachineType::PURIFICATION, MachineType::LIBRARY}},
            {Dummy::DummyEquipment::FLUORESCENCE, {MachineType::FLUORESCENCE}},
            {Dummy::DummyEquipment::CAPPER, {MachineType::FLUORESCENCE}}};

public:
    static std::vector<std::shared_ptr<Workflow>>
    transferAlloc(Reality& reality, std::shared_ptr<MachineManager> mac_manager_,
                  std::vector<std::shared_ptr<Workflow>> sources,
                  std::shared_ptr<spdlog::logger>        logger) {
        if (AllocMachine::logger_ == nullptr) {
            AllocMachine::logger_ = logger;
        }
        std::vector<std::shared_ptr<Workflow>> workflows = allocMachine(sources);

        workflows = allocTubes(reality, mac_manager_, workflows);

        workflows = instanceWorkflows(workflows);

        for (const auto& workflow : workflows) {
            auto steps = workflow->getSteps();
            for (auto& step : steps) {
                // print each step's name
                logger_->debug("Alloc machine: Step Name: {}, Step params: {}", step->getName(),
                               step->getParams());
            }
        }

        return workflows;
    }

    static std::vector<std::shared_ptr<Workflow>>
    allocMachine(std::vector<std::shared_ptr<Workflow>> workflows) {
        std::vector<std::shared_ptr<Workflow>> results;
        for (const auto& workflow : workflows) {
            auto                                   new_workflow = std::make_shared<Workflow>(1);
            std::queue<std::shared_ptr<DummyStep>> step_queue;
            MachineType                            last_machine_type = MachineType::PURIFICATION;
            for (const auto& step : workflow->getSteps()) {
                auto name = dynamic_cast<DummyStep*>(step.get())->getType();

                logger_->debug("Allocating step: {}, last_machine_type: {}",
                               magic_enum::enum_name(name),
                               magic_enum::enum_name(last_machine_type));

                if (name == Dummy::DummyType::MOVE_TUBE ||
                    name == Dummy::DummyType::MOVE_PCR_TUBE) {
                    auto dst_pos = dynamic_cast<DummyStep*>(step.get())
                                       ->getParams()[std::to_string(
                                           static_cast<int>(Dummy::ParamType::END_POSITION))];
                    if (equipment_sig_map_.find(dst_pos) != equipment_sig_map_.end()) {
                        auto        machines    = equipment_sig_map_.at(dst_pos);
                        MachineType machineType = machines[0];
                        for (const auto& machine : machines) {
                            if (machine == last_machine_type) {
                                machineType = machine;
                                break;
                            }
                        }

                        if (name == Dummy::DummyType::MOVE_PCR_TUBE) {
                            machineType = MachineType::LIBRARY;
                        }

                        dynamic_cast<DummyStep*>(step.get())->setMachine(machineType);
                        // logger_->debug("Directly allocated step: {}, to machine: {}",
                        //                magic_enum::enum_name(name),
                        //                magic_enum::enum_name(machineType));
                        new_workflow->addStep(step);
                        continue;
                    }
                }

                // if (name == Dummy::DummyType::PIPETTE) {
                //     auto dst_pos = dynamic_cast<DummyStep*>(step.get())
                //                        ->getParams()[std::to_string(
                //                            static_cast<int>(Dummy::ParamType::END_POSITION))];
                //     if (equipment_sig_map_.find(dst_pos) != equipment_sig_map_.end()) {
                //         auto        machines    = equipment_sig_map_.at(dst_pos);
                //         MachineType machineType = machines[0];
                //         for (const auto& machine : machines) {
                //             if (machine == last_machine_type) {
                //                 machineType = machine;
                //                 break;
                //             }
                //         }

                //         dynamic_cast<DummyStep*>(step.get())->setMachine(machineType);
                //         new_workflow->addStep(step);
                //         continue;
                //     }
                // }

                step_queue.push(std::dynamic_pointer_cast<DummyStep>(step));

                if (action_sig_map_.find(name) != action_sig_map_.end()) {
                    auto        machines    = action_sig_map_.at(name);
                    MachineType machineType = machines[0];
                    for (const auto& machine : machines) {
                        if (machine == last_machine_type) {
                            machineType = machine;
                            break;
                        }
                    }
                    last_machine_type = machineType;

                    while (step_queue.size() > 0) {
                        auto dummy_step = step_queue.front();
                        step_queue.pop();
                        dummy_step->setMachine(machineType);
                        // logger_->debug("Prepare Allocated step: {}, to machine: {}",
                        //                magic_enum::enum_name(dummy_step->getType()),
                        //                magic_enum::enum_name(machineType));
                        new_workflow->addStep(dummy_step);
                    }
                }
            }

            while (step_queue.size() > 0) {
                auto dummy_step = step_queue.front();
                step_queue.pop();
                dummy_step->setMachine(last_machine_type);
                // logger_->debug("Prepare Allocated step: {}, to machine: {}",
                //                magic_enum::enum_name(dummy_step->getType()),
                //                magic_enum::enum_name(machineType));
                new_workflow->addStep(dummy_step);
            }

            results.push_back(workflow);

            logger_->debug("Allocated {} workflows with machines with {} steps", results.size(),
                           new_workflow->getSteps().size());
        }
        return results;
    }

    static std::vector<std::shared_ptr<Workflow>>
    allocTubes(Reality& reality, std::shared_ptr<MachineManager> mac_manager_,
               std::vector<std::shared_ptr<Workflow>> workflows) {
        // get machine's tube allocation
        // check if has carrier
        // query carrier for a empty tube position
        // bind them

        return workflows;

        // check not static
        for (const auto& workflow : workflows) {
            for (const auto& step : workflow->getSteps()) {
                auto dummy_step = dynamic_cast<DummyStep*>(step.get());
                if (!dummy_step) {
                    continue;
                }
                auto used_tube_ids = dummy_step->getUsedTubeIds();
                for (const auto& tube_id : used_tube_ids) {
                    if (reality.isAlloced(tube_id)) {
                        continue;
                    }
                    // bind tube_id to the step
                    // get machine
                    auto tube    = reality.getTube(tube_id);
                    auto machine = mac_manager_->getMachine<Machine>(dummy_step->getMachineType());
                    if (machine) {
                        auto carrier_pair = machine->allocAConsumeTube(tube);
                        TubeManager::setTubeCarrier(tube, carrier_pair.first);
                        reality.setTubePosition(tube_id, carrier_pair.second,
                                                TubePositionType::WHOLE_TUBE);
                    }
                }
            }
        }

        return workflows;
    }

    static std::vector<std::shared_ptr<Workflow>>
    instanceWorkflows(std::vector<std::shared_ptr<Workflow>> workflows) {
        std::vector<std::shared_ptr<Workflow>> instances;
        for (const auto& workflow : workflows) {
            auto new_workflow = std::make_shared<Workflow>(1);

            for (const auto& step : workflow->getSteps()) {
                auto dummy_step = dynamic_cast<DummyStep*>(step.get());
                if (instance_func_map_.find(
                        std::make_pair(dummy_step->getType(), dummy_step->getMachine())) ==
                    instance_func_map_.end()) {
                    continue;
                }
                auto instance_func =
                    instance_func_map_.at(std::make_pair<Dummy::DummyType, MachineType>(
                        dummy_step->getType(), (MachineType)(dummy_step->getMachine())));
                if (!instance_func) {
                    continue;
                }
                auto instance_stage = instance_func(*dummy_step);
                if (!instance_stage) {
                    continue;
                }
                new_workflow->addStep(instance_stage);
            }
            instances.push_back(new_workflow);

            logger_->debug("Instantiated {} workflows with specific machine steps with {} steps",
                           instances.size(), new_workflow->getSteps().size());
        }

        return instances;
    }

private:
    static inline std::shared_ptr<spdlog::logger> logger_;
};