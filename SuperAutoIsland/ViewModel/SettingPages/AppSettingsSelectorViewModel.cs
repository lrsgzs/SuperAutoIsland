using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SuperAutoIsland.Models.Settings;
using SuperAutoIsland.Services.Automations.AppSettings;

namespace SuperAutoIsland.ViewModel.SettingPages;

/// <summary>
///     「选择要展示的设置项」的视图模型。
/// </summary>
public partial class AppSettingsSelectorViewModel : ObservableObject
{
    private static readonly (
        AppSettingGroup Group,
        string Title,
        string Note,
        bool IsWarning,
        bool IsMuted)[] GroupOrder =
        [
            (AppSettingGroup.Attributed, "有设置项信息",
             "这些设置项在 ClassIsland 中有明确的中文名与图标，默认展示。", false, false),
            (AppSettingGroup.Other, "其他设置项",
             "这些设置项没有设置项信息，默认不展示。", false, false),
            (AppSettingGroup.Json, "以 JSON 文本传递",
             "以下设置项以 JSON 文本形式读写，请自行保证格式正确。", true, false),
            (AppSettingGroup.GetOnly, "仅可获取",
             "以下设置项不可修改，勾选后只会生成获取积木。", false, true),
            (AppSettingGroup.Unsupported, "不支持",
             "以下设置项的类型（集合、字典等）无法用单个积木表达，不会生成积木。", false, true)
        ];

    private readonly IReadOnlyList<AppSettingDescriptor> _descriptors;

    private readonly AppSettingsBlocksModel _model;
    private readonly Dictionary<string, string> _previews;
    private readonly HashSet<string> _selectableNames;

    /// <summary>
    ///     已选 / 可选数量文本。
    /// </summary>
    [ObservableProperty]
    private string _countText = string.Empty;

    private bool _isRebuilding;

    /// <summary>
    ///     搜索关键字。
    /// </summary>
    [ObservableProperty]
    private string _searchText = string.Empty;

    private HashSet<string> _selected;

    public AppSettingsSelectorViewModel(AppSettingsBlocksModel model)
    {
        _model = model;

        // 与 Blockly「应用设置」分类使用同一套排序
        _descriptors = AppSettingGrouping.SortForDisplay(ClassIslandSettingsAccessor.Descriptors);

        // 预览值只读取一次，避免搜索时反复反射
        _previews = _descriptors.ToDictionary(
            d => d.PropertyName,
            d => d.IsSupported ? ClassIslandSettingsAccessor.GetPreview(d) : string.Empty,
            StringComparer.Ordinal);

        _selectableNames = _descriptors.Where(d => d.CanSelect)
                                       .Select(d => d.PropertyName)
                                       .ToHashSet(StringComparer.Ordinal);

        _selected = AppSettingsSelection.Resolve(model);

        Rebuild();
    }

    /// <summary>
    ///     抽屉中展示的行（含分组标题行）。
    /// </summary>
    public ObservableCollection<AppSettingRowViewModel> Rows { get; } = [];

    partial void OnSearchTextChanged(string value)
    {
        Rebuild();
    }

    /// <summary>
    ///     恢复默认选择（全部有设置项信息的设置项）。
    /// </summary>
    [RelayCommand]
    private void RestoreDefaults()
    {
        _selected = [.. ClassIslandSettingsAccessor.DefaultSelection];

        // null 表示「未自定义」，跟随 ClassIsland 的设置项信息
        _model.Selected = null;

        Rebuild();
    }

    /// <summary>
    ///     全选（不含不支持的类型）。
    /// </summary>
    [RelayCommand]
    private void SelectAll()
    {
        _isRebuilding = true;

        foreach (var row in Rows.Where(r => r.IsItem && r.IsSelectable))
        {
            row.IsSelected = true;

            if (row.Descriptor != null)
            {
                _selected.Add(row.Descriptor.PropertyName);
            }
        }

        _isRebuilding = false;
        Commit();
    }

    /// <summary>
    ///     全不选。
    /// </summary>
    [RelayCommand]
    private void SelectNone()
    {
        _isRebuilding = true;

        foreach (var row in Rows.Where(r => r.IsItem))
        {
            row.IsSelected = false;
        }

        _isRebuilding = false;
        _selected.Clear();
        Commit();
    }

    private void OnRowSelectionChanged(AppSettingRowViewModel row)
    {
        if (_isRebuilding || row.Descriptor == null)
        {
            return;
        }

        if (row.IsSelected)
        {
            _selected.Add(row.Descriptor.PropertyName);
        }
        else
        {
            _selected.Remove(row.Descriptor.PropertyName);
        }

        Commit();
    }

    private void Commit()
    {
        _model.Selected = [.. _selected];
        UpdateCounts();
    }

    private void Rebuild()
    {
        _isRebuilding = true;
        Rows.Clear();

        var keyword = SearchText.Trim();
        var grouped = new Dictionary<AppSettingGroup, List<AppSettingRowViewModel>>();

        foreach (var descriptor in _descriptors)
        {
            var group = AppSettingGrouping.Classify(descriptor);
            var row = new AppSettingRowViewModel(
                descriptor, group, _selected.Contains(descriptor.PropertyName),
                _previews.GetValueOrDefault(descriptor.PropertyName, string.Empty),
                OnRowSelectionChanged);

            if (!Matches(row, keyword))
            {
                continue;
            }

            if (!grouped.TryGetValue(group, out var items))
            {
                items = [];
                grouped[group] = items;
            }

            items.Add(row);
        }

        foreach (var (group, title, note, isWarning, isMuted) in GroupOrder)
        {
            if (!grouped.TryGetValue(group, out var items) || items.Count == 0)
            {
                continue;
            }

            Rows.Add(AppSettingRowViewModel.CreateHeader($"{title}（{items.Count}）", note, isWarning, isMuted));

            foreach (var item in items)
            {
                Rows.Add(item);
            }
        }

        if (Rows.Count == 0)
        {
            Rows.Add(AppSettingRowViewModel.CreateHeader("找不到匹配的设置项。", string.Empty, false, true));
        }

        _isRebuilding = false;
        UpdateCounts();
    }

    private void UpdateCounts()
    {
        // 只统计当前 ClassIsland 版本中确实存在且可勾选的设置项
        var selected = _selected.Count(_selectableNames.Contains);
        CountText = $"已选 {selected} / 共 {_selectableNames.Count} 项";
    }

    private static bool Matches(AppSettingRowViewModel row, string keyword)
    {
        if (string.IsNullOrEmpty(keyword))
        {
            return true;
        }

        var descriptor = row.Descriptor!;

        return descriptor.DisplayName.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
               descriptor.PropertyName.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
               descriptor.ValueType.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
               row.PreviewValue.Contains(keyword, StringComparison.OrdinalIgnoreCase);
    }

}

/// <summary>
///     「选择要展示的设置项」中的一行（分组标题或设置项）。
/// </summary>
public class AppSettingRowViewModel : ObservableObject
{
    private readonly Action<AppSettingRowViewModel>? _onSelectionChanged;
    private bool _isSelected;

    public AppSettingRowViewModel(AppSettingDescriptor descriptor, AppSettingGroup group, bool isSelected,
                                  string previewValue, Action<AppSettingRowViewModel> onSelectionChanged)
    {
        Descriptor = descriptor;
        _onSelectionChanged = onSelectionChanged;
        _isSelected = isSelected;
        IsSelectable = descriptor.CanSelect;
        Glyph = descriptor.Glyph;
        DisplayName = descriptor.DisplayName;
        PreviewValue = previewValue;
        Subtitle = BuildSubtitle(descriptor, group);
    }

    private AppSettingRowViewModel(string headerText, string headerNote, bool isWarning, bool isMuted)
    {
        IsHeader = true;
        HeaderText = headerText;
        HeaderNote = headerNote;
        IsWarning = isWarning;
        IsMuted = isMuted;
    }

    /// <summary>
    ///     对应的设置项。分组标题行时为 null。
    /// </summary>
    public AppSettingDescriptor? Descriptor { get; }

    public bool IsHeader { get; }

    public bool IsItem => !IsHeader;

    public string HeaderText { get; } = string.Empty;

    public string HeaderNote { get; } = string.Empty;

    public bool IsWarning { get; }

    public bool IsMuted { get; }

    public string Glyph { get; } = string.Empty;

    public string DisplayName { get; } = string.Empty;

    public string Subtitle { get; } = string.Empty;

    public string PreviewValue { get; } = string.Empty;

    public bool IsObsolete => Descriptor?.IsObsolete == true;

    public bool IsSelectable { get; }

    /// <summary>
    ///     是否可以复制当前值。不支持的类型不会生成积木，复制也没有意义。
    /// </summary>
    public bool CanCopy => Descriptor?.IsSupported == true;

    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (value == _isSelected)
            {
                return;
            }

            _isSelected = value;
            OnPropertyChanged();
            _onSelectionChanged?.Invoke(this);
        }
    }

    public static AppSettingRowViewModel CreateHeader(string text, string note, bool isWarning, bool isMuted)
    {
        return new AppSettingRowViewModel(text, note, isWarning, isMuted);
    }

    private static string BuildSubtitle(AppSettingDescriptor descriptor, AppSettingGroup group)
    {
        return group switch
        {
            AppSettingGroup.Unsupported => $"{descriptor.ValueType.Name} 无法用单个积木表达",
            AppSettingGroup.GetOnly     => $"{descriptor.ValueType.Name} · 仅可获取",
            AppSettingGroup.Json        => $"{descriptor.ValueType.Name} · 以 JSON 文本读写",
            _                                   => descriptor.ValueType.Name
        };
    }
}