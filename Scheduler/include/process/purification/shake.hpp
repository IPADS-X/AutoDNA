#pragma once

#include "procedure/stage.hpp"
#include "process/dummy.hpp"

class PuriShake {
public:
    enum { kOutput = 0 };
    inline static const std::string Name     = "PuriShake";
    inline static const std::string Duration = "Duration";
    inline static const std::string Speed    = "Speed";
    inline static const std::string Temp     = "Temp";

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

        if (input[Speed] < 200) {
            input[Speed] = 200;
        }

        if (dummy_params.contains(
                std::to_string(static_cast<int>(Dummy::ParamType::TEMPERATURE)))) {
            input[Temp] =
                dummy_params[std::to_string(static_cast<int>(Dummy::ParamType::TEMPERATURE))];
        } else {
            input[Temp] = 25;
        }

        return input;
    }
};

class PuriShakeStep : public CRTPStep<PuriShakeStep> {
public:
    PuriShakeStep(std::string name, Variables&& user_input = Variables())
        : CRTPStep<PuriShakeStep>(MachineType::PURIFICATION, name, std::move(user_input)) {
        step_funcs_ = {std::bind(&PuriShakeStep::phase0, this, std::placeholders::_1,
                                 std::placeholders::_2, std::placeholders::_3),
                       std::bind(&PuriShakeStep::phase1, this, std::placeholders::_1,
                                 std::placeholders::_2, std::placeholders::_3)};
    }

    long long getTime() const {
        // Simulate time for shaking
        // return user_input_.value(PuriShake::Duration, 0);
        return user_input_.value(PuriShake::Duration, 0) / 1000;
    }

    std::vector<ExecutionResult>
    phase0(Reality& reality, std::shared_ptr<MachineManager> mac_manager, ActionId action_id) {
        std::vector<ExecutionResult> results;
        auto machine = mac_manager->getMachine<PurificationModbusMachine>(machine_type_);

        auto     duration = user_input_[PuriShake::Duration].get<uint32_t>() / 1000;
        auto     speed    = user_input_[PuriShake::Speed].get<uint16_t>();
        uint16_t temp     = 0;
        if (user_input_.find(PuriShake::Temp) != user_input_.end()) {
            temp = user_input_[PuriShake::Temp].get<uint16_t>();
        } else {
            temp = 0;
        }

        machine->shake(duration, speed, temp, action_id);
        results.push_back(ExecutionResult());
        // results.push_back(ExecutionResult{next_steps_[PuriShake::kOutput], Variables(), false});
        return results;
    }

    std::vector<ExecutionResult>
    phase1(Reality& reality, std::shared_ptr<MachineManager> mac_manager, ActionId action_id) {
        std::vector<ExecutionResult> results;

        results.push_back(ExecutionResult{next_steps_[PuriShake::kOutput], Variables(), false});
        return results;
    }

    static std::shared_ptr<PuriShakeStep> fromDummy(DummyStep& dummy_step) {
        if (dummy_step.getType() == Dummy::DummyType::SHAKE) {
            return std::make_shared<PuriShakeStep>(PuriShake::Name,
                                                   PuriShake::fromDummy(dummy_step));
        }
        return nullptr;
    }
};

class PuriShakeStage : public TemplatedStage<PuriShake> {
public:
    PuriShakeStage(std::string id, Variables&& input) : TemplatedStage(std::move(id), true) {
        my_step_ = std::make_shared<PuriShakeStep>(name_, std::move(input));
    }

    void generateWorkflowHelper(Workflow& workflow) {
        if (next_stages_.size() != 1) {
            return;
        }
        genWorkflowForNextStage(workflow, PuriShake::kOutput);
    }
};