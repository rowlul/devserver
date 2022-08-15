namespace DevServer.Models;

public enum WineArch
{
    Win32,
    Win64
}

public record WineStartInfo(string Path, string Prefix, WineArch Arch, IDictionary<string, string>? Environment);
