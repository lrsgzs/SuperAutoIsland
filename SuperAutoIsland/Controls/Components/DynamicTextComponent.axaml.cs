using Avalonia.Interactivity;
using ClassIsland.Core.Abstractions.Controls;
using ClassIsland.Core.Attributes;
using ClassIsland.Core.Icons;
using ClassIsland.Shared;
using SuperAutoIsland.Models.Components;
using SuperAutoIsland.Services;

namespace SuperAutoIsland.Controls.Components;

[ComponentInfo(
    "3176D88A-18DC-4273-8ECE-AF84B2A2F9DB",
    "动态文本",
    FluentIcons.SlideTextRegular,
    "实时显示来自 SAI 的文本信息。"
)]
public partial class DynamicTextComponent : ComponentBase<DynamicTextSettings>
{
    private DynamicTextProvider _provider = IAppHost.GetService<DynamicTextProvider>();
    
    public DynamicTextComponent()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        Settings.LastText = _provider.GetText(Settings.Id) ?? "[未设置值]";
        
        _provider.Changed += (o, args) =>
        {
            if (args.Key != Settings.Id) return;
            Settings.LastText = args.Value;
        };
        
        Settings.PropertyChanged += (o, args) =>
        {
            if (args.PropertyName != nameof(Settings.Id)) return;
            Settings.LastText = _provider.GetText(Settings.Id) ?? "[未设置值]";
        };
    }
}