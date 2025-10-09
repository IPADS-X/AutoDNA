#pragma once

#include <memory>
#include <string>
#include <unordered_map>

#include "procedure/workflow.hpp"

class Stage {
public:
    Stage(std::string id, std::string name, bool require_portage = false)
        : id_(std::move(id)), name_(std::move(name)), require_portage_(require_portage) {}

    const std::string& getId() const { return id_; }
    const std::string& getName() const { return name_; }
    bool               requirePortage() const { return require_portage_; }

    std::shared_ptr<Stage> setNextStage(int index, std::shared_ptr<Stage> next_stage) {
        next_stages_[index] = std::move(next_stage);
        return next_stages_[index];
    }

    std::shared_ptr<Step> getMyStep() { return my_step_; }

    std::vector<std::shared_ptr<Stage>> getNextStages() const {
        std::vector<std::shared_ptr<Stage>> stages;
        for (const auto& pair : next_stages_) {
            stages.push_back(pair.second);
        }
        return stages;
    }

    void generateWorkflow(Workflow& workflow) {
        if (iterated_) {
            return;
        }
        iterated_ = true;

        if (my_step_ == nullptr) {
            throw std::runtime_error("my_step_ is not set");
        }
        workflow.addStep(my_step_);
        generateWorkflowHelper(workflow);
    }

    virtual void generateWorkflowHelper(Workflow& workflow) = 0;

protected:
    std::string                                     id_;
    std::string                                     name_;
    std::unordered_map<int, std::shared_ptr<Stage>> next_stages_;
    std::shared_ptr<Step>                           my_step_;
    bool                                            iterated_        = false;
    bool                                            require_portage_ = false;

    void genWorkflowForNextStage(Workflow& workflow, int index);
};

template <typename T>
class TemplatedStage : public Stage {
protected:
    TemplatedStage(std::string id, bool require_portage)
        : Stage(std::move(id), T::Name, require_portage) {}
};

class Procedure {};