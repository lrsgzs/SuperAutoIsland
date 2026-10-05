export const MSG_ID_FIELD = 'msgId';

/**
 * 已知的「回包」类型。
 *
 * 只有这些类型才允许走「回包没带 msgId」的兜底，
 * 否则后端主动推送的 log / runFinished 会被误当成某个在途请求的回包。
 */
const REPLY_TYPES = new Set(['result', 'error', 'bad-command-type', 'bad-project-type']);

type Waiter = {
    resolve: (value: any) => void;
    reject: (reason?: any) => void;
};

/** 后端主动推送的消息（不是请求回包），例如 log、runFinished */
export type ServerPush = { type: string } & Record<string, any>;

type PushHandler = (message: ServerPush) => void;

type SocketState = {
    /** 在途请求：msgId -> 等待者 */
    waiters: Map<string, Waiter>;
    /** 推送订阅者 */
    pushHandlers: Set<PushHandler>;
};

const statesBySocket = new WeakMap<WebSocket, SocketState>();

function createMsgId(waiters: Map<string, Waiter>): string {
    let id: string;
    do {
        id = String(Math.floor(10_000_000 + Math.random() * 90_000_000));
    } while (waiters.has(id));

    return id;
}

/**
 * 取得（必要时创建）某个 socket 的状态，并挂上唯一的消息监听器
 * @param ws 要监听的 websocket
 */
function ensureState(ws: WebSocket): SocketState {
    const existing = statesBySocket.get(ws);
    if (existing) {
        return existing;
    }

    const state: SocketState = {
        waiters: new Map<string, Waiter>(),
        pushHandlers: new Set<PushHandler>(),
    };
    statesBySocket.set(ws, state);

    ws.addEventListener('message', (event: MessageEvent) => {
        let message: { type?: string; msgId?: string } & Record<string, any>;
        try {
            message = JSON.parse(event.data as string);
        } catch (error) {
            console.warn('[SAI] 收到无法解析的回包', error, event.data);
            return;
        }

        console.debug('Receiving message', message);

        // 1) 优先按 msgId 关联；回包没有 msgId 时（老后端）只在仅有一个在途请求时兜底
        let key: string | undefined;
        if (typeof message.msgId === 'string' && state.waiters.has(message.msgId)) {
            key = message.msgId;
        } else if (
            message.msgId === undefined &&
            REPLY_TYPES.has(message.type ?? '') &&
            state.waiters.size === 1
        ) {
            console.warn('[SAI] 回包没有 msgId，按唯一在途请求兜底处理', message);
            key = state.waiters.keys().next().value;
        }

        if (key !== undefined) {
            const waiter = state.waiters.get(key)!;
            state.waiters.delete(key);

            if (message.type === 'result') {
                waiter.resolve(message);
            } else {
                waiter.reject(JSON.stringify(message));
            }

            return;
        }

        // 2) 剩下的都是后端推送
        if (state.pushHandlers.size > 0) {
            for (const handler of state.pushHandlers) {
                handler(message as ServerPush);
            }

            return;
        }

        console.warn('[SAI] 收到无法关联的回包，已忽略', message);
    });

    // 连接断开时把在途请求全部拒掉，并通知推送订阅者，避免调用方永远挂着
    const notifyClosed = (reason: string) => {
        for (const waiter of state.waiters.values()) {
            waiter.reject(reason);
        }

        state.waiters.clear();

        for (const handler of state.pushHandlers) {
            handler({ type: 'socketClosed', reason });
        }
    };
    ws.addEventListener('close', () => notifyClosed('websocket closed'));
    ws.addEventListener('error', () => notifyClosed('websocket error'));

    return state;
}

/**
 * 订阅后端主动推送的消息（log、runFinished 等）
 *
 * @param ws 要订阅的 websocket
 * @param handler 收到推送时调用
 * @returns 取消订阅的函数
 */
export function onServerPush(ws: WebSocket, handler: PushHandler): () => void {
    const state = ensureState(ws);
    state.pushHandlers.add(handler);

    return () => {
        state.pushHandlers.delete(handler);
    };
}

/**
 * websocket 发送消息并等待传回数据
 *
 * 每次请求带一个 8 位 `msgId`，后端原样带回，这里按 id 找到对应的等待者，
 * 因此同一条连接上并发发多个请求也不会像以前那样「谁先回谁 resolve」。
 *
 * @param ws 要发送的 websocket
 * @param sendMessage 要发送的消息
 */
export function wsWaitMessage<T>(ws: WebSocket, sendMessage: any): Promise<{ type: string } & T> {
    const state = ensureState(ws);
    const msgId = createMsgId(state.waiters);

    return new Promise<{ type: string } & T>((resolve, reject) => {
        state.waiters.set(msgId, { resolve, reject });
        ws.send(JSON.stringify({ ...sendMessage, [MSG_ID_FIELD]: msgId }));
    });
}
