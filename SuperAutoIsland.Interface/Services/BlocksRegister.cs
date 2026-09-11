using SuperAutoIsland.Interface.Metadata;
using SuperAutoIsland.Interface.Services.Automations;

namespace SuperAutoIsland.Interface.Services;

public class BlocksRegister(string categoryName)
{
    public List<BlockMetadata> Items { get; } = [];
    public Dictionary<string, BlockBase> Blocks { get; } = [];
    private string _categoryName = categoryName;

    public BlocksRegister() : this(string.Empty)
    {}

    public BlocksRegister AddBlock(BlockMetadata block)
    {
        // modify tooltip
        
        var tooltip = block.Tooltip;

        block.Tooltip = block.Kind switch
        {
            BlockKind.Action => $"(行动) {_categoryName}\n{block.Id}",
            BlockKind.Rule => $"(规则) {_categoryName}\n{block.Id} => Boolean",
            BlockKind.Data => $"(数据) {_categoryName}\n{block.Id} => {block.DataOutput}",
            _ => string.Empty
        };

        if (!string.IsNullOrWhiteSpace(tooltip))
        {
            block.Tooltip += "\n" + tooltip;
        }
        
        Items.Add(block);
        return this;
    }

    public BlocksRegister AddBlock<T>() where T : BlockBase, new()
    {
        var block = new T();
        var fieldsRegister = new FieldsRegister();
        block.GetFields(fieldsRegister);
        
        Blocks[block.Id] = block;
        AddBlock(new BlockMetadata(block.Id)
        {
            Kind = block.Kind,
            Name = block.Name,
            Icon = block.Icon,
            Tooltip = block.Tooltip,
            Fields = fieldsRegister.Fields,
            InlineBlock = block.InlineBlock,
            InlineField = block.InlineField,
            DataOutput = block.DataOutput,
        });
        return this;
    }

    public BlocksRegister AddLabel(string label)
    {
        Items.Add(new BlockMetadata(Guid.NewGuid().ToString()[..8])
        {
            Kind = BlockKind.Label,
            Name = label,
        });
        return this;
    }
}