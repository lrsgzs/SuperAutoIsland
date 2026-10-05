export const MSG_ID_FIELD = 'msgId';

type Waiter = {
    resolve: (value: any) => void;
    reject: (reason?: any) => void;
};

const waitersBySocket = new WeakMap<WebSocket, Map<string, Waiter>>();

function createMsgId(waiters: Map<string, Waiter>): string {
    let id: string;
    do {
        id = String(Math.floor(10_000_000 + Math.random() * 90_000_000));
    } while (waiters.has(id));

    return id;
}

/**
 * 取得（必要时创建）某个 socket 的等待表，并挂上唯一的消息监听器
 * @param ws 要监听的 websocket
 */
function getWaiters(ws: WebSocket): Map<string, Waiter> {
    const existing = waitersBySocket.get(ws);
    if (existing) {
        return existing;
    }

    const waiters = new Map<string, Waiter>();
    waitersBySocket.set(ws, waiters);

    ws.addEventListener('message', (event: MessageEvent) => {
        let message: { type?: string; msgId?: string } & Record<string, any>;
        try {
            message = JSON.parse(event.data as string);
        } catch (error) {
            console.warn('[SAI] 收到无法解析的回包', error, event.data);
            return;
        }

        console.debug('Receiving message', message);

        // 优先按 msgId 关联；回包没有 msgId 时（老后端）只在仅有一个在途请求时兜底
        let key: string | undefined;
        if (typeof message.msgId === 'string' && waiters.has(message.msgId)) {
            key = message.msgId;
        } else if (message.msgId === undefined && waiters.size === 1) {
            console.warn('[SAI] 回包没有 msgId，按唯一在途请求兜底处理', message);
            key = waiters.keys().next().value;
        }

        if (key === undefined) {
            console.warn('[SAI] 收到无法关联的回包，已忽略', message);
            return;
        }

        const waiter = waiters.get(key)!;
        waiters.delete(key);

        if (message.type === 'result') {
            waiter.resolve(message);
        } else {
            waiter.reject(JSON.stringify(message));
        }
    });

    // 连接断开时把在途请求全部拒掉，避免调用方永远挂着
    const rejectAll = (reason: string) => {
        for (const waiter of waiters.values()) {
            waiter.reject(reason);
        }

        waiters.clear();
    };
    ws.addEventListener('close', () => rejectAll('websocket closed'));
    ws.addEventListener('error', () => rejectAll('websocket error'));

    return waiters;
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
    const waiters = getWaiters(ws);
    const msgId = createMsgId(waiters);

    return new Promise<{ type: string } & T>((resolve, reject) => {
        waiters.set(msgId, { resolve, reject });
        ws.send(JSON.stringify({ ...sendMessage, [MSG_ID_FIELD]: msgId }));
    });
}
