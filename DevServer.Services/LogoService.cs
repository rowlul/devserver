using System.IO.Abstractions;

namespace DevServer.Services;

public class LogoService : ILogoService
{
    private readonly IPlatformService _platformService;
    private readonly IFileSystem _fileSystem;
    private readonly IHttpHandler _httpHandler;

    public string ImageCachePath => Path.Combine(_platformService.GetAppCachePath(), "images");

    public LogoService(IPlatformService platformService, IFileSystem fileSystem, IHttpHandler httpHandler)
    {
        _platformService = platformService;
        _fileSystem = fileSystem;
        _httpHandler = httpHandler;
    }

    public async Task<Stream?> GetLogoStream(string? source, string? cacheFileName = null)
    {
        if (source is null)
        {
            return null;
        }

        if (source[..4] == "http" || source[..5] == "https")
        {
            if (cacheFileName is null)
            {
                return await GetLogoStreamFromUrl(source);
            }

            var logoPath = Path.Combine(ImageCachePath,
                                        Path.GetFileNameWithoutExtension(cacheFileName) +
                                        Path.GetExtension(source));

            if (File.Exists(logoPath))
            {
                // return cached file if exists
                return await GetLogoStreamFromLocalFile(logoPath);
            }

            // create image in cache
            var logoStream = await GetLogoStreamFromUrl(source);
            await using var fileStream = _fileSystem.FileStream.Create(logoPath,
                                                                       FileMode.OpenOrCreate,
                                                                       FileAccess.ReadWrite,
                                                                       FileShare.ReadWrite,
                                                                       bufferSize: 4096,
                                                                       useAsync: true);
            await logoStream.CopyToAsync(fileStream);

            logoStream.Seek(0, SeekOrigin.Begin);
            return logoStream;
        }

        return await GetLogoStreamFromLocalFile(source);
    }

    public void PurgeCache()
    {
        foreach (var file in _fileSystem.DirectoryInfo.FromDirectoryName(ImageCachePath).EnumerateFiles())
        {
            file.Delete();
        }
    }

    internal async Task<Stream> GetLogoStreamFromUrl(string url)
    {
        var bytes = await _httpHandler.GetByteArrayAsync(url);
        return new MemoryStream(bytes);
    }

    internal Task<Stream> GetLogoStreamFromLocalFile(string path)
    {
        var stream = _fileSystem.FileStream.Create(path,
                                                   FileMode.Open,
                                                   FileAccess.Read,
                                                   FileShare.Read,
                                                   bufferSize: 4096,
                                                   useAsync: true);

        return Task.FromResult(stream);
    }
}
