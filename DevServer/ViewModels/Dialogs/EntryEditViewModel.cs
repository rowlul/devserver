using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using DevServer.Models;

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
    private void Done()
    {
        if (_name is null || _serverAddress is null)
        {
            return;
        }

        Entry = new Entry
        {
            Name = _name,
            Description = _description,
            Logo = _logo,
            ServerAddress = _serverAddress
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
