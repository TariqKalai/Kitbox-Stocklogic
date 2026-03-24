using Avalonia.Controls;
using Stocklogic.Models;
using Stocklogic.ViewModels;

namespace Stocklogic.Views;

public partial class LockerPage : UserControl
{
    public LockerPage(Cabinet cabinet)
    {
        InitializeComponent();
        DataContext = new LockerPageViewModel(cabinet);
    }
}
