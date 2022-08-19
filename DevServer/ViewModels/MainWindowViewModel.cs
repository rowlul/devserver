namespace DevServer.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public EntryListViewModel EntryListViewModel { get; }

    public MainWindowViewModel(EntryListViewModel entryListViewModel)
    {
        EntryListViewModel = entryListViewModel;
    }
}
