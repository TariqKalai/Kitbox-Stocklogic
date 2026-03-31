using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Stocklogic.Models;
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
        DataBaseService.ReadProductsByHeight();
    }

    [RelayCommand]
    private void Continue()
    {
        if (Depth.HasValue && Width.HasValue)
        {
            var cabinet = new Cabinet { Depth = Depth.Value, Width = Width.Value };
            NavigationService.Navigate(new LockerPage(cabinet));
        }
    }
}