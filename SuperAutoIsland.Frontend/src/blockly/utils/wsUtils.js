/**
 * websocket 发送消息并等待传回数据
 * @param ws 要发送的 websocket
 * @param sendMessage 要发送的消息
 */
export function wsWaitMessage(ws, sendMessage) {
    let promiseResolve;
    let promiseReject;
    const waiter = new Promise((resolve, reject) => {
        promiseResolve = resolve;
        promiseReject = reject;
    });
    const resolver = (event) => {
        const message = JSON.parse(event.data);
        if (message.type === 'result') {
            promiseResolve(message);
        }
        else {
            promiseReject(JSON.stringify(message));
        }
        ws.removeEventListener('message', resolver);
    };
    ws.addEventListener('message', resolver);
    ws.send(JSON.stringify(sendMessage));
    return waiter;
}
