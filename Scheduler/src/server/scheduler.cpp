#include "server/scheduler.hpp"
#include "transform/transfer_manager.hpp"

#include "process/library/heater.hpp"

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
            } else {
                std::string workflow_name = "default";
                if (json_data.contains("workflow_name")) {
                    workflow_name = json_data["workflow_name"];
                }

                int jump_from = 1;
                if (json_data.contains("jump_from")) {
                    jump_from = json_data["jump_from"];
                } else {
                    jump_from = 1;
                }

                if (paused_workflows_.count(workflow_name) > 0) {
                    // resume from stop step
                    jump_from = paused_workflows_[workflow_name].first->getStep()->getId();
                }

                int exec_times = 1;
                if (json_data.contains("times")) {
                    exec_times = json_data["times"];
                } else {
                    exec_times = 1;
                }
                auto workflow = TransferManager::parse_and_generate(
                    reality_, mac_manager_, workflow_name, exec_times, jump_from);
                logger->info("Generate a new workflow");
                workflow[0]->setName(workflow_name);
                workflow[0]->setOriginalTimes(exec_times);
                addWorkflowAndReOrder(nullptr, workflow[0]);
            }
        } else if (web_event) {
            // generateWorkflow(stage, web_event->get()->data);
            logger->debug("not accepted: {}", web_event->get()->data);
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