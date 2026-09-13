using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Interactivity;
using Avalonia.Media;
using ClassIsland.Core.Abstractions.Controls;
using ClassIsland.Core.Attributes;
using ClassIsland.Core.Icons;
using ClassIsland.Shared;
using SuperAutoIsland.Models;
using SuperAutoIsland.Models.Components;
using SuperAutoIsland.Services;

namespace SuperAutoIsland.Controls.Components;

[ComponentInfo(
    "3176D88A-18DC-4273-8ECE-AF84B2A2F9DB",
    "动态文本",
    FluentIcons.SlideTextRegular,
    "实时显示来自 SAI 的文本信息。"
)]
[PseudoClasses(":custom-text-color")]
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
        UpdateText();
        
        _provider.Changed += (o, args) =>
        {
            if (args.Key != Settings.Id) return;
            ApplyItem(args.Value);
        };
        
        Settings.PropertyChanged += (o, args) =>
        {
            if (args.PropertyName != nameof(Settings.Id)) return;
            UpdateText();
        };
    }

    private void UpdateText()
    {
        ApplyItem(_provider.GetText(Settings.Id));
    }

    private void ApplyItem(DynamicTextItem? item)
    {
        if (item == null)
        {
            Settings.LastText = "[未设置值]";
            SetCustomColor(false, default);
            return;
        }

        Settings.LastText = item.Text;
        SetCustomColor(item.HasCustomColor, item.Color);
    }

    private void SetCustomColor(bool hasCustomColor, Color color)
    {
        Settings.LastColor = color;
        PseudoClasses.Set(":custom-text-color", hasCustomColor);
    }
}
