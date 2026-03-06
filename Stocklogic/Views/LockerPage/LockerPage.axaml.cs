using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Stocklogic.ViewModels;

namespace Stocklogic.Views;
public partial class LockerPage : UserControl
{
    public LockerPage()
    {
        InitializeComponent();
        DataContext = new LockerPageViewModel();
    }
}