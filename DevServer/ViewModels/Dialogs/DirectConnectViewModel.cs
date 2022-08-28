using System;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using DevServer.Messages;
using DevServer.Services;

using Microsoft.Extensions.Logging;

namespace DevServer.ViewModels.Dialogs;

public partial class DirectConnectViewModel : DialogViewModelBase
{
    private readonly IGameLauncher _gameLauncher;
    private readonly ILogger<DirectConnectViewModel> _logger;

    [ObservableProperty]
    private string _serverAddress;

    public DirectConnectViewModel(ILogger<DirectConnectViewModel> logger,
                                        IGameLauncher gameLauncher)
    {
        _logger = logger;
        _gameLauncher = gameLauncher;
    }

    [RelayCommand]
    private async Task Play()
    {
        try
        {
            using var process = _gameLauncher.Start(ServerAddress);

            Messenger.Send(new ProcessRunningMessage(true));
            Close();

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
        catch (Exception e)
        {
            _logger.LogError(e, "Could not start game");
        }
    }
}
