import * as React from 'react';
import { createRoot } from 'react-dom/client';
import '../styles/base.css';
import BlocklyContainer from '../components/BlocklyContainer';
function IndexPage() {
    let saveCode = React.useCallback(async () => {
        await window.saveCode(window.workspace);
        alert("保存成功");
    }, []);
    return (React.createElement(React.Fragment, null,
        React.createElement(BlocklyContainer, { className: "w-full h-full" }),
        React.createElement("div", { className: "absolute top-2 right-2 flex gap-2" },
            React.createElement("button", { className: "p-2 bg-neutral-300 rounded-2xl hover:bg-neutral-400", onClick: saveCode }, "\u4FDD\u5B58"),
            React.createElement("button", { className: "p-2 bg-neutral-300 rounded-2xl hover:bg-neutral-400", onClick: () => window.runCode(window.workspace) }, "\u8FD0\u884C\u4EE3\u7801"))));
}
const dom = document.getElementById('app');
if (dom) {
    const root = createRoot(dom);
    root.render(React.createElement(IndexPage, null));
}
else {
    throw new Error('Cannot find dom element #app');
}
