using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using FluentAssertions;

using Xunit;

namespace DevServer.Services.Tests;

public class WineRunnerTests
{
    private readonly WineRunner _runner;
    
    public WineRunnerTests()
    {
        var processMock = new ProcessMock();
        _runner = new WineRunner(processMock);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("STAGING_AUDIO_PERIOD=10000")]
    [InlineData("STAGING_AUDIO_PERIOD=10000 WINEFSYNC=1")]
    public void RunWithArgs_ShouldStartProcessWithArgs(string? environment)
    {
        var envVars = environment?
            .Split(" ")
            .Select(x => x.Split("="))
            .ToDictionary(x => x[0],
                x => x[1]);
        
        var expected = new ProcessStartInfo
        {
            FileName = "/usr/bin/wine", 
            Arguments = "~/.wineprefix/osu!.exe -devserver localhost",
            Environment =
            {
                { "WINEPREFIX", "~/.wineprefix" },
                { "WINEARCH", "win32" }
            }
        };
        
        if (envVars != null)
        {
            foreach (var envVar in envVars)
            {
                expected.EnvironmentVariables[envVar.Key] = envVar.Value;
            }
        }

        var actual = _runner.RunWithArgs(
            "~/.wineprefix/osu!.exe", 
            "localhost",
            new WineStartInfo
            {
                Path = "/usr/bin/wine",
                Prefix = "~/.wineprefix",
                Arch = WineArch.Win32,
                Environment = envVars
            }
        )!.StartInfo;

        actual.Should().BeEquivalentTo(expected, x => x
            .Including(x => x.FileName)
            .Including(x => x.Arguments)
            .Including(x => x.Environment));
    }
}
