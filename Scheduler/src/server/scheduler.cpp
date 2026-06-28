#include "server/scheduler.hpp"
#include "transform/transfer_manager.hpp"

#include "process/library/heater.hpp"

// When more than WORKFLOW_BATCH_THRESHOLD workflows are dispatched at once,
// they are split into independent batches of WORKFLOW_BATCH_SIZE, each run
// through the full transform pipeline and dispatched separately.
// Override at build time with -DWORKFLOW_BATCH_THRESHOLD=.. / -DWORKFLOW_BATCH_SIZE=..
#ifndef WORKFLOW_BATCH_THRESHOLD
#define WORKFLOW_BATCH_THRESHOLD 50
#endif
#ifndef WORKFLOW_BATCH_SIZE
#define WORKFLOW_BATCH_SIZE 4
#endif

std::shared_ptr<spdlog::logger> ProductionLineScheduler::logger = nullptr;

bool ProductionLineScheduler::waitChangeCode(std::shared_ptr<Action> action) {
    if (action->getCurrentPhase() != 0) {
        return false;
    }
    // here check if can execute
    // if can not execute, called PDA to change code
    auto machine_type = action->getStep()->getMachineType();
    auto machine      = mac_manager_->getMachine<Machine>(machine_type);

    long long   worst_time = 0;
    std::string message;
    for (const auto& req :
         action->getStep()->getNeedLockEquipment(reality_, mac_manager_, action)) {
        auto target_machine = mac_manager_->getMachine<Machine>(req.first);
        auto alloc_step     = target_machine->getEquipmentStep(req.second);

        for (const auto& step : alloc_step) {
            auto new_worst_time = step->getWorstAboutWaitTime();
            if (new_worst_time > worst_time) {
                worst_time = new_worst_time;
                message    = step->getBlockMessage(10);
            }
        }
    }

    if (worst_time < 20 * 60) {
        return false;
    }

    // more than 10 mins

    // register a handler when generate new code
    // remove the workflow and all of the action/stage/step from scheduler
    // resume code executing

    paused_workflows_[workflows_.at(action->getWorkflowId())->getName()] =
        std::make_pair(action, workflows_.at(action->getWorkflowId()));

    // called PDA
    auto succ = callCodeAgent(action, message);

    return succ;
}

bool ProductionLineScheduler::waitConfirmChangeEquipment(std::shared_ptr<Action> action) {
    // here check if can execute
    // if can not execute, called PDA to change code

    auto message =
        "Waiting for equipment change confirmation for step " + std::to_string(action->getStepId());

    message += ", step change to: " + action->getStep()->getName();

    paused_workflows_[workflows_.at(action->getWorkflowId())->getName()] =
        std::make_pair(action, workflows_.at(action->getWorkflowId()));

    // called PDA
    auto succ = callCodeAgent(action, message);

    return true;
}

void ProductionLineScheduler::start() {
    int times = 1;

    logger->info("=====================Start===========================");
    while (running_) {
        auto web_event = web_recv_queue_.try_pop();

        if (web_event && nlohmann::json::accept(web_event->get()->data)) {
            // Extract reagent name and volume from the event data
            nlohmann::json json_data = nlohmann::json::parse(web_event->get()->data);
            std::string    command   = json_data["command"];
            if (command == "renew_reagent") {
                std::string reagent_name = json_data["reagent_name"];
                int         volume       = json_data["volume"];
                if (reality_.renewReagents(reagent_name, volume)) {
                    logger->info("Renewed reagent: {} with volume: {}", reagent_name, volume);
                } else {
                    logger->warn("Failed to renew reagent: {}", reagent_name);
                }
            } else if (command == "renew_consumer") {
                std::string   carrier_name = json_data["carrier_name"];
                MachineTypeId machine_type =
                    static_cast<MachineTypeId>(json_data["machine_type"].get<int>());
                AreaId area_id = static_cast<AreaId>(json_data["area_id"].get<int>());
                if (reality_.renewConsumer(carrier_name, machine_type, area_id, mac_manager_)) {
                    logger->info("Renewed consumer carrier: {} to machine: {}", carrier_name,
                                 magic_enum::enum_name((MachineType)machine_type));
                } else {
                    logger->warn("Failed to renew consumer carrier: {}", carrier_name);
                }
            } else if (command == "renew_pipettes") {
                MachineTypeId machine_type =
                    static_cast<MachineTypeId>(json_data["machine_type"].get<int>());
                if (reality_.renewPipettes(machine_type, mac_manager_)) {
                    logger->info("Renewed pipettes for machine type: {}",
                                 magic_enum::enum_name((MachineType)machine_type));
                } else {
                    logger->warn("Failed to renew pipettes for machine type: {}",
                                 magic_enum::enum_name((MachineType)machine_type));
                }
            } else {
                std::vector<std::string> workflow_names = {"default"};
                if (json_data.contains("workflow_names")) {
                    workflow_names = json_data["workflow_names"].get<std::vector<std::string>>();
                } else if (json_data.contains("workflow_name")) {
                    workflow_names = {json_data["workflow_name"]};
                }

                int jump_from = 1;
                if (json_data.contains("jump_from")) {
                    jump_from = json_data["jump_from"];
                } else {
                    jump_from = 1;
                }

                // Because after block, the workflow will merged and use the first workflow's name,
                // so here we just check the first workflow's name in the list to decide whether to
                // jump from a paused workflow.
                if (paused_workflows_.count(workflow_names[0]) > 0) {
                    // resume from stop step
                    jump_from = paused_workflows_[workflow_names[0]].first->getStep()->getId();
                }

                int exec_times = 1;
                if (json_data.contains("times")) {
                    exec_times = json_data["times"];
                } else {
                    exec_times = 1;
                }

                std::vector<std::string> all_workflow_names;
                for (int i = 0; i < exec_times; ++i) {
                    all_workflow_names.insert(all_workflow_names.end(), workflow_names.begin(),
                                              workflow_names.end());
                }

                bool is_prealloc = false;
                if (json_data.contains("prealloc")) {
                    is_prealloc = json_data["prealloc"];
                    logger->info("Workflows are prealloc: {}", is_prealloc);
                }

                if (json_data.contains("uninterruptible")) {
                    is_prealloc = json_data["uninterruptible"];
                    logger->info("Workflows are prealloc: {}", is_prealloc);
                }

                // When too many workflows are dispatched at once, intercept here and
                // dispatch them in independent batches: each batch runs through the
                // full parse/merge/alloc/interval pipeline and is added separately.
                // e.g. 78 workflows -> 20 batches of 4 -> 20 separate dispatches.
                const size_t kBatchThreshold = WORKFLOW_BATCH_THRESHOLD;
                const size_t kBatchSize      = WORKFLOW_BATCH_SIZE;

                std::vector<std::vector<std::string>> batches;
                if (all_workflow_names.size() > kBatchThreshold) {
                    for (size_t start = 0; start < all_workflow_names.size(); start += kBatchSize) {
                        size_t end = start + kBatchSize;
                        if (end > all_workflow_names.size()) {
                            end = all_workflow_names.size();
                        }
                        batches.emplace_back(all_workflow_names.begin() + start,
                                             all_workflow_names.begin() + end);
                    }
                    logger->info("Workflow count {} exceeds {}, dispatching in {} batches of {}",
                                 all_workflow_names.size(), kBatchThreshold, batches.size(),
                                 kBatchSize);
                } else {
                    batches.push_back(all_workflow_names);
                }

                for (const auto& batch : batches) {
                    pending_batches_.push({batch, jump_from, exec_times, is_prealloc});
                }
            }
        } else if (web_event) {
            // generateWorkflow(stage, web_event->get()->data);
            logger->debug("not accepted: {}", web_event->get()->data);
        }

        if (active_user_workflows_ == 0 && !pending_batches_.empty()) {
            auto pending = pending_batches_.front();
            pending_batches_.pop();
            auto workflows = TransferManager::parse_and_generate(
                reality_, mac_manager_, pending.batch, pending.jump_from, pending.is_prealloc);
            for (int i = 0; i < workflows.size(); ++i) {
                auto& workflow = workflows[i];
                logger->info("Generate a new workflow: {}", workflow->getName());
                workflow->setName(pending.batch[i]);
                workflow->setOriginalTimes(pending.exec_times);
                workflow->setPreAlloc(pending.is_prealloc);
                active_user_workflows_++;
                if (!addWorkflowAndReOrder(nullptr, workflow)) {
                    logger->info("Added workflow {} to scheduler failed, will be retried",
                                 workflow->getName());
                }
            }
        }

        if (failed_prealloc_workflows_.size() > 0) {
            auto  current_time  = std::chrono::duration_cast<std::chrono::milliseconds>(
                                      std::chrono::steady_clock::now().time_since_epoch())
                                      .count();
            auto& next_workflow = failed_prealloc_workflows_.front();
            if (next_workflow.first <= current_time) {
                logger->info("Retrying adding prealloc workflow {} at {} ms",
                             next_workflow.second->getName(), current_time);
                auto workflow = next_workflow.second;
                failed_prealloc_workflows_.pop();

                if (addWorkflowAndReOrder(nullptr, workflow, true, true)) {
                    logger->info("Retried adding prealloc workflow {} success",
                                 workflow->getName());
                }
            }
        }

        if (!ready_actions_.empty()) {
            logger->debug("There are {} ready actions", ready_actions_.size());
            auto action = ready_actions_.front();
            ready_actions_.pop();

            logger->debug("Activating action: {} for step: {}", action->getId(),
                          action->getStep()->getName());

            auto workflow = workflows_.at(action->getWorkflowId());
            if (!handleReadyAction(action, workflow)) {
                ready_actions_.push(action);
                logger->debug("Action {} is not ready, put it back to the queue", action->getId());
            } else {
                logger->debug("Action {} activated successfully", action->getId());
            }
        }

        std::this_thread::sleep_for(std::chrono::milliseconds(100));
        // std::this_thread::sleep_for(std::chrono::milliseconds(1000));
        auto event = mac_manager_->getEventQueue().try_pop();
        if (event) {
            auto success = handleEvent(*event);
            if (!success) {
                mac_manager_->getEventQueue().push(*event);
            }
        }

        times++;
        if (times % 100 == 0) {
            logger->info("Running for {} seconds", times / 10);
            times = 0;
        }
    }
}