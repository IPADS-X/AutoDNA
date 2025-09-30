#pragma once

#include "procedure/stage.hpp"

class ForLoop {
public:
    enum { kTrueOutput = 0, kFalseOutput };
    inline static const std::string Name         = "ForLoop";
    inline static const std::string Variable     = "Variable";
    inline static const std::string InitialValue = "InitialValue";
    inline static const std::string Sign         = "Sign";
    inline static const std::string Value        = "Value";

    enum Sign { kGreater = 0, kGreaterEqual, kLess, kLessEqual, kEqual, kNotEqual };
};

class ForLoopStep : public CRTPStep<ForLoopStep> {

    inline static std::function<bool(int, int)> comparators[] = {
        std::greater<int>(),    std::greater_equal<int>(), std::less<int>(),
        std::less_equal<int>(), std::equal_to<int>(),      std::not_equal_to<int>()};

    inline static std::string symbols[] = {">", ">=", "<", "<=", "==", "!="};

public:
    ForLoopStep(std::string name, Variables&& user_input)
        : CRTPStep<ForLoopStep>(MachineType::TOTAL_NUM, name, std::move(user_input)) {
        step_funcs_ = {std::bind(&ForLoopStep::phase0, this, std::placeholders::_1,
                                 std::placeholders::_2, std::placeholders::_3)};
    }

    std::vector<ExecutionResult>
    phase0(Reality& reality, std::shared_ptr<MachineManager> mac_manager, ActionId action_id) {
        std::vector<ExecutionResult> results;
        Variables                    output = step_input_;

        if (output.find(ForLoop::Variable) == output.end()) {
            output[ForLoop::Variable] = user_input_[ForLoop::InitialValue];
        }

        int left  = output[ForLoop::Variable];
        int sign  = user_input_[ForLoop::Sign];
        int right = user_input_[ForLoop::Value];

        bool condition = comparators[sign](left, right);

        logger->debug("ForLoopStep {} {} {} {}", left, symbols[sign], right, condition);

        output[ForLoop::Variable] = left + 1;
        if (condition) {
            results.push_back(
                ExecutionResult{next_steps_[ForLoop::kTrueOutput], std::move(output), false});
        } else {
            if (next_steps_.find(ForLoop::kFalseOutput) != next_steps_.end()) {
                results.push_back(
                    ExecutionResult{next_steps_[ForLoop::kFalseOutput], std::move(output), false});
            } else {
                results.push_back(ExecutionResult{nullptr, std::move(output), false});
            }
        }

        return results;
    }
};

class ForLoopStage : public TemplatedStage<ForLoop> {
public:
    ForLoopStage(std::string id, Variables&& input) : TemplatedStage(std::move(id), false) {
        my_step_ = std::make_shared<ForLoopStep>(name_, std::move(input));
    }

    void generateWorkflowHelper(Workflow& workflow) {
        genWorkflowForNextStage(workflow, ForLoop::kTrueOutput);
        if (next_stages_.find(ForLoop::kFalseOutput) != next_stages_.end()) {
            genWorkflowForNextStage(workflow, ForLoop::kFalseOutput);
        }
    }
};
