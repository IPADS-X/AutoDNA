#include "machine/machines.hpp"

void AmplificationModbusMachine::init() {
    logger->debug("AmplificationModbusMachine init");
    int rc;
    rc = client_.writeSingleRegister(AmplificationModbus::START_ADDR, 0);
    SPDLOG_ASSERT(rc == 1, "");
    rc = client_.writeSingleRegister(AmplificationModbus::FINISH_ADDR, 0);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(AmpMoveCarrierModbus::START_ADDR, 0);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(AmpAspirateMixModbus::START_ADDR, 0);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(AmpMoveTubeModbus::START_ADDR, 0);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(AmpPcrModbus::START_ADDR, 0);
    SPDLOG_ASSERT(rc == 1, "");

    ModbusMachine::init();
}

void AmplificationModbusMachine::amplification1(ActionId action_id) {
    subscribe((int)AmplificationModbus::READY_ADDR, 1, action_id);

    int rc = client_.writeSingleRegister(AmplificationModbus::START_ADDR, action_id);
    SPDLOG_ASSERT(rc == 1, "");
}

void AmplificationModbusMachine::amplification2(std::vector<uint16_t>& tubes, uint16_t volume,
                                                uint16_t volume2, uint16_t pcr_file,
                                                uint16_t use_pipetting, ActionId action_id) {
    subscribe((int)AmplificationModbus::FINISH_ADDR, action_id, action_id);

    ASSERT(tubes.size() <= TUBE_LEN);

    int      rc;
    uint16_t sn[SN_LEN];
    memset(sn, 0, sizeof(sn));

    // the machine requires me to do some weird stuff
    rc = client_.writeSingleRegister(AmplificationModbus::READY_ADDR, 0);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.readHoldingRegisters(AmplificationModbus::READY_SN_ADDR, SN_LEN, sn);
    ASSERT(rc == SN_LEN);

    rc = client_.writeMultipleRegisters(AmplificationModbus::NOTIFY_SN_ADDR, SN_LEN, sn);
    ASSERT(rc == SN_LEN);

    rc = client_.writeMultipleRegisters(AmplificationModbus::NOTIFY_TUBE_ADDR, tubes.size(),
                                        tubes.data());
    ASSERT(rc == tubes.size());

    rc = client_.writeSingleRegister(AmplificationModbus::NOTIFY_VOLUME_ADDR, volume);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(AmplificationModbus::NOTIFY_PCR_FILE_ADDR, pcr_file);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(AmplificationModbus::NOTIFY_PIPETTING_ADDR, use_pipetting);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(AmplificationModbus::NOTIFY_VOLUME2_ADDR, volume2);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(AmplificationModbus::NOTIFY_ADDR, 1);
    SPDLOG_ASSERT(rc == 1, "");
}

void AmplificationModbusMachine::onAmplificationFinished(std::string            sn,
                                                         std::vector<uint16_t>& tubes) {
    int     rc;
    uint8_t buf[SN_LEN * 2];
    memset(buf, 0, sizeof(buf));
    tubes.resize(TUBE_LEN);

    uint16_t dest;
    rc = client_.readHoldingRegisters(AmplificationModbus::FINISH_STATUS_ADDR, 1, &dest);
    SPDLOG_ASSERT(rc == 1, "");
    // ASSERT(dest == 1);

    rc = client_.readHoldingRegisters(AmplificationModbus::FINISH_OLD_SN_ADDR, SN_LEN,
                                      (uint16_t*)buf);
    ASSERT(rc == SN_LEN);

    rc = client_.writeMultipleRegisters(AmplificationModbus::FINISH_NEW_SN_ADDR, SN_LEN,
                                        (uint16_t*)buf);
    ASSERT(rc == SN_LEN);

    rc =
        client_.readHoldingRegisters(AmplificationModbus::FINISH_TUBE_ADDR, TUBE_LEN, tubes.data());
    ASSERT(rc == TUBE_LEN);

    rc = client_.writeMultipleRegisters(AmplificationModbus::FINISH_CONFIRM_SN_ADDR, SN_LEN,
                                        (uint16_t*)buf);
    ASSERT(rc == SN_LEN);

    rc = client_.writeSingleRegister(AmplificationModbus::FINISH_CONFIRM_ADDR, 1);
    SPDLOG_ASSERT(rc == 1, "");

    sn = std::string((char*)buf);

    // resize tubes to the last non-zero element
    reduce_vector_zero_part(tubes);
}

void AmplificationModbusMachine::move_carrier(uint16_t start_pos, uint16_t end_pos,
                                              ActionId action_id) {
    subscribe((int)AmpMoveCarrierModbus::FINISH_ADDR, action_id, action_id);

    int rc;
    rc = client_.writeSingleRegister(AmpMoveCarrierModbus::START_POS_ADDR, start_pos);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(AmpMoveCarrierModbus::END_POS_ADDR, end_pos);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(AmpMoveCarrierModbus::START_ADDR, action_id);
    SPDLOG_ASSERT(rc == 1, "");
}

void AmplificationModbusMachine::move_tube(uint16_t start_pos, uint16_t start_index,
                                           uint16_t end_pos, uint16_t end_index,
                                           ActionId action_id) {
    subscribe((int)AmpMoveTubeModbus::FINISH_ADDR, action_id, action_id);

    int rc;
    rc = client_.writeSingleRegister(AmpMoveTubeModbus::START_POS_ADDR, start_pos);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(AmpMoveTubeModbus::START_INDEX_ADDR, start_index);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(AmpMoveTubeModbus::END_POS_ADDR, end_pos);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(AmpMoveTubeModbus::END_INDEX_ADDR, end_index);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(AmpMoveTubeModbus::START_ADDR, action_id);
    SPDLOG_ASSERT(rc == 1, "");
}

void AmplificationModbusMachine::aspirate_mix(uint16_t pos, uint16_t index, uint16_t volume,
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

    subscribe((int)AmpAspirateMixModbus::FINISH_ADDR, action_id, action_id);

    int rc;
    rc = client_.writeSingleRegister(AmpAspirateMixModbus::POS_ADDR, pos);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(AmpAspirateMixModbus::INDEX_ADDR, index);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(AmpAspirateMixModbus::VOLUME_ADDR, volume);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(AmpAspirateMixModbus::TOTAL_ADDR, total);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(AmpAspirateMixModbus::NUM_ADDR, num);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(AmpAspirateMixModbus::TR_ADDR, pipette_tr_index);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(AmpAspirateMixModbus::MIX_SPEED_ADDR, mix_speed);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(AmpAspirateMixModbus::START_ADDR, action_id);
    SPDLOG_ASSERT(rc == 1, "");
}

void AmplificationModbusMachine::pcr(uint16_t file, ActionId action_id) {
    subscribe((int)AmpPcrModbus::FINISH_ADDR, action_id, action_id);

    int rc;
    rc = client_.writeSingleRegister(AmpPcrModbus::FILE_ADDR, file);
    SPDLOG_ASSERT(rc == 1, "");

    rc = client_.writeSingleRegister(AmpPcrModbus::START_ADDR, action_id);
    SPDLOG_ASSERT(rc == 1, "");
}