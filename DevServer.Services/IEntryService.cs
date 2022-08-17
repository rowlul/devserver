using DevServer.Models;

namespace DevServer.Services;

public interface IEntryService
{
    IAsyncEnumerable<Entry> GetEntries();
    Task AddEntry(Entry entry);
    Task EditEntry(Entry entry);
    Task DeleteEntry(string filePath);
    Task<Stream?> GetLogoStream(string? source);
}
