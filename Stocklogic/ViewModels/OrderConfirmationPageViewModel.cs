using CommunityToolkit.Mvvm.ComponentModel;

namespace Stocklogic.ViewModels;

public partial class OrderConfirmationPageViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _orderNumberText = "";

    public OrderConfirmationPageViewModel(int orderId)
    {
        OrderNumberText = $"Your number order is #{orderId}.";
    }
}