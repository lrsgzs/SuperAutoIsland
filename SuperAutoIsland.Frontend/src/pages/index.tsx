import * as React from 'react';
import { createRoot } from 'react-dom/client';
import '../styles/base.css';
import BlocklyContainer from '../components/BlocklyContainer';

function IndexPage() {
    return <BlocklyContainer className="w-full h-full" />;
}

const dom = document.getElementById('app');
if (dom) {
    const root = createRoot(dom);
    root.render(<IndexPage />);
} else {
    throw new Error('Cannot find dom element #app');
}
