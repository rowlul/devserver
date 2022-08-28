using System.Collections.Immutable;
using System.IO.Abstractions;

using DevServer.Models;

using XFS = System.IO.Abstractions.TestingHelpers.MockUnixSupport;

namespace DevServer.Services.Tests;

public class ConfigurationMock : ConfigurationManager
{
    public new Settings Settings { get; set; } = new()
    {
        OsuExePath = XFS.Path(@"C:\osu.exe"),
        WineSettings = new WineStartInfo
        {
            Path = "/bin/wine",
            Prefix = "/wineprefix",
            Arch = WineArch.Win32,
            Environment = ImmutableDictionary<string, string>.Empty
        }
    };

    public ConfigurationMock(IPlatformService platformService, IFileSystem fileSystem) : base(platformService, fileSystem)
    {
    }
}
