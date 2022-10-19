namespace DevServer.Services;

public interface IEntryService
{
    string EntryStorePath { get; }
    IAsyncEnumerable<Entry> GetEntries();
    Task UpsertEntry(Entry entry);
    Task DeleteEntry(string filePath);
}
