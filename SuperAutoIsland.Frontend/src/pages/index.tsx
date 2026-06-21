import * as React from 'react';
import { createRoot } from 'react-dom/client';
import '../styles/base.css';
import BlocklyContainer from '../components/BlocklyContainer';

function IndexPage() {
    let saveCode = React.useCallback(async () => {
        await window.saveCode(window.workspace);
        alert("保存成功");
    }, [])
    
    return (
        <>
            <BlocklyContainer className="w-full h-full" />
            <div className="absolute top-2 right-2 flex gap-2">
                <button className="p-2 bg-neutral-300 rounded-2xl hover:bg-neutral-400" 
                        onClick={saveCode}>
                    保存
                </button>
                <button className="p-2 bg-neutral-300 rounded-2xl hover:bg-neutral-400"
                        onClick={() => window.runCode(window.workspace)}>
                    运行代码
                </button>
            </div>
        </>
    );
}

const dom = document.getElementById('app');
if (dom) {
    const root = createRoot(dom);
    root.render(<IndexPage />);
} else {
    throw new Error('Cannot find dom element #app');
}
