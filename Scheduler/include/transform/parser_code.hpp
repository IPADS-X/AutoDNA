#include "fstream"
#include "procedure/stage.hpp"
#include "procedure/workflow.hpp"
#include "process/dummy.hpp"
#include "reality/reality.hpp"

#include "transform/common.hpp"

#include "process/amplification.hpp"
#include "process/amplification/aspirate_mix.hpp"
#include "process/amplification/move_carrier.hpp"
#include "process/amplification/move_tube.hpp"
#include "process/amplification/pcr.hpp"
#include "process/branch.hpp"
#include "process/fluorescence.hpp"
#include "process/fluorescence/aspirate_mix.hpp"
#include "process/fluorescence/captube.hpp"
#include "process/fluorescence/fluo.hpp"
#include "process/fluorescence/move_carrier.hpp"
#include "process/fluorescence/move_tube.hpp"
#include "process/fluorescence/pipette.hpp"
#include "process/forloop.hpp"
#include "process/library/aspirate_mix.hpp"
#include "process/library/centrifuge.hpp"
#include "process/library/heater.hpp"
#include "process/library/move_carrier.hpp"
#include "process/library/move_tube.hpp"
#include "process/library/pipette.hpp"
#include "process/library/timer.hpp"
#include "process/portage.hpp"
#include "process/purification.hpp"
#include "process/purification/aspirate_mix.hpp"
#include "process/purification/centrifugal.hpp"
#include "process/purification/centrifuge.hpp"
#include "process/purification/move_carrier.hpp"
#include "process/purification/move_tube.hpp"
#include "process/purification/pcr.hpp"
#include "process/purification/pipette.hpp"
#include "process/purification/shake.hpp"
#include "process/purification/timer.hpp"
#include "process/refrigerator/picking.hpp"
#include "process/refrigerator/placement.hpp"

class ParserCode {
public:
    static inline std::string CONFIG_DIR_PATH =
        "/home/ethereal/vm5/code/RAG/productionLine/ProductionLineScheduler/config/";

    static inline std::string BASE_FILE_PATH = CONFIG_DIR_PATH + "protocol_flow_";

    static inline std::string DEFAULT_FILE_PATH = CONFIG_DIR_PATH + "protocol_flow.json";

    static inline std::string REAGENT_FILE_PATH = CONFIG_DIR_PATH + "reagents.json";

public:
    static const inline std::map<std::pair<std::string, TubeType>, Dummy::DummyType>
        dummy_type_map_ = {
            {{"pipette_move", TubeType::STRIP_TUBE}, Dummy::DummyType::PIPETTE},
            {{"pipette_move", TubeType::PCR_TUBE}, Dummy::DummyType::PIPETTE_PCR_TUBE},
            {{"pipette_move", TubeType::CHAMBER}, Dummy::DummyType::PIPETTE},
            // {{"robot_move_tube", TubeType::STRIP_TUBE}, Dummy::DummyType::MOVE_TUBE},
            // {{"robot_move_tube", TubeType::PCR_TUBE}, Dummy::DummyType::MOVE_PCR_TUBE},
            {{"robot_move_container", TubeType::STRIP_TUBE}, Dummy::DummyType::MOVE_TUBE},
            {{"robot_move_container", TubeType::PCR_TUBE}, Dummy::DummyType::MOVE_PCR_TUBE},
            {{"MOVE_CARRIER", TubeType::STRIP_TUBE}, Dummy::DummyType::MOVE_CARRIER},
            {{"heatershaker_start", TubeType::STRIP_TUBE}, Dummy::DummyType::SHAKE},
            {{"heatershaker_start", TubeType::PCR_TUBE}, Dummy::DummyType::SHAKE},
            {{"heatershaker_set_temp", TubeType::STRIP_TUBE},
             Dummy::DummyType::SET_HEATER_SHAKER_TEMP},
            {{"heatershaker_set_temp", TubeType::PCR_TUBE},
             Dummy::DummyType::SET_HEATER_SHAKER_TEMP},
            {{"heatershaker_set_speed", TubeType::STRIP_TUBE},
             Dummy::DummyType::SET_HEATER_SHAKER_SPEED},
            {{"heatershaker_set_speed", TubeType::PCR_TUBE},
             Dummy::DummyType::SET_HEATER_SHAKER_SPEED},
            {{"magrack_wait", TubeType::STRIP_TUBE}, Dummy::DummyType::TIME},
            {{"magrack_wait", TubeType::PCR_TUBE}, Dummy::DummyType::TIME},
            {{"timer_wait", TubeType::STRIP_TUBE}, Dummy::DummyType::TIME},
            {{"timer_wait", TubeType::PCR_TUBE}, Dummy::DummyType::TIME},
            {{"heater_start", TubeType::STRIP_TUBE}, Dummy::DummyType::HEATER},
            {{"heater_start", TubeType::PCR_TUBE}, Dummy::DummyType::HEATER},
            {{"heater_set_temp", TubeType::STRIP_TUBE}, Dummy::DummyType::SET_HEATER_TEMP},
            {{"heater_set_temp", TubeType::PCR_TUBE}, Dummy::DummyType::SET_HEATER_TEMP},
            {{"thermal_cycler_start_isothermal", TubeType::STRIP_TUBE}, Dummy::DummyType::PCR},
            {{"thermal_cycler_start_isothermal", TubeType::PCR_TUBE}, Dummy::DummyType::PCR},
            {{"thermal_cycler_run_program", TubeType::STRIP_TUBE}, Dummy::DummyType::PCR},
            {{"thermal_cycler_run_program", TubeType::PCR_TUBE}, Dummy::DummyType::PCR},
            {{"centrifuge_start", TubeType::STRIP_TUBE}, Dummy::DummyType::CENTRIFUGE},
            {{"centrifuge_start", TubeType::PCR_TUBE}, Dummy::DummyType::CENTRIFUGE_PCR_TUBE},
            {{"pipette_mix", TubeType::STRIP_TUBE}, Dummy::DummyType::ASPIRATE_MIX},
            {{"pipette_mix", TubeType::PCR_TUBE}, Dummy::DummyType::ASPIRATE_MIX},
            {{"MIX", TubeType::STRIP_TUBE}, Dummy::DummyType::MIX},
            {{"fluorometer_measure", TubeType::STRIP_TUBE}, Dummy::DummyType::FLUO},
            {{"fluorometer_measure", TubeType::PCR_TUBE}, Dummy::DummyType::FLUO},
            // {{"capper_cap_tube", TubeType::STRIP_TUBE}, Dummy::DummyType::CAP},
            // {{"capper_cap_tube", TubeType::PCR_TUBE}, Dummy::DummyType::CAP},
            {{"capper_cap_container", TubeType::STRIP_TUBE}, Dummy::DummyType::CAP},
            {{"capper_cap_container", TubeType::PCR_TUBE}, Dummy::DummyType::CAP},
            // {{"tube_allocate", TubeType::STRIP_TUBE}, Dummy::DummyType::ALLOC_TUBE},
            // {{"tube_allocate", TubeType::PCR_TUBE}, Dummy::DummyType::ALLOC_TUBE},
            // {{"tube_allocate", TubeType::CHAMBER}, Dummy::DummyType::ALLOC_TUBE},
            // {{"tube_get", TubeType::STRIP_TUBE}, Dummy::DummyType::GET_TUBE},
            // {{"tube_get", TubeType::PCR_TUBE}, Dummy::DummyType::GET_TUBE},
            // {{"tube_get", TubeType::CHAMBER}, Dummy::DummyType::GET_TUBE},
            {{"open_pcr_lid", TubeType::STRIP_TUBE}, Dummy::DummyType::OPEN_PCR_LID},
            {{"close_pcr_lid", TubeType::STRIP_TUBE}, Dummy::DummyType::CLOSE_PCR_LID},
            {{"open_port", TubeType::STRIP_TUBE}, Dummy::DummyType::OPEN_PORT},
            {{"open_port", TubeType::PCR_TUBE}, Dummy::DummyType::OPEN_PORT},
            {{"close_port", TubeType::STRIP_TUBE}, Dummy::DummyType::CLOSE_PORT},
            {{"close_port", TubeType::PCR_TUBE}, Dummy::DummyType::CLOSE_PORT},
            {{"container_allocate", TubeType::STRIP_TUBE}, Dummy::DummyType::ALLOC_TUBE},
            {{"container_allocate", TubeType::PCR_TUBE}, Dummy::DummyType::ALLOC_TUBE},
            {{"container_allocate", TubeType::CHAMBER}, Dummy::DummyType::ALLOC_TUBE},
            {{"container_get", TubeType::STRIP_TUBE}, Dummy::DummyType::GET_TUBE},
            {{"container_get", TubeType::PCR_TUBE}, Dummy::DummyType::GET_TUBE},
            {{"container_get", TubeType::CHAMBER}, Dummy::DummyType::GET_TUBE},
    };

    static const inline std::map<std::string, TubeType> tube_type_map_ = {
        {"P1500", TubeType::PCR_TUBE},   {"P200", TubeType::STRIP_TUBE},
        {"'P1500'", TubeType::PCR_TUBE}, {"'P200'", TubeType::STRIP_TUBE},
        {"P50K", TubeType::CHAMBER},     {"'P50K'", TubeType::CHAMBER}};

    // static const inline std::string TUBE_INDEX = "tube_index";
    // static const inline std::string TUBE_LABEL = "tube_label";
    // static const inline std::string TUBE_TYPE  = "tube_type";
    static const inline std::string TUBE_INDEX = "container_index";
    static const inline std::string TUBE_LABEL = "container_label";
    static const inline std::string TUBE_TYPE  = "container_type";
    static const inline std::string PARAMS     = "parameters";
    static const inline std::string ID         = "id";
    static const inline std::string ACTION     = "action";
    static const inline std::string WASTE_TUBE = "Waste Tube";

    static const inline std::map<std::string, Dummy::ParamType> param_type_map_ = {
        {TUBE_INDEX, Dummy::ParamType::TUBE_INDEX},
        {TUBE_LABEL, Dummy::ParamType::TUBE_NAME},
        // {"src_tube_index", Dummy::ParamType::SRC_TUBE_INDEX},
        // {"dst_tube_index", Dummy::ParamType::DEST_TUBE_INDEX},
        {"src_container_index", Dummy::ParamType::SRC_TUBE_INDEX},
        {"dst_container_index", Dummy::ParamType::DEST_TUBE_INDEX},
        {"volume", Dummy::ParamType::VOLUME},
        {"dst_pos", Dummy::ParamType::END_POSITION},
        {"pos", Dummy::ParamType::POSITION},
        {"time", Dummy::ParamType::DURATION},
        {"temp", Dummy::ParamType::TEMPERATURE},
        {"speed", Dummy::ParamType::MIX_SPEED},
        // {"tubes_on_machine", Dummy::ParamType::TUBE_LIST},
        {"containers_on_machine", Dummy::ParamType::TUBE_LIST}};

    static const inline std::map<std::string, Dummy::DummyEquipment> equipment_map_ = {
        {"magrack", Dummy::DummyEquipment::MAGNETIC_RACK},
        {"'magrack'", Dummy::DummyEquipment::MAGNETIC_RACK},
        {"heater_shaker", Dummy::DummyEquipment::HEATER_SHAKER},
        {"'heater_shaker'", Dummy::DummyEquipment::HEATER_SHAKER},
        {"heater", Dummy::DummyEquipment::METAL_BATH},
        {"'heater'", Dummy::DummyEquipment::METAL_BATH},
        {"centrifuge", Dummy::DummyEquipment::CENTRIFUGE},
        {"'centrifuge'", Dummy::DummyEquipment::CENTRIFUGE},
        {"fluorometer", Dummy::DummyEquipment::FLUORESCENCE},
        {"'fluorometer'", Dummy::DummyEquipment::FLUORESCENCE},
        {"thermal_cycler", Dummy::DummyEquipment::PCR},
        {"'thermal_cycler'", Dummy::DummyEquipment::PCR},
        {"capper", Dummy::DummyEquipment::CAPPER},
        {"'capper'", Dummy::DummyEquipment::CAPPER},
        // {"tube_holder", Dummy::DummyEquipment::REACTION_POS},
        // {"'tube_holder'", Dummy::DummyEquipment::REACTION_POS},
        {"container_holder", Dummy::DummyEquipment::REACTION_POS},
        {"'container_holder'", Dummy::DummyEquipment::REACTION_POS}};

public:
    static void changeConfigPath(std::string path) {
        if (!std::filesystem::exists(path)) {
            throw std::runtime_error("Config path not exists: " + path);
        }
        CONFIG_DIR_PATH = path;
        if (CONFIG_DIR_PATH.back() != '/') {
            CONFIG_DIR_PATH += '/';
        }
        BASE_FILE_PATH    = CONFIG_DIR_PATH + "protocol_flow_";
        DEFAULT_FILE_PATH = CONFIG_DIR_PATH + "protocol_flow.json";
        REAGENT_FILE_PATH = CONFIG_DIR_PATH + "reagents.json";
    }

    static std::vector<std::shared_ptr<Workflow>>
    transferParser(Reality& reality, std::shared_ptr<MachineManager> mac_manager_,
                   std::vector<std::shared_ptr<Workflow>> sources,
                   std::shared_ptr<spdlog::logger> logger, std::string workflow_name, int times) {
        if (ParserCode::logger_ == nullptr) {
            ParserCode::logger_ = logger;
        }

        // workflow_name = CommonTransformFunc::removeLastNumber(workflow_name);

        std::string file_path = ParserCode::BASE_FILE_PATH + workflow_name + ".json";

        if (!std::filesystem::exists(file_path)) {
            logger->warn("Workflow file not found: {}, using default", file_path);
            file_path = ParserCode::DEFAULT_FILE_PATH;
        }

        std::vector<std::shared_ptr<Workflow>> workflows;
        for (int i = 0; i < times; i++) {
            auto workflow =
                parseWorkflows(reality, workflow_name + "_" + std::to_string(i), file_path);
            if (workflow.size() > 0)
                workflows.push_back(workflow[0]);
        }
        return workflows;
    }

    static std::vector<std::pair<std::string, nlohmann::basic_json<>::value_type>>
    transferValue(Reality& reality, Dummy::DummyType type, Dummy::ParamType key,
                  nlohmann::basic_json<>::value_type                 value,
                  std::map<std::string, std::pair<TubeId, IndexId>>& tubeid_map) {
        if (key == Dummy::ParamType::TUBE_INDEX || key == Dummy::ParamType::SRC_TUBE_INDEX ||
            key == Dummy::ParamType::DEST_TUBE_INDEX) {
            // auto real_tube_id = tubeid_map.at(value);
            if (tubeid_map.find(std::to_string(value.get<int>())) == tubeid_map.end()) {
                return {{std::to_string(static_cast<int>(key)), value}};
            }
            auto tube = tubeid_map.at(std::to_string(value.get<int>()));
            std::vector<std::pair<std::string, nlohmann::basic_json<>::value_type>> results = {
                {std::to_string(static_cast<int>(key)), tube.first}};

            // refine tube type here
            auto tube_container = reality.getTube(tube.first);
            auto ttype          = TubeManager::getTubeType(tube_container);
            if (ttype == TubeType::PCR_TUBE) {
                results.push_back({std::to_string(static_cast<int>(Dummy::ParamType::TUBE_TYPE)),
                                   (TubeType)ttype});

                logger_->debug("refine tube id: {} with type: {}", tube.first,
                               magic_enum::enum_name((TubeType)ttype));
            }

            if (key == Dummy::ParamType::SRC_TUBE_INDEX) {
                results.push_back(
                    {std::to_string(static_cast<int>(Dummy::ParamType::START_INDEX)), tube.second});
            }
            if (key == Dummy::ParamType::DEST_TUBE_INDEX) {
                results.push_back(
                    {std::to_string(static_cast<int>(Dummy::ParamType::END_INDEX)), tube.second});
            }
            if (key == Dummy::ParamType::TUBE_INDEX) {
                results.push_back(
                    {std::to_string(static_cast<int>(Dummy::ParamType::INDEX)), tube.second});
            }
            return results;
        }

        if (key == Dummy::ParamType::END_POSITION || key == Dummy::ParamType::POSITION) {
            logger_->debug("Getting real position for value: {}", value);
            auto real_pos = equipment_map_.at(value);
            return {{std::to_string(static_cast<int>(key)), real_pos}};
        }

        if (key == Dummy::ParamType::VOLUME) {
            int real_volume = value.get<float>() * 100;
            return {{std::to_string(static_cast<int>(key)), real_volume}};
        }

        if (key == Dummy::ParamType::DURATION) {
            auto real_duration = std::stoi(std::to_string(value.get<int>())) * 1000;
            return {{std::to_string(static_cast<int>(key)), real_duration}};
        }

        if (key == Dummy::ParamType::TUBE_LIST) {
            std::vector<TubeId> real_tube_ids;
            for (const auto& tube : value) {
                real_tube_ids.push_back(tubeid_map.at(std::to_string(tube.get<int>())).first);
            }
            return {{std::to_string(static_cast<int>(key)), real_tube_ids}};
        }

        if (key == Dummy::ParamType::TEMPERATURE) {
            auto temp = std::stoi(std::to_string(value.get<int>()));
            return {{std::to_string(static_cast<int>(key)), temp}};
        }

        if (key == Dummy::ParamType::MIX_SPEED) {
            auto speed = std::stoi(std::to_string(value.get<int>()));
            return {{std::to_string(static_cast<int>(key)), speed}};
        }

        return {{std::to_string(static_cast<int>(key)), value}};
    }

    static bool allocTube(Reality& reality, Dummy::DummyType type, nlohmann::json params,
                          std::map<std::string, std::pair<TubeId, IndexId>>& tubeid_map) {
        auto python_id = std::to_string(params[TUBE_INDEX].get<int>());
        if (tubeid_map.find(python_id) != tubeid_map.end()) {
            return false;
        }
        TubeType  tube_type  = TubeType::STRIP_TUBE;
        TubeLabel tube_label = "";
        if (params.contains(TUBE_TYPE)) {
            // tube_type = params["tube_type"].get<TubeType>();
            tube_type = tube_type_map_.at(params[TUBE_TYPE]);
        }

        // we only support strip tubes and pcr tubes
        if (tube_type == TubeType::CHAMBER) {
            tube_type = TubeType::STRIP_TUBE;
        }

        if (params.contains(TUBE_LABEL)) {
            tube_label = params[TUBE_LABEL].get<TubeLabel>();
        }
        logger_->debug("Allocating tube with type: {}, label: {}", magic_enum::enum_name(tube_type),
                       tube_label);

        if (tube_label == WASTE_TUBE) {
            // if waste, just get it
            return getTube(reality, type, params, tubeid_map);
        }

        auto tube             = reality.createTube(tube_label, tube_type);
        tubeid_map[python_id] = {TubeManager::getTubeId(tube), 0};
        return true;
    }

    static bool getTube(Reality& reality, Dummy::DummyType type, nlohmann::json params,
                        std::map<std::string, std::pair<TubeId, IndexId>>& tubeid_map) {
        auto python_id = std::to_string(params[TUBE_INDEX].get<int>());
        // has alloced
        if (tubeid_map.find(python_id) != tubeid_map.end()) {
            return true;
        }
        TubeLabel tube_label = "";
        if (params.contains(TUBE_LABEL)) {
            tube_label = params[TUBE_LABEL].get<TubeLabel>();
        }
        auto                                       reagent = reality.getReagent(tube_label);
        std::pair<std::shared_ptr<Container>, int> tube_pair;
        if (reagent == nullptr) {
            logger_->info("Reagent not found for label: {}", tube_label);
            auto tubes = reality.getLabelTubes(tube_label);
            if (tubes.size() > 0) {
                tube_pair = {tubes[0], 0};
            } else {
                logger_->error("Tube not found for label: {}", tube_label);
                return false;
            }
        } else {
            tube_pair = TubeManager::getTubeByReagent(reagent);
        }
        // get a not used tubes
        auto tube_id          = TubeManager::getTubeId(tube_pair.first);
        tubeid_map[python_id] = {tube_id, tube_pair.second};

        logger_->debug("Getting tube with id: {}, label: {}", tube_id, tube_label);
        return true;
    }

    static std::vector<std::shared_ptr<Workflow>>
    parseWorkflows(Reality& reality, std::string workflow_name,
                   std::string file_path = ParserCode::DEFAULT_FILE_PATH) {
        // parse
        std::ifstream ifs(file_path);
        std::string   data((std::istreambuf_iterator<char>(ifs)), std::istreambuf_iterator<char>());
        nlohmann::json                         json = nlohmann::json::parse(data);
        std::vector<std::shared_ptr<Workflow>> workflows;

        auto workflow = std::make_shared<Workflow>(1);
        workflow->setName(workflow_name);
        nlohmann::json dummy_params_json = nlohmann::json::object();
        dummy_params_json[std::to_string(static_cast<int>(Dummy::ParamType::TUBE_TYPE))] =
            TubeType::UNKNOWN;
        auto start_stage = std::make_shared<DummyStage>("0", std::move(dummy_params_json),
                                                        Dummy::DummyType::UNKNOWN);
        auto prev_stage  = start_stage;

        // if it rewrite, the tube map should find from reality
        std::map<std::string, std::pair<TubeId, IndexId>> tubeid_map =
            reality.getWorkflowTubesRelation(workflow_name);

        std::map<Dummy::DummyEquipment, std::map<Dummy::ParamType, int>> equipment_params;

        equipment_params[Dummy::DummyEquipment::METAL_BATH] = {{Dummy::ParamType::TEMPERATURE, 25}};

        equipment_params[Dummy::DummyEquipment::HEATER_SHAKER] = {
            {Dummy::ParamType::TEMPERATURE, 37}, {Dummy::ParamType::MIX_SPEED, 1300}};

        for (const auto& step : json["steps"]) {
            auto     action    = std::string(step[ACTION]);
            TubeType tube_type = TubeType::STRIP_TUBE;
            if (step.contains(PARAMS) && step[PARAMS].contains(TUBE_TYPE) &&
                tube_type_map_.find(step[PARAMS][TUBE_TYPE]) != tube_type_map_.end()) {
                tube_type = tube_type_map_.at(step[PARAMS][TUBE_TYPE]);
            }

            auto type = dummy_type_map_.find(std::make_pair(action, tube_type));
            if (type == dummy_type_map_.end()) {
                // do nothing
                continue;
                // type = dummy_type_map_.find(std::make_pair("UNKNOWN", tube_type));
            }

            nlohmann::json new_params_json = nlohmann::json::object();
            new_params_json[std::to_string(static_cast<int>(Dummy::ParamType::TUBE_TYPE))] =
                tube_type;

            if (type->second == Dummy::DummyType::ALLOC_TUBE) {
                allocTube(reality, type->second, step[PARAMS], tubeid_map);
                continue;
            }

            if (type->second == Dummy::DummyType::GET_TUBE) {
                // make sure there are a tube
                if (!getTube(reality, type->second, step[PARAMS], tubeid_map)) {
                    throw std::runtime_error("Failed to get tube: " + step[PARAMS].dump());
                    // allocTube(reality, type->second, step["parameters"], tubeid_map);
                }
                continue;
            }

            if (step.contains(PARAMS) && step[PARAMS].is_object()) {
                for (const auto& param : step[PARAMS].items()) {
                    const std::string& key   = param.key();
                    const auto         value = param.value();

                    auto it = param_type_map_.find(key);

                    if (it != param_type_map_.end()) {
                        Dummy::ParamType param_type = it->second;
                        std::vector<std::pair<std::string, nlohmann::basic_json<>::value_type>>
                            value_transfer =
                                transferValue(reality, type->second, param_type, value, tubeid_map);
                        for (auto& [new_key, new_value] : value_transfer) {
                            new_params_json[new_key] = new_value;
                        }
                    }
                }
            }

            // refine type
            type = dummy_type_map_.find(std::make_pair(
                action,
                new_params_json[std::to_string(static_cast<int>(Dummy::ParamType::TUBE_TYPE))]
                    .get<TubeType>()));

            if (type->second == Dummy::DummyType::SET_HEATER_TEMP) {
                equipment_params[Dummy::DummyEquipment::METAL_BATH][Dummy::ParamType::TEMPERATURE] =
                    new_params_json[std::to_string(static_cast<int>(Dummy::ParamType::TEMPERATURE))]
                        .get<int>();
                continue;
            } else if (type->second == Dummy::DummyType::SET_HEATER_SHAKER_TEMP) {
                equipment_params[Dummy::DummyEquipment::HEATER_SHAKER]
                                [Dummy::ParamType::TEMPERATURE] =
                                    new_params_json[std::to_string(static_cast<int>(
                                                        Dummy::ParamType::TEMPERATURE))]
                                        .get<int>();
                continue;
            } else if (type->second == Dummy::DummyType::SET_HEATER_SHAKER_SPEED) {
                equipment_params[Dummy::DummyEquipment::HEATER_SHAKER]
                                [Dummy::ParamType::MIX_SPEED] =
                                    new_params_json[std::to_string(static_cast<int>(
                                                        Dummy::ParamType::MIX_SPEED))]
                                        .get<int>();
            }

            if (type->second == Dummy::DummyType::HEATER) {
                // set default temp if not set
                if (new_params_json.find(std::to_string(static_cast<int>(
                        Dummy::ParamType::TEMPERATURE))) == new_params_json.end()) {
                    new_params_json[std::to_string(
                        static_cast<int>(Dummy::ParamType::TEMPERATURE))] =
                        equipment_params[Dummy::DummyEquipment::METAL_BATH]
                                        [Dummy::ParamType::TEMPERATURE];
                }
            } else if (type->second == Dummy::DummyType::SHAKE) {
                // set default temp if not set
                if (new_params_json.find(std::to_string(static_cast<int>(
                        Dummy::ParamType::TEMPERATURE))) == new_params_json.end()) {
                    new_params_json[std::to_string(
                        static_cast<int>(Dummy::ParamType::TEMPERATURE))] =
                        equipment_params[Dummy::DummyEquipment::HEATER_SHAKER]
                                        [Dummy::ParamType::TEMPERATURE];
                }
                if (new_params_json.find(std::to_string(
                        static_cast<int>(Dummy::ParamType::MIX_SPEED))) == new_params_json.end()) {
                    new_params_json[std::to_string(static_cast<int>(Dummy::ParamType::MIX_SPEED))] =
                        equipment_params[Dummy::DummyEquipment::HEATER_SHAKER]
                                        [Dummy::ParamType::MIX_SPEED];
                }
            }

            auto stage = std::make_shared<DummyStage>(std::to_string(step[ID].get<int>()),
                                                      std::move(new_params_json), type->second);
            prev_stage->setNextStage(Dummy::kOutput, stage);
            prev_stage = stage;
        }

        start_stage->generateWorkflow(*workflow);

        workflows.push_back(workflow);

        logger_->debug("Parsed {} workflows from file: {} with {} steps", workflows.size(),
                       file_path, workflow->getSteps().size());

        reality.markWorkflowTubes(workflow_name, tubeid_map);

        return workflows;
    }

private:
    static inline std::shared_ptr<spdlog::logger> logger_;
};