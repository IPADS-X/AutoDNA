#pragma once

#include "container/container.hpp"

#include <memory>
#include <string>

class Reagent {
public:
    Reagent(std::string name) : name_(name) {}

    std::string getName() const { return name_; }

    void setContainer(std::shared_ptr<Container> container);

    std::shared_ptr<Container> getContainer() const { return container_; }

private:
    std::shared_ptr<Container> container_ = nullptr;

    std::string name_;

    int volume_;
};
