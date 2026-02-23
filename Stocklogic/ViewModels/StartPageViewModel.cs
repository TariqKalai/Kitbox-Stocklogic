using CommunityToolkit.Mvvm.Input;
using Stocklogic.Services;

namespace Stocklogic.ViewModels;

public partial class StartPageViewModel : ViewModelBase
{
    [RelayCommand]
    private void Start()
    {
        NavigationService.Navigate(new DimensionPage());
    }
}