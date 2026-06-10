#pragma once

#include <algorithm>
#include <memory>
#include <stack>
#include <string>
#include <unordered_map>
#include <vector>

#include "procedure/workflow.hpp"

class Stage {
public:
    Stage(std::string id, std::string name, bool require_portage = false)
        : id_(std::move(id)), name_(std::move(name)), require_portage_(require_portage) {}

    virtual ~Stage() {
        // Iteratively sever the next_stages_ chain before releasing shared_ptrs.
        // Without this, destroying a 37k+ step linear chain would recurse 37k levels
        // through ~unordered_map -> ~shared_ptr -> ~Stage and overflow the stack.
        std::stack<std::shared_ptr<Stage>> stk;
        for (auto& [k, next] : next_stages_) stk.push(std::move(next));
        next_stages_.clear();
        while (!stk.empty()) {
            auto s = std::move(stk.top());
            stk.pop();
            if (!s || s.use_count() != 1) continue;
            for (auto& [k, next] : s->next_stages_) stk.push(std::move(next));
            s->next_stages_.clear();
            // s released here; its ~Stage() finds next_stages_ empty — no further recursion.
        }
    }

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
        // Iterative DFS — avoids stack overflow on deep stage chains (e.g. 37k+ steps).
        std::stack<Stage*> stk;
        stk.push(this);
        while (!stk.empty()) {
            Stage* cur = stk.top();
            stk.pop();
            if (cur->iterated_) continue;
            cur->iterated_ = true;
            if (cur->my_step_ == nullptr)
                throw std::runtime_error("my_step_ is not set");
            workflow.addStep(cur->my_step_);
            // Collect next-stage keys and sort descending so that after pushing
            // onto the stack the lowest index (kTrueOutput=0) is processed first.
            std::vector<int> keys;
            keys.reserve(cur->next_stages_.size());
            for (auto& [k, _] : cur->next_stages_) keys.push_back(k);
            std::sort(keys.rbegin(), keys.rend());
            for (int k : keys) {
                auto& next = cur->next_stages_[k];
                cur->my_step_->setNextStep(k, next->my_step_);
                stk.push(next.get());
            }
        }
    }

    virtual void generateWorkflowHelper(Workflow& workflow) {}

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