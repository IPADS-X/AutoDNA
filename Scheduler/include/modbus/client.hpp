#pragma once

#include "modbus/common.hpp"

#include "common/dbug_logging.h"
#include "common/logging.h"

#include <map>
#include <modbus/modbus.h>
#include <mutex>
#include <thread>
#include <vector>

class ModbusClient {

public:
    ModbusClient(const char* ip, int port, int debug = 0) {
        logger->debug("ModbusClient connect to {}:{}", ip, port);

        uint32_t old_response_to_sec;
        uint32_t old_response_to_usec;
        uint32_t new_response_to_sec;
        uint32_t new_response_to_usec;

        ctx_ = modbus_new_tcp(ip, port);
        modbus_set_debug(ctx_, debug);
        modbus_set_error_recovery(ctx_,
                                  (modbus_error_recovery_mode)(MODBUS_ERROR_RECOVERY_LINK |
                                                               MODBUS_ERROR_RECOVERY_PROTOCOL));

        modbus_get_response_timeout(ctx_, &old_response_to_sec, &old_response_to_usec);
        if (modbus_connect(ctx_) == -1) {
            SPDLOG_ASSERT(false, "");
            modbus_free(ctx_);
            ctx_ = nullptr;
            return;
        }

        modbus_get_response_timeout(ctx_, &new_response_to_sec, &new_response_to_usec);
        ASSERT(old_response_to_sec == new_response_to_sec &&
               old_response_to_usec == new_response_to_usec);
    }

    ~ModbusClient() {
        if (ctx_ != nullptr) {
            modbus_close(ctx_);
            modbus_free(ctx_);
        }
    }

    int writeSingleCoil(int addr, int flag) {
        std::lock_guard<std::mutex> lock(mtx_);
        return modbus_write_bit(ctx_, addr, 1);
    }

    int writeMultipleCoils(int addr, int nb, const uint8_t* src) {
        std::lock_guard<std::mutex> lock(mtx_);
        return modbus_write_bits(ctx_, addr, nb, src);
    }

    int writeSingleRegister(int addr, const uint16_t value) {
        std::lock_guard<std::mutex> lock(mtx_);
        return modbus_write_register(ctx_, addr, value);
    }

    int writeMultipleRegisters(int addr, int nb, const uint16_t* src) {
        std::lock_guard<std::mutex> lock(mtx_);
        return modbus_write_registers(ctx_, addr, nb, src);
    }

    void waitUntilSingleRegisterEqual(int addr, uint16_t value) {
        logger->debug("wait until value of addr({}) == {}", addr, value);
        uint16_t dest, times = 0;
        do {
            std::this_thread::sleep_for(std::chrono::milliseconds(1000));
            int rc = readHoldingRegisters(addr, 1, &dest);
            logger->debug("loop times({}) has read value({})", times, dest);
            ASSERT(rc == 1);
            if (++times > 100) {
                ASSERT(false);
            }
        } while (dest != value);
    }

    int readCoils(int addr, int nb, uint8_t* dest) {
        std::lock_guard<std::mutex> lock(mtx_);
        return modbus_read_bits(ctx_, addr, nb, dest);
    }

    int readDiscreteInputs(int addr, int nb, uint8_t* dest) {
        std::lock_guard<std::mutex> lock(mtx_);
        return modbus_read_input_bits(ctx_, addr, nb, dest);
    }

    int readHoldingRegisters(int addr, int nb, uint16_t* dest) {
        std::lock_guard<std::mutex> lock(mtx_);
        return modbus_read_registers(ctx_, addr, nb, dest);
    }

    int writeAndReadRegisters(int write_addr, int write_nb, const uint16_t* src, int read_addr,
                              int read_nb, uint16_t* dest) {
        std::lock_guard<std::mutex> lock(mtx_);
        return modbus_write_and_read_registers(ctx_, write_addr, write_nb, src, read_addr, read_nb,
                                               dest);
    }

    int readInputRegisters(int addr, int nb, uint16_t* dest) {
        std::lock_guard<std::mutex> lock(mtx_);
        return modbus_read_input_registers(ctx_, addr, nb, dest);
    }

    static void initLogger(spdlog::level::level_enum lvl, spdlog::sinks_init_list sinks) {
        logger = std::make_shared<spdlog::logger>("MB_Client", sinks);
        logger->set_level(lvl);
    }

private:
    modbus_t*  ctx_ = nullptr;
    std::mutex mtx_;

    static std::shared_ptr<spdlog::logger> logger;
};
