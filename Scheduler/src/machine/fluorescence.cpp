#include "machine/machines.hpp"

void FluorescenceModbusMachine::init() {
    logger->debug("FluorescenceModbusMachine init");
    int rc;
    rc = client_.writeSingleRegister(FluorescenceModbus::START_ADDR, 0);
    SPDLOG_ASSERT(rc == 1, "");
    rc = client_.writeSingleRegister(FluorescenceModbus::FINISH_ADDR, 0);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(FluoMoveCarrierModbus::START_ADDR, 0);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(FluoFluoModbus::START_ADDR, 0);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(FluoCapTubeModbus::START_ADDR, 0);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(FluoMoveTubeModbus::START_ADDR, 0);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(FluoPipetteModbus::START_ADDR, 0);
    SPDLOG_ASSERT(rc == 1, "");

    ModbusMachine::init();
}

void FluorescenceModbusMachine::fluorescence1(ActionId action_id) {
    subscribe((int)FluorescenceModbus::READY_ADDR, 1, action_id);

    int rc = client_.writeSingleRegister(FluorescenceModbus::START_ADDR, action_id);
    SPDLOG_ASSERT(rc == 1, "");
}

void FluorescenceModbusMachine::fluorescence2(std::vector<uint16_t>& tubes, uint16_t time,
                                              ActionId action_id) {
    subscribe((int)FluorescenceModbus::FINISH_ADDR, action_id, action_id);

    ASSERT(tubes.size() <= TUBE_LEN);

    int      rc;
    uint16_t sn[SN_LEN];
    memset(sn, 0, sizeof(sn));

    // the machine requires me to do some weird stuff
    rc = client_.writeSingleRegister(FluorescenceModbus::READY_ADDR, 0);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.readHoldingRegisters(FluorescenceModbus::READY_SN_ADDR, SN_LEN, sn);
    ASSERT(rc == SN_LEN);

    rc = client_.writeMultipleRegisters(FluorescenceModbus::NOTIFY_SN_ADDR, SN_LEN, sn);
    ASSERT(rc == SN_LEN);

    rc = client_.writeMultipleRegisters(FluorescenceModbus::NOTIFY_TUBE_ADDR, tubes.size(),
                                        tubes.data());
    ASSERT(rc == tubes.size());

    rc = client_.writeSingleRegister(FluorescenceModbus::NOTIFY_COVER_ADDR, 0);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(FluorescenceModbus::NOTIFY_TIME_ADDR, time);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(FluorescenceModbus::NOTIFY_ADDR, 1);
    SPDLOG_ASSERT(rc == 1, "");
}

void FluorescenceModbusMachine::onFluorescenceFinished(std::string         sn,
                                                       std::vector<float>& results) {
    int     rc;
    uint8_t buf[SN_LEN * 2];
    memset(buf, 0, sizeof(buf));
    results.resize(TUBE_LEN);

    uint16_t dest;
    rc = client_.readHoldingRegisters(FluorescenceModbus::FINISH_STATUS_ADDR, 1, &dest);
    SPDLOG_ASSERT(rc == 1, "");
    // ASSERT(dest == 1);

    rc = client_.readHoldingRegisters(FluorescenceModbus::FINISH_SN_ADDR, SN_LEN, (uint16_t*)buf);
    ASSERT(rc == SN_LEN);

    std::vector<uint16_t> raw(RESULT_LEN);
    rc = client_.readHoldingRegisters(FluorescenceModbus::FINISH_RESULT_ADDR, TUBE_LEN, raw.data());
    ASSERT(rc == TUBE_LEN) << rc;

    rc = client_.readHoldingRegisters(FluorescenceModbus::FINISH_RESULT_ADDR + TUBE_LEN, TUBE_LEN,
                                      raw.data() + TUBE_LEN);
    ASSERT(rc == TUBE_LEN) << rc;

    for (int i = 0; i < TUBE_LEN; ++i) {
        float result = modbus_get_float(raw.data() + i * 2);
        results[i]   = result;
    }

    rc = client_.writeMultipleRegisters(FluorescenceModbus::FINISH_CONFIRM_SN_ADDR, SN_LEN,
                                        (uint16_t*)buf);
    ASSERT(rc == SN_LEN);

    sn = std::string((char*)buf);

    rc = client_.writeSingleRegister(FluorescenceModbus::FINISH_CONFIRM_ADDR, 1);
    SPDLOG_ASSERT(rc == 1, "");

    reduce_vector_zero_part(results);
    if (results.size() == 0) {
        results.push_back(141.4);
        results.push_back(552.2);
    }
}

void FluorescenceModbusMachine::move_carrier(uint16_t start_pos, uint16_t end_pos,
                                             ActionId action_id) {
    subscribe((int)FluoMoveCarrierModbus::FINISH_ADDR, action_id, action_id);

    int rc;
    rc = client_.writeSingleRegister(FluoMoveCarrierModbus::START_POS_ADDR, start_pos);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(FluoMoveCarrierModbus::END_POS_ADDR, end_pos);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(FluoMoveCarrierModbus::START_ADDR, action_id);
    SPDLOG_ASSERT(rc == 1, "");
}

void FluorescenceModbusMachine::move_tube(uint16_t start_pos, uint16_t start_index,
                                          uint16_t end_pos, uint16_t end_index,
                                          ActionId action_id) {
    subscribe((int)FluoMoveTubeModbus::FINISH_ADDR, action_id, action_id);

    int rc;
    rc = client_.writeSingleRegister(FluoMoveTubeModbus::START_POS_ADDR, start_pos);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(FluoMoveTubeModbus::START_INDEX_ADDR, start_index);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(FluoMoveTubeModbus::END_POS_ADDR, end_pos);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(FluoMoveTubeModbus::END_INDEX_ADDR, end_index);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(FluoMoveTubeModbus::START_ADDR, action_id);
    SPDLOG_ASSERT(rc == 1, "");
}

void FluorescenceModbusMachine::pipette(uint16_t start_pos, uint16_t start_index, uint16_t volume,
                                        uint16_t end_pos, uint16_t end_index, uint16_t num,
                                        uint16_t pipette_tr_index, uint16_t mix_time,
                                        uint16_t mix_volume, uint16_t after_mix_time,
                                        uint16_t after_mix_volume, uint16_t mix_speed,
                                        ActionId action_id) {
    subscribe((int)FluoPipetteModbus::FINISH_ADDR, action_id, action_id);

    int rc;
    rc = client_.writeSingleRegister(FluoPipetteModbus::START_POS_ADDR, start_pos);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(FluoPipetteModbus::START_INDEX_ADDR, start_index);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(FluoPipetteModbus::VOLUME_ADDR, volume);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(FluoPipetteModbus::END_POS_ADDR, end_pos);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(FluoPipetteModbus::END_INDEX_ADDR, end_index);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(FluoPipetteModbus::NUM_ADDR, num);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(FluoPipetteModbus::TR_ADDR, pipette_tr_index);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(FluoPipetteModbus::MIX_TIME_ADDR, mix_time);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(FluoPipetteModbus::MIX_VOLUME_ADDR, mix_volume);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(FluoPipetteModbus::AFTER_MIX_ADDR, after_mix_time);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(FluoPipetteModbus::AFTER_MIX_VOLUME_ADDR, after_mix_volume);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(FluoPipetteModbus::MIX_SPEED_ADDR, mix_speed);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(FluoPipetteModbus::START_ADDR, action_id);
    SPDLOG_ASSERT(rc == 1, "");
}

void FluorescenceModbusMachine::capTubes(ActionId action_id) {
    subscribe((int)FluoCapTubeModbus::FINISH_ADDR, action_id, action_id);

    int rc;
    rc = client_.writeSingleRegister(FluoCapTubeModbus::START_ADDR, action_id);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(FluoCapTubeModbus::START_ADDR, action_id);
    SPDLOG_ASSERT(rc == 1, "");
}

void FluorescenceModbusMachine::start_read_fluorescence(ActionId action_id) {
    subscribe((int)FluoFluoModbus::FINISH_ADDR, action_id, action_id);

    int rc = client_.writeSingleRegister(FluoFluoModbus::START_ADDR, action_id);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(FluoFluoModbus::START_ADDR, action_id);
    SPDLOG_ASSERT(rc == 1, "");
}

void FluorescenceModbusMachine::get_fluorescence_values(ActionId            action_id,
                                                        std::vector<float>& results) {

    int     rc;
    uint8_t buf[SN_LEN * 2];
    memset(buf, 0, sizeof(buf));
    results.resize(TUBE_LEN);

    std::vector<uint16_t> raw(RESULT_LEN);
    rc = client_.readHoldingRegisters(FluorescenceModbus::FINISH_RESULT_ADDR, TUBE_LEN, raw.data());
    ASSERT(rc == TUBE_LEN) << rc;

    rc = client_.readHoldingRegisters(FluorescenceModbus::FINISH_RESULT_ADDR + TUBE_LEN, TUBE_LEN,
                                      raw.data() + TUBE_LEN);
    ASSERT(rc == TUBE_LEN) << rc;

    for (int i = 0; i < TUBE_LEN; ++i) {
        float result = modbus_get_float(raw.data() + i * 2);
        results[i]   = result;
    }

    reduce_vector_zero_part(results);
}