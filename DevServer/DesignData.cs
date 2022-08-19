using DevServer.Models;
using DevServer.ViewModels;

namespace DevServer;

public static class DesignData
{
    public static MainWindowViewModel MainWindowViewModel => new(EntryListViewModel);

    public static EntryListViewModel EntryListViewModel => new(null!, null!);

    public static EntryViewModel EntryViewModel { get; } =
        new(new Entry
        {
            FilePath = null!,
            Name = "server",
            Description = "server description",
            Logo = "",
            ServerAddress = "localhost"
        });
}
