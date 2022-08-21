using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using DevServer.Messages;
using DevServer.Services;

using Microsoft.Extensions.Logging;

namespace DevServer.ViewModels;

public partial class MainPanelViewModel : RecipientViewModelBase
{
    private readonly IEntryService _entryService;
    private readonly ILogger<MainPanelViewModel> _logger;

    [ObservableProperty]
    private bool _isEnabled = true;

    public MainPanelViewModel(ILogger<MainPanelViewModel> logger,
                              IEntryService entryService)
    {
        _logger = logger;
        _entryService = entryService;

        IsActive = true;
    }

    protected override void OnActivated()
    {
        Messenger.Register<MainPanelViewModel, EntriesRequestMessage>(this,
                                                                      (r, m) =>
                                                                          m.Reply(r.GetEntryList()));
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
    private async Task RefreshEntries()
    {
        var entries = await GetEntryList();
        Messenger.Send(new EntriesChangedMessage(entries));
    }

    private async Task<IList<EntryViewModel>> GetEntryList()
    {
        List<EntryViewModel> entries = new();

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
                entries.Add(new EntryViewModel(enumerator.Current));
            }
        }

        return entries;
    }
}
