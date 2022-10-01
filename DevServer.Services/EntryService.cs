using System.IO.Abstractions;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

using DevServer.Models;

namespace DevServer.Services;

public class EntryService : IEntryService
{
    private readonly IPlatformService _platformService;
    private readonly IFileSystem _fileSystem;
    private readonly IHttpHandler _httpHandler;

    private static JsonSerializerOptions JsonSerializerOptions { get; } = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNameCaseInsensitive = true,
        WriteIndented = true
    };

    public EntryService(IPlatformService platformService,
                        IFileSystem fileSystem,
                        IHttpHandler httpHandler)
    {
        _platformService = platformService;
        _fileSystem = fileSystem;
        _httpHandler = httpHandler;
    }

    public async IAsyncEnumerable<Entry> GetEntries()
    {
        var files = _fileSystem.Directory.EnumerateFiles(
            _platformService.GetEntryStorePath(),
            "*.json",
            SearchOption.TopDirectoryOnly);

        foreach (var file in files)
        {
            await using var fileStream = _fileSystem.File.OpenRead(file);

            var entry = (await JsonSerializer.DeserializeAsync<Entry>(fileStream, JsonSerializerOptions))!;
            entry.FilePath = file;

            yield return entry;
        }
    }

    public async Task UpsertEntry(Entry entry)
    {
        entry.FilePath = Path.Combine(_platformService.GetEntryStorePath(),
                                      Regex.Replace(entry.Name, @"[^0-9a-zA-Z_-]+", string.Empty).ToLower() + ".json");

        await using var fileStream = _fileSystem.FileStream.Create(entry.FilePath,
                                                                   FileMode.Create,
                                                                   FileAccess.ReadWrite,
                                                                   FileShare.ReadWrite,
                                                                   bufferSize: 4096,
                                                                   useAsync: true);

        await JsonSerializer.SerializeAsync(fileStream, entry, JsonSerializerOptions);
    }

    public async Task DeleteEntry(string filePath)
    {
        await Task.Run(() => _fileSystem.File.Delete(filePath));
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

            var logoPath = Path.Combine(_platformService.GetImageCachePath(),
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
