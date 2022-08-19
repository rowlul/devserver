using System;
using System.Threading;

using Avalonia;
using Avalonia.Controls;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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

            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
        }
        catch (Exception e)
        {
            var services = (IServiceProvider)Application.Current!.FindResource(typeof(IServiceProvider))!;
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(e, "Unexpected exception");
        }
        finally
        {
            mutex.ReleaseMutex();
        }
    }

    public static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure<App>()
                         .UsePlatformDetect()
                         .LogToTrace();
    }
}
