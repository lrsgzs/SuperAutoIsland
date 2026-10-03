using ClassIsland.Core.Icons;
using SuperAutoIsland.Interface.Metadata;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Models.Data;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.Schedule;

/// <summary>
///     选择一个课程（日程项目）。
/// </summary>
public class ScheduleItemByGuidBlock : DataBlockBase
{
    public override string Id => "sai.profile.data.scheduleItem";
    public override string Name => "课程";
    public override (string, string) Icon => ("日程", FluentIcons.CalendarRegular);
    public override Type SettingsType => typeof(StringValueData);
    public override string DataOutput => "SAI_Profile_ScheduleItem";

    public override void GetFields(FieldsRegister it)
    {
        it
            .AddField("Value", BasicFields.DynamicDropdown("", "sai.profile.dd.scheduleItems"));
    }

    public override Task<object> Handler(object? data)
    {
        return Task.FromResult<object>(data is StringValueData settings
                                           ? settings.Value
                                           : Guid.Empty.ToString());
    }
}