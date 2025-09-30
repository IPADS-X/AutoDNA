#include "machine/machines.hpp"

void LibraryModbusMachine::init() {
    logger->debug("LibraryModbusMachine init");
    int rc;
    rc = client_.writeSingleRegister(LibMoveCarrierModbus::START_ADDR, 0);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(LibAspirateMixModbus::START_ADDR, 0);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(LibMoveTubeModbus::START_ADDR, 0);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(LibMovePcrTubeModbus::START_ADDR, 0);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(LibPipetteModbus::START_ADDR, 0);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(LibCentrifugeModbus::START_ADDR, 0);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(LibMoveTubeModbus::START_ADDR, 0);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(LibTimeModbus::START_ADDR, 0);
    SPDLOG_ASSERT(rc == 1, "");

    ModbusMachine::init();
}

void LibraryModbusMachine::move_carrier(uint16_t start_pos, uint16_t end_pos, ActionId action_id) {
    subscribe((int)LibMoveCarrierModbus::FINISH_ADDR, action_id, action_id);

    int rc;
    rc = client_.writeSingleRegister(LibMoveCarrierModbus::START_POS_ADDR, start_pos);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(LibMoveCarrierModbus::END_POS_ADDR, end_pos);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(LibMoveCarrierModbus::START_ADDR, action_id);
    SPDLOG_ASSERT(rc == 1, "");
}

void LibraryModbusMachine::pipette(uint16_t start_pos, uint16_t start_index, uint16_t volume,
                                   uint16_t end_pos, uint16_t end_index, uint16_t num,
                                   uint16_t pipette_tr_index, uint16_t mix_time,
                                   uint16_t mix_volume, uint16_t after_mix_time,
                                   uint16_t after_mix_volume, uint16_t mix_speed,
                                   ActionId action_id) {
    subscribe((int)LibPipetteModbus::FINISH_ADDR, action_id, action_id);

    int rc;
    rc = client_.writeSingleRegister(LibPipetteModbus::START_POS_ADDR, start_pos);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(LibPipetteModbus::START_INDEX_ADDR, start_index);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(LibPipetteModbus::END_POS_ADDR, end_pos);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(LibPipetteModbus::END_INDEX_ADDR, end_index);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(LibPipetteModbus::VOLUME_ADDR, volume);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(LibPipetteModbus::NUM_ADDR, num);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(LibPipetteModbus::TR_ADDR, pipette_tr_index);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(LibPipetteModbus::MIX_TIME_ADDR, mix_time);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(LibPipetteModbus::MIX_VOLUME_ADDR, mix_volume);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(LibPipetteModbus::AFTER_MIX_ADDR, after_mix_time);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(LibPipetteModbus::AFTER_MIX_VOLUME_ADDR, after_mix_volume);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(LibPipetteModbus::MIX_SPEED_ADDR, mix_speed);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(LibPipetteModbus::START_ADDR, action_id);
    SPDLOG_ASSERT(rc == 1, "");
}

void LibraryModbusMachine::move_tube(uint16_t start_pos, uint16_t start_index, uint16_t end_pos,
                                     uint16_t end_index, ActionId action_id) {
    subscribe((int)LibMoveTubeModbus::FINISH_ADDR, action_id, action_id);

    int rc;
    rc = client_.writeSingleRegister(LibMoveTubeModbus::START_POS_ADDR, start_pos);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(LibMoveTubeModbus::START_INDEX_ADDR, start_index);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(LibMoveTubeModbus::END_POS_ADDR, end_pos);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(LibMoveTubeModbus::END_INDEX_ADDR, end_index);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(LibMoveTubeModbus::START_ADDR, action_id);
    SPDLOG_ASSERT(rc == 1, "");
}

void LibraryModbusMachine::move_pcr_tube(uint16_t start_pos, uint16_t start_index, uint16_t end_pos,
                                         uint16_t end_index, ActionId action_id) {
    subscribe((int)LibMovePcrTubeModbus::FINISH_ADDR, action_id, action_id);

    int rc;
    rc = client_.writeSingleRegister(LibMovePcrTubeModbus::START_POS_ADDR, start_pos);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(LibMovePcrTubeModbus::START_INDEX_ADDR, start_index);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(LibMovePcrTubeModbus::END_POS_ADDR, end_pos);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(LibMovePcrTubeModbus::END_INDEX_ADDR, end_index);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(LibMovePcrTubeModbus::START_ADDR, action_id);
    SPDLOG_ASSERT(rc == 1, "");
}

void LibraryModbusMachine::centrifuge_8_tube(uint16_t duration, uint16_t speed,
                                             ActionId action_id) {
    subscribe((int)LibCentrifugeModbus::FINISH_ADDR, action_id, action_id);

    int rc;
    rc = client_.writeSingleRegister(LibCentrifugeModbus::DURATION_ADDR, duration);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(LibCentrifugeModbus::SPEED_ADDR, speed);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(LibCentrifugeModbus::TYPE_ADDR, 1); // 0: 8-tube
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(LibCentrifugeModbus::START_ADDR, action_id);
    SPDLOG_ASSERT(rc == 1, "");
}

void LibraryModbusMachine::centrifuge_pcr_tube(uint16_t duration, uint16_t speed,
                                               ActionId action_id) {
    subscribe((int)LibCentrifugeModbus::FINISH_ADDR, action_id, action_id);

    int rc;
    rc = client_.writeSingleRegister(LibCentrifugeModbus::DURATION_ADDR, duration);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(LibCentrifugeModbus::SPEED_ADDR, speed);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(LibCentrifugeModbus::TYPE_ADDR, 0); // 1: PCR tube
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(LibCentrifugeModbus::START_ADDR, action_id);
    SPDLOG_ASSERT(rc == 1, "");
}

void LibraryModbusMachine::centrifuge_mix_8_tube(uint16_t duration, uint16_t speed,
                                                 ActionId action_id) {
    subscribe((int)LibCentrifugeModbus::FINISH_ADDR, action_id, action_id);

    int rc;
    rc = client_.writeSingleRegister(LibCentrifugeModbus::DURATION_ADDR, duration);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(LibCentrifugeModbus::SPEED_ADDR, speed);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(LibCentrifugeModbus::TYPE_ADDR, 2); // 0: 8-tube
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(LibCentrifugeModbus::START_ADDR, action_id);
    SPDLOG_ASSERT(rc == 1, "");
}

void LibraryModbusMachine::time(uint32_t duration, ActionId action_id) {
    subscribe(0, duration / 1000, action_id);

    // subscribe((int)LibTimeModbus::FINISH_ADDR, action_id, action_id);

    // uint16_t time_type = (uint16_t)LibTimeModbus::TimeType::MILLISECOND;
    // int      rc;

    // if (duration > UINT16_MAX) {
    //     time_type = (uint16_t)LibTimeModbus::TimeType::SECOND;
    //     duration /= 1000;

    //     if (duration > UINT16_MAX) {
    //         time_type = (uint16_t)LibTimeModbus::TimeType::MINUTE;
    //         duration /= 60;
    //     }
    // }

    // rc = client_.writeSingleRegister(LibTimeModbus::DURATION_ADDR, (uint16_t)duration);
    // SPDLOG_ASSERT(rc == 1, "");

    // rc = client_.writeSingleRegister(LibTimeModbus::TYPE_ADDR, time_type);
    // SPDLOG_ASSERT(rc == 1, "");

    // rc = client_.writeSingleRegister(LibTimeModbus::START_ADDR, action_id);
    // SPDLOG_ASSERT(rc == 1, "");
}

void LibraryModbusMachine::heater(uint32_t duration, uint16_t temperature, ActionId action_id) {
    int rc;
    rc = client_.writeSingleRegister(LibSetHeaterTempModbus::TEMP_ADDR, temperature);
    SPDLOG_ASSERT(rc == 1, "");

    // just called timer
    time(duration, action_id);
}

void LibraryModbusMachine::aspirate_mix(uint16_t pos, uint16_t index, uint16_t volume,
                                        uint16_t total, uint16_t num, uint16_t pipette_tr_index,
                                        uint16_t mix_speed, ActionId action_id) {
    subscribe((int)LibAspirateMixModbus::FINISH_ADDR, action_id, action_id);

    int rc;
    rc = client_.writeSingleRegister(LibAspirateMixModbus::POS_ADDR, pos);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(LibAspirateMixModbus::INDEX_ADDR, index);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(LibAspirateMixModbus::VOLUME_ADDR, volume);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(LibAspirateMixModbus::TOTAL_ADDR, total);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(LibAspirateMixModbus::NUM_ADDR, num);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(LibAspirateMixModbus::TR_ADDR, pipette_tr_index);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(LibAspirateMixModbus::MIX_SPEED_ADDR, mix_speed);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(LibAspirateMixModbus::START_ADDR, action_id);
    SPDLOG_ASSERT(rc == 1, "");
}