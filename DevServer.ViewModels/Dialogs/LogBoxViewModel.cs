using System.Diagnostics;

using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;

using DevServer.Services;

namespace DevServer.ViewModels;

public partial class LogBoxViewModel : DialogViewModelBase
{
    private readonly IProcess _process;

    public string Title { get; }
    public string LogText { get; }
    public bool ShowReport { get; }
    public MessageBoxIcon? Icon { get; }

    public LogBoxViewModel(string title, string logText, bool showReport = false, MessageBoxIcon? icon = null)
    {
        _process = Ioc.Default.GetRequiredService<IProcess>();

        Title = title;
        LogText = logText;
        ShowReport = showReport;
        Icon = icon;
    }

    [RelayCommand]
    private void Ok()
    {
        DialogResult = true;
        base.Close();
    }

    [RelayCommand]
    private void Report()
    {
        _process.Start(
            new ProcessStartInfo("https://github.com/rowlul/devserver/issues/new") { UseShellExecute = true });
    }
}
