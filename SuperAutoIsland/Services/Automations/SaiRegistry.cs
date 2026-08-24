using ClassIsland.Core.Icons;
using ClassIsland.Shared;
using ClassIsland.Shared.Models.Automation;
using SuperAutoIsland.Enums;
using SuperAutoIsland.Interface.Metadata;
using SuperAutoIsland.Interface.Services;
using SuperAutoIsland.Services.Automations.Blocks;
using SuperAutoIsland.Shared;

namespace SuperAutoIsland.Services.Automations;

public static class SaiRegistry
{
    private static ISaiServer SaiServer { get; } = IAppHost.GetService<ISaiServer>();

    public static void Register()
    {
        SaiServer.RegisterBlocks("SuperAutoIsland", it => it
            .AddLabel("动态文本")
            .AddBlock(new BlockMetadata("sai.actions.setDynamicText")
            {
                Kind = BlockKind.Action,
                Name = "设置动态文本",
                Icon = ("文本编辑", FluentIcons.TextEditStyleRegular),
                Fields = new Dictionary<string, Field>
                {
                    ["Key"] = BasicFields.Text("将"),
                    ["Value"] = BasicFields.Text("修改为")
                }
            })
            .AddBlock<GetDynamicTextBlock>()
            .AddLabel("项目")
            .AddBlock(new BlockMetadata("sai.actions.runBlockly")
            {
                Kind = BlockKind.Action,
                Name = "运行 Blockly 项目",
                Icon = ("Blockly 项目", FluentIcons.AlignSpaceEvenlyVerticalRegular),
                Fields = new Dictionary<string, Field>
                {
                    ["ProjectGuid"] = BasicFields.DynamicDropdown("", "sai.actions.runBlockly.options")
                }
            })
            .AddBlock(new BlockMetadata("sai.actions.runActionSet")
            {
                Kind = BlockKind.Action,
                Name = "运行可复用的行动组",
                Icon = ("行动组", FluentIcons.AirplaneTakeOffRegular),
                Fields = new Dictionary<string, Field>
                {
                    ["ProjectGuid"] = BasicFields.DynamicDropdown("", "sai.actions.runActionSet.options")
                }
            })
            .AddBlock(new BlockMetadata("sai.rules.runCiRuleset")
            {
                Kind = BlockKind.Rule,
                Name = "运行可复用的规则集",
                Icon = ("规则集", FluentIcons.TagMultipleRegular),
                Fields = new Dictionary<string, Field>
                {
                    ["ProjectGuid"] = BasicFields.DynamicDropdown("", "sai.rules.runCiRuleset.options")
                }
            })
            .AddLabel("对话框")
            .AddBlock(new BlockMetadata("sai.rules.dialogs.yesNo")
            {
                Kind = BlockKind.Rule,
                Name = "YesNo 对话框",
                Icon = ("对话框", FluentIcons.WindowHeaderHorizontalRegular),
                Fields = new Dictionary<string, Field>
                {
                    ["dummy1"] = BasicFields.Dummy(""),
                    ["Header"] = BasicFields.Text("标题", "选择您的答案..."),
                    ["Message"] = BasicFields.Text("消息", "NO ONE YES MAN!"),
                    ["YesText"] = BasicFields.Text("「Yes」按钮文本", "是"),
                    ["NoText"] = BasicFields.Text("「No」按钮文本", "否"),
                    ["PreferYes"] = BasicFields.Boolean("默认按钮为「Yes」？", true),
                    ["Topmost"] = BasicFields.Boolean("置顶？", false),
                    ["CountdownEnabled"] = BasicFields.Boolean("启用倒计时？", false),
                    ["CountdownMode"] = BasicFields.Dropdown("倒计时模式", [
                        ("可交互前倒计时", "0"), ("自动 Yes 倒计时", "1"), ("自动 No 倒计时", "2")
                    ], true),
                    ["CountdownTime"] = BasicFields.Number("倒计时时长(s)", 5),
                }
            })
            .AddBlock<TextDialogBlock>());

        SaiServer.RegisterDynamicDropdown("sai.actions.runBlockly.options", async () =>
            EnsureListHasItemOrDefaultListItem(
                GlobalConstants.Configs.ProjectConfig!.Data.Projects
                    .Where(e => e.Type is ProjectsType.BlocklyAction)
                    .Select(e => (e.Name, e.Id.ToString()))
                    .ToList(),
                new ValueTuple<string, string>("???",
                    GlobalConstants.Assets.ProjectNullGuid.ToString())));

        SaiServer.RegisterDynamicDropdown("sai.actions.runActionSet.options", async () =>
            EnsureListHasItemOrDefaultListItem(
                GlobalConstants.Configs.ProjectConfig!.Data.Projects
                    .Where(e => e.Type is ProjectsType.CiActionSet)
                    .Select(e => (e.Name, e.Id.ToString()))
                    .ToList(),
                new ValueTuple<string, string>("???",
                    GlobalConstants.Assets.ProjectNullGuid.ToString())));

        SaiServer.RegisterDynamicDropdown("sai.rules.runCiRuleset.options", async () =>
            EnsureListHasItemOrDefaultListItem(
                GlobalConstants.Configs.ProjectConfig!.Data.Projects
                    .Where(e => e.Type is ProjectsType.CiRuleset)
                    .Select(e => (e.Name, e.Id.ToString()))
                    .ToList(),
                new ValueTuple<string, string>("???",
                    GlobalConstants.Assets.ProjectNullGuid.ToString())));
    }

    private static List<T> EnsureListHasItemOrDefaultListItem<T>(List<T> data, T defaultItem)
    {
        return data.Count > 0 ? data : [defaultItem];
    }
}
