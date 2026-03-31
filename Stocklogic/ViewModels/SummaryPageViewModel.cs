using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Stocklogic.Models;
using Stocklogic.Services;
using Stocklogic.Views;

namespace Stocklogic.ViewModels;

public partial class SummaryPageViewModel : ViewModelBase
{
    public List<Locker> FinalLockers { get; }
    public Cabinet      Cabinet      { get; }

    [ObservableProperty] private decimal _totalPrice = 0;
    [ObservableProperty] private string  _email      = "";
    [ObservableProperty] private string  _payMessage = "";

    public SummaryPageViewModel(List<Locker> lockers, Cabinet cabinet)
    {
        FinalLockers = lockers;
        Cabinet      = cabinet;
        CalculateTotal();
    }

    [RelayCommand]
    private void ContinueShopping() => NavigationService.Navigate(new DimensionPage());

    [RelayCommand]
    private void Pay()
    {
        if (string.IsNullOrWhiteSpace(Email))
        {
            PayMessage = "Please enter your email.";
            return;
        }

        // 1. Compute all parts needed for the cabinet
        var parts   = CabinetComposerService.ComposeCabinet(Cabinet);
        var codes   = parts.Select(p => p.PartCode).ToList();

        // 2. Fetch part IDs from DB (needed to save Locker_Part rows)
        var partIds = DataBaseService.GetPartIdsByCode(codes);

        // 3. Save the cabinet structure and create the order
        int cabinId = DataBaseService.SaveCabinet(Cabinet, partIds);
        if (cabinId < 0)
        {
            PayMessage = "Error saving order. Please try again.";
            return;
        }

        int orderId = DataBaseService.CreateOrder(Email, TotalPrice, 0, "Pending", cabinId);
        if (orderId < 0)
        {
            PayMessage = "Error creating order. Please try again.";
            return;
        }

        NavigationService.Navigate(new StartPage());
    }

    private void CalculateTotal()
    {
        var parts    = CabinetComposerService.ComposeCabinet(Cabinet);
        var partData = DataBaseService.GetPartsByCode(parts.Select(p => p.PartCode));
        TotalPrice   = parts.Sum(p => partData.TryGetValue(p.PartCode, out var d) ? d.Price * p.Quantity : 0);
    }
}