import { addMetaBlock } from '../utils/superGenerator';

// TODO: 审查 dropdown 是否真的为目标类型

const lessons: [string, string][] = [
    ['语文', 'guid1'],
    ['数学', 'guid2'],
    ['英语', 'guid3'],
];

const weathers: [string, string][] = [
    ['晴', '0'],
    ['阴', '1'],
    ['多云', '2'],
];

addMetaBlock({
    id: 'classisland.windows.className',
    name: '前台窗口类名',
    icon: ['未知', '\uEDFB'],
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
    icon: ['未知', '\uEDFB'],
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
    icon: ['未知', '\uEDFB'],
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
    icon: ['未知', '\uEDFB'],
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
    icon: ['未知', '\uEDFB'],
    args: {
        SubjectId: ['', 'dropdown', lessons],
    },
    isRule: true,
    inlineBlock: true,
    inlineField: true,
});

addMetaBlock({
    id: 'classisland.lessons.nextSubject',
    name: '下节课科目是',
    icon: ['未知', '\uEDFB'],
    args: {
        SubjectId: ['', 'dropdown', lessons],
    },
    isRule: true,
    inlineBlock: true,
    inlineField: true,
});

addMetaBlock({
    id: 'classisland.lessons.previousSubject',
    name: '上节课科目是',
    icon: ['未知', '\uEDFB'],
    args: {
        SubjectId: ['', 'dropdown', lessons],
    },
    isRule: true,
    inlineBlock: true,
    inlineField: true,
});

addMetaBlock({
    id: 'classisland.lessons.timeState',
    name: '当前时间状态是',
    icon: ['未知', '\uEDFB'],
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
    icon: ['未知', '\uEDFB'],
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
    name: '当前天气是',
    icon: ['未知', '\uEDFB'],
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
    icon: ['未知', '\uEDFB'],
    args: {
        label1: ['\n', 'dummy'], // dummy 废了？强行换行！
        IsRemainingTime: ['是否为剩余时间:', 'checkbox', false],
        RainTimeMinutes: ['距离开始/结束剩余时间（分钟）', 'number'],
    },
    isRule: true,
    inlineBlock: false,
    inlineField: false,
});
