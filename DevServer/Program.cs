using System;
using System.Threading;

using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.ReactiveUI;

namespace DevServer
{
    class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        
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
            finally
            {
                mutex.ReleaseMutex();
            }
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace()
                .UseReactiveUI();
    }
}
