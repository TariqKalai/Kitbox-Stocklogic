using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using Stocklogic.Models;
using Stocklogic.Services;
using Stocklogic.Views;

namespace Stocklogic.ViewModels;

public partial class SummaryPageViewModel : ViewModelBase
{
    public ObservableCollection<CabinetOrder> AllOrders { get; } = new();

    public SummaryPageViewModel(CabinetOrder currentOrder)
    {
        foreach (var order in OrderSessionService.Orders)
            AllOrders.Add(order);

        AllOrders.Add(currentOrder);
    }

    [RelayCommand]
    private void ContinueShopping()
    {
        // Save the current order (last in the list)
        OrderSessionService.AddOrder(AllOrders[^1]);
        NavigationService.Navigate(new DimensionPage());
    }

    [RelayCommand]
    private void Pay()
    {
        // TODO: payment logic
        OrderSessionService.Clear();
        NavigationService.Navigate(new StartPage());
    }
}