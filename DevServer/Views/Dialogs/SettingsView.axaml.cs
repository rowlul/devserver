using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace DevServer.Views.Dialogs;

public partial class SettingsView : Window
{
    public SettingsView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
