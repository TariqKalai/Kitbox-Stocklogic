using Avalonia.Controls;

namespace Stocklogic.Services;

public static class NavigationService
{
    public static ContentControl? PageHost;

    //simply changes the pagehsot
    public static void Navigate(Control page)
    {
        if (PageHost != null)
            PageHost.Content = page;
    }
}