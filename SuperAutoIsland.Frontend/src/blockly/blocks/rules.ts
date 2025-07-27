import { addMetaBlock } from '../utils/superGenerator';

addMetaBlock({
    id: 'classisland.windows.className',
    name: '前台窗口类名',
    args: {
        UseRegex: ['正则:', 'boolean'],
        Text: ['匹配内容:', 'text'],
    },
    isRule: true,
});
