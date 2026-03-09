using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Stocklogic.ViewModels;

namespace Stocklogic.Views;

public partial class SummaryPage : UserControl
{
    public SummaryPage()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
}