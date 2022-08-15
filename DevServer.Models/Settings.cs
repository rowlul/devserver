namespace DevServer.Models;

public class Settings
{
    private static string GetPath()
    {
        if (OperatingSystem.IsWindows())
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                                "osu!",
                                "osu!.exe");
        }

        return string.Empty;
    }

    public string OsuExePath { get; set; } = GetPath();
    public WineStartInfo? WineSettings { get; set; }
}
