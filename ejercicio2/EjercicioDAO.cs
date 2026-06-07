using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ejercicio2
{
    public class EjercicioDAO : IDisposable
    {
        private readonly SqliteConnection _conexion;

        public EjercicioDAO(string connectionString)
        {
            _conexion = new SqliteConnection(connectionString);
            _conexion.Open();
        }

        public void Crea(Ejercicio ejercicio)
        {
            var sql = "INSERT INTO ejercicios (nombre, grupos_musculares) VALUES (@nombre, @grupos)";
            using var command = new SqliteCommand(sql, _conexion);
            command.Parameters.AddWithValue("@nombre", ejercicio.Nombre);
            string grupos = string.Join(",", ejercicio.GruposMusculares);
            command.Parameters.AddWithValue("@grupos", grupos);
            command.ExecuteNonQuery();
        }

        public IEnumerable<Ejercicio> Lee()
        {
            var sql = "SELECT id, nombre, grupos_musculares FROM ejercicios";
            using var command = new SqliteCommand(sql, _conexion);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var id = reader.GetInt32(0);
                var nombre = reader.GetString(1);
                var gruposStr = reader.GetString(2);
                
                var grupos = new List<Musculo>();
                if (!string.IsNullOrEmpty(gruposStr))
                {
                    grupos = gruposStr.Split(',')
                                      .Select(e => Enum.Parse<Musculo>(e.Trim()))
                                      .ToList();
                }

                yield return new Ejercicio(id, nombre, grupos);
            }
        }

        public void Actualiza(Ejercicio ejercicio)
        {
            var sql = "UPDATE ejercicios SET nombre = @nombre, grupos_musculares = @grupos WHERE id = @id";
            using var command = new SqliteCommand(sql, _conexion);
            command.Parameters.AddWithValue("@id", ejercicio.Id);
            command.Parameters.AddWithValue("@nombre", ejercicio.Nombre);
            string grupos = string.Join(",", ejercicio.GruposMusculares);
            command.Parameters.AddWithValue("@grupos", grupos);
            command.ExecuteNonQuery();
        }

        public void Elimina(int id)
        {
            var sql = "DELETE FROM ejercicios WHERE id = @id";
            using var command = new SqliteCommand(sql, _conexion);
            command.Parameters.AddWithValue("@id", id);
            command.ExecuteNonQuery();
        }

        public EjerciciosPorMusculo ObtenEjerciciosPorMusculo(Musculo musculo)
        {
            var sql = "SELECT nombre FROM ejercicios WHERE grupos_musculares LIKE @musculo";
            using var command = new SqliteCommand(sql, _conexion);
            command.Parameters.AddWithValue("@musculo", $"%{musculo}%");
            using var reader = command.ExecuteReader();

            var nombres = new List<string>();
            while (reader.Read())
            {
                nombres.Add(reader.GetString(0));
            }

            return new EjerciciosPorMusculo(musculo, nombres);
        }

        public void Dispose()
        {
            _conexion?.Close();
            _conexion?.Dispose();
        }
    }
}