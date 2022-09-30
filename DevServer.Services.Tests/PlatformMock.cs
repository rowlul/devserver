// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

using System.Runtime.InteropServices;

namespace DevServer.Services.Tests;

using XFS = System.IO.Abstractions.TestingHelpers.MockUnixSupport;

/// <summary>
/// Returns <c>C:\</c> for each path for simplicity
/// </summary>
public class PlatformMock : IPlatformService
{
    // leaving public and mutable if needs to be changed later
    public string Path { get; set; } = XFS.Path(@"C:\");
    public OSPlatform Platform { get; set; }

    public string GetUserDataPath() => Path;

    public string GetUserCachePath() => Path;

    public string GetImageCachePath() => Path;

    public string GetAppRootPath() => Path;

    public string GetAppCachePath() => Path;

    public string GetEntryStorePath() => Path;

    public string GetConfigFile() => Path + "settings.json";

    public OSPlatform GetOperatingSystem() => Platform;
}
