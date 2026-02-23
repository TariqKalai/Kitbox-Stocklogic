using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Stocklogic.Views;

public partial class DimensionPage : Window
{
    public DimensionPage()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

}