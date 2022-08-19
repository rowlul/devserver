using System.Threading.Tasks;

using Avalonia.Collections;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using DevServer.Services;

namespace DevServer.ViewModels;

public partial class EntryListViewModel : ViewModelBase
{
    private readonly IEntryService _entryService;

    [ObservableProperty] private AvaloniaList<EntryViewModel> _entries = new();

    [ObservableProperty] private EntryViewModel? _selectedEntry;

    public EntryListViewModel(IEntryService entryService)
    {
        _entryService = entryService;

        UpdateEntriesCommand.ExecuteAsync(null);
    }


    [RelayCommand]
    private async Task UpdateEntries()
    {
        Entries.Clear();

        await foreach (var entry in _entryService.GetEntries())
        {
            Entries.Add(new EntryViewModel(entry));
        }
    }

    [RelayCommand]
    private Task DirectConnect()
    {
        return Task.CompletedTask;
    }

    [RelayCommand]
    private Task AddEntry()
    {
        return Task.CompletedTask;
    }

    [RelayCommand]
    private Task EditEntry()
    {
        return Task.CompletedTask;
    }

    [RelayCommand]
    private async Task DeleteEntry()
    {
        if (_selectedEntry is null)
        {
            return;
        }

        await _entryService.DeleteEntry(_selectedEntry.FilePath);
        _entries.Remove(_selectedEntry);
    }
}
