#pragma once

#include <functional>
#include <iostream>
#include <map>
#include <memory>
#include <queue>
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
                if (!action->getResultsString().empty()) {
                    std::cout << "Results: " << action->getResultsString() << std::endl;
                }
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

    void printResults() {
        std::cout << "Workflow " << id_ << std::endl;
        for (const auto& step : steps_) {
            std::cout << "Step " << step.first << ": " << step.second->getName() << std::endl;
        }
        displayActions();
    }

private:
    WorkflowId  id_;
    std::string name_;
    uint        original_times_      = 1;
    uint        step_id_             = 1;
    uint        num_ongoing_actions_ = 0;

    std::shared_ptr<Action>                     initial_action_;
    std::map<uint, std::shared_ptr<Step>>       steps_;
    std::map<ActionId, std::shared_ptr<Action>> actions_;
};