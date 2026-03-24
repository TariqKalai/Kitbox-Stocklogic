using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

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
