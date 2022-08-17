using System;
using System.IO.Abstractions;
using System.Threading;

using Avalonia;
using Avalonia.ReactiveUI;

using DevServer.Services;
using DevServer.ViewModels;

using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.Avalonia;

using Splat;

namespace DevServer;

internal class Program
{
    private const int TimeoutSeconds = 3;

    [STAThread]
    public static void Main(string[] args)
    {
        var mutex = new Mutex(false, typeof(Program).FullName);

        try
        {
            if (!mutex.WaitOne(TimeSpan.FromSeconds(TimeoutSeconds), true))
            {
                return;
            }

            SplatRegistrations.SetupIOC();
            RegisterDependencies();

            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
        }
        finally
        {
            mutex.ReleaseMutex();
        }
    }

    public static void RegisterDependencies()
    {
        var mutableResolver = Locator.CurrentMutable;
        var readonlyResolver = Locator.Current;

        mutableResolver.RegisterLazySingleton<IPlatformService>(() => new PlatformService("devserver"));
        mutableResolver.RegisterLazySingleton<IHttpHandler>(
            () => new HttpClientHandler(new System.Net.Http.HttpClient()));

        mutableResolver.RegisterLazySingleton<IDialogService>(
            () => new DialogService(
                new DialogManager(viewLocator: new ViewLocator(),
                                  dialogFactory: new DialogFactory().AddMessageBox()),
                viewModelFactory: x => readonlyResolver.GetService(x)));

        SplatRegistrations.RegisterLazySingleton<IProcess, ProcessProxy>();
        SplatRegistrations.RegisterLazySingleton<INativeRunner, NativeRunner>();
        SplatRegistrations.RegisterLazySingleton<IWineRunner, WineRunner>();
        SplatRegistrations.RegisterLazySingleton<IFileSystem, FileSystem>();
        SplatRegistrations.RegisterLazySingleton<IConfigurationManager, ConfigurationManager>();
        SplatRegistrations.RegisterLazySingleton<IHttpHandler, HttpClientHandler>();
        SplatRegistrations.RegisterLazySingleton<IEntryService, EntryService>();

        SplatRegistrations.RegisterLazySingleton<EntryListViewModel>();
        SplatRegistrations.RegisterLazySingleton<MainWindowViewModel>();
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
                     .UsePlatformDetect()
                     .LogToTrace()
                     .UseReactiveUI();
}
