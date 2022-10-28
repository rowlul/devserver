using System.Diagnostics;
using System.Reflection;

using CommunityToolkit.Mvvm.Input;

using DevServer.Services;

namespace DevServer.ViewModels;

public partial class AboutViewModel : DialogViewModelBase
{
    private readonly IProcess _process;

    public string ApplicationName => Assembly.GetEntryAssembly()?.GetName().Name ?? "devserver";
    public string ApplicationVersion => (Assembly.GetEntryAssembly()?.GetName().Version ?? new Version()).ToString();

    public AboutViewModel(IProcess process)
    {
        _process = process;
    }

    [RelayCommand]
    private void OpenRepo()
    {
        _process.Start(new ProcessStartInfo("https://github.com/rowlul/devserver") { UseShellExecute = true });
    }
}
