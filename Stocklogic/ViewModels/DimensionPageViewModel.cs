using System.Collections.Generic; // À ajouter pour List<>
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Stocklogic.Services;
using Stocklogic.Views;

namespace Stocklogic.ViewModels;

public partial class DimensionPageViewModel : ViewModelBase
{
    // Propriétés liées aux choix sélectionnés
    [ObservableProperty]
    private int? _depth;

    [ObservableProperty]
    private int? _width;

    // Listes des options disponibles pour les menus déroulants
    public List<int> AvailableDepths { get; } = new() { 32, 42, 52, 62 };
    public List<int> AvailableWidths { get; } = new() { 32, 42, 52, 62, 80, 100, 120 };

    [RelayCommand]
    private void Back()
    {
        NavigationService.Navigate(new StartPage());
    }

    [RelayCommand]
    private void Continue()
    {
        if (Depth.HasValue && Width.HasValue)
        {
            NavigationService.Navigate(new LockerPage());
        }
    }
}