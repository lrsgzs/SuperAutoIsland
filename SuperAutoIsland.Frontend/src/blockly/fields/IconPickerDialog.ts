import {
    FLUENT_FONT_FAMILY,
    LUCIDE_FONT_FAMILY,
    formatIconExpression,
    parseIconExpression,
    type IconExpressionType,
} from '../utils/iconExpression';
import { searchIcons, type IconCatalogEntry, type IconCatalogType } from '../utils/iconCatalog';
import { pickLocalFiles } from '../utils/filePicker';

const FALLBACK_FLUENT_GLYPH = '\ue9b0';
const FALLBACK_LUCIDE_GLYPH = '\ue0ff';
const RENDER_BATCH = 210;

let pickerOpen = false;

export interface IconPickerOptions {
    /** 打开编辑器时默认选中的标签页。 */
    initialType?: IconExpressionType;
    /** 当前图标表达式，用于回显。 */
    expression?: string | null;
    /** 选择变化时回调，返回最新的图标表达式（空字符串表示未选择）。 */
    onChange?: (expression: string) => void;
}

type FontIconType = 'fluent' | 'lucide';

interface PickerState {
    type: IconExpressionType;
    glyph: string;
    imagePath: string;
}

function fallbackGlyph(type: FontIconType): string {
    return type === 'lucide' ? FALLBACK_LUCIDE_GLYPH : FALLBACK_FLUENT_GLYPH;
}

/**
 * 打开与 ClassIsland IconExpressionEditor 类似的图标选择器。
 */
export function openIconPicker(options: IconPickerOptions = {}): Promise<void> {
    if (pickerOpen) return Promise.resolve();
    pickerOpen = true;
    return new Promise(resolve => {
        const parsed = parseIconExpression(options.expression);
        const selectedByType: Partial<Record<FontIconType, string>> = {};
        if (parsed && parsed.type !== 'img') selectedByType[parsed.type] = parsed.argument;
        const state: PickerState = {
            type: parsed?.type ?? options.initialType ?? 'fluent',
            glyph: '',
            imagePath: parsed?.type === 'img' ? parsed.argument : '',
        };
        state.glyph = state.type === 'img' ? '' : (selectedByType[state.type] ?? fallbackGlyph(state.type));

        const overlay = document.createElement('div');
        overlay.className = 'sai-icon-picker-overlay';

        const panel = document.createElement('div');
        panel.className = 'sai-icon-picker';
        overlay.appendChild(panel);

        const tooltip = document.createElement('div');
        tooltip.className = 'sai-ip-tooltip transition';
        overlay.appendChild(tooltip);

        let tooltipTimer: number | undefined;

        const hideTooltip = () => {
            if (tooltipTimer !== undefined) {
                clearTimeout(tooltipTimer);
                tooltipTimer = undefined;
            }
            tooltip.classList.remove('visible');
        };

        const showTooltip = (target: HTMLElement, text: string) => {
            if (tooltipTimer !== undefined) clearTimeout(tooltipTimer);
            tooltipTimer = window.setTimeout(() => {
                tooltipTimer = undefined;
                tooltip.textContent = text;
                tooltip.classList.add('visible');
                const rect = target.getBoundingClientRect();
                const size = tooltip.getBoundingClientRect();
                let top = rect.top - size.height - 6;
                if (top < 8) top = rect.bottom + 6;
                let left = rect.left + rect.width / 2 - size.width / 2;
                left = Math.max(8, Math.min(left, window.innerWidth - size.width - 8));
                tooltip.style.left = `${left}px`;
                tooltip.style.top = `${top}px`;
            }, 200);
        };

        const header = document.createElement('div');
        header.className = 'sai-ip-header';
        const preview = document.createElement('div');
        preview.className = 'sai-ip-preview';
        const tabs = document.createElement('div');
        tabs.className = 'sai-ip-tabs';
        header.append(preview, tabs);
        panel.appendChild(header);

        const tabButtons: Record<IconExpressionType, HTMLButtonElement> = {
            fluent: document.createElement('button'),
            lucide: document.createElement('button'),
            img: document.createElement('button'),
        };
        const tabLabels: Record<IconExpressionType, string> = { fluent: 'Fluent', lucide: 'Lucide', img: '图像' };
        (Object.keys(tabButtons) as IconExpressionType[]).forEach(type => {
            const button = tabButtons[type];
            button.type = 'button';
            button.className = 'sai-ip-tab transition';
            const label = document.createElement('span');
            label.className = 'sai-ip-tab-label';
            label.textContent = tabLabels[type];
            const pill = document.createElement('span');
            pill.className = 'sai-ip-pill';
            button.append(label, pill);
            button.addEventListener('click', () => switchType(type));
            tabs.appendChild(button);
        });

        const search = document.createElement('div');
        search.className = 'sai-ip-search';
        const searchIcon = document.createElement('span');
        searchIcon.className = 'sai-ip-search-icon';
        searchIcon.textContent = '\uef33';
        const searchInput = document.createElement('input');
        searchInput.type = 'text';
        searchInput.className = 'transition';
        searchInput.placeholder = '名称或 Unicode 码';
        searchInput.autocomplete = 'off';
        search.append(searchIcon, searchInput);
        panel.appendChild(search);

        const gridPanel = document.createElement('div');
        gridPanel.className = 'sai-ip-grid-panel';
        const grid = document.createElement('div');
        grid.className = 'sai-ip-grid';
        const empty = document.createElement('div');
        empty.className = 'sai-ip-empty';
        const emptyTitle = document.createElement('div');
        emptyTitle.textContent = '没有找到匹配的图标';
        const emptyHint = document.createElement('div');
        emptyHint.textContent = '试试其他名称或 Unicode 码';
        empty.append(emptyTitle, emptyHint);
        gridPanel.append(grid, empty);
        panel.appendChild(gridPanel);

        const imageRow = document.createElement('div');
        imageRow.className = 'sai-ip-image';
        const imageInput = document.createElement('input');
        imageInput.type = 'text';
        imageInput.className = 'transition';
        imageInput.placeholder = '图片路径或 URI';
        imageInput.autocomplete = 'off';
        const browseButton = document.createElement('button');
        browseButton.type = 'button';
        browseButton.className = 'sai-ip-browse transition';
        browseButton.textContent = '\ue88d';
        browseButton.title = '浏览图片…';
        imageRow.append(imageInput, browseButton);
        panel.appendChild(imageRow);

        const footer = document.createElement('div');
        footer.className = 'sai-ip-footer';
        const message = document.createElement('span');
        message.className = 'sai-ip-message';
        const doneButton = document.createElement('button');
        doneButton.type = 'button';
        doneButton.className = 'sai-ip-button primary transition';
        doneButton.textContent = '完成';
        footer.append(message, doneButton);
        panel.appendChild(footer);

        let currentResults: IconCatalogEntry[] = [];
        let renderedCount = 0;
        let searchToken = 0;
        let closed = false;

        const emit = () => {
            if (!options.onChange) return;
            if (state.type === 'img') {
                options.onChange(state.imagePath ? formatIconExpression('img', state.imagePath) : '');
            } else {
                options.onChange(formatIconExpression(state.type, state.glyph));
            }
        };

        const updatePreview = () => {
            preview.textContent = '';
            if (state.type === 'img') {
                preview.style.fontFamily = FLUENT_FONT_FAMILY;
                preview.style.fontSize = '32px';
                preview.textContent = '\ue9b2';
                return;
            }
            preview.style.fontFamily = state.type === 'lucide' ? LUCIDE_FONT_FAMILY : FLUENT_FONT_FAMILY;
            preview.style.fontSize = '32px';
            preview.textContent = state.glyph;
        };

        const updateTabs = () => {
            (Object.keys(tabButtons) as IconExpressionType[]).forEach(type => {
                tabButtons[type].classList.toggle('active', type === state.type);
            });
        };

        const renderMore = () => {
            const end = Math.min(currentResults.length, renderedCount + RENDER_BATCH);
            for (let i = renderedCount; i < end; i++) {
                const entry = currentResults[i];
                const button = document.createElement('button');
                button.type = 'button';
                button.className = 'sai-ip-icon transition';
                button.style.fontFamily = state.type === 'lucide' ? LUCIDE_FONT_FAMILY : FLUENT_FONT_FAMILY;
                button.textContent = entry.glyph;
                const description = `${entry.name} · U+${entry.codePoint.toString(16).toUpperCase().padStart(4, '0')}`;
                button.dataset.glyph = entry.glyph;
                button.classList.toggle('selected', entry.glyph === state.glyph);
                button.addEventListener('click', () => selectGlyph(entry.glyph));
                button.addEventListener('mouseenter', () => showTooltip(button, description));
                button.addEventListener('mouseleave', hideTooltip);
                button.addEventListener('focus', () => showTooltip(button, description));
                button.addEventListener('blur', hideTooltip);
                grid.appendChild(button);
            }
            renderedCount = end;
            empty.style.display = currentResults.length === 0 ? 'flex' : 'none';
        };

        const renderResults = (results: IconCatalogEntry[]) => {
            hideTooltip();
            currentResults = results;
            renderedCount = 0;
            grid.textContent = '';
            grid.scrollTop = 0;
            renderMore();
        };

        const refreshGrid = async () => {
            const token = ++searchToken;
            const type: IconCatalogType = state.type === 'lucide' ? 'lucide' : 'fluent';
            const results = await searchIcons(type, searchInput.value);
            if (closed || token !== searchToken) return;
            renderResults(results);
        };

        const selectGlyph = (glyph: string) => {
            if (state.type !== 'img') selectedByType[state.type] = glyph;
            state.glyph = glyph;
            grid.querySelectorAll<HTMLButtonElement>('.sai-ip-icon.selected').forEach(el =>
                el.classList.remove('selected'),
            );
            const target = [...grid.querySelectorAll<HTMLButtonElement>('.sai-ip-icon')].find(
                el => el.dataset.glyph === glyph,
            );
            target?.classList.add('selected');
            updatePreview();
            emit();
        };

        const applyTypeVisibility = () => {
            const isImage = state.type === 'img';
            search.style.display = isImage ? 'none' : '';
            gridPanel.style.display = isImage ? 'none' : '';
            imageRow.style.display = isImage ? '' : 'none';
            searchInput.value = '';
        };

        const switchType = (type: IconExpressionType) => {
            if (state.type === type) return;
            state.type = type;
            if (type !== 'img') {
                state.glyph = selectedByType[type] ?? fallbackGlyph(type);
            }
            updateTabs();
            applyTypeVisibility();
            updatePreview();
            if (type === 'img') {
                imageInput.value = state.imagePath;
            } else {
                void refreshGrid();
            }
        };

        const close = () => {
            if (closed) return;
            closed = true;
            pickerOpen = false;
            document.removeEventListener('keydown', onKeyDown, true);
            overlay.remove();
            resolve();
        };

        const onKeyDown = (event: KeyboardEvent) => {
            if (event.key === 'Escape') {
                event.preventDefault();
                close();
                return;
            }
            if (state.type === 'img') return;
            const fromSearch = event.target === searchInput;
            const inGrid = grid.contains(event.target as Node);
            if (!fromSearch && !inGrid) return;

            if (fromSearch) {
                if (event.key === 'ArrowDown' && currentResults.length > 0) {
                    event.preventDefault();
                    focusIndex(0);
                }
                return;
            }

            const buttons = [...grid.querySelectorAll<HTMLButtonElement>('.sai-ip-icon')];
            let currentIndex = buttons.findIndex(el => el === document.activeElement);
            if (currentIndex < 0) currentIndex = buttons.findIndex(el => el.classList.contains('selected'));
            const columns = Math.max(1, Math.floor((grid.clientWidth - 8) / 36));
            switch (event.key) {
                case 'ArrowLeft':
                    focusIndex(currentIndex - 1);
                    event.preventDefault();
                    break;
                case 'ArrowRight':
                    focusIndex(currentIndex + 1);
                    event.preventDefault();
                    break;
                case 'ArrowUp':
                    focusIndex(currentIndex - columns);
                    event.preventDefault();
                    break;
                case 'ArrowDown':
                    focusIndex(currentIndex + columns);
                    event.preventDefault();
                    break;
                case 'Home':
                    focusIndex(0);
                    event.preventDefault();
                    break;
                case 'End':
                    focusIndex(currentResults.length - 1);
                    event.preventDefault();
                    break;
            }
        };

        const focusIndex = (target: number) => {
            if (currentResults.length === 0) return;
            const index = Math.min(Math.max(target, 0), currentResults.length - 1);
            while (index >= renderedCount && renderedCount < currentResults.length) {
                renderMore();
            }
            const element = grid.querySelectorAll<HTMLButtonElement>('.sai-ip-icon')[index];
            element?.focus({ preventScroll: true });
            element?.scrollIntoView({ block: 'nearest' });
        };

        searchInput.addEventListener('input', () => void refreshGrid());
        imageInput.addEventListener('input', () => {
            state.imagePath = imageInput.value.trim();
            updatePreview();
            emit();
        });
        browseButton.addEventListener('click', async () => {
            if (browseButton.disabled) return;
            browseButton.disabled = true;
            message.textContent = '';
            try {
                const result = await pickLocalFiles({ kind: 'image', title: '选择图片' });
                if (result.paths.length > 0) {
                    state.imagePath = result.paths[0];
                    imageInput.value = state.imagePath;
                    updatePreview();
                    emit();
                }
                if (result.message) {
                    message.textContent = result.message;
                }
            } catch {
                message.textContent = '无法打开文件选择器，请直接输入图片路径。';
            } finally {
                browseButton.disabled = false;
            }
        });
        grid.addEventListener('scroll', () => {
            hideTooltip();
            if (renderedCount < currentResults.length && grid.scrollTop + grid.clientHeight >= grid.scrollHeight - 40) {
                renderMore();
            }
        });
        doneButton.addEventListener('click', close);
        overlay.addEventListener('mousedown', event => {
            if (event.target === overlay) close();
        });
        document.addEventListener('keydown', onKeyDown, true);

        updateTabs();
        applyTypeVisibility();
        updatePreview();
        if (state.type === 'img') {
            imageInput.value = state.imagePath;
        } else {
            void refreshGrid();
        }
        document.body.appendChild(overlay);
        (state.type === 'img' ? imageInput : searchInput).focus();
    });
}
