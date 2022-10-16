using System;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Threading.Tasks;

using FluentAssertions;

using RichardSzalay.MockHttp;

using Xunit;

using XFS = System.IO.Abstractions.TestingHelpers.MockUnixSupport;

namespace DevServer.Services.Tests;

public class LogoServiceTests
{
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
        var service = new LogoService(mockPlatform, mockFileSystem, httpHandler);

        var image = await service.GetLogoStream(XFS.Path(source));
        image.Should().NotBeNull();
    }

    [Fact]
    public async Task GetImage_ShouldReturnNull()
    {
        var mockPlatform = new PlatformMock();
        var mockFileSystem = new MockFileSystem();
        var mockHttp = new MockHttpMessageHandler();
        var httpHandler = new HttpClientHandler(mockHttp.ToHttpClient());
        var service = new LogoService(mockPlatform, mockFileSystem, httpHandler);

        var image = await service.GetLogoStream(null);
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

        using var expected = new MemoryStream(imageBytes);
        var mockHttp = new MockHttpMessageHandler();
        mockHttp.When(url).Respond("image/png", expected);

        var httpHandler = new HttpClientHandler(mockHttp.ToHttpClient());

        var service = new LogoService(mockPlatform, mockFileSystem, httpHandler);

        await using var logoStream = await service.GetLogoStreamFromUrl(url);
        using var actual = new MemoryStream();
        await logoStream.CopyToAsync(actual);

        actual.ToArray().Should().BeEquivalentTo(expected.ToArray());
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

        var mockHttp = new MockHttpMessageHandler();
        var httpHandler = new HttpClientHandler(mockHttp.ToHttpClient());
        var service = new LogoService(mockPlatform, mockFileSystem, httpHandler);

        using var expected = new MemoryStream(imageBytes);

        await using var logoStream = await service.GetLogoStreamFromLocalFile(XFS.Path(path));
        using var actual = new MemoryStream();
        await logoStream.CopyToAsync(actual);

        actual.ToArray().Should().BeEquivalentTo(expected.ToArray());
    }
}
