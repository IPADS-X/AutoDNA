#pragma once

#include "procedure/stage.hpp"
#include "process/dummy.hpp"

class PuriCentrifuge {
public:
    enum { kOutput = 0 };
    inline static const std::string Name     = "PuriCentrifuge";
    inline static const std::string Duration = "Duration";
    inline static const std::string Speed    = "Speed";

    static nlohmann::json fromDummy(const DummyStep& dummy_step) {
        nlohmann::json input        = nlohmann::json::object();
        auto           dummy_params = dummy_step.getParams();
        if (dummy_params.contains(std::to_string(static_cast<int>(Dummy::ParamType::DURATION)))) {
            input[Duration] =
                dummy_params[std::to_string(static_cast<int>(Dummy::ParamType::DURATION))];
        } else {
            input[Duration] = 30 * 1000;
        }

        if (dummy_params.contains(std::to_string(static_cast<int>(Dummy::ParamType::MIX_SPEED)))) {
            input[Speed] =
                dummy_params[std::to_string(static_cast<int>(Dummy::ParamType::MIX_SPEED))];
        } else {
            input[Speed] = 1000;
        }

        return input;
    }
};

class PuriCentrifugeStep : public CRTPStep<PuriCentrifugeStep> {
public:
    PuriCentrifugeStep(std::string name, Variables&& user_input = Variables())
        : CRTPStep<PuriCentrifugeStep>(MachineType::PURIFICATION, name, std::move(user_input)) {
        step_funcs_ = {std::bind(&PuriCentrifugeStep::phase0, this, std::placeholders::_1,
                                 std::placeholders::_2, std::placeholders::_3),
                       std::bind(&PuriCentrifugeStep::phase1, this, std::placeholders::_1,
                                 std::placeholders::_2, std::placeholders::_3)};
    }

    long long getTime() const {
        // Simulate time for shaking
        return user_input_.value(PuriCentrifuge::Duration, 0) / 1000;
    }

    std::vector<ExecutionResult>
    phase0(Reality& reality, std::shared_ptr<MachineManager> mac_manager, ActionId action_id) {
        std::vector<ExecutionResult> results;
        auto machine = mac_manager->getMachine<PurificationModbusMachine>(machine_type_);

        auto duration = user_input_[PuriCentrifuge::Duration].get<uint16_t>();
        auto speed    = user_input_[PuriCentrifuge::Speed].get<uint16_t>();

        machine->centrifuge(duration, speed, action_id);
        results.push_back(ExecutionResult());
        // results.push_back(ExecutionResult{next_steps_[PuriShake::kOutput], Variables(), false});
        return results;
    }

    std::vector<ExecutionResult>
    phase1(Reality& reality, std::shared_ptr<MachineManager> mac_manager, ActionId action_id) {
        std::vector<ExecutionResult> results;

        results.push_back(
            ExecutionResult{next_steps_[PuriCentrifuge::kOutput], Variables(), false});
        return results;
    }

    static std::shared_ptr<PuriCentrifugeStep> fromDummy(DummyStep& dummy_step) {
        if (dummy_step.getType() == Dummy::DummyType::CENTRIFUGE) {
            return std::make_shared<PuriCentrifugeStep>(PuriCentrifuge::Name,
                                                        PuriCentrifuge::fromDummy(dummy_step));
        }
        return nullptr;
    }
};

class PuriCentrifugeStage : public TemplatedStage<PuriCentrifuge> {
public:
    PuriCentrifugeStage(std::string id, Variables&& input) : TemplatedStage(std::move(id), true) {
        my_step_ = std::make_shared<PuriCentrifugeStep>(name_, std::move(input));
    }

    void generateWorkflowHelper(Workflow& workflow) {
        if (next_stages_.size() != 1) {
            return;
        }
        genWorkflowForNextStage(workflow, PuriCentrifuge::kOutput);
    }
};