import { wsWaitMessage } from '../utils/wsUtils';
import * as Blockly from 'blockly';
import { BlockMetadata } from '../utils/v2Generator';

declare global {
    interface Window {
        extraBlocks: Record<string, BlockMetadata[]>;
        saiWS: WebSocket;
        saiWaitMessage: typeof wsWaitMessage;
        workspace: Blockly.Workspace;
        runCode: (workspace?: Blockly.Workspace) => Promise<void>;
        saveCode: (workspace?: Blockly.Workspace) => Promise<void>;
    }
}

export {};
