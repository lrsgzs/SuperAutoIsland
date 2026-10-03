using System.Text.Json;
using ClassIsland.Core.Icons;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Services.Automations.Blocks.Profile.Common;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.Schedule;

/// <summary>
///     获取课程的科目，输出科目 GUID。
/// </summary>
public class GetScheduleItemSubjectBlock : DataBlockBase
{
    public override string Id => "sai.profile.data.scheduleItemSubject";
    public override string Name => "课程";
    public override (string, string) Icon => ("日程", FluentIcons.CalendarRegular);
    public override string Tooltip => "获取课程的科目。课程不存在时返回空 GUID。";
    public override string DataOutput => "SAI_Profile_Subject";
    public override bool InlineBlock => true;
    public override bool InlineField => true;

    public override void GetFields(FieldsRegister it)
    {
        it
            .AddField("ScheduleItem", ProfileFields.ScheduleItem(""))
            .AddDummy("的科目");
    }

    public override Task<object> Handler(object? data)
    {
        var settings = JsonSerializer.SerializeToElement(data);
        var subjectId = ProfileBlockHelpers.ScheduleItem(settings)?.SubjectId ?? Guid.Empty;
        return Task.FromResult<object>(subjectId.ToString());
    }
}