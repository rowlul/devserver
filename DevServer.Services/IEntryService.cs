using DevServer.Models;

namespace DevServer.Services;

public interface IEntryService
{
    IAsyncEnumerable<Entry> GetEntries();
    Task AddEntry(Entry entry);
    Task<Entry> EditEntry(Entry entry);
    Task<Entry> DeleteEntry(Entry entry);
}
