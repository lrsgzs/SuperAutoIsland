namespace SuperAutoIsland.Models;

public record LibItem(string FileNameSubfix, string UrlSubfix)
{
    public string FileName => $"ClearScriptV8.{FileNameSubfix}";
    public string Url => $"https://livefile.xesimg.com/programme/python_assets/{UrlSubfix}";
}