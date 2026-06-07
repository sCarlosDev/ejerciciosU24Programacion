using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ejercicio2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Ejercicio 2: Gestión de Rutinas de Gimnasio\n");

            string connectionString = "Data Source=gimnasio.db";

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                CreaTabla(connection);
            }

            Console.WriteLine("Creando e insertando ejercicios...");
            using (var dao = new EjercicioDAO(connectionString))
            {
                dao.Crea(new Ejercicio(0, "Press de Banca", new List<Musculo> { Musculo.Pecho }));
                dao.Crea(new Ejercicio(0, "Flexiones", new List<Musculo> { Musculo.Pecho, Musculo.Triceps }));
                dao.Crea(new Ejercicio(0, "Sentadillas", new List<Musculo> { Musculo.Cuadriceps, Musculo.Gluteos }));
                dao.Crea(new Ejercicio(0, "Dominadas", new List<Musculo> { Musculo.Espalda, Musculo.Biceps }));

                Console.WriteLine("\nEjercicios que trabajan el músculo 'Pecho':");
                var ejerciciosPecho = dao.ObtenEjerciciosPorMusculo(Musculo.Pecho);
                foreach (var nombre in ejerciciosPecho.NombresEjercicios)
                {
                    Console.WriteLine($"- {nombre}");
                }

                Console.WriteLine("\nActualizando 'Flexiones' a 'Flexiones Diamante'...");
                var flexiones = dao.Lee().FirstOrDefault(e => e.Nombre == "Flexiones");
                if (flexiones != null)
                {
                    dao.Actualiza(flexiones with { Nombre = "Flexiones Diamante" });
                }

                Console.WriteLine("Eliminando 'Press de Banca'...");
                var pressBanca = dao.Lee().FirstOrDefault(e => e.Nombre == "Press de Banca");
                if (pressBanca != null)
                {
                    dao.Elimina(pressBanca.Id);
                }

                Console.WriteLine("\nListado final de ejercicios:");
                var todos = dao.Lee();
                foreach (var ej in todos)
                {
                    string musculos = string.Join(", ", ej.GruposMusculares);
                    Console.WriteLine($"- {ej.Nombre} (Músculos: {musculos})");
                }
            }

            Console.WriteLine("\nOperaciones completadas.");
        }

        public static void CreaTabla(SqliteConnection conexion)
        {
            var sql = @"
                CREATE TABLE IF NOT EXISTS ejercicios (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    nombre TEXT NOT NULL,
                    grupos_musculares TEXT NOT NULL
                );";

            using var command = new SqliteCommand(sql, conexion);
            command.ExecuteNonQuery();
        }
    }
}
