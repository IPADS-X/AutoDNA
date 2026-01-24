#include "transform/simulator.hpp"
#include "process/dummy.hpp"

// Add methods for simulation logic here
std::set<std::pair<TubeId, TubeId>> Simulator::runSimulation(std::vector<Block> blocks,
                                                             Reality&           reality) {
    // Simulation logic goes here

    // goes and check if all on equipment tubes are reacted at same time
    // that only possible when move_tube splitted by other ops

    // if occurs, add conflict to these tubes, just return them

    std::map<TubeId, Dummy::DummyEquipment> tube_pos;

    auto tmp_capped_tubes = std::set<TubeId>();
    auto tmp_conflicts    = std::set<std::pair<TubeId, TubeId>>();
    for (auto& block : blocks) {
        if (block.combined_steps_.empty()) {
            continue;
        }
        if (block.combined_steps_.front().steps_.empty()) {
            continue;
        }

        // ===================================
        // FIRST CHECK CAPPED TUBES
        if (dynamic_cast<DummyStep*>(block.combined_steps_.front().steps_.front().get())) {
            auto dummy_step =
                dynamic_cast<DummyStep*>(block.combined_steps_.front().steps_.front().get());
            if (dummy_step->getType() == Dummy::DummyType::CAP) {
                auto params = dummy_step->getParams();
                if (params.find(std::to_string(static_cast<int>(Dummy::ParamType::TUBE_INDEX))) !=
                    params.end()) {
                    auto tube_id =
                        params.find(std::to_string(static_cast<int>(Dummy::ParamType::TUBE_INDEX)))
                            .value()
                            .get<TubeId>();
                    // get all tubes allocated from this tube
                    for (const auto& [origin_tube, new_tube] : tmp_tube_map_) {
                        if (origin_tube == tube_id || new_tube.first == tube_id) {
                            tmp_capped_tubes.insert(origin_tube);
                        }
                    }
                }
            }

            // if pipette a capped tube, then conflict
            for (auto& step_group : block.combined_steps_) {
                for (auto& step : step_group.steps_) {
                    auto dummy_step = std::dynamic_pointer_cast<DummyStep>(step);
                    if (!dummy_step) {
                        continue;
                    }
                    if (dummy_step->getType() == Dummy::DummyType::PIPETTE) {
                        auto params = dummy_step->getParams();
                        if (params.find(std::to_string(
                                static_cast<int>(Dummy::ParamType::TUBE_INDEX))) != params.end()) {
                            auto tube_id = params
                                               .find(std::to_string(
                                                   static_cast<int>(Dummy::ParamType::TUBE_INDEX)))
                                               .value()
                                               .get<TubeId>();
                            if (tmp_capped_tubes.find(tube_id) != tmp_capped_tubes.end()) {
                                // tube is capped, conflict
                                for (const auto& capped_tube : tmp_capped_tubes) {
                                    tmp_conflicts.insert({tube_id, capped_tube});
                                    tmp_conflicts.insert({capped_tube, tube_id});
                                }
                            }
                        }
                    }
                }
            }
        }

        // ===================================
        // THEN CHECK EQUIPMENT POSITIONS
        // first move all merged step's tubes to equipment
        for (auto& step_group : block.combined_steps_) {
            for (auto& step : step_group.steps_) {
                auto dummy_step = std::dynamic_pointer_cast<DummyStep>(step);
                if (!dummy_step) {
                    continue;
                }
                auto params = dummy_step->getParams();
                if (params.find(std::to_string(static_cast<int>(Dummy::ParamType::END_POSITION))) !=
                    params.end()) {
                    auto tube_id =
                        params.find(std::to_string(static_cast<int>(Dummy::ParamType::TUBE_INDEX)))
                            .value()
                            .get<TubeId>();
                    tube_pos[tube_id] =
                        params
                            .find(std::to_string(static_cast<int>(Dummy::ParamType::END_POSITION)))
                            .value()
                            .get<Dummy::DummyEquipment>();
                }
            }
        }

        // get all merged steps's only on machine tubes
        std::vector<TubeId>   expect_on_machine_tubes;
        Dummy::DummyEquipment expect_on_machine = Dummy::DummyEquipment::REACTION_POS;
        for (auto& step_group : block.combined_steps_) {
            for (auto& step : step_group.steps_) {
                auto dummy_step = std::dynamic_pointer_cast<DummyStep>(step);
                if (!dummy_step) {
                    continue;
                }

                auto params = dummy_step->getParams();
                if (params.find(std::to_string(static_cast<int>(Dummy::ParamType::TUBE_LIST))) !=
                    params.end()) {
                    // check if all tubes are on the same place
                    for (const auto& tube_id :
                         params.find(std::to_string(static_cast<int>(Dummy::ParamType::TUBE_LIST)))
                             .value()
                             .get<std::vector<TubeId>>()) {
                        expect_on_machine_tubes.push_back(tube_id);
                    }
                }

                if (params.find(std::to_string(static_cast<int>(Dummy::ParamType::POSITION))) !=
                    params.end()) {
                    expect_on_machine =
                        params.find(std::to_string(static_cast<int>(Dummy::ParamType::POSITION)))
                            .value()
                            .get<Dummy::DummyEquipment>();
                }

                if (params.find(std::to_string(static_cast<int>(Dummy::ParamType::POSITION))) !=
                    params.end()) {
                    expect_on_machine =
                        params.find(std::to_string(static_cast<int>(Dummy::ParamType::POSITION)))
                            .value()
                            .get<Dummy::DummyEquipment>();
                }
            }
        }

        if (expect_on_machine_tubes.empty()) {
            continue;
        }

        // if (expect_on_machine != Dummy::DummyEquipment::HEATER_SHAKER){
        //     continue;
        // }

        // get all tubes on machine
        std::vector<TubeId> real_on_machine_tubes;
        for (const auto& [tube_id, equipment] : tube_pos) {
            if (equipment == expect_on_machine) {
                real_on_machine_tubes.push_back(tube_id);
            }
        }

        // check if they are same
        if (real_on_machine_tubes != expect_on_machine_tubes) {
            // handle the case where they are not the same
            for (const auto& tube_id : real_on_machine_tubes) {
                if (std::find(expect_on_machine_tubes.begin(), expect_on_machine_tubes.end(),
                              tube_id) == expect_on_machine_tubes.end()) {
                    // tube is expected to be on machine but is not
                    for (auto& expect_tube : expect_on_machine_tubes) {
                        tmp_conflicts.insert({expect_tube, tube_id});
                        tmp_conflicts.insert({tube_id, expect_tube});
                    }
                }
            }
            break;
        }
    }

    if (!tmp_conflicts.empty()) {
        for (const auto& [tube1, tube2] : tmp_conflicts) {
            conflict_map_.insert({tube1, tube2});
        }
    }

    return tmp_conflicts;
}

bool StepGroup::operator==(const StepGroup& other) const {
    if (steps_.size() != other.steps_.size()) {
        // std::cout << "Step size mismatch: " << steps_.size() << " vs "
        //           << other.steps_.size() << "\n";
        return false;
    }
    for (size_t i = 0; i < steps_.size(); ++i) {
        if (dynamic_cast<DummyStep*>(steps_[i].get()) &&
            dynamic_cast<DummyStep*>(other.steps_[i].get())) {
            if (!dynamic_cast<DummyStep*>(steps_[i].get())
                     ->check(*dynamic_cast<DummyStep*>(other.steps_[i].get()))) {
                // std::cout << "Step " << i << " is not equal" << std::endl;
                return false;
            }

            std::cout << "Checking step "
                      << magic_enum::enum_name(dynamic_cast<DummyStep*>(steps_[i].get())->getType())
                      << " and " << magic_enum::enum_name(dynamic_cast<DummyStep*>(other.steps_[i].get())->getType())
                      << std::endl;

            auto user_input_  = dynamic_cast<DummyStep*>(steps_[i].get())->getParams();
            auto other_input_ = dynamic_cast<DummyStep*>(other.steps_[i].get())->getParams();
            // 3. alloc tubes
            bool res = true;
            if (res &&
                user_input_.contains(
                    std::to_string(static_cast<int>(Dummy::ParamType::TUBE_INDEX))) &&
                other_input_.contains(
                    std::to_string(static_cast<int>(Dummy::ParamType::TUBE_INDEX)))) {
                int origin_index = 0;
                if (user_input_.contains(
                        std::to_string(static_cast<int>(Dummy::ParamType::INDEX)))) {
                    origin_index =
                        user_input_.at(std::to_string(static_cast<int>(Dummy::ParamType::INDEX)));
                }
                int new_index = 0;
                if (other_input_.contains(
                        std::to_string(static_cast<int>(Dummy::ParamType::INDEX)))) {
                    new_index =
                        other_input_.at(std::to_string(static_cast<int>(Dummy::ParamType::INDEX)));
                }
                auto value = Simulator::tryAllocNeighbor(
                    user_input_.at(std::to_string(static_cast<int>(Dummy::ParamType::TUBE_INDEX))),
                    other_input_.at(std::to_string(static_cast<int>(Dummy::ParamType::TUBE_INDEX))),
                    origin_index, new_index);
                if (!value) {
                    res = false;
                }
            }
            if (res && user_input_.contains(
                           std::to_string(static_cast<int>(Dummy::ParamType::SRC_TUBE_INDEX)))) {
                int origin_index = 0;
                if (user_input_.contains(
                        std::to_string(static_cast<int>(Dummy::ParamType::START_INDEX)))) {
                    origin_index = user_input_.at(
                        std::to_string(static_cast<int>(Dummy::ParamType::START_INDEX)));
                }
                int new_index = 0;
                if (other_input_.contains(
                        std::to_string(static_cast<int>(Dummy::ParamType::START_INDEX)))) {
                    new_index = other_input_.at(
                        std::to_string(static_cast<int>(Dummy::ParamType::START_INDEX)));
                }
                auto value = Simulator::tryAllocNeighbor(
                    user_input_.at(
                        std::to_string(static_cast<int>(Dummy::ParamType::SRC_TUBE_INDEX))),
                    other_input_.at(
                        std::to_string(static_cast<int>(Dummy::ParamType::SRC_TUBE_INDEX))),
                    origin_index, new_index);
                if (!value) {
                    res = false;
                }
            }
            if (res && user_input_.contains(
                           std::to_string(static_cast<int>(Dummy::ParamType::DEST_TUBE_INDEX)))) {
                int origin_index = 0;
                if (user_input_.contains(
                        std::to_string(static_cast<int>(Dummy::ParamType::END_INDEX)))) {
                    origin_index = user_input_.at(
                        std::to_string(static_cast<int>(Dummy::ParamType::END_INDEX)));
                }
                int new_index = 0;
                if (other_input_.contains(
                        std::to_string(static_cast<int>(Dummy::ParamType::END_INDEX)))) {
                    new_index = other_input_.at(
                        std::to_string(static_cast<int>(Dummy::ParamType::END_INDEX)));
                }
                auto value = Simulator::tryAllocNeighbor(
                    user_input_.at(
                        std::to_string(static_cast<int>(Dummy::ParamType::DEST_TUBE_INDEX))),
                    other_input_.at(
                        std::to_string(static_cast<int>(Dummy::ParamType::DEST_TUBE_INDEX))),
                    origin_index, new_index);
                if (!value) {
                    res = false;
                }
            }

            if (!res) {
                return false;
            }

            continue; // Both are dummy steps, consider them equal
        }
        if (!(*steps_[i] == *other.steps_[i])) {
            return false;
        }
    }
    return true;
}