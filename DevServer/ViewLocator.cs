using System;

using Avalonia.Controls;
using Avalonia.Controls.Templates;

using DevServer.ViewModels;

using Splat;

namespace DevServer;

public class ViewLocator : IDataTemplate
{
    public IControl Build(object data)
    {
        var name = data.GetType().FullName!.Replace("ViewModel", "View");
        var type = Type.GetType(name);

        if (type is null)
        {
            return new TextBlock { Text = "Not Found: " + name };
        }

        return (Control)Locator.Current.GetService(type);
    }

    public bool Match(object data) => data is ViewModelBase;
}
