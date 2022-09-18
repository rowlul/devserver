using System;
using System.Threading.Tasks;

using Avalonia.Media.Imaging;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using DevServer.Messages;
using DevServer.Models;
using DevServer.Services;

using Microsoft.Extensions.Logging;

namespace DevServer.ViewModels;

public partial class EntryViewModel : ViewModelBase
{
    private readonly Entry _entry;

    private readonly ILogger<EntryViewModel> _logger;
    private readonly IEntryService _entryService;
    private readonly IGameLauncher _gameLauncher;

    [ObservableProperty]
    private Bitmap? _logo;

    public string Name => _entry.Name;
    public string? Description => _entry.Description;

    public EntryViewModel(Entry entry)
    {
        _entry = entry;

        _logger = Ioc.Default.GetRequiredService<ILogger<EntryViewModel>>();
        _entryService = Ioc.Default.GetRequiredService<IEntryService>();
        _gameLauncher = Ioc.Default.GetRequiredService<IGameLauncher>();
    }

    [RelayCommand]
    private async Task LoadLogo()
    {
        Logo = await Task.Run(async () =>
        {
            try
            {
                return Bitmap.DecodeToWidth(await _entryService.GetLogoStream(_entry.Logo), 42);
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
            using var process = _gameLauncher.Start(_entry.ServerAddress);

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
        }
    }

    [RelayCommand]
    private Task EditEntry()
    {
        return Task.CompletedTask;
    }

    [RelayCommand]
    private async Task DeleteEntry()
    {
        await _entryService.DeleteEntry(_entry.FilePath);
        Messenger.Send(new EntryDeletedMessage(this));
    }
}
