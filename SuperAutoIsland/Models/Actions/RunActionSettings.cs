/*
 * 从 https://github.com/ClassIsland/ClassIsland/tree/dev/ClassIsland/Models/Actions/RunActionSettings.cs 复制的代码。
 * 用途：制作 SuperAutoIsland Wrapper
 * 本代码以 GNU General Public License v3.0 协议发行。
 */

using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;

namespace SuperAutoIsland.Models.Actions;

/// <summary>
///     "运行"行动设置。
/// </summary>
public partial class RunActionSettings : ObservableRecipient
{
    /// <summary>
    ///     "运行"行动运行类型。
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum RunActionRunType
    {
        /// <summary>
        ///     应用程序
        /// </summary>
        Application,

        /// <summary>
        ///     命令
        /// </summary>
        Command,

        /// <summary>
        ///     文件
        /// </summary>
        File,

        /// <summary>
        ///     文件夹
        /// </summary>
        Folder,

        /// <summary>
        ///     Url 链接
        /// </summary>
        Url
    }

    [ObservableProperty]
    private string _args = "";

    [ObservableProperty]
    private RunActionRunType _runType;

    [ObservableProperty]
    private string _value = "";
}