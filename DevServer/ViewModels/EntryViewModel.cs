using System;
using System.Diagnostics;
using System.Reactive;
using System.Threading.Tasks;

using Avalonia.Media.Imaging;

using DevServer.Models;
using DevServer.Services;

using ReactiveUI;

using Splat;

namespace DevServer.ViewModels;

public class EntryViewModel : ViewModelBase
{
    private readonly INativeRunner _nativeRunner;
    private readonly IWineRunner _wineRunner;
    private readonly IConfigurationManager _configurationManager;

    private readonly Entry _entry;

    private Bitmap? _logo;

    public string Name => _entry.Name;
    public string? Description => _entry.Description;

    public Bitmap? Logo
    {
        get => _logo;
        private set => this.RaiseAndSetIfChanged(ref _logo, value);
    }

    public ReactiveCommand<Unit, Unit> PlayCommand { get; }
    public ReactiveCommand<Unit, Unit> LoadLogo { get; }

    public EntryViewModel(Entry entry)
    {
        // using locator here instead of passing through constructor
        // because we always want to initialize this vm programmatically
        _nativeRunner = Locator.Current.GetService<INativeRunner>();
        _wineRunner = Locator.Current.GetService<IWineRunner>();
        _configurationManager = Locator.Current.GetService<IConfigurationManager>();

        _entry = entry;

        PlayCommand = ReactiveCommand.CreateFromTask(Play);

        LoadLogo = ReactiveCommand.CreateFromTask(async () =>
        {
            await using var stream = _entry.Logo?.EncodedData.AsStream();
            Logo = Bitmap.DecodeToWidth(stream, 42);
        });

        LoadLogo.Execute();
    }

    private async Task Play()
    {
        using var process = OperatingSystem.IsLinux()
            ? _wineRunner.RunWithArgs(_configurationManager.Settings.OsuExePath,
                                      _entry.ServerAddress,
                                      _configurationManager.Settings.WineSettings!)
            : _nativeRunner.RunWithArgs(_configurationManager.Settings.OsuExePath, _entry.ServerAddress);

        process.ErrorDataReceived += (_, args) => this.Log().Error(args.Data);
        process.OutputDataReceived += (_, args) => this.Log().Info(args.Data);

        await process.WaitForExitAsync();
    }
}
