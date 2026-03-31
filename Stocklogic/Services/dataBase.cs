using MySql.Data.MySqlClient;
using Stocklogic.Views;
using Stocklogic.Services;
using Stocklogic.Models;
using System;
using System.Collections.Generic;
using System.Linq;

public static class DataBaseService
{
    private const string ConnString =
        "server=pat.infolab.ecam.be;port=63317;database=Stocklogic_dB;uid=stocklogic;pwd=1234Azer;";

    // Hardcoded supplier names matching fournisseurs.txt
    private static readonly string[] SupplierNames = { "TRABELBO SA", "TraitBois SPRL" };

    public static void UpdateStock()
    {
        using var connection = new MySqlConnection(ConnString);
        connection.Open();
        string sql = "UPDATE Part SET In_stock = 10 WHERE PK_ID_Part = 1";
        var cmd = new MySqlCommand(sql, connection);
        cmd.ExecuteNonQuery();
        NavigationService.Navigate(new DimensionPage());
    }

    public static void ReadProductsByHeight()
    {
        using var connection = new MySqlConnection(ConnString);
        try
        {
            connection.Open();
            string sql = "SELECT PK_ID_Part, Kind, Color, In_stock FROM Part WHERE Height = 50";
            using var cmd = new MySqlCommand(sql, connection);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Console.WriteLine($"Produit trouvé - ID: {reader["PK_ID_Part"]}, Nom: {reader["Kind"]}, " +
                                  $"Stock: {reader["In_stock"]}, Color: {reader["Color"]}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Erreur de lecture : " + ex.Message);
        }
    }

    // ─── Stock Manager ───────────────────────────────────────────────────────

    public static List<Part> GetAllParts()
    {
        var parts = new List<Part>();
        try
        {
            using var connection = new MySqlConnection(ConnString);
            connection.Open();

            string sql = @"SELECT PK_ID_Part, Kind, Color, Height,
                                  IFNULL(In_stock, 0)      AS In_stock,
                                  IFNULL(Minimal_stock, 0) AS Minimal_stock
                           FROM Part
                           ORDER BY Kind, Color";

            using var cmd    = new MySqlCommand(sql, connection);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                parts.Add(new Part
                {
                    Id       = Convert.ToInt32(reader["PK_ID_Part"]),
                    Kind     = reader["Kind"]?.ToString()  ?? "",
                    Color    = reader["Color"]?.ToString() ?? "",
                    Height   = reader["Height"]   == DBNull.Value ? 0 : Convert.ToInt32(reader["Height"]),
                    InStock  = reader["In_stock"]  == DBNull.Value ? 0 : Convert.ToInt32(reader["In_stock"]),
                    MinStock = reader["Minimal_stock"] == DBNull.Value ? 0 : Convert.ToInt32(reader["Minimal_stock"]),
                });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("GetAllParts error: " + ex.Message);
        }
        return parts;
    }

    /// <summary>
    /// Builds supplier list from the Supplier1/Supplier2 columns embedded in the Part row.
    /// Sorted by price (then delivery days) — first entry is the recommended supplier.
    /// </summary>
    public static List<Supplier> GetSuppliersForPart(int partId)
    {
        var suppliers = new List<Supplier>();
        try
        {
            using var connection = new MySqlConnection(ConnString);
            connection.Open();

            string sql = @"SELECT Supplier1_price, Supplier1_shipping_time,
                                  Supplier2_price, Supplier2_shipping_time
                           FROM Part WHERE PK_ID_Part = @id";

            using var cmd = new MySqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@id", partId);
            using var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                for (int i = 0; i < 2; i++)
                {
                    string priceCol    = $"Supplier{i + 1}_price";
                    string deliveryCol = $"Supplier{i + 1}_shipping_time";

                    if (reader[priceCol] == DBNull.Value) continue;

                    suppliers.Add(new Supplier
                    {
                        Id           = i + 1,
                        Name         = SupplierNames[i],
                        Price        = Convert.ToDecimal(reader[priceCol]),
                        DeliveryDays = reader[deliveryCol] == DBNull.Value
                                       ? 0 : Convert.ToInt32(reader[deliveryCol]),
                    });
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("GetSuppliersForPart error: " + ex.Message);
        }

        // Sort: best price first, then best delivery time
        suppliers.Sort((a, b) =>
        {
            int cmp = a.Price.CompareTo(b.Price);
            return cmp != 0 ? cmp : a.DeliveryDays.CompareTo(b.DeliveryDays);
        });

        return suppliers;
    }

    public static bool UpdatePartStock(int partId, int newStock)
    {
        try
        {
            using var connection = new MySqlConnection(ConnString);
            connection.Open();
            string sql = "UPDATE Part SET In_stock = @stock WHERE PK_ID_Part = @id";
            using var cmd = new MySqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@stock", newStock);
            cmd.Parameters.AddWithValue("@id",    partId);
            cmd.ExecuteNonQuery();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("UpdatePartStock error: " + ex.Message);
            return false;
        }
    }

    public static bool UpdatePartMinStock(int partId, int newMinStock)
    {
        try
        {
            using var connection = new MySqlConnection(ConnString);
            connection.Open();
            string sql = "UPDATE Part SET Minimal_stock = @min WHERE PK_ID_Part = @id";
            using var cmd = new MySqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@min", newMinStock);
            cmd.Parameters.AddWithValue("@id",  partId);
            cmd.ExecuteNonQuery();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("UpdatePartMinStock error: " + ex.Message);
            return false;
        }
    }

    // ─── Orders ──────────────────────────────────────────────────────────────

    public static List<CustomerOrder> GetAllOrders()
    {
        var orders = new List<CustomerOrder>();
        try
        {
            using var connection = new MySqlConnection(ConnString);
            connection.Open();

            string sql = @"SELECT PK_ID_order, Date, Email,
                                  IFNULL(Total_price, 0) AS Total_price,
                                  IFNULL(Deposit, 0)     AS Deposit,
                                  Status
                           FROM `Customer Order`
                           ORDER BY Date DESC";

            using var cmd    = new MySqlCommand(sql, connection);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                orders.Add(new CustomerOrder
                {
                    Id         = Convert.ToInt32(reader["PK_ID_order"]),
                    Date       = reader["Date"] == DBNull.Value
                                 ? DateTime.Today : Convert.ToDateTime(reader["Date"]),
                    Email      = reader["Email"]?.ToString() ?? "",
                    TotalPrice = Convert.ToDecimal(reader["Total_price"]),
                    Deposit    = Convert.ToDecimal(reader["Deposit"]),
                    Status     = CustomerOrder.ParseStatus(reader["Status"]?.ToString()),
                });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("GetAllOrders error: " + ex.Message);
        }
        return orders;
    }

    /// <summary>
    /// Returns stock info and price for each requested part code.
    /// Key = Code, Value = (InStock, CustomerPrice).
    /// Codes not found in DB are simply absent from the result.
    /// </summary>
    public static Dictionary<string, (int InStock, decimal Price)> GetPartsByCode(IEnumerable<string> codes)
    {
        var result = new Dictionary<string, (int, decimal)>();
        try
        {
            using var connection = new MySqlConnection(ConnString);
            connection.Open();

            string inClause = string.Join(",", codes.Select((_, i) => $"@c{i}"));
            string sql = $"SELECT Code, IFNULL(In_stock,0) AS In_stock, " +
                         $"IFNULL(Customer_price,0) AS Customer_price " +
                         $"FROM Part WHERE Code IN ({inClause})";

            using var cmd = new MySqlCommand(sql, connection);
            int idx = 0;
            foreach (var code in codes)
                cmd.Parameters.AddWithValue($"@c{idx++}", code);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                string code     = reader["Code"]?.ToString() ?? "";
                int    inStock  = Convert.ToInt32(reader["In_stock"]);
                decimal price   = Convert.ToDecimal(reader["Customer_price"]);
                result[code]    = (inStock, price);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("GetPartsByCode error: " + ex.Message);
        }
        return result;
    }

    public static int CreateOrder(string email, decimal totalPrice, decimal deposit, string status)
    {
        try
        {
            using var connection = new MySqlConnection(ConnString);
            connection.Open();

            string sql = "INSERT INTO `Customer Order` (Date, Email, Total_price, Deposit, Status) " +
                         "VALUES (CURDATE(), @email, @total, @deposit, @status)";
            using var cmd = new MySqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@email",   email);
            cmd.Parameters.AddWithValue("@total",   totalPrice);
            cmd.Parameters.AddWithValue("@deposit", deposit);
            cmd.Parameters.AddWithValue("@status",  status);
            cmd.ExecuteNonQuery();
            return (int)cmd.LastInsertedId;
        }
        catch (Exception ex)
        {
            Console.WriteLine("CreateOrder error: " + ex.Message);
            return -1;
        }
    }

    public static void DecrementStock(Dictionary<string, int> partQuantities)
    {
        try
        {
            using var connection = new MySqlConnection(ConnString);
            connection.Open();
            foreach (var (code, qty) in partQuantities)
            {
                string sql = "UPDATE Part SET In_stock = In_stock - @qty WHERE Code = @code";
                using var cmd = new MySqlCommand(sql, connection);
                cmd.Parameters.AddWithValue("@qty",  qty);
                cmd.Parameters.AddWithValue("@code", code);
                cmd.ExecuteNonQuery();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("DecrementStock error: " + ex.Message);
        }
    }

    public static bool UpdateOrderStatus(int orderId, string newStatus)
    {
        try
        {
            using var connection = new MySqlConnection(ConnString);
            connection.Open();
            string sql = "UPDATE `Customer Order` SET Status = @status WHERE PK_ID_order = @id";
            using var cmd = new MySqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@status", newStatus);
            cmd.Parameters.AddWithValue("@id",     orderId);
            cmd.ExecuteNonQuery();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("UpdateOrderStatus error: " + ex.Message);
            return false;
        }
    }
}
