using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Stocklogic.ViewModels;

public partial class DimensionPageViewModel : ViewModelBase
{
    // These properties link to {Binding Depth} and {Binding Width}
    // The CommunityToolkit will automatically generate "Depth" and "Width" properties
    [ObservableProperty]
    private string? _depth;

    [ObservableProperty]
    private string? _width;

    // This links to {Binding BackCommand}
    [RelayCommand]
    private void Back()
    {
        // Add your navigation logic here to go to the previous screen
    }

    // This links to {Binding ContinueCommand}
    [RelayCommand]
    private void Continue()
    {
        // Add logic to save the dimensions and move to the next screen
        if (!string.IsNullOrEmpty(Depth) && !string.IsNullOrEmpty(Width))
        {
            // Transition logic...
        }
    }
}