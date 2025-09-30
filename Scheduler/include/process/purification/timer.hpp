#pragma once

#include "procedure/stage.hpp"
#include "process/dummy.hpp"

class PuriTime {
public:
    enum { kOutput = 0 };
    inline static const std::string Name     = "PuriTime";
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

class PuriTimeStep : public CRTPStep<PuriTimeStep> {
public:
    PuriTimeStep(std::string name, Variables&& user_input = Variables())
        : CRTPStep<PuriTimeStep>(MachineType::PURIFICATION, name, std::move(user_input)) {
        step_funcs_ = {std::bind(&PuriTimeStep::phase0, this, std::placeholders::_1,
                                 std::placeholders::_2, std::placeholders::_3),
                       std::bind(&PuriTimeStep::phase1, this, std::placeholders::_1,
                                 std::placeholders::_2, std::placeholders::_3)};
    }

    long long getTime() const {
        // Simulate time for purification
        return user_input_.value(PuriTime::Duration, 0) / 1000;
    }

    std::vector<ExecutionResult>
    phase0(Reality& reality, std::shared_ptr<MachineManager> mac_manager, ActionId action_id) {
        std::vector<ExecutionResult> results;
        auto machine = mac_manager->getMachine<PurificationModbusMachine>(machine_type_);

        auto duration = user_input_[PuriTime::Duration].get<uint16_t>();

        machine->time(duration, action_id);

        results.push_back(ExecutionResult());
        return results;
    }

    std::vector<ExecutionResult>
    phase1(Reality& reality, std::shared_ptr<MachineManager> mac_manager, ActionId action_id) {
        std::vector<ExecutionResult> results;

        results.push_back(ExecutionResult{next_steps_[PuriTime::kOutput], Variables(), false});
        return results;
    }

    std::vector<Equipment> getLockedEquipment() const { return {}; }

    std::vector<Equipment> getUnlockedEquipment() const { return {}; }

    static std::shared_ptr<PuriTimeStep> fromDummy(DummyStep& dummy_step) {
        if (dummy_step.getType() == Dummy::DummyType::TIME) {
            return std::make_shared<PuriTimeStep>(PuriTime::Name, PuriTime::fromDummy(dummy_step));
        }
        return nullptr;
    }
};

class PuriTimeStage : public TemplatedStage<PuriTime> {
public:
    PuriTimeStage(std::string id, Variables&& input) : TemplatedStage(std::move(id), true) {
        my_step_ = std::make_shared<PuriTimeStep>(name_, std::move(input));
    }

    void generateWorkflowHelper(Workflow& workflow) {
        if (next_stages_.size() != 1) {
            return;
        }
        genWorkflowForNextStage(workflow, PuriTime::kOutput);
    }
};