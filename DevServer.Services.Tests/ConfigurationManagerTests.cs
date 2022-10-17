using System;
using System.Collections.Immutable;
using System.IO.Abstractions.TestingHelpers;
using System.Text.Json;
using System.Threading.Tasks;

using DevServer.Models;
using DevServer.Services.Helpers;

using FluentAssertions;

using Xunit;

using XFS = System.IO.Abstractions.TestingHelpers.MockUnixSupport;

namespace DevServer.Services.Tests;

public class ConfigurationManagerTests
{
    [Fact]
    public void Load_ShouldDeserializeToProperty()
    {
        var expected = new Settings
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

        var json = JsonSerializer.Serialize(expected);

        var mockPlatform = new PlatformMock();

        var mockFileSystem = new MockFileSystem();
        mockFileSystem.AddFile(mockPlatform.GetConfigFile(), new MockFileData(json));

        var service = new ConfigurationManager(mockPlatform, mockFileSystem);
        service.Load();

        var actual = service.Settings;

        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task LoadAsync_ShouldDeserializeToProperty()
    {
        var expected = new Settings
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

        var json = JsonSerializer.Serialize(expected);

        var mockPlatform = new PlatformMock();

        var mockFileSystem = new MockFileSystem();
        mockFileSystem.AddFile(mockPlatform.GetConfigFile(), new MockFileData(json));

        var service = new ConfigurationManager(mockPlatform, mockFileSystem);
        await service.LoadAsync();

        var actual = service.Settings;

        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void Save_ShouldWriteToFile()
    {
        var settings = new Settings
        {
            OsuExePath = XFS.Path(@"C:\osu.exe"),
            WineSettings = new WineStartInfo
            {
                Path = "/bin/wine",
                Prefix = "/wineprefix",
                Arch = WineArch.Win32,
                Environment = ImmutableDictionary<string, string>.Empty
            },
            LastServerAddress = "localhost"
        };

        var jsonSerializerOptions = new JsonSerializerOptions
        {
            WriteIndented = true, Converters = { new JsonEnumMemberStringEnumConverter() }
        };

        var expected = JsonSerializer.Serialize(settings, jsonSerializerOptions);
        expected += Environment.NewLine;

        var mockPlatform = new PlatformMock();
        var mockFileSystem = new MockFileSystem();
        var service = new ConfigurationManager(mockPlatform, mockFileSystem)
        {
            Settings =
            {
                OsuExePath = settings.OsuExePath,
                WineSettings = settings.WineSettings,
                LastServerAddress = settings.LastServerAddress
            }
        };

        service.Save();

        var actual = mockFileSystem.File.ReadAllText(mockPlatform.GetConfigFile());

        actual.Should().Be(expected);
    }

    [Fact]
    public async Task SaveAsync_ShouldWriteToFile()
    {
        var settings = new Settings
        {
            OsuExePath = XFS.Path(@"C:\osu.exe"),
            WineSettings = new WineStartInfo
            {
                Path = "/bin/wine",
                Prefix = "/wineprefix",
                Arch = WineArch.Win32,
                Environment = ImmutableDictionary<string, string>.Empty
            },
            LastServerAddress = "localhost"
        };

        var jsonSerializerOptions = new JsonSerializerOptions
        {
            WriteIndented = true, Converters = { new JsonEnumMemberStringEnumConverter() }
        };

        var expected = JsonSerializer.Serialize(settings, jsonSerializerOptions);
        expected += Environment.NewLine;

        var mockPlatform = new PlatformMock();
        var mockFileSystem = new MockFileSystem();
        var service = new ConfigurationManager(mockPlatform, mockFileSystem)
        {
            Settings =
            {
                OsuExePath = settings.OsuExePath,
                WineSettings = settings.WineSettings,
                LastServerAddress = settings.LastServerAddress
            }
        };

        await service.SaveAsync();

        var actual = await mockFileSystem.File.ReadAllTextAsync(mockPlatform.GetConfigFile());

        actual.Should().Be(expected);
    }
}
