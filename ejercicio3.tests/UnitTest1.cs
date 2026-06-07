using Xunit;
using ejercicio3;
using System.Linq;
using System.IO;
using Microsoft.Data.Sqlite;
using System;

namespace ejercicio3.tests
{
    public class UnitTest1 : IDisposable
    {
        private const string TestDbFile = "test_vivero_ej3.db";
        private readonly string _connectionString;

        public UnitTest1()
        {
            _connectionString = $"Data Source={TestDbFile}";
            // Setup: Limpieza previa y creación de base de datos
            if (File.Exists(TestDbFile))
            {
                File.Delete(TestDbFile);
            }
            // Utilizamos el método estático de Program para crear la estructura
            Program.CreaTabla(_connectionString);
        }

        [Fact]
        public void TestPlantaDAOCrudYLogica()
        {
            using var dao = new PlantaDAO(_connectionString);

            // 1. Create
            var planta = new Planta { NombreComun = "Pino", NombreCientifico = "Pinus", Precio = 20.0, Stock = 10 };
            dao.Create(planta);

            // 2. Count
            Assert.Equal(1, dao.Count());

            // 3. Read All
            var plantas = dao.Read().ToList();
            Assert.Single(plantas);
            Assert.Equal("Pino", plantas[0].NombreComun);
            // El Id se genera autoincremental, recuperamos el asignado
            int idGenerado = plantas[0].Id;

            // 4. Read by Id
            var plantaLeida = dao.Read(idGenerado);
            Assert.NotNull(plantaLeida);
            Assert.Equal("Pinus", plantaLeida!.NombreCientifico);

            // 5. Update
            plantaLeida.NombreComun = "Pino Carrasco";
            plantaLeida.Stock = 15;
            dao.Update(plantaLeida);

            var plantaActualizada = dao.Read(idGenerado);
            Assert.Equal("Pino Carrasco", plantaActualizada!.NombreComun);
            Assert.Equal(15, plantaActualizada.Stock);

            // 6. Inventario Valorado
            var inventario = dao.ObteneInventario().ToList();
            Assert.Single(inventario);
            // 20.0 * 15 = 300.0
            Assert.Equal(300.0, inventario[0].ValorTotal);

            // 7. Delete
            dao.Delete(idGenerado);
            Assert.Equal(0, dao.Count());
            Assert.Null(dao.Read(idGenerado));
        }

        public void Dispose()
        {
            // Cleanup
            try
            {
                SqliteConnection.ClearAllPools();
                if (File.Exists(TestDbFile))
                {
                    File.Delete(TestDbFile);
                }
            }
            catch
            {
                // Ignorar errores de limpieza
            }
        }
    }
}
