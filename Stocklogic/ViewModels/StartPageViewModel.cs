using System;
using CommunityToolkit.Mvvm.Input;
using Stocklogic.Services;
using Stocklogic.Views;

namespace Stocklogic.ViewModels;

public partial class StartPageViewModel : ViewModelBase
{
    [RelayCommand]
    private void Start()
    {
        // This triggers your static service to swap the content
        NavigationService.Navigate(new DimensionPage());
        Console.WriteLine("Start button clicked, navigating to DimensionPage.");
    }
}