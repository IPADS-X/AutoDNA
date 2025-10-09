#pragma once

#include "carrier/strip_tube_carrier.hpp"
#include "container/tube.hpp"
#include "reagent.hpp"

#include "machine/common.hpp"

#include <map>
#include <string>

class MachineManager;

class Reality {
public:
    std::shared_ptr<Reagent> getOrCreateReagent(const std::string& name) {
        auto reagent = getReagent(name);
        if (!reagent) {
            reagent         = std::make_shared<Reagent>(name);
            reagents_[name] = reagent;
        }
        return reagent;
    }

    std::shared_ptr<Container> createTube(TubeLabel label, TubeType type,
                                          const std::string name = "") {
        return tube_manager_.allocTube(label, name, type);
    }

    template <typename CARRIER>
    std::shared_ptr<CARRIER> getOrCreateCarrier(const std::string& name) {
        auto it = carriers_.find(name);
        if (it != carriers_.end()) {
            return std::dynamic_pointer_cast<CARRIER>(it->second);
        } else {
            auto carrier    = std::make_shared<CARRIER>(name);
            carriers_[name] = carrier;
            return std::dynamic_pointer_cast<CARRIER>(carrier);
        }
    }

    std::shared_ptr<Reagent> getReagent(const std::string& name) {
        auto res = reagents_[name];
        if (res == nullptr && name.size() > 2 && name[0] == '\'' && name[name.size() - 1] == '\'') {
            return reagents_[name.substr(1, name.size() - 2)];
        } else {
            return res;
        }
    }

    std::shared_ptr<Carrier> getCarrier(const std::string& sn) { return carriers_[sn]; }

    std::shared_ptr<Container> getTube(TubeId id) { return tube_manager_.getTube(id); }

    std::vector<std::shared_ptr<Container>> getLabelTubes(const TubeLabel& label) {
        auto tubes = tube_manager_.getLabelTubes(label);
        if (tubes.size() > 0) {
            return tubes;
        }
        return {};
    }

    std::vector<std::shared_ptr<Container>> getTubes(const TubeLabel& label) {
        return tube_manager_.getLabelTubes(label);
    }

    TubePosition getTubePosition(TubeId id, TubePositionType type, IndexId index = 0) {
        return tube_manager_.getTubePosition(id, type, index);
    }

    TubePosition getTubeLastPosition(TubeId id, TubePositionType type, IndexId index = 0) {
        return tube_manager_.getTubeLastPosition(id, type, index);
    }

    TubePosition getTubeReactionPosition(TubeId id, TubePositionType type, IndexId index = 0) {
        return tube_manager_.getTubeReactionPosition(id, type, index);
    }

    void setTubePosition(TubeId id, TubePosition tube_position, TubePositionType type) {
        tube_manager_.setTubePosition(id, tube_position, type);
    }

    bool isAlloced(TubeId id) { return tube_manager_.isAlloced(id); }

    bool renewConsumer(std::string carrier_name, MachineTypeId machine_type, AreaId area_id,
                       std::shared_ptr<MachineManager> mac_manager);

    bool renewReagents(std::string reagent_name, int volume);

    bool addTransfer(TubeId origin_tube, TubeId new_id) {
        return tube_manager_.addTransfer(origin_tube, new_id);
    }

    bool markWorkflowTubes(std::string                                       workflow_name,
                           std::map<std::string, std::pair<TubeId, IndexId>> tmp_tubes_relation_) {
        return tube_manager_.markWorkflowTubes(workflow_name, tmp_tubes_relation_);
    }

    std::map<std::string, std::pair<TubeId, IndexId>>
    getWorkflowTubesRelation(std::string workflow_name) {
        return tube_manager_.getWorkflowTubesRelation(workflow_name);
    }

    void init(std::shared_ptr<MachineManager> mac_manager);

private:
    std::map<std::string, std::shared_ptr<Reagent>> reagents_;

    TubeManager tube_manager_;

    std::map<std::string, std::shared_ptr<Carrier>> carriers_;
};