using System.IO.Abstractions;
using System.Text.Json.Nodes;

using DevServer.Models;

namespace DevServer.Services;

public class EntryService : IEntryService
{
    private readonly IPlatformService _platformService;
    private readonly IFileSystem _fileSystem;
    private readonly IHttpHandler _httpHandler;

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

            var node = JsonNode.Parse(fileStream, new JsonNodeOptions { PropertyNameCaseInsensitive = true })!;
            yield return new Entry(
                file,
                node["name"]?.ToString() ?? throw new InvalidOperationException("Property doesn't exist or is null"),
                node["description"]?.ToString(),
                node["logo"]?.ToString(),
                node["serveraddress"]?.ToString() ??
                throw new InvalidOperationException("Property doesn't exist or is null"));
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

    public async Task DeleteEntry(Entry entry)
    {
        await Task.Run(() => _fileSystem.File.Delete(entry.FilePath));
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
        var stream = await _httpHandler.GetStreamAsync(url);
        return stream;
    }

    internal async Task<Stream> GetLogoStreamFromLocalFile(string path)
    {
        await using var fileStream = _fileSystem.File.OpenRead(path);
        return fileStream;
    }
}
