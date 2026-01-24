#pragma once

#include "procedure/stage.hpp"
#include "process/dummy.hpp"

class LibCentrifuge {
public:
    enum { kOutput = 0 };
    inline static const std::string Name     = "LibCentrifuge";
    inline static const std::string Duration = "Duration";
    inline static const std::string Speed    = "Speed";
    inline static const std::string Type     = "Type"; // 0: pcr, 1: 8

    enum class CentrifugeType { TUBE_PCR, TUBE_8, TUBE_MIXER };

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

        if (dummy_params.contains(std::to_string(static_cast<int>(Dummy::ParamType::TUBE_TYPE)))) {
            if (dummy_params[std::to_string(static_cast<int>(Dummy::ParamType::TUBE_TYPE))] ==
                (uint16_t)TubeType::PCR_TUBE) {
                input[Type] = (uint16_t)CentrifugeType::TUBE_PCR;
            } else {
                input[Type] = (uint16_t)CentrifugeType::TUBE_8;
            }
        } else {
            input[Type] = (uint16_t)CentrifugeType::TUBE_8;
        }

        return input;
    }
};

class LibCentrifugeStep : public CRTPStep<LibCentrifugeStep> {
public:
    LibCentrifugeStep(std::string name, Variables&& user_input = Variables())
        : CRTPStep<LibCentrifugeStep>(MachineType::LIBRARY, name, std::move(user_input)) {
        step_funcs_ = {std::bind(&LibCentrifugeStep::phase0, this, std::placeholders::_1,
                                 std::placeholders::_2, std::placeholders::_3),
                       std::bind(&LibCentrifugeStep::phase1, this, std::placeholders::_1,
                                 std::placeholders::_2, std::placeholders::_3)};
    }

    long long getTime() const {
        // Simulate time for shaking
        return user_input_.value(LibCentrifuge::Duration, 0) / 1000;
    }

    std::vector<ExecutionResult>
    phase0(Reality& reality, std::shared_ptr<MachineManager> mac_manager, ActionId action_id) {
        std::vector<ExecutionResult> results;
        auto machine = mac_manager->getMachine<LibraryModbusMachine>(machine_type_);

        auto duration = user_input_[LibCentrifuge::Duration].get<uint16_t>();
        auto speed    = user_input_[LibCentrifuge::Speed].get<uint16_t>();

        if (user_input_.find(LibCentrifuge::Type) != user_input_.end()) {
            auto type = user_input_[LibCentrifuge::Type].get<uint16_t>();
            if (type == (uint16_t)LibCentrifuge::CentrifugeType::TUBE_PCR) {
                machine->centrifuge_pcr_tube(duration, speed, action_id);
            } else if (type == (uint16_t)LibCentrifuge::CentrifugeType::TUBE_MIXER) {
                machine->centrifuge_mix_8_tube(duration, speed, action_id);
            } else {
                machine->centrifuge_8_tube(duration, speed, action_id);
            }
        } else {
            machine->centrifuge_8_tube(duration, speed, action_id);
        }
        results.push_back(ExecutionResult());
        // results.push_back(ExecutionResult{next_steps_[PuriShake::kOutput], Variables(), false});
        return results;
    }

    std::vector<ExecutionResult>
    phase1(Reality& reality, std::shared_ptr<MachineManager> mac_manager, ActionId action_id) {
        std::vector<ExecutionResult> results;

        results.push_back(ExecutionResult{next_steps_[LibCentrifuge::kOutput], Variables(), false});
        return results;
    }

    static std::shared_ptr<LibCentrifugeStep> fromDummy(DummyStep& dummy_step) {
        if (dummy_step.getType() == Dummy::DummyType::CENTRIFUGE || dummy_step.getType() == Dummy::DummyType::CENTRIFUGE_PCR_TUBE) {
            return std::make_shared<LibCentrifugeStep>(LibCentrifuge::Name,
                                                       LibCentrifuge::fromDummy(dummy_step));
        }
        return nullptr;
    }
};

class LibCentrifugeStage : public TemplatedStage<LibCentrifuge> {
public:
    LibCentrifugeStage(std::string id, Variables&& input) : TemplatedStage(std::move(id), true) {
        my_step_ = std::make_shared<LibCentrifugeStep>(name_, std::move(input));
    }

    void generateWorkflowHelper(Workflow& workflow) {
        if (next_stages_.size() != 1) {
            return;
        }
        genWorkflowForNextStage(workflow, LibCentrifuge::kOutput);
    }
};