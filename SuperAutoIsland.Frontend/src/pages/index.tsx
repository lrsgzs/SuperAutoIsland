import * as React from 'react';
import { createRoot } from 'react-dom/client';
import '../styles/base.css';
import BlocklyContainer from '../components/BlocklyContainer';
import RunOutputPanel, { type RunLogLine } from '../components/RunOutputPanel';

const buttonClass = 'p-1 px-2 border rounded-xl transition';
const neutralButtonClass = `${buttonClass} border-neutral-600 bg-neutral-300 hover:bg-neutral-400`;
const runButtonClass = `${buttonClass} border-sky-700 bg-sky-200 hover:bg-sky-300`;
const stopButtonClass = `${buttonClass} border-pink-700 bg-pink-200 hover:bg-pink-300 disabled:opacity-40 disabled:hover:bg-pink-200`;
const outputToggleOnClass = `${buttonClass} border-neutral-700 bg-neutral-500 text-white hover:bg-neutral-600`;
const outputToggleOffClass = neutralButtonClass;

function IndexPage() {
    const [logs, setLogs] = React.useState<RunLogLine[]>([]);
    const [status, setStatus] = React.useState('就绪');
    const [running, setRunning] = React.useState(false);
    const [outputOpen, setOutputOpen] = React.useState(false);

    const appendLog = React.useCallback((level: string, text: string) => {
        setLogs(previous => [...previous, { level, text }].slice(-500));
    }, []);

    // 后端推送：运行日志 / 运行结果
    React.useEffect(() => {
        return window.saiOnServerPush(window.saiWS, message => {
            switch (message.type) {
                case 'log':
                    appendLog(String(message.level ?? 'INFO'), String(message.message ?? ''));
                    break;
                case 'runFinished':
                    setRunning(false);
                    if (message.cancelled) {
                        setStatus('已停止');
                    } else if (message.ok) {
                        setStatus(`运行完成（${message.elapsedMs}ms）`);
                    } else {
                        setStatus('运行失败');
                        appendLog('ERROR', String(message.error ?? '未知错误'));
                    }

                    break;
                case 'socketClosed':
                    setRunning(false);
                    setStatus(`连接已断开：${message.reason ?? ''}`);
                    break;
            }
        });
    }, [appendLog]);

    const saveCode = React.useCallback(async () => {
        try {
            await window.saveCode(window.workspace);
            setStatus('已保存');
            alert('保存成功');
        } catch (e) {
            console.error('保存失败', e);
            setStatus('保存失败');
            alert(`保存失败 ${e}`);
        }
    }, []);

    const runCode = React.useCallback(async () => {
        setLogs([]);
        setStatus('运行中…');
        setRunning(true);
        setOutputOpen(true);

        try {
            await window.runCode(window.workspace);
        } catch (e) {
            setRunning(false);
            setStatus('运行启动失败');
            appendLog('ERROR', String(e));
        }
    }, [appendLog]);

    const stopRun = React.useCallback(async () => {
        try {
            await window.stopRun();
            setStatus('正在停止…');
        } catch (e) {
            appendLog('ERROR', String(e));
        }
    }, [appendLog]);

    return (
        <div className="grid grid-rows-[auto_1fr] h-full">
            <div className="w-full p-2 flex gap-2 bg-neutral-100">
                <img src="/favicon.ico" alt="logo" className="self-center w-[32px] h-[32px]" />

                <span className="content-center">SuperAutoIsland Blockly 编辑器</span>

                <div className="flex-1" />

                <button className={neutralButtonClass} onClick={saveCode}>
                    保存
                </button>

                <button
                    className={`${runButtonClass} disabled:opacity-40 disabled:hover:bg-sky-200`}
                    disabled={running}
                    onClick={runCode}
                >
                    {running ? '运行中…' : '运行'}
                </button>

                <button className={stopButtonClass} disabled={!running} onClick={stopRun}>
                    停止
                </button>

                <button
                    className={outputOpen ? outputToggleOnClass : outputToggleOffClass}
                    onClick={() => setOutputOpen(open => !open)}
                >
                    运行输出
                </button>
            </div>

            <BlocklyContainer className="w-full" />

            <RunOutputPanel
                open={outputOpen}
                logs={logs}
                status={status}
                onClose={() => setOutputOpen(false)}
                onClear={() => setLogs([])}
            />
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
