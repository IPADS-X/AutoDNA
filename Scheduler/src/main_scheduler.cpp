#include <functional>
#include <iostream>

#include "common/thread_safe_queue.hpp"
#include "machine/mac_manager.hpp"
#include "procedure/workflow.hpp"
#include "server/scheduler.hpp"
#include "server/websocket_server.hpp"
#include "transform/transfer_manager.hpp"

#include "common/logging.h"

#include <chrono>
#include <iomanip>
#include <sstream>

std::shared_ptr<spdlog::sinks::stdout_color_sink_mt> console_sink = nullptr;
std::shared_ptr<spdlog::sinks::basic_file_sink_mt>   file_sink    = nullptr;

std::shared_ptr<MachineManager>          mac_manager = nullptr;
std::shared_ptr<ProductionLineScheduler> scheduler   = nullptr;

void setSpdLog() {

    console_sink = std::make_shared<spdlog::sinks::stdout_color_sink_mt>();
    console_sink->set_level(spdlog::level::debug);
    console_sink->set_pattern("[%H:%M:%S] [%-10n] %^[%5l] %v%$");

    // generate a filename with current date and timestamp
    auto              now       = std::chrono::system_clock::now();
    auto              in_time_t = std::chrono::system_clock::to_time_t(now);
    std::stringstream ss;
    ss << "logs/" << std::put_time(std::localtime(&in_time_t), "%Y-%m-%d_%H-%M-%S") << ".log";
    file_sink = std::make_shared<spdlog::sinks::basic_file_sink_mt>(ss.str(), true);
    file_sink->set_level(spdlog::level::debug);
    file_sink->set_pattern("[%H:%M:%S] [%-10n] [%5l] %v");

    spdlog::level::level_enum lvl = spdlog::level::debug;
    MachineManager::initLogger(lvl, {console_sink, file_sink});
    Step::initLogger(lvl, {console_sink, file_sink});
    Action::initLogger(lvl, {console_sink, file_sink});
    ModbusClient::initLogger(lvl, {console_sink, file_sink});
    Machine::initLogger(lvl, {console_sink, file_sink});
    ProductionLineScheduler::initLogger(lvl, {console_sink, file_sink});
    CheckManager::initLogger(lvl, {console_sink, file_sink});
    TransferManager::initLogger(lvl, {console_sink, file_sink});
    BlockTransformer::initLogger(lvl, {console_sink, file_sink});
}

int main(int argc, char* argv[]) {
    // get the step_id_skip_until from the argv if it is provided
    std::vector<StepId> step_id_skip_until = {};
    if (argc > 1) {
        std::string config_dir_path = argv[1];
        std::cout << "Using config dir path: " << config_dir_path << std::endl;
        ParserCode::changeConfigPath(config_dir_path);
    }

    setSpdLog();

    ThreadSafeQueue<std::shared_ptr<WebEvent>> web_recv_queue;
    ThreadSafeQueue<std::shared_ptr<WebEvent>> web_send_queue;
    ThreadSafeQueue<std::shared_ptr<MyEvent>>  event_queue;
    std::vector<std::thread>                   threads;

#ifdef WEB_MODE
    auto web_server = std::make_shared<WebSocketServer>(web_recv_queue, web_send_queue);
    threads.emplace_back([&web_server]() { web_server->start(); });
    std::cout << "WebSocket server started" << std::endl;
#elif defined(MERGED_WORKFLOW)
    web_recv_queue.push(std::make_shared<WebEvent>("merged_workflow4"));
    // web_recv_queue.push(std::make_shared<WebEvent>("merged_workflow8"));
#else
    // web_recv_queue.push(std::make_shared<WebEvent>("polyA"));
    // web_recv_queue.push(std::make_shared<WebEvent>("library"));
    // web_recv_queue.push(std::make_shared<WebEvent>("nucleic_acid"));
    // web_recv_queue.push(std::make_shared<WebEvent>("rna"));
#endif

    mac_manager = std::make_shared<MachineManager>(event_queue);
    threads.emplace_back([]() { mac_manager->start(); });

    scheduler = std::make_shared<ProductionLineScheduler>(mac_manager, web_recv_queue,
                                                          web_send_queue, step_id_skip_until);
    threads.emplace_back([]() { scheduler->start(); });

    // fix for signal handling
    signal(SIGINT, [](int) {
        mac_manager->stop();
        scheduler->stop();
    });

    // join threads
    for (auto& thread : threads) {
        thread.join();
    }

    return 0;
}
