using SuperAutoIsland.Models;

namespace SuperAutoIsland.Shared;

public static class ClearScriptV8Libs
{
    public static Dictionary<string, List<LibItem>> Libs { get; } = new()
    {
        ["linux-arm64"] =
        [
            new LibItem("linux-arm64.so", "03784047DC2BF00858C08FA83F0C0289.so"),
            new LibItem("linux-arm64.so.pem", "BA2F99F5751359BE109D04289342303D.pem"),
            new LibItem("linux-arm64.so.sig", "2BFB8D9E6C489D2A0857742760094982.sig")
        ],
        ["linux-x64"] =
        [
            new LibItem("linux-x64.so", "32148AEDE4BF3681D19E77608F000533.so"),
            new LibItem("linux-x64.so.pem", "BA2F99F5751359BE109D04289342303D.pem"),
            new LibItem("linux-x64.so.sig", "14C3374CF76893DA9439334907F1DDAA.sig")
        ],
        ["osx-arm64"] = [new LibItem("osx-arm64.dylib", "7787CAE93E19CE8CAF08A7147534D232.dylib")],
        ["osx-x64"] = [new LibItem("osx-x64.dylib", "3EAA529D87C9CB833F91549424FB78B1.dylib")],
        ["win-arm64"] = [new LibItem("win-arm64.dll", "2845BDB0552DC98CFF78469921CF7E6B.dll")],
        ["win-x64"] = [new LibItem("win-x64.dll", "6097298B84DA7124977A20E3E1BD1DBA.dll")],
        ["win-x86"] = [new LibItem("win-x86.dll", "46F5BEBDCB924EA226E46B1943880E47.dll")]
    };
}