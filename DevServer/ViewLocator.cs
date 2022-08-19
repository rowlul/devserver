using System;
using System.Reflection;

using Avalonia.Controls;

using CommunityToolkit.Mvvm.ComponentModel;

using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.Avalonia;

using Microsoft.Extensions.DependencyInjection;

namespace DevServer;

public class ViewLocator : ViewLocatorBase
{
    private readonly IServiceProvider _serviceProvider;

    public ViewLocator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override string GetViewName(object viewModel)
    {
        return viewModel.GetType().FullName!.Replace("ViewModel", "");
    }

    public override object Locate(object viewModel)
    {
        var viewName = GetViewName(viewModel);
        var type = Assembly.GetEntryAssembly()?.GetType(viewName);
        var obj = type != null ? _serviceProvider.GetRequiredService(type) : null;

        switch (obj)
        {
            case Window _:
            case IControl _:
            case IWindow _:
                return obj;
            default:
                throw new TypeLoadException($"Dialog {viewName} is missing");
        }
    }

    public override bool Match(object data)
    {
        return data is ObservableObject;
    }
}
