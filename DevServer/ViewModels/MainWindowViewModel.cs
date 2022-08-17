using Splat;

namespace DevServer.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    [DependencyInjectionProperty]
    public EntryListViewModel EntryListViewModel { get; set; }
}
