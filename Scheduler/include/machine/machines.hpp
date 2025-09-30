#pragma once

#include "machine.hpp"

class AmplificationModbusMachine : public ModbusMachine {
public:
    AmplificationModbusMachine(const char* ip, int port)
        : ModbusMachine(
              MachineType::AMPLIFICATION, ip, port,
              {Equipment::build(EquipmentType::ROBOT_ARM),
               Equipment::build(EquipmentType::PIPETEE_GUN), Equipment::build(EquipmentType::PCR),
               Equipment::build(EquipmentType::ENTER_POS),
               Equipment::build(EquipmentType::EXIT_POS)},
              {(AreaId)AmplificationArea::SAMPLE_AREA, (AreaId)AmplificationArea::TUBE_AREA_01, (AreaId)AmplificationArea::TUBE_AREA_02}) {}

    void init();
    void amplification1(ActionId action_id);
    void amplification2(std::vector<uint16_t>& tubes, uint16_t volume, uint16_t volume2,
                        uint16_t pcr_file, uint16_t use_pipetting, ActionId action_id);
    void onAmplificationFinished(std::string sn, std::vector<uint16_t>& tubes);

    void move_carrier(uint16_t start_pos, uint16_t end_pos, ActionId action_id);

    void move_tube(uint16_t start_pos, uint16_t start_index, uint16_t end_pos, uint16_t end_index,
                   ActionId action_id);

    void aspirate_mix(uint16_t pos, uint16_t index, uint16_t volume, uint16_t total, uint16_t num,
                      uint16_t pipette_tr_index, uint16_t mix_speed, ActionId action_id);

    void pcr(uint16_t file, ActionId action_id);
};

class RefrigeratorModbusMachine : public ModbusMachine {
public:
    RefrigeratorModbusMachine(const char* ip, int port)
        : ModbusMachine(MachineType::REFRIGERATOR, ip, port) {}
};

class FluorescenceModbusMachine : public ModbusMachine {
public:
    FluorescenceModbusMachine(const char* ip, int port)
        : ModbusMachine(MachineType::FLUORESCENCE, ip, port,
                        {Equipment::build(EquipmentType::ROBOT_ARM),
                         Equipment::build(EquipmentType::PIPETEE_GUN),
                         Equipment::build(EquipmentType::FLUORESCENCE),
                         Equipment::build(EquipmentType::CAPPER),
                         Equipment::build(EquipmentType::ENTER_POS),
                         Equipment::build(EquipmentType::EXIT_POS)},
                        {(AreaId)FluorescenceArea::SAMPLE_AREA, (AreaId)FluorescenceArea::PCR_STRIP_SAMPLE_CONSUMABLE_AREA}) {}

    void init();
    void fluorescence1(ActionId action_id);
    void fluorescence2(std::vector<uint16_t>& tubes, uint16_t time, ActionId action_id);
    void onFluorescenceFinished(std::string sn, std::vector<float>& results);

    void move_carrier(uint16_t start_pos, uint16_t end_pos, ActionId action_id);

    void move_tube(uint16_t start_pos, uint16_t start_index, uint16_t end_pos, uint16_t end_index,
                   ActionId action_id);

    void pipette(uint16_t start_pos, uint16_t start_index, uint16_t volume, uint16_t end_pos,
                 uint16_t end_index, uint16_t num, uint16_t pipette_tr_index, uint16_t mix_time,
                 uint16_t mix_volume, uint16_t after_mix_time, uint16_t after_mix_volume,
                 uint16_t mix_speed, ActionId action_id);

    void capTubes(ActionId action_id);

    void start_read_fluorescence(ActionId action_id);

    void get_fluorescence_values(ActionId action_id, std::vector<float>& results);
};

class PurificationModbusMachine : public ModbusMachine {
public:
    PurificationModbusMachine(const char* ip, int port)
        : ModbusMachine(
              MachineType::PURIFICATION, ip, port,
              {Equipment::build(EquipmentType::ROBOT_ARM),
               Equipment::build(EquipmentType::PIPETEE_GUN),
               Equipment::build(EquipmentType::CENTRIFUGE),
               Equipment::build(EquipmentType::MAGNETIC_RACK, 3),
               Equipment::build(EquipmentType::HEATER_SHAKER),
               Equipment::build(EquipmentType::PCR),
               Equipment::build(EquipmentType::ENTER_POS),
               Equipment::build(EquipmentType::EXIT_POS), Equipment::build(EquipmentType::TIMER)},
              {(AreaId)PurificationArea::SAMPLE_AREA, (AreaId)PurificationArea::NORMAL_TEMP_AREA,
               (AreaId)PurificationArea::LOW_TEMP_AREA, (AreaId)PurificationArea::EMPTY_TUBE_AREA,
               (AreaId)PurificationArea::REACTION_AREA_02,
               (AreaId)PurificationArea::CLEANING_AREA}) {}

            //    {(AreaId)PurificationArea::SAMPLE_AREA

    void init();

    void centrifugal1(ActionId action_id);
    void centrifugal2(std::vector<uint16_t>& tubes, uint16_t speed, uint16_t duration,
                      ActionId action_id);
    void onCentrifugalFinished(std::string sn, std::vector<uint16_t>& tubes);

    void pipette(uint16_t start_pos, uint16_t start_index, uint16_t volume, uint16_t end_pos,
                 uint16_t end_index, uint16_t num, uint16_t pipette_tr_index, uint16_t mix_time,
                 uint16_t mix_volume, uint16_t after_mix_time, uint16_t after_mix_volume,
                 uint16_t mix_speed, ActionId action_id);

    void move_tube(uint16_t start_pos, uint16_t start_index, uint16_t end_pos, uint16_t end_index,
                   ActionId action_id);

    void move_carrier(uint16_t start_pos, uint16_t end_pos, ActionId action_id);

    void aspirate_mix(uint16_t pos, uint16_t index, uint16_t volume, uint16_t total, uint16_t num,
                      uint16_t pipette_tr_index, uint16_t mix_speed, ActionId action_id);

    void shake(uint16_t duration, uint16_t speed, uint16_t temp, ActionId action_id);

    void centrifuge(uint16_t duration, uint16_t speed, ActionId action_id);

    void pcr(uint16_t file, ActionId action_id);

    void time(uint32_t duration, ActionId action_id);
};

class LibraryModbusMachine : public ModbusMachine {
public:
    LibraryModbusMachine(const char* ip, int port)
        : ModbusMachine(
              MachineType::LIBRARY, ip, port,
              {Equipment::build(EquipmentType::ROBOT_ARM),
               Equipment::build(EquipmentType::PIPETEE_GUN),
               Equipment::build(EquipmentType::CENTRIFUGE),
               Equipment::build(EquipmentType::CENTRIFUGE_PCR),
               Equipment::build(EquipmentType::MAGNETIC_RACK, 4),
               Equipment::build(EquipmentType::METAL_BATH, 4),
               Equipment::build(EquipmentType::MIXER), Equipment::build(EquipmentType::ENTER_POS),
               Equipment::build(EquipmentType::EXIT_POS), Equipment::build(EquipmentType::TIMER)},
              {(AreaId)LibraryArea::NORMAL_TEMP_REAGENT_AREA, (AreaId)LibraryArea::PCR_STRIP_CONSUMABLE_AREA, (AreaId)LibraryArea::SAMPLE_AREA, (AreaId)LibraryArea::COLD_LABEL_AREA, (AreaId)LibraryArea::COLD_REAGENT_AREA}) {}

    void init();

    void pipette(uint16_t start_pos, uint16_t start_index, uint16_t volume, uint16_t end_pos,
                 uint16_t end_index, uint16_t num, uint16_t pipette_tr_index, uint16_t mix_time,
                 uint16_t mix_volume, uint16_t after_mix_time, uint16_t after_mix_volume,
                 uint16_t mix_speed, ActionId action_id);

    void move_tube(uint16_t start_pos, uint16_t start_index, uint16_t end_pos, uint16_t end_index,
                   ActionId action_id);

    void move_pcr_tube(uint16_t start_pos, uint16_t start_index, uint16_t end_pos,
                       uint16_t end_index, ActionId action_id);

    void move_carrier(uint16_t start_pos, uint16_t end_pos, ActionId action_id);

    void centrifuge_8_tube(uint16_t duration, uint16_t speed, ActionId action_id);

    void centrifuge_pcr_tube(uint16_t duration, uint16_t speed, ActionId action_id);

    void centrifuge_mix_8_tube(uint16_t duration, uint16_t speed, ActionId action_id);

    void time(uint32_t duration, ActionId action_id);

    void heater(uint32_t duration, uint16_t temperature, ActionId action_id);

    void aspirate_mix(uint16_t pos, uint16_t index, uint16_t volume, uint16_t total, uint16_t num,
                      uint16_t pipette_tr_index, uint16_t mix_speed, ActionId action_id);
};

class PortageModbusMachine : public ModbusMachine {
public:
    PortageModbusMachine(const char* ip, int port)
        : ModbusMachine(MachineType::PORTAGE, ip, port, {Equipment::build(EquipmentType::REALM)},
                        {}) {}

    void init();
    void autoPortage(uint16_t from, uint16_t to, ActionId action_id);
    void manualPortage(uint16_t to, const std::string sn, ActionId action_id);
    void onPortageFinished();
};

template <typename T>
inline static void reduce_vector_zero_part(std::vector<T>& vec) {
    int old_size = vec.size();
    for (int i = vec.size() - 1; i >= 0; --i) {
        if (vec[i] != 0) {
            vec.resize(i + 1);
            return;
        }
    }
    vec.resize(0);
}