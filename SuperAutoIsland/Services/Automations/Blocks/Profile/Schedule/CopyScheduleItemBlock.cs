using System.Text.Json;
using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Core.Icons;
using ClassIsland.Shared;
using ClassIsland.Shared.Helpers;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Services.Automations.Blocks.Profile.Common;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.Schedule;

/// <summary>
/// 复制指定课程（日程项目），并输出新课程的 GUID。
/// </summary>
public class CopyScheduleItemBlock : DataBlockBase
{
    public override string Id => "sai.profile.data.copyScheduleItem";
    public override string Name => "复制课程";
    public override (string, string) Icon => ("日程", FluentIcons.CalendarRegular);
    public override string Tooltip => "复制指定课程（日程项目），并输出新课程的 GUID。复制出的课程信息与原课程相同。";
    public override string DataOutput => "SAI_Profile_ScheduleItem";

    public override void GetFields(FieldsRegister it) => it
        .AddField("ScheduleItem", ProfileFields.ScheduleItem(""));

    public override Task<object> Handler(object? data)
    {
        var settings = JsonSerializer.SerializeToElement(data);
        var profile = IAppHost.GetService<IProfileService>().Profile;
        var source = ProfileBlockHelpers.ScheduleItem(settings);
        var copy = source is null ? null : ConfigureFileHelper.CopyObject(source);
        if (copy is null)
        {
            return Task.FromResult<object>(Guid.Empty.ToString());
        }

        var id = Guid.NewGuid();
        profile.ScheduleItems.Add(id, copy);
        return Task.FromResult<object>(id.ToString());
    }
}
