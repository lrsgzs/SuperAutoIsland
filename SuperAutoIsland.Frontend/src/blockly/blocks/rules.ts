import { addMetaBlock } from '../utils/superGenerator';

addMetaBlock({
    id: 'classisland.windows.className',
    name: '前台窗口类名',
    icon: ['最大化', '\uEB6B'],
    args: {
        UseRegex: ['正则:', 'checkbox', false],
        Text: ['', 'text'],
    },
    isRule: true,
    inlineBlock: false,
    inlineField: true,
});
