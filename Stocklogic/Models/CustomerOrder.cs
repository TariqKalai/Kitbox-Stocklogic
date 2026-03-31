using System;

namespace Stocklogic.Models;

public enum OrderStatus { Pending, Ready, Completed }

public class CustomerOrder
{
    public int         Id         { get; set; }
    public DateTime    Date       { get; set; }
    public string      Email      { get; set; } = "";
    public decimal     TotalPrice { get; set; }
    public decimal     Deposit    { get; set; }
    public OrderStatus Status     { get; set; }

    public string StatusLabel => Status switch
    {
        OrderStatus.Ready     => "Ready",
        OrderStatus.Completed => "Completed",
        _                     => "Pending",
    };

    public static OrderStatus ParseStatus(string? raw) => raw?.ToLower() switch
    {
        "ready"     => OrderStatus.Ready,
        "completed" => OrderStatus.Completed,
        _           => OrderStatus.Pending,
    };
}
