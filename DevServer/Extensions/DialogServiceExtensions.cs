using System.Threading.Tasks;

using CommunityToolkit.Mvvm.DependencyInjection;

using DevServer.ViewModels;
using DevServer.ViewModels.Dialogs;

using HanumanInstitute.MvvmDialogs;

namespace DevServer.Extensions;

public static class DialogServiceExtensions
{
    public static Task ShowDirectConnectDialog(this IDialogService service)
    {
        var owner = Ioc.Default.GetRequiredService<MainWindowViewModel>();
        var modal = Ioc.Default.GetRequiredService<DirectConnectDialogViewModel>();
        return service.ShowDialogAsync(owner, modal);
    }
}
