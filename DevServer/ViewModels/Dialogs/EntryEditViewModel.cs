using System.Collections.Generic;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;

using DevServer.Extensions;
using DevServer.Models;

using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs;

using Material.Icons;

namespace DevServer.ViewModels.Dialogs;

public partial class EntryEditViewModel : DialogViewModelBase
{
    public string HeaderText
    {
        get
        {
            return Entry == null ? "Create new entry" : "Edit existing entry";
        }
    }

    public Entry? Entry { get; private set; }

    private readonly IDialogService _dialogService;

    [ObservableProperty]
    private string? _name;

    [ObservableProperty]
    private string? _description;

    [ObservableProperty]
    private string? _logo;

    [ObservableProperty]
    private string? _serverAddress;

    public EntryEditViewModel(Entry? entry = null)
    {
        Entry = entry;

        _dialogService = Ioc.Default.GetRequiredService<IDialogService>();

        _name = Entry?.Name;
        _description = Entry?.Description;
        _logo = Entry?.Logo;
        _serverAddress = Entry?.ServerAddress;
    }

    [RelayCommand]
    private async Task Done()
    {
        if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(ServerAddress))
        {
            await _dialogService.ShowMessageBox("Warning",
                                                "Name or Server Address cannot be empty.",
                                                MaterialIconKind.Warning);
            return;
        }

        Entry = new Entry
        {
            Name = Name,
            Description = Description,
            Logo = Logo,
            ServerAddress = ServerAddress
        };

        DialogResult = true;
        base.Close();
    }

    [RelayCommand]
    private async Task OpenLogo()
    {
        var filters = new List<FileFilter>
        {
            new("All pictures", new[] { ".jpg", ".jpeg", ".jfif", ".png", ".bmp", ".gif", ".tif", ".tiff", ".ico" }),
            new("All files", ".*")
        };

        var result = await _dialogService.ShowOpenFileDialog(new OpenFileDialogSettings { Filters = filters });
        if (result is not null)
        {
            Logo = result;
        }
    }

    protected override void Close()
    {
        DialogResult = false;
        base.Close();
    }
}
