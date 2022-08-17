using DevServer.Models;
using DevServer.ViewModels;

namespace DevServer;

public static class DesignData
{
    public static MainWindowViewModel MainWindowViewModel => new();

    public static EntryListViewModel EntryListViewModel => new(null!);

    public static ToolBarPanelViewModel ToolBarPanelViewModel { get; } = new(EntryListViewModel);

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
