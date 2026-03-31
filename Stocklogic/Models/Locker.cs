namespace Stocklogic.Models;

public class Locker
{
    public int Height { get; set; }
    public int Depth { get; set; }
    public int Width { get; set; }
    public string Color { get; set; } = "White";
    public bool HasDoors { get; set; }
    public string DoorColor { get; set; } = "White";

    public string DisplayName => $"{Width}×{Depth}×{Height} cm — {Color}" +
        (HasDoors ? $" + {DoorColor} doors" : string.Empty);
}
