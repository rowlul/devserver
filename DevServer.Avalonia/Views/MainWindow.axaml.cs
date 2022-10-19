using Avalonia.Controls;

namespace DevServer.Avalonia.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    public MainWindow(object? dataContext) : this()
    {
        DataContext = dataContext;
    }
}
