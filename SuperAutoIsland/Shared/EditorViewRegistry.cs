using ClassIsland.Core.Abstractions.Controls;

namespace SuperAutoIsland.Shared;

/// <summary>
///     应用内编辑器窗口的登记表。
///     <para>
///         同一个目标的同一种视图只保留一个已打开的编辑器窗口：再次打开时复用并置前，
///         还没有打开过则新建视图，也就是新开一个非模态窗口。视图关闭后会自动从登记表里移除。
///     </para>
///     <para>
///         键里带上视图类型：同一个项目可以同时开着多种编辑器（例如 Blockly 编辑器与只读的代码查看器），
///         它们各算一个目标，不会互相顶掉登记项。
///     </para>
/// </summary>
/// <remarks>只在 UI 线程上使用。</remarks>
public static class EditorViewRegistry
{
    private static readonly Dictionary<(string Key, Type ViewType), ViewBase> OpenedViews = [];

    /// <summary>
    ///     打开（或置前）目标对应的编辑器视图。
    /// </summary>
    /// <param name="key">目标标识，一般是项目 Guid</param>
    /// <param name="createView">新建视图的工厂方法</param>
    /// <param name="initializeView">新建视图后载入目标内容的操作（复用已有窗口时不会执行）</param>
    /// <param name="activateView">
    ///     复用已有窗口并置前时的刷新操作。用于内容可能已被别的窗口改写的只读视图：
    ///     不传时保持窗口原样（有未保存改动的编辑器必须如此，否则会冲掉用户的编辑）。
    /// </param>
    /// <typeparam name="TView">编辑器视图类型</typeparam>
    public static void OpenOrActivate<TView>(string key, Func<TView> createView, Action<TView>? initializeView = null,
                                             Action<TView>? activateView = null)
        where TView : ViewBase
    {
        var viewKey = (key, typeof(TView));

        // AssociatedViewHost 为空说明视图已经关闭（正常情况下关闭时已被移除，这里兜底）
        if (OpenedViews.TryGetValue(viewKey, out var opened) && opened is TView existing && existing.AssociatedViewHost is not null)
        {
            // 已经打开：先把内容刷新到最新，再把窗口置前；仍在打开（Show 还没完成）时什么都不做，避免重复入栈
            activateView?.Invoke(existing);

            if (existing.ShowedOnce)
            {
                existing.Open();
            }

            return;
        }

        var view = createView();
        OpenedViews[viewKey] = view;
        view.Closed += (_, _) => Remove(viewKey, view);

        initializeView?.Invoke(view);
        view.Open();
    }

    private static void Remove((string Key, Type ViewType) viewKey, ViewBase view)
    {
        if (OpenedViews.TryGetValue(viewKey, out var opened) && ReferenceEquals(opened, view))
        {
            OpenedViews.Remove(viewKey);
        }
    }
}
