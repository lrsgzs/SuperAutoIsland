namespace SuperAutoIsland.Interface.Metadata;

/// <summary>
///     分类配色：分类下积木用的三个颜色。
/// </summary>
/// <remarks>
///     <para>
///         Blockly 的积木用三个颜色来画（主色 / 阴影色 / 描边色），三个颜色必须取自 <b>同一色相</b>，
///         只是明度不同，否则积木的阴影和高光会串色：
///     </para>
///     <list type="bullet">
///         <item>
///             <see cref="Primary" />：主色，积木的填充色，同时用作工具箱里分类行的颜色
///         </item>
///         <item>
///             <see cref="Secondary" />：阴影色，同色相的<b>浅</b>色（默认取「主色与白色按 6:4 混合」，
///             例如 <c>#00AAFF</c> → <c>#99DDFF</c>）
///         </item>
///         <item>
///             <see cref="Tertiary" />：描边色，同色相的<b>深</b>色（默认取「主色 × 0.8」，即暗 20%，
///             例如 <c>#00AAFF</c> → <c>#0088CC</c>）
///         </item>
///     </list>
///     <para>
///         颜色写法为 <c>#AARRGGBB</c> 或 <c>#RRGGBB</c>，Alpha 会被忽略（积木颜色不透明）。
///         <see cref="Secondary" /> 与 <see cref="Tertiary" /> 留空时按上面的规则从
///         <see cref="Primary" /> 自动推导，所以只给一个主色也能用。
///     </para>
/// </remarks>
public class CategoryColors
{
    /// <summary>
    ///     默认主色（不指定配色时的表现）
    /// </summary>
    public const string DefaultPrimary = "#00AAFF";

    public CategoryColors()
    {
    }

    /// <summary>
    ///     只指定主色，另两个颜色自动推导
    /// </summary>
    public CategoryColors(string primary)
    {
        Primary = primary;
    }

    /// <summary>
    ///     指定三个颜色
    /// </summary>
    public CategoryColors(string primary, string? secondary, string? tertiary)
    {
        Primary = primary;
        Secondary = secondary;
        Tertiary = tertiary;
    }

    /// <summary>
    ///     主色：积木填充色，同时用作分类行的颜色
    /// </summary>
    public string Primary { get; set; } = DefaultPrimary;

    /// <summary>
    ///     阴影色：同色相的浅色。留空时取「主色与白色按 6:4 混合」
    /// </summary>
    public string? Secondary { get; set; }

    /// <summary>
    ///     描边色：同色相的深色。留空时取「主色 × 0.8」（暗 20%）
    /// </summary>
    public string? Tertiary { get; set; }
}
