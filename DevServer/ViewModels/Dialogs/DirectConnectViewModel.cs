using System;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using DevServer.Extensions;
using DevServer.Messages;
using DevServer.Services;

using HanumanInstitute.MvvmDialogs;

using Microsoft.Extensions.Logging;

namespace DevServer.ViewModels.Dialogs;

public partial class DirectConnectViewModel : DialogViewModelBase
{
    private readonly ILogger<DirectConnectViewModel> _logger;
    private readonly IGameLauncher _gameLauncher;
    private readonly IDialogService _dialogService;
    private readonly IConfigurationManager _configurationManager;

    [ObservableProperty]
    private string _serverAddress;

    public DirectConnectViewModel(ILogger<DirectConnectViewModel> logger,
                                  IGameLauncher gameLauncher,
                                  IDialogService dialogService,
                                  IConfigurationManager configurationManager)
    {
        _logger = logger;
        _gameLauncher = gameLauncher;
        _dialogService = dialogService;
        _configurationManager = configurationManager;

        ServerAddress = _configurationManager.Settings.LastServerAddress ?? string.Empty;
    }

    [RelayCommand]
    private async Task Play()
    {
        if (string.IsNullOrEmpty(ServerAddress))
        {
            await _dialogService.ShowMessageBox("Warning",
                                                "Server address cannot be empty.",
                                                MessageBoxIcon.Warning);
            return;
        }

        _configurationManager.Settings.LastServerAddress = ServerAddress;

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
