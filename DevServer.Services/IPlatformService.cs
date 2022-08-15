namespace DevServer.Services;

public interface IPlatformService
{
    string GetUserDataPath();
    string GetUserCachePath();
    string GetAppRootPath();
    string GetAppCachePath();
    string GetEntryStorePath();
}
