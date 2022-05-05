namespace DevServer.Services;

public enum WineArch
{
    Win32,
    Win64
}

public sealed class WineStartInfo
{
    public string Path { get; set; }
    public string Prefix { get; set; }
    public WineArch Arch { get; set; }
    public IDictionary<string, string>? Environment { get; set; }
}
