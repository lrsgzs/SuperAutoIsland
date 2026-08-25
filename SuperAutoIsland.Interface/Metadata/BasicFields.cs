namespace SuperAutoIsland.Interface.Metadata;

public delegate void FieldSetter(Field field);
public delegate void InputFieldSetter(InputField field);

public static class BasicFields
{
    public static Field CreateField(string name, FieldSetter? setter = null)
    {
        var field = new Field
        {
            Name = name
        };
        setter?.Invoke(field);
        return field;
    }

    public static InputField CreateInputField(string name, InputFieldSetter? setter = null)
    {
        var field = new InputField
        {
            Type = "input_value",
            Name = name
        };
        setter?.Invoke(field);
        return field;
    }
    
    public static Field Dummy(string name, FieldSetter? setter = null) =>
        CreateField(name, x =>
        {
            x.Type = "input_dummy";
            setter?.Invoke(x);
        });
    
    public static InputField Text(string name, string defaultValue = "", InputFieldSetter? setter = null) =>
        CreateInputField(name, x =>
        {
            x.Check = "String";
            x.ShadowBlockType = "text";
            x.Options["TEXT"] = defaultValue;
            setter?.Invoke(x);
        });
    
    public static InputField Number(string name, double defaultValue = 0, InputFieldSetter? setter = null) =>
        CreateInputField(name, x =>
        {
            x.Check = "Number";
            x.ShadowBlockType = "math_number";
            x.Options["NUM"] = defaultValue;
            setter?.Invoke(x);
        });
    
    public static InputField Boolean(string name, bool defaultValue = false, InputFieldSetter? setter = null) =>
        CreateInputField(name, x =>
        {
            x.Check = "Boolean";
            x.ShadowBlockType = "logic_boolean";
            x.Options["BOOL"] = defaultValue ? "TRUE" : "FALSE";
            setter?.Invoke(x);
        });
    
    public static InputField Date(string name, DateOnly defaultValue = default, InputFieldSetter? setter = null) =>
        CreateInputField(name, x =>
        {
            x.Check = "SAI_Date";
            x.ShadowBlockType = "date_block";
            x.Options["DATE"] = defaultValue;
            setter?.Invoke(x);
        });
    
    public static InputField Time(string name, TimeSpan defaultValue = default, InputFieldSetter? setter = null) =>
        CreateInputField(name, x =>
        {
            x.Check = "SAI_Time";
            x.ShadowBlockType = "time_block";
            x.Options["TIME"] = defaultValue;
            setter?.Invoke(x);
        });
    
    public static Field CheckBox(string name, bool defaultValue = false, FieldSetter? setter = null) =>
        CreateField(name, x =>
        {
            x.Type = "field_checkbox";
            x.Options["checked"] = defaultValue;
            setter?.Invoke(x);
        });
    
    public static Field DatePicker(string name, DateOnly defaultValue = default, FieldSetter? setter = null) =>
        CreateField(name, x =>
        {
            x.Type = "field_date";
            x.Options["date"] = defaultValue;
            setter?.Invoke(x);
        });
    
    /// <summary>
    /// options 的 ValueTuple 第一项为显示内容，第二项为后台内容
    /// </summary>
    public static Field Dropdown(string name, List<(string, string)> options, bool useNumbers = false, FieldSetter? setter = null) =>
        CreateField(name, x =>
        {
            x.Type = "field_dropdown";
            x.Options["options"] = options
                .Select(tuple => new List<string> { tuple.Item1, tuple.Item2 })
                .ToList();
            x.Options["useNumbers"] = useNumbers;
            setter?.Invoke(x);
        });
    
    public static Field DynamicDropdown(string name, string id, bool useNumbers = false, FieldSetter? setter = null) =>
        CreateField(name, x =>
        {
            x.Type = "internal_dynamic_dropdown";
            x.Options["id"] = id;
            x.Options["useNumbers"] = useNumbers;
            setter?.Invoke(x);
        });
    
    public static InputField Dictionary(string name, InputFieldSetter? setter = null) =>
        CreateInputField(name, x =>
        {
            x.Check = "Dictionary";
            setter?.Invoke(x);
        });
}