using DevServer.Models;

namespace DevServer.Services;

public interface IEntryService
{
    IAsyncEnumerable<Entry> GetEntries();
    Task UpsertEntry(Entry entry);
    Task DeleteEntry(string filePath);
    Task<Stream?> GetLogoStream(string? source, string? cacheFileName = null);
}
