using HanumanInstitute.MvvmDialogs.Avalonia;

namespace DevServer;

public class ViewLocator : ViewLocatorBase
{
    protected override string GetViewName(object viewModel)
    {
        return viewModel.GetType().FullName!.Replace("ViewModel", "");
    }
}
