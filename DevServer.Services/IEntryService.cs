using DevServer.Models;

namespace DevServer.Services;

public interface IEntryService
{
    IAsyncEnumerable<Entry> GetEntries();
    Task AddEntry(Entry entry);
    Task EditEntry(Entry entry);
    Task DeleteEntry(Entry entry);
    Task<Stream?> GetLogoStream(string? source);
}
