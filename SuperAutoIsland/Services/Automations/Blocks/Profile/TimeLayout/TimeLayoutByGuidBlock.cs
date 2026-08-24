using ClassIsland.Core.Icons;
using SuperAutoIsland.Interface.Metadata;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Interface.Services.Automations;
using SuperAutoIsland.Models.Data;

namespace SuperAutoIsland.Services.Automations.Blocks.Profile.TimeLayout;

public class TimeLayoutByGuidBlock : DataBlockBase
{
    public override string Id => "sai.profile.data.timeLayoutByGuid";
    public override string Name => "时间表";
    public override (string, string) Icon => ("表格", FluentIcons.TableRegular);
    public override Type SettingsType => typeof(StringValueData);
    public override string DataOutput => "SAI_Profile_TimeLayout";

    public override void GetFields(FieldsRegister it) => it
        .AddField("Value", BasicFields.DynamicDropdown("", "sai.profile.dd.timeLayouts"));

    public override Task<object> Handler(object? data) =>
        Task.FromResult<object>(data is StringValueData settings
            ? settings.Value
            : Guid.Empty.ToString());
}
