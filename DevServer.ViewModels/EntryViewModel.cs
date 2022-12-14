using Avalonia.Media.Imaging;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using DevServer.Services;
using DevServer.ViewModels.Extensions;
using DevServer.ViewModels.Messages;

using HanumanInstitute.MvvmDialogs;

using Microsoft.Extensions.Logging;

namespace DevServer.ViewModels;

public partial class EntryViewModel : ObservableRecipient
{
    private readonly ILogger<EntryViewModel> _logger;
    private readonly IEntryService _entryService;
    private readonly ILogoService _logoService;
    private readonly IGameLauncher _gameLauncher;
    private readonly IConfigurationManager _config;
    private readonly IDialogService _dialogService;

    private Entry _entry;

    [ObservableProperty]
    private Bitmap? _logo;

    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private string? _description;

    public EntryViewModel(Entry entry)
    {
        _entry = entry;

        _logger = Ioc.Default.GetRequiredService<ILogger<EntryViewModel>>();
        _entryService = Ioc.Default.GetRequiredService<IEntryService>();
        _logoService = Ioc.Default.GetRequiredService<ILogoService>();
        _gameLauncher = Ioc.Default.GetRequiredService<IGameLauncher>();
        _config = Ioc.Default.GetRequiredService<IConfigurationManager>();
        _dialogService = Ioc.Default.GetRequiredService<IDialogService>();

        _name = _entry.Name;
        _description = _entry.Description;
    }

    [RelayCommand]
    private async Task LoadLogo()
    {
        Logo = await Task.Run(async () =>
        {
            try
            {
                return Bitmap.DecodeToWidth(
                    await _logoService.GetLogoStream(source: _entry.Logo,
                        cacheFileName: _entry.FilePath),
                    42);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Could not load logo for entry {}", _entry.FilePath);
            }

            return null;
        });
    }

    [RelayCommand]
    private async Task Play()
    {
        try
        {
            using var process = _gameLauncher.Start(_config.Settings.OsuExePath,
                _entry.ServerAddress,
                _config.Settings.WineSettings);

            Messenger.Send(new ProcessRunningMessage(true));

            process.ErrorDataReceived += (_, args) => _logger.LogError(args.Data);
            process.OutputDataReceived += (_, args) => _logger.LogTrace(args.Data);
            process.Exited += (_, _) =>
            {
                Messenger.Send(new ProcessRunningMessage(false));
                _logger.LogInformation("Process exited with exit code {}", process.ExitCode);
            };

            await process.WaitForExitAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Could not start game");
            await _dialogService.ShowLogBox(Ioc.Default.GetRequiredService<MainWindowViewModel>(),
                $"Could not start game",
                e.Message, showReport: true, MessageBoxIcon.Error);
        }
    }

    [RelayCommand]
    private async Task EditEntry()
    {
        var entry = await _dialogService.ShowEntryEditViewModel(_entry);
        if (entry is not null)
        {
            await _entryService.UpsertEntry(entry);
            _entry = entry;

            Name = entry.Name;
            Description = entry.Description;
            await LoadLogo();
        }
    }

    [RelayCommand]
    private async Task DeleteEntry()
    {
        var result = await _dialogService.ShowMessageBox(
            Ioc.Default.GetRequiredService<MainWindowViewModel>(),
            $"Are you sure to delete {_name}?",
            "This action is irreversible.",
            MessageBoxIcon.Question,
            MessageBoxButtons.OkCancel);

        if (result is false)
        {
            return;
        }

        await _entryService.DeleteEntry(_entry.FilePath);
        Messenger.Send(new EntryDeletedMessage(this));
    }
}
