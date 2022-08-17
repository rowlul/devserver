using System.IO.Abstractions;
using System.Text.Json;
using System.Text.Json.Serialization;

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

    public Task AddEntry(Entry entry)
    {
        throw new NotImplementedException();
    }

    public Task EditEntry(Entry entry)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteEntry(string filePath)
    {
        await Task.Run(() => _fileSystem.File.Delete(filePath));
    }

    public async Task<Stream?> GetLogoStream(string? source)
    {
        if (source is null)
        {
            return null;
        }

        if (source[..4] == "http" || source[..5] == "https")
        {
            return await GetLogoStreamFromUrl(source);
        }

        return await GetLogoStreamFromLocalFile(source);
    }

    internal async Task<Stream> GetLogoStreamFromUrl(string url)
    {
        var bytes = await _httpHandler.GetByteArrayAsync(url);
        return new MemoryStream(bytes);
    }

    internal async Task<Stream> GetLogoStreamFromLocalFile(string path)
    {
        var stream = await Task.Run(() => _fileSystem.File.OpenRead(path));
        return stream;
    }
}
