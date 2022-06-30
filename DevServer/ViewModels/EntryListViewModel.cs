using System.Collections.ObjectModel;

using ReactiveUI;

namespace DevServer.ViewModels;

public class EntryListViewModel : ViewModelBase
{
    private ObservableCollection<EntryViewModel> _entries;
    private EntryViewModel _selectedEntry;

    public ObservableCollection<EntryViewModel> Entries
    {
        get => _entries;
        set => this.RaiseAndSetIfChanged(ref _entries, value);
    }

    public EntryViewModel SelectedEntry
    {
        get => _selectedEntry;
        set => this.RaiseAndSetIfChanged(ref _selectedEntry, value);
    }
}
