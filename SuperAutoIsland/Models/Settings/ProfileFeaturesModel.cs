using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;

namespace SuperAutoIsland.Models.Settings;

/// <summary>
/// 档案功能设置模型。按积木所属板块划分，每个板块分为读取、写入两个子功能。
/// 子功能开关变化后无需重启，SAI 会重新构建档案分类的积木。
/// </summary>
public partial class ProfileFeaturesModel : ObservableObject
{
    /// <summary>
    /// 构造函数，监听各板块的开关变化
    /// <see cref="ProfileFeaturesModel"/>
    /// </summary>
    public ProfileFeaturesModel()
    {
        foreach (var section in Sections)
        {
            section.PropertyChanged += OnSectionPropertyChanged;
        }
    }

    /// <summary>
    /// 科目
    /// </summary>
    public ProfileSectionSettings Subject
    {
        get;
        set => SetSection(ref field, value);
    } = new();

    /// <summary>
    /// 时间点
    /// </summary>
    public ProfileSectionSettings TimePoint
    {
        get;
        set => SetSection(ref field, value);
    } = new(false, false);

    /// <summary>
    /// 时间表
    /// </summary>
    public ProfileSectionSettings TimeLayout
    {
        get;
        set => SetSection(ref field, value);
    } = new();

    /// <summary>
    /// 课表
    /// </summary>
    public ProfileSectionSettings ClassPlan
    {
        get;
        set => SetSection(ref field, value);
    } = new();

    /// <summary>
    /// 课表群
    /// </summary>
    public ProfileSectionSettings ClassPlanGroup
    {
        get;
        set => SetSection(ref field, value);
    } = new();

    /// <summary>
    /// 日程（课程）
    /// </summary>
    public ProfileSectionSettings Schedule
    {
        get;
        set => SetSection(ref field, value);
    } = new();

    /// <summary>
    /// 所有板块，顺序与分类中的积木分组一致
    /// </summary>
    [JsonIgnore]
    public IReadOnlyList<ProfileSectionSettings> Sections =>
        [Subject, TimePoint, TimeLayout, ClassPlan, ClassPlanGroup, Schedule];

    /// <summary>
    /// 设置板块开关，并把属性变化监听迁移到新的对象上
    /// </summary>
    /// <param name="field">原板块开关</param>
    /// <param name="value">新板块开关</param>
    /// <param name="propertyName">属性名</param>
    private void SetSection(ref ProfileSectionSettings field, ProfileSectionSettings value,
        [CallerMemberName] string? propertyName = null)
    {
        if (value is null || ReferenceEquals(field, value))
        {
            return;
        }

        field.PropertyChanged -= OnSectionPropertyChanged;
        field = value;
        field.PropertyChanged += OnSectionPropertyChanged;

        OnPropertyChanged(propertyName);
    }

    /// <summary>
    /// 板块开关变化时向上转发，便于配置保存与积木重建
    /// </summary>
    private void OnSectionPropertyChanged(object? sender, PropertyChangedEventArgs e) =>
        OnPropertyChanged(nameof(Sections));
}
