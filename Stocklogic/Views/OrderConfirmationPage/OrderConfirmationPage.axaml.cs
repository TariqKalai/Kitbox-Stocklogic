using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Stocklogic.ViewModels;

namespace Stocklogic.Views;

public partial class OrderConfirmationPage : UserControl
{
    public OrderConfirmationPage(int orderId)
    {
        InitializeComponent();
        DataContext = new OrderConfirmationPageViewModel(orderId);
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}