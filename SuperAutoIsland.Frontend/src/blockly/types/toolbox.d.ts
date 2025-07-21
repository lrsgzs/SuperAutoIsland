/**
 * Fixed Blockly Toolbox Type Define
 */

import type { ConnectionState } from 'blockly/core/serialization/blocks.js';
import type { CssConfig as CategoryCssConfig } from 'blockly/core/toolbox/category.js';
import type { CssConfig as SeparatorCssConfig } from 'blockly/core/toolbox/separator.js';

/**
 * The information needed to create a block in the toolbox.
 * Note that disabled has a different type for backwards compatibility.
 */
export interface BlockInfo {
    kind: 'block';
    blockxml?: string | Node;
    type?: string;
    gap?: string | number;
    disabled?: string | boolean;
    disabledReasons?: string[];
    enabled?: boolean;
    id?: string;
    collapsed?: boolean;
    inline?: boolean;
    data?: string;
    extraState?: any;
    icons?: {
        [key: string]: any;
    };
    fields?: {
        [key: string]: any;
    };
    inputs?: {
        [key: string]: ConnectionState;
    };
    next?: ConnectionState;
}

/**
 * The information needed to create a separator in the toolbox.
 */
export interface SeparatorInfo {
    kind: 'sep';
    id?: string;
    gap?: number;
    cssconfig?: SeparatorCssConfig;
}

/**
 * The information needed to create a button in the toolbox.
 */
export interface ButtonInfo {
    kind: 'button';
    text: string;
    callbackkey: string;
}

/**
 * The information needed to create a label in the toolbox.
 */
export interface LabelInfo {
    kind: 'label';
    text: string;
    id?: string;
}

/**
 * The information needed to create either a button or a label in the flyout.
 */
export type ButtonOrLabelInfo = ButtonInfo | LabelInfo;

/**
 * The information needed to create a category in the toolbox.
 */
export interface StaticCategoryInfo {
    kind: 'category';
    name: string;
    contents: ToolboxItemInfo[];
    id?: string;
    categorystyle?: string;
    colour?: string;
    cssconfig?: CategoryCssConfig;
    hidden?: string;
    expanded?: string | boolean;
}

/**
 * The information needed to create a custom category.
 */
export interface DynamicCategoryInfo {
    kind: 'category';
    custom: 'VARIABLE' | 'VARIABLE_DYNAMIC' | 'PROCEDURE';
    id?: string;
    categorystyle?: string;
    colour?: string;
    cssconfig?: CategoryCssConfig;
    hidden?: string;
    expanded?: string | boolean;
}

/**
 * The information needed to create either a dynamic or static category.
 */
export type CategoryInfo = StaticCategoryInfo | DynamicCategoryInfo;

/**
 * All the different types that can be displayed in a flyout.
 */
export type FlyoutItemInfo = BlockInfo | SeparatorInfo | ButtonInfo | LabelInfo | DynamicCategoryInfo;

/**
 * Any information that can be used to create an item in the toolbox.
 */
export type ToolboxItemInfo = FlyoutItemInfo | StaticCategoryInfo;

/**
 * The JSON definition of a toolbox.
 */
export interface ToolboxInfo {
    kind?: 'categoryToolbox';
    contents: ToolboxItemInfo[];
}

/**
 * An array holding flyout items.
 */
export type FlyoutItemInfoArray = FlyoutItemInfo[];

/**
 * All of the different types that can create a toolbox.
 */
export type ToolboxDefinition = Node | ToolboxInfo | string;

/**
 * All of the different types that can be used to show items in a flyout.
 */
export type FlyoutDefinition = FlyoutItemInfoArray | NodeList | ToolboxInfo | Node[];

/**
 * Position of the toolbox and/or flyout relative to the workspace.
 */
export declare enum Position {
    TOP = 0,
    BOTTOM = 1,
    LEFT = 2,
    RIGHT = 3,
}
