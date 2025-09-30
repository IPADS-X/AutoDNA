#pragma once

#include "machines.hpp"

#include "common/logging.h"
#include "common/thread_safe_queue.hpp"
#include "procedure/common.hpp"

#include <map>
#include <thread>

class MachineManager {
public:
    MachineManager(ThreadSafeQueue<std::shared_ptr<MyEvent>>& event_queue)
        : event_queue_(event_queue) {

#ifndef LOCAL_TEST
        addMachine(std::make_shared<AmplificationModbusMachine>("192.168.250.40", 502));
        addMachine(std::make_shared<FluorescenceModbusMachine>("192.168.250.60", 502));
        addMachine(std::make_shared<PurificationModbusMachine>("192.168.250.50", 502));
        addMachine(std::make_shared<LibraryModbusMachine>("192.168.250.70", 502));
        addMachine(std::make_shared<PortageModbusMachine>("192.168.250.150", 502));
        // addMachine(std::make_shared<PortageModbusMachine>("127.0.0.1", 1502));
#else
        addMachine(std::make_shared<AmplificationModbusMachine>("127.0.0.1", 1500));
        addMachine(std::make_shared<FluorescenceModbusMachine>("127.0.0.1", 1501));
        addMachine(std::make_shared<PurificationModbusMachine>("127.0.0.1", 1502));
        addMachine(std::make_shared<PortageModbusMachine>("127.0.0.1", 1503));
        addMachine(std::make_shared<LibraryModbusMachine>("127.0.0.1", 1504));
#endif
    }
    ~MachineManager() {}

    template <typename T>
    std::shared_ptr<T> getMachine(MachineType type) {
        return getMachine<T>((MachineTypeId)type);
    }

    template <typename T>
    std::shared_ptr<T> getMachine(MachineTypeId type) {
        if (machines_.find(type) == machines_.end()) {
            logger->warn("Machine type {} not found", (int)type);
            return nullptr;
        }
        return std::dynamic_pointer_cast<T>(machines_[type]);
    }

    void start() {
        while (running_) {
            // print hello for every 5 seconds
#ifdef LOCAL_TEST
            std::this_thread::sleep_for(std::chrono::milliseconds(100));
#else
            std::this_thread::sleep_for(std::chrono::seconds(5));
#endif
            for (auto& [id, machine] : machines_) {
                // logger->debug("Checking events for machine: {}", machine->getName());
                machine->check_events(event_queue_);
            }
        }
    }

    void stop() { running_ = false; }

    static void initLogger(spdlog::level::level_enum lvl, spdlog::sinks_init_list sinks) {
        logger = std::make_shared<spdlog::logger>("MacManager", sinks);
        logger->set_level(lvl);
    }

    ThreadSafeQueue<std::shared_ptr<MyEvent>>& getEventQueue() { return event_queue_; }

    static MachineType toMachineType(std::string machine_type) {
        for (int i = 0; i < (int)MachineType::TOTAL_NUM; i++) {
            if (machine_type == magic_enum::enum_name((MachineType)i)) {
                return (MachineType)i;
            }
        }

        return MachineType::TOTAL_NUM; // Not found
    }

    static AreaId toAreaId(MachineType machine_type, std::string area_id) {
        if (machine_type == MachineType::FLUORESCENCE) {
            return (AreaId)magic_enum::enum_cast<FluorescenceArea>(area_id).value_or(FluorescenceArea::AUTO);
        } else if (machine_type == MachineType::AMPLIFICATION) {
            return (AreaId)magic_enum::enum_cast<AmplificationArea>(area_id).value_or(AmplificationArea::AUTO);
        } else if (machine_type == MachineType::PURIFICATION) {
            return (AreaId)magic_enum::enum_cast<PurificationArea>(area_id).value_or(PurificationArea::AUTO);
        } else if (machine_type == MachineType::LIBRARY) {
            return (AreaId)magic_enum::enum_cast<LibraryArea>(area_id).value_or(LibraryArea::AUTO);
        }
        // Add other machine types as needed
        return (AreaId)CommonAreaId::NOT_SUCCESS;
    }

    static std::basic_string_view<char> fromAreaId(MachineType machine_type, AreaId area_id) {
        if (machine_type == MachineType::FLUORESCENCE) {
            return magic_enum::enum_name((FluorescenceArea)area_id);
        } else if (machine_type == MachineType::AMPLIFICATION) {
            return magic_enum::enum_name((AmplificationArea)area_id);
        } else if (machine_type == MachineType::PURIFICATION) {
            return magic_enum::enum_name((PurificationArea)area_id);
        } else if (machine_type == MachineType::LIBRARY) {
            return magic_enum::enum_name((LibraryArea)area_id);
        }
        // Add other machine types as needed
        return magic_enum::enum_name((CommonAreaId)area_id);
    }

    bool removeTube(Container* tube){
        for (auto machine:machines_){
            machine.second->removeTube(tube);
        }
        return true;
    }

private:
    void addMachine(std::shared_ptr<Machine> machine);

    bool                                              running_ = true;
    std::map<MachineTypeId, std::shared_ptr<Machine>> machines_;
    ThreadSafeQueue<std::shared_ptr<MyEvent>>&        event_queue_;

    static std::shared_ptr<spdlog::logger> logger;
};