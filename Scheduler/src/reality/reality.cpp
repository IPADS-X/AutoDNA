#include "reality/reality.hpp"
#include "fstream"
#include "machine/mac_manager.hpp"
#include "nlohmann/json.hpp"
#include "reality/carrier/chamber_tube_carrier.hpp"
#include "reality/carrier/pcr_tube_carrier.hpp"
#include "reality/carrier/strip_tube_carrier.hpp"

#include "transform/parser_code.hpp"

void Reality::init(std::shared_ptr<MachineManager> mac_manager) {
    // loading from file

    std::ifstream  ifs(ParserCode::REAGENT_FILE_PATH);
    std::string    data((std::istreambuf_iterator<char>(ifs)), std::istreambuf_iterator<char>());
    nlohmann::json json = nlohmann::json::parse(data);

    // Parse the JSON and initialize the reality
    std::vector<std::shared_ptr<Carrier>> consumer_carriers;
    for (const auto& carrier : json["carriers"]) {
        std::string              name = carrier["name"];
        std::shared_ptr<Carrier> carrier_ptr;
        if (carrier["type"] == "STRIP_TUBE") {
            carrier_ptr = getOrCreateCarrier<StripTubeCarrier>(name);
        } else if (carrier["type"] == "PCR_TUBE") {
            carrier_ptr = getOrCreateCarrier<PcrTubeCarrier>(name);
        } else {
            carrier_ptr = getOrCreateCarrier<ChamberTubeCarrier>(name);
        }
        carrier_ptr->setMachineType(
            (MachineTypeId)MachineManager::toMachineType(carrier["machine"]));
        carrier_ptr->setAreaId((AreaId)MachineManager::toAreaId(
            MachineManager::toMachineType(carrier["machine"]), carrier["machineArea"]));

        if (carrier.find("is_consume") != carrier.end()) {
            carrier_ptr->setIsConsumer(carrier["is_consume"]);
            consumer_carriers.push_back(carrier_ptr);
        }

        mac_manager
            ->getMachine<Machine>((MachineTypeId)MachineManager::toMachineType(carrier["machine"]))
            ->allocPlaceArea(
                (AreaId)MachineManager::toAreaId(MachineManager::toMachineType(carrier["machine"]),
                                                 carrier["machineArea"]),
                carrier_ptr);
    }

    // set a waste carrier
    auto waste_carrier = getOrCreateCarrier<ChamberTubeCarrier>("WASTE_CARRIER");
    waste_carrier->setMachineType((MachineTypeId)MachineType::TOTAL_NUM);
    waste_carrier->setAreaId((AreaId)CommonAreaId::WASTE_AREA);

    // Parse the JSON and initialize the reality
    for (const auto& tube : json["tubes"]) {
        std::string name         = tube["name"];
        std::string label        = tube["label"];
        auto        machine_type = MachineManager::toMachineType(tube["machine"]);
        auto        area_id      = MachineManager::toAreaId(machine_type, tube["machineArea"]);
        // createTube(tube["label"], tube["type"], name);
        auto tube_type = TubeManager::toTubeType(tube["type"]);

        auto tube_ptr = tube_manager_.allocTube(label, name, tube_type);

        bool is_reagent = true;

        if (tube.find("is_reagent") != tube.end()) {
            is_reagent = tube["is_reagent"];
        }

        TubeManager::setTubeIsReagent(tube_ptr, is_reagent);

        std::shared_ptr<Carrier> carrier;
        auto                     machine = mac_manager->getMachine<Machine>(machine_type);
        if (machine) {
            carrier = machine->getCarrier(area_id);
        }
        if (carrier) {
            std::cout << "Allocating tube: " << tube["label"]
                      << " to carrier: " << carrier->getName() << std::endl;
            carrier->forceAllocTubeIndex(tube_ptr.get(), tube["AreaIndex"]);
            TubeManager::setTubeCarrier(tube_ptr, carrier);
            setTubePosition(TubeManager::getTubeId(tube_ptr),
                            {machine_type, area_id, tube["AreaIndex"]},
                            TubePositionType::WHOLE_TUBE);
        }
    }

    // set a waste tube
    auto waste_tube = tube_manager_.allocTube(ParserCode::WASTE_TUBE, ParserCode::WASTE_TUBE, TubeType::CHAMBER);
    waste_carrier->setTube(TubeManager::toChamberTube(waste_tube).get());
    TubeManager::setTubeCarrier(waste_tube, waste_carrier);
    setTubePosition(TubeManager::getTubeId(waste_tube),
                    {MachineType::TOTAL_NUM, (AreaId)CommonAreaId::WASTE_AREA, 0},
                    TubePositionType::WHOLE_TUBE);

    // Parse the JSON and initialize the reality
    for (const auto& reagent : json["reagents"]) {
        std::string name      = reagent["reagent"];
        std::string tube_name = reagent["tube"];
        int         volume    = reagent["volume"];
        volume                = volume * 100;
        int tubeIndex         = reagent["tube_index"];
        int num               = 1;
        if (reagent.find("num") != reagent.end()) {
            num = reagent["num"];
        }
        TubeType tube_type = TubeManager::toTubeType(reagent["tube_type"]);
        // addReagent(name, tubeIndex);

        auto reagent_ptr = getOrCreateReagent(name);
        // reagent_ptr->setVolume(volume);
        auto tube = tube_manager_.getTube(tube_name, tube_type);
        reagent_ptr->setContainer(tube);

        auto tube_id = TubeManager::getTubeId(tube);

        std::cout << "Registering reagent: " << name << " in tube: " << tube_name
                  << "tube_id: " << tube_id << " with volume: " << volume << " of num: " << num
                  << std::endl;

        for (int i = 0; i < num; i++) {
            tube->setVolume(volume, tubeIndex + i);
            tube->forceSetReagentIndex(reagent_ptr.get(), tubeIndex + i);
        }
    }

    
    // set a waste reagent
    auto waste_reagent = getOrCreateReagent(ParserCode::WASTE_TUBE);
    waste_reagent->setContainer(waste_tube);
    waste_tube->setVolume(0, 0);
    waste_tube->forceSetReagentIndex(waste_reagent.get(), 0);

    for (auto& carrier : consumer_carriers) {
        carrier->markOtherIndexConsumer();
    }
}

bool Reality::renewConsumer(std::string carrier_name, MachineTypeId machine_type, AreaId area_id,
                            std::shared_ptr<MachineManager> mac_manager) {
    auto carrier = getCarrier(carrier_name);
    if (carrier) {
        for (auto tube : carrier->getContainers()) {
            mac_manager->removeTube(tube);
        }

        carrier->renewConsumer(machine_type, area_id);
        if (machine_type == (MachineTypeId)MachineType::TOTAL_NUM ||
            area_id == (AreaId)CommonAreaId::NOT_SUCCESS) {
            return false;
        }
        mac_manager->getMachine<Machine>((MachineTypeId)machine_type)
            ->allocPlaceArea(area_id, carrier);
        return true;
    }
    return false;
}

bool Reality::renewReagents(std::string reagent_name, int volume) {
    auto reagent = getReagent(reagent_name);
    if (reagent) {
        auto tube = reagent->getContainer();
        if (tube) {
            tube->setAllReagentVolume(reagent, volume);
            return true;
        }
    }
    return false;
}