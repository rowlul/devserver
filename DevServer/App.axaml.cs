using System;
using System.IO;
using System.IO.Abstractions;
using System.Net.Http;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

using DevServer.Services;
using DevServer.ViewModels;
using DevServer.Views;

using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.Avalonia;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using HttpClientHandler = DevServer.Services.HttpClientHandler;

namespace DevServer;

public class App : Application
{
    private readonly IServiceProvider _serviceCollection;

    public App()
    {
        _serviceCollection = ConfigureServices();
    }

    public override void Initialize()
    {
        Resources[typeof(IServiceProvider)] = _serviceCollection;

        DataTemplates.Add(_serviceCollection.GetRequiredService<ViewLocator>());

        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow =
                new MainWindow(_serviceCollection.GetRequiredService<MainWindowViewModel>());
        }

        if (!Design.IsDesignMode)
        {
            var config = _serviceCollection.GetRequiredService<IConfigurationManager>();
            var platform = _serviceCollection.GetRequiredService<IPlatformService>();

            if (!Directory.Exists(platform.GetAppRootPath()))
            {
                Directory.CreateDirectory(platform.GetAppRootPath());
            }

            if (!Directory.Exists(platform.GetEntryStorePath()))
            {
                Directory.CreateDirectory(platform.GetEntryStorePath());
            }

            if (!File.Exists(platform.GetConfigFile()))
            {
                config.Save();
            }
            else
            {
                config.Load();
            }
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        services.AddLogging(logging => logging.AddConsole());
        services.AddSingleton<IPlatformService>(new PlatformService("devserver"));
        services.AddSingleton<IHttpHandler>(new HttpClientHandler(new HttpClient()));
        services.AddSingleton<ViewLocator>();
        services.AddSingleton<IDialogService>(provider => new DialogService(
                                                  new DialogManager(provider.GetRequiredService<ViewLocator>(),
                                                                    new DialogFactory().AddMessageBox()),
                                                  viewModelFactory: x => provider.GetRequiredService<ViewLocator>()));
        services.AddSingleton<IProcess, ProcessProxy>();
        services.AddSingleton<INativeRunner>(provider => new NativeRunner(provider.GetRequiredService<IProcess>()));
        services.AddSingleton<IWineRunner>(provider => new WineRunner(provider.GetRequiredService<IProcess>()));
        services.AddSingleton<IFileSystem, FileSystem>();
        services.AddSingleton<IConfigurationManager>(
            provider => new ConfigurationManager(provider.GetRequiredService<IPlatformService>(),
                                                 provider.GetRequiredService<IFileSystem>()));
        services.AddSingleton<IEntryService>(provider => new EntryService(
                                                 provider.GetRequiredService<IPlatformService>(),
                                                 provider.GetRequiredService<IFileSystem>(),
                                                 provider.GetRequiredService<IHttpHandler>()));

        services.AddSingleton(provider => new EntryListViewModel(
                                  provider.GetRequiredService<ILogger<EntryListViewModel>>(),
                                  provider.GetRequiredService<IEntryService>()));
        services.AddSingleton(provider => new MainWindowViewModel(provider.GetRequiredService<EntryListViewModel>()));

        return services.BuildServiceProvider();
    }
}
