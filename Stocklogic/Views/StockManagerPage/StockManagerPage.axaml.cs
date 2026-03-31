using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Stocklogic.ViewModels;

namespace Stocklogic.Views;

public partial class StockManagerPage : UserControl
{
    public StockManagerPage()
    {
        InitializeComponent();
        DataContext = new StockManagerPageViewModel();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
