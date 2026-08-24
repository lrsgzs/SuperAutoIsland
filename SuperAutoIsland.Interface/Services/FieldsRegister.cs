using SuperAutoIsland.Interface.Metadata;

namespace SuperAutoIsland.Interface.Services;

public class FieldsRegister
{
    public Dictionary<string, Field> Fields { get; } = [];

    public FieldsRegister AddField(string id, Field field)
    {
        Fields.Add(id, field);
        return this;
    }

    public FieldsRegister AddDummy(string text = "")
    {
        return AddField(
            "dummy_" + Guid.NewGuid().ToString("N")[..8],
            BasicFields.Dummy(text));
    }
}