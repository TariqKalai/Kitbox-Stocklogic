using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Stocklogic.ViewModels;

namespace Stocklogic.Views;

public partial class StartPage : UserControl
{
    public StartPage()
    {
        InitializeComponent();
        DataContext = new StartPageViewModel();
    }

    private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            
        }

    

}