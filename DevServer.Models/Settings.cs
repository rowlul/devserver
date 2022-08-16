using System.Runtime.Serialization;
using System.Text.Json.Serialization;

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
            ? new WineStartInfo(Path: "/usr/bin/wine",
                                Prefix:
                                $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}/.wineprefix",
                                Arch: WineArch.Win32,
                                new Dictionary<string, string>())
            : null;

    [JsonPropertyName("ExecutablePath")]
    public string OsuExePath { get; set; } = GetPath();

    [JsonPropertyName("Wine")]
    public WineStartInfo? WineSettings { get; set; } = GetWineSettings();
}
