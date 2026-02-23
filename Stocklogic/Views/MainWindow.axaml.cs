using Avalonia.Controls;
using Stocklogic.Services;

namespace Stocklogic.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        var host = this.FindControl<ContentControl>("PageHost");
        NavigationService.PageHost = host;

        // Set the initial page
        NavigationService.Navigate(new StartPage());
    }
}