using SuperAutoIsland.Interface.Enums;
using SuperAutoIsland.Models;
using SuperAutoIsland.Shared;
using SuperAutoIsland.Shared.Logger;

namespace SuperAutoIsland.Services;

public static class ProjectsConfigManager
{
    private static readonly Logger Logger = new(typeof(ProjectsConfigManager).ToString());
    private static string _configPath = string.Empty;

    public static void Initialization()
    {
        _configPath = Path.Combine(GlobalConstants.PluginConfigFolder!, "Projects");
        if (!Directory.Exists(_configPath))
        {
            Directory.CreateDirectory(_configPath);
            Logger.Info("已创建项目文件夹");
        }
    }

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
    
    public static bool CheckProject(Guid id)
    {
        return GlobalConstants.Configs.ProjectConfig!.Data.Projects.Any(project => project.Id == id);
    }
    
    public static void SaveBlocklyProject(Project project, string workspace, string code)
    {
        Logger.Info($"正在保存 Blockly 项目 {project.Name}");
        if (project.Type is not ProjectsType.BlocklyAction) throw new ArgumentException();
        
        WriteFile(Path.Combine(_configPath, $"{project.Id}.workspace.json"), workspace);
        WriteFile(Path.Combine(_configPath, $"{project.Id}.js"), code);
    }

    public static void DeleteProject(Project project)
    {
        if (!CheckProject(project.Id)) return;

        Logger.Info($"正在删除项目 {project.Name}");
        GlobalConstants.Configs.ProjectConfig!.Data.Projects.Remove(project);
    }

    public static string LoadProject(Project project)
    {
        Logger.Info($"正在加载项目 {project.Name}");
        if (project.Type is ProjectsType.BlocklyAction)
        {
            Logger.Debug("项目类型：BlocklyAction");
            return LoadFile(Path.Combine(_configPath, $"{project.Id}.workspace.json"));
        }

        throw new NotSupportedException();
    }

    public static string LoadProjectJs(Project project)
    {
        Logger.Info($"正在加载项目 {project.Name}");
        if (project.Type is ProjectsType.BlocklyAction)
        {
            Logger.Debug("项目类型：BlocklyAction");
            return LoadFile(Path.Combine(_configPath, $"{project.Id}.js"));
        }

        throw new NotSupportedException();
    }

    private static void WriteFile(string path, string content)
    {
        Logger.Debug($"正在写入文件 {path}");
        using var fileStream = new FileStream(path, FileMode.Create);
        using var streamWriter = new StreamWriter(fileStream);
        streamWriter.Write(content);
        fileStream.Flush(true);
    }

    private static string LoadFile(string path)
    {
        Logger.Debug($"正在读取文件 {path}");
        return File.ReadAllText(path);
    }
}