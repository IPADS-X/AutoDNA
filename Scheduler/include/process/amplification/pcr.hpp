#pragma once

#include "procedure/stage.hpp"
#include "process/dummy.hpp"

class AmpPcr {
public:
    enum { kOutput = 0 };
    inline static const std::string Name = "AmpPcr";
    inline static const std::string File = "File";

    static nlohmann::json fromDummy(const DummyStep& dummy_step) {
        nlohmann::json input        = nlohmann::json::object();
        auto           dummy_params = dummy_step.getParams();

        if (dummy_params.contains(
                std::to_string(static_cast<int>(Dummy::ParamType::TEMPERATURE))) &&
            dummy_params.contains(std::to_string(static_cast<int>(Dummy::ParamType::DURATION)))) {
            auto temp =
                dummy_params[std::to_string(static_cast<int>(Dummy::ParamType::TEMPERATURE))];
            auto dur = dummy_params[std::to_string(static_cast<int>(Dummy::ParamType::DURATION))];
            if (temp == 39 && dur == 300000) {
                input[File] = 4;
            } else {
                input[File] = 1;
            }
        } else {
            input[File] = 1;
        }

        return input;
    }
};

class AmpPcrStep : public CRTPStep<AmpPcrStep> {
public:
    AmpPcrStep(std::string name, Variables&& user_input = Variables())
        : CRTPStep<AmpPcrStep>(MachineType::AMPLIFICATION, name, std::move(user_input)) {
        step_funcs_ = {std::bind(&AmpPcrStep::phase0, this, std::placeholders::_1,
                                 std::placeholders::_2, std::placeholders::_3),
                       std::bind(&AmpPcrStep::phase1, this, std::placeholders::_1,
                                 std::placeholders::_2, std::placeholders::_3)};
    }

    std::vector<ExecutionResult>
    phase0(Reality& reality, std::shared_ptr<MachineManager> mac_manager, ActionId action_id) {
        std::vector<ExecutionResult> results;
        auto machine = mac_manager->getMachine<AmplificationModbusMachine>(machine_type_);

        auto file = user_input_[AmpPcr::File].get<uint16_t>();

        machine->pcr(file, action_id);

        results.push_back(ExecutionResult());
        return results;
    }

    std::vector<ExecutionResult>
    phase1(Reality& reality, std::shared_ptr<MachineManager> mac_manager, ActionId action_id) {
        std::vector<ExecutionResult> results;

        results.push_back(ExecutionResult{next_steps_[AmpPcr::kOutput], Variables(), false});
        return results;
    }

    static std::shared_ptr<AmpPcrStep> fromDummy(DummyStep& dummy_step) {
        if (dummy_step.getType() == Dummy::DummyType::PCR) {
            return std::make_shared<AmpPcrStep>(AmpPcr::Name, AmpPcr::fromDummy(dummy_step));
        }
        return nullptr;
    }
};

class AmpPcrStage : public TemplatedStage<AmpPcr> {
public:
    AmpPcrStage(std::string id, Variables&& input) : TemplatedStage(std::move(id), true) {
        my_step_ = std::make_shared<AmpPcrStep>(name_, std::move(input));
    }

    void generateWorkflowHelper(Workflow& workflow) {
        if (next_stages_.size() != 1) {
            return;
        }
        genWorkflowForNextStage(workflow, AmpPcr::kOutput);
    }
};