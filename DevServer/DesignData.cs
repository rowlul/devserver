using DevServer.Models;
using DevServer.ViewModels;

using SkiaSharp;

using Splat;

namespace DevServer;

public static class DesignData
{
    public static MainWindowViewModel MainWindowViewModel => new();

    public static EntryListViewModel EntryListViewModel => new(null!);

    public static ToolBarPanelViewModel ToolBarPanelViewModel { get; } = new(null!);

    public static EntryViewModel EntryViewModel { get; } =
        new(new Entry("server", "server description", SKImage.Create(SKImageInfo.Empty), "localhost"));
}
