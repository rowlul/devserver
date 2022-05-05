using DevServer.Extensions;
using DevServer.ViewModels;

using Splat;

namespace DevServer;

public static class Bootstrapper
{
    public static void RegisterDependencies(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
    {
        RegisterViewModels(services, resolver);
    }

    public static void RegisterViewModels(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
    {
        services.RegisterLazySingleton(() => new MainWindowViewModel());
        services.RegisterLazySingleton(() => new ToolBarPanelViewModel());
        services.RegisterLazySingleton(() => new EntryListViewModel());
    }
}
