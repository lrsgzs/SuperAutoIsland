using System.Collections.ObjectModel;
using System.Collections.Specialized;
using ClassIsland.Core.Icons;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SuperAutoIsland.Interface.Metadata;
using SuperAutoIsland.Models.Settings;
using SuperAutoIsland.Services;

namespace SuperAutoIsland.ViewModel.SettingPages;

/// <summary>
///     「积木分类管理」的视图模型。
///     <para>
///         只调整 Blockly 工具箱里的分类顺序与是否展示：结果写入 <see cref="BlocklyCategoriesModel" />，
///         由 <see cref="SaiBlocksRegistry" /> 重建时应用到发给编辑器的分类列表。
///     </para>
/// </summary>
public partial class BlocklyCategoryManagerViewModel : ObservableObject
{
    private readonly BlocklyCategoriesModel _model;
    private bool _isRebuilding;

    public BlocklyCategoryManagerViewModel(BlocklyCategoriesModel model)
    {
        _model = model;
        Rows.CollectionChanged += OnRowsChanged;
        Rebuild();
    }

    /// <summary>
    ///     列表中的分类行。
    /// </summary>
    public ObservableCollection<BlocklyCategoryRowViewModel> Rows { get; } = [];

    /// <summary>
    ///     分类数量与隐藏数量文本。
    /// </summary>
    [ObservableProperty]
    private string _countText = string.Empty;

    /// <summary>
    ///     恢复默认：按分类注册顺序展示全部分类。
    /// </summary>
    [RelayCommand]
    private void RestoreDefaults()
    {
        _model.Order = null;
        _model.Hidden = null;
        Rebuild();
    }

    private void Rebuild()
    {
        _isRebuilding = true;
        Rows.Clear();

        // 分类提供方按注册顺序排列，新注册的分类据此追加到列表末尾
        var metadata = new Dictionary<string, CategoryMetadata>(StringComparer.Ordinal);
        var registered = new List<string>();

        foreach (var provider in SaiBlocksRegistry.CategoryProviders)
        {
            if (metadata.TryAdd(provider.Metadata.Name, provider.Metadata))
            {
                registered.Add(provider.Metadata.Name);
            }
        }

        var hidden = BlocklyCategorySelection.HiddenNames();

        foreach (var name in BlocklyCategorySelection.BuildRows(registered))
        {
            metadata.TryGetValue(name, out var category);
            Rows.Add(new BlocklyCategoryRowViewModel(
                         name,
                         category,
                         !hidden.Contains(name),
                         OnRowVisibilityChanged,
                         DeleteCategory));
        }

        _isRebuilding = false;
        UpdateCount();
    }

    /// <summary>
    ///     勾选状态变化：把当前不展示的分类写回配置，设置页据此重建后端分类并保存。
    /// </summary>
    private void OnRowVisibilityChanged(BlocklyCategoryRowViewModel row)
    {
        var hidden = BlocklyCategorySelection.HiddenNames();

        if (row.IsVisible)
        {
            hidden.Remove(row.Name);
        }
        else
        {
            hidden.Add(row.Name);
        }

        _model.Hidden = [.. hidden];
        UpdateCount();
    }

    /// <summary>
    ///     拖拽排序后把列表顺序写回配置。
    ///     <para>
    ///         拖动是「移除 + 插入」两步，只在插入后写回，避免中间状态被保存；
    ///         已不存在的分类也一并写入，插件恢复后仍落在原来的位置。
    ///     </para>
    /// </summary>
    private void OnRowsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (_isRebuilding || e.Action is NotifyCollectionChangedAction.Remove)
        {
            return;
        }

        _model.Order = [.. Rows.Select(row => row.Name)];
    }

    /// <summary>
    ///     删除列表中已不存在的分类：顺序和隐藏列表里都不再保留它。
    /// </summary>
    private void DeleteCategory(BlocklyCategoryRowViewModel row)
    {
        _isRebuilding = true;
        Rows.Remove(row);
        _isRebuilding = false;

        if (_model.Order is not null)
        {
            _model.Order = [.. _model.Order.Where(name => name != row.Name)];
        }

        if (_model.Hidden is not null)
        {
            _model.Hidden = [.. _model.Hidden.Where(name => name != row.Name)];
        }

        UpdateCount();
    }

    private void UpdateCount()
    {
        var hidden = Rows.Count(row => !row.IsVisible);
        var missing = Rows.Count(row => row.IsMissing);

        CountText = missing > 0
                        ? $"共 {Rows.Count} 个分类 · 已隐藏 {hidden} 个 · 已不存在 {missing} 个"
                        : $"共 {Rows.Count} 个分类 · 已隐藏 {hidden} 个";
    }
}

/// <summary>
///     「积木分类管理」中的一行分类。
/// </summary>
public partial class BlocklyCategoryRowViewModel : ObservableObject
{
    private readonly Action<BlocklyCategoryRowViewModel>? _onDeleteRequested;
    private readonly Action<BlocklyCategoryRowViewModel>? _onVisibilityChanged;
    private bool _isVisible;

    public BlocklyCategoryRowViewModel(string name, CategoryMetadata? metadata, bool isVisible,
                                       Action<BlocklyCategoryRowViewModel> onVisibilityChanged,
                                       Action<BlocklyCategoryRowViewModel> onDeleteRequested)
    {
        Name = name;
        Metadata = metadata;
        _isVisible = isVisible;
        _onVisibilityChanged = onVisibilityChanged;
        _onDeleteRequested = onDeleteRequested;
    }

    /// <summary>
    ///     分类名称（同时是分类的唯一标识）。
    /// </summary>
    public string Name { get; }

    /// <summary>
    ///     分类元数据。分类已不存在时为 null。
    /// </summary>
    public CategoryMetadata? Metadata { get; }

    /// <summary>
    ///     当前是否仍注册了该分类。
    /// </summary>
    public bool Exists => Metadata is not null;

    /// <summary>
    ///     分类已不存在（插件被卸载或更新）：保留在列表中并标黄提示。
    /// </summary>
    public bool IsMissing => Metadata is null;

    /// <summary>
    ///     分类图标字形。已不存在的分类取不到元数据，用问号图标兜底。
    /// </summary>
    public string Glyph => Metadata?.Icon.Item2 ?? FluentIcons.QuestionCircleRegular;

    /// <summary>
    ///     分类主题色（主色），与 Blockly 工具箱里分类行的颜色一致。
    ///     已不存在的分类没有配色，此时为空，界面上只留一个空心色块占位。
    /// </summary>
    public string? PrimaryColor => Metadata?.Colors.Primary;

    /// <summary>
    ///     是否在 Blockly 工具箱中展示。
    /// </summary>
    public bool IsVisible
    {
        get => _isVisible;
        set
        {
            if (value == _isVisible)
            {
                return;
            }

            _isVisible = value;
            OnPropertyChanged();
            _onVisibilityChanged?.Invoke(this);
        }
    }

    /// <summary>
    ///     从列表中删除已不存在的分类。
    /// </summary>
    [RelayCommand]
    private void Delete()
    {
        _onDeleteRequested?.Invoke(this);
    }
}
