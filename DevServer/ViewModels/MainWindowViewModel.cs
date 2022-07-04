namespace DevServer.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public EntryListViewModel EntryListViewModel { get; }
    public ToolBarPanelViewModel ToolBarPanelViewModel { get; }

    public MainWindowViewModel(
        EntryListViewModel entryListViewModel,
        ToolBarPanelViewModel toolBarPanelViewModel)
    {
        EntryListViewModel = entryListViewModel;
        ToolBarPanelViewModel = toolBarPanelViewModel;
    }
}
