#include "magic_enum.hpp"
#include "procedure/stage.hpp"
#include "procedure/workflow.hpp"
#include "reality/reality.hpp"
#include <dtl/dtl.hpp>

#include "transform/simulator.hpp"

#include "process/amplification.hpp"
#include "process/amplification/aspirate_mix.hpp"
#include "process/amplification/move_carrier.hpp"
#include "process/amplification/move_tube.hpp"
#include "process/amplification/pcr.hpp"
#include "process/branch.hpp"
#include "process/fluorescence.hpp"
#include "process/fluorescence/aspirate_mix.hpp"
#include "process/fluorescence/captube.hpp"
#include "process/fluorescence/fluo.hpp"
#include "process/fluorescence/move_carrier.hpp"
#include "process/fluorescence/move_tube.hpp"
#include "process/fluorescence/pipette.hpp"
#include "process/forloop.hpp"
#include "process/library/aspirate_mix.hpp"
#include "process/library/centrifuge.hpp"
#include "process/library/heater.hpp"
#include "process/library/move_carrier.hpp"
#include "process/library/move_tube.hpp"
#include "process/library/pipette.hpp"
#include "process/library/timer.hpp"
#include "process/portage.hpp"
#include "process/purification.hpp"
#include "process/purification/aspirate_mix.hpp"
#include "process/purification/centrifugal.hpp"
#include "process/purification/centrifuge.hpp"
#include "process/purification/move_carrier.hpp"
#include "process/purification/move_tube.hpp"
#include "process/purification/pcr.hpp"
#include "process/purification/pipette.hpp"
#include "process/purification/shake.hpp"
#include "process/purification/timer.hpp"
#include "process/refrigerator/picking.hpp"
#include "process/refrigerator/placement.hpp"

class BlockTransformer {
    inline const static std::map<Dummy::DummyType, int> maxConcurrentSteps = {
        {Dummy::DummyType::ASPIRATE_MIX, 4},
        {Dummy::DummyType::MOVE_TUBE, 8},
        {Dummy::DummyType::PIPETTE, 4},
        {Dummy::DummyType::ALLOC_TUBE, 96}};

public:
    BlockTransformer(Reality& reality) : reality(reality) {}

    static void initLogger(spdlog::level::level_enum lvl, spdlog::sinks_init_list sinks) {
        logger_ = std::make_shared<spdlog::logger>("BlockManager", sinks);
        logger_->set_level(lvl);
    }

    static std::vector<Block> merge_blocks(dtl::Ses<Block>& ses, std::vector<Block>& b1,
                                           std::vector<Block>& b2) {
        std::vector<Block> blocks;
        for (const auto& elem : ses.getSequence()) {
            auto        block     = elem.first;       // The step that was changed.
            dtl::edit_t type      = elem.second.type; // The type of edit (add, delete, common).
            long long   beforeIdx = elem.second.beforeIdx; // Index in the old text.
            long long   afterIdx  = elem.second.afterIdx;  // Index in the new text.

            switch (type) {
            case dtl::SES_ADD:
                // std::cout << "Added Block at index " << afterIdx << "\n";
                // blocks.push_back(block);
                blocks.push_back(b2[afterIdx - 1]); // Assuming we want to keep the block from b2
                break;
            case dtl::SES_DELETE:
                // std::cout << "Deleted Block from index " << beforeIdx << "\n";
                blocks.push_back(b1[beforeIdx - 1]); // Assuming we want to keep the block from b1
                break;
            case dtl::SES_COMMON:
                // std::cout << "Common Block at index " << beforeIdx << " and " << afterIdx <<
                // "\n";
                blocks.push_back(merge_block(b1[beforeIdx - 1], b2[afterIdx - 1]));
                // blocks.push_back(append_block(b1[beforeIdx - 1]));
                break;
            }
        }
        return blocks;
    }

    static Block append_block(const Block& b1) {
        Block new_block;
        // Logic to append a block to itself
        // This is a placeholder; actual appending logic will depend on the specific requirements
        new_block.combined_steps_ = b1.combined_steps_;
        new_block.combined_steps_.push_back(new_block.combined_steps_.back());
        return new_block;
    }

    // consider merge shake with different time, need to move immediately
    static Block merge_block(const Block& b1, const Block& b2) {
        // logger_->debug("Merging two blocks: {} and {}",
        // magic_enum::enum_name(dynamic_cast<DummyStep*>(b1.combined_steps_.front().steps_.front().get())->getType()),
        //                magic_enum::enum_name(dynamic_cast<DummyStep*>(b2.combined_steps_.front().steps_.front().get())->getType()));
        Block merged_block;
        // Logic to merge two blocks into one
        // This is a placeholder; actual merging logic will depend on the specific requirements
        merged_block.combined_steps_.insert(merged_block.combined_steps_.end(),
                                            b1.combined_steps_.begin(), b1.combined_steps_.end());
        merged_block.combined_steps_.insert(merged_block.combined_steps_.end(),
                                            b2.combined_steps_.begin(), b2.combined_steps_.end());
        return merged_block;
    }

    static Block generate_block(std::vector<std::shared_ptr<Step>> steps) {
        Block block;
        // Logic to generate a block from a stage
        // This is a placeholder; actual logic will depend on the specific requirements
        block.combined_steps_ = {StepGroup(steps)};
        return block;
    }

    static std::vector<Block> generate_blocks(const std::vector<std::shared_ptr<Step>>& steps) {
        std::vector<Block> blocks;
        // if continuely move to a equipment, need merge them all
        // move to simulator to verify
        for (size_t i = 0; i < steps.size(); ++i) {
            blocks.push_back(generate_block({steps[i]}));
            // std::vector<std::shared_ptr<Step>> need_make_block_steps = {};
            // bool                               after_equipment       = false;
            // while (i < steps.size() && dynamic_cast<DummyStep*>(steps[i].get()) != nullptr) {
            //     auto step = dynamic_cast<DummyStep*>(steps[i].get());
            //     if (!after_equipment && step->targetIsEquipment()) {
            //         need_make_block_steps.push_back(steps[i]);
            //     } else if (step->nowIsEquipment()) {
            //         need_make_block_steps.push_back(steps[i]);
            //         after_equipment = true;
            //     } else if (after_equipment && step->targetIsEquipment()) {
            //         need_make_block_steps.push_back(steps[i]);
            //     } else {
            //         break;
            //     }
            //     i++;
            // }

            // if (need_make_block_steps.empty()) {
            //     blocks.push_back(generate_block({steps[i]}));
            // } else {
            //     auto block = generate_block(need_make_block_steps);
            //     blocks.push_back(block);
            // }
        }
        return blocks;
    }

    static std::vector<Block> split_blocks(const std::vector<Block>& blocks) {
        std::vector<Block> split_blocks;
        for (const auto& block : blocks) {
            auto max_steps = maxConcurrentSteps.end();
            if (auto step =
                    dynamic_cast<DummyStep*>(block.combined_steps_.front().steps_[0].get())) {
                max_steps = maxConcurrentSteps.find(step->getType());
                if (max_steps == maxConcurrentSteps.end()) {
                    max_steps = maxConcurrentSteps.find(Dummy::DummyType::ALLOC_TUBE);
                }
            }

            if (max_steps != maxConcurrentSteps.end() &&
                block.combined_steps_.size() > max_steps->second) {
                int max_count = max_steps->second;
                for (size_t i = 0; i < block.combined_steps_.size(); i += max_count) {
                    Block new_block;
                    new_block.combined_steps_.insert(
                        new_block.combined_steps_.end(), block.combined_steps_.begin() + i,
                        block.combined_steps_.begin() +
                            std::min(i + max_count, block.combined_steps_.size()));
                    split_blocks.push_back(new_block);
                }
            } else {
                split_blocks.push_back(block); // No splitting needed
            }
        }

        return split_blocks;
    }

    static long long get_blocks_time(const std::vector<Block>& blocks) {
        long long total_time = 0;
        for (const auto& block : blocks) {
            total_time +=
                block.combined_steps_[0].getTime(); // Assuming first step's time is representative
        }
        return total_time;
    }

    static std::shared_ptr<Step> multiple_steps_num(std::shared_ptr<Step> step, int num) {
        // auto new_step = std::make_shared<Step>(*step);
        auto now_num = step->getCurrentNum();
        step->setCurrentNum(num * now_num);
        return step;
    }

    static std::vector<std::shared_ptr<Step>>
    get_steps_from_blocks(const std::vector<Block>& blocks) {
        std::vector<std::shared_ptr<Step>> steps{};

        for (const auto& block : blocks) {
            for (const auto& step : block.combined_steps_[0].steps_) {
                steps.push_back(multiple_steps_num(step, block.combined_steps_.size()));
            }
        }

        for (size_t i = 0; i + 1 < steps.size(); ++i) {
            steps[i]->setNextStep(0, steps[i + 1]);
        }
        return steps;
    }

    void add_need_merge_steps(const std::vector<std::shared_ptr<Step>>& steps) {
        need_merge_steps_.push_back(steps);
    }

    void add_need_merge_workflow(std::shared_ptr<Workflow> workflow) {
        auto steps = workflow->getSteps();
        need_merge_steps_.push_back(steps);
    }

    std::vector<std::shared_ptr<Step>> get_merged_steps() {
        if (split_blocks_.empty()) {
            get_merged_blocks();
        }
        return BlockTransformer::get_steps_from_blocks(split_blocks_);
    }

    std::shared_ptr<Workflow> get_merged_workflow() {
        auto steps    = get_merged_steps();
        auto workflow = std::make_shared<Workflow>(1); // Assuming workflow ID is 1
        for (const auto& step : steps) {
            workflow->addStep(step);
        }
        return workflow;
    }

    // void fill_tmp_tubes_map() {
    //     tmp_tube_map_.clear();
    //     for (auto& block : merged_blocks_) {
    //         for (const auto& step_group : block.combined_steps_) {
    //             for (const auto& step : step_group.steps_) {
    //                 auto dummy_step = dynamic_cast<DummyStep*>(step.get());
    //                 if (dummy_step) {
    //                     auto tube_id = dummy_step->getTubeId();
    //                     if (reality.isAlloced(tube_id)) {
    //                         tmp_tube_map_[tube_id] = {tube_id, 0}; // Assuming index 0 for
    //                         simplicity
    //                     }
    //                 }
    //             }
    //         }
    //     }
    // }

    std::vector<Block> get_merged_blocks() {
        need_merge_blocks_.clear();
        need_merge_blocks_.reserve(need_merge_steps_.size());
        merged_blocks_ = BlockTransformer::generate_blocks(need_merge_steps_.front());
        need_merge_blocks_.push_back(merged_blocks_);
        Simulator::startNew();
        for (size_t i = 1; i < need_merge_steps_.size();) {

            auto new_blocks = BlockTransformer::generate_blocks(need_merge_steps_[i]);
            need_merge_blocks_.push_back(new_blocks);

            // alloc tubes here
            dtl::Diff<Block> diff(merged_blocks_, new_blocks);
            diff.compose();
            auto result = diff.getSes();

            // finish alloc
            auto try_merged_blocks_ =
                BlockTransformer::merge_blocks(result, merged_blocks_, new_blocks);

            auto conflict = Simulator::runSimulation(try_merged_blocks_, reality);
            if (conflict.empty()) {
                merged_blocks_ = try_merged_blocks_;
                Simulator::startNew();
                i++;
            } else {
                // Simulator::allocTubePos(conflict);
                // just start a new simulation
                Simulator::startNew(false);
                logger_->warn("Conflict detected when merging workflows, start a new simulation");
            }
        }

        split_blocks_ = BlockTransformer::split_blocks(merged_blocks_);

        return split_blocks_;
    }

    std::vector<long long> get_all_times() const {
        long long before_concurrent_time = 0;
        for (const auto& steps : need_merge_steps_) {
            for (const auto& step : steps) {
                before_concurrent_time += step->getTime();
            }
        }
        long long merged_time = BlockTransformer::get_blocks_time(merged_blocks_);
        long long split_time  = BlockTransformer::get_blocks_time(split_blocks_);

        return {before_concurrent_time, merged_time, split_time};
    }

    std::vector<std::vector<std::string>> get_concurrent_info(int time_interval = 10) {
        if (split_blocks_.empty()) {
            get_merged_blocks();
        }
        std::vector<std::vector<std::string>> infos;
        auto                                  start_time = 0;
        auto                                  line       = 0;
        for (const auto& block : split_blocks_) {
            auto end_time      = start_time + block.combined_steps_.front().getTime();
            int  in_block_time = 0;
            while (start_time < end_time) {
                infos.push_back(std::vector<std::string>());
                for (int i = 1; i <= need_merge_steps_.size(); ++i) {
                    bool found = false;
                    for (auto&& step : block.combined_steps_) {
                        if (step.getWorkflowId() == i) {
                            infos[line].push_back(step.getInBlockName(in_block_time));
                            found = true;
                            break;
                        }
                    }
                    if (!found) {
                        infos[line].push_back(" ");
                    }
                }
                in_block_time += time_interval;
                start_time += time_interval;
                line++;
            }
        }

        return infos;
    }

    static std::vector<std::shared_ptr<Workflow>>
    transferSelfBlock(Reality& reality, std::shared_ptr<MachineManager> mac_manager_,
                      std::vector<std::shared_ptr<Workflow>> sources,
                      std::shared_ptr<spdlog::logger>        logger) {
        std::vector<std::shared_ptr<Workflow>> results;
        for (const auto& workflow : sources) {
            // // Perform self-block transfer logic here
            // results.push_back(workflow);

            Simulator::startNew();
            // for every step in the workflow
            std::shared_ptr<Workflow> new_workflow = std::make_shared<Workflow>(workflow->getId());

            // first merge pipette and mix
            std::vector<std::shared_ptr<StepGroup>> step_groups;
            auto                                    steps = workflow->getSteps();
            for (int i = 0; i < steps.size(); i++) {
                auto dummy_step = dynamic_cast<DummyStep*>(steps[i].get());
                if (i + 1 < steps.size()) {
                    auto next_dummy_step = dynamic_cast<DummyStep*>(steps[i + 1].get());
                    if (dummy_step->getType() == Dummy::DummyType::PIPETTE &&
                        next_dummy_step->getType() == Dummy::DummyType::ASPIRATE_MIX) {
                        step_groups.push_back(std::make_shared<StepGroup>(
                            std::vector<std::shared_ptr<Step>>{steps[i], steps[i + 1]}));
                        i++;
                    } else {
                        step_groups.push_back(std::make_shared<StepGroup>(
                            std::vector<std::shared_ptr<Step>>{steps[i]}));
                    }
                } else {
                    step_groups.push_back(
                        std::make_shared<StepGroup>(std::vector<std::shared_ptr<Step>>{steps[i]}));
                }
            }

            // for every step and next, try merge them by similar algo of block
            for (int i = 0; i < step_groups.size(); i++) {
                int start_i = i;
                while (i + 1 < step_groups.size()) {
                    // logger_->debug("Comparing step group {} and {}", i, i + 1);
                    if (*step_groups[i] == *step_groups[i + 1]) {
                        // can merge
                        logger_->debug("Inner Merging steps in workflow {}: step {} and step {}",
                                       workflow->getId(), i, i + 1);
                        i++;
                    } else {
                        break;
                    }
                }
                for (const auto& step : step_groups[start_i]->steps_) {
                    new_workflow->addStep(multiple_steps_num(step, i - start_i + 1));
                }
            }

            std::vector<Block> try_merged_blocks_;
            for (const auto& step : new_workflow->getSteps()) {
                auto block = generate_block({step});
                try_merged_blocks_.push_back(block);
            }

            // finally run simulation to check if right
            auto conflict = Simulator::runSimulation(try_merged_blocks_, reality);
            if (conflict.empty()) {
                logger_->debug("No conflict detected in workflow {}", workflow->getId());
                results.push_back(new_workflow);
            } else {
                logger_->warn("Conflict detected in workflow {}", workflow->getId());
                results.push_back(workflow);
            }
        }
        return results;
    }

    static std::vector<std::shared_ptr<Workflow>>
    transferBlock(Reality& reality, std::shared_ptr<MachineManager> mac_manager_,
                  std::vector<std::shared_ptr<Workflow>> sources,
                  std::shared_ptr<spdlog::logger>        logger) {
        auto block_transformer = BlockTransformer(reality);

        logger_->info("Get {} workflows to merge", sources.size());

        Simulator::setReality(reality);

        for (int i = 0; i < sources.size(); ++i) {
            std::shared_ptr<Workflow> workflow = sources[i];

            block_transformer.add_need_merge_workflow(workflow);
        }

        auto workflow = block_transformer.get_merged_workflow();

        workflow =
            BlockTransformer::transferSelfBlock(reality, mac_manager_, {workflow}, logger_)[0];

        for (auto& step : workflow->getSteps()) {
            auto dummy_step = dynamic_cast<DummyStep*>(step.get());
            if (dummy_step) {
                logger_->debug("Step type: {}, Step params: {}",
                               magic_enum::enum_name(dummy_step->getType()),
                               dummy_step->getParams());
            }
        }

        return {workflow};
    }

    void setReality(Reality& reality) { this->reality = reality; }

private:
    std::vector<std::vector<std::shared_ptr<Step>>> need_merge_steps_;
    std::vector<std::vector<Block>>                 need_merge_blocks_;
    std::vector<Block>                              merged_blocks_;
    std::vector<Block>                              split_blocks_;

    Simulator simulator_;

    Reality& reality;

    static inline std::shared_ptr<spdlog::logger> logger_ = nullptr;
};