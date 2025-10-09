#pragma once

#include "fmt/format.h"
#include <nlohmann/json.hpp>
#include <string>

using Variables             = nlohmann::json;
using ActionId              = uint16_t;
using StepId                = uint;
using WorkflowId            = uint16_t;
static const StepId NULL_ID = 0;

class Step;

struct ExecutionResult {
    ExecutionResult() : next_step(nullptr), async(true) {}

    ExecutionResult(bool skip_to_next_phase) : next_step(nullptr), async(false), skip_to_next_phase(skip_to_next_phase) {}

    ExecutionResult(std::shared_ptr<Step> next_step, Variables&& output, bool async)
        : next_step(next_step), output(std::move(output)), async(async) {}
    std::shared_ptr<Step> next_step;
    Variables             output;
    bool                  async;
    bool                  skip_to_next_phase = false;
};

inline static void to_json(Variables& j, const ExecutionResult& r) {
    j = Variables{{"output", r.output}, {"async", r.async}, {"skip", r.skip_to_next_phase}};
}

template <>
struct fmt::formatter<ExecutionResult> : fmt::formatter<std::string> {
    auto format(const ExecutionResult& j, fmt::format_context& ctx) const {
        std::string output = fmt::format("{}, output: {}, async: {}", j.output.dump(), j.async);
        return fmt::formatter<std::string>::format(output, ctx);
    }
};

struct WebEvent {
    WebEvent(std::string&& data) : data(std::move(data)) {}
    std::string data;
};

struct MyEvent {
    MyEvent(ActionId action_id) : action_id(action_id) {}
    ActionId action_id;
};

template <>
struct fmt::formatter<nlohmann::json> : fmt::formatter<std::string> {
    auto format(const nlohmann::json& j, fmt::format_context& ctx) const {
        return fmt::formatter<std::string>::format(j.dump(), ctx);
    }
};
