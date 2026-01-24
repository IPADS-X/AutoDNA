#pragma once

#include <condition_variable>
#include <memory>
#include <mutex>
#include <queue>

template <typename T>
class ThreadSafeQueue {
public:
    ThreadSafeQueue() = default;

    void push(T new_value) {
        std::lock_guard<std::mutex> lock(mutex_);
        queue_.push(std::move(new_value));
        condition_.notify_one();
    }

    T pop() {
        std::unique_lock<std::mutex> lock(mutex_);
        while (queue_.empty()) {
            condition_.wait(lock);
        }

        auto value = queue_.front();
        queue_.pop();
        return value;
    }

    std::shared_ptr<T> try_pop() {
        std::lock_guard<std::mutex> lock(mutex_);
        if (queue_.empty()) {
            return std::shared_ptr<T>();
        }

        auto result = std::make_shared<T>(std::move(queue_.front()));
        queue_.pop();
        return result;
    }

    bool empty() const {
        std::lock_guard<std::mutex> lock(mutex_);
        return queue_.empty();
    }

private:
    mutable std::mutex      mutex_;
    std::queue<T>           queue_;
    std::condition_variable condition_;
};