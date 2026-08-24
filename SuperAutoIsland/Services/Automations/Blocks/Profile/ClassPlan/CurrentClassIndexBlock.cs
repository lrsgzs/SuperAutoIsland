using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Shared;
using SuperAutoIsland.Interface.Services.Automations;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.ClassPlan;

public class CurrentClassIndexBlock : DataBlockBase
{
    public override string Id => "sai.profile.data.currentClassIndex";
    public override string Name => "当前为第几节课";
    public override string DataOutput => "Number";
    public override Task<object> Handler(object? data)
    {
        var lessons = IAppHost.GetService<ILessonsService>();
        var plan = lessons.CurrentClassPlan;
        var layoutIndex = lessons.CurrentSelectedIndex;
        var classIndex = plan?.TimeLayout?.Layouts
            .Take(layoutIndex + 1)
            .Count(x => x.TimeType == 0) ?? 0;
        var currentItem = plan?.TimeLayout?.Layouts.ElementAtOrDefault(layoutIndex);
        return Task.FromResult<object>(currentItem?.TimeType == 0 ? classIndex : 0);
    }
}
