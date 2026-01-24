#pragma once

#include "procedure/stage.hpp"
#include "process/dummy.hpp"

class PuriPcr {
public:
    enum { kOutput = 0 };
    inline static const std::string Name = "PuriPcr";
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
            if (temp == 39 && dur == 5 * 60) {
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

class PuriPcrStep : public CRTPStep<PuriPcrStep> {
public:
    PuriPcrStep(std::string name, Variables&& user_input = Variables())
        : CRTPStep<PuriPcrStep>(MachineType::PURIFICATION, name, std::move(user_input)) {
        step_funcs_ = {std::bind(&PuriPcrStep::phase0, this, std::placeholders::_1,
                                 std::placeholders::_2, std::placeholders::_3),
                       std::bind(&PuriPcrStep::phase1, this, std::placeholders::_1,
                                 std::placeholders::_2, std::placeholders::_3)};
    }

    std::vector<ExecutionResult>
    phase0(Reality& reality, std::shared_ptr<MachineManager> mac_manager, ActionId action_id) {
        std::vector<ExecutionResult> results;
        auto machine = mac_manager->getMachine<PurificationModbusMachine>(machine_type_);

        auto file = user_input_[PuriPcr::File].get<uint16_t>();

        machine->pcr(file, action_id);

        results.push_back(ExecutionResult());
        return results;
    }

    std::vector<ExecutionResult>
    phase1(Reality& reality, std::shared_ptr<MachineManager> mac_manager, ActionId action_id) {
        std::vector<ExecutionResult> results;

        results.push_back(ExecutionResult{next_steps_[PuriPcr::kOutput], Variables(), false});
        return results;
    }

    static std::shared_ptr<PuriPcrStep> fromDummy(DummyStep& dummy_step) {
        if (dummy_step.getType() == Dummy::DummyType::PCR) {
            return std::make_shared<PuriPcrStep>(PuriPcr::Name, PuriPcr::fromDummy(dummy_step));
        }
        return nullptr;
    }
};

class PuriPcrStage : public TemplatedStage<PuriPcr> {
public:
    PuriPcrStage(std::string id, Variables&& input) : TemplatedStage(std::move(id), true) {
        my_step_ = std::make_shared<PuriPcrStep>(name_, std::move(input));
    }

    void generateWorkflowHelper(Workflow& workflow) {
        if (next_stages_.size() != 1) {
            return;
        }
        genWorkflowForNextStage(workflow, PuriPcr::kOutput);
    }
};