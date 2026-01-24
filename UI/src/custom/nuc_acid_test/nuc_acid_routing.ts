import { step0_EPA_chat, step0_EPA_step } from "./step0_EPA";
import { step1_EPA_chat, step1_EPA_step } from "./step1_EPA";
import { step2_PGA_chat, step2_PGA_step } from "./step2_PGA";
import { step3_EPA_chat, step3_EPA_step } from "./step3_EPA";
import { step4_RMA_chat, step4_RMA_step } from "./step4_RMA";
import { step5_EPA_chat, step5_EPA_step } from "./step5_EPA";
import { step6_PGA_chat, step6_PGA_step } from "./step6_PGA";
import { step7_EPA_chat, step7_EPA_step } from "./step7_EPA";
import { step8_PDA_chat, step8_PDA_step } from "./step8_PDA";
import { step9_PDA_chat, step9_PDA_step } from "./step9_PDA";
import { step10_EPA_chat, step10_EPA_step } from "./step10_EPA";
import { step11_HEVA_chat, step11_HEVA_step } from "./step11_HEVA";
import { type } from "os";

export let step0_chats = [step0_EPA_chat];
export let step0_steps = [step0_EPA_step];

export let step1_chats = step0_chats.concat([step1_EPA_chat]);
export let step1_steps = step0_steps.concat([step1_EPA_step]);

export let step2_chats = step1_chats.concat([step2_PGA_chat]);
export let step2_steps = step1_steps.concat([step2_PGA_step]);

export let step3_chats = step2_chats.concat([step3_EPA_chat]);
export let step3_steps = step2_steps.concat([step3_EPA_step]);

export let step4_chats = step3_chats.concat([step4_RMA_chat]);
export let step4_steps = step3_steps.concat([step4_RMA_step]);

export let step5_chats = step4_chats.concat([step5_EPA_chat]);
export let step5_steps = step4_steps.concat([step5_EPA_step]);

export let step6_chats = step5_chats.concat([step6_PGA_chat]);
export let step6_steps = step5_steps.concat([step6_PGA_step]);

export let step7_chats = step6_chats.concat([step7_EPA_chat]);
export let step7_steps = step6_steps.concat([step7_EPA_step]);

export let step8_chats = step7_chats.concat([step8_PDA_chat]);
export let step8_steps = step7_steps.concat([step8_PDA_step]);

export let step9_chats = step7_chats.concat([step9_PDA_chat]);
export let step9_steps = step7_steps.concat([step9_PDA_step]);

export let step10_chats = step9_chats.concat([step10_EPA_chat]);
export let step10_steps = step9_steps.concat([step10_EPA_step]);

export let step11_chats = step10_chats.concat([step11_HEVA_chat]);
export let step11_steps = step10_steps.concat([step11_HEVA_step]);


type ChatMessage = {
    role: string;
    content: string;
};

// 2. 定义单个聊天序列
type ChatHistory = ChatMessage[];

// 3. 定义一个步骤中的所有聊天分支
export type AllChats = ChatHistory[];

// 4. 定义每个步骤状态的元组
type NucStateTuple = [AllChats, string[]];

// 5. 定义最终的映射类型
export type NucStateMap = {
    [key: string]: NucStateTuple;
};


export var nuc_state_map: NucStateMap = {
    "step0": [step0_chats, step0_steps],
    "step1": [step1_chats, step1_steps],
    "step2": [step2_chats, step2_steps],
    "step3": [step3_chats, step3_steps],
    "step4": [step4_chats, step4_steps],
    "step5": [step5_chats, step5_steps],
    "step6": [step6_chats, step6_steps],
    "step7": [step7_chats, step7_steps],
    "step8": [step8_chats, step8_steps],
    "step9": [step9_chats, step9_steps],
    "step10": [step10_chats, step10_steps],
    "step11": [step11_chats, step11_steps],
};

export var nuc_states = ["step0", "step1", "step2", "step3", "step4", "step5", "step6", "step7", "step8", "step9", "step10", "step11"];
// export var nuc_states = ["step0"];
// export var nuc_states = ["step7", "step8", "step9"];

// export var nuc_states = ["step10", "step11"];
