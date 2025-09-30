#pragma once

#include <cstdint>
#include <map>
#include <string>

using MachineTypeId = uint16_t;
enum class MachineType : MachineTypeId {
    AMPLIFICATION = 0,
    FLUORESCENCE,
    PURIFICATION,
    REFRIGERATOR,
    PORTAGE,
    LIBRARY,
    TOTAL_NUM
};

inline static const std::string MachineNames[] = {
    "amplification", "fluorescence", "purification", "refrigerator", "portage", "library", "none"};

inline static const uint16_t MachinePortageIds[] = {3, 4, 5, 11, 0, 6, 0};

using EquipmentId = uint16_t;
enum class EquipmentType : EquipmentId {
    PCR            = 0,
    METAL_BATH     = 1,
    PIPETEE_GUN    = 2,
    CENTRIFUGE     = 3,
    ROBOT_ARM      = 4,
    FLUORESCENCE   = 5,
    HEATER_SHAKER  = 6,
    REALM          = 7,
    CAPPER         = 8,
    MAGNETIC_RACK  = 9,
    MIXER          = 10,
    CENTRIFUGE_PCR = 11,
    TIMER          = 12,
    // other virtual equipments
    ENTER_POS    = 13,
    EXIT_POS     = 14,
    REACTION_POS = 15
};

using IndexId = int16_t;
enum class CommonIndexId : IndexId {
    NOT_SUCCESS = -1,
    AUTO        = 100,
};

using VolumeId = int16_t;
enum class CommonVolumeId : VolumeId {
    NOT_SUCCESS = -1,
    AUTO        = 1000,
    AUTO_1_1    = 1001, // all volume
    AUTO_1_2    = 1002, // 1/2
    AUTO_1_3    = 1003, // 1/3
    AUTO_1_4    = 1004, // 1/4
    AUTO_1_5    = 1005, // 1/5
    AUTO_1_6    = 1006, // 1/6
    AUTO_1_7    = 1007, // 1/7
    AUTO_1_8    = 1008  // 1/8
};

using AreaId = int16_t;
enum class CommonAreaId : AreaId {
    NOT_SUCCESS = -1,
    REACTION    = 100,
    AUTO        = 101,
    ENTER_AREA  = 102,
    EXIT_AREA   = 103,
    WASTE_AREA   = 104,
};

enum class PurificationArea : AreaId {
    EMPTY_TUBE_AREA          = 0,
    REACTION_AREA_02         = 1,
    SAMPLE_AREA              = 2,
    MAGNETIC_AREA            = 3,
    SHAKING_AREA             = 4,
    WASTE_AREA               = 5,
    CLEANING_AREA            = 6,
    NORMAL_TEMP_AREA         = 7,
    LOW_TEMP_AREA            = 8,
    THERMAL_CYCLING_AREA     = 9,
    THERMAL_CYCLING_CAP_AREA = 10,
    CENTRIFUGE_AREA          = 11,
    ENTER_AREA               = 12,
    EXIT_AREA                = 13,
    AUTO                     = 14
};

enum class LibraryArea : AreaId {
    WASTE_AREA                   = 0,  // 废料区
    MIXER_AREA                   = 1,  // 混匀仪区
    MAGNETIC_RACK_AREA           = 2,  // 磁力架区
    HIGH_TEMP_AREA               = 3,  // 高温区
    TUBE_CENTRIFUGE_AREA         = 4,  // 八连排离心区
    PCR_STRIP_CENTRIFUGE_AREA    = 5,  // 试管离心区
    COLD_LABEL_AREA              = 6,  // 零度标签区
    SAMPLE_AREA                  = 7,  // 样本区
    FLUOROMETER_AREA             = 8,  // 荧光计区
    FLUORESCENCE_CAP_AREA        = 9,  // 荧光盖区
    NORMAL_TEMP_REAGENT_AREA     = 10, // 常温试剂区
    COLD_REAGENT_AREA            = 11, // 零度试剂区
    TUBE_CONSUMABLE_AREA         = 12, // 八连排耗材区
    PCR_STRIP_CONSUMABLE_AREA    = 13, // 试管耗材区
    NORMAL_TEMP_REAGENT_LID_AREA = 14, // 常温试剂盖板区
    SMALL_CARRIER_INLET_AREA     = 15, // 小载具进料区
    SMALL_CARRIER_OUTLET_AREA    = 16, // 小载具出料区
    LARGE_CARRIER_INLET_AREA     = 17, // 大载具进料区
    LARGE_CARRIER_OUTLET_AREA    = 18, // 大载具出料区
    ENTER_AREA                   = 19, // 进料区
    EXIT_AREA                    = 20, // 出料区
    AUTO                         = 21, // 自动分配区
};

enum class FluorescenceArea : AreaId {
    WASTE_AREA                         = 0,  // 废料区
    COLD_REAGENT_AREA                  = 1,  // 零度试剂区
    SAMPLE_AREA                        = 2,  // 样本区
    FLUOROMETER_LID_AREA               = 3,  // 荧光仪盖板区
    FLUOROMETER_AREA                   = 4,  // 荧光仪区
    PCR_STRIP_SAMPLE_CONSUMABLE_AREA   = 5,  // 八连排样本耗材区
    PCR_STRIP_STANDARD_CONSUMABLE_AREA = 6,  // 八连排标准耗材区
    CAPPING_CONSUMABLE_AREA            = 7,  // 盖盖耗材区
    CAPPER_AREA                        = 8,  // 盖盖仪区
    CAPPER_LID_PLACEMENT_AREA          = 9,  // 盖盖仪放盖子区
    ENTER_AREA                         = 10, // 进料区
    EXIT_AREA                          = 11, // 出料区
    AUTO                               = 12,
};

enum class AmplificationArea : AreaId {
    WASTE_AREA           = 0, // 废料区
    HIGH_TEMP_AREA       = 1, // 高温区
    LOW_TEMP_AREA        = 2, // 低温区
    SAMPLE_AREA          = 3, // 样本区
    TUBE_AREA_01         = 4, // 试管01区
    THERMAL_CYCLING_AREA = 5, // 热循环区
    NORMAL_TEMP_AREA     = 6, // 常温区
    TUBE_AREA_02         = 7, // 试管02区
    ENTER_AREA           = 8, // 进料区
    EXIT_AREA            = 9, // 出料区
    AUTO                 = 10,
};

using IngredientId = uint16_t;
enum class IngredientType : IngredientId {
    MAGNETIC_BEAD = 0,
    BW_BUFFER,
    BW_BUFFER_X2,
    PB_BUFFER,
    IDNA,
    WATER,
    DNTP_A,
    DNTP_C,
    DNTP_G,
    DNTP_T,
    TDT,
    PB_BUFFER_X10,
    COCL2_X10,
    TWEEN,
    CB_BUFFER,
    MIX_BUFFER,
    PEG_BIOTIN,
    BIOTIN,
    TWEEEN,
    LYSIS_BUFFER,
    T7_X10,
    DNTP_U,
    DTT,
    DNA_TEMPLATE,
    DNASE,
    MAGBINDING_BUFFER,
    MAGBINDING_BEADS,
    ISOPROPANOL,
    RNA_PREP_BUFFER,
    ETHANOL,
    POLYA_WASH_BUFFER,
    POLYA_MIX_BUFFER_A,
    POLYA_MIX_BUFFER_T,
    RPA_SAMPLE,
    RPA_INITIALIZER,
    LIBRARY_DNA_REPAIR_BUFFER,
    LIBRARY_DNA_REPAIR_MIX,
    LIBRARY_END_PREP_BUFFER,
    LIBRARY_END_PREP_ENZYME,
    LIBRARY_BARCODE,
    LIBRARY_BT_MIX,
    LIBRARY_EDTA,
    LIBRARY_NATIVE_ADAPTER,
    LIBRARY_LNB,
    LIBRARY_QT4,
    LIBRARY_EB,
    LIBRARY_SFB,
    SAMPLE,
    // sequencing
    PHANTA_MAX_MASTER_MIX,
    LONG_IDNA_FORWARD_PRIMER,
    LONG_IDNA_REVERSE_PRIMER_TG
};

const std::map<std::string, IngredientType> IngredientTypeNameMap = {
    {"MAGNETIC_BEAD", IngredientType::MAGNETIC_BEAD},
    {"BW_BUFFER", IngredientType::BW_BUFFER},
    {"BW_BUFFER_X2", IngredientType::BW_BUFFER_X2},
    {"PB_BUFFER", IngredientType::PB_BUFFER},
    {"IDNA", IngredientType::IDNA},
    {"WATER", IngredientType::WATER},
    {"A", IngredientType::DNTP_A},
    {"C", IngredientType::DNTP_C},
    {"G", IngredientType::DNTP_G},
    {"T", IngredientType::DNTP_T},
    {"TDT", IngredientType::TDT},
    {"PB_BUFFER_X10", IngredientType::PB_BUFFER_X10},
    {"COCL2_X10", IngredientType::COCL2_X10},
    {"TWEEN", IngredientType::TWEEN},
    {"CB_BUFFER", IngredientType::CB_BUFFER},
    {"MIX_BUFFER", IngredientType::MIX_BUFFER},
    {"PEG_BIOTIN", IngredientType::PEG_BIOTIN},
    {"BIOTIN", IngredientType::BIOTIN},
};

using PipetteTrIndex = uint16_t;
enum class PipetteTrType : PipetteTrIndex {
    UL_50  = 0,
    UL_200 = 2,
};

using TubeTypeId = uint16_t;
enum class TubeType : TubeTypeId { PCR_TUBE, STRIP_TUBE, CHAMBER, UNKNOWN };

class Location {};