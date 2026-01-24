#pragma once

#include <string>
#include <memory>

class Reagent;

class Container {
public:
    virtual void setReagent(Reagent* reagent) = 0;

    virtual bool forceSetReagentIndex(Reagent* reagent, size_t index) = 0;

    virtual bool setVolume(int volume, int index) = 0;

    virtual bool setAllReagentVolume(std::shared_ptr<Reagent> reagent, int volume) = 0;

    virtual int getVolume(int index) const = 0;
};