using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Stocklogic.ViewModels;

namespace Stocklogic.Views;

public partial class DimensionPage : UserControl
{
    public DimensionPage()
    {
        InitializeComponent();
        DataContext = new DimensionPageViewModel();
    }

    private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

}