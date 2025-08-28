namespace SuperAutoIsland;

public struct PackageInfo
{
    public string Url;
    public string FileName;
}

public static class ClearScriptPackages
{
    public static readonly Dictionary<string, PackageInfo> Infos = new();

    public static void Initialize()
    {
        Infos.Add("win-x86", new PackageInfo()
        {
            Url = "https://livefile.xesimg.com/programme/python_assets/d12fccd1cfcc8ae6975178923606a8d2.dll",
            FileName = "ClearScriptV8.win-x86.dll"
        });
        
        Infos.Add("win-x64", new PackageInfo()
        {
            Url = "https://livefile.xesimg.com/programme/python_assets/078840e21d89ef0c4c372fce1b364f77.dll",
            FileName = "ClearScriptV8.win-x64.dll"
        });
        
        Infos.Add("win-arm64", new PackageInfo()
        {
            Url = "https://livefile.xesimg.com/programme/python_assets/587722ab79c428c4c48f07774f063866.dll",
            FileName = "ClearScriptV8.win-arm64.dll"
        });
        
        Infos.Add("linux-x64", new PackageInfo()
        {
            Url = "https://livefile.xesimg.com/programme/python_assets/67329ae1400c3ca6e24b496ab3c02db2.so",
            FileName = "ClearScriptV8.linux-x64.so"
        });
        
        Infos.Add("linux-arm64", new PackageInfo()
        {
            Url = "https://livefile.xesimg.com/programme/python_assets/cd72f239c19a36d5961c930ac7a57a86.so",
            FileName = "ClearScriptV8.linux-arm64.so"
        });
        
        Infos.Add("osx-x64", new PackageInfo()
        {
            Url = "https://livefile.xesimg.com/programme/python_assets/e4c5fcd003b3920f110f037b5473bc37.dylib",
            FileName = "ClearScriptV8.osx-x64.dylib"
        });
        
        Infos.Add("osx-arm64", new PackageInfo()
        {
            Url = "https://livefile.xesimg.com/programme/python_assets/c3dea963b5c3a89203078c3a8787e471.dylib",
            FileName = "ClearScriptV8.osx-arm64.dylib"
        });
    }
}