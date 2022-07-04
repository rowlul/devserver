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
        services.RegisterLazySingleton(() => new EntryListViewModel());
        services.RegisterLazySingleton(() => new ToolBarPanelViewModel());
        services.RegisterLazySingleton(() => new MainWindowViewModel(
                                           resolver.GetRequiredService<EntryListViewModel>(),
                                           resolver.GetRequiredService<ToolBarPanelViewModel>()));
    }
}
