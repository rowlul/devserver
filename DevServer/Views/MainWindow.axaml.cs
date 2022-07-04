using Avalonia.Controls;

namespace DevServer.Views;

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
