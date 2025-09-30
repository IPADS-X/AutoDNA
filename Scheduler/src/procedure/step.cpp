#include "procedure/step.hpp"
#include "procedure/action.hpp"
#include "procedure/workflow.hpp"
#include "process/dummy.hpp"

#include "process/purification/move_tube.hpp"
#include "process/purification/pipette.hpp"
#include "process/purification/shake.hpp"
#include "process/purification/timer.hpp"

#include "magic_enum.hpp"

// #define USER_CONTROL

// #define CONTINUE_UNTIL_ADD_REGRENT

std::shared_ptr<spdlog::logger> Step::logger = nullptr;

void Step::genWorkflowForNextStep(Workflow& workflow, int index) {
    // auto next_stage = next_stages_[index];
    // auto next_step  = next_stage->getMyStep();
    // next_stage->generateWorkflow(workflow);
    // my_step_->setNextStep(index, next_step);
    setNextStep(0, workflow.getSteps()[index]);
    if (index + 1 < workflow.getSteps().size()) {
        workflow.getSteps()[index]->genWorkflowForNextStep(workflow, index + 1);
    }
}

std::vector<TubeId> Step::getUsedTubeIds() {
    std::vector<TubeId> used_tube_ids;
    for (const auto& [key, value] : user_input_.items()) {
        if (key == Tube || key == PuriPipette::SrcTube || key == PuriPipette::DstTube) {
            used_tube_ids.push_back(value.get<TubeId>());
        }
    }
    return used_tube_ids;
}

std::vector<ExecutionResult> Step::execute(std::shared_ptr<Action> action, Reality& reality,
                                           std::shared_ptr<MachineManager> mac_manager) {
    auto current_phase = action->getCurrentPhase();
    auto action_id     = action->getId();
    logger->info(
        "Step{} {} of workflow {} with action_id {} enter phase{}, user_input {}, step_input {}",
        id_, name_, workflow_id_, action_id, current_phase, user_input_, step_input_);

    auto user_continue_func = [&](std::string msg) {
        std::string dummy;
        logger->warn("Next will {}, please input 'c' to continue", msg);
        while (std::getline(std::cin, dummy) && dummy != "c") {
            logger->warn("Please input 'c' to continue");
        }
    };

#ifdef USER_CONTROL

#ifdef CONTINUE_UNTIL_ADD_REGRENT
    if (current_phase == 0) {
        if (name_ == PuriPipette::Name) {
            PurificationArea startPos = user_input_[PuriPipette::StartPos].get<PurificationArea>();
            if (startPos == PurificationArea::REACTION_AREA_02 ||
                startPos == PurificationArea::EMPTY_TUBE_AREA ||
                startPos == PurificationArea::LOW_TEMP_AREA) {
                user_continue_func("add reagent");
            }
        } else if (name_ == PuriShake::Name) {
            user_continue_func("shake tube");
        } else if (name_ == PuriMoveTube::Name) {
            PurificationArea startPos = user_input_[PuriPipette::StartPos].get<PurificationArea>();
            if (startPos == PurificationArea::SHAKING_AREA) {
                user_continue_func("move tube from shaking area");
            }
        }
    }
#else
    std::string dummy;
    std::getline(std::cin, dummy);
#endif

#endif
    SPDLOG_ASSERT(!step_funcs_.empty(), "Step{} has no step_funcs", id_);
    return step_funcs_[current_phase](reality, mac_manager, action_id);
}