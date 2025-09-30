import { encoding_chat, encoding_step } from "./encoding"
import { workflow_chat, workflow_step } from "./workflow"
import { coding_chat, coding_step } from "./coding"
import { run_code_chat, run_code_step } from "./run_code";
import { pre_synthesis_chat, pre_synthesis_step } from "./pre_synthesis";

export let synthesis_chats = [pre_synthesis_chat, encoding_chat, workflow_chat, coding_chat, run_code_chat];

export let synthesis_steps = [pre_synthesis_step, encoding_step, workflow_step, coding_step, run_code_step];

export let synthesis_pre_chats = [pre_synthesis_chat];

export let synthesis_pre_steps = [pre_synthesis_step];