#include <boost/asio/connect.hpp>
#include <boost/asio/ip/tcp.hpp>
#include <boost/beast/core.hpp>
#include <boost/beast/websocket.hpp>
#include <cstdlib>
#include <iostream>
#include <nlohmann/json.hpp>
#include <string>

// Use namespaces from Boost.Beast for convenience
namespace beast     = boost::beast;     // from <boost/beast.hpp>
namespace http      = beast::http;      // from <boost/beast/http.hpp>
namespace websocket = beast::websocket; // from <boost/beast/websocket.hpp>
namespace net       = boost::asio;      // from <boost/asio.hpp>
using tcp           = net::ip::tcp;     // from <boost/asio/ip/tcp.hpp>

/**
 * @brief A synchronous WebSocket client.
 * * This program connects to a WebSocket server, sends a message provided
 * via the command line, reads the echoed response, and then closes
 * the connection gracefully.
 */
int main(int argc, char** argv) {
    try {
        // Check for the correct number of command-line arguments.
        // The program expects one argument: the message to send.
        if (argc < 3) {
            std::cerr << "Sends a message to a WebSocket echo server.\n"
                      << "Usage: websocket_client <type> <message>\n"
                      << "Example:\n"
                      << "    websocket_client -r \"Hello, WebSocket!\"\n";
            return EXIT_FAILURE;
        }

        // Hardcode the server's host and port.
        // The server is expected to be running on the local machine.
        auto const host    = "127.0.0.1";
        auto const port    = "8080";
        auto const message = argv[2];

        // The io_context is required for all I/O operations.
        net::io_context ioc;

        // The resolver and websocket stream are our main I/O objects.
        tcp::resolver                  resolver{ioc};
        websocket::stream<tcp::socket> ws{ioc};

        // Use the resolver to look up the server's domain name and port.
        auto const results = resolver.resolve(host, port);

        // Make the TCP connection to the IP address we found.
        auto endpoint = net::connect(ws.next_layer(), results);

        // The WebSocket handshake requires the host name.
        // We construct it from the host and the connected endpoint's port.
        std::string host_and_port = std::string(host) + ":" + std::to_string(endpoint.port());

        // Set a decorator to change the User-Agent of the handshake request.
        // This is optional but good practice.
        ws.set_option(websocket::stream_base::decorator([](websocket::request_type& req) {
            req.set(http::field::user_agent,
                    std::string(BOOST_BEAST_VERSION_STRING) + " websocket-client");
        }));

        // Perform the WebSocket handshake to upgrade the TCP connection.
        ws.handshake(host_and_port, "/");

        if (argv[1] == std::string("-r")) {
            // // Send the message from the command-line argument to the server.
            // std::cout << "Sending: " << message << std::endl;
            // ws.write(net::buffer(std::string(message)));

            nlohmann::json json_message;
            json_message["command"]      = "renew_reagent";
            json_message["reagent_name"] = argv[2];
            if (argc >= 4) {
                json_message["volume"] = std::stoi(argv[3]);
            } else {
                json_message["volume"] = 1000; // default volume
            }
            ws.write(net::buffer(json_message.dump()));
        } else if (argv[1] == std::string("-c")) {
            nlohmann::json json_message;
            json_message["command"]      = "renew_consumer";
            json_message["carrier_name"] = argv[2];

            if (argc >= 4) {
                json_message["machine_type"] = std::stoi(argv[3]);
            } else {
                json_message["machine_type"] = 0; // default machine type
            }

            if (argc >= 5) {
                json_message["area_id"] = std::stoi(argv[4]);
            } else {
                json_message["area_id"] = 0; // default area id
            }

            ws.write(net::buffer(json_message.dump()));
        } else if (argv[1] == std::string("-w")) {
            nlohmann::json json_message;
            json_message["command"]       = "new_workflow";
            json_message["workflow_name"] = argv[2];
            if (argc >= 4) {
                json_message["times"] = nlohmann::json::parse(argv[3]);
            } else {
                json_message["times"] = 1; // default empty object
            }
            if (argc >= 5) {
                json_message["jump_from"] = argv[4];
            } else {
                json_message["jump_from"] = 1; // default empty data
            }
            ws.write(net::buffer(json_message.dump()));
        } else {
            std::cerr << "Unknown command type: " << argv[1] << std::endl;
            return EXIT_FAILURE;
        }

        // This buffer will hold the incoming message from the server.
        beast::flat_buffer buffer;

        // Block and read the echoed message into our buffer.
        ws.read(buffer);

        // Print the received message to the console.
        std::cout << "Received: " << beast::make_printable(buffer.data()) << std::endl;

        // Gracefully close the WebSocket connection.
        ws.close(websocket::close_code::normal);
    } catch (std::exception const& e) {
        // If any part of the process fails, report the error.
        std::cerr << "Error: " << e.what() << std::endl;
        return EXIT_FAILURE;
    }

    // Indicate successful execution.
    return EXIT_SUCCESS;
}
