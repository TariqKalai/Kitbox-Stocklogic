using CommunityToolkit.Mvvm.Input;

namespace Stocklogic.ViewModels;

public partial class StartPageViewModel : ViewModelBase
{
    [RelayCommand]
    private void Start()
    {
        // Logic to navigate to DimensionPage
        // If you are using a ViewLocator/ReactiveUI, you'd change a 'CurrentPage' property here
    }
}