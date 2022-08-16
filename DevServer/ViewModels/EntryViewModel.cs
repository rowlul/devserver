using System.Reactive;
using System.Threading.Tasks;

using Avalonia.Media.Imaging;

using DevServer.Models;

using ReactiveUI;

namespace DevServer.ViewModels;

public class EntryViewModel : ViewModelBase
{
    private readonly Entry _entry;

    private Bitmap? _logo;

    public string Name => _entry.Name;
    public string? Description => _entry.Description;

    public Bitmap? Logo
    {
        get => _logo;
        private set => this.RaiseAndSetIfChanged(ref _logo, value);
    }

    public ReactiveCommand<Unit, Unit> LoadLogo { get; }

    public EntryViewModel(Entry entry)
    {
        _entry = entry;

        LoadLogo = ReactiveCommand.CreateFromTask(async () =>
        {
            await using var stream = _entry.Logo?.EncodedData.AsStream();
            Logo = Bitmap.DecodeToWidth(stream, 42);
        });

        LoadLogo.Execute();
    }
}
