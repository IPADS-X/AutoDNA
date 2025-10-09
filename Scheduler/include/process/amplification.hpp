#pragma once

#include "procedure/stage.hpp"

class Amplification {
public:
    enum { kOutput = 0 };
    inline static const std::string Name         = "Amplification";
    inline static const std::string Volume       = "Volume";
    inline static const std::string Volume2      = "Volume2";
    inline static const std::string PcrFile      = "PcrFile";
    inline static const std::string UsePipetting = "UsePipetting";
};

class AmplificationStep : public CRTPStep<AmplificationStep> {
public:
    AmplificationStep(std::string name, Variables user_input)
        : CRTPStep<AmplificationStep>(MachineType::AMPLIFICATION, name, std::move(user_input)) {
        step_funcs_ = {std::bind(&AmplificationStep::phase0, this, std::placeholders::_1,
                                 std::placeholders::_2, std::placeholders::_3),
                       std::bind(&AmplificationStep::phase1, this, std::placeholders::_1,
                                 std::placeholders::_2, std::placeholders::_3),
                       std::bind(&AmplificationStep::phase2, this, std::placeholders::_1,
                                 std::placeholders::_2, std::placeholders::_3)};
    }

    std::vector<ExecutionResult>
    phase0(Reality& reality, std::shared_ptr<MachineManager> mac_manager, ActionId action_id) {
        std::vector<ExecutionResult> results;

        auto machine = mac_manager->getMachine<AmplificationModbusMachine>(machine_type_);
        machine->amplification1(action_id);

        logger->debug("[Action {}]: amplification first try", action_id);

        results.push_back(ExecutionResult());
        return results;
    }

    std::vector<ExecutionResult>
    phase1(Reality& reality, std::shared_ptr<MachineManager> mac_manager, ActionId action_id) {
        std::vector<ExecutionResult> results;

        auto machine = mac_manager->getMachine<AmplificationModbusMachine>(machine_type_);

        auto carrier_sn = step_input_[Carrier::NAME].get<std::string>();
        auto carrier    = reality.getCarrier(carrier_sn);
        auto tubes      = carrier->getBitmap();

        logger->debug("[Action {}]: carrier_sn: {}, tubes: {}", action_id, carrier_sn,
                      Variables(tubes));

        uint16_t volume        = user_input_[Amplification::Volume].get<uint16_t>();
        uint16_t volume2       = user_input_[Amplification::Volume2].get<uint16_t>();
        uint16_t pcr_file      = user_input_[Amplification::PcrFile].get<uint16_t>();
        uint16_t use_pipetting = user_input_[Amplification::UsePipetting].get<uint16_t>();
        if (first_time_) {
            first_time_ = false;
        } else {
            use_pipetting = 0;
        }
        machine->amplification2(tubes, volume, volume2, pcr_file, use_pipetting, action_id);

        results.push_back(ExecutionResult());
        return results;
    }

    std::vector<ExecutionResult>
    phase2(Reality& reality, std::shared_ptr<MachineManager> mac_manager, ActionId action_id) {
        std::vector<ExecutionResult> results;

        auto        manager = std::dynamic_pointer_cast<MachineManager>(mac_manager);
        auto        machine = manager->getMachine<AmplificationModbusMachine>(machine_type_);
        std::string sn;
        std::vector<uint16_t> tubes;
        machine->onAmplificationFinished(sn, tubes);

        Variables             output = step_input_;
        std::shared_ptr<Step> next_step;

        auto iter = next_steps_.find(Amplification::kOutput);
        if (iter != next_steps_.end()) {
            next_step = iter->second;
        }
        results.push_back(ExecutionResult{next_step, std::move(output), false});
        return results;
    }

private:
    bool first_time_ = true;
};

class AmplificationStage : public TemplatedStage<Amplification> {
public:
    AmplificationStage(std::string id, Variables&& input) : TemplatedStage(std::move(id), true) {
        my_step_ = std::make_shared<AmplificationStep>(name_, std::move(input));
    }

    void generateWorkflowHelper(Workflow& workflow) {
        if (next_stages_.size() != 1) {
            return;
        }
        genWorkflowForNextStage(workflow, Amplification::kOutput);
    }
};