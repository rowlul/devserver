using System.Runtime.Serialization;

namespace DevServer.Models;

public enum WineArch
{
    [EnumMember(Value = "win32")] Win32,
    [EnumMember(Value = "win64")] Win64
}

public class WineStartInfo
{
    public string Path { get; set; }
    public string Prefix { get; set; }
    public WineArch Arch { get; set; }
    public IDictionary<string, string>? Environment { get; set; }
}
