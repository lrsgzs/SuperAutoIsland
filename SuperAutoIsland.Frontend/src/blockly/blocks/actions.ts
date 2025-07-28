import { addBlock } from '../utils/blockGenerator';
import { Order } from 'blockly/javascript';
import { addMetaBlock } from '../utils/superGenerator';

const componentsConfigs: [string, string][] = [['Default', 'Default']];

// TODO: 审查 dropdown 是否真的为目标类型

addMetaBlock({
    id: 'classisland.broadcastSignal',
    name: '广播信号',
    icon: ['广播', '\uE561'],
    args: {
        SignalName: ['', 'text'],
    },
    inlineBlock: false,
    inlineField: false,
});

addMetaBlock({
    id: 'classisland.settings.currentComponentConfig',
    name: '组件配置方案',
    icon: ['组件', '\uE06F'],
    args: {
        Value: ['修改为', 'dropdown', componentsConfigs],
    },
    inlineBlock: true,
    inlineField: true,
});

addMetaBlock({
    id: 'classisland.settings.theme',
    name: '应用主题',
    icon: ['主题', '\uE5CB'],
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
    inlineBlock: true,
    inlineField: true,
    dropdownUseNumbers: true,
});

addMetaBlock({
    id: 'classisland.settings.windowDockingLocation',
    name: '窗口停靠位置',
    icon: ['TV', '\uF397'],
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
    inlineBlock: true,
    inlineField: true,
    dropdownUseNumbers: true,
});

addMetaBlock({
    id: 'classisland.settings.windowLayer',
    name: '窗口层级',
    icon: ['层级', '\uEA2F'],
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
    inlineBlock: true,
    inlineField: true,
    dropdownUseNumbers: true,
});

addMetaBlock({
    id: 'classisland.settings.windowDockingOffsetX',
    name: '窗口向右偏移',
    icon: ['左右箭头', '\uE099'],
    args: {
        Value: ['修改为', 'number'],
    },
    inlineBlock: false,
    inlineField: false,
});

addMetaBlock({
    id: 'classisland.settings.windowDockingOffsetY',
    name: '窗口向下偏移',
    icon: ['下箭头', '\uE094'],
    args: {
        Value: ['修改为', 'number'],
    },
    inlineBlock: false,
    inlineField: false,
});

addMetaBlock({
    id: 'classisland.os.run',
    name: '运行',
    icon: ['打开', '\uEC2E'],
    args: {
        Value: ['', 'text'],
        Arg: ['应用程序启动参数', 'text'],
    },
    inlineBlock: false,
    inlineField: false,
});

addMetaBlock({
    id: 'classisland.showNotification',
    name: '显示提醒',
    icon: ['提醒', '\uE02B'],
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
    inlineBlock: false,
    inlineField: false,
});

// 不使用 MetaBlock
addBlock(
    {
        type: 'classisland_action_sleep',
        message: '[%1 等待时长] %2 秒',
        inputs: {
            ICON: {
                type: 'field_icon',
                data: {
                    text: '沙漏',
                    icon: '\uE9AE',
                },
            },
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
    icon: ['多云', '\uF44F'],
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
    inlineBlock: false,
    inlineField: false,
    dropdownUseNumbers: true,
});

addMetaBlock({
    id: 'classisland.app.quit',
    name: '退出 ClassIsland',
    icon: ['退出', '\uE0DE'],
    args: {},
    inlineBlock: false,
    inlineField: false,
});

addMetaBlock({
    id: 'classisland.app.restart',
    name: '重启 ClassIsland',
    icon: ['转圈箭头', '\uE0BD'],
    args: {
        Value: ['静默重启:', 'boolean'],
    },
    inlineBlock: false,
    inlineField: false,
});
