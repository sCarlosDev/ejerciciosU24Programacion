using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;

namespace ejercicio3
{
    public class PlantaDAO : IDisposable
    {
        private readonly SqliteConnection _conexion;

        public PlantaDAO(string connectionString)
        {
            _conexion = new SqliteConnection(connectionString);
            _conexion.Open();
        }

        public void Create(Planta p)
        {
            var sql = @"
                INSERT INTO planta (nombre_comun, nombre_cientifico, precio, stock) 
                VALUES (@nombre, @cientifico, @precio, @stock);
                SELECT last_insert_rowid();";
            
            using var command = new SqliteCommand(sql, _conexion);
            command.Parameters.AddWithValue("@nombre", p.NombreComun);
            command.Parameters.AddWithValue("@cientifico", p.NombreCientifico ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@precio", p.Precio);
            command.Parameters.AddWithValue("@stock", p.Stock);

            p.Id = Convert.ToInt32(command.ExecuteScalar());
        }

        public IEnumerable<Planta> Read()
        {
            var sql = "SELECT id, nombre_comun, nombre_cientifico, precio, stock FROM planta";
            using var command = new SqliteCommand(sql, _conexion);
            using var reader = command.ExecuteReader();
            
            while (reader.Read())
            {
                yield return new Planta
                {
                    Id = reader.GetInt32(0),
                    NombreComun = reader.GetString(1),
                    NombreCientifico = reader.IsDBNull(2) ? null : reader.GetString(2),
                    Precio = reader.GetDouble(3),
                    Stock = reader.GetInt32(4)
                };
            }
        }

        public Planta Read(int id)
        {
            var sql = "SELECT id, nombre_comun, nombre_cientifico, precio, stock FROM planta WHERE id = @id";
            using var command = new SqliteCommand(sql, _conexion);
            command.Parameters.AddWithValue("@id", id);
            using var reader = command.ExecuteReader();
            
            if (reader.Read())
            {
                return new Planta
                {
                    Id = reader.GetInt32(0),
                    NombreComun = reader.GetString(1),
                    NombreCientifico = reader.IsDBNull(2) ? null : reader.GetString(2),
                    Precio = reader.GetDouble(3),
                    Stock = reader.GetInt32(4)
                };
            }
            return null;
        }

        public void Update(Planta p)
        {
            var existente = Read(p.Id);
            if (existente == null) return;

            var sql = "UPDATE planta SET nombre_comun = @nombre, nombre_cientifico = @cientifico, precio = @precio, stock = @stock WHERE id = @id";
            using var command = new SqliteCommand(sql, _conexion);
            command.Parameters.AddWithValue("@id", p.Id);
            command.Parameters.AddWithValue("@nombre", p.NombreComun);
            command.Parameters.AddWithValue("@cientifico", p.NombreCientifico ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@precio", p.Precio);
            command.Parameters.AddWithValue("@stock", p.Stock);
            
            command.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            var existente = Read(id);
            if (existente == null) return;

            var sql = "DELETE FROM planta WHERE id = @id";
            using var command = new SqliteCommand(sql, _conexion);
            command.Parameters.AddWithValue("@id", id);
            command.ExecuteNonQuery();
        }

        public int Count()
        {
            var sql = "SELECT COUNT(*) FROM planta";
            using var command = new SqliteCommand(sql, _conexion);
            return Convert.ToInt32(command.ExecuteScalar());
        }

        public IEnumerable<PlantaStock> ObteneInventario()
        {
            var sql = "SELECT nombre_comun, stock, precio * stock as valor FROM planta";
            using var command = new SqliteCommand(sql, _conexion);
            using var reader = command.ExecuteReader();
            
            var result = new List<PlantaStock>();
            while (reader.Read())
            {
                result.Add(new PlantaStock(
                    reader.GetString(0), 
                    reader.GetInt32(1), 
                    reader.GetDouble(2)
                ));
            }
            return result;
        }

        public void Dispose()
        {
            _conexion?.Close();
            _conexion?.Dispose();
        }
    }
}