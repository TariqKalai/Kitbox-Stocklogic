using System.Collections.Generic;
using Stocklogic.Models;

namespace Stocklogic.Services;

public static class OrderSessionService
{
    private static readonly List<CabinetOrder> _orders = new();

    public static IReadOnlyList<CabinetOrder> Orders => _orders;

    public static void AddOrder(CabinetOrder order) => _orders.Add(order);

    public static void Clear() => _orders.Clear();
}