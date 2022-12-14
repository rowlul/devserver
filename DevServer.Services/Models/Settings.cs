using System.Text.Json.Serialization;

namespace DevServer.Services;

public class Settings
{
    [JsonPropertyName("ExecutablePath")]
    public string OsuExePath { get; set; } = GetPath();

    [JsonPropertyName("Wine")]
    public WineStartInfo? WineSettings { get; set; } = GetWineSettings();

    [JsonPropertyName("LastServer")]
    public string? LastServerAddress { get; set; }

    private static string GetPath()
    {
        return OperatingSystem.IsWindows()
            ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                           "osu!",
                           "osu!.exe")
            : string.Empty;
    }

    private static WineStartInfo? GetWineSettings()
    {
        return OperatingSystem.IsLinux()
            ? new WineStartInfo
            {
                Path = "/usr/bin/wine",
                Prefix =
                    $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}/.wineprefix",
                Arch = WineArch.Win32,
                Environment = new Dictionary<string, string>()
            }
            : null;
    }
}
