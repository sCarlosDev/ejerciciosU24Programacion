using Microsoft.Data.Sqlite;

namespace ejercicio1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Ejercicio 1: Gestión de Plantas\n");
            
            using var connection = new SqliteConnection("Data Source=vivero.db");
            connection.Open();

            CreaTabla(connection);

            Console.WriteLine("Insertando datos de ejemplo...");
            InsertaPlanta(connection, "Rosa", "Rosa spp.", 10.50, 50);
            InsertaPlanta(connection, "Lirio", "Lilium", 7.80, 80);
            InsertaPlanta(connection, "Tulipán", "Tulipa", 5.25, 100);
            InsertaPlanta(connection, "Orquídea", "Orchidaceae", 25.00, 20);
            
            Console.WriteLine("Datos insertados correctamente.");
        }

        public static void CreaTabla(SqliteConnection connection)
        {
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

        public static void InsertaPlanta(SqliteConnection connection, string nombreComun, string nombreCientifico, double precio, int stock)
        {
            var sql = @"
                INSERT INTO planta (nombre_comun, nombre_cientifico, precio, stock)
                VALUES (@nombreComun, @nombreCientifico, @precio, @stock);";

            using var command = new SqliteCommand(sql, connection);
            command.Parameters.AddWithValue("@nombreComun", nombreComun);
            command.Parameters.AddWithValue("@nombreCientifico", nombreCientifico ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@precio", precio);
            command.Parameters.AddWithValue("@stock", stock);
            
            command.ExecuteNonQuery();
            Console.WriteLine($"Insertando -> {nombreComun}, {nombreCientifico}, {precio}, {stock}");
        }
    }
}
