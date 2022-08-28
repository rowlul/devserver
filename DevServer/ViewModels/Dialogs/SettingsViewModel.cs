using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

using Avalonia.Controls;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using DevServer.Models;
using DevServer.Services;

namespace DevServer.ViewModels.Dialogs;

public partial class SettingsViewModel : DialogViewModelBase
{
    private readonly IConfigurationManager _configurationManager;

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

    [ObservableProperty]
    private string _osuExePath;

    [ObservableProperty]
    private string? _winePath;

    [ObservableProperty]
    private string? _winePrefixPath;

    [ObservableProperty]
    private string? _environment;

    [ObservableProperty]
    private WineArch? _selectedWineArch;

    public SettingsViewModel(IConfigurationManager configurationManager)
    {
        _configurationManager = configurationManager;

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

    private static IDictionary<string, string> SplitEnvironment(string? env) =>
        !string.IsNullOrWhiteSpace(env)
            ? env.Split('\n', ' ')
                 .Select(x => x.Split('='))
                 .Where(x => x.Length == 2)
                 .ToDictionary(x => x[0], x => x[1])
            : ImmutableDictionary<string, string>.Empty;

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
    private async Task Reload()
    {
        await _configurationManager.LoadAsync();
        Load();
    }

    [RelayCommand]
    private void Reset()
    {
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
}
