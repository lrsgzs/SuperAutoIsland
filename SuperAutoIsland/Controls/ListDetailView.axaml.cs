/*
 * Copied from https://github.com/ClassIsland/ClassIsland/tree/dev/ClassIsland/Controls/ListDetailView.axaml
 * Copyright By HelloWRC and ClassIsland's Contributors, Thanks to them!
 * Licensed Under GNU General Public License v3.0.
 *
 * 从 https://github.com/ClassIsland/ClassIsland/tree/dev/ClassIsland/Controls/ListDetailView.axaml 复制的代码。
 * 版权由 HelloWRC 和 ClassIsland 的贡献者所有，在此感谢！
 * 本代码以 GNU General Public License v3.0 协议发行。
 */

using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace SuperAutoIsland.Controls;

/// <summary>
///     ListDetailView.axaml 的交互逻辑
/// </summary>
public partial class ListDetailView : UserControl
{
    public static readonly StyledProperty<object> LeftContentProperty =
        AvaloniaProperty.Register<ListDetailView, object>(
            nameof(LeftContent));

    public static readonly StyledProperty<object> RightContentProperty =
        AvaloniaProperty.Register<ListDetailView, object>(
            nameof(RightContent));

    public static readonly StyledProperty<object> TitleElementProperty =
        AvaloniaProperty.Register<ListDetailView, object>(
            nameof(TitleElement));

    public static readonly StyledProperty<bool> IsPanelOpenedProperty = AvaloniaProperty.Register<ListDetailView, bool>(
        nameof(IsPanelOpened));

    public static readonly StyledProperty<double> MinCompressWidthProperty =
        AvaloniaProperty.Register<ListDetailView, double>(
            nameof(MinCompressWidth), 800.0);

    public static readonly StyledProperty<bool> IsCompressedModeProperty =
        AvaloniaProperty.Register<ListDetailView, bool>(
            nameof(IsCompressedMode));

    public static readonly StyledProperty<bool> ShowTitleWhenNotCompressedProperty =
        AvaloniaProperty.Register<ListDetailView, bool>(
            nameof(ShowTitleWhenNotCompressed));

    public ListDetailView()
    {
        InitializeComponent();
    }

    public object LeftContent
    {
        get => GetValue(LeftContentProperty);
        set => SetValue(LeftContentProperty, value);
    }

    public object RightContent
    {
        get => GetValue(RightContentProperty);
        set => SetValue(RightContentProperty, value);
    }

    public object TitleElement
    {
        get => GetValue(TitleElementProperty);
        set => SetValue(TitleElementProperty, value);
    }

    public bool IsPanelOpened
    {
        get => GetValue(IsPanelOpenedProperty);
        set => SetValue(IsPanelOpenedProperty, value);
    }

    public double MinCompressWidth
    {
        get => GetValue(MinCompressWidthProperty);
        set => SetValue(MinCompressWidthProperty, value);
    }

    public bool IsCompressedMode
    {
        get => GetValue(IsCompressedModeProperty);
        set => SetValue(IsCompressedModeProperty, value);
    }

    public bool ShowTitleWhenNotCompressed
    {
        get => GetValue(ShowTitleWhenNotCompressedProperty);
        set => SetValue(ShowTitleWhenNotCompressedProperty, value);
    }

    private void GridRoot_OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        IsCompressedMode = e.NewSize.Width <= MinCompressWidth;
    }

    private void ButtonBack_OnClick(object sender, RoutedEventArgs e)
    {
        IsPanelOpened = false;
    }
}