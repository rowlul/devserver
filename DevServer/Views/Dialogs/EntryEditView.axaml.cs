using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace DevServer.Views.Dialogs;

public partial class EntryEditView : Window
{
    public EntryEditView()
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
