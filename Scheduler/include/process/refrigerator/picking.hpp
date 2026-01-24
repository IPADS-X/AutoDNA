#pragma once

#include "procedure/stage.hpp"

class RefrigeratorPicking {
public:
    enum { kOutput = 0 };
    inline static const std::string Name     = "RefrigeratorPicking";
    inline static const std::string Position = "Position";
};

class RefrigeratorPickingStep : public CRTPStep<RefrigeratorPickingStep> {
public:
    RefrigeratorPickingStep(std::string name, Variables&& user_input)
        : CRTPStep<RefrigeratorPickingStep>(MachineType::REFRIGERATOR, name, std::move(user_input)) {
        step_funcs_ = {std::bind(&RefrigeratorPickingStep::phase0, this, std::placeholders::_1,
                                 std::placeholders::_2, std::placeholders::_3),
                       std::bind(&RefrigeratorPickingStep::phase1, this, std::placeholders::_1,
                                 std::placeholders::_2, std::placeholders::_3)};
    }

    std::vector<ExecutionResult>
    phase0(Reality& reality, std::shared_ptr<MachineManager> mac_manager, ActionId action_id) {
        std::vector<ExecutionResult> results;
        ASSERT(false);

        auto manager = std::dynamic_pointer_cast<MachineManager>(mac_manager);

        results.push_back(ExecutionResult());
        return results;
    }

    std::vector<ExecutionResult>
    phase1(Reality& reality, std::shared_ptr<MachineManager> mac_manager, ActionId action_id) {
        std::vector<ExecutionResult> results;

        Variables             output;
        std::shared_ptr<Step> next_step;
        // output[CarrierInfo::Sample] = "RP_Sample";
        // output[Portage::From]   = machine_type_;

        auto iter = next_steps_.find(RefrigeratorPicking::kOutput);
        if (iter != next_steps_.end()) {
            next_step = iter->second;
        }

        results.push_back(ExecutionResult{next_step, std::move(output), false});
        return results;
    }
};

class RefrigeratorPickingStage : public TemplatedStage<RefrigeratorPicking> {
public:
    RefrigeratorPickingStage(std::string id, Variables&& input)
        : TemplatedStage(std::move(id), true) {
        my_step_ = std::make_shared<RefrigeratorPickingStep>(name_, std::move(input));
    }

    void generateWorkflowHelper(Workflow& workflow) {
        if (next_stages_.size() != 1) {
            return;
        }
        genWorkflowForNextStage(workflow, RefrigeratorPicking::kOutput);
    }
};