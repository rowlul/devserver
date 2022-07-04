using Avalonia.Media.Imaging;

using DevServer.Models;

using ReactiveUI;

namespace DevServer.ViewModels;

public class EntryViewModel : ViewModelBase
{
    private readonly Entry _entry;

    private Bitmap? _logo;

    public EntryViewModel(Entry entry)
    {
        _entry = entry;
    }

    public string Name => _entry.Name;
    public string? Description => _entry.Description;

    public Bitmap? Logo
    {
        get => _logo;
        private set => this.RaiseAndSetIfChanged(ref _logo, value);
    }

    // TODO: load logo async method
    // TODO: do business in a seperate service
}
