using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using DevServer.Messages;
using DevServer.Services;

using Microsoft.Extensions.Logging;

namespace DevServer.ViewModels.Dialogs;

public partial class DirectConnectDialogViewModel : DialogViewModelBase
{
    private readonly IGameLauncher _gameLauncher;
    private readonly ILogger<DirectConnectDialogViewModel> _logger;

    [ObservableProperty]
    private bool _isTextBoxEnabled = true;

    [ObservableProperty]
    private string _errorText;

    [ObservableProperty]
    private string _errorToolTip;

    [ObservableProperty]
    private bool _isAwaitingInput;

    [ObservableProperty]
    private bool _isInvalid;

    [ObservableProperty]
    private bool _isValid;

    [ObservableProperty]
    private string _serverAddress;

    public DirectConnectDialogViewModel(ILogger<DirectConnectDialogViewModel> logger,
                                        IGameLauncher gameLauncher)
    {
        _logger = logger;
        _gameLauncher = gameLauncher;
    }

    [RelayCommand]
    private async Task Play()
    {
        Messenger.Send(new ProcessRunningMessage(true));
        Close();

        using var process = _gameLauncher.Start(ServerAddress);

        process.ErrorDataReceived += (_, args) => _logger.LogError(args.Data);
        process.OutputDataReceived += (_, args) => _logger.LogTrace(args.Data);
        process.Exited += (_, _) =>
        {
            Messenger.Send(new ProcessRunningMessage(false));
            _logger.LogInformation("Process exited with exit code {}", process.ExitCode);
            DialogResult = true;
        };

        await process.WaitForExitAsync();
    }
}
