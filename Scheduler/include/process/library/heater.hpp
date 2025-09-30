#pragma once

#include "procedure/stage.hpp"
#include "process/dummy.hpp"

class LibHeat {
public:
    enum { kOutput = 0 };
    inline static const std::string Name        = "LibHeat";
    inline static const std::string Duration    = "Duration";
    inline static const std::string Temperature = "Temperature";

    static nlohmann::json fromDummy(const DummyStep& dummy_step) {
        nlohmann::json input        = nlohmann::json::object();
        auto           dummy_params = dummy_step.getParams();
        if (dummy_params.contains(std::to_string(static_cast<int>(Dummy::ParamType::DURATION)))) {
            input[Duration] =
                dummy_params[std::to_string(static_cast<int>(Dummy::ParamType::DURATION))];
        } else {
            input[Duration] = 30 * 1000;
        }
        if (dummy_params.contains(
                std::to_string(static_cast<int>(Dummy::ParamType::TEMPERATURE)))) {
            input[Temperature] =
                dummy_params[std::to_string(static_cast<int>(Dummy::ParamType::TEMPERATURE))];
        } else {
            input[Temperature] = 37;
        }
        return input;
    }
};

class LibHeatStep : public CRTPStep<LibHeatStep> {
public:
    LibHeatStep(std::string name, Variables&& user_input = Variables())
        : CRTPStep<LibHeatStep>(MachineType::LIBRARY, name, std::move(user_input)) {
        step_funcs_ = {std::bind(&LibHeatStep::phase0, this, std::placeholders::_1,
                                 std::placeholders::_2, std::placeholders::_3),
                       std::bind(&LibHeatStep::phase1, this, std::placeholders::_1,
                                 std::placeholders::_2, std::placeholders::_3)};
    }

    long long getTime() const {
        // Simulate time for purification
        auto duration = user_input_[LibHeat::Duration].get<uint32_t>();
        return duration / 1000;
    }

    const std::string getBlockMessage(int threshold = 10) {
        return "Error information:\n"
               "\"The heater is estimated to be occupied in " +
               std::to_string(getTime()) +
               " minutes. So the waiting time will exceed the user defined threshold (" +
               std::to_string(threshold) +
               " minutes).\" \n"
               "Code repair information:\n"
               "\"A replacement instrument is needed for " +
               std::string(user_input_[LibHeat::Temperature]) +
               "°C static incubation. Please select the replacement that best preserves the "
               "original experimental conditions.\""
               "\n";
    }

    std::vector<ExecutionResult>
    phase0(Reality& reality, std::shared_ptr<MachineManager> mac_manager, ActionId action_id) {
        std::vector<ExecutionResult> results;
        auto machine = mac_manager->getMachine<LibraryModbusMachine>(machine_type_);

        auto duration = user_input_[LibHeat::Duration].get<uint32_t>();

        auto temperature = user_input_[LibHeat::Temperature].get<uint16_t>();

        machine->heater(duration, temperature, action_id);

        results.push_back(ExecutionResult());
        return results;
    }

    std::vector<ExecutionResult>
    phase1(Reality& reality, std::shared_ptr<MachineManager> mac_manager, ActionId action_id) {
        std::vector<ExecutionResult> results;

        results.push_back(ExecutionResult{next_steps_[LibHeat::kOutput], Variables(), false});
        return results;
    }

    static std::shared_ptr<LibHeatStep> fromDummy(DummyStep& dummy_step) {
        if (dummy_step.getType() == Dummy::DummyType::HEATER) {
            return std::make_shared<LibHeatStep>(LibHeat::Name, LibHeat::fromDummy(dummy_step));
        }
        return nullptr;
    }
};

class LibHeatStage : public TemplatedStage<LibHeat> {
public:
    LibHeatStage(std::string id, Variables&& input) : TemplatedStage(std::move(id), true) {
        my_step_ = std::make_shared<LibHeatStep>(name_, std::move(input));
    }

    void generateWorkflowHelper(Workflow& workflow) {
        if (next_stages_.size() != 1) {
            return;
        }
        genWorkflowForNextStage(workflow, LibHeat::kOutput);
    }
};