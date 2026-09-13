using System.Text.Json;
using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Core.Icons;
using ClassIsland.Shared;
using SuperAutoIsland.Interface.Metadata;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Services.Automations.Blocks.Profile.Common;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.ClassPlan;

/// <summary>
/// 按日期获取当天生效的课表。经典模式返回课表 GUID，日程模式返回「[sched]yyyy-MM-dd」引用。
/// </summary>
public class ClassPlanByDateBlock : DataBlockBase
{
    public override string Id => "sai.profile.data.classPlanByDate";
    public override string Name => "按日期获取课表";
    public override (string, string) Icon => ("文档", FluentIcons.DocumentDataRegular);
    public override string Tooltip => "按日期获取当天生效的课表。经典模式返回课表 GUID，日程模式返回「[sched]yyyy-MM-dd」引用。";
    public override string DataOutput => "SAI_Profile_ClassPlan";

    public override void GetFields(FieldsRegister it) => it
        .AddField("Date", BasicFields.Date("", DateOnly.FromDateTime(DateTime.Today)));

    public override Task<object> Handler(object? data)
    {
        var settings = JsonSerializer.SerializeToElement(data);
        var date = ProfileBlockHelpers.Date(settings, "Date");
        var plan = IAppHost.GetService<ILessonsService>()
            .GetClassPlanByDate(date.ToDateTime(TimeOnly.MinValue), out var id);
        if (id is null)
        {
            return Task.FromResult<object>(plan is null
                ? Guid.Empty.ToString()
                : ProfileBlockHelpers.CreateScheduleRef(date));
        }

        return Task.FromResult<object>(id.Value.ToString());
    }
}
