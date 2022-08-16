using DevServer.Models;

namespace DevServer.Services;

public interface IConfigurationManager
{
    Settings Settings { get; }
    void Load();
    Task LoadAsync();
    void Save();
    Task SaveAsync();
}
