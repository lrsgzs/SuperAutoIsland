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
}
