using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using DevServer.Services;
using DevServer.ViewModels.Extensions;
using DevServer.ViewModels.Messages;

using HanumanInstitute.MvvmDialogs;

using Microsoft.Extensions.Logging;

namespace DevServer.ViewModels;

public partial class MainPanelViewModel : ViewModelBase
{
    private readonly ILogger<MainPanelViewModel> _logger;
    private readonly IDialogService _dialogService;
    private readonly IEntryService _entryService;
    private readonly ILogoService _logoService;

    [ObservableProperty]
    private bool _isEnabled = true;

    public MainPanelViewModel(ILogger<MainPanelViewModel> logger,
                              IDialogService dialogService,
                              IEntryService entryService,
                              ILogoService logoService)
    {
        _logger = logger;
        _dialogService = dialogService;
        _entryService = entryService;
        _logoService = logoService;

        IsActive = true;
    }

    protected override void OnActivated()
    {
        Messenger.Register<MainPanelViewModel, EntriesRequestMessage>(this,
                                                                      (r, m) =>
                                                                          m.Reply(r.GetEntryList()));
    }

    [RelayCommand]
    private async Task DirectConnect()
    {
        await _dialogService.ShowDirectConnectDialog();
    }

    [RelayCommand]
    private async Task AddEntry()
    {
        var entry = await _dialogService.ShowEntryEditViewModel();
        if (entry is not null)
        {
            await _entryService.UpsertEntry(entry);
            Messenger.Send(new EntryCreatedMessage(new EntryViewModel(entry)));
        }
    }

    [RelayCommand]
    private async Task RefreshEntries()
    {
        var result = await _dialogService.ShowMessageBox(
            Ioc.Default.GetRequiredService<MainWindowViewModel>(),
            "Are you sure to refresh all servers?",
            "Icon cache will also be cleared thus new icons should appear.",
            MessageBoxIcon.Question,
            MessageBoxButtons.OkCancel);

        if (result is false)
        {
            return;
        }

        _logoService.PurgeCache();

        var entries = await GetEntryList();
        Messenger.Send(new EntriesChangedMessage(entries));
    }

    [RelayCommand]
    private async Task ShowSettings()
    {
        await _dialogService.ShowSettingsDialog();
    }

    [RelayCommand]
    private async Task ShowAbout()
    {
        await _dialogService.ShowAboutDialog();
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
                _logger.LogError(e, "Could not parse one or more entries");
                await _dialogService.ShowLogBox(Ioc.Default.GetRequiredService<MainWindowViewModel>(),
                    $"Could not parse one or more entries",
                    e.Message, showReport: true, MessageBoxIcon.Error);
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
