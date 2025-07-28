import { addMetaBlock } from '../utils/superGenerator';

addMetaBlock({
    id: 'classisland.windows.className',
    name: '前台窗口类名',
    args: {
        UseRegex: ['正则:', 'checkbox', false],
        Text: ['', 'text'],
    },
    isRule: true,
    inlineBlock: false,
    inlineField: true,
});
