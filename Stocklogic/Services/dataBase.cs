using MySql.Data.MySqlClient;
using CommunityToolkit.Mvvm.Input;
using Stocklogic.Views;
using Stocklogic.Services;
using System;

public static class DataBaseService
{
    public static void UpdateStock()
    {
        // Chaîne de connexion directe
        string conn = "server=pat.infolab.ecam.be;port=63317;database=stocklogic;uid=stocklogic;pwd=1234Azer;";

        using (var connection = new MySqlConnection(conn))
        {
            connection.Open();

            // Requête "brute" (Hardcoded)
            // On écrit les chiffres directement dans le texte
            string sql = "UPDATE products SET stock_count = 10 WHERE id = 1";

            var cmd = new MySqlCommand(sql, connection);
            cmd.ExecuteNonQuery();

            // On change de page direct après
            NavigationService.Navigate(new DimensionPage());
        }
    }

    public static void ReadProductsByHeight()
    {
        string connString = "server=pat.infolab.ecam.be;user=stocklogic;database=Stocklogic_dB;port=63317;password=1234Azer";

        using (MySqlConnection connection = new MySqlConnection(connString))
        {
            try
            {
                connection.Open();

                // Requête pour lire les éléments avec height = 50
                string sql = "SELECT id, Kind, Color, In_stock FROM products WHERE Height = 50";

                using (MySqlCommand cmd = new MySqlCommand(sql, connection))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        // On boucle tant qu'il y a des lignes qui correspondent
                        while (reader.Read())
                        {
                            // On récupère les données par nom de colonne
                            var id = reader["id"];
                            var name = reader["kind"];
                            var stock = reader["In_stock"];
                            var color = reader["Color"];

                            Console.WriteLine($"Produit trouvé - ID: {id}, Nom: {name}, Stock: {stock}, Color {color}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur de lecture : " + ex.Message);
            }
        }
    }
}