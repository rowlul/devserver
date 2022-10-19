using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace DevServer.Avalonia.Views;

public partial class MainPanelView : UserControl
{
    public MainPanelView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
