using System.Collections.Immutable;

using Avalonia.Controls;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using DevServer.Services;
using DevServer.ViewModels.Extensions;

using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs;

namespace DevServer.ViewModels;

public partial class SettingsViewModel : ObservableDialog
{
    private readonly IConfigurationManager _configurationManager;
    private readonly IDialogService _dialogService;

    [ObservableProperty]
    private string? _environment;

    [ObservableProperty]
    private string _osuExePath;

    [ObservableProperty]
    private WineArch? _selectedWineArch;

    [ObservableProperty]
    private string? _winePath;

    [ObservableProperty]
    private string? _winePrefixPath;

    public bool IsWineSettingsAvailable
    {
        get
        {
            if (Design.IsDesignMode)
            {
                return true;
            }

            return _configurationManager.Settings.WineSettings is not null;
        }
    }

    public static List<WineArch> WineArches => new() { WineArch.Win32, WineArch.Win64 };

    public SettingsViewModel(IConfigurationManager configurationManager,
                             IDialogService dialogService)
    {
        _configurationManager = configurationManager;
        _dialogService = dialogService;

        Load();
    }

    private static string StringifyEnvironment(IDictionary<string, string>? dictionary)
    {
        return dictionary is not null
            ? string.Join("\n",
                          dictionary.Select(
                              kvp => kvp.Key + "=" + kvp.Value))
            : string.Empty;
    }

    private static IDictionary<string, string> SplitEnvironment(string? env)
    {
        return !string.IsNullOrWhiteSpace(env)
            ? env.Split('\n', ' ')
                 .Select(x => x.Split('=', 2))
                 .Where(x => x.Length == 2)
                 .ToDictionary(x => x[0], x => x[1])
            : ImmutableDictionary<string, string>.Empty;
    }

    [RelayCommand]
    private void Load()
    {
        OsuExePath = _configurationManager.Settings.OsuExePath;

        if (_configurationManager.Settings.WineSettings is not null)
        {
            WinePath = _configurationManager.Settings.WineSettings.Path;
            WinePrefixPath = _configurationManager.Settings.WineSettings.Prefix;
            Environment = StringifyEnvironment(_configurationManager.Settings.WineSettings.Environment);
            SelectedWineArch = _configurationManager.Settings.WineSettings.Arch;
        }
    }

    [RelayCommand]
    private async Task Reset()
    {
        var result = await _dialogService.ShowMessageBox(
            this,
            "Are you sure to reset ALL settings?",
            "This action is irreversible.",
            MessageBoxIcon.Question,
            MessageBoxButtons.OkCancel);

        if (result is false)
        {
            return;
        }

        var settings = new Settings();
        OsuExePath = settings.OsuExePath;

        if (settings.WineSettings is not null)
        {
            WinePath = settings.WineSettings.Path;
            Environment = StringifyEnvironment(settings.WineSettings.Environment);
            SelectedWineArch = settings.WineSettings.Arch;
        }
    }

    [RelayCommand]
    private async Task Apply()
    {
        _configurationManager.Settings.OsuExePath = OsuExePath;

        if (_configurationManager.Settings.WineSettings is not null)
        {
            _configurationManager.Settings.WineSettings.Path = WinePath!;
            _configurationManager.Settings.WineSettings.Prefix = WinePrefixPath!;
            _configurationManager.Settings.WineSettings.Arch = SelectedWineArch!.Value;
            _configurationManager.Settings.WineSettings.Environment = SplitEnvironment(Environment);

            // update text with formatted string
            Environment = StringifyEnvironment(_configurationManager.Settings.WineSettings.Environment);
        }

        await _configurationManager.SaveAsync();
    }

    [RelayCommand]
    private async Task OpenOsuExePath()
    {
        var result = await _dialogService.ShowOpenFileDialog(
            new OpenFileDialogSettings { Filters = new List<FileFilter> { new("osu! executable", ".exe") } });

        if (result is not null)
        {
            OsuExePath = result;
        }
    }

    [RelayCommand]
    private async Task OpenWinePath()
    {
        var result = await _dialogService.ShowOpenDirectoryDialog();

        if (result is not null)
        {
            WinePath = result;
        }
    }

    [RelayCommand]
    private async Task OpenWinePrefix()
    {
        var result = await _dialogService.ShowOpenDirectoryDialog();

        if (result is not null)
        {
            WinePrefixPath = result;
        }
    }
}
