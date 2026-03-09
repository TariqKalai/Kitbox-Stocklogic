using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Stocklogic.Services;
using Stocklogic.Views;

namespace Stocklogic.ViewModels;

public partial class SummaryPageViewModel : ViewModelBase
{
    [ObservableProperty]
    private ObservableCollection<LockerModel> _finalLockers;

    [ObservableProperty]
    private double _totalPrice;

    public SummaryPageViewModel(ObservableCollection<LockerModel> lockers)
    {
        FinalLockers = lockers;
        TotalPrice = lockers.Sum(l => l.Price);
    }

    [RelayCommand]
    private void ContinueShopping()
    {
        // Retourne à la page de sélection initiale ou des dimensions
        NavigationService.Navigate(new DimensionPage());
    }

    [RelayCommand]
    private void Pay()
    {
        // Logique de paiement ou page de succès
        // NavigationService.Navigate(new SuccessPage());
    }
}