import { addLabel, addMetaBlock } from '../utils/superGenerator';
import '../types/extraData.d.ts';
import { wsWaitMessage } from '../utils/wsUtils';

const subjectsResponse = await wsWaitMessage<{ subjects: { Id: string; Name: string; }[] }>(window.saiWS, {
    type: "getSubjects"
})
const subjects: [string, string][] = subjectsResponse.subjects.map(e => [e.Name, e.Id]);

const weathers: [string, string][] = [
    ['晴', '0'],
    ['多云', '1'],
    ['阴', '2'],
    ['阵雨', '3'],
    ['雷阵雨', '4'],
    ['雷阵雨并伴有冰雹', '5'],
    ['雨夹雪', '6'],
    ['小雨', '7'],
    ['中雨', '8'],
    ['大雨', '9'],
    ['暴雨', '10'],
    ['大暴雨', '11'],
    ['特大暴雨', '12'],
    ['阵雪', '13'],
    ['小雪', '14'],
    ['中雪', '15'],
    ['大雪', '16'],
    ['暴雪', '17'],
    ['雾', '18'],
    ['冻雨', '19'],
    ['沙尘暴', '20'],
    ['小雨-中雨', '21'],
    ['中雨-大雨', '22'],
    ['大雨-暴雨', '23'],
    ['暴雨-大暴雨', '24'],
    ['大暴雨-特大暴雨', '25'],
    ['小雪-中雪', '26'],
    ['中雪-大雪', '27'],
    ['大雪-暴雪', '28'],
    ['浮尘', '29'],
    ['扬沙', '30'],
    ['强沙尘暴', '31'],
    ['飑', '32'],
    ['龙卷风', '33'],
    ['弱高吹雪', '34'],
    ['轻雾', '35'],
    ['霾', '53'],
    ['雨', '301'],
    ['雪', '302'],
    ['未知', '99'],
];

addLabel('ClassIsland');

addMetaBlock({
    id: 'classisland.windows.className',
    name: '前台窗口类名',
    icon: ['窗口指纹', '\uF4A2'],
    args: {
        UseRegex: ['正则:', 'checkbox', false],
        Text: ['', 'text'],
    },
    isRule: true,
    inlineBlock: false,
    inlineField: true,
});

addMetaBlock({
    id: 'classisland.windows.text',
    name: '前台窗口标题',
    icon: ['文本字段', '\uF26B'],
    args: {
        UseRegex: ['正则:', 'checkbox', false],
        Text: ['', 'text'],
    },
    isRule: true,
    inlineBlock: false,
    inlineField: true,
});

addMetaBlock({
    id: 'classisland.windows.status',
    name: '前台窗口状态是',
    icon: ['面板独立窗口', '\uEC83'],
    args: {
        State: [
            '',
            'dropdown',
            [
                ['正常', '0'],
                ['最大化', '1'],
                ['最小化', '2'],
                ['全屏', '3'],
            ],
        ],
    },
    isRule: true,
    inlineBlock: true,
    inlineField: true,
    dropdownUseNumbers: true,
});

addMetaBlock({
    id: 'classisland.windows.processName',
    name: '前台窗口进程',
    icon: ['窗口AD人', '\uF488'],
    args: {
        UseRegex: ['正则:', 'checkbox', false],
        Text: ['', 'text'],
    },
    isRule: true,
    inlineBlock: false,
    inlineField: true,
});

addMetaBlock({
    id: 'classisland.lessons.currentSubject',
    name: '科目是',
    icon: ['书', '\uE215'],
    args: {
        SubjectId: ['', 'dropdown', subjects],
    },
    isRule: true,
    inlineBlock: true,
    inlineField: true,
});

addMetaBlock({
    id: 'classisland.lessons.nextSubject',
    name: '下节课科目是',
    icon: ['创建书', '\uE217'],
    args: {
        SubjectId: ['', 'dropdown', subjects],
    },
    isRule: true,
    inlineBlock: true,
    inlineField: true,
});

addMetaBlock({
    id: 'classisland.lessons.previousSubject',
    name: '上节课科目是',
    icon: ['删除书', '\uE226'],
    args: {
        SubjectId: ['', 'dropdown', subjects],
    },
    isRule: true,
    inlineBlock: true,
    inlineField: true,
});

addMetaBlock({
    id: 'classisland.lessons.timeState',
    name: '当前时间状态是',
    icon: ['钟表', '\uE4C4'],
    args: {
        State: [
            '',
            'dropdown',
            [
                ['无', '0'],
                ['上课', '1'],
                ['准备上课（这个没用）', '2'],
                ['课间休息', '3'],
                ['放学后', '4'],
            ],
        ],
    },
    isRule: true,
    inlineBlock: true,
    inlineField: true,
    dropdownUseNumbers: true,
});

addMetaBlock({
    id: 'classisland.weather.currentWeather',
    name: '当前天气是',
    icon: ['多云', '\uE4DC'],
    args: {
        WeatherId: ['', 'dropdown', weathers],
    },
    isRule: true,
    inlineBlock: true,
    inlineField: true,
    dropdownUseNumbers: true,
});

addMetaBlock({
    id: 'classisland.weather.hasWeatherAlert',
    name: '存在气象预警',
    icon: ['警告', '\uF431'],
    args: {
        UseRegex: ['正则:', 'checkbox', false],
        Text: ['', 'text'],
    },
    isRule: true,
    inlineBlock: false,
    inlineField: true,
});

addMetaBlock({
    id: 'classisland.weather.rainTime',
    name: '距离降水开始/结束还剩',
    icon: ['雨', '\uF43F'],
    args: {
        label1: ['\n', 'dummy'], // dummy 废了？强行换行！
        IsRemainingTime: ['是否为距离结束:', 'checkbox', false],
        RainTimeMinutes: ['距离开始/结束剩余时间（分钟）', 'number'],
    },
    isRule: true,
    inlineBlock: false,
    inlineField: false,
});

for (let pluginName in window.extraBlocks) {
    if (window.extraBlocks[pluginName].rules.length == 0) continue;

    addLabel(pluginName);
    for (let block of window.extraBlocks[pluginName].rules) {
        addMetaBlock(block);
    }
}
