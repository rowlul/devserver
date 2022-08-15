namespace DevServer.Services;

public class PlatformService : IPlatformService
{
    private readonly string _appBaseName;

    public PlatformService(string appBaseName)
    {
        _appBaseName = appBaseName;
    }

    public string GetUserDataPath() => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

    public string GetUserCachePath()
    {
        if (OperatingSystem.IsWindows())
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Temp");
        }
        else if (OperatingSystem.IsLinux())
        {
            // return ~/.cache if XDG envvar is not set
            return Environment.GetEnvironmentVariable("XDG_CACHE_HOME") ??
                   Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".cache");
        }
        else if (OperatingSystem.IsMacOS())
        {
            // Environment.SpecialFolder.InternetCache should return Library/Caches as well but it's misleading
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Library", "Caches");
        }

        throw new PlatformNotSupportedException();
    }

    public string GetAppRootPath() => Path.Combine(GetUserDataPath(), _appBaseName);

    public string GetAppCachePath() => Path.Combine(GetUserCachePath(), _appBaseName);

    public string GetEntryStorePath() => Path.Combine(GetAppRootPath(), "servers");

    public string GetConfigFile() => Path.Combine(GetAppRootPath(), "settings.json");
}
