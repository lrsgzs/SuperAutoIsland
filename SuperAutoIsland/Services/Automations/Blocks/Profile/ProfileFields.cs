using SuperAutoIsland.Interface.Metadata;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile;

public static class ProfileFields
{
    private static InputField Create(string name, string check, string shadowBlockType)
    {
        return BasicFields.CreateInputField(name, field =>
        {
            field.Check = check;
            field.ShadowBlockType = shadowBlockType;
        });
    }

    public static InputField Subject(string name)
    {
        return Create(name, "SAI_Profile_Subject", "sai_profile_data_subjectByGuid");
    }

    public static InputField TimeLayout(string name)
    {
        return Create(name, "SAI_Profile_TimeLayout", "sai_profile_data_timeLayoutByGuid");
    }

    public static InputField ClassPlan(string name)
    {
        return Create(name, "SAI_Profile_ClassPlan", "sai_profile_data_classPlanByGuid");
    }

    public static InputField ClassPlanGroup(string name)
    {
        return Create(name, "SAI_Profile_ClassPlanGroup", "sai_profile_data_classPlanGroupByGuid");
    }

    /// <summary>
    ///     课程（日程项目）输入。
    /// </summary>
    public static InputField ScheduleItem(string name)
    {
        return Create(name, "SAI_Profile_ScheduleItem", "sai_profile_data_scheduleItem");
    }

    /// <summary>
    ///     时间表时间点输入。后台值为「时间表 GUID[序号]」格式的字符串。
    /// </summary>
    public static InputField TimeLayoutItem(string name)
    {
        return Create(name, "SAI_Profile_TimeLayoutItem", "sai_profile_data_timeLayoutItem");
    }

    /// <summary>
    ///     时间点类型下拉框。后台值为数字：0-上课，1-课间，2-分割线，3-行动。
    ///     仅供「时间点类型」积木自身使用，其余积木请用 <see cref="TimePointTypeInput" /> 复用该积木。
    /// </summary>
    public static Field TimePointType(string name)
    {
        return BasicFields.Dropdown(name, [
            ("上课", "0"),
            ("课间", "1"),
            ("分割线", "2"),
            ("行动", "3")
        ], true);
    }

    /// <summary>
    ///     时间点类型输入。可插入已有的「时间点类型」积木，后台值为类型编号。
    /// </summary>
    public static InputField TimePointTypeInput(string name)
    {
        return Create(name, "SAI_Profile_TimeLayoutItemType", "sai_profile_data_timePointType");
    }

    /// <summary>
    ///     可复用的行动组（SAI 项目）下拉框，后台值为项目 GUID。
    /// </summary>
    public static Field ActionSet(string name)
    {
        return BasicFields.DynamicDropdown(name, "sai.actions.runActionSet.options");
    }
}