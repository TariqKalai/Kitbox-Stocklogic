using System;
using System.Collections.Generic;
using System.Linq;
using Stocklogic.Models;

namespace Stocklogic.Services;

/// <summary>
/// Calcule la liste des pièces (code + quantité) nécessaires pour un cabinet configuré.
/// </summary>
public static class CabinetComposerService
{
    private static readonly Dictionary<int, int> LockerHeightToVba = new()
    {
        { 32, 27 },
        { 42, 37 },
        { 52, 47 }
    };

    private static readonly int[] CutSizes =
    {
        50, 75, 100, 125, 150, 175, 200, 225, 250, 275, 300, 325, 350, 375
    };

    public static IReadOnlyList<int>    ValidLockerHeights { get; } = [32, 42, 52];
    public static IReadOnlyList<int>    ValidDepths        { get; } = [32, 42, 52, 62];
    public static IReadOnlyList<int>    ValidWidths        { get; } = [32, 42, 52, 62, 80, 100, 120];
    public static IReadOnlyList<int>    ValidWidthsWithDoor{ get; } = [32, 42, 52, 62];
    public static IReadOnlyList<string> PanelColors        { get; } = ["White", "Marron"];
    public static IReadOnlyList<string> DoorColors         { get; } = ["White", "Marron", "Glass"];
    public static IReadOnlyList<string> AngleIronColors    { get; } = ["White", "Marron", "Galva", "Black"];

    /// <summary>Retourne les pièces nécessaires pour le cabinet complet (code, quantité).</summary>
    public static List<(string PartCode, int Quantity)> ComposeCabinet(Cabinet cabinet)
    {
        var result = new Dictionary<string, int>();

        foreach (var locker in cabinet.Lockers)
            foreach (var (code, qty) in GetLockerParts(locker))
                result[code] = result.GetValueOrDefault(code) + qty;

        var (angCode, angQty) = GetAngleIronParts(cabinet);
        result[angCode] = result.GetValueOrDefault(angCode) + angQty;

        return result.Select(kv => (kv.Key, kv.Value)).ToList();
    }

    /// <summary>Retourne les pièces nécessaires pour un seul casier.</summary>
    public static IEnumerable<(string PartCode, int Quantity)> GetLockerParts(Locker locker)
    {
        if (!LockerHeightToVba.TryGetValue(locker.Height, out int vbaHeight))
            throw new ArgumentException($"Hauteur de casier invalide : {locker.Height}. Valeurs valides : 32, 42, 52.");

        string col = ColorCode(locker.Color);

        yield return ($"VBA{vbaHeight}", 4);
        yield return ($"FCB{locker.Width}", 2);
        yield return ($"BCB{locker.Width}", 2);
        yield return ($"TRG{locker.Depth}", 4);
        yield return ($"BTP{locker.Depth}{locker.Width}{col}", 2);
        yield return ($"LRP{locker.Height}{locker.Depth}{col}", 2);
        yield return ($"BKP{locker.Height}{locker.Width}{col}", 1);

        if (locker.HasDoors)
        {
            string doorCol = ColorCode(locker.DoorColor);
            yield return ($"DOR{locker.Height}{locker.Width}{doorCol}", 2);
            if (locker.DoorColor != "Glass")
                yield return ("CUPHANDLE", 2);
        }
    }

    /// <summary>Retourne le code et la quantité des cornières pour le cabinet entier.</summary>
    public static (string PartCode, int Quantity) GetAngleIronParts(Cabinet cabinet)
    {
        int totalHeight = cabinet.Lockers.Sum(l => l.Height + 4);
        string col = ColorCode(cabinet.AngleIronColor);
        int cutSize = CutSizes.FirstOrDefault(s => s >= totalHeight);
        if (cutSize == 0)
            throw new InvalidOperationException($"Cabinet trop haut ({totalHeight} cm). Maximum supporté : {CutSizes.Last()} cm.");
        return ($"ANG{cutSize}{col}CUT", 4);
    }

    private static string ColorCode(string? color) => color switch
    {
        "White"  or "WH" => "WH",
        "Marron" or "MA" => "MA",
        "Galva"  or "GA" => "GA",
        "Black"  or "BL" => "BL",
        "Glass"  or "GL" => "GL",
        _ => "WH"
    };
}
