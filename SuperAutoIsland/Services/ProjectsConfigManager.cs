using SuperAutoIsland.Enums;
using SuperAutoIsland.Models;
using SuperAutoIsland.Shared;
using SuperAutoIsland.Shared.Logger;

namespace SuperAutoIsland.Services;

/// <summary>
/// 提供项目配置管理的功能，包括初始化、创建、获取、保存和删除项目。
/// </summary>
public static class ProjectsConfigManager
{
    private static readonly Logger Logger = new(typeof(ProjectsConfigManager).ToString());
    private static string _configPath = string.Empty;

    /// <summary>
    /// 初始化项目配置管理器，设置配置路径并创建项目文件夹（如果不存在）。
    /// </summary>
    public static void Initialization()
    {
        _configPath = Path.Combine(GlobalConstants.PluginConfigFolder!, "Projects");
        if (!Directory.Exists(_configPath))
        {
            Directory.CreateDirectory(_configPath);
            Logger.Info("已创建项目文件夹");
        }
    }

    /// <summary>
    /// 创建一个新的项目并添加到项目配置中。
    /// </summary>
    /// <param name="type">项目的类型。</param>
    /// <param name="name">项目的名称。</param>
    /// <returns>新创建的项目实例。</returns>
    public static Project CreateProject(ProjectsType type, string name)
    {
        var newProject = new Project()
        {
            Type = type,
            Name = name,
        };
        GlobalConstants.Configs.ProjectConfig!.Data.Projects.Add(newProject);
        return newProject;
    }

    /// <summary>
    /// 根据 Guid 获取项目实例。
    /// </summary>
    /// <param name="guid">项目的 Guid。</param>
    /// <returns>找到的项目实例。</returns>
    /// <exception cref="KeyNotFoundException">如果找不到对应的项目，则抛出此异常。</exception>
    public static Project GetProject(Guid guid)
    {
        Logger.Debug($"获取项目 {guid} 中");
        foreach (var project in GlobalConstants.Configs.ProjectConfig!.Data.Projects)
        {
            if (project.Id == guid)
            {
                Logger.Debug($"获取项目 {guid} 成功");
                return project;
            }
        }

        Logger.Debug($"获取项目 {guid} 失败！");
        throw new KeyNotFoundException();
    }
    
    /// <summary>
    /// 根据 Guid 获取项目实例，如果不存在则创建新项目。
    /// </summary>
    /// <param name="projectsType">项目的类型。</param>
    /// <param name="guid">项目的 Guid。</param>
    /// <param name="name">项目的名称，当创建新项目时使用。</param>
    /// <returns>找到或新创建的项目实例。</returns>
    public static Project GetOrCreateProject(ProjectsType projectsType, Guid guid, string? name)
    {
        Logger.Debug($"获取项目 {guid} fallback {name} 中");
        foreach (var project in GlobalConstants.Configs.ProjectConfig!.Data.Projects)
        {
            if (project.Id == guid)
            {
                Logger.Debug($"获取项目 {guid} 成功");
                return project;
            }
        }

        Logger.Debug($"获取项目 {guid} 失败！Falling back.");
        return CreateProject(projectsType, name ?? "新项目");
    }
    
    /// <summary>
    /// 检查指定 Guid 的项目是否存在。
    /// </summary>
    /// <param name="id">项目的 Guid。</param>
    /// <returns>如果项目存在则返回true，否则返回false。</returns>
    public static bool CheckProject(Guid id)
    {
        return GlobalConstants.Configs.ProjectConfig!.Data.Projects.Any(project => project.Id == id);
    }
    
    /// <summary>
    /// 保存 Blockly 类型项目的 workspace 和生成的 JavaScript 代码。
    /// </summary>
    /// <param name="project">要保存的项目实例。</param>
    /// <param name="workspace">Blockly 的工作区数据。</param>
    /// <param name="code">生成的 JavaScript 代码。</param>
    /// <exception cref="ArgumentException">如果项目类型不是 BlocklyAction，则抛出此异常。</exception>
    public static void SaveBlocklyProject(Project project, string workspace, string code)
    {
        Logger.Info($"正在保存 Blockly 项目 {project.Name}");
        if (project.Type is not ProjectsType.BlocklyAction) throw new ArgumentException();
        
        WriteFile(Path.Combine(_configPath, $"{project.Id}.workspace.json"), workspace);
        WriteFile(Path.Combine(_configPath, $"{project.Id}.js"), code);
    }

    /// <summary>
    /// 删除指定的项目。
    /// </summary>
    /// <param name="project">要删除的项目实例。</param>
    public static void DeleteProject(Project project)
    {
        if (!CheckProject(project.Id)) return;

        Logger.Info($"正在删除项目 {project.Name}");
        GlobalConstants.Configs.ProjectConfig!.Data.Projects.Remove(project);
    }

    /// <summary>
    /// 加载 Blockly 类型项目的 workspace 数据。
    /// </summary>
    /// <param name="project">要加载的项目实例。</param>
    /// <returns>项目的工作区数据。</returns>
    /// <exception cref="NotSupportedException">如果项目类型不是 BlocklyAction，则抛出此异常。</exception>
    public static string LoadBlocklyProjectWorkspace(Project project)
    {
        Logger.Info($"正在加载项目 {project.Name}");
        if (project.Type is ProjectsType.BlocklyAction)
        {
            Logger.Debug("项目类型：BlocklyAction");
            return LoadFile(Path.Combine(_configPath, $"{project.Id}.workspace.json"));
        }

        throw new NotSupportedException();
    }

    /// <summary>
    /// 加载 Blockly 类型项目的 JavaScript 代码。
    /// </summary>
    /// <param name="project">要加载的项目实例。</param>
    /// <returns>项目的 JavaScript 代码。</returns>
    /// <exception cref="NotSupportedException">如果项目类型不是 BlocklyAction，则抛出此异常。</exception>
    public static string LoadBlocklyProjectJs(Project project)
    {
        Logger.Info($"正在加载项目 {project.Name}");
        if (project.Type is ProjectsType.BlocklyAction)
        {
            Logger.Debug("项目类型：BlocklyAction");
            return LoadFile(Path.Combine(_configPath, $"{project.Id}.js"));
        }

        throw new NotSupportedException();
    }

    /// <summary>
    /// 将内容写入指定路径的文件。
    /// </summary>
    /// <param name="path">文件路径。</param>
    /// <param name="content">要写入的内容。</param>
    private static void WriteFile(string path, string content)
    {
        Logger.Debug($"正在写入文件 {path}");
        using var fileStream = new FileStream(path, FileMode.Create);
        using var streamWriter = new StreamWriter(fileStream);
        streamWriter.Write(content);
        fileStream.Flush(true);
    }

    /// <summary>
    /// 从指定路径的文件中读取内容。
    /// </summary>
    /// <param name="path">文件路径。</param>
    /// <returns>文件内容。</returns>
    private static string LoadFile(string path)
    {
        Logger.Debug($"正在读取文件 {path}");
        return File.ReadAllText(path);
    }
}
