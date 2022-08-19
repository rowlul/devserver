using System;
using System.Threading.Tasks;

using Avalonia.Collections;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using DevServer.Messages;
using DevServer.Services;

using Microsoft.Extensions.Logging;

namespace DevServer.ViewModels;

public partial class EntryListViewModel : RecipientViewModelBase
{
    private readonly ILogger<EntryListViewModel> _logger;
    private readonly IEntryService _entryService;

    [ObservableProperty]
    private AvaloniaList<EntryViewModel> _entries = new();

    [ObservableProperty]
    private EntryViewModel? _selectedEntry;

    [ObservableProperty]
    private bool _isEnabled = true;

    public EntryListViewModel(
        ILogger<EntryListViewModel> logger,
        IEntryService entryService)
    {
        _logger = logger;
        _entryService = entryService;

        IsActive = true;
    }

    protected override void OnActivated()
    {
        Messenger.Register<EntryListViewModel, ProcessRunningMessage>(
            this,
            (r, m) =>
                r.IsEnabled = !m.Value);
    }

    [RelayCommand]
    private async Task UpdateEntries()
    {
        Entries.Clear();

        var enumerable = _entryService.GetEntries();
        await using var enumerator = enumerable.GetAsyncEnumerator();
        for (var more = true; more;)
        {
            try
            {
                more = await enumerator.MoveNextAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Could not parse entry");
                continue;
            }

            if (more is not false)
            {
                // ensure we're not adding an extra entry if failed
                Entries.Add(new EntryViewModel(enumerator.Current));
            }
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
