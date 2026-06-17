using System;
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
    private const decimal DepositRate = 0.30m;

    public List<Locker> FinalLockers { get; }
    public Cabinet      Cabinet      { get; }

    [ObservableProperty] private decimal _totalPrice      = 0;
    [ObservableProperty] private decimal _depositAmount   = 0;
    [ObservableProperty] private bool    _isDepositNeeded = false;
    [ObservableProperty] private string  _email           = "";
    [ObservableProperty] private string  _payMessage      = "";

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

        // 1. Compute all parts needed for the cabinet, and re-check stock
        //    right before creating the order (stock may have moved since
        //    this page was first opened).
        var (parts, _, allAvailable) = GetPartsAndAvailability();
        var codes = parts.Select(p => p.PartCode).ToList();

        // 2. Fetch part IDs from DB (needed to save Locker_Part rows)
        var partIds = DataBaseService.GetPartIdsByCode(codes);

        // 3. Save the cabinet structure and create the order
        int cabinId = DataBaseService.SaveCabinet(Cabinet, partIds);
        if (cabinId < 0)
        {
            PayMessage = "Error saving order. Please try again.";
            return;
        }

        // 4. Status + deposit depend on whether every part is in stock:
        //    fully available -> "Ready", paid in full ; otherwise -> "Pending",
        //    customer pays a 30% deposit and comes back when restocked.
        string  status  = allAvailable ? "Ready"    : "Pending";
        decimal deposit = allAvailable ? TotalPrice  : Math.Round(TotalPrice * DepositRate, 2);

        int orderId = DataBaseService.CreateOrder(Email, TotalPrice, deposit, status, cabinId);
        if (orderId < 0)
        {
            PayMessage = "Error creating order. Please try again.";
            return;
        }

        // 5. Only decrement stock for parts that are actually available;
        //    out-of-stock parts are left untouched, waiting for restock.
        if (allAvailable)
            DataBaseService.DecrementStock(parts.ToDictionary(p => p.PartCode, p => p.Quantity));

        NavigationService.Navigate(new OrderConfirmationPage(orderId));
    }

    private void CalculateTotal()
    {
        var (parts, partData, allAvailable) = GetPartsAndAvailability();

        TotalPrice      = parts.Sum(p => partData.TryGetValue(p.PartCode, out var d) ? d.Price * p.Quantity : 0);
        IsDepositNeeded = !allAvailable;
        DepositAmount   = IsDepositNeeded ? Math.Round(TotalPrice * DepositRate, 2) : TotalPrice;
    }

    /// <summary>
    /// Composes the cabinet's parts list and checks whether every part has
    /// enough stock in the DB to fulfil the order immediately.
    /// </summary>
    private (List<(string PartCode, int Quantity)> Parts,
              Dictionary<string, (int InStock, decimal Price)> PartData,
              bool AllAvailable) GetPartsAndAvailability()
    {
        var parts    = CabinetComposerService.ComposeCabinet(Cabinet);
        var partData = DataBaseService.GetPartsByCode(parts.Select(p => p.PartCode));

        bool allAvailable = parts.All(p =>
            partData.TryGetValue(p.PartCode, out var d) && d.InStock >= p.Quantity);

        return (parts, partData, allAvailable);
    }
}