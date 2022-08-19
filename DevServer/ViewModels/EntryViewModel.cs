using System;
using System.Threading.Tasks;

using Avalonia.Media.Imaging;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using DevServer.Messages;
using DevServer.Models;
using DevServer.Services;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DevServer.ViewModels;

public partial class EntryViewModel : RecipientViewModelBase
{
    private readonly Entry _entry;

    private readonly ILogger<EntryViewModel> _logger;
    private readonly IConfigurationManager _configurationManager;
    private readonly INativeRunner _nativeRunner;
    private readonly IWineRunner _wineRunner;

    [ObservableProperty]
    private Bitmap? _logo;

    public string FilePath => _entry.FilePath;
    public string Name => _entry.Name;
    public string? Description => _entry.Description;

    public EntryViewModel(Entry entry)
    {
        _entry = entry;

        _logger = Services.GetRequiredService<ILogger<EntryViewModel>>();
        _configurationManager = Services.GetRequiredService<IConfigurationManager>();
        _nativeRunner = Services.GetRequiredService<INativeRunner>();
        _wineRunner = Services.GetRequiredService<IWineRunner>();
    }

    [RelayCommand]
    private async Task Play()
    {
        Messenger.Send(new ProcessRunningMessage(true));

        using var process = OperatingSystem.IsLinux()
            ? _wineRunner.RunWithArgs(_configurationManager.Settings.OsuExePath,
                                      _entry.ServerAddress,
                                      _configurationManager.Settings.WineSettings!)
            : _nativeRunner.RunWithArgs(_configurationManager.Settings.OsuExePath, _entry.ServerAddress);

        process.ErrorDataReceived += (_, args) => _logger.LogError(args.Data);
        process.OutputDataReceived += (_, args) => _logger.LogTrace(args.Data);
        process.Exited += (_, _) =>
        {
            Messenger.Send(new ProcessRunningMessage(false));
            _logger.LogInformation("Process exited with exit code {}", process.ExitCode);
        };

        await process.WaitForExitAsync();
    }

    [RelayCommand]
    private async Task LoadLogo()
    {
        var entryService = Services.GetRequiredService<IEntryService>();
        Logo = await Task.Run(async () =>
        {
            try
            {
                return Bitmap.DecodeToWidth(await entryService.GetLogoStream(_entry.Logo), 42);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Could not load logo for entry {}", FilePath);
            }

            return null;
        });
    }
}
