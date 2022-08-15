using System.IO;

using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

using DevServer.Models;
using DevServer.Services;
using DevServer.ViewModels;
using DevServer.Views;

using Splat;

namespace DevServer;

public class App : Application
{
    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        var resolver = Locator.Current;

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow =
                new MainWindow(resolver.GetService<MainWindowViewModel>());
        }

        var config = resolver.GetService<IConfigurationManager>();
        var platform = resolver.GetService<IPlatformService>();
        if (!File.Exists(platform.GetConfigFile()))
        {
            config.Settings = new Settings();
            config.Save();
        }
        else
        {
            config.Load();
        }


        base.OnFrameworkInitializationCompleted();
    }
}
