using System.Drawing;
using System.IO.Abstractions;
using System.Text.Json;

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
            using var jsonDocument = await JsonDocument.ParseAsync(fileStream);

            JsonElement root = jsonDocument.RootElement;
            yield return new Entry(
                root.GetProperty("name").GetString() ?? throw new InvalidOperationException(""),
                root.GetProperty("description").GetString(),
                await GetBitmap(root.GetProperty("logo").GetString()),
                root.GetProperty("serveraddress").GetString() ?? throw new InvalidOperationException(""));
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

    internal static async Task<Bitmap> GetBitmapFromLocalFile(string path)
    {
        await using var stream = File.OpenRead(path);
        return new Bitmap(stream);
    }
}
