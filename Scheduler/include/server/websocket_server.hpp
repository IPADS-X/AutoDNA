#pragma once

#include <boost/asio/ip/tcp.hpp>
#include <boost/beast/core.hpp>
#include <boost/beast/websocket.hpp>

#include <iostream>
#include <string>
#include <thread>

#include "common/thread_safe_queue.hpp"
#include "procedure/common.hpp"

namespace beast     = boost::beast;     // from <boost/beast.hpp>
namespace http      = beast::http;      // from <boost/beast/http.hpp>
namespace websocket = beast::websocket; // from <boost/beast/websocket.hpp>
namespace net       = boost::asio;      // from <boost/asio.hpp>
using tcp           = net::ip::tcp;     // from <boost/asio/ip/tcp.hpp>

class WebSocketServer {
public:
    WebSocketServer(ThreadSafeQueue<std::shared_ptr<WebEvent>>& recv_queue,
                    ThreadSafeQueue<std::shared_ptr<WebEvent>>& send_queue)
        : recv_queue_(recv_queue), send_queue_(send_queue) {}

    // Echoes back all received WebSocket messages
    void do_session(tcp::socket socket) {
        try {
            ws_ = std::make_shared<websocket::stream<tcp::socket>>(std::move(socket));
            std::cout << "WebSocket session started" << std::endl;
            // Accept the WebSocket handshake
            ws_->accept();

            for (;;) {
                beast::flat_buffer buffer;
                // Read a message
                ws_->read(buffer);
                // print the message payload
                auto data = beast::buffers_to_string(buffer.data());
                recv_queue_.push(std::make_shared<WebEvent>(std::move(data)));

                // Echo the message back
                ws_->text(ws_->got_text());
                ws_->write(buffer.data());
            }
        } catch (beast::system_error const& se) {
            // This indicates that the session was closed
            if (se.code() != websocket::error::closed)
                std::cerr << "Error: " << se.code().message() << std::endl;
        } catch (std::exception const& e) {
            std::cerr << "Exception: " << e.what() << std::endl;
        }
    }

    void send(std::string&& data) { ws_->write(net::buffer(data)); }

    void start_check_send_queue() {
#ifndef WEB_MODE
        return;
#endif
        try {
            for (;;) {
                auto web_event = send_queue_.try_pop();
                if (web_event) {
                    send(std::move(web_event->get()->data));
                    std::cout << "Sent WebSocket message: " << web_event->get()->data << std::endl;
                }
                std::this_thread::sleep_for(std::chrono::milliseconds(500));
            }
        } catch (std::exception const& e) {
            std::cerr << "Exception: " << e.what() << std::endl;
        }
    }

    void start() {
#ifndef WEB_MODE
        recv_queue_.push(std::make_shared<WebEvent>(""));
        return;
#endif

        std::thread([this]() { this->start_check_send_queue(); }).detach();

        try {
            auto const     address = net::ip::make_address("0.0.0.0");
            unsigned short port    = 8080;

            net::io_context ioc{1};

            tcp::acceptor acceptor{ioc, {address, port}};
            for (;;) {
                // check if there is any new connection
                tcp::socket socket{ioc};
                acceptor.accept(socket);
                std::cout << "Accepted connection" << std::endl;
                std::thread([s = std::move(socket), this]() mutable {
                    do_session(std::move(s));
                }).detach();
            }
        } catch (std::exception const& e) {
            std::cerr << "Exception: " << e.what() << std::endl;
        }
    }

private:
    ThreadSafeQueue<std::shared_ptr<WebEvent>>&recv_queue_, &send_queue_;

    std::shared_ptr<websocket::stream<tcp::socket>> ws_;
};