using System.Runtime.InteropServices;

namespace DevServer.Services;

public interface IPlatformService
{
    string GetUserDataPath();
    string GetUserCachePath();
    string GetAppRootPath();
    string GetAppCachePath();
    string GetEntryStorePath();
    string GetConfigFile();
    OSPlatform GetOperatingSystem();
}
