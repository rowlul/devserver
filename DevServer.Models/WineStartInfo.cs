using System.Runtime.Serialization;

namespace DevServer.Models;

public enum WineArch
{
    [EnumMember(Value = "win32")] Win32,
    [EnumMember(Value = "win64")] Win64
}

public record WineStartInfo(string Path, string Prefix, WineArch Arch, IDictionary<string, string>? Environment);
