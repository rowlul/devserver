using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace DevServer.Avalonia.Views;

public partial class DirectConnectView : Window
{
    public DirectConnectView()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
