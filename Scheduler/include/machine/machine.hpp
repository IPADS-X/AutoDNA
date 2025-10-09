#pragma once

#include "common.hpp"

#include "common/logging.h"
#include "common/thread_safe_queue.hpp"
#include "machine/equipment.hpp"
#include "procedure/common.hpp"
#include "reality/carrier/carrier.hpp"
#include "reality/container/tube.hpp"

#include "modbus/client.hpp"
#include <set>

#include "magic_enum.hpp"

enum class CheckType { CHECKONLY, APPLY, RELEASE };

class Machine {

public:
    Machine(MachineType machine_type, std::vector<std::shared_ptr<Equipment>> equipments,
            std::set<AreaId> areas)
        : machine_type_(machine_type), all_place_areas_(areas), avail_place_areas_(areas) {
        for (const auto& equipment : equipments) {
            if (all_equipments_.find(equipment->getType()) == all_equipments_.end()) {
                all_equipments_[equipment->getType()] = equipment;
            } else {
                logger->warn("Duplicate equipment type {} in machine {}",
                             magic_enum::enum_name(equipment->getType()), getName());
            }
        }
    }

    virtual ~Machine() = default;

    MachineTypeId      getType() { return (MachineTypeId)machine_type_; }
    const std::string& getName() { return MachineNames[(MachineTypeId)machine_type_]; }

    virtual void init() {};
    virtual void check_events(ThreadSafeQueue<std::shared_ptr<MyEvent>>& event_queue) {};

    static void initLogger(spdlog::level::level_enum lvl, spdlog::sinks_init_list sinks) {
        logger = std::make_shared<spdlog::logger>("Machine", sinks);
        logger->set_level(lvl);
    }

    std::set<EquipmentType> getAllEquipments() const {
        std::set<EquipmentType> types;
        for (const auto& equipment : all_equipments_) {
            types.insert(equipment.second->getType());
        }
        return types;
    }

    std::set<EquipmentType> getAvailEquipments() const {
        std::set<EquipmentType> types;
        for (const auto& equipment : all_equipments_) {
            if (equipment.second->isAvailable()) {
                types.insert(equipment.second->getType());
            }
        }
        return types;
    }

    std::shared_ptr<Equipment> getEquipment(EquipmentType equipment) {
        auto it = all_equipments_.find(equipment);
        if (it != all_equipments_.end()) {
            return it->second;
        }
        return nullptr;
    }

    IndexId allocEquipment(EquipmentType equipment, CheckType checkType, std::shared_ptr<Step> step,
                           std::shared_ptr<Container> container);

    bool releaseEquipment(EquipmentType equipment, std::shared_ptr<Step> step,
                          std::shared_ptr<Container> container);

    std::vector<std::shared_ptr<Step>> getEquipmentStep(EquipmentType equipment) const {
        auto it = all_equipments_.find(equipment);
        if (it != all_equipments_.end()) {
            return it->second->getSteps();
        }
        return {};
    }

    bool allocPlaceArea(AreaId area_id, std::shared_ptr<Carrier> carrier) {
        std::lock_guard<std::mutex> lock(mtx_);
        auto it = std::find(avail_place_areas_.begin(), avail_place_areas_.end(), area_id);
        if (it != avail_place_areas_.end()) {
            avail_place_areas_.erase(it);
            place_area_carriers_[area_id] = carrier;
            return true;
        }
        return false;
    }

    bool releasePlaceArea(AreaId area_id) {
        std::lock_guard<std::mutex> lock(mtx_);
        auto                        it = place_area_carriers_.find(area_id);
        if (it != place_area_carriers_.end()) {
            avail_place_areas_.insert(area_id);
            place_area_carriers_.erase(it);
            return true;
        }
        return false;
    }

    std::pair<bool, AreaId> allocNewArea(std::shared_ptr<Carrier> carrier,
                                         AreaId area_id = (AreaId)CommonAreaId::NOT_SUCCESS) {
        if (area_id != (AreaId)CommonAreaId::NOT_SUCCESS) {
            if (allocPlaceArea(area_id, carrier)) {
                return {true, area_id};
            }
        }
        for (auto it = avail_place_areas_.begin(); it != avail_place_areas_.end(); ++it) {
            AreaId area_id = *it;
            if (allocPlaceArea(area_id, carrier)) {
                return {true, area_id};
            }
        }
        return {false, (AreaId)CommonAreaId::NOT_SUCCESS};
    }

    std::shared_ptr<Carrier> getCarrier(AreaId area_id) const {
        auto it = place_area_carriers_.find(area_id);
        if (it != place_area_carriers_.end()) {
            return it->second;
        }
        return nullptr;
    }

    std::pair<std::shared_ptr<Carrier>, TubePosition>
    allocAConsumeTube(std::shared_ptr<Container> tube) {
        for (const auto& [area_id, carrier] : place_area_carriers_) {
            if (carrier->isConsumer()) {
                int index = carrier->getOrAllocTubeIndex(tube.get());
                if (index != -1) {
                    logger->debug("Allocated tube {} to carrier {} in index {}", TubeManager::getTubeId(tube),
                                  carrier->getName(), index);
                    TubeManager::setTubeCarrier(tube, carrier);
                    return {carrier, {machine_type_, area_id, (IndexId)index}};
                }
            }
        }
        return {nullptr, {MachineType::TOTAL_NUM, -1, -1}};
    }

    bool removeTube(Container* tube) {
        for (auto equip : all_equipments_) {
            if (equip.second->release(tube)) {
                return true;
            }
        }
        return false;
    }

protected:
    MachineType machine_type_;

    static std::shared_ptr<spdlog::logger> logger;

    std::map<EquipmentType, std::shared_ptr<Equipment>> all_equipments_;

    std::set<AreaId>                           all_place_areas_;
    std::set<AreaId>                           avail_place_areas_;
    std::map<AreaId, std::shared_ptr<Carrier>> place_area_carriers_;

private:
    std::mutex mtx_;
};

class ModbusMachine : public Machine {
public:
    ModbusMachine(MachineType machine_type, const char* ip, int port,
                  std::vector<std::shared_ptr<Equipment>> equipments, std::set<AreaId> areas)
        : Machine(machine_type, equipments, areas),
          client_(ip, port),
          support_automic_service_(true) {}

    ModbusMachine(MachineType machine_type, const char* ip, int port)
        : Machine(machine_type, {}, {}), client_(ip, port), support_automic_service_(false) {}

    struct Subscription {
        enum class SubscriptionType { OTHER, TIME };
        uint64_t         expected_value;
        ActionId         action_id;
        SubscriptionType type;
    };

    virtual ~ModbusMachine() {}

    virtual void init() {
        int rc = client_.writeSingleRegister(ConnecctPortageModbus::START_ADDR, 0);
        SPDLOG_ASSERT(rc == 1, "Failed to initialize {}: writeSingleRegister returned {}",
                      getName(), rc);
    }

    virtual void check_events(ThreadSafeQueue<std::shared_ptr<MyEvent>>& event_queue) {
        auto subscriptions = getSubscriptions();
        for (auto& [addr, subscription] : subscriptions) {
            if (subscription.type == Subscription::SubscriptionType::TIME) {
                // timer
                auto now_time = std::chrono::duration_cast<std::chrono::seconds>(
                                    std::chrono::system_clock::now().time_since_epoch())
                                    .count();
                // logger every 10 seconds
                if (now_time - last_check_time > 10) {
                    logger->debug(
                        "Checking timer: {}'s address({}) with expected_value {}, now time {}",
                        getName(), addr, subscription.expected_value, now_time);
                    last_check_time = now_time;
                }
                if (now_time >= subscription.expected_value) {
                    // trigger event
                    unsubscribe(addr);
                    event_queue.push(std::make_shared<MyEvent>(subscription.action_id));
                }
            } else {
                uint16_t target;
                int      rc = client_.readHoldingRegisters(addr, 1, &target);
                SPDLOG_ASSERT(rc == 1, "");

                logger->debug("Read: {}'s address({}) has value {} while expected_value is {}",
                              getName(), addr, target, subscription.expected_value);

                // if the value is 0, we don't need to push it to the event queue
                if (target == subscription.expected_value) {
                    logger->info("{}'s address({}) changed to expected_value {} for action_id {}",
                                 getName(), addr, subscription.expected_value,
                                 subscription.action_id);
                    unsubscribe(addr);
                    event_queue.push(std::make_shared<MyEvent>(subscription.action_id));
                }
            }
        }
    }

    void subscribe(int addr, uint16_t expected_value, ActionId action_id) {
        std::lock_guard<std::mutex> lock(sub_mtx_);
        if (addr == 0) {
            // timer
            addr               = allocNewTimeAddr();
            uint64_t end_value = std::chrono::duration_cast<std::chrono::seconds>(
                                     std::chrono::system_clock::now().time_since_epoch())
                                     .count() +
                                 expected_value; // now + duration
            logger->debug("Subscribe a timer: {}'s address({}) with end_value {}, action_id {}, expected value {}",
                          getName(), addr, end_value, action_id, expected_value);
            subscriptions_.insert(
                {addr, Subscription{end_value, action_id, Subscription::SubscriptionType::TIME}});
            return;
        }

        uint16_t target;
        int      rc = client_.readHoldingRegisters(addr, 1, &target);
        SPDLOG_ASSERT(rc == 1, "subscribe not returned 1");
        // SPDLOG_ASSERT(target != expected_value, "Subscribe: {}'s address({}) has value {}",
        //               getName(), addr, target);

        subscriptions_[addr] = {expected_value, action_id, Subscription::SubscriptionType::OTHER};
    }

    std::map<int, Subscription> getSubscriptions() {
        std::lock_guard<std::mutex> lock(sub_mtx_);
        return subscriptions_;
    }

    void unsubscribe(int addr) {
        std::lock_guard<std::mutex> lock(sub_mtx_);
        subscriptions_.erase(addr);
    }

    int allocNewTimeAddr() {
        int new_addr = 1;
        // get max addr less than 10
        for (const auto& [addr, sub] : subscriptions_) {
            if (addr < 10) {
                new_addr = std::max(new_addr, addr);
            }
        }
        new_addr += 1; // next addr
        return new_addr;
    }

    bool supportAutomicService() const { return support_automic_service_; }

    void acceptPortageTransfer(ActionId action_id);

    void requestPortageTransfer(ActionId action_id);

protected:
    ModbusClient                client_;
    std::map<int, Subscription> subscriptions_;
    bool                        support_automic_service_ = false;
    uint64_t                    last_check_time;

private:
    std::mutex sub_mtx_;
};
