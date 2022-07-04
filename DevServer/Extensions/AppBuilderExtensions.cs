using Avalonia.Controls;
using Avalonia.ReactiveUI;
using Avalonia.Threading;

using ReactiveUI;

using Splat;

namespace DevServer.Extensions;

public static class AppBuilderExtensions
{
    public static TAppBuilder RegisterDependencies<TAppBuilder>(this TAppBuilder builder)
        where TAppBuilder : AppBuilderBase<TAppBuilder>, new() =>
        builder.AfterPlatformServicesSetup(_ =>
        {
            Bootstrapper.RegisterDependencies(Locator.CurrentMutable, Locator.Current);
        });
}
