// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace DevServer.Services.Tests;

using XFS = System.IO.Abstractions.TestingHelpers.MockUnixSupport;

/// <summary>
/// Returns <c>C:\</c> for each path for simplicity
/// </summary>
public class PlatformMock : IPlatformService
{
    // leaving public and mutable if needs to be changed later
    public string Path { get; set; } = XFS.Path(@"C:\");

    public string GetUserDataPath() => Path;

    public string GetUserCachePath() => Path;

    public string GetAppRootPath() => Path;

    public string GetAppCachePath() => Path;

    public string GetEntryStorePath() => Path;
}
