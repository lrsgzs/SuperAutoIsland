import * as Blockly from 'blockly/core';

const blockShowAlert = {
    type: 'show_alert',
    message0: 'Show Alert %1',
    args0: [
        {
            type: 'input_value',
            name: 'TEXT',
            check: 'String',
        },
    ],
    previousStatement: null,
    nextStatement: null,
    colour: 160,
    tooltip: '',
    helpUrl: '',
};

export const blocks = Blockly.common.createBlockDefinitionsFromJsonArray([blockShowAlert]);
