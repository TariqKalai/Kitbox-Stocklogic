
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Stocklogic.Models;
using Stocklogic.Services;
using Stocklogic.Views;
using System;

namespace Stocklogic.ViewModels;

public partial class SummaryPageViewModel : ViewModelBase
{
    public List<Locker> FinalLockers { get; }
    public Cabinet Cabinet { get; }

    // Le prix total sera calculé depuis la DB dans une prochaine itération
    [ObservableProperty]
    private decimal _totalPrice = 0;

    public SummaryPageViewModel(List<Locker> lockers, Cabinet cabinet)
    {
        FinalLockers = lockers;
        Cabinet      = cabinet;
    }

    [RelayCommand]
    private void ContinueShopping()
    {
        NavigationService.Navigate(new DimensionPage());
    }

    [RelayCommand]
    private void Pay()
    {
        // TODO: logique de paiement / page de confirmation
    }
}
