using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;

using DevServer.Extensions;
using DevServer.Models;

using HanumanInstitute.MvvmDialogs;

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
            var dialog = Ioc.Default.GetRequiredService<IDialogService>();
            await dialog.ShowMessageBox("Warning", "Name or Server Address cannot be empty", MaterialIconKind.Warning);
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

    protected override void Close()
    {
        DialogResult = false;
        base.Close();
    }
}
