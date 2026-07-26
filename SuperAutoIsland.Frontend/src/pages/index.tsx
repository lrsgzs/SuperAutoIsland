import * as React from 'react';
import { createRoot } from 'react-dom/client';
import '../styles/base.css';
import BlocklyContainer from '../components/BlocklyContainer';

function IndexPage() {
    let saveCode = React.useCallback(async () => {
        try {
            await window.saveCode(window.workspace);
            alert("保存成功");
        } catch (e) {
            console.error('保存失败', e);
            alert(`保存失败 ${e}`);
        }
    }, [])
    
    return (
        <div className="grid grid-rows-[auto_1fr] h-full">
            <div className="w-full p-2 flex gap-2 bg-neutral-100">
                <img src="/favicon.ico" alt="logo" className="self-center w-[32px] h-[32px]" />

                <span className="content-center">SuperAutoIsland Blockly 编辑器</span>

                <div className="flex-1" />

                <button
                    className="p-1 px-2 bg-neutral-300 border border-neutral-600 rounded-xl hover:bg-neutral-400 transition"
                    onClick={saveCode}
                >
                    保存
                </button>

                <button
                    className="p-1 px-2 bg-neutral-300 border border-neutral-600 rounded-xl hover:bg-neutral-400 transition"
                    onClick={() => window.runCode(window.workspace)}
                >
                    运行代码
                </button>
            </div>

            <BlocklyContainer className="w-full" />
        </div>
    );
}

const dom = document.getElementById('app');
if (dom) {
    const root = createRoot(dom);
    root.render(<IndexPage />);
} else {
    throw new Error('Cannot find dom element #app');
}
