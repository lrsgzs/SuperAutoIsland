import type { Metadata } from '../utils/superGenerator';
import { wsWaitMessage } from '../utils/wsUtils';
import * as Blockly from 'blockly';

declare global {
    interface Window {
        extraBlocks: Record<string, Record<'rules' | 'actions', Metadata[]>>;
        saiWS: WebSocket;
        saiWaitMessage: typeof wsWaitMessage;
        workspace: Blockly.Workspace;
        runCode: (workspace?: Blockly.Workspace) => void;
        saveCode: (workspace?: Blockly.Workspace) => void;
    }
}

export {};
