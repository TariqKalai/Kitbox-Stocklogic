namespace Stocklogic.Models;

public class Supplier
{
    public int     Id           { get; set; }
    public string  Name         { get; set; } = "";
    public decimal Price        { get; set; }
    public int     DeliveryDays { get; set; }
}
