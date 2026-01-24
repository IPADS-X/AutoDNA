import { craft_chat, craft_step } from "./craft"
import { decoding_chat, decoding_step } from "./decoding"
import { coding_chat, coding_step } from "./coding";
import { run_code_chat, run_code_step } from "./run_code";
import { pre_sequence_chat, pre_sequence_step } from "./pre_sequence";
import { read_chat, read_step } from "./read_code";

export let sequence_chats = [pre_sequence_chat, craft_chat, decoding_chat, coding_chat, run_code_chat];

export let sequence_steps = [pre_sequence_step, craft_step, decoding_step, coding_step, run_code_step];

// export let sequence_chats = [pre_sequence_chat, craft_chat, decoding_chat, coding_chat, run_code_chat, read_chat];

// export let sequence_steps = [pre_sequence_step, craft_step, decoding_step, coding_step, run_code_step, read_step];

export let sequence_pre_chats = [pre_sequence_chat];

export let sequence_pre_steps = [pre_sequence_step];