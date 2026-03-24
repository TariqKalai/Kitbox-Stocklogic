using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Stocklogic.Models;

namespace Stocklogic.Converters;

/// <summary>Convertit la hauteur d'un casier (cm) en pixels pour le prévisualisation.</summary>
public class LockerHeightConverter : IValueConverter
{
    public static readonly LockerHeightConverter Instance = new();
    private const double Scale = 2.5;

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is int h ? h * Scale : 80.0;

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

/// <summary>Convertit le nom de couleur d'un panneau en brush pour le fond du casier.</summary>
public class PanelColorConverter : IValueConverter
{
    public static readonly PanelColorConverter Instance = new();

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value?.ToString() switch
        {
            "Marron" => new SolidColorBrush(Color.Parse("#8D6E63")),
            _        => new SolidColorBrush(Color.Parse("#F5F0E8")),
        };

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

/// <summary>Convertit le nom de couleur d'une porte en brush.</summary>
public class DoorColorConverter : IValueConverter
{
    public static readonly DoorColorConverter Instance = new();

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value?.ToString() switch
        {
            "Marron" => new SolidColorBrush(Color.Parse("#6D4C41")),
            "Glass"  => new SolidColorBrush(Color.FromArgb(100, 176, 224, 230)),
            _        => new SolidColorBrush(Color.Parse("#EFEBE9")),
        };

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

/// <summary>Maps StockStatus to a background brush for parts list rows.</summary>
public class StockStatusColorConverter : IValueConverter
{
    public static readonly StockStatusColorConverter Instance = new();

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is StockStatus s ? s switch
        {
            StockStatus.OutOfStock => new SolidColorBrush(Color.Parse("#FFEBEE")),
            StockStatus.LowStock   => new SolidColorBrush(Color.Parse("#FFF8E1")),
            _                      => new SolidColorBrush(Colors.Transparent),
        } : new SolidColorBrush(Colors.Transparent);

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

/// <summary>Returns true when an int value is zero (used to show "no suppliers" message).</summary>
public class ZeroToBoolConverter : IValueConverter
{
    public static readonly ZeroToBoolConverter Instance = new();

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is int i && i == 0;

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

/// <summary>Maps OrderStatus to a background brush for order list rows.</summary>
public class OrderStatusColorConverter : IValueConverter
{
    public static readonly OrderStatusColorConverter Instance = new();

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is OrderStatus s ? s switch
        {
            OrderStatus.Ready     => new SolidColorBrush(Color.Parse("#E8F5E9")),
            OrderStatus.Completed => new SolidColorBrush(Color.Parse("#ECEFF1")),
            _                     => new SolidColorBrush(Color.Parse("#FFF8E1")),
        } : new SolidColorBrush(Colors.Transparent);

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

/// <summary>Maps OrderStatus to a foreground text brush.</summary>
public class OrderStatusTextColorConverter : IValueConverter
{
    public static readonly OrderStatusTextColorConverter Instance = new();

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is OrderStatus s ? s switch
        {
            OrderStatus.Ready     => new SolidColorBrush(Color.Parse("#2E7D32")),
            OrderStatus.Completed => new SolidColorBrush(Color.Parse("#546E7A")),
            _                     => new SolidColorBrush(Color.Parse("#E65100")),
        } : new SolidColorBrush(Colors.Black);

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

/// <summary>
/// Converts a bool to one of two brushes.
/// ConverterParameter must be "colorIfTrue|colorIfFalse" (hex strings).
/// </summary>
public class BoolToBrushConverter : IValueConverter
{
    public static readonly BoolToBrushConverter Instance = new();

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        bool flag = value is bool b && b;
        var parts = parameter?.ToString()?.Split('|');
        string hex = flag ? (parts?[0] ?? "#00000000") : (parts?[1] ?? "#00000000");
        return new SolidColorBrush(Color.Parse(hex));
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
