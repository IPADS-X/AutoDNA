#pragma once

#include "machine/check_manager.hpp"
#include "procedure/stage.hpp"
#include "procedure/workflow.hpp"
#include "reality/reality.hpp"

#include "common/logging.h"

class ProductionLineScheduler {
public:
    ProductionLineScheduler(std::shared_ptr<MachineManager>             mac_manager,
                            ThreadSafeQueue<std::shared_ptr<WebEvent>>& web_recv_queue,
                            ThreadSafeQueue<std::shared_ptr<WebEvent>>& web_send_queue,
                            std::vector<StepId>                         step_id_skip_until = {})
        : mac_manager_(mac_manager),
          web_recv_queue_(web_recv_queue),
          web_send_queue_(web_send_queue),
          step_id_skip_until_(step_id_skip_until) {
        while (mac_manager_->getEventQueue().try_pop()) {
            std::cout << "Clearing event queue" << std::endl;
        };

        reality_.init(mac_manager_);
    }

    ActionId getNewActionId() { return action_id_++; }

    WorkflowId getNewWorkflowId(bool is_user = true) {
        if (is_user) {
            return workflow_id_++;
        }
        return system_workflow_id_++;
    }

    bool isAllSystemWorkflowDone(std::shared_ptr<Action> action) {
        if (!action) {
            return true;
        }

        if (action_depends_system_workflow_.count(action->getId()) == 0) {
            return true;
        }

        auto depends_workflow_id = action_depends_system_workflow_[action->getId()];

        if (depends_workflow_id >= system_workflow_id_base_ &&
            !workflows_[depends_workflow_id]->isFinished()) {
            return false;
        }
        return true;
    }

    int extractLastNumber(const std::string& str) {
        size_t last_digit_pos = str.find_last_of("0123456789");
        if (last_digit_pos == std::string::npos) {
            return 1;
        }
        size_t first_nondigit_pos = str.find_last_not_of("0123456789", last_digit_pos);
        size_t start_pos = (first_nondigit_pos == std::string::npos) ? 0 : first_nondigit_pos + 1;

        logger->debug("Extracted number from string: {} -> {}", str,
                      str.substr(start_pos, last_digit_pos - start_pos + 1));

        return std::stoi(str.substr(start_pos, last_digit_pos - start_pos + 1));
    }

    void generateWorkflow(std::shared_ptr<Stage> stage, std::string data = "") {
        auto new_workflow_id = getNewWorkflowId();
#ifdef JUMP_ORIGIN_STEP
        auto jump_until_from_data = 1;
        if (!data.empty()) {
            // parse end number
            std::istringstream iss(data);
            // just get the end number
            jump_until_from_data = extractLastNumber(data);
        }

        if (step_id_skip_until_.size() < new_workflow_id) {
            step_id_skip_until_.resize(new_workflow_id);
            step_id_skip_until_[new_workflow_id - 1] = jump_until_from_data;
        }

        auto jump_index = 1;
        if (step_id_skip_until_.size() > new_workflow_id - 1) {
            jump_index = step_id_skip_until_[new_workflow_id - 1];
        }
        logger->info("Jumping to stage {} in workflow {}", jump_index, new_workflow_id);
        while (stage->getNextStages().size() > 0 && jump_index > 1) {
            stage = stage->getNextStages()[0];
            jump_index--;
        }
#endif

        std::shared_ptr<Workflow> workflow = std::make_shared<Workflow>(new_workflow_id);
#ifdef JUMP_ORIGIN_STEP
        workflow->setBaseStepId(std::stoi(stage->getId()));
#endif
        stage->generateWorkflow(*workflow);
        auto action = activateStep(stage->getMyStep(), workflow);
        workflow->setInitialAction(action);

        workflows_[new_workflow_id] = workflow;
    }

    void addWorkflow(std::shared_ptr<Workflow> workflow, bool is_user = true) {
        auto new_workflow_id = getNewWorkflowId(is_user);
        workflow->setId(new_workflow_id);

        auto action = activateStep(workflow->getSteps()[0], workflow);
        workflow->setInitialAction(action);
        workflows_[new_workflow_id] = workflow;
    }

    void start();

    bool canExecute(std::shared_ptr<Action> action) {
        return CheckManager::check(
            reality_, mac_manager_, action, [&](std::shared_ptr<Workflow> new_workflow) {
                return this->addWorkflowAndReOrder(action, new_workflow, false);
            });
    }

    void lockEquipment(std::shared_ptr<Action> action) {
        CheckManager::apply(reality_, mac_manager_, action,
                            [&](std::shared_ptr<Workflow> new_workflow) {
                                return this->addWorkflowAndReOrder(action, new_workflow, false);
                            });
    }

    void unlockEquipment(std::shared_ptr<Action> action) {
        CheckManager::release(reality_, mac_manager_, action,
                              [&](std::shared_ptr<Workflow> new_workflow) {
                                  return this->addWorkflowAndReOrder(action, new_workflow, false);
                              });
    }

    bool callCodeAgent(std::shared_ptr<Action> action, std::string message) {
        // Call the Code agent with the callback and action

        logger->info("Calling Code agent for action {}: {}", action->getId(), message);

        auto workflow_name = workflows_.at(action->getWorkflowId())->getName();

        auto original_times = workflows_.at(action->getWorkflowId())->getOriginalTimes();

        auto send_json = nlohmann::json{
            {"type", "call_code_agent"},
            {"action_id", action->getId()},
            {"workflow_id", action->getWorkflowId()},
            {"workflow_name", workflow_name},
            {"step_id", action->getStepId()},
            {"current_phase", action->getCurrentPhase()},
            {"message", message},
            {"times", original_times},
        };

        web_send_queue_.push(std::make_shared<WebEvent>(send_json.dump()));

        return true;
    }

    bool waitChangeCode(std::shared_ptr<Action> action);

    bool handleEvent(std::shared_ptr<MyEvent> event) {
        ActionId action_id = event->action_id;
        SPDLOG_ASSERT(actions_waiting_ack_.count(action_id) != 0, "Action not found: {}",
                      action_id);

        auto action   = actions_waiting_ack_.at(action_id);
        auto workflow = workflows_.at(action->getWorkflowId());

        // TODO: co_await?
        if (waitChangeCode(action)) {
            logger->info("Action {} need to change code", action_id);
            return true; // need to change code, do not execute now
        }

        if (!canExecute(action)) {
            logger->debug("Action {} can not execute now", action_id);
            return false;
        }

        // execute now!
        logger->debug("Action {} in phase {} can execute, and prepare to lock equipment", action_id,
                      action->getCurrentPhase());
        lockEquipment(action);
        action->receiveEvent(event);

        if (action->finishWaiting()) {
            actions_waiting_ack_.erase(action_id);
            execute(action, workflow);
            unlockEquipment(action);
            return true;
        }
        unlockEquipment(action);

        return false;
    }

    bool handleReadyAction(std::shared_ptr<Action> action, std::shared_ptr<Workflow> workflow) {
        auto action_id = action->getId();
        // TODO: co_await?
        if (waitChangeCode(action)) {
            logger->debug("Action {} need to change code", action_id);
            return true; // need to change code, do not execute now
        }

        if (!canExecute(action)) {
            logger->debug("Action {} can not execute now", action_id);
            return false;
        }

        // execute now
        logger->debug("Action {} in phase {} can execute, and prepare to lock equipment", action_id,
                      action->getCurrentPhase());
        lockEquipment(action);
        execute(action, workflow);
        unlockEquipment(action);

        return true;
    }

    void execute(std::shared_ptr<Action> action, std::shared_ptr<Workflow> workflow) {
        auto step = action->getStep();

        std::vector<ExecutionResult> results;
        auto                         step_id_skip_until_current_workflow = 1;
        if (step_id_skip_until_.size() > step->getWorkflowId() - 1) {
#ifndef JUMP_ORIGIN_STEP
            step_id_skip_until_current_workflow = step_id_skip_until_[step->getWorkflowId() - 1];
#endif
        }
        if (step->getId() >= step_id_skip_until_current_workflow) {
            // loop until return wait or results
            while (true) {
                results = step->execute(action, reality_, mac_manager_);
                if (results.empty() || !results[0].skip_to_next_phase) {
                    break;
                }
                action->nextPhase(0);
            }
        } else {
            SPDLOG_ASSERT(step->next_steps_.size() == 1, "Step{} has multiple next steps",
                          step->getId());
            auto next_step = step->next_steps_[0];
            results.push_back(ExecutionResult{next_step, Variables(), false});
        }

        int num_waiting = 0;
        for (const auto& result : results) {
            if (result.async) {
                num_waiting++;
            } else {
                auto next_step = result.next_step;
                if (next_step == nullptr) {
                    continue;
                }
                auto new_action = activateStep(next_step, workflow, result.output);
                action->addNext(new_action);
            }
        }
        action->setResults(results);

        if (num_waiting != 0) {
            action->nextPhase(num_waiting);
            actions_waiting_ack_[action->getId()] = action;
        } else {
            action->done();
            workflow->actionDone();
        }

        if (workflow->isFinished()) {
            // TODO: NOTIFY finished?
            workflow->printResults();
        }
    }

    std::shared_ptr<Action> activateStep(std::shared_ptr<Step>     step,
                                         std::shared_ptr<Workflow> workflow,
                                         Variables                 step_input = Variables()) {
        step->setStepInput(step_input);

        auto new_action_id = getNewActionId();
        auto new_action    = std::make_shared<Action>(new_action_id, step, workflow->getId());
        ready_actions_.push(new_action);

        workflow->addAction(new_action_id, new_action);
        return new_action;
    }

    void stop() { running_ = false; }

    bool addWorkflowAndReOrder(std::shared_ptr<Action> action, std::shared_ptr<Workflow> workflow,
                               bool is_user = true) {
        if (!is_user && !isAllSystemWorkflowDone(action)) {
            return false;
        }

        if (!workflow) {
            // just test if can submit
            return true;
        }

        addWorkflow(workflow, is_user);
        workflow->getSteps()[0]->genWorkflowForNextStep(*workflow, 1);
        if (!is_user && action) {
            action_depends_system_workflow_[action->getId()] = workflow->getId();
        }
        return true;
    }

    static void initLogger(spdlog::level::level_enum lvl, spdlog::sinks_init_list sinks) {
        logger = std::make_shared<spdlog::logger>("Scheduler", sinks);
        logger->set_level(lvl);
    }

private:
    bool running_ = true;

    ActionId   action_id_   = 1;
    WorkflowId workflow_id_ = 1;

    const WorkflowId    system_workflow_id_base_ = 1000;
    WorkflowId          system_workflow_id_      = system_workflow_id_base_;
    std::vector<StepId> step_id_skip_until_;

    std::shared_ptr<MachineManager>            mac_manager_;
    ThreadSafeQueue<std::shared_ptr<WebEvent>>&web_recv_queue_, &web_send_queue_;

    Reality reality_;

    std::unordered_map<WorkflowId, std::shared_ptr<Workflow>> workflows_;
    std::unordered_map<ActionId, WorkflowId>                  action_depends_system_workflow_;
    std::queue<std::shared_ptr<Action>>                       ready_actions_;
    std::unordered_map<ActionId, std::shared_ptr<Action>>     actions_waiting_ack_;

    std::map<std::string, std::pair<std::shared_ptr<Action>, std::shared_ptr<Workflow>>>
        paused_workflows_;

    std::mutex TOCTTOU_mutex_;

    static std::shared_ptr<spdlog::logger> logger;
};