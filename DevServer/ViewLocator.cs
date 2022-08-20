using System;
using System.Reflection;

using Avalonia.Controls;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;

using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.Avalonia;

using Microsoft.Extensions.DependencyInjection;

namespace DevServer;

public class ViewLocator : ViewLocatorBase
{
    protected override string GetViewName(object viewModel)
    {
        return viewModel.GetType().FullName!.Replace("ViewModel", "");
    }

    public override object Locate(object viewModel)
    {
        var viewName = GetViewName(viewModel);
        var type = Assembly.GetEntryAssembly()?.GetType(viewName);
        var obj = type != null ? Ioc.Default.GetRequiredService(type) : null;

        return obj switch
        {
             Window or IWindow or IControl => obj,
            _ => throw new TypeLoadException($"Dialog {viewName} is missing")
        };
    }

    public override bool Match(object data)
    {
        return data is ObservableObject;
    }
}
