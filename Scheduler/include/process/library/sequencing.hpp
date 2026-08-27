#pragma once

#include "procedure/stage.hpp"
#include "process/dummy.hpp"

class LibSequencing {
public:
    enum { kOutput = 0 };
    inline static const std::string Name      = "LibSequencing";
    inline static const std::string Duration  = "Duration";
    inline static const std::string FileName  = "FileName";
    inline static const std::string FileIndex = "FileIndex";

    inline static const std::string FilePrefix = "sequencing_";
    inline static const std::string FileSuffix = ".fastq";

    static nlohmann::json fromDummy(const DummyStep& dummy_step) {
        nlohmann::json input        = nlohmann::json::object();
        auto           dummy_params = dummy_step.getParams();
        if (dummy_params.contains(std::to_string(static_cast<int>(Dummy::ParamType::DURATION)))) {
            input[Duration] =
                dummy_params[std::to_string(static_cast<int>(Dummy::ParamType::DURATION))];
        } else {
            input[Duration] = 60 * 1000;
        }
        return input;
    }

    static std::string fileName(uint16_t index) {
        auto raw = std::to_string(index);
        return FilePrefix + std::string(raw.size() >= 4 ? 0 : 4 - raw.size(), '0') + raw +
               FileSuffix;
    }
};

class LibSequencingStep : public CRTPStep<LibSequencingStep> {
public:
    LibSequencingStep(std::string name, Variables&& user_input = Variables())
        : CRTPStep<LibSequencingStep>(MachineType::LIBRARY, name, std::move(user_input)) {
        step_funcs_ = {std::bind(&LibSequencingStep::phase0, this, std::placeholders::_1,
                                 std::placeholders::_2, std::placeholders::_3),
                       std::bind(&LibSequencingStep::phase1, this, std::placeholders::_1,
                                 std::placeholders::_2, std::placeholders::_3)};
    }

    long long getTime(bool conflict = false) const {
        if (conflict) {
            return 0;
        }
        auto duration = user_input_[LibSequencing::Duration].get<uint32_t>();
        return duration / 1000;
    }

    std::vector<ExecutionResult>
    phase0(Reality& reality, std::shared_ptr<MachineManager> mac_manager, ActionId action_id) {
        std::vector<ExecutionResult> results;
        auto machine = mac_manager->getMachine<LibraryModbusMachine>(machine_type_);

        machine->sequencing(action_id);

        results.push_back(ExecutionResult());
        return results;
    }

    std::vector<ExecutionResult>
    phase1(Reality& reality, std::shared_ptr<MachineManager> mac_manager, ActionId action_id) {
        std::vector<ExecutionResult> results;
        auto machine = mac_manager->getMachine<LibraryModbusMachine>(machine_type_);

        auto index = nextFileIndex(machine->get_sequencing_file_index());

        Variables output                = step_input_;
        output[LibSequencing::FileIndex] = index;
        output[LibSequencing::FileName]  = LibSequencing::fileName(index);

        std::shared_ptr<Step> next_step;
        auto                  iter = next_steps_.find(LibSequencing::kOutput);
        if (iter != next_steps_.end()) {
            next_step = iter->second;
        }

        results.push_back(ExecutionResult{next_step, std::move(output), false});
        return results;
    }

    std::string getOperationName() const override { return "Sequencing"; }

    static std::shared_ptr<LibSequencingStep> fromDummy(DummyStep& dummy_step) {
        if (dummy_step.getType() == Dummy::DummyType::SEQUENCING) {
            return std::make_shared<LibSequencingStep>(LibSequencing::Name,
                                                       LibSequencing::fromDummy(dummy_step));
        }
        return nullptr;
    }

private:
    // the machine keeps its own run counter, but the returned name must always
    // increase, so fall back to last + 1 when the register did not move
    static uint16_t nextFileIndex(uint16_t machine_index) {
        static std::mutex mtx;
        static uint16_t   last_index = 0;

        std::lock_guard<std::mutex> lock(mtx);
        last_index = machine_index > last_index ? machine_index : last_index + 1;
        return last_index;
    }
};

class LibSequencingStage : public TemplatedStage<LibSequencing> {
public:
    LibSequencingStage(std::string id, Variables&& input) : TemplatedStage(std::move(id), true) {
        my_step_ = std::make_shared<LibSequencingStep>(name_, std::move(input));
    }

    void generateWorkflowHelper(Workflow& workflow) {
        if (next_stages_.size() != 1) {
            return;
        }
        genWorkflowForNextStage(workflow, LibSequencing::kOutput);
    }
};
