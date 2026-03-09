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
    public ObservableCollection<string> AvailableColors { get; } = new() { "White", "Black", "Grey", "Beige" };
public ObservableCollection<string> AvailableDoorTypes { get; } = new() { "Wood", "Glass", "Plastic" };



    // Casier actuellement sélectionné par l'utilisateur
    [ObservableProperty]
    private LockerModel? _selectedLocker;
    public bool IsLockerSelected => SelectedLocker != null;

// Quand SelectedLocker change, notifier IsLockerSelected
partial void OnSelectedLockerChanged(LockerModel? value)
{
    OnPropertyChanged(nameof(IsLockerSelected));
}

    // Propriété pour le prix total
    [ObservableProperty]
    private double _totalPrice;
    [ObservableProperty] private bool _height30;
[ObservableProperty] private bool _height45;
[ObservableProperty] private bool _height70 = true;

[ObservableProperty] private bool _doorWood;
[ObservableProperty] private bool _doorGlass = true;
[ObservableProperty] private bool _doorPlastic;

    // Propriété pour le nombre total de casiers
    public int TotalItemsCount => Lockers.Count;
    public int TotalHeight => Lockers.Sum(l => l.Height);
public int TotalWidth => 62; 
public int TotalDepth => 42; 

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
        OnPropertyChanged(nameof(TotalHeight));  
    OnPropertyChanged(nameof(TotalWidth));   
    OnPropertyChanged(nameof(TotalDepth));   
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
    [RelayCommand]
private void SelectLocker(LockerModel locker)
{
    SelectedLocker = locker;
}
}

// Modèle simple pour représenter un casier
public class LockerModel
{
    public int Height { get; set; }
    public double Price { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Dimensions => $"{Height}x62x42 cm";
    
    public string LockerColor { get; set; } = "White";
    public string DoorColor { get; set; } = "White";
    public string DoorType { get; set; } = "Wood";
}