#pragma once

#include "procedure/stage.hpp"

class Branch {
public:
    enum { kTrueOutput = 0, kFalseOutput };
    inline static const std::string Name     = "Branch";
    inline static const std::string Standard = "Standard";
};

class BranchStep : public CRTPStep<BranchStep> {
public:
    BranchStep(std::string name, Variables&& user_input)
        : CRTPStep<BranchStep>(MachineType::TOTAL_NUM, name, std::move(user_input)) {
        step_funcs_ = {std::bind(&BranchStep::phase0, this, std::placeholders::_1,
                                 std::placeholders::_2, std::placeholders::_3)};
    }

    std::vector<ExecutionResult>
    phase0(Reality& reality, std::shared_ptr<MachineManager> mac_manager, ActionId action_id) {
        std::vector<ExecutionResult> results;

        // float result   = step_input_[Fluorescence::Result];
        // float standard = user_input_[Branch::Standard];

        // static int i = 0;
        // ++i;

        // Variables output = step_input_;
        // if (result > standard || i > 5) {
        //     std::cout << "Executing BranchStep: " << id_ << " (true)" << std::endl;
        //     results.push_back(
        //         ExecutionResult{next_steps_[Branch::kTrueOutput], std::move(output), false});
        // } else {
        //     std::cout << "Executing BranchStep: " << id_ << " (false)" << std::endl;
        //     results.push_back(
        //         ExecutionResult{next_steps_[Branch::kFalseOutput], std::move(output), false});
        // }

        return results;
    }
};

class BranchStage : public TemplatedStage<Branch> {
public:
    BranchStage(std::string id, Variables&& input) : TemplatedStage(std::move(id), false) {
        my_step_ = std::make_shared<BranchStep>(name_, std::move(input));
    }

    void generateWorkflowHelper(Workflow& workflow) {
        genWorkflowForNextStage(workflow, Branch::kTrueOutput);
        genWorkflowForNextStage(workflow, Branch::kFalseOutput);
    }
};
