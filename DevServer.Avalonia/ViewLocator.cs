using System.Reflection;

using Avalonia.Controls;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;

using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.Avalonia;

namespace DevServer.Avalonia;

public class ViewLocator : ViewLocatorBase
{
    public override object Locate(object viewModel)
    {
        var viewName = GetViewName(viewModel);
        var ass = Assembly.GetExecutingAssembly();
        var type = Assembly.GetExecutingAssembly()?.GetType(viewName);
        var obj = type != null ? Ioc.Default.GetService(type) ?? Activator.CreateInstance(type) : null;

        return obj switch
        {
            Window or IView or IControl => obj,
            _ => throw new TypeLoadException($"Dialog {viewName} is missing")
        };
    }

    protected override string GetViewName(object viewModel)
    {
        var name = viewModel.GetType().Name;
        return Assembly.GetExecutingAssembly().GetName().Name + ".Views." + name.Replace("ViewModel", "View");
    }

    public override bool Match(object data)
    {
        return data is ObservableObject;
    }
}
