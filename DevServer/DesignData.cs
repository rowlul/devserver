using DevServer.ViewModels;

using Splat;

namespace DevServer;

public static class DesignData
{
    public static MainWindowViewModel MainWindowViewModel { get; } = Locator.Current.GetService<MainWindowViewModel>();
    public static EntryListViewModel EntryListViewModel { get; } = Locator.Current.GetService<EntryListViewModel>();

    public static ToolBarPanelViewModel ToolBarPanelViewModel { get; } =
        Locator.Current.GetService<ToolBarPanelViewModel>();
}
