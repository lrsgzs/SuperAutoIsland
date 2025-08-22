import type { Metadata } from '../utils/superGenerator';
import { wsWaitMessage } from '../utils/wsUtils';

declare global {
    interface Window {
        extraBlocks: Record<string, Record<'rules' | 'actions', Metadata[]>>;
        saiWS: WebSocket;
        saiWaitMessage: typeof wsWaitMessage;
    }
}

export {};
