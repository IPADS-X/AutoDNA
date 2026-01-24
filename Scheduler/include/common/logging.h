#pragma once

#include "spdlog/sinks/basic_file_sink.h"
#include "spdlog/sinks/stdout_color_sinks.h"
#include "spdlog/spdlog.h"

#include <memory>

#define SPDLOG_ASSERT(cond, ...)                                                                 \
    if (!(cond)) {                                                                               \
        std::string full_path(__FILE__);                                                         \
        logger->error("Assertion failed({}): {} (file: {}:{})", #cond, fmt::format(__VA_ARGS__), \
                      full_path.substr(full_path.find_last_of("/\\") + 1), __LINE__);            \
        throw std::runtime_error(                                                                \
            fmt::format("Assertion failed: {}:{}:{}", __FILE__, __LINE__, #cond));               \
    }

extern std::shared_ptr<spdlog::sinks::stdout_color_sink_mt> console_sink;
extern std::shared_ptr<spdlog::sinks::basic_file_sink_mt>   file_sink;