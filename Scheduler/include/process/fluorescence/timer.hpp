#pragma once

#include "procedure/stage.hpp"
#include "process/dummy.hpp"

class FluoTime {
public:
    enum { kOutput = 0 };
    inline static const std::string Name     = "FluoTime";
    inline static const std::string Duration = "Duration";

    static nlohmann::json fromDummy(const DummyStep& dummy_step) {
        nlohmann::json input        = nlohmann::json::object();
        auto           dummy_params = dummy_step.getParams();

        auto num = 1;

        if (dummy_params.contains(
                std::to_string(static_cast<int>(Dummy::ParamType::PIPETTE_NUM)))) {
            num = dummy_params[std::to_string(static_cast<int>(Dummy::ParamType::PIPETTE_NUM))];
        }

        if (dummy_params.contains(std::to_string(static_cast<int>(Dummy::ParamType::DURATION)))) {
            input[Duration] =
                num *
                (int)dummy_params[std::to_string(static_cast<int>(Dummy::ParamType::DURATION))];
        } else {
            input[Duration] = 30 * 1000;
        }

        return input;
    }
};

class FluoTimeStep : public CRTPStep<FluoTimeStep> {
public:
    FluoTimeStep(std::string name, Variables&& user_input = Variables())
        : CRTPStep<FluoTimeStep>(MachineType::FLUORESCENCE, name, std::move(user_input)) {
        step_funcs_ = {std::bind(&FluoTimeStep::phase0, this, std::placeholders::_1,
                                 std::placeholders::_2, std::placeholders::_3),
                       std::bind(&FluoTimeStep::phase1, this, std::placeholders::_1,
                                 std::placeholders::_2, std::placeholders::_3)};
    }

    long long getTime() const {
        // Simulate time for purification
        auto duration = user_input_[FluoTime::Duration].get<uint32_t>();
        return duration / 1000;
    }

    std::vector<ExecutionResult>
    phase0(Reality& reality, std::shared_ptr<MachineManager> mac_manager, ActionId action_id) {
        std::vector<ExecutionResult> results;
        auto machine = mac_manager->getMachine<FluorescenceModbusMachine>(machine_type_);

        auto duration = user_input_[FluoTime::Duration].get<uint32_t>();

        machine->time(duration, action_id);

        results.push_back(ExecutionResult());
        return results;
    }

    std::vector<ExecutionResult>
    phase1(Reality& reality, std::shared_ptr<MachineManager> mac_manager, ActionId action_id) {
        std::vector<ExecutionResult> results;

        results.push_back(ExecutionResult{next_steps_[FluoTime::kOutput], Variables(), false});
        return results;
    }

    std::string getOperationName() const override { return "Time"; }

    std::vector<Equipment> getLockedEquipment() const { return {}; }

    std::vector<Equipment> getUnlockedEquipment() const { return {}; }

    static std::shared_ptr<FluoTimeStep> fromDummy(DummyStep& dummy_step) {
        if (dummy_step.getType() == Dummy::DummyType::TIME) {
            return std::make_shared<FluoTimeStep>(FluoTime::Name, FluoTime::fromDummy(dummy_step));
        }
        return nullptr;
    }
};

class FluoTimeStage : public TemplatedStage<FluoTime> {
public:
    FluoTimeStage(std::string id, Variables&& input) : TemplatedStage(std::move(id), true) {
        my_step_ = std::make_shared<FluoTimeStep>(name_, std::move(input));
    }

    void generateWorkflowHelper(Workflow& workflow) {
        if (next_stages_.size() != 1) {
            return;
        }
        genWorkflowForNextStage(workflow, FluoTime::kOutput);
    }
};