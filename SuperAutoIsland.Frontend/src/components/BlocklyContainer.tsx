import * as React from 'react';
import { HTMLAttributes, useEffect, useRef } from 'react';
import { Blockly, injectBlockly } from '../blockly';

export default function BlocklyContainer({ ...props }: HTMLAttributes<HTMLDivElement>) {
    const containerRef = useRef<HTMLDivElement>(null);
    const workspace = useRef<Blockly.Workspace>(null);
    useEffect(() => {
        (async () => {
            if (containerRef.current) {
                workspace.current = await injectBlockly(containerRef.current);
            }
        })();
    }, [containerRef]);

    return <div {...props} ref={containerRef} />;
}
