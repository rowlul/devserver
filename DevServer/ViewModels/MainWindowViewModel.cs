using Avalonia.Controls;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

using DevServer.Messages;

namespace DevServer.ViewModels;

public partial class MainWindowViewModel : RecipientViewModelBase
{
    [ObservableProperty]
    private bool _canExecute;

    [ObservableProperty]
    private WindowState _windowState;

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
                // can execute if process is not running
                r.CanExecute = !m.Value;

                r.WindowState = m.Value ? WindowState.Minimized : WindowState.Normal;
            });
    }
}
