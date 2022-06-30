using System;
using System.Reactive;
using System.Threading.Tasks;

using ReactiveUI;

namespace DevServer.ViewModels;

public class ToolBarPanelViewModel : ViewModelBase
{
    private string _serverAddress;
    
    public ToolBarPanelViewModel()
    {
        IObservable<bool> isNotEmpty = this.WhenAnyValue(
            x => x.ServerAddress,
            x => !string.IsNullOrWhiteSpace(x));

        PlayCommand = ReactiveCommand.CreateFromTask(Play, isNotEmpty);
    }

    public string ServerAddress
    {
        get => _serverAddress;
        set => this.RaiseAndSetIfChanged(ref _serverAddress, value);
    }

    public ReactiveCommand<Unit, Unit> PlayCommand { get; }

    private Task Play()
    {
        return Task.CompletedTask;
    }
}
