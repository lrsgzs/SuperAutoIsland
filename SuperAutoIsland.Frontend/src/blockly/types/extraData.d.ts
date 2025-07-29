import type { Metadata } from '../utils/superGenerator';

declare global {
    interface Window {
        extraBlocks: Record<string, Record<'rules' | 'actions', Metadata[]>>;
    }
}

export {};
