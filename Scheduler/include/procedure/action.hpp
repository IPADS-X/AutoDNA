#pragma once

#include <memory>
#include <string>
#include <vector>

#include "common.hpp"
#include "common/logging.h"
#include "procedure/common.hpp"
// #include "procedure/step.hpp"
// #include "procedure/workflow.hpp"

class Step;
class Workflow;

// A DAG (Directed Acyclic Graph) of actions executed in a workflow
class Action {
public:
    Action(ActionId action_id, std::shared_ptr<Step> my_step, WorkflowId workflow_id)
        : action_id_(action_id), my_step_(my_step), workflow_id_(workflow_id), finished(false) {}

    void addNext(std::shared_ptr<Action> next) { next_.push_back(std::move(next)); }

    ActionId              getId() const { return action_id_; }
    WorkflowId            getWorkflowId() const { return workflow_id_; }
    StepId                getStepId() const;
    std::shared_ptr<Step> getStep() const { return my_step_; }
    void                  setStep(std::shared_ptr<Step> step) { my_step_ = step; }
    int                   getCurrentPhase() const { return cur_phase_; }
    int                   getPhaseTotalNum() const;

    void setStepTransfered() { step_transfered_ = true; }
    bool isStepTransfered() const { return step_transfered_; }

    void done() {
        finished = true;
        logger->info("Action {} done with results: {}", action_id_, Variables{results_map_});
    }

    bool isDone() const { return finished; }

    void nextPhase(int num_waiting) {
        num_waiting_ = num_waiting;
        cur_phase_ += 1;
        received_events_.clear();
    }

    void receiveEvent(std::shared_ptr<MyEvent> event) {
        received_events_.push_back(std::move(event));
    }

    bool finishWaiting() const { return num_waiting_ == received_events_.size(); }

    std::vector<std::shared_ptr<Action>> getNext() const { return next_; }

    void setResults(std::vector<ExecutionResult> execution_results) {
        SPDLOG_ASSERT(results_map_.count(cur_phase_) == 0, "Phase {} already set", cur_phase_);
        results_map_[cur_phase_] = execution_results;
    }

    const std::map<int, std::vector<ExecutionResult>>& getResultsMap() const {
        return results_map_;
    }

    std::string getResultsString() const {
        std::string res = "";
        for (const auto& [phase, results] : results_map_) {
            for (const auto& result : results) {
                if (result.output.is_null()) {
                    continue;
                }
                res += result.output.dump() + "\n";
            }
        }
        return res;
    }

    static void initLogger(spdlog::level::level_enum lvl, spdlog::sinks_init_list sinks) {
        logger = std::make_shared<spdlog::logger>("Action", sinks);
        logger->set_level(lvl);
    }

private:
    int                   num_waiting_ = 0;
    int                   cur_phase_   = 0;
    WorkflowId            workflow_id_;
    ActionId              action_id_;
    std::shared_ptr<Step> my_step_;
    bool                  step_transfered_ = false; // whether the step has been transfered to other machine for execution
    std::vector<std::shared_ptr<MyEvent>>       received_events_;
    std::map<int, std::vector<ExecutionResult>> results_map_;
    bool                                        finished;
    // std::vector<ExecutionResult>          execution_results_;
    // std::vector<ExecutionResult>          event_results_;
    std::vector<std::shared_ptr<Action>> next_;

    static std::shared_ptr<spdlog::logger> logger;
};