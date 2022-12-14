using System.ComponentModel;

using CommunityToolkit.Mvvm.DependencyInjection;

using DevServer.Services;

using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs;

namespace DevServer.ViewModels.Extensions;

public static class DialogServiceExtensions
{
    public static async Task<bool?> ShowMessageBox(this IDialogService service, INotifyPropertyChanged owner,
        string title, string text, MessageBoxIcon? icon = null, MessageBoxButtons? buttons = MessageBoxButtons.Ok)
    {
        var modal = new MessageBoxViewModel(title, text, icon, buttons);
        return await service.ShowDialogAsync(owner, modal);
    }

    public static async Task ShowLogBox(this IDialogService service, INotifyPropertyChanged owner,
        string title, string logText, bool showReport = false, MessageBoxIcon? icon = null)
    {
        var modal = new LogBoxViewModel(title, logText, showReport, icon);
        await service.ShowDialogAsync(owner, modal);
    }

    public static Task ShowDirectConnectDialog(this IDialogService service)
    {
        var owner = Ioc.Default.GetRequiredService<MainWindowViewModel>();
        var modal = Ioc.Default.GetRequiredService<DirectConnectViewModel>();
        return service.ShowDialogAsync(owner, modal);
    }

    public static Task ShowAboutDialog(this IDialogService service)
    {
        var owner = Ioc.Default.GetRequiredService<MainWindowViewModel>();
        var modal = Ioc.Default.GetRequiredService<AboutViewModel>();
        return service.ShowDialogAsync(owner, modal);
    }

    public static Task ShowSettingsDialog(this IDialogService service)
    {
        var owner = Ioc.Default.GetRequiredService<MainWindowViewModel>();
        var modal = Ioc.Default.GetRequiredService<SettingsViewModel>();
        return service.ShowDialogAsync(owner, modal);
    }

    public static async Task<Entry?> ShowEntryEditViewModel(this IDialogService service, Entry? entry = null)
    {
        var owner = Ioc.Default.GetRequiredService<MainWindowViewModel>();
        var modal = new EntryEditViewModel(entry);
        var result = await service.ShowDialogAsync(owner, modal);
        return result == true ? modal.Entry : null;
    }

    public static Task<string?> ShowOpenFileDialog(this IDialogService service, OpenFileDialogSettings? settings = null)
    {
        var owner = Ioc.Default.GetRequiredService<MainWindowViewModel>();
        return service.ShowOpenFileDialogAsync(owner, settings);
    }

    public static Task<string[]> ShowOpenFilesDialog(this IDialogService service,
        OpenFileDialogSettings? settings = null)
    {
        var owner = Ioc.Default.GetRequiredService<MainWindowViewModel>();
        return service.ShowOpenFilesDialogAsync(owner, settings);
    }

    public static Task<string?> ShowOpenDirectoryDialog(this IDialogService service,
        OpenFolderDialogSettings? settings = null)
    {
        var owner = Ioc.Default.GetRequiredService<MainWindowViewModel>();
        return service.ShowOpenFolderDialogAsync(owner, settings);
    }
}
