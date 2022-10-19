using DevServer.Services;
using DevServer.ViewModels;

namespace DevServer.Avalonia.Views;

public static class DesignData
{
    public static MainWindowViewModel MainWindowViewModel => new(EntryListViewModel, MainPanelViewModel);

    public static EntryListViewModel EntryListViewModel => new();

    public static MainPanelViewModel MainPanelViewModel => new(null!, null!, null!);

    public static DirectConnectViewModel DirectConnectViewModel => new(null!, null!, null!, null!);

    public static AboutViewModel AboutViewModel => new(null!);

    public static SettingsViewModel SettingsViewModel => new(null!, null!);

    public static EntryEditViewModel EntryEditViewModel => new();

    public static MessageBoxViewModel MessageBoxViewModel =>
        new("Title", "Messagebox text", MessageBoxIcon.Warning, MessageBoxButtons.OkCancel);

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
