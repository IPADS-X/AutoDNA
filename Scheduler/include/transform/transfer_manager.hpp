#include <iostream>
#include <string>

// #include "common.hpp"
#include "machine/mac_manager.hpp"
#include "procedure/workflow.hpp"
#include "reality/reality.hpp"
#include "transform/alloc_machine.hpp"
#include "transform/block.hpp"
#include "transform/interval.hpp"
#include "transform/parser_code.hpp"

#include "common/logging.h"

class Action;

class TransferManager {
public:
    using TransferFunc = std::function<std::vector<std::shared_ptr<Workflow>>(
        Reality&, std::shared_ptr<MachineManager>, std::vector<std::shared_ptr<Workflow>>,
        std::shared_ptr<spdlog::logger>, std::string, int, int)>;
    static std::vector<std::shared_ptr<Workflow>>
    transferParser(Reality& reality, std::shared_ptr<MachineManager> mac_manager_,
                   std::vector<std::shared_ptr<Workflow>> sources,
                   std::shared_ptr<spdlog::logger> logger, std::string workflow_name, int times,
                   int jump_from) {
        return ParserCode::transferParser(reality, mac_manager_, sources, logger, workflow_name,
                                          times);
    }

    static std::vector<std::shared_ptr<Workflow>>
    transferBlock(Reality& reality, std::shared_ptr<MachineManager> mac_manager_,
                  std::vector<std::shared_ptr<Workflow>> sources,
                  std::shared_ptr<spdlog::logger> logger, std::string workflow_name, int times,
                  int jump_from) {
        return BlockTransformer::transferBlock(reality, mac_manager_, sources, logger);
    }

    static std::vector<std::shared_ptr<Workflow>>
    transferAlloc(Reality& reality, std::shared_ptr<MachineManager> mac_manager_,
                  std::vector<std::shared_ptr<Workflow>> sources,
                  std::shared_ptr<spdlog::logger> logger, std::string workflow_name, int times,
                  int jump_from) {
        return AllocMachine::transferAlloc(reality, mac_manager_, sources, logger);
    }

    static std::vector<std::shared_ptr<Workflow>>
    transferInterval(Reality& reality, std::shared_ptr<MachineManager> mac_manager_,
                     std::vector<std::shared_ptr<Workflow>> sources,
                     std::shared_ptr<spdlog::logger> logger, std::string workflow_name, int times,
                     int jump_from) {
        return IntervalProcedure::transferInterval(reality, mac_manager_, sources, logger,
                                                   workflow_name, jump_from);
    }

    // TODO: stage 4: alloc carrier and tubes

    // TODO: stage 5: how to move reagents? need lock reagent's carrier? or just move a copy to
    // neighbor

    static void initLogger(spdlog::level::level_enum lvl, spdlog::sinks_init_list sinks) {
        logger = std::make_shared<spdlog::logger>("CheckManager", sinks);
        logger->set_level(lvl);
    }

    static std::vector<std::shared_ptr<Workflow>>
    parse_and_generate(Reality& reality, std::shared_ptr<MachineManager> mac_manager_,
                       std::string workflow_name, int times, int jump_from = 1) {
        std::vector<std::shared_ptr<Workflow>> sources = {};
        for (const auto& func : transfer_funcs_) {
            sources = func(reality, mac_manager_, sources, logger, workflow_name, times, jump_from);
        }
        return sources;
    }

private:
    static inline std::vector<TransferFunc> transfer_funcs_ = {
        TransferFunc(transferParser), TransferFunc(transferBlock), TransferFunc(transferAlloc),
        TransferFunc(transferInterval)};

    static inline std::shared_ptr<spdlog::logger> logger = nullptr;
};