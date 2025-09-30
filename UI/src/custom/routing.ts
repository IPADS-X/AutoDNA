import { at, max } from "lodash";
import { synthesis_chats, synthesis_steps, synthesis_pre_chats, synthesis_pre_steps } from "./synthesis/synthesis_routing";
import { sequence_chats, sequence_steps, sequence_pre_chats, sequence_pre_steps } from "./sequence/sequence_routing";

import { nuc_state_map, nuc_states, NucStateMap, AllChats } from "./nuc_acid_test/nuc_acid_routing";

import _ from 'lodash';

import { io } from 'socket.io-client';
import { json } from "stream/consumers";
import { J } from "node_modules/framer-motion/dist/types.d-B50aGbjN";
import { run, sleep } from "@trpc/server/unstable-core-do-not-import";

interface GlobalState {
    index: number;
    state: string;
    started: boolean;
}

export var global_var: GlobalState = {
    index: 1,
    state: "synthesis",
    started: false
}

export var taskId = 0;

export var running = false;

export function change_running(value: boolean) {
    running = value;
}

type Callback = (data: GlobalState) => void;

export const eventEmitter = {
    events: {} as Record<string, Callback[]>,
    subscribe(event: string, callback: Callback) {
        if (!this.events[event]) {
            this.events[event] = [];
        }
        this.events[event].push(callback);
    },
    emit(event: string, data: GlobalState) {
        if (this.events[event]) {
            this.events[event].forEach(callback => callback(data));
        }
    },
};


export var state_map: {
    [key: string]: [{
        role: string;
        content: string;
        agent?: string;
        new_content?: string;
    }[][], string[]]
} = {
    "synthesis": [synthesis_chats, synthesis_steps],
    "sequence": [sequence_chats, sequence_steps],
    "synthesis_pre": [synthesis_pre_chats, synthesis_pre_steps],
    "sequence_pre": [sequence_pre_chats, sequence_pre_steps],
}

state_map = { ...state_map, ...nuc_state_map };

export function next() {
    if (global_var.index < synthesis_steps.length - 1) {
        let new_var = global_var;
        new_var.index++;
        console.log(new_var);
        eventEmitter.emit('stateChange', new_var);
    }
    return state_map[global_var.state];
}

export async function change_to_last(need_generate: boolean = false) {
    let real_index = state_map[global_var.state][1].length - 1;
    let new_var = global_var;
    new_var.index = real_index;
    new_var.started = true;
    console.log(new_var);
    console.log("change_to_last", new_var);
    // await sleep(500);
    // eventEmitter.emit('stateChange', new_var);


    if (need_generate) {
        startGenerating(_.cloneDeep(global_var));
    } else {
        eventEmitter.emit('stateChange', new_var);
    }
    // eventEmitter.emit('stateChange', new_var);
    return state_map[global_var.state];
}

export async function change_to(title: string, need_generate: boolean = false) {
    let real_index = state_map[global_var.state][1].findIndex((step) => step === title);
    let new_var = global_var;
    new_var.index = real_index;
    new_var.started = true;
    console.log(new_var);
    // await sleep(500);
    // eventEmitter.emit('stateChange', new_var);
    if (need_generate) {
        startGenerating(_.cloneDeep(global_var));
    } else {
        eventEmitter.emit('stateChange', new_var);
    }
    // eventEmitter.emit('stateChange', new_var);
    return state_map[global_var.state];
}

export function get_length(static_var: GlobalState) {
    let last_index = state_map[static_var.state][1].length - 1;
    let origin_chat = _.cloneDeep(state_map[static_var.state][0][last_index]);
    console.log("get_length", origin_chat);
    console.log("global_var", static_var);
    let assis_length = 0;
    let assis_index_length = 0;
    for (let i = 0; i < origin_chat.length; i++) {
        console.log("get_length", i, origin_chat[i]);
        if (origin_chat[i].role === "assistant") {
            assis_length = assis_length + origin_chat[i].content.length;
            assis_index_length++;
        }
    }
    console.log("assis_length", assis_length);
    return [assis_length, assis_index_length];
}

export async function startGenerating(static_var: GlobalState) {
    let origin_chat = _.cloneDeep(state_map[static_var.state][0][static_var.index]);
    let max_times = 10;

    let num = 0;
    for (let i = 0; i < origin_chat.length; i++) {
        console.log("startGenerating", i, origin_chat[i]);
        // let new_var = global_var;

        if (origin_chat[i].role === "assistant") {
            num++;
            state_map[static_var.state][0][static_var.index][i].content = "";
            eventEmitter.emit('stateChange', static_var);
        }
    }


    for (let i = 0; i < origin_chat.length; i++) {
        console.log("startGenerating", i, origin_chat[i]);
        // let new_var = global_var;

        if (origin_chat[i].role === "assistant") {
            // if (origin_chat[i].new_content) {
            //     eventEmitter.emit('stateChange', static_var);
            //     i++;
            //     continue;
            // }
            max_times = Math.min(50, Math.max(10, Math.floor(origin_chat[i].content.length / 10)));
            let now_times = 0;
            let id = setInterval(() => {
                // 在这里调用你想每秒执行的函数
                console.log(origin_chat[i].content.length * now_times / max_times);
                console.log("now_times", now_times);
                console.log("length", origin_chat[i].content.length);
                console.log("content", origin_chat[i].content);
                state_map[static_var.state][0][static_var.index][i].content = origin_chat[i].content.slice(0, origin_chat[i].content.length * now_times / max_times);
                eventEmitter.emit('stateChange', static_var);

                now_times++;
                if (now_times > max_times) {
                    clearInterval(id);
                    return;
                }
            }, 300);

            if (num >= 2) {
                await sleep(300 * (max_times + 1) + 200);
            }
        }
    }
}

const test_mode = false;
// const test_mode = true;

export function start_synthesis_pre() {
    global_var.state = "synthesis_pre";
    global_var.index = -1;
    change_to(synthesis_pre_steps[0], true);
    let id = setInterval(() => {
        // 在这里调用你想每秒执行的函数
        let new_var = _.cloneDeep(global_var);
        if (new_var.index < synthesis_pre_steps.length - 1) {
            change_to(synthesis_pre_steps[new_var.index + 1], true);
        } else {
            clearInterval(id);
        }

        new_var.started = true;
        // eventEmitter.emit('stateChange', new_var);
    }, 7000, 0);
}

export function start_synthesis() {
    global_var.state = "synthesis";
    global_var.index = 0;
    change_to(synthesis_steps[1], true);
    let now_index = 1;
    let id = setInterval(() => {
        // 在这里调用你想每秒执行的函数
        let new_var = _.cloneDeep(global_var);
        new_var.index = now_index;
        if (new_var.index < synthesis_steps.length - 1) {
            change_to(synthesis_steps[new_var.index + 1], true);
        } else {
            clearInterval(id);
            running = false;
            if (!test_mode) {
                run_synthesis();
            }
        }

        new_var.started = true;
        now_index++;
        // eventEmitter.emit('stateChange', new_var);
    }, 7000, 0);
}

export function start_synthesis_all(message: string = "") {
    if (message === undefined || message === null) {
        message = "";
    }
    message = message.replace("我要写入", "思源同学正在为您写入");
    // message = message.replace("请写入", "");
    message = message.replace("，", "");
    message = message.replace(",", "");
    message = message === "" ? "你好" : message;
    console.log(synthesis_pre_chats[0][0].content);
    synthesis_pre_chats[0][0].content = message;
    // synthesis_pre_chats[0][0].content=synthesis_pre_chats[0][0].content.replace("“”", "“" + message + "”");
    console.log(synthesis_pre_chats[0][0].content);
    start_synthesis_pre();
    let timeout = setTimeout(() => {
        start_synthesis();
        clearTimeout(timeout);
    }
        , 7000);
}

export function start_sequence_pre() {
    global_var.state = "sequence_pre";
    global_var.index = -1;
    change_to(sequence_pre_steps[0], true);
    let id = setInterval(() => {
        // 在这里调用你想每秒执行的函数
        let new_var = _.cloneDeep(global_var);
        if (new_var.index < sequence_pre_steps.length - 1) {
            change_to(sequence_pre_steps[new_var.index + 1], true);
        } else {
            clearInterval(id);
        }

        new_var.started = true;
        // eventEmitter.emit('stateChange', new_var);
    }, 7000, 0);
}

export function start_sequence() {
    global_var.state = "sequence";
    global_var.index = 0;
    change_to(sequence_steps[1], true);
    let now_index = 1;
    let id = setInterval(() => {
        // 在这里调用你想每秒执行的函数
        let new_var = _.cloneDeep(global_var);
        new_var.index = now_index;
        if (new_var.index < sequence_steps.length - 1) {
            change_to(sequence_steps[new_var.index + 1], true);
        } else {
            clearInterval(id);
            running = false;
            if (!test_mode) {
                run_sequence();
            }
        }

        new_var.started = true;
        now_index++;
        // eventEmitter.emit('stateChange', new_var);
    }, 7000);

}

export function start_sequence_all() {
    // let timeout1 = setTimeout(() => {
    start_sequence_pre();
    let timeout = setTimeout(() => {
        start_sequence();
        clearTimeout(timeout);
    }
        , 10000);
    // }, 10000);
}


export async function start_nuc_acid_test() {
    running = true;
    global_var.state = "step0";
    global_var.index = 0;
    let length = get_length(global_var);
    change_to_last(true);
    let now_index = 1;
    console.log("nuclength", length);
    await sleep(Math.min(50, Math.max(10, Math.floor(length[0] / 10))) * 300 + 3500);
    for (; now_index < nuc_states.length; now_index++) {
        // 在这里调用你想每秒执行的函数
        global_var.state = nuc_states[now_index];
        global_var.index = 0;
        console.log("start nuc change to " + nuc_states[now_index], global_var, now_index);
        length = get_length(global_var);
        if (now_index < nuc_states.length) {
            change_to_last(true);
        }

        await sleep(Math.min(50, Math.max(10, Math.floor(length[0] / 10))) * 300 + 3500);

        if (length[1] >= 2) {
            await sleep(8000);
        }

        global_var.started = true;
    }
}

export async function start_append(new_state_map: NucStateMap) {
    running = true;
    
    const new_keys = Object.keys(new_state_map);
    let last_index = new_keys.length - 1;
    nuc_states.length = 0;
    nuc_states.push(...new_keys);

    for (const key in nuc_state_map) {
        if (Object.prototype.hasOwnProperty.call(nuc_state_map, key)) {
            delete nuc_state_map[key];
        }
    }

    Object.assign(nuc_state_map, new_state_map);
    Object.assign(state_map, nuc_state_map);

    global_var.state = nuc_states[last_index];
    global_var.index = 0;
    console.log("start nuc change to " + nuc_states[last_index], global_var, last_index);
    let length = get_length(global_var);
    if (last_index < nuc_states.length) {
        change_to_last(true);
    }

    await sleep(Math.min(50, Math.max(10, Math.floor(length[0] / 10))) * 300 + 3500);

    if (length[1] >= 2) {
        await sleep(8000);
    }

    global_var.started = true;

    running = false;
}

export function transfer_data(old_data: NucStateMap) {
    let new_data: NucStateMap = {}

    let old_keys = Object.keys(old_data)

    // 如果旧数据为空，则直接返回空对象
    if (old_keys.length === 0) {
        return new_data;
    }

    // 1. 初始化累积器
    // combined_chats 累积所有步骤的第一个元素 (AllChats: ChatMessage[][])
    let combined_chats: AllChats = [];
    // combined_steps 累积所有步骤的第二个元素 (string[]: 步骤说明)
    let combined_steps: string[] = [];

    // 2. 遍历并合并所有值
    for (const key of old_keys) {
        const value = old_data[key];

        // 假设 value 是 [AllChats, string[]] 元组
        const chats_array = value[0];  // 聊天记录数组 (ChatMessage[][])
        const steps_array = value[1];  // 步骤说明数组 (string[])

        // 叠加第一个数组：使用 concat 或扩展运算符进行合并
        combined_chats = combined_chats.concat(chats_array);

        // 叠加第二个数组：使用 concat 或扩展运算符进行合并
        combined_steps = combined_steps.concat(steps_array);

        new_data[key] = [[], []]
    }

    // 3. 获取最后一个键
    const last_key = old_keys[old_keys.length - 1];

    // 4. 将合并后的值赋给新数据中的最后一个键
    // new_data 最终将只包含一个键：最后一个键
    new_data[last_key] = [combined_chats, combined_steps];

    return new_data;
}

var pollingIntervalId: NodeJS.Timeout | null;

// var backup_state_map = _.cloneDeep(nuc_state_map);
// var index = -1;
export async function run_all(user_prompt: string) {

    // console.log(JSON.stringify(nuc_state_map))

    while (nuc_states.length > 0) {
        nuc_states.pop()
    }
    for (const key in nuc_state_map) {
        if (Object.prototype.hasOwnProperty.call(nuc_state_map, key)) {
            delete nuc_state_map[key];
        }
    }

    if (pollingIntervalId !== null) {
        clearInterval(pollingIntervalId);
        pollingIntervalId = null;
    }

    // pollingIntervalId = setInterval(async () => {
    //     // 使用 async 关键字以便在 .then() 链中更方便地处理异步逻辑

    //     if (running) {
    //         // 如果上一个 start_append 仍在运行，则跳过本次轮询
    //         return;
    //     }

    //     index++;

    //     let data: NucStateMap = {};

    //     const all_keys = Object.keys(backup_state_map);

    //     for (let i = 0; i < index; i++) {
    //         data[all_keys[i]] = backup_state_map[all_keys[i]]
    //     }

    //     // data 应该是 NucStateMap 类型
    //     // 假设后端返回的数据是一个非空的 NucStateMap 对象
    //     const keys = Object.keys(data);
    //     if (keys.length > 0 && keys.length != nuc_states.length) {
    //         // 调用 start_append 来处理新状态
    //         await start_append(data);
    //     }

    // }, 1000) as unknown as NodeJS.Timeout; // 轮询间隔 1000ms (1秒)



    // 4. 发送 user_prompt 给后端服务
    const send_url = "http://localhost:8081/prompt"; // 假设的提交 URL

    try {
        // 使用 POST 请求发送用户提示，JSON 格式是最佳实践
        await fetch(send_url, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({ user_prompt: user_prompt }),
        });

        console.log(`User prompt sent to ${send_url}`);
    } catch (error) {
        console.error('Error submitting user prompt:', error);
        // 如果发送失败，可能需要提前退出或显示错误
        running = false;
        return;
    }

    const http_url = "http://localhost:8081/heartbeat"; // 假设的轮询 URL

    pollingIntervalId = setInterval(async () => {
        // 使用 async 关键字以便在 .then() 链中更方便地处理异步逻辑

        if (running) {
            // 如果上一个 start_append 仍在运行，则跳过本次轮询
            return;
        }

        try {
            const response = await fetch(http_url);

            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            const data = await response.json();

            console.log("get data: ", JSON.stringify(data))

            // data 应该是 NucStateMap 类型
            // 假设后端返回的数据是一个非空的 NucStateMap 对象
            const keys = Object.keys(data);
            if (keys.length > 0 && keys.length != nuc_states.length) {
                // transfer data
                let new_data = transfer_data(data);

                // 调用 start_append 来处理新状态
                await start_append(new_data);
            }
        } catch (error) {
            console.warn('Error fetching new state:', error);
            // 考虑在多次失败后停止轮询
        }

    }, 1000) as unknown as NodeJS.Timeout; // 轮询间隔 1000ms (1秒)
}


export function get() {
    return state_map[global_var.state];
}

export function get_all_steps() {
    return state_map[global_var.state][1];
}

export function get_chat_by_index(index: number) {
    return state_map[global_var.state][0][global_var.index][Number(index)];
}

export function get_chats() {
    return state_map[global_var.state][0];
}

// const socket = new WebSocket('ws://192.168.250.231:8765/');

// socket.onopen = (event) => {
//     console.log('WebSocket is open now.');
//   };

//   socket.onmessage = (event) => {
//     console.log('Received message:', event.data);
//     let data = JSON.parse(event.data);
//     if (data.taskId !== taskId) {
//         taskId = data.taskId;
//         if (data.state === "synthesis") {
//             start_synthesis_all(data.message);
//         }
//         else if (data.state === "sequence") {
//             start_sequence_all();
//         }
//     }
//   };

//   socket.onerror = (event) => {
//     console.error('WebSocket error:', event);
//   };

//   socket.onclose = (event) => {
//     console.log('WebSocket is closed now.');
//   };

// const socket = io('http://192.168.250.177:29998/'); // 替换为你的服务器地址

// socket.on("connect", () => {
//     console.log("Connected to server");
//   });


async function run_synthesis() {
    console.log("run_synthesis");
    // let json_data = {
    //     forward: true,
    //     type: "synthesis",
    //     taskId: taskId,
    // };
    // socket.send(JSON.stringify(json_data));
    try {
        const response = await fetch("http://192.168.250.177:29998/", {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({
                startsjtu: true,
                type: "synthesis",
                taskId: taskId,
            }),
        });

        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        const result = await response.json();
        console.log(result);
    } catch (error) {
        console.error('Error posting data:', error);
    }
}

async function run_sequence() {
    // console.log("run_sequence");
    // let json_data = {
    //     forward: true,
    //     type: "sequence",
    //     taskId: taskId,
    // };
    // socket.send(JSON.stringify(json_data));
    try {
        const response = await fetch("http://192.168.250.177:29998/", {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({
                startsjtu: true,
                type: "sequence",
                taskId: taskId,
            }),
        });

        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        const result = await response.json();
        console.log(result);
    } catch (error) {
        console.error('Error posting data:', error);
    }
}


// socket.on('message', (msg) => {
//     console.log(msg);
//     let data = JSON.parse(msg);
//     if (data.taskId !== taskId) {
//         taskId = data.taskId;
//         if (data.state === "synthesis") {
//             start_synthesis_all(data.message);
//         }
//         else if (data.state === "sequence") {
//             start_sequence_all();
//         }
//     }
// });



// let polling = setInterval(() => {
//     console.log("polling and running", running);

//     // socket.send('polling');

//     // 发送http请求，获取当前状态，决定是否开始

//     let http_url = "http://192.168.250.177:29998/";
//     if (test_mode) {
//         http_url = "http://localhost:3001/";
//     } else {
//         http_url = "http://192.168.250.177:29998/";
//     }

//     fetch(http_url)
//         .then(response => response.json())
//         .then(data => {
//             console.log(data);
//             console.log(running);
//             if (running) {
//                 return;
//             }
//             if (data.taskId !== taskId) {
//                 taskId = data.taskId;
//                 // running = true; // NOTE: ALL change will be set it to true, even chang to zero (client restarted)
//                 if (data.state === "synthesis") {
//                     running = true;
//                     start_synthesis_all(data.message);
//                 } else if (data.state === "sequence") {
//                     running = true;
//                     start_sequence_all();
//                 }
//             }
//         })
//         .catch(error => {
//             console.warn('Error:', error);
//         });
// }, 1000);

// import express, { Request, Response } from 'express';

// const app = express();
// const port = 3001;

// app.get('/start_synthesis', (req: Request, res: Response) => {
//     start_synthesis();
//     res.send('start_synthesis');
// });

// app.get('/start_sequence', (req: Request, res: Response) => {
//     start_sequence();
//     res.send('start_sequence');
// });

// app.listen(port, () => {
//   console.log(`Server is running at http://localhost:${port}`);
// });