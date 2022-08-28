using System.Collections.Immutable;
using System.Diagnostics;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;

using DevServer.Models;

using FluentAssertions;

using Xunit;

using XFS = System.IO.Abstractions.TestingHelpers.MockUnixSupport;

namespace DevServer.Services.Tests;

public class GameLauncherTests
{
    [Fact]
    public void Start_ShouldStartProcess_Native()
    {
        var mockProcess = new ProcessMock();
        var mockPlatform = new PlatformMock();
        var mockFileSystem = new MockFileSystem();
        var mockConfig = new ConfigurationManager(mockPlatform, mockFileSystem);
        var service = new GameLauncher(mockProcess, mockConfig);

        mockConfig.Settings.OsuExePath = XFS.Path(@"C:\osu.exe");
        mockConfig.Settings.WineSettings = null;

        var expected = mockConfig.Settings.OsuExePath;
        var actual = service.Start("localhost").StartInfo.FileName;

        actual.Should().Be(expected);
    }

    [Fact]
    public void Start_ShouldStartProcess_Wine()
    {
        var mockProcess = new ProcessMock();
        var mockPlatform = new PlatformMock();
        var mockFileSystem = new MockFileSystem();
        var mockConfig = new ConfigurationManager(mockPlatform, mockFileSystem);
        var service = new GameLauncher(mockProcess, mockConfig);

        mockConfig.Settings.OsuExePath = XFS.Path(@"C:\osu.exe");
        mockConfig.Settings.WineSettings = new WineStartInfo
        {
            Path = "/bin/wine",
            Prefix = "/wineprefix",
            Arch = WineArch.Win32,
            Environment = ImmutableDictionary<string, string>.Empty
        };

        var expected = mockConfig.Settings.WineSettings.Path;
        var actual = service.Start("localhost").StartInfo.FileName;

        actual.Should().Be(expected);
    }

    [Fact]
    public void StartNative_ShouldStartProcess()
    {
        var mockProcess = new ProcessMock();
        var mockPlatform = new PlatformMock();
        var mockFileSystem = new MockFileSystem();
        var mockConfig = new ConfigurationManager(mockPlatform, mockFileSystem);
        var service = new GameLauncher(mockProcess, mockConfig);

        mockConfig.Settings.OsuExePath = XFS.Path(@"C:\osu.exe");
        mockConfig.Settings.WineSettings = null;

        var expected = new ProcessStartInfo
        {
            FileName = mockConfig.Settings.OsuExePath, Arguments = "-devserver localhost"
        };
        var actual = service.StartNative("localhost")?.StartInfo;

        actual.Should().BeEquivalentTo(expected,
                                       x => x
                                            .Including(p => p.FileName)
                                            .Including(p => p.Arguments));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("STAGING_AUDIO_PERIOD=10000")]
    [InlineData("STAGING_AUDIO_PERIOD=10000 WINEFSYNC=1")]
    public void StartWine_ShouldStartProcess(string? environment)
    {
        var mockProcess = new ProcessMock();
        var mockPlatform = new PlatformMock();
        var mockFileSystem = new MockFileSystem();
        var mockConfig = new ConfigurationManager(mockPlatform, mockFileSystem);
        var service = new GameLauncher(mockProcess, mockConfig);

        var envVars = environment?
                      .Split(" ")
                      .Select(x => x.Split("="))
                      .ToDictionary(x => x[0],
                                    x => x[1]);

        mockConfig.Settings.OsuExePath = XFS.Path(@"C:\osu.exe");
        mockConfig.Settings.WineSettings = new WineStartInfo
        {
            Path = "/bin/wine", Prefix = "/wineprefix", Arch = WineArch.Win32, Environment = envVars
        };

        var expected = new ProcessStartInfo
        {
            FileName = mockConfig.Settings.WineSettings.Path,
            Arguments = $"{mockConfig.Settings.OsuExePath} -devserver localhost",
            Environment =
            {
                { "WINEPREFIX", $"{mockConfig.Settings.WineSettings.Prefix}" },
                { "WINEARCH", mockConfig.Settings.WineSettings.Arch.ToString().ToLower() }
            }
        };

        if (envVars != null)
        {
            foreach (var envVar in envVars)
            {
                expected.EnvironmentVariables[envVar.Key] = envVar.Value;
            }
        }

        var actual = service.StartWine("localhost")!.StartInfo;

        actual.Should().BeEquivalentTo(expected,
                                       x => x
                                            .Including(p => p.FileName)
                                            .Including(p => p.Arguments)
                                            .Including(p => p.Environment));
    }
}
