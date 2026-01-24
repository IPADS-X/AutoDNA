#pragma once

#include "procedure/stage.hpp"
#include "process/dummy.hpp"

class FluoFluo {
public:
    enum { kOutput = 0 };
    inline static const std::string Name   = "FluoFluo";
    inline static const std::string Result = "Result";
};

class FluoFluoStep : public CRTPStep<FluoFluoStep>  {
public:
    FluoFluoStep(std::string name, Variables&& user_input = Variables())
        : CRTPStep<FluoFluoStep> (MachineType::FLUORESCENCE, name, std::move(user_input)) {
        step_funcs_ = {std::bind(&FluoFluoStep::phase0, this, std::placeholders::_1,
                                 std::placeholders::_2, std::placeholders::_3),
                       std::bind(&FluoFluoStep::phase1, this, std::placeholders::_1,
                                 std::placeholders::_2, std::placeholders::_3)};
    }

    long long getTime() const {
        // Simulate time for purification
        return 30;
    }

    std::vector<ExecutionResult>
    phase0(Reality& reality, std::shared_ptr<MachineManager> mac_manager, ActionId action_id) {
        std::vector<ExecutionResult> results;
        auto machine = mac_manager->getMachine<FluorescenceModbusMachine>(machine_type_);

        machine->start_read_fluorescence(action_id);

        results.push_back(ExecutionResult());
        return results;
    }

    std::vector<ExecutionResult>
    phase1(Reality& reality, std::shared_ptr<MachineManager> mac_manager, ActionId action_id) {
        std::vector<ExecutionResult> results;
        auto machine = mac_manager->getMachine<FluorescenceModbusMachine>(machine_type_);

        std::vector<float> values;
        machine->get_fluorescence_values(action_id, values);

        Variables             output = step_input_;
        std::shared_ptr<Step> next_step;
        output[FluoFluo::Result] = values;

        auto iter = next_steps_.find(FluoFluo::kOutput);
        if (iter != next_steps_.end()) {
            next_step = iter->second;
        }

        results.push_back(ExecutionResult{next_step, std::move(output), false});
        return results;
    }

    static std::shared_ptr<FluoFluoStep> fromDummy(DummyStep& dummy_step) {
        if (dummy_step.getType() == Dummy::DummyType::FLUO) {
            return std::make_shared<FluoFluoStep>(FluoFluo::Name, Variables());
        }
        return nullptr;
    }
};

class FluoFluoStage : public TemplatedStage<FluoFluo> {
public:
    FluoFluoStage(std::string id, Variables&& input) : TemplatedStage(std::move(id), true) {
        my_step_ = std::make_shared<FluoFluoStep>(FluoFluo::Name, std::move(input));
    }

    void generateWorkflowHelper(Workflow& workflow) {
        if (next_stages_.size() != 1) {
            return;
        }
        genWorkflowForNextStage(workflow, FluoFluo::kOutput);
    }
};