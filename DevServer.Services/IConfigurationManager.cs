using DevServer.Models;

namespace DevServer.Services;

public interface IConfigurationManager
{
    Settings? Settings { get; }
    Task Load();
    Task Save();
}
