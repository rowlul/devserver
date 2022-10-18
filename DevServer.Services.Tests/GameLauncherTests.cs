using System.Collections.Immutable;
using System.Diagnostics;
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
        var service = new GameLauncher(mockProcess);

        var path = XFS.Path(@"C:\osu.exe");

        var expected = path;
        var actual = service.Start(path, "localhost", null).StartInfo.FileName;

        actual.Should().Be(expected);
    }

    [Fact]
    public void Start_ShouldStartProcess_Wine()
    {
        var mockProcess = new ProcessMock();
        var service = new GameLauncher(mockProcess);

        var wine = new WineStartInfo
        {
            Path = "/bin/wine",
            Prefix = "/wineprefix",
            Arch = WineArch.Win32,
            Environment = ImmutableDictionary<string, string>.Empty
        };

        var expected = wine.Path;
        var actual = service.Start(XFS.Path(@"C:\osu.exe"), "localhost", wine).StartInfo.FileName;

        actual.Should().Be(expected);
    }

    [Fact]
    public void StartNative_ShouldStartProcess()
    {
        var mockProcess = new ProcessMock();
        var service = new GameLauncher(mockProcess);

        var path = XFS.Path(@"C:\osu.exe");
        var address = "localhost";

        var expected = new ProcessStartInfo { FileName = path, Arguments = $"-devserver {address}" };
        var actual = service.StartNative(path, address)?.StartInfo;

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
        var service = new GameLauncher(mockProcess);

        var envVars = environment?
                      .Split(" ")
                      .Select(x => x.Split("="))
                      .ToDictionary(x => x[0],
                                    x => x[1]);

        var path = XFS.Path(@"C:\osu.exe");
        var address = "localhost";
        var wine = new WineStartInfo
        {
            Path = "/bin/wine", Prefix = "/wineprefix", Arch = WineArch.Win32, Environment = envVars
        };

        var expected = new ProcessStartInfo
        {
            FileName = wine.Path,
            Arguments = $"{path} -devserver {address}",
            Environment =
            {
                { "WINEPREFIX", $"{wine.Prefix}" },
                { "WINEARCH", wine.Arch.ToString().ToLower() }
            }
        };

        if (envVars != null)
        {
            foreach (var envVar in envVars)
            {
                expected.EnvironmentVariables[envVar.Key] = envVar.Value;
            }
        }

        var actual = service.StartWine(path, "localhost", wine)!.StartInfo;

        actual.Should().BeEquivalentTo(expected,
                                       x => x
                                            .Including(p => p.FileName)
                                            .Including(p => p.Arguments)
                                            .Including(p => p.Environment));
    }
}
