using System;

using Avalonia.Controls;
using Avalonia.Controls.Templates;

using DevServer.ViewModels;

using HanumanInstitute.MvvmDialogs.Avalonia;

using Splat;

namespace DevServer;

public class ViewLocator : ViewLocatorBase
{
    protected override string GetViewName(object viewModel) => viewModel.GetType().FullName!.Replace("ViewModel", "");
}
