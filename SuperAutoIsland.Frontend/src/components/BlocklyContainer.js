import * as React from 'react';
import { useEffect, useRef } from 'react';
import { injectBlockly } from '../blockly';
export default function BlocklyContainer({ ...props }) {
    const containerRef = useRef(null);
    const workspace = useRef(null);
    useEffect(() => {
        (async () => {
            if (containerRef.current) {
                workspace.current = await injectBlockly(containerRef.current);
            }
        })();
    }, [containerRef]);
    return React.createElement("div", { ...props, ref: containerRef });
}
