#include "machine/machines.hpp"

void PurificationModbusMachine::init() {
    return;
    logger->debug("PurificationModbusMachine init");
    int rc;
    rc = client_.writeSingleRegister(CentrifugalModbus::START_ADDR, 0);
    SPDLOG_ASSERT(rc == 1, "");
    rc = client_.writeSingleRegister(CentrifugalModbus::FINISH_ADDR, 0);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(PuriPipetteModbus::START_ADDR, 0);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(PuriMoveTubeModbus::START_ADDR, 0);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(PuriMoveCarrierModbus::START_ADDR, 0);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(PuriAspirateMixModbus::START_ADDR, 0);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(PuriShakeModbus::START_ADDR, 0);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(PuriPcrModbus::START_ADDR, 0);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(PuriCentrifugeModbus::START_ADDR, 0);
    SPDLOG_ASSERT(rc == 1, "");

    ModbusMachine::init();
}

void PurificationModbusMachine::centrifugal1(ActionId action_id) {
    subscribe((int)CentrifugalModbus::READY_ADDR, 1, action_id);

    int rc = client_.writeSingleRegister(CentrifugalModbus::START_ADDR, action_id);
    SPDLOG_ASSERT(rc == 1, "");
}

void PurificationModbusMachine::centrifugal2(std::vector<uint16_t>& tubes, uint16_t speed,
                                             uint16_t duration, ActionId action_id) {
    subscribe((int)CentrifugalModbus::FINISH_ADDR, action_id, action_id);

    SPDLOG_ASSERT(tubes.size() <= TUBE_LEN, "");
    reduce_vector_zero_part(tubes);

    int      rc;
    uint16_t sn[SN_LEN];
    memset(sn, 0, sizeof(sn));

    // the machine requires me to do some weird stuff
    rc = client_.writeSingleRegister(CentrifugalModbus::READY_ADDR, 0);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.readHoldingRegisters(CentrifugalModbus::READY_SN_ADDR, SN_LEN, sn);
    ASSERT(rc == SN_LEN);

    rc = client_.writeMultipleRegisters(CentrifugalModbus::NOTIFY_SN_ADDR, SN_LEN, sn);
    ASSERT(rc == SN_LEN);

    rc = client_.writeMultipleRegisters(CentrifugalModbus::NOTIFY_TUBE_ADDR, tubes.size(),
                                        tubes.data());
    ASSERT(rc == tubes.size());

    rc = client_.writeSingleRegister(CentrifugalModbus::NOTIFY_MODE_ADDR, 2);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(CentrifugalModbus::NOTIFY_SPEED_ADDR, speed);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(CentrifugalModbus::NOTIFY_DURATION_ADDR, duration);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(CentrifugalModbus::NOTIFY_ADDR, 1);
    SPDLOG_ASSERT(rc == 1, "");
}

void PurificationModbusMachine::onCentrifugalFinished(std::string            sn,
                                                      std::vector<uint16_t>& tubes) {
    int     rc;
    uint8_t buf[SN_LEN * 2];
    memset(buf, 0, sizeof(buf));
    tubes.resize(TUBE_LEN);

    uint16_t dest;
    rc = client_.readHoldingRegisters(CentrifugalModbus::FINISH_STATUS_ADDR, 1, &dest);
    SPDLOG_ASSERT(rc == 1, "");
    // ASSERT(dest == 1);

    rc = client_.readHoldingRegisters(CentrifugalModbus::FINISH_SN_ADDR, SN_LEN, (uint16_t*)buf);
    ASSERT(rc == SN_LEN);

    rc = client_.readHoldingRegisters(CentrifugalModbus::FINISH_TUBE_ADDR, TUBE_LEN, tubes.data());
    ASSERT(rc == TUBE_LEN);

    rc = client_.writeMultipleRegisters(CentrifugalModbus::FINISH_CONFIRM_SN_ADDR, SN_LEN,
                                        (uint16_t*)buf);
    ASSERT(rc == SN_LEN);

    rc = client_.writeSingleRegister(CentrifugalModbus::FINISH_CONFIRM_ADDR, 1);
    SPDLOG_ASSERT(rc == 1, "");

    sn = std::string((char*)buf);
    reduce_vector_zero_part(tubes);
}

void PurificationModbusMachine::pipette(uint16_t start_pos, uint16_t start_index, uint16_t volume,
                                        uint16_t end_pos, uint16_t end_index, uint16_t num,
                                        uint16_t pipette_tr_index, uint16_t mix_time,
                                        uint16_t mix_volume, uint16_t after_mix_time,
                                        uint16_t after_mix_volume, uint16_t mix_speed,
                                        ActionId action_id) {

    SPDLOG_ASSERT(start_index < 96, "");
    SPDLOG_ASSERT(end_index < 96, "");
    SPDLOG_ASSERT(num <= 4, "");
    SPDLOG_ASSERT(pipette_tr_index < 4, "");
    SPDLOG_ASSERT(mix_speed <= 1000, "");
    SPDLOG_ASSERT(mix_speed >= 50, "");

    if (pipette_tr_index == (uint16_t)PipetteTrType::UL_50) {
        SPDLOG_ASSERT(volume <= 5000, "");
    } else if (pipette_tr_index == (uint16_t)PipetteTrType::UL_200) {
        SPDLOG_ASSERT(volume <= 20000, "");
    }

    subscribe((int)PuriPipetteModbus::FINISH_ADDR, action_id, action_id);

    int rc;
    rc = client_.writeSingleRegister(PuriPipetteModbus::START_POS_ADDR, start_pos);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(PuriPipetteModbus::START_INDEX_ADDR, start_index);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(PuriPipetteModbus::VOLUME_ADDR, volume);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(PuriPipetteModbus::END_POS_ADDR, end_pos);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(PuriPipetteModbus::END_INDEX_ADDR, end_index);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(PuriPipetteModbus::NUM_ADDR, num);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(PuriPipetteModbus::TR_ADDR, pipette_tr_index);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(PuriPipetteModbus::MIX_TIME_ADDR, mix_time);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(PuriPipetteModbus::MIX_VOLUME_ADDR, mix_volume);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(PuriPipetteModbus::AFTER_MIX_ADDR, after_mix_time);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(PuriPipetteModbus::AFTER_MIX_VOLUME_ADDR, after_mix_volume);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(PuriPipetteModbus::MIX_SPEED_ADDR, mix_speed);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(PuriPipetteModbus::START_ADDR, action_id);
    SPDLOG_ASSERT(rc == 1, "");
}

void PurificationModbusMachine::move_tube(uint16_t start_pos, uint16_t start_index,
                                          uint16_t end_pos, uint16_t end_index,
                                          ActionId action_id) {
    SPDLOG_ASSERT(start_index < 12, "");
    SPDLOG_ASSERT(end_index < 12, "");

    subscribe((int)PuriMoveTubeModbus::FINISH_ADDR, action_id, action_id);

    int rc;
    rc = client_.writeSingleRegister(PuriMoveTubeModbus::START_POS_ADDR, start_pos);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(PuriMoveTubeModbus::START_INDEX_ADDR, start_index);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(PuriMoveTubeModbus::END_POS_ADDR, end_pos);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(PuriMoveTubeModbus::END_INDEX_ADDR, end_index);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(PuriMoveTubeModbus::START_ADDR, action_id);
    SPDLOG_ASSERT(rc == 1, "");
}

void PurificationModbusMachine::move_carrier(uint16_t start_pos, uint16_t end_pos,
                                             ActionId action_id) {
    subscribe((int)PuriMoveCarrierModbus::FINISH_ADDR, action_id, action_id);

    int rc;
    rc = client_.writeSingleRegister(PuriMoveCarrierModbus::START_POS_ADDR, start_pos);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(PuriMoveCarrierModbus::END_POS_ADDR, end_pos);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(PuriMoveCarrierModbus::START_ADDR, action_id);
    SPDLOG_ASSERT(rc == 1, "");
}

void PurificationModbusMachine::aspirate_mix(uint16_t pos, uint16_t index, uint16_t volume,
                                             uint16_t total, uint16_t num,
                                             uint16_t pipette_tr_index, uint16_t mix_speed,
                                             ActionId action_id) {
    SPDLOG_ASSERT(index < 96, "");
    SPDLOG_ASSERT(num <= 4, "");
    SPDLOG_ASSERT(pipette_tr_index < 4, "");
    SPDLOG_ASSERT(mix_speed <= 1000, "");
    SPDLOG_ASSERT(mix_speed >= 50, "");

    if (pipette_tr_index == (uint16_t)PipetteTrType::UL_50) {
        SPDLOG_ASSERT(volume <= 5000, "");
    } else if (pipette_tr_index == (uint16_t)PipetteTrType::UL_200) {
        SPDLOG_ASSERT(volume <= 20000, "");
    }

    subscribe((int)PuriAspirateMixModbus::FINISH_ADDR, action_id, action_id);

    int rc;
    rc = client_.writeSingleRegister(PuriAspirateMixModbus::POS_ADDR, pos);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(PuriAspirateMixModbus::INDEX_ADDR, index);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(PuriAspirateMixModbus::VOLUME_ADDR, volume);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(PuriAspirateMixModbus::TOTAL_ADDR, total);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(PuriAspirateMixModbus::NUM_ADDR, num);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(PuriAspirateMixModbus::TR_ADDR, pipette_tr_index);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(PuriAspirateMixModbus::MIX_SPEED_ADDR, mix_speed);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(PuriAspirateMixModbus::START_ADDR, action_id);
    SPDLOG_ASSERT(rc == 1, "");
}

void PurificationModbusMachine::shake(uint16_t duration, uint16_t speed, uint16_t temp,
                                      ActionId action_id) {
    subscribe((int)PuriShakeModbus::FINISH_ADDR, action_id, action_id);

    int rc;
    rc = client_.writeSingleRegister(PuriShakeModbus::DURATION_ADDR, duration);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(PuriShakeModbus::SPEED_ADDR, speed);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(PuriShakeModbus::TEMP_ADDR, temp);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(PuriShakeModbus::START_ADDR, action_id);
    SPDLOG_ASSERT(rc == 1, "");
}

void PurificationModbusMachine::centrifuge(uint16_t duration, uint16_t speed, ActionId action_id) {
    subscribe((int)PuriCentrifugeModbus::FINISH_ADDR, action_id, action_id);

    int rc;
    rc = client_.writeSingleRegister(PuriCentrifugeModbus::DURATION_ADDR, duration);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(PuriCentrifugeModbus::SPEED_ADDR, speed);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(PuriCentrifugeModbus::START_ADDR, action_id);
    SPDLOG_ASSERT(rc == 1, "");
}

void PurificationModbusMachine::time(uint32_t duration, ActionId action_id) {
    subscribe(0, duration / 1000, action_id);
    // subscribe((int)PuriTimeModbus::FINISH_ADDR, action_id, action_id);

    // uint16_t time_type = (uint16_t)PuriTimeModbus::TimeType::MILLISECOND;
    // int      rc;

    // if (duration > UINT16_MAX) {
    //     time_type = (uint16_t)PuriTimeModbus::TimeType::SECOND;
    //     duration /= 1000;

    //     if (duration > UINT16_MAX) {
    //         time_type = (uint16_t)PuriTimeModbus::TimeType::MINUTE;
    //         duration /= 60;
    //     }
    // }

    // rc = client_.writeSingleRegister(PuriTimeModbus::DURATION_ADDR, (uint16_t)duration);
    // SPDLOG_ASSERT(rc == 1, "");

    // rc = client_.writeSingleRegister(PuriTimeModbus::TYPE_ADDR, time_type);
    // SPDLOG_ASSERT(rc == 1, "");

    // rc = client_.writeSingleRegister(PuriTimeModbus::START_ADDR, action_id);
    // SPDLOG_ASSERT(rc == 1, "");
}

void PurificationModbusMachine::pcr(uint16_t file, ActionId action_id) {
    subscribe((int)PuriPcrModbus::FINISH_ADDR, action_id, action_id);

    int rc;
    rc = client_.writeSingleRegister(PuriPcrModbus::FILE_ADDR, file);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(PuriPcrModbus::START_ADDR, action_id);
    SPDLOG_ASSERT(rc == 1, "");
}