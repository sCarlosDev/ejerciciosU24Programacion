using Microsoft.Data.Sqlite;
using Xunit;
using ejercicio1;

namespace ejercicio1.tests
{
    public class UnitTest1
    {
        [Fact]
        public void TestCreaTablaEInsertaPlanta()
        {
            // Usamos una base de datos en memoria para el test
            using var connection = new SqliteConnection("Data Source=:memory:");
            connection.Open();

            // 1. Probamos que se crea la tabla correctamente
            Program.CreaTabla(connection);

            // Verificamos que la tabla existe consultando sqlite_master
            using (var command = new SqliteCommand("SELECT name FROM sqlite_master WHERE type='table' AND name='planta';", connection))
            {
                var tableName = command.ExecuteScalar() as string;
                Assert.Equal("planta", tableName);
            }

            // 2. Probamos la inserción de una planta
            Program.InsertaPlanta(connection, "TestPlanta", "TestSpp", 99.99, 10);

            // Verificamos que el registro se ha insertado
            using (var command = new SqliteCommand("SELECT nombre_comun, nombre_cientifico, precio, stock FROM planta WHERE nombre_comun='TestPlanta';", connection))
            using (var reader = command.ExecuteReader())
            {
                Assert.True(reader.Read(), "Debería existir un registro con nombre 'TestPlanta'");
                Assert.Equal("TestPlanta", reader.GetString(0));
                Assert.Equal("TestSpp", reader.GetString(1));
                Assert.Equal(99.99, reader.GetDouble(2));
                Assert.Equal(10, reader.GetInt32(3));
            }
        }
    }
}
