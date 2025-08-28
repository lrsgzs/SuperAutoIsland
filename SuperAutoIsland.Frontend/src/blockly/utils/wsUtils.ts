export function wsWaitMessage<T>(ws: WebSocket, sendMessage: any) {
    let promiseResolve: (value: ({ type: string } & T) | PromiseLike<{ type: string } & T>) => void;
    let promiseReject: (reason?: any) => void;
    const waiter = new Promise<{ type: string } & T>((resolve, reject) => {
        promiseResolve = resolve;
        promiseReject = reject;
    });

    const resolver = (event: MessageEvent) => {
        const message: { type: string } & T = JSON.parse(event.data as string);
        if (message.type === 'result') {
            promiseResolve(message);
        } else {
            promiseReject(JSON.stringify(message));
        }
        ws.removeEventListener('message', resolver);
    };
    ws.addEventListener('message', resolver);

    ws.send(JSON.stringify(sendMessage));
    return waiter;
}
