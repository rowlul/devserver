using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace DevServer.Avalonia.Views;

public partial class EntryView : UserControl
{
    public EntryView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
