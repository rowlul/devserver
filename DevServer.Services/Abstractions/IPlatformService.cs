using System.Runtime.InteropServices;

namespace DevServer.Services;

public interface IPlatformService
{
    string GetUserDataPath();
    string GetUserCachePath();
    string GetAppDataPath();
    string GetAppCachePath();
    OSPlatform GetOperatingSystem();
}
