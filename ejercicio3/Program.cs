using System;
using System.IO;
using System.Text.Json;
using Microsoft.Data.Sqlite;
using System.Linq;

namespace ejercicio3
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Patrón DAO (Data Access Object)\n");

            string connectionString = "Data Source=vivero_ej3.db";
            CreaTabla(connectionString);

            string jsonPath = "vivero.json";
            if (!File.Exists(jsonPath))
            {
                jsonPath = Path.Combine("..", "vivero.json");
            }

            string jsonString = File.ReadAllText(jsonPath);
            Vivero vivero = JsonSerializer.Deserialize<Vivero>(jsonString);

            using (PlantaDAO dao = new PlantaDAO(connectionString))
            {
                if (dao.Count() == 0)
                {
                    foreach (var p in vivero.Plantas)
                    {
                        dao.Create(p);
                    }
                }

                // Manipulación
                var plantas = dao.Read().ToList();
                if (plantas.Count > 0)
                {
                    var primera = plantas[0];
                    primera.Stock += 10;
                    dao.Update(primera);
                }

                if (plantas.Count > 1)
                {
                    var segunda = plantas[1];
                    dao.Delete(segunda.Id);
                }

                // Informe Final
                Console.WriteLine($"Vivero: {vivero.Nombre}");
                Console.WriteLine($"Dirección: {vivero.Direccion}");
                Console.WriteLine("Inventario Valorado:");
                
                var inventario = dao.ObteneInventario();
                foreach (var item in inventario)
                {
                    Console.WriteLine($"{item.Nombre_Comun}: {item.Stock} unidades - Valor Total: {item.ValorTotal:C}");
                }
            }

            Console.WriteLine("\nOperaciones completadas.");
        }

        public static void CreaTabla(string connectionString)
        {
            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            var sql = @"
                CREATE TABLE IF NOT EXISTS planta (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    nombre_comun TEXT NOT NULL,
                    nombre_cientifico TEXT,
                    precio REAL,
                    stock INTEGER
                );";

            using var command = new SqliteCommand(sql, connection);
            command.ExecuteNonQuery();
        }
    }
}

