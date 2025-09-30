#include "procedure/stage.hpp"
#include "process/portage.hpp"

void Stage::genWorkflowForNextStage(Workflow& workflow, int index) {
    auto next_stage = next_stages_[index];
    auto next_step  = next_stage->getMyStep();
    next_stage->generateWorkflow(workflow);
    my_step_->setNextStep(index, next_step);
}