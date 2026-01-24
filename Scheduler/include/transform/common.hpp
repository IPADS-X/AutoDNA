#pragma once
#include <string>

class CommonTransformFunc {
public:
    static int extractLastNumber(const std::string& str) {
        size_t last_digit_pos = str.find_last_of("0123456789");
        if (last_digit_pos == std::string::npos) {
            return 1;
        }
        size_t first_nondigit_pos = str.find_last_not_of("0123456789", last_digit_pos);
        size_t start_pos = (first_nondigit_pos == std::string::npos) ? 0 : first_nondigit_pos + 1;

        return std::stoi(str.substr(start_pos, last_digit_pos - start_pos + 1));
    }

    static std::string removeLastNumber(const std::string& str) {
        size_t last_digit_pos = str.find_last_of("0123456789");
        if (last_digit_pos == std::string::npos) {
            return str;
        }
        size_t first_nondigit_pos = str.find_last_not_of("0123456789", last_digit_pos);
        size_t end_pos = (first_nondigit_pos == std::string::npos) ? 0 : first_nondigit_pos + 1;

        return str.substr(0, end_pos);
    }
};