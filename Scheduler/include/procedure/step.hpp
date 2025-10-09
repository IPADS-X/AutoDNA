#pragma once

#include <iostream>
#include <string>

#include "common.hpp"
// #include "machine/check_manager.hpp"
#include "machine/mac_manager.hpp"
#include "reality/reality.hpp"

#include "common/logging.h"

using StepFunc = std::function<std::vector<ExecutionResult>(
    Reality&, std::shared_ptr<MachineManager>, ActionId)>;

class Action;
class Workflow;

class Step {
public:
    const inline static std::string Tube = "Tube";
    Step(MachineType machine_type, std::string name, Variables&& user_input = Variables())
        : machine_type_(machine_type), name_(name), user_input_(std::move(user_input)) {}

    virtual ~Step() = default;

    void setId(StepId id) {
        id_ = id;
        // logger->info("Step{} {} created", id, name_);
    }

    void       setWorkflowId(WorkflowId workflow_id) { workflow_id_ = workflow_id; }
    WorkflowId getWorkflowId() const { return workflow_id_; }
    StepId     getId() const { return id_; }
    TubeId     getTubeId() const {
        if (user_input_.contains(Tube)) {
            return user_input_[Tube].get<TubeId>();
        }
        return TubeId(-1);
    }

    virtual std::vector<std::shared_ptr<Step>>
    getTubeTypeSupportsSteps(Reality& reality, std::map<TubeId, TubeId>& tube_map) {
        return {};
    }

    virtual std::shared_ptr<Step> buildNewStep(Variables&& new_params) { return nullptr; }

    MachineTypeId      getMachineType() const { return (MachineTypeId)machine_type_; }
    const std::string& getName() const { return name_; }
    const std::string  getBlockMessage(int threshold = 10) {
        return "Block because of step: " + name_ + ", params: " + user_input_.dump();
    }
    void setStepInput(Variables step_input) { step_input_ = std::move(step_input); }

    auto getParams() const { return user_input_; }

    void setParams(Variables&& new_params) { user_input_ = std::move(new_params); }

    std::vector<TubeId> getUsedTubeIds();

    std::shared_ptr<Step> setNextStep(int index, std::shared_ptr<Step> next_step) {
        next_steps_[index] = std::move(next_step);
        return next_steps_[index];
    }

    void genWorkflowForNextStep(Workflow& workflow, int index);

    std::vector<ExecutionResult> execute(std::shared_ptr<Action> action, Reality& reality,
                                         std::shared_ptr<MachineManager> mac_manager);

    static void initLogger(spdlog::level::level_enum lvl, spdlog::sinks_init_list sinks) {
        logger = std::make_shared<spdlog::logger>("Step", sinks);
        logger->set_level(lvl);
    }

    bool operator==(const Step& other) const { return machine_type_ == other.machine_type_; }

    std::string getInfo() const {
        std::string info = "Name: " + name_ + ", User Input: " + user_input_.dump();
        return info;
    }

    virtual bool setCurrentNum(uint16_t current_num) { return true; }

    virtual uint16_t getCurrentNum() const { return 1; }

    virtual long long getTime() const { return 0; }

    virtual std::vector<std::pair<MachineType, EquipmentType>>
    getNeedLockEquipment(Reality& reality, std::shared_ptr<MachineManager> mac_manager_,
                         std::shared_ptr<Action> action) const {
        return {};
    }

    virtual std::vector<std::pair<MachineType, EquipmentType>>
    getNeedUnlockEquipment(Reality& reality, std::shared_ptr<MachineManager> mac_manager_,
                           std::shared_ptr<Action> action) const {
        return {};
    }

    virtual long long getWorstAboutWaitTime() {
        long long wait_time = 0;
        // try next 2 steps
        auto now_step = this;
        for (int i = 0; i < 2; i++) {
            if (now_step) {
                wait_time += now_step->getTime();
            }
            if (now_step->next_steps_.empty()) {
                break;
            }
            now_step = now_step->next_steps_.begin()->second.get();
        }
        return wait_time;
    }

    int getPhaseTotalNum() const { return step_funcs_.size(); }

protected:
    StepId                               id_;
    WorkflowId                           workflow_id_;
    MachineType                          machine_type_;
    std::string                          name_;
    Variables                            user_input_;
    Variables                            step_input_;
    std::map<int, std::shared_ptr<Step>> next_steps_;
    std::vector<StepFunc>                step_funcs_;

    static std::shared_ptr<spdlog::logger> logger;

    friend class ProductionLineScheduler;
};

template <typename Derived>
class CRTPStep : public Step {
public:
    std::shared_ptr<CRTPStep> clone(Variables&& new_params) const {
        const Derived& self = static_cast<const Derived&>(*this);
        return std::make_shared<Derived>(this->name_, std::move(new_params));
    }

    std::vector<std::shared_ptr<Step>>
    getTubeTypeSupportsSteps(Reality& reality, std::map<TubeId, TubeId>& tube_map) override {
        auto new_params = user_input_;
        return {this->clone(std::move(new_params))};
    }

    std::shared_ptr<Step> buildNewStep(Variables&& new_params) override {
        return this->clone(std::move(new_params));
    }

    CRTPStep(MachineType machine_type, std::string name, Variables&& user_input)
        : Step(machine_type, std::move(name), std::move(user_input)) {}
};
