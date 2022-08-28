using System.Threading.Tasks;

using Avalonia.Collections;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using DevServer.Messages;

namespace DevServer.ViewModels;

public partial class EntryListViewModel : ViewModelBase
{
    [ObservableProperty]
    private AvaloniaList<EntryViewModel> _entries = new();

    [ObservableProperty]
    private bool _isEnabled = true;

    [ObservableProperty]
    private EntryViewModel? _selectedEntry;

    public EntryListViewModel()
    {
        IsActive = true;
    }

    protected override void OnActivated()
    {
        Messenger.Register<EntryListViewModel, EntryChangedMessage>(
            this,
            (r, m) =>
                r.Entries.Remove(m.Value));

        Messenger.Register<EntryListViewModel, EntriesChangedMessage>(this,
                                                                      (r, m) =>
                                                                      {
                                                                          r.Entries.Clear();
                                                                          r.Entries.AddRange(m.Value);
                                                                      });
    }

    [RelayCommand]
    private async Task LoadEntries()
    {
        Entries = new AvaloniaList<EntryViewModel>(await Messenger.Send<EntriesRequestMessage>());
    }
}
