#pragma once

#include "procedure/stage.hpp"
#include "process/dummy.hpp"

class FluoPipette {
public:
    enum { kOutput = 0 };
    inline static const std::string Name            = "FluoPipette";
    inline static const std::string StartPos        = "StartPos";
    inline static const std::string StartIndex      = "StartIndex";
    inline static const std::string EndPos          = "EndPos";
    inline static const std::string EndIndex        = "EndIndex";
    inline static const std::string Volume          = "Volume";
    inline static const std::string PipetteNum      = "PipetteNum";
    inline static const std::string PipetteTrIndex  = "PipetteTrIndex";
    inline static const std::string BeforeMixTimes  = "BeforeMixTimes";
    inline static const std::string BeforeMixVolume = "BeforeMixVolume";
    inline static const std::string AfterMixTimes   = "AfterMixTimes";
    inline static const std::string AfterMixVolume  = "AfterMixVolume";
    inline static const std::string MixSpeed        = "MixSpeed";
    inline static const std::string SrcTube         = "SrcTube";
    inline static const std::string DstTube         = "DstTube";

    static nlohmann::json fromDummy(const DummyStep& dummy_step) {
        nlohmann::json input        = nlohmann::json::object();
        auto           dummy_params = dummy_step.getParams();
        input[SrcTube] =
            dummy_params[std::to_string(static_cast<int>(Dummy::ParamType::SRC_TUBE_INDEX))];
        input[DstTube] =
            dummy_params[std::to_string(static_cast<int>(Dummy::ParamType::DEST_TUBE_INDEX))];

        if (dummy_params.contains(
                std::to_string(static_cast<int>(Dummy::ParamType::START_POSITION)))) {
            input[StartPos] =
                dummy_params[std::to_string(static_cast<int>(Dummy::ParamType::START_POSITION))];
        } else {
            input[StartPos] = (uint16_t)FluorescenceArea::AUTO;
        }

        if (dummy_params.contains(
                std::to_string(static_cast<int>(Dummy::ParamType::START_INDEX)))) {
            input[StartIndex] =
                dummy_params[std::to_string(static_cast<int>(Dummy::ParamType::START_INDEX))];
        } else {
            input[StartIndex] = (uint16_t)CommonIndexId::AUTO;
        }

        if (dummy_params.contains(
                std::to_string(static_cast<int>(Dummy::ParamType::END_POSITION)))) {
            input[EndPos] =
                dummy_params[std::to_string(static_cast<int>(Dummy::ParamType::END_POSITION))];
        } else {
            input[EndPos] = (uint16_t)FluorescenceArea::AUTO;
        }
        if (dummy_params.contains(std::to_string(static_cast<int>(Dummy::ParamType::END_INDEX)))) {
            input[EndIndex] =
                dummy_params[std::to_string(static_cast<int>(Dummy::ParamType::END_INDEX))];
        } else {
            input[EndIndex] = (uint16_t)CommonIndexId::AUTO;
        }

        if (dummy_params.contains(std::to_string(static_cast<int>(Dummy::ParamType::VOLUME)))) {
            input[Volume] =
                dummy_params[std::to_string(static_cast<int>(Dummy::ParamType::VOLUME))];
        } else {
            input[Volume] = (uint16_t)CommonVolumeId::AUTO;
        }

        if (dummy_params.contains(
                std::to_string(static_cast<int>(Dummy::ParamType::PIPETTE_NUM)))) {
            input[PipetteNum] =
                dummy_params[std::to_string(static_cast<int>(Dummy::ParamType::PIPETTE_NUM))];
        } else {
            input[PipetteNum] = 1;
        }

        if (dummy_params.contains(
                std::to_string(static_cast<int>(Dummy::ParamType::PIPETTE_TR_INDEX)))) {
            input[PipetteTrIndex] =
                dummy_params[std::to_string(static_cast<int>(Dummy::ParamType::PIPETTE_TR_INDEX))];
        } else {
            input[PipetteTrIndex] = PipetteTrType::UL_200;
        }

        return input;
    }
};

class FluoPipetteStep : public CRTPStep<FluoPipetteStep> {
public:
    FluoPipetteStep(std::string name, Variables&& user_input = Variables())
        : CRTPStep<FluoPipetteStep>(MachineType::FLUORESCENCE, name, std::move(user_input)) {
        step_funcs_ = {std::bind(&FluoPipetteStep::phase0, this, std::placeholders::_1,
                                 std::placeholders::_2, std::placeholders::_3),
                       std::bind(&FluoPipetteStep::phase1, this, std::placeholders::_1,
                                 std::placeholders::_2, std::placeholders::_3)};
    }

    bool setCurrentNum(uint16_t current_num) override {
        if (current_num <= 0 || current_num > 4) {
            logger->error("FluoPipetteStep, setCurrentNum: current_num {} is out of range [1, 8]",
                          current_num);
            return false;
        }

        if (user_input_.find(FluoPipette::PipetteNum) != user_input_.end()) {
            user_input_[FluoPipette::PipetteNum] = current_num;
            if (user_input_[FluoPipette::StartPos].get<uint16_t>() ==
                (uint16_t)FluorescenceArea::SAMPLE_AREA) {
                user_input_[FluoPipette::StartIndex] = workflow_id_ - 1;
            } else if (user_input_[FluoPipette::EndPos].get<uint16_t>() ==
                       (uint16_t)FluorescenceArea::SAMPLE_AREA) {
                user_input_[FluoPipette::EndIndex] = workflow_id_ - 1;
            }
            return true;
        }

        logger->error("FluoPipetteStep, setCurrentNum: user_input_ does not contain PipetteNum");

        std::cout << user_input_.dump(4) << std::endl;
        return false;
    }

    uint16_t getCurrentNum() const {
        if (user_input_.find(FluoPipette::PipetteNum) !=
            user_input_.end()) {
            return user_input_
                .at(FluoPipette::PipetteNum)
                .get<uint16_t>();
        }
        return 1;
    }

    long long getTime() const {
        return 50; // simulate time for pipetting
    }

    std::vector<ExecutionResult>
    phase0(Reality& reality, std::shared_ptr<MachineManager> mac_manager, ActionId action_id) {
        std::vector<ExecutionResult> results;
        auto machine = mac_manager->getMachine<FluorescenceModbusMachine>(machine_type_);

        auto start_pos        = user_input_[FluoPipette::StartPos].get<uint16_t>();
        auto start_index      = user_input_[FluoPipette::StartIndex].get<uint16_t>();
        auto volume           = user_input_[FluoPipette::Volume].get<uint16_t>();
        auto end_pos          = user_input_[FluoPipette::EndPos].get<uint16_t>();
        auto end_index        = user_input_[FluoPipette::EndIndex].get<uint16_t>();
        auto num              = user_input_[FluoPipette::PipetteNum].get<uint16_t>();
        auto pipette_tr_index = user_input_[FluoPipette::PipetteTrIndex].get<uint16_t>();

        logger->info("FluoPipetteStep, phase0: start_pos: {}, start_index: {}, volume: {}, "
                     "end_pos: {}, end_index: {}, num: {}",
                     start_pos, start_index, volume, end_pos, end_index, num);
        if (volume <= 5000) {
            pipette_tr_index = (uint16_t)PipetteTrType::UL_50;
        } else {
            SPDLOG_ASSERT(pipette_tr_index == (uint16_t)PipetteTrType::UL_200,
                          "FluoPipetteStep, phase0: volume > 5000, but pipette_tr_index != UL_200");
        }

        uint16_t before_mix_time   = 0;
        uint16_t before_mix_volume = 0;
        if (user_input_.find(FluoPipette::BeforeMixTimes) != user_input_.end() &&
            user_input_.find(FluoPipette::BeforeMixVolume) != user_input_.end()) {
            before_mix_time   = user_input_[FluoPipette::BeforeMixTimes].get<uint16_t>();
            before_mix_volume = user_input_[FluoPipette::BeforeMixVolume].get<uint16_t>();
        }

        uint16_t after_mix_time   = 0;
        uint16_t after_mix_volume = 0;
        if (user_input_.find(FluoPipette::AfterMixTimes) != user_input_.end() &&
            user_input_.find(FluoPipette::AfterMixVolume) != user_input_.end()) {
            after_mix_time   = user_input_[FluoPipette::AfterMixTimes].get<uint16_t>();
            after_mix_volume = user_input_[FluoPipette::AfterMixVolume].get<uint16_t>();
        }

        if (pipette_tr_index == (uint16_t)PipetteTrType::UL_50) {
            before_mix_volume = std::min(before_mix_volume, (uint16_t)4500);
            after_mix_volume  = std::min(after_mix_volume, (uint16_t)4500);
        }

        SPDLOG_ASSERT(before_mix_volume <= 20000, "");
        SPDLOG_ASSERT(after_mix_volume <= 20000, "");

        uint16_t mix_speed = 0;
        if (user_input_.find(FluoPipette::MixSpeed) != user_input_.end()) {
            mix_speed = user_input_[FluoPipette::MixSpeed].get<uint16_t>();
        } else {
            mix_speed = 100;
        }

        if (start_pos == (uint16_t)FluorescenceArea::AUTO) {
            auto tube = user_input_[FluoPipette::SrcTube].get<TubeId>();
            auto tube_pos =
                reality.getTubePosition(tube, TubePositionType::SINGLE_TUBE, start_index);

            start_pos   = tube_pos.area_id;
            start_index = tube_pos.index;

            if (start_pos == (uint16_t)CommonAreaId::NOT_SUCCESS) {
                start_pos = (uint16_t)FluorescenceArea::SAMPLE_AREA;
            }
        }

        if (end_pos == (uint16_t)FluorescenceArea::AUTO) {
            auto tube     = user_input_[FluoPipette::DstTube].get<TubeId>();
            auto tube_pos = reality.getTubePosition(tube, TubePositionType::SINGLE_TUBE, end_index);

            end_pos   = tube_pos.area_id;
            end_index = tube_pos.index;

            if (end_pos == (uint16_t)CommonAreaId::NOT_SUCCESS) {
                end_pos = (uint16_t)FluorescenceArea::SAMPLE_AREA;
            }

            if (end_pos == (uint16_t)CommonAreaId::WASTE_AREA) {
                end_pos = (uint16_t)FluorescenceArea::WASTE_AREA;
                end_index = 0;
            }
        }

        if (start_pos == (uint16_t)FluorescenceArea::AUTO &&
            end_pos == (uint16_t)FluorescenceArea::AUTO) {
            auto start_tube = reality.getTube(user_input_[FluoPipette::SrcTube].get<TubeId>());
            auto end_tube   = reality.getTube(user_input_[FluoPipette::DstTube].get<TubeId>());

            if (volume > (int)CommonVolumeId::AUTO) {
                volume = start_tube->getVolume(0) / (volume - (int)CommonVolumeId::AUTO);
            }

            for (int i = 0; i < user_input_[FluoPipette::PipetteNum].get<uint16_t>(); i++) {
                auto start_volume = start_tube->getVolume(i);
                auto end_volume   = end_tube->getVolume(i);
                start_volume      = std::max(0, start_volume - volume);
                end_volume        = std::max(0, end_volume + volume);

                start_tube->setVolume(i, start_volume);
                end_tube->setVolume(i, end_volume);
            }
        }

        logger->debug("Pipetting from {}:{} to {}:{} with volume {} and {} times mixing",
                       magic_enum::enum_name((FluorescenceArea)start_pos), start_index,
                       magic_enum::enum_name((FluorescenceArea)end_pos), end_index,
                       volume, before_mix_time);

        machine->pipette(start_pos, start_index, volume, end_pos, end_index, num, pipette_tr_index,
                         before_mix_time, before_mix_volume, after_mix_time, after_mix_volume,
                         mix_speed, action_id);

        results.push_back(ExecutionResult());
        return results;
    }

    std::vector<ExecutionResult>
    phase1(Reality& reality, std::shared_ptr<MachineManager> mac_manager, ActionId action_id) {
        std::vector<ExecutionResult> results;

        results.push_back(ExecutionResult{next_steps_[FluoPipette::kOutput], Variables(), false});
        return results;
    }

    std::vector<std::pair<MachineType, EquipmentType>>
    getNeedLockEquipment(Reality& reality, std::shared_ptr<MachineManager> mac_manager_,
                         std::shared_ptr<Action> action) const {
        if (action->getCurrentPhase() != 0) {
            return {};
        }

        return {std::make_pair(machine_type_, EquipmentType::ROBOT_ARM),
                std::make_pair(machine_type_, EquipmentType::PIPETEE_GUN)};
    }

    std::vector<std::pair<MachineType, EquipmentType>>
    getNeedUnlockEquipment(Reality& reality, std::shared_ptr<MachineManager> mac_manager_,
                           std::shared_ptr<Action> action) const {
        if (!action->isDone()) {
            return {};
        }

        return {std::make_pair(machine_type_, EquipmentType::ROBOT_ARM),
                std::make_pair(machine_type_, EquipmentType::PIPETEE_GUN)};
    }

    std::vector<std::shared_ptr<Step>>
    getTubeTypeSupportsSteps(Reality& reality, std::map<TubeId, TubeId>& tube_map) {
        auto new_params = user_input_;
        if (user_input_.find(FluoPipette::SrcTube) == user_input_.end() ||
            user_input_.find(FluoPipette::DstTube) == user_input_.end()) {
            return {std::make_shared<FluoPipetteStep>(name_, std::move(new_params))};
        }
        auto dst_tube_id  = user_input_[FluoPipette::DstTube].get<TubeId>();
        auto dst_tube_ptr = reality.getTube(dst_tube_id);

        std::vector<std::shared_ptr<Step>> res;

        if (TubeManager::getTubeType(dst_tube_ptr) == TubeType::PCR_TUBE) {
            // change according to tube_map
            if (tube_map.find(dst_tube_id) != tube_map.end()) {
                dst_tube_id = tube_map[dst_tube_id];
            }
            new_params[FluoPipette::DstTube] = dst_tube_id;
            auto volume                       = user_input_[FluoPipette::Volume].get<int>() / 8;
            for (int i = 0; i < 8; i++) {
                new_params[FluoPipette::EndIndex] = i;
                new_params[FluoPipette::Volume]   = volume;
                res.push_back(std::make_shared<FluoPipetteStep>(name_, std::move(new_params)));
            }
            return res;
        }
        return {std::make_shared<FluoPipetteStep>(name_, std::move(new_params))};
    }

    static std::shared_ptr<FluoPipetteStep> fromDummy(DummyStep& dummy_step) {
        if (dummy_step.getType() == Dummy::DummyType::PIPETTE) {
            return std::make_shared<FluoPipetteStep>(FluoPipette::Name,
                                                     FluoPipette::fromDummy(dummy_step));
        }
        return nullptr;
    }
};

class FluoPipetteStage : public TemplatedStage<FluoPipette> {
public:
    FluoPipetteStage(std::string id, Variables&& input) : TemplatedStage(std::move(id), true) {
        my_step_ = std::make_shared<FluoPipetteStep>(name_, std::move(input));
    }

    void generateWorkflowHelper(Workflow& workflow) {
        if (next_stages_.size() != 1) {
            return;
        }
        genWorkflowForNextStage(workflow, FluoPipette::kOutput);
    }
};