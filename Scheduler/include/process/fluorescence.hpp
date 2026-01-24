#pragma once

#include "procedure/stage.hpp"

class Fluorescence {
public:
    enum { kOutput = 0 };
    inline static const std::string Name   = "Fluorescence";
    inline static const std::string Time   = "FluoTime";
    inline static const std::string Result = "Result";
};

class FluorescenceStep : public CRTPStep<FluorescenceStep> {
public:
    FluorescenceStep(std::string name, Variables user_input)
        : CRTPStep<FluorescenceStep>(MachineType::FLUORESCENCE, name, std::move(user_input)) {
        step_funcs_ = {std::bind(&FluorescenceStep::phase0, this, std::placeholders::_1,
                                 std::placeholders::_2, std::placeholders::_3),
                       std::bind(&FluorescenceStep::phase1, this, std::placeholders::_1,
                                 std::placeholders::_2, std::placeholders::_3),
                       std::bind(&FluorescenceStep::phase2, this, std::placeholders::_1,
                                 std::placeholders::_2, std::placeholders::_3)};
    }

    std::vector<ExecutionResult>
    phase0(Reality& reality, std::shared_ptr<MachineManager> mac_manager, ActionId action_id) {
        std::vector<ExecutionResult> results;

        auto machine = mac_manager->getMachine<FluorescenceModbusMachine>(machine_type_);
        machine->fluorescence1(action_id);

        logger->debug("[Action {}]: fluorescence first try", action_id);

        results.push_back(ExecutionResult());
        return results;
    }

    std::vector<ExecutionResult>
    phase1(Reality& reality, std::shared_ptr<MachineManager> mac_manager, ActionId action_id) {
        std::vector<ExecutionResult> results;

        auto machine = mac_manager->getMachine<FluorescenceModbusMachine>(machine_type_);

        auto carrier_sn = step_input_[Carrier::NAME].get<std::string>();
        auto carrier    = reality.getCarrier(carrier_sn);
        auto tubes      = carrier->getBitmap();

        uint16_t time = user_input_[Fluorescence::Time].get<uint16_t>();

        machine->fluorescence2(tubes, time, action_id);

        results.push_back(ExecutionResult());
        return results;
    }

    std::vector<ExecutionResult>
    phase2(Reality& reality, std::shared_ptr<MachineManager> mac_manager, ActionId action_id) {
        std::vector<ExecutionResult> results;

        auto machine = mac_manager->getMachine<FluorescenceModbusMachine>(machine_type_);

        std::string        sn;
        std::vector<float> fluo_results;
        machine->onFluorescenceFinished(sn, fluo_results);

        // std::cout << "Fluorescence result: " << result << std::endl;

        Variables             output = step_input_;
        std::shared_ptr<Step> next_step;
        output[Fluorescence::Result] = fluo_results;

        auto iter = next_steps_.find(Fluorescence::kOutput);
        if (iter != next_steps_.end()) {
            next_step = iter->second;
        }

        results.push_back(ExecutionResult{next_step, std::move(output), false});
        return results;
    }
};

class FluorescenceStage : public TemplatedStage<Fluorescence> {
public:
    FluorescenceStage(std::string id, Variables&& input) : TemplatedStage(std::move(id), true) {
        my_step_ = std::make_shared<FluorescenceStep>(name_, std::move(input));
    }

    void generateWorkflowHelper(Workflow& workflow) {
        if (next_stages_.size() != 1) {
            return;
        }
        genWorkflowForNextStage(workflow, Fluorescence::kOutput);
    }
};