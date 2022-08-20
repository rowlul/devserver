using System;
using System.IO;
using System.IO.Abstractions;
using System.Net.Http;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

using CommunityToolkit.Mvvm.DependencyInjection;

using DevServer.Services;
using DevServer.ViewModels;
using DevServer.Views;

using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.Avalonia;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Serilog;

using HttpClientHandler = DevServer.Services.HttpClientHandler;

namespace DevServer;

public class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        Ioc.Default.ConfigureServices(ConfigureServicesInternal());
        var ioc = Ioc.Default;

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow =
                new MainWindow(ioc.GetRequiredService<MainWindowViewModel>());
        }

        if (Design.IsDesignMode)
        {
            return;
        }

        var config = ioc.GetRequiredService<IConfigurationManager>();
        var platform = ioc.GetRequiredService<IPlatformService>();

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

    private static IServiceProvider ConfigureServicesInternal()
    {
        var services = new ServiceCollection();

        services.AddSingleton<IPlatformService>(new PlatformService("devserver"));
        services.AddLogging(logging =>
        {
            logging.AddSerilog(
                new LoggerConfiguration()
                    .Enrich.FromLogContext()
#if !DEBUG
                    .MinimumLevel.Information()
                    .WriteTo.File(Path.Combine(Environment.ProcessPath!, "log.txt"))
                    .WriteTo.Console()
#else
                    .MinimumLevel.Debug()
                    .WriteTo.Debug()
#endif

                    .CreateLogger());
        });
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

        services.AddSingleton(provider => new MainPanelViewModel(
                                  provider.GetRequiredService<ILogger<MainPanelViewModel>>(),
                                  provider.GetRequiredService<IEntryService>()));
        services.AddSingleton(provider => new EntryListViewModel());
        services.AddSingleton(provider => new MainWindowViewModel(provider.GetRequiredService<EntryListViewModel>(),
                                                                  provider.GetRequiredService<MainPanelViewModel>()));

        return services.BuildServiceProvider();
    }
}
