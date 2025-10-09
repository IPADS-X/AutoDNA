#pragma once

#include "procedure/stage.hpp"

class RefrigeratorPlacement {
public:
    inline static const std::string Name     = "RefrigeratorPlacement";
    inline static const std::string Position = "Position";
};

class RefrigeratorPlacementStep : public CRTPStep<RefrigeratorPlacementStep> {
public:
    RefrigeratorPlacementStep(std::string name, Variables&& user_input)
        : CRTPStep<RefrigeratorPlacementStep>(MachineType::REFRIGERATOR, name, std::move(user_input)) {
        step_funcs_ = {std::bind(&RefrigeratorPlacementStep::phase0, this, std::placeholders::_1,
                                 std::placeholders::_2, std::placeholders::_3),
                       std::bind(&RefrigeratorPlacementStep::phase1, this, std::placeholders::_1,
                                 std::placeholders::_2, std::placeholders::_3)};
    }

    std::vector<ExecutionResult>
    phase0(Reality& reality, std::shared_ptr<MachineManager> mac_manager, ActionId action_id) {
        std::vector<ExecutionResult> results;

        results.push_back(ExecutionResult());
        return results;
    }

    std::vector<ExecutionResult>
    phase1(Reality& reality, std::shared_ptr<MachineManager> mac_manager, ActionId action_id) {
        std::vector<ExecutionResult> results;

        Variables output;
        output["key"] = "RefrigeratorPlacementStepOutput";
        results.push_back(ExecutionResult{nullptr, std::move(output), false});
        return results;
    }
};

class RefrigeratorPlacementStage : public TemplatedStage<RefrigeratorPlacement> {
public:
    RefrigeratorPlacementStage(std::string id, Variables&& input)
        : TemplatedStage(std::move(id), true) {
        my_step_ = std::make_shared<RefrigeratorPlacementStep>(name_, std::move(input));
    }

    void generateWorkflowHelper(Workflow& workflow) {}
};