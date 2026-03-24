using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Stocklogic.Services;
using Stocklogic.Views;

namespace Stocklogic.ViewModels;

public partial class StartPageViewModel : ViewModelBase
{
    private const string StockManagerPassword = "admin1234";

    [ObservableProperty] private bool   _isPasswordOverlayVisible;
    [ObservableProperty] private string _passwordInput = "";
    [ObservableProperty] private string _passwordError = "";

    [RelayCommand]
    private void Start()
    {
        NavigationService.Navigate(new DimensionPage());
        Console.WriteLine("Start button clicked, navigating to DimensionPage.");
    }

    [RelayCommand]
    private void OpenStockManager()
    {
        PasswordInput            = "";
        PasswordError            = "";
        IsPasswordOverlayVisible = true;
    }

    [RelayCommand]
    private void ConfirmPassword()
    {
        if (PasswordInput == StockManagerPassword)
        {
            IsPasswordOverlayVisible = false;
            NavigationService.Navigate(new StockManagerPage());
        }
        else
        {
            PasswordError = "Incorrect password.";
        }
    }

    [RelayCommand]
    private void CancelPassword()
    {
        IsPasswordOverlayVisible = false;
        PasswordInput            = "";
        PasswordError            = "";
    }
}
