import * as Blockly from 'blockly/core';

const storageKey = 'mainWorkspace';

export const save = function (workspace: Blockly.Workspace) {
    const data = Blockly.serialization.workspaces.save(workspace);
    window.localStorage?.setItem(storageKey, JSON.stringify(data));
};

export const load = function (workspace: Blockly.Workspace) {
    const data = window.localStorage?.getItem(storageKey);
    if (!data) return;

    Blockly.Events.disable();
    Blockly.serialization.workspaces.load(JSON.parse(data), workspace, undefined);
    Blockly.Events.enable();
};
