using System;

using Avalonia;
using Avalonia.Controls;

using CommunityToolkit.Mvvm.ComponentModel;

namespace DevServer.ViewModels;

public class ViewModelBase : ObservableObject
{
    protected static IServiceProvider Services =>
        (IServiceProvider)Application.Current!.FindResource(typeof(IServiceProvider))!;
}
