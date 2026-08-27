#pragma once

#include <chrono>
#include <functional>
#include <iomanip>
#include <iostream>
#include <map>
#include <memory>
#include <queue>
#include <sstream>
#include <stdexcept>
#include <string>
#include <vector>

#include "action.hpp"
#include "procedure/action.hpp"
#include "step.hpp"

class Stage;
class ProductionLineScheduler;
class Action;

class Workflow {
public:
    Workflow(WorkflowId id) : id_(id) {}

    void setName(const std::string& name) { name_ = name; }

    const std::string& getName() const { return name_; }

    void setOriginalTimes(uint times) { original_times_ = times; }

    const uint getOriginalTimes() const { return original_times_; }

    static uint64_t nowMs() {
        return std::chrono::duration_cast<std::chrono::milliseconds>(
                   std::chrono::steady_clock::now().time_since_epoch())
            .count();
    }

    // Wall-clock timing of the whole workflow. Nothing is ever subtracted, so the
    // reported duration also covers the time the workflow sat interrupted (paused
    // while the code agent regenerated it): a resumed run inherits the start time
    // of the run it continues.
    void markStarted() {
        if (start_time_ms_ == 0) {
            start_time_ms_ = nowMs();
        }
    }

    void setStartTimeMs(uint64_t start_time_ms) { start_time_ms_ = start_time_ms; }

    uint64_t getStartTimeMs() const { return start_time_ms_; }

    void markFinished() {
        if (end_time_ms_ == 0) {
            end_time_ms_ = nowMs();
        }
    }

    uint64_t getDurationMs() const {
        if (start_time_ms_ == 0) {
            return 0;
        }
        return (end_time_ms_ == 0 ? nowMs() : end_time_ms_) - start_time_ms_;
    }

    static std::string formatDuration(uint64_t duration_ms) {
        uint64_t total_seconds = duration_ms / 1000;
        uint64_t hours         = total_seconds / 3600;
        uint64_t minutes       = (total_seconds % 3600) / 60;
        uint64_t seconds       = total_seconds % 60;

        std::ostringstream oss;
        if (hours > 0) {
            oss << hours << "h ";
        }
        if (hours > 0 || minutes > 0) {
            oss << minutes << "m ";
        }
        oss << seconds << "." << std::setw(3) << std::setfill('0') << duration_ms % 1000 << "s";
        return oss.str();
    }

    std::string getDurationString() const { return formatDuration(getDurationMs()); }

    void addStep(std::shared_ptr<Step> step) {
        StepId id = getNewStepId();
        step->setId(id);
        step->setWorkflowId(id_);
        if (steps_.count(id) > 0) {
            throw std::runtime_error("Step ID must be unique: " + std::to_string(id));
        }
        
        steps_.emplace(id, std::move(step));
    }

    void setId(WorkflowId id) {
        id_ = id;

        for (const auto& step_pair : steps_) {
            step_pair.second->setWorkflowId(id);
        }
    }

    WorkflowId getId() const { return id_; }

    StepId getNewStepId() { return step_id_++; }

    void setBaseStepId(StepId base_step_id) { step_id_ = base_step_id; }

    bool isFinished() const { return num_ongoing_actions_ == 0; }

    std::vector<std::shared_ptr<Step>> getSteps() const {
        std::vector<std::shared_ptr<Step>> step_list;
        for (const auto& step_pair : steps_) {
            step_list.push_back(step_pair.second);
        }
        return step_list;
    }

    void displayActions() const {
        if (initial_action_ == nullptr) {
            return;
        }
        // Display all actions from initial_action_
        std::queue<std::shared_ptr<Action>> actions;
        actions.push(initial_action_);
        while (!actions.empty()) {
            auto action = actions.front();
            actions.pop();
            for (const auto& next : action->getNext()) {
                std::cout << action->getId() << "(" << steps_.at(action->getStepId())->getName()
                          << ")"
                          << " -> " << next->getId() << "("
                          << steps_.at(next->getStepId())->getName() << ")" << std::endl;
                actions.push(next);
            }
        }
    }

    void setInitialAction(std::shared_ptr<Action> initial_action) {
        initial_action_ = initial_action;
    }

    void actionDone() { num_ongoing_actions_ -= 1; }

    void addAction(ActionId new_action_id, std::shared_ptr<Action> new_action) {
        actions_.emplace(new_action_id, new_action);
        num_ongoing_actions_ += 1;
    }

    // Collect every non-null output of every action of this workflow.
    // Walks actions_ instead of the action DAG, so mid-pipeline results (e.g. a
    // fluorescence read) and the results of the very last action are both kept,
    // and an action with several next actions is not reported more than once.
    Variables collectResults() const {
        Variables results = Variables::array();
        for (const auto& [action_id, action] : actions_) {
            for (const auto& [phase, phase_results] : action->getResultsMap()) {
                for (const auto& result : phase_results) {
                    if (result.output.is_null()) {
                        continue;
                    }
                    auto step = action->getStep();
                    results.push_back(Variables{
                        {"action_id", action_id},
                        {"step_id", action->getStepId()},
                        {"step_name", step ? step->getName() : std::string()},
                        {"phase", phase},
                        {"output", result.output},
                    });
                }
            }
        }
        return results;
    }

    void printResults() {
        std::cout << "Workflow " << id_ << std::endl;
        for (const auto& step : steps_) {
            std::cout << "Step " << step.first << ": " << step.second->getName() << std::endl;
        }
        displayActions();

        std::cout << "Total time of workflow " << id_ << ": " << getDurationString() << " ("
                  << getDurationMs() << " ms, interruptions included)" << std::endl;

        auto results = collectResults();
        std::cout << "Results of workflow " << id_ << " (" << results.size() << "):" << std::endl;
        for (const auto& result : results) {
            std::cout << "  " << result.dump() << std::endl;
        }
    }

    void setPreAlloc(bool is_pre_alloc) { is_pre_alloc_ = is_pre_alloc; }

    bool isPreAlloc() const { return is_pre_alloc_; }

private:
    WorkflowId  id_;
    std::string name_;
    uint        original_times_      = 1;
    uint        step_id_             = 1;
    uint        num_ongoing_actions_ = 0;

    uint64_t start_time_ms_ = 0;
    uint64_t end_time_ms_   = 0;

    bool is_pre_alloc_ = false;

    std::shared_ptr<Action>                     initial_action_;
    std::map<uint, std::shared_ptr<Step>>       steps_;
    std::map<ActionId, std::shared_ptr<Action>> actions_;
};