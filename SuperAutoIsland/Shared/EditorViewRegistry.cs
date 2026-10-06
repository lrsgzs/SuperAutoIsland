using ClassIsland.Core.Abstractions.Controls;

namespace SuperAutoIsland.Shared;

/// <summary>
///     应用内编辑器窗口的登记表。
///     <para>
///         同一个目标（一般是项目 Guid）只保留一个已打开的编辑器视图：再次打开时复用并置前，
///         还没有打开过则新建视图，也就是新开一个非模态窗口。视图关闭后会自动从登记表里移除。
///     </para>
/// </summary>
/// <remarks>只在 UI 线程上使用。</remarks>
public static class EditorViewRegistry
{
    private static readonly Dictionary<string, ViewBase> OpenedViews = [];

    /// <summary>
    ///     打开（或置前）目标对应的编辑器视图。
    /// </summary>
    /// <param name="key">目标标识，一般是项目 Guid</param>
    /// <param name="createView">新建视图的工厂方法</param>
    /// <param name="initializeView">新建视图后载入目标内容的操作（复用已有窗口时不会执行）</param>
    /// <typeparam name="TView">编辑器视图类型</typeparam>
    public static void OpenOrActivate<TView>(string key, Func<TView> createView, Action<TView>? initializeView = null)
        where TView : ViewBase
    {
        // AssociatedViewHost 为空说明视图已经关闭（正常情况下关闭时已被移除，这里兜底）
        if (OpenedViews.TryGetValue(key, out var opened) && opened is TView existing && existing.AssociatedViewHost is not null)
        {
            // 已经打开：把窗口置前；仍在打开（Show 还没完成）时什么都不做，避免重复入栈
            if (existing.ShowedOnce)
            {
                existing.Open();
            }

            return;
        }

        var view = createView();
        OpenedViews[key] = view;
        view.Closed += (_, _) => Remove(key, view);

        initializeView?.Invoke(view);
        view.Open();
    }

    private static void Remove(string key, ViewBase view)
    {
        if (OpenedViews.TryGetValue(key, out var opened) && ReferenceEquals(opened, view))
        {
            OpenedViews.Remove(key);
        }
    }
}
