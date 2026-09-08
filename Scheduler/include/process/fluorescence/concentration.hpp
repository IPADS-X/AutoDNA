#pragma once

#include "procedure/stage.hpp"
#include "process/dummy.hpp"

class FluoConcentration {
public:
    enum { kOutput = 0 };
    inline static const std::string Name   = "FluoConcentration";
    inline static const std::string Result = "Concentration";
    inline static const std::string Volume = "SampleVolume";

    static nlohmann::json fromDummy(const DummyStep& dummy_step) {
        nlohmann::json input        = nlohmann::json::object();
        auto           dummy_params = dummy_step.getParams();
        auto           volume_key   = std::to_string(static_cast<int>(Dummy::ParamType::VOLUME));
        if (dummy_params.contains(volume_key)) {
            input[Volume] = dummy_params[volume_key];
        } else {
            input[Volume] = 0;
        }
        return input;
    }
};

class FluoConcentrationStep : public CRTPStep<FluoConcentrationStep> {
public:
    FluoConcentrationStep(std::string name, Variables&& user_input = Variables())
        : CRTPStep<FluoConcentrationStep>(MachineType::FLUORESCENCE, name, std::move(user_input)) {
        step_funcs_ = {std::bind(&FluoConcentrationStep::phase0, this, std::placeholders::_1,
                                 std::placeholders::_2, std::placeholders::_3),
                       std::bind(&FluoConcentrationStep::phase1, this, std::placeholders::_1,
                                 std::placeholders::_2, std::placeholders::_3)};
    }

    long long getTime(bool conflict = false) const {
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
        machine->get_concentration_values(action_id, values);

        Variables output                  = step_input_;
        output[FluoConcentration::Result] = values;

        std::shared_ptr<Step> next_step;
        auto                  iter = next_steps_.find(FluoConcentration::kOutput);
        if (iter != next_steps_.end()) {
            next_step = iter->second;
        }

        results.push_back(ExecutionResult{next_step, std::move(output), false});
        return results;
    }

    std::string getOperationName() const override { return "Concentration"; }

    static std::shared_ptr<FluoConcentrationStep> fromDummy(DummyStep& dummy_step) {
        if (dummy_step.getType() == Dummy::DummyType::FLUO_CONCENTRATION) {
            return std::make_shared<FluoConcentrationStep>(
                FluoConcentration::Name, FluoConcentration::fromDummy(dummy_step));
        }
        return nullptr;
    }
};

class FluoConcentrationStage : public TemplatedStage<FluoConcentration> {
public:
    FluoConcentrationStage(std::string id, Variables&& input)
        : TemplatedStage(std::move(id), true) {
        my_step_ =
            std::make_shared<FluoConcentrationStep>(FluoConcentration::Name, std::move(input));
    }

    void generateWorkflowHelper(Workflow& workflow) {
        if (next_stages_.size() != 1) {
            return;
        }
        genWorkflowForNextStage(workflow, FluoConcentration::kOutput);
    }
};
