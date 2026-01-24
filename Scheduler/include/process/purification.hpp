#pragma once

#include "procedure/stage.hpp"

class Purification {
public:
    enum { kOutput = 0 };
    inline static const std::string Name     = "Purification";
    inline static const std::string Speed    = "Speed";
    inline static const std::string Duration = "Duration";
};

class PurificationStep : public CRTPStep<PurificationStep> {
public:
    PurificationStep(std::string name, Variables user_input)
        : CRTPStep<PurificationStep>(MachineType::PURIFICATION, name, std::move(user_input)) {
        step_funcs_ = {std::bind(&PurificationStep::phase0, this, std::placeholders::_1,
                                 std::placeholders::_2, std::placeholders::_3),
                       std::bind(&PurificationStep::phase1, this, std::placeholders::_1,
                                 std::placeholders::_2, std::placeholders::_3)};
    }

    std::vector<ExecutionResult>
    phase0(Reality& reality, std::shared_ptr<MachineManager> mac_manager, ActionId action_id) {
        std::vector<ExecutionResult> results;

        float speed;
        float duration;
        if (step_input_.count(Fluorescence::Result) == 0) {
            speed    = user_input_[Purification::Speed];
            duration = user_input_[Purification::Duration];
        } else {
            speed    = step_input_[Fluorescence::Result];
            duration = 5;
        }
        std::cout << speed << " " << duration << std::endl;

        results.push_back(ExecutionResult());
        return results;
    }

    std::vector<ExecutionResult>
    phase1(Reality& reality, std::shared_ptr<MachineManager> mac_manager, ActionId action_id) {
        std::vector<ExecutionResult> results;

        Variables             output;
        std::shared_ptr<Step> next_step;
        // output[CarrierInfo::Sample] = "P_Sample";
        // output[Portage::From]   = machine_type_;

        auto iter = next_steps_.find(Purification::kOutput);
        if (iter != next_steps_.end()) {
            next_step = iter->second;
        }
        results.push_back(ExecutionResult{next_step, std::move(output), false});
        return results;
    }
};

class PurificationStage : public TemplatedStage<Purification> {
public:
    PurificationStage(std::string id, Variables&& input) : TemplatedStage(std::move(id), true) {
        my_step_ = std::make_shared<PurificationStep>(name_, std::move(input));
    }

    void generateWorkflowHelper(Workflow& workflow) {
        if (next_stages_.size() != 1) {
            return;
        }
        genWorkflowForNextStage(workflow, Purification::kOutput);
    }
};