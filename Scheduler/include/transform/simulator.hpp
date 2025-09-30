#include "procedure/stage.hpp"
#include "procedure/workflow.hpp"
#include "reality/reality.hpp"
#include <dtl/dtl.hpp>

class StepGroup {
public:
    std::vector<std::shared_ptr<Step>> steps_;
    StepGroup(std::vector<std::shared_ptr<Step>> steps) : steps_(std::move(steps)) {}

    bool operator==(const StepGroup& other) const;

    long long getTime() const {
        long long total_time = 0;
        for (const auto& step : steps_) {
            total_time += step->getTime();
        }
        return total_time;
    }

    int getWorkflowId() const {
        if (steps_.empty()) {
            return -1; // No steps, no workflow ID
        }
        return steps_.front()->getWorkflowId();
    }

    std::string getInBlockName(int time) const {
        int start_time = 0;
        for (const auto& step : steps_) {
            auto end_time = start_time + step->getTime();
            if (start_time <= time && time < end_time) {
                return step->getName();
            }
            start_time = end_time;
        }
        return ""; // Time not found in any step
    }
};

class Block {
public:
    // Constructor
    Block() = default;

    std::vector<StepGroup> combined_steps_;

    bool operator==(const Block& other) const {
        // if first of the combined steps are equal, then the blocks are equal
        if (combined_steps_.empty() || other.combined_steps_.empty()) {
            return false;
        }

        // try alloc tubes
        if (!(combined_steps_.front() == other.combined_steps_.front()))
            return false;

        return true;
    }
};

class Simulator {
public:
    Simulator() = default;

    // Add methods for simulation logic here
    static std::set<std::pair<TubeId, TubeId>> runSimulation(std::vector<Block> blocks,
                                                             Reality&           reality);

    static void allocTubePos(const std::vector<std::pair<TubeId, IndexId>>& alloc_map) {
        for (const auto& [tube_id, index_id] : alloc_map) {
            tmp_tube_map_[tube_id] = {tube_id, index_id};
        }
    }

    static void startNew(bool clear_conflict = true) {
        tmp_tube_map_.clear();
        if (clear_conflict) {
            conflict_map_.clear();
        }
    }

    static bool tryAllocNeighbor(TubeId origin_tube_id, TubeId new_tube_id, int origin_index,
                                 int new_index) {
        // STEP 0: query tube manager to check if any are alloced
        auto origin_tube_alloc = reality->isAlloced(origin_tube_id);
        auto new_tube_alloc    = reality->isAlloced(new_tube_id);
        auto origin_tube       = reality->getTube(origin_tube_id);
        auto new_tube          = reality->getTube(new_tube_id);

        if (origin_tube_alloc) {
            tmp_tube_map_[origin_tube_id] = {origin_tube_id, 0};
        }
        if (new_tube_alloc) {
            tmp_tube_map_[new_tube_id] = {new_tube_id, 0};
        }

        // STEP 1: anyway, just alloc origin tube
        if (tmp_tube_map_.find(origin_tube_id) == tmp_tube_map_.end()) {
            tmp_tube_map_[origin_tube_id] = {origin_tube_id, 0};
        }

        int has_alloc_ids = 0;
        for (auto tube_id : tmp_tube_map_) {
            if (tube_id.second.first == origin_tube_id) {
                has_alloc_ids++;
            }
        }

        if (origin_tube_alloc && new_tube_alloc) {
            if (origin_index == new_index) {
                return origin_tube == new_tube && origin_index == new_index &&
                       TubeManager::checkSameReagent(origin_tube, origin_index, has_alloc_ids);
            } else {
                return origin_tube == new_tube;
            }
        }

        // TODO: delete this, this can queryed by tube manager
        if (TubeManager::getTubeType(new_tube) == TubeType::CHAMBER) {
            tmp_tube_map_[new_tube_id] = {origin_tube_id, 0};
            return true;
        }

        if (conflict_map_.find({origin_tube_id, new_tube_id}) != conflict_map_.end()) {
            // conflict detected
            tmp_tube_map_[origin_tube_id] = {origin_tube_id, 0};
            tmp_tube_map_[new_tube_id]    = {new_tube_id, 0};
            return false;
        }

        if (TubeManager::getTubeType(new_tube) != TubeType::STRIP_TUBE) {
            // alloc a new position
            // std::cout << "Allocating new position for tube: " << new_tube_id
            //           << " fail because not a strip tube, is type: "
            //           << magic_enum::enum_name(TubeManager::getTubeType(new_tube));
            tmp_tube_map_[new_tube_id] = {new_tube_id, 0};
            return false;
        }

        if (has_alloc_ids >= 8) {
                tmp_tube_map_[new_tube_id] = {new_tube_id, 0};
            return false;
        }

        tmp_tube_map_[new_tube_id] = {origin_tube_id, has_alloc_ids};

        return true;
    }

    static void setReality(Reality& reality) { Simulator::reality = &reality; }

    // Add private methods or member variables as needed

    // tmp values
    // tmp alloc tube->(real tube, index)
    static inline std::map<TubeId, std::pair<TubeId, int>> tmp_tube_map_ = {};

    static inline std::set<std::pair<TubeId, TubeId>> conflict_map_ = {};

    static inline Reality* reality = nullptr;
};