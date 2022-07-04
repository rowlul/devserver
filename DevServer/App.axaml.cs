using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

using DevServer.Extensions;
using DevServer.ViewModels;
using DevServer.Views;

using Splat;

namespace DevServer;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow =
                new MainWindow(Locator.Current.GetRequiredService<MainWindowViewModel>());
        }

        base.OnFrameworkInitializationCompleted();
    }
}
