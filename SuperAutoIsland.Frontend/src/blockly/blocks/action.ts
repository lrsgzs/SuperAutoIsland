import { addBlock, type ArgDefinition } from '../utils/blockGenerator';
import { Order } from 'blockly/javascript';
import { addMetaBlock } from '../utils/superGenerator';

const quote_ = (text: string) => {
    // @ts-ignore
    return "'" + text.replaceAll("'", "\\'") + "'";
};

const componentsConfigs: [string, string][] = [['Default', 'Default']];

addMetaBlock({
    id: 'classisland.broadcastSignal',
    name: '广播信号',
    args: {
        SignalName: ['', 'text'],
    },
    inline: false,
});

addMetaBlock({
    id: 'classisland.settings.currentComponentConfig',
    name: '组件配置方案',
    args: {
        Value: ['修改为', 'dropdown', componentsConfigs],
    },
    inline: true,
});

addMetaBlock({
    id: 'classisland.settings.theme',
    name: '应用主题',
    args: {
        Value: [
            '修改为',
            'dropdown',
            [
                ['跟随系统', '0'],
                ['明亮', '1'],
                ['黑暗', '2'],
            ],
        ],
    },
    inline: true,
    dropdownUseNumbers: true,
});

addMetaBlock({
    id: 'classisland.settings.windowDockingLocation',
    name: '窗口停靠位置',
    args: {
        Value: [
            '移动到',
            'dropdown',
            [
                ['左上角', '0'],
                ['中上侧', '1'],
                ['右上角', '2'],
                ['左下角', '3'],
                ['中下侧', '4'],
                ['右下角', '5'],
            ],
        ],
    },
    inline: true,
    dropdownUseNumbers: true,
});

addMetaBlock({
    id: 'classisland.settings.windowLayer',
    name: '窗口层级',
    args: {
        Value: [
            '',
            'dropdown',
            [
                ['置底', '0'],
                ['置顶', '1'],
            ],
        ],
    },
    inline: true,
    dropdownUseNumbers: true,
});

addMetaBlock({
    id: 'classisland.settings.windowDockingOffsetX',
    name: '窗口向右偏移',
    args: {
        Value: ['修改为', 'number'],
    },
    inline: false,
});

addMetaBlock({
    id: 'classisland.settings.windowDockingOffsetY',
    name: '窗口向下偏移',
    args: {
        Value: ['修改为', 'number'],
    },
    inline: false,
});

addMetaBlock({
    id: 'classisland.os.run',
    name: '运行',
    args: {
        Value: ['', 'text'],
        Arg: ['应用程序启动参数', 'text'],
    },
    inline: false,
});

addMetaBlock({
    id: 'classisland.showNotification',
    name: '显示提醒',
    args: {
        label1: ['提醒内容设置', 'dummy'],
        Mask: ['遮罩内容', 'text'],
        MaskDurationSeconds: ['遮罩持续时长(秒)', 'number'],
        Content: ['正文内容', 'text'],
        ContentDurationSeconds: ['正文持续时长(秒)', 'number'],
        IsMaskSpeechEnabled: ['启用遮罩语音', 'boolean'],
        IsContentSpeechEnabled: ['启用正文语音', 'boolean'],
        label2: ['高级设置', 'dummy'],
        IsTopmostEnabled: ['置顶提醒', 'boolean'],
        IsEffectEnabled: ['启用提醒特效', 'boolean'],
        IsSoundEffectEnabled: ['启用提醒音效', 'boolean'],
    },
    inline: false,
});

// 不使用 MetaBlock
addBlock(
    {
        type: 'classisland_action_sleep',
        message: '[等待时长] %1 秒',
        inputs: {
            SECONDS: {
                type: 'input_value',
                blockType: 'math_number',
                check: 'Number',
                fields: {
                    NUM: 5,
                },
            },
        },
        inline: true,
        style: 'my_blocks',
    },
    (block, generator) => {
        const value = generator.valueToCode(block, 'SECONDS', Order.NONE) || "''";
        const wrapper = generator.provideFunction_(
            'classisland_action_sleep',
            `async function ${generator.FUNCTION_NAME_PLACEHOLDER_}(value) {
                const actionId = "classisland.action.sleep";
                const actionData = { Value: value };
                console.log("Internal Action", actionId, actionData);
                await new Promise(resolve => {
                    setTimeout(resolve, actionData.Value * 1000);
                });
            }`,
        );
        return `await ${wrapper}(${value});\n`;
    },
);

addMetaBlock({
    id: 'classisland.notification.weather',
    name: '显示天气提醒',
    args: {
        Value: [
            '',
            'dropdown',
            [
                ['三天天气预报', '0'],
                ['气象预警', '1'],
                ['逐小时天气预报', '2'],
            ],
        ],
    },
    inline: false,
    dropdownUseNumbers: true,
});

addMetaBlock({
    id: 'classisland.app.quit',
    name: '退出 ClassIsland',
    args: {},
    inline: false,
});

addMetaBlock({
    id: 'classisland.app.restart',
    name: '重启 ClassIsland',
    args: {
        Value: ['静默重启:', 'boolean'],
    },
    inline: false,
});
