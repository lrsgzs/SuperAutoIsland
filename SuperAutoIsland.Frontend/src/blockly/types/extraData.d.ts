import { wsWaitMessage } from '../utils/wsUtils';
import * as Blockly from 'blockly';
import { CategoryContent } from '../utils/v2Generator';

declare global {
    interface Window {
        /** 后端送来的分类（完整分类元数据 + 分类下的积木） */
        extraBlocks: CategoryContent[];
        saiWS: WebSocket;
        saiWaitMessage: typeof wsWaitMessage;
        workspace: Blockly.Workspace;
        runCode: (workspace?: Blockly.Workspace) => Promise<void>;
        saveCode: (workspace?: Blockly.Workspace) => Promise<void>;
    }
}

export {};
