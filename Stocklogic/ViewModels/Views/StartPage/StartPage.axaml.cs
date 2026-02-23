using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Stocklogic.Views;

public partial class StartPage : Window
{
    public StartPage()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

    

}