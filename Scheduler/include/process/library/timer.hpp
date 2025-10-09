#pragma once

#include "procedure/stage.hpp"
#include "process/dummy.hpp"

class LibTime {
public:
    enum { kOutput = 0 };
    inline static const std::string Name     = "LibTime";
    inline static const std::string Duration = "Duration";

    static nlohmann::json fromDummy(const DummyStep& dummy_step) {
        nlohmann::json input        = nlohmann::json::object();
        auto           dummy_params = dummy_step.getParams();

        if (dummy_params.contains(std::to_string(static_cast<int>(Dummy::ParamType::DURATION)))) {
            input[Duration] =
                dummy_params[std::to_string(static_cast<int>(Dummy::ParamType::DURATION))];
        } else {
            input[Duration] = 30 * 1000;
        }

        return input;
    }
};

class LibTimeStep : public CRTPStep<LibTimeStep> {
public:
    LibTimeStep(std::string name, Variables&& user_input = Variables())
        : CRTPStep<LibTimeStep>(MachineType::LIBRARY, name, std::move(user_input)) {
        step_funcs_ = {std::bind(&LibTimeStep::phase0, this, std::placeholders::_1,
                                 std::placeholders::_2, std::placeholders::_3),
                       std::bind(&LibTimeStep::phase1, this, std::placeholders::_1,
                                 std::placeholders::_2, std::placeholders::_3)};
    }

    long long getTime() const {
        // Simulate time for purification
        auto duration = user_input_[LibTime::Duration].get<uint32_t>();
        return duration / 1000;
    }

    std::vector<ExecutionResult>
    phase0(Reality& reality, std::shared_ptr<MachineManager> mac_manager, ActionId action_id) {
        std::vector<ExecutionResult> results;
        auto machine = mac_manager->getMachine<LibraryModbusMachine>(machine_type_);

        auto duration = user_input_[LibTime::Duration].get<uint32_t>();

        machine->time(duration, action_id);

        results.push_back(ExecutionResult());
        return results;
    }

    std::vector<ExecutionResult>
    phase1(Reality& reality, std::shared_ptr<MachineManager> mac_manager, ActionId action_id) {
        std::vector<ExecutionResult> results;

        results.push_back(ExecutionResult{next_steps_[LibTime::kOutput], Variables(), false});
        return results;
    }

    std::vector<Equipment> getLockedEquipment() const { return {}; }

    std::vector<Equipment> getUnlockedEquipment() const { return {}; }

    static std::shared_ptr<LibTimeStep> fromDummy(DummyStep& dummy_step) {
        if (dummy_step.getType() == Dummy::DummyType::TIME) {
            return std::make_shared<LibTimeStep>(LibTime::Name, LibTime::fromDummy(dummy_step));
        }
        return nullptr;
    }
};

class LibTimeStage : public TemplatedStage<LibTime> {
public:
    LibTimeStage(std::string id, Variables&& input) : TemplatedStage(std::move(id), true) {
        my_step_ = std::make_shared<LibTimeStep>(name_, std::move(input));
    }

    void generateWorkflowHelper(Workflow& workflow) {
        if (next_stages_.size() != 1) {
            return;
        }
        genWorkflowForNextStage(workflow, LibTime::kOutput);
    }
};