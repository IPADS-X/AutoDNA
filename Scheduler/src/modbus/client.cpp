#include "modbus/client.hpp"

#include <vector>

std::shared_ptr<spdlog::logger> ModbusClient::logger =
    std::make_shared<spdlog::logger>("MB_Client", spdlog::sinks_init_list{console_sink, file_sink});