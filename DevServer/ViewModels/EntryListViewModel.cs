using System.Reactive;

using Avalonia.Collections;

using DevServer.Services;

using ReactiveUI;

namespace DevServer.ViewModels;

public class EntryListViewModel : ViewModelBase
{
    private readonly IEntryService _entryService;

    private AvaloniaList<EntryViewModel> _entries = new();
    private EntryViewModel? _selectedEntry;

    public AvaloniaList<EntryViewModel> Entries
    {
        get => _entries;
        set => this.RaiseAndSetIfChanged(ref _entries, value);
    }

    public EntryViewModel? SelectedEntry
    {
        get => _selectedEntry;
        set => this.RaiseAndSetIfChanged(ref _selectedEntry, value);
    }

    public ReactiveCommand<Unit, Unit> UpdateEntries { get; }

    public EntryListViewModel(IEntryService entryService)
    {
        _entryService = entryService;

        UpdateEntries = ReactiveCommand.CreateFromTask(async () =>
        {
            Entries.Clear();

            await foreach (var entry in entryService.GetEntries())
            {
                Entries.Add(new EntryViewModel(entry));
            }
        });

        UpdateEntries.Execute();
    }
}
