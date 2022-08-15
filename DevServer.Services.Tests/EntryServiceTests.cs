using System;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Text.Json;
using System.Threading.Tasks;

using DevServer.Models;

using FluentAssertions;

using RichardSzalay.MockHttp;

using SkiaSharp;

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

        var mockFileSystem = new MockFileSystem();
        mockFileSystem.AddFile("server.json", new MockFileData(json));

        var mockHttp = new MockHttpMessageHandler();
        var httpHandler = new HttpClientHandler(mockHttp.ToHttpClient());

        var service = new EntryService(mockPlatform, mockFileSystem, httpHandler);

        var expected = JsonSerializer.Deserialize<Entry>(json);

        var entries = service.GetEntries().GetAsyncEnumerator();
        await entries.MoveNextAsync();
        var actual = entries.Current;

        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetEntries_ShouldReturnEntry_AllFields()
    {
        var server = new
        {
            Name = "server", Description = "server description", Logo = "logo.png", ServerAddress = "localhost"
        };

        var json = JsonSerializer.Serialize(server);

        const int imageByteSize = 100 * 100 * 3;

        // Generate a completely random image
        var rnd = new Random();
        var imageBytes = new byte[imageByteSize];
        rnd.NextBytes(imageBytes);

        using var memoryStream = new MemoryStream(imageBytes);
        var image = SKImage.FromEncodedData(memoryStream);

        var mockPlatform = new PlatformMock();

        var mockFileSystem = new MockFileSystem();
        mockFileSystem.AddFile("server.json", new MockFileData(json));
        mockFileSystem.AddFile("logo.png", new MockFileData(imageBytes));

        var mockHttp = new MockHttpMessageHandler();
        var httpHandler = new HttpClientHandler(mockHttp.ToHttpClient());

        var service = new EntryService(mockPlatform, mockFileSystem, httpHandler);

        var expected = new Entry(server.Name, server.Description, image, server.ServerAddress);

        var entries = service.GetEntries().GetAsyncEnumerator();
        await entries.MoveNextAsync();
        var actual = entries.Current;

        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetEntries_ShouldFail_MissingNecessaryFields()
    {
        var json = JsonSerializer.Serialize(new { Description = "server description" });

        var mockPlatform = new PlatformMock();

        var mockFileSystem = new MockFileSystem();
        mockFileSystem.AddFile("server.json", new MockFileData(json));

        var mockHttp = new MockHttpMessageHandler();
        var httpHandler = new HttpClientHandler(mockHttp.ToHttpClient());

        var service = new EntryService(mockPlatform, mockFileSystem, httpHandler);

        var entries = service.GetEntries().GetAsyncEnumerator();

        Func<Task> act = async () => await entries.MoveNextAsync();
        await act.Should()
                 .ThrowAsync<InvalidOperationException>()
                 .WithMessage("Property doesn't exist or is null");
    }

    [Theory]
    [InlineData("https://localhost/image.png")]
    [InlineData("http://localhost/image.png")]
    [InlineData(@"C:\image.png")]
    public async Task GetImage_ShouldNotReturnNull(string source)
    {
        const int imageByteSize = 100 * 100 * 3;

        var rnd = new Random();
        var imageBytes = new byte[imageByteSize];
        rnd.NextBytes(imageBytes);

        var mockPlatform = new PlatformMock();

        var mockFileSystem = new MockFileSystem();
        mockFileSystem.AddFile(XFS.Path(source), new MockFileData(imageBytes));

        using var memoryStream = new MemoryStream(imageBytes);
        var mockHttp = new MockHttpMessageHandler();
        mockHttp.When(source).Respond("image/png", memoryStream);

        var httpHandler = new HttpClientHandler(mockHttp.ToHttpClient());
        var service = new EntryService(mockPlatform, mockFileSystem, httpHandler);

        var image = await service.GetImage(source);
        image.Should().NotBeNull();
    }

    [Fact]
    public async Task GetImage_ShouldReturnNull()
    {
        var mockPlatform = new PlatformMock();
        var mockFileSystem = new MockFileSystem();
        var mockHttp = new MockHttpMessageHandler();
        var httpHandler = new HttpClientHandler(mockHttp.ToHttpClient());
        var service = new EntryService(mockPlatform, mockFileSystem, httpHandler);

        var image = await service.GetImage(null);
        image.Should().BeNull();
    }

    [Fact]
    public async Task GetImageFromUrl_ShouldReturnImage()
    {
        var mockFileSystem = new MockFileSystem();

        const string url = "https://localhost/image.png";
        const int imageByteSize = 100 * 100 * 3;

        var rnd = new Random();
        var imageBytes = new byte[imageByteSize];
        rnd.NextBytes(imageBytes);

        var mockPlatform = new PlatformMock();

        using var memoryStream = new MemoryStream(imageBytes);
        var mockHttp = new MockHttpMessageHandler();
        mockHttp.When(url).Respond("image/png", memoryStream);

        var httpHandler = new HttpClientHandler(mockHttp.ToHttpClient());

        var service = new EntryService(mockPlatform, mockFileSystem, httpHandler);

        var expected = SKImage.FromEncodedData(memoryStream);
        var actual = await service.GetImageFromUrl(url);

        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetImageFromLocalFile_ShouldReturnImage()
    {
        const string path = @"C:\image.png";
        const int imageByteSize = 100 * 100 * 3;

        var rnd = new Random();
        var imageBytes = new byte[imageByteSize];
        rnd.NextBytes(imageBytes);

        var mockPlatform = new PlatformMock();

        var mockFileSystem = new MockFileSystem();
        mockFileSystem.AddFile(XFS.Path(path), new MockFileData(imageBytes));

        using var memoryStream = new MemoryStream(imageBytes);

        var mockHttp = new MockHttpMessageHandler();
        var httpHandler = new HttpClientHandler(mockHttp.ToHttpClient());
        var service = new EntryService(mockPlatform, mockFileSystem, httpHandler);

        var expected = SKImage.FromEncodedData(memoryStream);
        var actual = await service.GetImageFromLocalFile(XFS.Path(path));

        actual.Should().BeEquivalentTo(expected);
    }
}
