cmake_minimum_required(VERSION 3.5)

set(CMAKE_CXX_STANDARD 17)

project(ProductionLine)

find_package(nlohmann_json 3.2.0 REQUIRED)

include_directories(
    ${PROJECT_SOURCE_DIR}/include
    ${PROJECT_SOURCE_DIR}/3rdparty/dtl
)


set(CMAKE_RUNTIME_OUTPUT_DIRECTORY ${CMAKE_CURRENT_BINARY_DIR}/bin)

# set source files
file(GLOB PROCEDURE_SRC "src/procedure/*.cpp")
file(GLOB SERVER_SRC "src/server/*.cpp")
file(GLOB REALITY_SRC "src/reality/*.cpp")
file(GLOB MODBUS_SRC "src/modbus/*.cpp")
file(GLOB MACHINE_SRC "src/machine/*.cpp")
file(GLOB TRANSFER_SRC "src/transform/*.cpp")

############################################################################
# modbus server
############################################################################

add_executable(main_server
    src/main_server.cpp
)

target_compile_options(main_server PUBLIC ${ADDITIONAL_PUBLIC_COMPILE_OPTIONS})
target_link_libraries(main_server modbus)

############################################################################
# client
############################################################################


set(MODES "web" "local_web")

foreach(mode ${MODES})
    add_executable(main_${mode}
        src/main_scheduler.cpp
        ${SERVER_SRC}
        ${REALITY_SRC}
        ${PROCEDURE_SRC}
        ${MODBUS_SRC}
        ${MACHINE_SRC}
        ${TRANSFER_SRC}
    )

    target_compile_options(main_${mode} PUBLIC ${ADDITIONAL_PUBLIC_COMPILE_OPTIONS})
    target_link_libraries(main_${mode}
        ${ADDITIONAL_LINK_LIBRARIES}
        modbus
        spdlog
        fmt
        nlohmann_json::nlohmann_json
        ${SSL_SUPPORT_LINK_LIBRARIES}
    )
endforeach(mode)

target_compile_definitions(main_local_web PUBLIC -DLOCAL_TEST -DJUMP_ORIGIN_STEP -DWEB_MODE)
target_compile_options(main_local_web PUBLIC -g)
target_compile_definitions(main_web PUBLIC -DJUMP_ORIGIN_STEP -DWEB_MODE)


# sender
add_executable(main_sender
    src/main_sender.cpp
)

target_compile_options(main_sender PUBLIC ${ADDITIONAL_PUBLIC_COMPILE_OPTIONS})
target_link_libraries(main_sender
    ${ADDITIONAL_LINK_LIBRARIES}
    modbus
    spdlog
    fmt
    nlohmann_json::nlohmann_json
    ${SSL_SUPPORT_LINK_LIBRARIES}
)