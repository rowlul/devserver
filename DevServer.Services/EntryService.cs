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

    private static JsonSerializerOptions JsonSerializerOptions { get; } = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNameCaseInsensitive = true,
        WriteIndented = true
    };

    public string EntryStorePath => Path.Combine(_platformService.GetAppDataPath(), "servers");

    public EntryService(IPlatformService platformService,
                        IFileSystem fileSystem)
    {
        _platformService = platformService;
        _fileSystem = fileSystem;
    }

    public async IAsyncEnumerable<Entry> GetEntries()
    {
        var files = _fileSystem.Directory.EnumerateFiles(
            EntryStorePath,
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
        entry.FilePath = Path.Combine(EntryStorePath,
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
}
