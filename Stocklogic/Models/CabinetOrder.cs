using System.Collections.Generic;
using Stocklogic.Models;

namespace Stocklogic.Models;

public class CabinetOrder
{
    public List<Locker> Lockers { get; }
    public Cabinet      Cabinet { get; }

    public CabinetOrder(List<Locker> lockers, Cabinet cabinet)
    {
        Lockers = new List<Locker>(lockers);
        Cabinet = cabinet;
    }
}