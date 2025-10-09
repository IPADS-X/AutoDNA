#include "reality/reagent.hpp"

void Reagent::setContainer(std::shared_ptr<Container> container) {
    container_ = container;

    container_->setReagent(this);
}