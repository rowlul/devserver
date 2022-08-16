using System;
using System.IO;

using Avalonia;
using Avalonia.Platform;

using DevServer.Models;
using DevServer.ViewModels;

using SkiaSharp;

using Splat;

namespace DevServer;

public static class DesignData
{
    public static MainWindowViewModel MainWindowViewModel { get; } = Locator.Current.GetService<MainWindowViewModel>();

    public static EntryListViewModel EntryListViewModel { get; } = Locator.Current.GetService<EntryListViewModel>();

    public static ToolBarPanelViewModel ToolBarPanelViewModel { get; } =
        Locator.Current.GetService<ToolBarPanelViewModel>();

    public static EntryViewModel EntryViewModel { get; } =
        new(new Entry("server", "server description", SKImage.Create(SKImageInfo.Empty), "localhost"));
}
