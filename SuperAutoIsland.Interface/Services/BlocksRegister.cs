using SuperAutoIsland.Interface.Metadata;
using SuperAutoIsland.Interface.Services.Automations;

namespace SuperAutoIsland.Interface.Services;

public class BlocksRegister
{
    public List<BlockMetadata> Items { get; } = [];
    public Dictionary<string, BlockBase> Blocks { get; } = [];

    public BlocksRegister AddBlock(BlockMetadata block)
    {
        Items.Add(block);
        return this;
    }

    public BlocksRegister AddBlock<T>() where T : BlockBase, new()
    {
        var block = new T();
        var fieldsRegister = new FieldsRegister();
        block.GetFields(fieldsRegister);
        
        Items.Add(new BlockMetadata(block.Id)
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
        Blocks[block.Id] = block;
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