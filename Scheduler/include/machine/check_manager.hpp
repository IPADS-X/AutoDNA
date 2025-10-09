#include <iostream>
#include <string>

#include "common.hpp"
#include "machine/mac_manager.hpp"
#include "reality/reality.hpp"

#include "common/logging.h"

class Action;
class MachineManager;
class Workflow;

class CheckManager {
public:
    using SubmitFunc = std::function<bool(std::shared_ptr<Workflow>)>;

    using CheckFunc = std::function<bool(Reality&, std::shared_ptr<MachineManager>,
                                         std::shared_ptr<Action>, CheckType, SubmitFunc)>;

    using CarrierFunc = std::function<std::shared_ptr<Step>(TubeId, AreaId, AreaId)>;

    using MoveTubeToCarrierFunc = std::function<std::shared_ptr<Step>(TubeId)>;

    static std::map<MachineType, CarrierFunc> carrier_funcs_;

    static std::map<MachineType, MoveTubeToCarrierFunc> move_tube_to_carrier_funcs_;

    static bool checkConsumable(Reality& reality, std::shared_ptr<MachineManager> mac_manager_,
                                std::shared_ptr<Action> action, CheckType check_type,
                                SubmitFunc func);

    static bool checkPortageInner(Reality& reality, std::shared_ptr<MachineManager> mac_manager_,
                                  std::shared_ptr<Action> action, CheckType check_type,
                                  SubmitFunc func, bool is_again);

    static bool checkPortageOnce(Reality& reality, std::shared_ptr<MachineManager> mac_manager_,
                                 std::shared_ptr<Action> action, CheckType check_type,
                                 SubmitFunc func) {
        return checkPortageInner(reality, mac_manager_, action, check_type, func, false);
    }

    static bool checkPortageTwice(Reality& reality, std::shared_ptr<MachineManager> mac_manager_,
                                 std::shared_ptr<Action> action, CheckType check_type,
                                 SubmitFunc func) {
        return checkPortageInner(reality, mac_manager_, action, check_type, func, true);
    }

    static bool checkTubeType(Reality& reality, std::shared_ptr<MachineManager> mac_manager_,
                              std::shared_ptr<Action> action, CheckType check_type,
                              SubmitFunc func);

    static bool checkReagents(Reality& reality, std::shared_ptr<MachineManager> mac_manager_,
                              std::shared_ptr<Action> action, CheckType check_type,
                              SubmitFunc func);

    static bool checkEquipment(Reality& reality, std::shared_ptr<MachineManager> mac_manager_,
                               std::shared_ptr<Action> action, CheckType check_type,
                               SubmitFunc func);

    static void initLogger(spdlog::level::level_enum lvl, spdlog::sinks_init_list sinks) {
        logger = std::make_shared<spdlog::logger>("CheckManager", sinks);
        logger->set_level(lvl);
    }

    static bool check(Reality& reality, std::shared_ptr<MachineManager> mac_manager_,
                      std::shared_ptr<Action> action, SubmitFunc submit_func) {
        for (const auto& func : check_funcs_) {
            if (!func(reality, mac_manager_, action, CheckType::CHECKONLY, submit_func)) {
                return false;
            }
        }
        return true;
    }

    static bool apply(Reality& reality, std::shared_ptr<MachineManager> mac_manager_,
                      std::shared_ptr<Action> action, SubmitFunc submit_func) {
        for (const auto& func : check_funcs_) {
            if (!func(reality, mac_manager_, action, CheckType::APPLY, submit_func)) {
                return false;
            }
        }
        return true;
    }

    static bool release(Reality& reality, std::shared_ptr<MachineManager> mac_manager_,
                        std::shared_ptr<Action> action, SubmitFunc submit_func) {
        for (const auto& func : check_funcs_) {
            if (!func(reality, mac_manager_, action, CheckType::RELEASE, submit_func)) {
                return false;
            }
        }
        return true;
    }

private:
    static inline std::vector<CheckFunc> check_funcs_ = {
        CheckFunc(checkPortageOnce), CheckFunc(checkConsumable), CheckFunc(checkPortageTwice), CheckFunc(checkTubeType),
        CheckFunc(checkReagents), CheckFunc(checkEquipment)};

    static inline std::shared_ptr<spdlog::logger> logger = nullptr;
};