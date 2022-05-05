using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace DevServer.Views;

public partial class ToolBarPanel : UserControl
{
    public ToolBarPanel()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}

