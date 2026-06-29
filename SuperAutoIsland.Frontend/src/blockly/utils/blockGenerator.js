export const data = {
    initialized: false,
};
/**
 * 配置环境
 */
export function setup(forBlock, category, blocks) {
    data.forBlock = forBlock;
    data.category = category;
    data.blocks = blocks;
    data.initialized = true;
}
/**
 * 生成积木
 * @param block 积木定义
 */
export function generateBlock(block) {
    if (!data.initialized)
        throw new Error('未初始化 generator!');
    const toolboxInputs = {};
    const definitionArgs = [];
    for (let inputName in block.inputs) {
        if (block.inputs[inputName].type == 'input_value') {
            const toolboxFields = {};
            for (let fieldName in block.inputs[inputName].fields) {
                // @ts-ignore
                toolboxFields[fieldName] = block.inputs[inputName].fields[fieldName];
            }
            if (block.inputs[inputName].blockType) {
                toolboxInputs[inputName] = {
                    shadow: {
                        type: block.inputs[inputName].blockType,
                        fields: toolboxFields,
                    },
                };
            }
        }
        definitionArgs.push({
            name: inputName,
            ...block.inputs[inputName],
            ...block.inputs[inputName].data,
        });
    }
    return {
        toolbox: {
            kind: 'block',
            type: block.type,
            inputs: toolboxInputs,
            inline: block.inline,
        },
        definition: {
            type: block.type,
            message0: block.message,
            // @ts-ignore
            args0: definitionArgs,
            output: block.output,
            tooltip: block.tooltip,
            helpUrl: block.helpUrl,
            colour: block.colour,
            style: block.style,
            ...(block.isReporter
                ? {}
                : {
                    previousStatement: null,
                    nextStatement: null,
                }),
        },
    };
}
/**
 * 在当前环境添加积木
 * @param block 积木定义
 * @param generator 代码生成器函数
 */
export function addBlock(block, generator) {
    if (!data.initialized)
        throw new Error('未初始化 generator!');
    const blockDefinition = generateBlock(block);
    data.category?.contents.push(blockDefinition.toolbox);
    data.blocks?.push(blockDefinition.definition);
    data.forBlock[block.type] = generator;
}
