using System;
using System.Threading.Tasks;

using Avalonia.Media.Imaging;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using DevServer.Models;
using DevServer.Services;

using Microsoft.Extensions.DependencyInjection;

namespace DevServer.ViewModels;

public partial class EntryViewModel : ViewModelBase
{
    private readonly IConfigurationManager _configurationManager;
    private readonly Entry _entry;
    private readonly INativeRunner _nativeRunner;
    private readonly IWineRunner _wineRunner;

    [ObservableProperty] private Bitmap? _logo;

    public string FilePath => _entry.FilePath;
    public string Name => _entry.Name;
    public string? Description => _entry.Description;

    public EntryViewModel(Entry entry)
    {
        _entry = entry;

        _configurationManager = Services.GetRequiredService<IConfigurationManager>();
        _nativeRunner = Services.GetRequiredService<INativeRunner>();
        _wineRunner = Services.GetRequiredService<IWineRunner>();

        LoadLogoCommand.ExecuteAsync(null);
    }

    [RelayCommand]
    private async Task Play()
    {
        using var process = OperatingSystem.IsLinux()
            ? _wineRunner.RunWithArgs(_configurationManager.Settings.OsuExePath,
                                      _entry.ServerAddress,
                                      _configurationManager.Settings.WineSettings!)
            : _nativeRunner.RunWithArgs(_configurationManager.Settings.OsuExePath, _entry.ServerAddress);

        // TODO: use a proper logger
        process.ErrorDataReceived += (_, args) => Console.WriteLine(args.Data);
        process.OutputDataReceived += (_, args) => Console.WriteLine(args.Data);

        await process.WaitForExitAsync();
    }

    [RelayCommand]
    private async Task LoadLogo()
    {
        var entryService = Services.GetRequiredService<IEntryService>();
        var stream = await entryService.GetLogoStream(_entry.Logo);
        Logo = await Task.Run(() => Bitmap.DecodeToWidth(stream, 42));
    }
}
