/*
 * Copyright © Stéphane Raimbault <stephane.raimbault@gmail.com>
 *
 * SPDX-License-Identifier: BSD-3-Clause
 */

#include <errno.h>
#include <map>
#include <signal.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <unistd.h>

#include <modbus/modbus.h>

#include <arpa/inet.h>
#include <sys/select.h>
#include <sys/socket.h>

#include <iostream>

#include <thread>
#include <vector>

#include "modbus/common.hpp"

#define NB_CONNECTION 1

std::map<uint16_t, uint16_t> start_to_finish_address = {
    {PuriPipetteModbus::START_ADDR, PuriPipetteModbus::FINISH_ADDR},
    {PuriMoveTubeModbus::START_ADDR, PuriMoveTubeModbus::FINISH_ADDR},
    {PuriAspirateMixModbus::START_ADDR, PuriAspirateMixModbus::FINISH_ADDR},
    {PuriMoveCarrierModbus::START_ADDR, PuriMoveCarrierModbus::FINISH_ADDR},
    {PuriShakeModbus::START_ADDR, PuriShakeModbus::FINISH_ADDR},
    {PuriPcrModbus::START_ADDR, PuriPcrModbus::FINISH_ADDR},
    {PuriTimeModbus::START_ADDR, PuriTimeModbus::FINISH_ADDR},
    {LibAspirateMixModbus::START_ADDR, LibAspirateMixModbus::FINISH_ADDR},
    {LibCentrifugeModbus::START_ADDR, LibCentrifugeModbus::FINISH_ADDR},
    {LibMoveCarrierModbus::START_ADDR, LibMoveCarrierModbus::FINISH_ADDR},
    {LibMoveTubeModbus::START_ADDR, LibMoveTubeModbus::FINISH_ADDR},
    {LibMovePcrTubeModbus::START_ADDR, LibMovePcrTubeModbus::FINISH_ADDR},
    {LibPipetteModbus::START_ADDR, LibPipetteModbus::FINISH_ADDR},
    {LibTimeModbus::START_ADDR, LibTimeModbus::FINISH_ADDR},
    {FluoCapTubeModbus::START_ADDR, FluoCapTubeModbus::FINISH_ADDR},
    {FluoFluoModbus::START_ADDR, FluoFluoModbus::FINISH_ADDR},
    {FluoMoveCarrierModbus::START_ADDR, FluoMoveCarrierModbus::FINISH_ADDR},
    {FluoMoveTubeModbus::START_ADDR, FluoMoveTubeModbus::FINISH_ADDR},
    {FluoPipetteModbus::START_ADDR, FluoPipetteModbus::FINISH_ADDR},
    {AmpAspirateMixModbus::START_ADDR, AmpAspirateMixModbus::FINISH_ADDR},
    {AmplificationModbus::START_ADDR, AmplificationModbus::FINISH_ADDR},
    {AmpMoveCarrierModbus::START_ADDR, AmpMoveCarrierModbus::FINISH_ADDR},
    {AmpMoveTubeModbus::START_ADDR, AmpMoveTubeModbus::FINISH_ADDR},
    {AmpPcrModbus::START_ADDR, AmpPcrModbus::FINISH_ADDR},
    {ConnecctPortageModbus::START_ADDR, ConnecctPortageModbus::FINISH_ADDR},
};

void handle(uint8_t function, uint16_t address, uint8_t* query, modbus_mapping_t* mb_mapping) {
    int value;
    // std::cout << "Function: " << (int)function << ",Address: " << address
    //           << ",Value: " << mb_mapping->tab_registers[address] << std::endl;

    switch (function) {
    case MODBUS_FC_READ_HOLDING_REGISTERS:
        std::cout<< "Read Holding Registers(" << address << ") from "
                  << mb_mapping->tab_registers[address] << std::endl;
        // modbus_read_registers(mb_mapping->tab_registers, address, 1, mb_mapping->tab_registers);
        break;
    case MODBUS_FC_WRITE_SINGLE_REGISTER:
        value = (query[10] << 8) | query[11];
        std::cout << "Write Single Register(" << address << ") from "
                  << mb_mapping->tab_registers[address] << " to " << value << std::endl;

        if (address == PortageModbus::START_ADDR) {
            mb_mapping->tab_registers[PortageModbus::FINISH_ADDR] = value;
        } else if (address == AmplificationModbus::START_ADDR) {
            mb_mapping->tab_registers[AmplificationModbus::FINISH_ADDR] = value;
            mb_mapping->tab_registers[AmplificationModbus::READY_ADDR]  = 1;
        } else if (address == FluorescenceModbus::START_ADDR) {
            mb_mapping->tab_registers[FluorescenceModbus::FINISH_ADDR] = value;
            mb_mapping->tab_registers[FluorescenceModbus::READY_ADDR]  = 1;
        } else if (address == CentrifugalModbus::START_ADDR) {
            mb_mapping->tab_registers[CentrifugalModbus::FINISH_ADDR] = value;
            mb_mapping->tab_registers[CentrifugalModbus::READY_ADDR]  = 1;
        } else if (start_to_finish_address.find(address) != start_to_finish_address.end()) {
            mb_mapping->tab_registers[start_to_finish_address[address]] = value;
        } else {
            std::cerr << "Address not found" << std::endl;
        }
        // modbus_write_register(mb_mapping->tab_registers, address,
        // mb_mapping->tab_registers[address]);
        break;
    case MODBUS_FC_WRITE_MULTIPLE_REGISTERS:
        // modbus_write_registers(mb_mapping->tab_registers, address, 1, mb_mapping->tab_registers);
        break;
    default:
        std::cerr << "Function not implemented" << std::endl;
        break;
    }
}

void one_machine(const char* ip, int port) {
    uint8_t query[MODBUS_TCP_MAX_ADU_LENGTH];
    int     rc;

    modbus_t* ctx = modbus_new_tcp(ip, port);

    modbus_mapping_t* mb_mapping = modbus_mapping_new(MODBUS_MAX_READ_BITS, 0, 0xffff, 0);
    if (mb_mapping == NULL) {
        fprintf(stderr, "Failed to allocate the mapping: %s\n", modbus_strerror(errno));
        modbus_free(ctx);
        return;
    }

    int server_socket = modbus_tcp_listen(ctx, NB_CONNECTION);
    if (server_socket == -1) {
        fprintf(stderr, "Unable to listen TCP connection\n");
        modbus_free(ctx);
        return;
    }

    fd_set refset;
    fd_set rdset;
    FD_ZERO(&refset);
    FD_SET(server_socket, &refset);
    int fdmax = server_socket;

    int header_length = modbus_get_header_length(ctx);

    for (;;) {
        rdset = refset;
        if (select(fdmax + 1, &rdset, NULL, NULL, NULL) == -1) {
            perror("Server select() failure.");
            return;
        }

        /* Run through the existing connections looking for data to be
         * read */
        for (int master_socket = 0; master_socket <= fdmax; master_socket++) {

            if (!FD_ISSET(master_socket, &rdset)) {
                continue;
            }

            if (master_socket == server_socket) {
                /* A client is asking a new connection */
                socklen_t          addrlen;
                struct sockaddr_in clientaddr;
                int                newfd;

                /* Handle new connections */
                addrlen = sizeof(clientaddr);
                memset(&clientaddr, 0, sizeof(clientaddr));
                newfd = accept(server_socket, (struct sockaddr*)&clientaddr, &addrlen);
                if (newfd == -1) {
                    perror("Server accept() error");
                } else {
                    FD_SET(newfd, &refset);

                    if (newfd > fdmax) {
                        /* Keep track of the maximum */
                        fdmax = newfd;
                    }
                    printf("New connection from %s:%d on socket %d\n",
                           inet_ntoa(clientaddr.sin_addr), clientaddr.sin_port, newfd);
                }
            } else {
                modbus_set_socket(ctx, master_socket);
                rc = modbus_receive(ctx, query);

                if (rc > 0) {
                    uint8_t  function = query[header_length];
                    uint16_t address  = MODBUS_GET_INT16_FROM_INT8(query, header_length + 1);

                    handle(function, address, query, mb_mapping);

                    modbus_reply(ctx, query, rc, mb_mapping);
                } else if (rc == -1) {
                    /* This example server in ended on connection closing or
                     * any errors. */
                    printf("Connection closed on socket %d\n", master_socket);
                    close(master_socket);

                    /* Remove from reference set */
                    FD_CLR(master_socket, &refset);

                    if (master_socket == fdmax) {
                        fdmax--;
                    }
                }
            }
        }
    }
}

int main(void) {

    std::vector<std::thread> threads;
    threads.emplace_back(one_machine, "127.0.0.1", 1500);
    threads.emplace_back(one_machine, "127.0.0.1", 1501);
    threads.emplace_back(one_machine, "127.0.0.1", 1502);
    threads.emplace_back(one_machine, "127.0.0.1", 1503);
    threads.emplace_back(one_machine, "127.0.0.1", 1504);

    for (auto& thread : threads) {
        thread.join();
    }

    return 0;
}