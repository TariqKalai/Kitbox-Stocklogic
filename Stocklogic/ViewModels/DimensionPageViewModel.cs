using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Stocklogic.Services;
using Stocklogic.Views;

namespace Stocklogic.ViewModels;

public partial class DimensionPageViewModel : ViewModelBase
{
    // These properties link to {Binding Depth} and {Binding Width}
    [ObservableProperty]
    private int? _depth;

    [ObservableProperty]
    private int? _width;

    // This links to {Binding BackCommand}
    [RelayCommand]
    private void Back()
    {
        NavigationService.Navigate(new StartPage());
    }

    // This links to {Binding ContinueCommand}
    [RelayCommand]
    private void Continue()
    {
        // If depth and width have a value, we can then proceed and navigate to the next page
        if (Depth.HasValue && Width.HasValue)
        {
        }
    }
}