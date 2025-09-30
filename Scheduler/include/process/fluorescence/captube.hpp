#pragma once

#include "procedure/stage.hpp"
#include "process/dummy.hpp"

class FluoCapTube {
public:
    enum { kOutput = 0 };
    inline static const std::string Name = "FluoCapTube";
};

class FluoCapTubeStep : public CRTPStep<FluoCapTubeStep> {
public:
    FluoCapTubeStep(std::string name, Variables&& user_input = Variables())
        : CRTPStep<FluoCapTubeStep>(MachineType::FLUORESCENCE, name, std::move(user_input)) {
        step_funcs_ = {std::bind(&FluoCapTubeStep::phase0, this, std::placeholders::_1,
                                 std::placeholders::_2, std::placeholders::_3),
                       std::bind(&FluoCapTubeStep::phase1, this, std::placeholders::_1,
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

        machine->capTubes(action_id);

        results.push_back(ExecutionResult());
        return results;
    }

    std::vector<ExecutionResult>
    phase1(Reality& reality, std::shared_ptr<MachineManager> mac_manager, ActionId action_id) {
        std::vector<ExecutionResult> results;

        results.push_back(ExecutionResult{next_steps_[FluoCapTube::kOutput], Variables(), false});
        return results;
    }

    static std::shared_ptr<FluoCapTubeStep> fromDummy(DummyStep& dummy_step) {
        if (dummy_step.getType() == Dummy::DummyType::CAP) {
            return std::make_shared<FluoCapTubeStep>(FluoCapTube::Name, Variables());
        }
        return nullptr;
    }
};

class FluoCapTubeStage : public TemplatedStage<FluoCapTube> {
public:
    FluoCapTubeStage(std::string id, Variables&& input) : TemplatedStage(std::move(id), true) {
        my_step_ = std::make_shared<FluoCapTubeStep>(name_, std::move(input));
    }

    void generateWorkflowHelper(Workflow& workflow) {
        if (next_stages_.size() != 1) {
            return;
        }
        genWorkflowForNextStage(workflow, FluoCapTube::kOutput);
    }
};