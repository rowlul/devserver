using System;
using System.Reactive;
using System.Threading.Tasks;

using ReactiveUI;

namespace DevServer.ViewModels;

public class ToolBarPanelViewModel : ViewModelBase
{
    public ReactiveCommand<Unit, Unit> RefreshEntries { get; }

    public ToolBarPanelViewModel(EntryListViewModel entryListViewModel)
    {
        RefreshEntries = entryListViewModel.UpdateEntries;
    }
}
