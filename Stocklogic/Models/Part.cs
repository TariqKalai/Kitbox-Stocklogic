namespace Stocklogic.Models;

public enum StockStatus { Ok, LowStock, OutOfStock }

public class Part
{
    public int    Id       { get; set; }
    public string Kind     { get; set; } = "";
    public string Color    { get; set; } = "";
    public int    Height   { get; set; }
    public int    InStock  { get; set; }
    public int    MinStock { get; set; }

    public StockStatus Status => InStock == 0       ? StockStatus.OutOfStock
                               : InStock < MinStock ? StockStatus.LowStock
                               :                      StockStatus.Ok;

    public string DisplayName => Height > 0
        ? $"{Kind} – {Color} (H:{Height})"
        : $"{Kind} – {Color}";
}
