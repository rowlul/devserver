using System.Reactive;
using System.Threading.Tasks;

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

    public ReactiveCommand<Unit, Unit> UpdateEntriesCommand { get; }
    public ReactiveCommand<Unit, Unit> DirectConnectCommand { get; }
    public ReactiveCommand<Unit, Unit> AddEntryCommand { get; }

    public EntryListViewModel(IEntryService entryService)
    {
        _entryService = entryService;

        UpdateEntriesCommand = ReactiveCommand.CreateFromTask(UpdateEntries);
        DirectConnectCommand = ReactiveCommand.CreateFromTask(DirectConnect);
        AddEntryCommand = ReactiveCommand.CreateFromTask(AddEntry);

        UpdateEntriesCommand.Execute();
    }


    private async Task UpdateEntries()
    {
        Entries.Clear();

        await foreach (var entry in _entryService.GetEntries())
        {
            Entries.Add(new EntryViewModel(entry));
        }
    }

    private Task DirectConnect()
    {
        return Task.CompletedTask;
    }

    private Task AddEntry()
    {
        return Task.CompletedTask;
    }
}
