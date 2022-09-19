using System.Threading.Tasks;

using CommunityToolkit.Mvvm.DependencyInjection;

using DevServer.Models;
using DevServer.ViewModels;
using DevServer.ViewModels.Dialogs;

using HanumanInstitute.MvvmDialogs;

namespace DevServer.Extensions;

public static class DialogServiceExtensions
{
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
}
