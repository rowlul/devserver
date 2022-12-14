using System;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Text.Json;
using System.Threading.Tasks;

using FluentAssertions;

using Xunit;

using XFS = System.IO.Abstractions.TestingHelpers.MockUnixSupport;

namespace DevServer.Services.Tests;

public class EntryServiceTests
{
    [Fact]
    public async Task GetEntries_ShouldReturnEntry_NecessaryFields()
    {
        var json = JsonSerializer.Serialize(new { Name = "server", ServerAddress = "localhost" });

        var mockPlatform = new PlatformMock();

        var mockFileSystem = new MockFileSystem(files: new Dictionary<string, MockFileData> { { "server.json", json } },
                                                currentDirectory: mockPlatform.Path + "servers");

        var service = new EntryService(mockPlatform, mockFileSystem);

        var expected = JsonSerializer.Deserialize<Entry>(json);

        var entries = service.GetEntries().GetAsyncEnumerator();
        await entries.MoveNextAsync();
        var actual = entries.Current;

        actual.Should().BeEquivalentTo(expected, x => x.Excluding(e => e!.FilePath));
    }

    [Fact]
    public async Task GetEntries_ShouldReturnEntry_AllFields()
    {
        var server = new
        {
            Name = "server", Description = "server description", Logo = "logo.png", ServerAddress = "localhost"
        };

        var json = JsonSerializer.Serialize(server);

        var mockPlatform = new PlatformMock();

        var mockFileSystem = new MockFileSystem(files: new Dictionary<string, MockFileData> { { "server.json", json } },
                                                currentDirectory: mockPlatform.Path + "servers");

        var service = new EntryService(mockPlatform, mockFileSystem);

        var expected = new Entry
        {
            FilePath = XFS.Path(@"C:\servers\server.json"),
            Name = server.Name,
            Description = server.Description,
            Logo = server.Logo,
            ServerAddress = server.ServerAddress
        };

        var entries = service.GetEntries().GetAsyncEnumerator();
        await entries.MoveNextAsync();
        var actual = entries.Current;

        actual.Should().BeEquivalentTo(expected);
    }

    [Fact(Skip = "exception not yet implemented")]
    public async Task GetEntries_ShouldFail_MissingNecessaryFields()
    {
        var json = JsonSerializer.Serialize(new { Description = "server description" });

        var mockPlatform = new PlatformMock();

        var mockFileSystem = new MockFileSystem(files: new Dictionary<string, MockFileData> { { "server.json", json } },
                                                currentDirectory: mockPlatform.Path + "servers");

        var service = new EntryService(mockPlatform, mockFileSystem);

        var entries = service.GetEntries().GetAsyncEnumerator();

        Func<Task> act = async () => await entries.MoveNextAsync();
        await act.Should()
                 .ThrowAsync<InvalidOperationException>()
                 .WithMessage("Property doesn't exist or is null");
    }
}
