using System.Drawing;
using System.IO.Abstractions;
using System.Text.Json;
using System.Text.Json.Nodes;

using DevServer.Models;

namespace DevServer.Services;

public class EntryService : IEntryService
{
    private readonly string _entryStorePath;
    private readonly IFileSystem _fileSystem;
    private readonly IHttpHandler _httpHandler;

    public EntryService(string entryStorePath,
                        IFileSystem fileSystem,
                        IHttpHandler httpHandler)
    {
        _entryStorePath = entryStorePath;
        _fileSystem = fileSystem;
        _httpHandler = httpHandler;
    }

    public async IAsyncEnumerable<Entry> GetEntries()
    {
        var files = _fileSystem.Directory.EnumerateFiles(_entryStorePath,
                                                         "*.json",
                                                         SearchOption.TopDirectoryOnly);

        foreach (var file in files)
        {
            await using var fileStream = _fileSystem.File.OpenRead(file);

            var node = JsonNode.Parse(fileStream, new JsonNodeOptions { PropertyNameCaseInsensitive = true })!;
            yield return new Entry(
                node["name"]?.ToString() ?? throw new InvalidOperationException("Property doesn't exist or is null"),
                node["description"]?.ToString(),
                await GetBitmap(node["logo"]?.ToString()),
                node["serveraddress"]?.ToString() ??
                throw new InvalidOperationException("Property doesn't exist or is null"));
        }
    }

    public Task AddEntry(Entry entry)
    {
        throw new NotImplementedException();
    }

    public Task<Entry> EditEntry(Entry entry)
    {
        throw new NotImplementedException();
    }

    public Task<Entry> DeleteEntry(Entry entry)
    {
        throw new NotImplementedException();
    }

    internal async Task<Bitmap?> GetBitmap(string? source)
    {
        if (source is null)
        {
            return null;
        }

        if (source[..4] == "http" || source[..5] == "https")
        {
            return await GetBitmapFromUrl(source);
        }

        return await GetBitmapFromLocalFile(source);
    }

    internal async Task<Bitmap> GetBitmapFromUrl(string url)
    {
        await using var stream = await _httpHandler.GetStreamAsync(url);
        return new Bitmap(stream);
    }

    internal async Task<Bitmap> GetBitmapFromLocalFile(string path)
    {
        await using var stream = _fileSystem.File.OpenRead(path);
        return new Bitmap(stream);
    }
}
