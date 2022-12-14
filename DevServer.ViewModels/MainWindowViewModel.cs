using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

using DevServer.ViewModels.Messages;

namespace DevServer.ViewModels;

public class MainWindowViewModel : ObservableRecipient
{
    public EntryListViewModel EntryListViewModel { get; }
    public MainPanelViewModel MainPanelViewModel { get; }

    public MainWindowViewModel(EntryListViewModel entryListViewModel, MainPanelViewModel mainPanelViewModel)
    {
        EntryListViewModel = entryListViewModel;
        MainPanelViewModel = mainPanelViewModel;

        IsActive = true;
    }

    protected override void OnActivated()
    {
        Messenger.Register<MainWindowViewModel, ProcessRunningMessage>(
            this,
            (r, m) =>
            {
                // enabled if process is not running
                r.MainPanelViewModel.IsEnabled = !m.Value;
                r.EntryListViewModel.IsEnabled = !m.Value;
            });
    }
}
