import * as Blockly from 'blockly/core';
import { Backpack, backpackChange } from '@blockly/workspace-backpack';

/**
 * @fileoverview 背包内容的持久化。
 *
 * 插件默认把背包内容注册成 workspace 的序列化器，于是背包会跟着项目存档一起走。
 * 这里改成存在 localStorage 里：背包是「用户自己的收藏」，跨项目共享，
 * 也不会被一起上传到后端。
 */

/** localStorage 里的键名。 */
const STORAGE_KEY = 'sai.blockly.backpack';

/** 读取 localStorage 中的背包内容；没有存过时返回 `null`。 */
function readStoredContents(): string[] | null {
    try {
        const raw = window.localStorage.getItem(STORAGE_KEY);
        if (raw === null) return null;
        const parsed: unknown = JSON.parse(raw);
        if (!Array.isArray(parsed)) return null;
        return parsed.filter((item): item is string => typeof item === 'string');
    } catch (error) {
        console.warn('[SAI] 读取背包内容失败', error);
        return null;
    }
}

/** 写入 localStorage；内容为空时直接删掉这条记录。 */
function writeStoredContents(contents: string[]): void {
    try {
        if (contents.length === 0) window.localStorage.removeItem(STORAGE_KEY);
        else window.localStorage.setItem(STORAGE_KEY, JSON.stringify(contents));
    } catch (error) {
        console.warn('[SAI] 保存背包内容失败', error);
    }
}

/**
 * 安装背包：内容存 localStorage，不注册 workspace 序列化器。
 *
 * @param workspace 目标工作区
 * @returns 背包实例
 */
export function installBackpack(workspace: Blockly.WorkspaceSvg): Backpack {
    const backpack = new Backpack(workspace, { skipSerializerRegistration: true });
    backpack.init();

    const stored = readStoredContents();
    if (stored && stored.length > 0) backpack.setContents(stored);

    // 背包内容变化时立刻落盘（插件每次增删都会派发 backpack_change）
    workspace.addChangeListener(event => {
        if (event.type !== backpackChange) return;
        writeStoredContents(backpack.getContents());
    });

    return backpack;
}
