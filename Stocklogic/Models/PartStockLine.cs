namespace Stocklogic.Models;

public class PartStockLine
{
    public string  PartCode       { get; set; } = "";
    public string  PartName       { get; set; } = "";
    public int     QuantityNeeded { get; set; }
    public int     InStock        { get; set; }
    public decimal UnitPrice      { get; set; }

    public bool    IsAvailable => InStock >= QuantityNeeded;
    public decimal LineTotal   => UnitPrice * QuantityNeeded;
}
