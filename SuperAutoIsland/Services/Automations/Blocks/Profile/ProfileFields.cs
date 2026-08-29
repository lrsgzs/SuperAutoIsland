using SuperAutoIsland.Interface.Metadata;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile;

public static class ProfileFields
{
    private static InputField Create(string name, string check, string shadowBlockType) =>
        BasicFields.CreateInputField(name, field =>
        {
            field.Check = check;
            field.ShadowBlockType = shadowBlockType;
        });
    
    public static InputField Subject(string name) =>
        Create(name, "SAI_Profile_Subject", "sai_profile_data_subjectByGuid");
    
    public static InputField TimeLayout(string name) =>
        Create(name, "SAI_Profile_TimeLayout", "sai_profile_data_timeLayoutByGuid");
    
    public static InputField ClassPlan(string name) =>
        Create(name, "SAI_Profile_ClassPlan", "sai_profile_data_classPlanByGuid");
    
    public static InputField ClassPlanGroup(string name) =>
        Create(name, "SAI_Profile_ClassPlanGroup", "sai_profile_data_classPlanGroupByGuid");

    /// <summary>
    /// 时间表时间点输入。后台值为「时间表 GUID[序号]」格式的字符串。
    /// </summary>
    public static InputField TimeLayoutItem(string name) =>
        Create(name, "SAI_Profile_TimeLayoutItem", "sai_profile_data_timeLayoutItem");

    /// <summary>
    /// 时间点类型下拉框。后台值为数字：0-上课，1-课间，2-分割线，3-行动。
    /// </summary>
    public static Field TimePointType(string name) =>
        BasicFields.Dropdown(name, [
            ("上课", "0"),
            ("课间", "1"),
            ("分割线", "2"),
            ("行动", "3")
        ], true);
}
