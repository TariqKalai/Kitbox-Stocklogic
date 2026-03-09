using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Stocklogic.Services;
using Stocklogic.Views;

namespace Stocklogic.ViewModels;

public partial class LockerPageViewModel : ViewModelBase
{
    // Liste observable des casiers (se met à jour automatiquement dans l'UI)
    public ObservableCollection<LockerModel> Lockers { get; } = new();

    // Casier actuellement sélectionné par l'utilisateur
    [ObservableProperty]
    private LockerModel? _selectedLocker;

    // Propriété pour le prix total
    [ObservableProperty]
    private double _totalPrice;

    // Propriété pour le nombre total de casiers
    public int TotalItemsCount => Lockers.Count;

    // Options pour le formulaire d'ajout (exemples)
    public ObservableCollection<int> AvailableHeights { get; } = new() { 32, 42, 52 };

    [ObservableProperty]
    private int _newLockerHeight = 32;

    [RelayCommand]
    private void AddLocker()
    {
        // Logique d'ajout d'un nouveau casier
        var newLocker = new LockerModel 
        { 
            Height = NewLockerHeight, 
            Price = NewLockerHeight * 0.5, // Exemple de calcul de prix
            Description = $"Casier hauteur {NewLockerHeight}cm" 
        };
        
        Lockers.Add(newLocker);
        UpdateTotals();
    }

    [RelayCommand]
    private void RemoveLocker()
    {
        if (SelectedLocker != null)
        {
            Lockers.Remove(SelectedLocker);
            SelectedLocker = null; // Déselectionner après suppression
            UpdateTotals();
        }
    }

    private void UpdateTotals()
    {
        // Recalcul du prix total
        TotalPrice = Lockers.Sum(l => l.Price);
        // Notifier le changement pour le compteur
        OnPropertyChanged(nameof(TotalItemsCount));
    }

    [RelayCommand]
    private void Back()
    {
        // Retourne à la page des dimensions
        NavigationService.Navigate(new DimensionPage());
    }

    [RelayCommand]
    private void Continue()
    {
        if (Lockers.Count > 0)
        {
            // On passe la liste des casiers au constructeur du nouveau ViewModel
            var summaryVM = new SummaryPageViewModel(Lockers);
            var summaryPage = new SummaryPage { DataContext = summaryVM };
            NavigationService.Navigate(summaryPage);
        }
    }
}

// Modèle simple pour représenter un casier
public class LockerModel
{
    public int Height { get; set; }
    public double Price { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Dimensions => $"{Height}x62x42 cm"; // Exemple
}