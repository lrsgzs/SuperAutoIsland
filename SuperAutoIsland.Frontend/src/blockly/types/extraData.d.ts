import { onServerPush, wsWaitMessage } from '../utils/wsUtils';
import * as Blockly from 'blockly';
import { CategoryContent } from '../utils/v2Generator';

declare global {
    interface Window {
        /** 后端送来的分类（完整分类元数据 + 分类下的积木） */
        extraBlocks: CategoryContent[];
        saiWS: WebSocket;
        saiWaitMessage: typeof wsWaitMessage;
        /** 订阅后端主动推送的消息（log / runFinished / socketClosed） */
        saiOnServerPush: typeof onServerPush;
        workspace: Blockly.Workspace;
        /** 在后端 Jint 里跑，日志和结果由后端推送回来 */
        runCode: (workspace?: Blockly.Workspace) => Promise<unknown>;
        /** 停止当前正在跑的脚本 */
        stopRun: () => Promise<unknown>;
        saveCode: (workspace?: Blockly.Workspace) => Promise<void>;
    }
}

export {};
