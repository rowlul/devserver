using System.Runtime.Serialization;

namespace DevServer.Models;

public class Settings
{
    private static string GetPath() =>
        OperatingSystem.IsWindows()
            ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                           "osu!",
                           "osu!.exe")
            : string.Empty;

    private static WineStartInfo? GetWineSettings() =>
        OperatingSystem.IsLinux()
            ? new WineStartInfo(Path: "/usr/bin/wine", Prefix: "~/.wineprefix", Arch: WineArch.Win32, null)
            : null;

    public string OsuExePath { get; set; } = GetPath();
    public WineStartInfo? WineSettings { get; set; } = GetWineSettings();
}
