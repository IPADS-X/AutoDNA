#include "procedure/action.hpp"
#include "procedure/step.hpp"

std::shared_ptr<spdlog::logger> Action::logger = nullptr;

StepId Action::getStepId() const { return my_step_->getId(); }

int Action::getPhaseTotalNum() const { return my_step_->getPhaseTotalNum(); }
