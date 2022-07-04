using System;
using System.Threading;

using Avalonia;
using Avalonia.ReactiveUI;

using DevServer.ViewModels;

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
        SplatRegistrations.RegisterLazySingleton<EntryListViewModel>();
        SplatRegistrations.RegisterLazySingleton<ToolBarPanelViewModel>();
        SplatRegistrations.RegisterLazySingleton<MainWindowViewModel>();
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
                     .UsePlatformDetect()
                     .LogToTrace()
                     .UseReactiveUI();
}
