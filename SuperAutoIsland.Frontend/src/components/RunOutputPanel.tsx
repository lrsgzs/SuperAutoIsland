import * as React from 'react';

export type RunLogLine = {
    level: string;
    text: string;
};

type RunOutputPanelProps = {
    /** 是否显示 */
    open: boolean;
    /** 关闭（日志保留，下次运行/手动打开时还在） */
    onClose: () => void;
    /** 清空日志 */
    onClear: () => void;
    /** 状态栏文字 */
    status: string;
    logs: RunLogLine[];
};

const LEVEL_TEXT_CLASS: Record<string, string> = {
    DEBUG: 'text-neutral-500',
    INFO: 'text-neutral-800',
    WARN: 'text-amber-700',
    ERROR: 'text-red-700',
};

const PANEL_WIDTH = 560;
const PANEL_HEIGHT = 280;
const MARGIN = 16;

/**
 * 浮动的运行输出面板：标题栏可拖动，可关闭
 *
 * 位置只存在组件内部，被关掉时组件不卸载，所以拖到哪儿下次还在哪儿。
 */
export default function RunOutputPanel({ open, onClose, onClear, status, logs }: RunOutputPanelProps) {
    const panelRef = React.useRef<HTMLDivElement>(null);
    const logBoxRef = React.useRef<HTMLDivElement>(null);
    const dragOffsetRef = React.useRef<{ x: number; y: number } | null>(null);
    const [position, setPosition] = React.useState<{ x: number; y: number } | null>(null);

    // 有新日志就滚到底
    React.useEffect(() => {
        const box = logBoxRef.current;
        if (box) {
            box.scrollTop = box.scrollHeight;
        }
    }, [logs, open]);

    if (!open) {
        return null;
    }

    const handlePointerDown = (event: React.PointerEvent<HTMLDivElement>) => {
        // 标题栏上的按钮不参与拖动
        if ((event.target as HTMLElement).closest('button')) {
            return;
        }

        const rect = panelRef.current?.getBoundingClientRect();
        if (!rect) {
            return;
        }

        dragOffsetRef.current = { x: event.clientX - rect.left, y: event.clientY - rect.top };
        event.currentTarget.setPointerCapture(event.pointerId);
        event.preventDefault();
    };

    const handlePointerMove = (event: React.PointerEvent<HTMLDivElement>) => {
        const offset = dragOffsetRef.current;
        if (!offset) {
            return;
        }

        const maxX = Math.max(0, window.innerWidth - PANEL_WIDTH);
        const maxY = Math.max(0, window.innerHeight - PANEL_HEIGHT);
        setPosition({
            x: Math.min(Math.max(0, event.clientX - offset.x), maxX),
            y: Math.min(Math.max(0, event.clientY - offset.y), maxY),
        });
    };

    const handlePointerUp = (event: React.PointerEvent<HTMLDivElement>) => {
        if (event.currentTarget.hasPointerCapture(event.pointerId)) {
            event.currentTarget.releasePointerCapture(event.pointerId);
        }

        dragOffsetRef.current = null;
    };

    return (
        <div
            ref={panelRef}
            className="fixed z-50 flex flex-col overflow-hidden rounded-xl border border-neutral-400 bg-white shadow-xl"
            style={
                position
                    ? { left: position.x, top: position.y, width: PANEL_WIDTH, height: PANEL_HEIGHT }
                    : { right: MARGIN, bottom: MARGIN, width: PANEL_WIDTH, height: PANEL_HEIGHT }
            }
        >
            <div
                className="flex items-center gap-2 px-2 py-1 text-xs bg-neutral-200 border-b border-neutral-300 cursor-move select-none touch-none"
                onPointerDown={handlePointerDown}
                onPointerMove={handlePointerMove}
                onPointerUp={handlePointerUp}
                onPointerCancel={handlePointerUp}
            >
                <span className="font-medium">运行输出</span>
                <span className="flex-1 truncate text-neutral-600">{status}</span>

                <button
                    className="px-2 py-0.5 rounded border border-neutral-400 bg-white hover:bg-neutral-100 transition"
                    onClick={onClear}
                >
                    清空
                </button>
                <button
                    className="px-2 py-0.5 rounded border border-neutral-400 bg-white hover:bg-neutral-100 transition"
                    onClick={onClose}
                >
                    关闭
                </button>
            </div>

            <div ref={logBoxRef} className="flex-1 overflow-auto px-2 py-1 font-mono text-xs leading-5 bg-neutral-50">
                {logs.length === 0 ? (
                    <span className="text-neutral-400">（暂无输出）</span>
                ) : (
                    logs.map((line, index) => (
                        <div key={index} className={LEVEL_TEXT_CLASS[line.level] ?? 'text-neutral-800'}>
                            [{line.level}] {line.text}
                        </div>
                    ))
                )}
            </div>
        </div>
    );
}
