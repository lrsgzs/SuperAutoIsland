export interface PickLocalFilesOptions {
    /** 文件类型，如 image、json、text，其它值表示全部文件。 */
    kind?: string;
    allowMultiple?: boolean;
    title?: string;
}

export interface PickLocalFilesResult {
    paths: string[];
    message: string;
}

/**
 * 通过 SaiServer 打开本地文件选择器并获取可用的本地路径。
 *
 * 浏览器无法直接取得文件的真实路径，因此统一走 server 桥接。
 */
export async function pickLocalFiles(options: PickLocalFilesOptions = {}): Promise<PickLocalFilesResult> {
    const result = await window.saiWaitMessage<{ paths?: string[]; message?: string }>(window.saiWS, {
        type: 'pickFile',
        ...options,
    });
    return {
        paths: result.paths ?? [],
        message: result.message ?? '',
    };
}
