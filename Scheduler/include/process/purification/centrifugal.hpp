#pragma once

#include "procedure/stage.hpp"
#include "process/dummy.hpp"

class Centrifugal {
public:
    enum { kOutput = 0 };
    inline static const std::string Name     = "Centrifugal";
    inline static const std::string Speed    = "Speed";
    inline static const std::string Duration = "Duration";
};

class CentrifugalStep : public CRTPStep<CentrifugalStep> {
public:
    CentrifugalStep(std::string name, Variables user_input)
        : CRTPStep<CentrifugalStep>(MachineType::PURIFICATION, name, std::move(user_input)) {
        step_funcs_ = {std::bind(&CentrifugalStep::phase0, this, std::placeholders::_1,
                                 std::placeholders::_2, std::placeholders::_3),
                       std::bind(&CentrifugalStep::phase1, this, std::placeholders::_1,
                                 std::placeholders::_2, std::placeholders::_3),
                       std::bind(&CentrifugalStep::phase2, this, std::placeholders::_1,
                                 std::placeholders::_2, std::placeholders::_3)};
    }

    std::vector<ExecutionResult>
    phase0(Reality& reality, std::shared_ptr<MachineManager> mac_manager, ActionId action_id) {
        std::vector<ExecutionResult> results;

        auto machine = mac_manager->getMachine<PurificationModbusMachine>(machine_type_);
        machine->centrifugal1(action_id);

        logger->debug("[Action {}]: centrifugal first try", action_id);

        results.push_back(ExecutionResult());
        return results;
    }

    std::vector<ExecutionResult>
    phase1(Reality& reality, std::shared_ptr<MachineManager> mac_manager, ActionId action_id) {
        std::vector<ExecutionResult> results;

        auto machine = mac_manager->getMachine<PurificationModbusMachine>(machine_type_);

        auto carrier_sn = step_input_[Carrier::NAME].get<std::string>();
        auto carrier    = reality.getCarrier(carrier_sn);
        auto tubes      = carrier->getBitmap();

        // 参数8600对应转速2000rpm
        uint16_t speed    = user_input_[Centrifugal::Speed].get<uint16_t>();
        uint16_t duration = user_input_[Centrifugal::Duration].get<uint16_t>();
        machine->centrifugal2(tubes, speed, duration, action_id);

        results.push_back(ExecutionResult());
        return results;
    }

    std::vector<ExecutionResult>
    phase2(Reality& reality, std::shared_ptr<MachineManager> mac_manager, ActionId action_id) {
        std::vector<ExecutionResult> results;

        auto        manager = std::dynamic_pointer_cast<MachineManager>(mac_manager);
        auto        machine = manager->getMachine<PurificationModbusMachine>(machine_type_);
        std::string sn;
        std::vector<uint16_t> tubes;
        machine->onCentrifugalFinished(sn, tubes);

        Variables             output = step_input_;
        std::shared_ptr<Step> next_step;

        auto iter = next_steps_.find(Centrifugal::kOutput);
        if (iter != next_steps_.end()) {
            next_step = iter->second;
        }
        results.push_back(ExecutionResult{next_step, std::move(output), false});
        return results;
    }
};

class CentrifugalStage : public TemplatedStage<Centrifugal> {
public:
    CentrifugalStage(std::string id, Variables&& input) : TemplatedStage(std::move(id), true) {
        my_step_ = std::make_shared<CentrifugalStep>(name_, std::move(input));
    }

    void generateWorkflowHelper(Workflow& workflow) {
        if (next_stages_.size() != 1) {
            return;
        }
        genWorkflowForNextStage(workflow, Centrifugal::kOutput);
    }
};