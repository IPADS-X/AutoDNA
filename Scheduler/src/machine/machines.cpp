#include "machine/machines.hpp"

void PortageModbusMachine::init() {
    logger->debug("PortageModbusMachine init");
    int rc;
    rc = client_.writeSingleRegister(PortageModbus::START_ADDR, 0);
    SPDLOG_ASSERT(rc == 1, "");
    rc = client_.writeSingleRegister(PortageModbus::REAL_FINISH_ADDR, 0);
    SPDLOG_ASSERT(rc == 1, "");
    rc = client_.writeSingleRegister(PortageModbus::FINISH_ADDR, 0);
    SPDLOG_ASSERT(rc == 1, "");

    ModbusMachine::init();
}

void PortageModbusMachine::autoPortage(uint16_t from, uint16_t to, ActionId action_id) {
    subscribe((int)PortageModbus::FINISH_ADDR, action_id, action_id);

    int      rc;
    uint16_t dest[SN_LEN];
    memset(dest, 0, sizeof(dest));

    rc = client_.writeSingleRegister(PortageModbus::START_ADDR, action_id);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.readHoldingRegisters(PortageModbus::READY_ADDR, 1, dest);
    SPDLOG_ASSERT(rc == 1, "");
    // ASSERT(dest[0] == 1);

    rc = client_.writeSingleRegister(PortageModbus::NOTIFY_MODE_ADDR, 1);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(PortageModbus::NOTIFY_SRC_ADDR, from);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(PortageModbus::NOTIFY_DEST_ADDR, to);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(PortageModbus::NOTIFY_ADDR, 1);
    SPDLOG_ASSERT(rc == 1, "");
}

void PortageModbusMachine::manualPortage(uint16_t to, const std::string sn, ActionId action_id) {
    subscribe((int)PortageModbus::FINISH_ADDR, action_id, action_id);

    int      rc;
    uint16_t dest;

    rc = client_.writeSingleRegister(PortageModbus::START_ADDR, action_id);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.readHoldingRegisters(PortageModbus::READY_ADDR, 1, &dest);
    SPDLOG_ASSERT(rc == 1, "");
    // ASSERT(dest == 1);

    rc = client_.writeSingleRegister(PortageModbus::NOTIFY_MODE_ADDR, 2);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(PortageModbus::NOTIFY_MANUAL_POS_ADDR, 5);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(PortageModbus::NOTIFY_MANUAL_DEST_ADDR, to);
    SPDLOG_ASSERT(rc == 1, "");

    int size = (sn.size() + 1) / 2;
    rc       = client_.writeMultipleRegisters(PortageModbus::NOTIFY_MANUAL_SN_ADDR, size,
                                              (uint16_t*)sn.data());
    ASSERT(rc == size);

    rc = client_.writeSingleRegister(PortageModbus::NOTIFY_ADDR, 1);
    SPDLOG_ASSERT(rc == 1, "");
}

void PortageModbusMachine::onPortageFinished() {
    int      rc;
    uint16_t dest;
    rc = client_.readHoldingRegisters(PortageModbus::FINISH_ADDR, 1, &dest);
    SPDLOG_ASSERT(rc == 1, "");
    // ASSERT(dest == 1);
}

void ModbusMachine::acceptPortageTransfer(ActionId action_id) {
    subscribe((int)ConnecctPortageModbus::FINISH_ADDR, action_id, action_id);

    int rc = client_.writeSingleRegister(ConnecctPortageModbus::MODE_ADDR, 1);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(ConnecctPortageModbus::START_ADDR, action_id);
    SPDLOG_ASSERT(rc == 1, "");
}

void ModbusMachine::requestPortageTransfer(ActionId action_id) {
    subscribe((int)ConnecctPortageModbus::FINISH_ADDR, action_id, action_id);

    int rc = client_.writeSingleRegister(ConnecctPortageModbus::MODE_ADDR, 0);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(ConnecctPortageModbus::START_ADDR, action_id);
    SPDLOG_ASSERT(rc == 1, "");
}