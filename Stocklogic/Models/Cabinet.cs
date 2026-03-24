using System.Collections.Generic;
using System.Linq;

namespace Stocklogic.Models;

public class Cabinet
{
    public int Depth { get; set; }
    public int Width { get; set; }
    public string AngleIronColor { get; set; } = "White";
    public List<Locker> Lockers { get; } = [];

    public bool CanAddLocker   => Lockers.Count < 7;
    public int AngleIronHeight => Lockers.Sum(l => l.Height + 4);
}
