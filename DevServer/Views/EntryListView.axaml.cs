using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace DevServer.Views;

public partial class EntryListView : UserControl
{
    public EntryListView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}

