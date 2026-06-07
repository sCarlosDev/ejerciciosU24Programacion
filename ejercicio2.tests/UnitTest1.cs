using Microsoft.Data.Sqlite;
using Xunit;
using ejercicio2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ejercicio2.tests
{
	public sealed class EjercicioDAOTests : IDisposable
	{
		private readonly string _dbPath;
		private readonly string _connectionString;

		public EjercicioDAOTests()
		{
			_dbPath = Path.Combine(Path.GetTempPath(), $"gimnasio_test_{Guid.NewGuid():N}.db");
			_connectionString = $"Data Source={_dbPath};Pooling=False";

			using var connection = new SqliteConnection(_connectionString);
			connection.Open();
			Program.CreaTabla(connection);
		}

		public void Dispose()
		{
			try
			{
				SqliteConnection.ClearAllPools();
				if (File.Exists(_dbPath))
				{
					File.Delete(_dbPath);
				}
			}
			catch (IOException)
			{
				// Si el fichero está bloqueado por el sistema/pooling, no hacemos fallar el test.
			}
		}

		[Fact]
		public void CreaYLee_GuardaYRecuperaEjercicios()
		{
			using var dao = new EjercicioDAO(_connectionString);

			dao.Crea(new Ejercicio(0, "Press de Banca", new List<Musculo> { Musculo.Pecho }));
			dao.Crea(new Ejercicio(0, "Sentadillas", new List<Musculo> { Musculo.Cuadriceps, Musculo.Gluteos }));

			var ejercicios = dao.Lee().ToList();

			Assert.Equal(2, ejercicios.Count);
			Assert.Contains(ejercicios, e => e.Nombre == "Press de Banca" && e.GruposMusculares.SequenceEqual(new[] { Musculo.Pecho }));
			Assert.Contains(ejercicios, e => e.Nombre == "Sentadillas" && e.GruposMusculares.SequenceEqual(new[] { Musculo.Cuadriceps, Musculo.Gluteos }));
		}

		[Fact]
		public void Actualiza_ModificaEjercicioExistente()
		{
			using var dao = new EjercicioDAO(_connectionString);

			dao.Crea(new Ejercicio(0, "Flexiones", new List<Musculo> { Musculo.Pecho, Musculo.Triceps }));
			var creado = dao.Lee().Single(e => e.Nombre == "Flexiones");

			dao.Actualiza(creado with
			{
				Nombre = "Flexiones Diamante",
				GruposMusculares = new List<Musculo> { Musculo.Pecho, Musculo.Triceps }
			});

			var actualizado = dao.Lee().Single(e => e.Id == creado.Id);
			Assert.Equal("Flexiones Diamante", actualizado.Nombre);
			Assert.True(actualizado.GruposMusculares.SequenceEqual(new[] { Musculo.Pecho, Musculo.Triceps }));
		}

		[Fact]
		public void Elimina_BorraEjercicioPorId()
		{
			using var dao = new EjercicioDAO(_connectionString);

			dao.Crea(new Ejercicio(0, "Dominadas", new List<Musculo> { Musculo.Espalda, Musculo.Biceps }));
			var creado = dao.Lee().Single();

			dao.Elimina(creado.Id);
			var ejercicios = dao.Lee().ToList();

			Assert.Empty(ejercicios);
		}

		[Fact]
		public void ObtenEjerciciosPorMusculo_DevuelveNombresQueContienenElMusculo()
		{
			using var dao = new EjercicioDAO(_connectionString);

			dao.Crea(new Ejercicio(0, "Press de Banca", new List<Musculo> { Musculo.Pecho }));
			dao.Crea(new Ejercicio(0, "Flexiones", new List<Musculo> { Musculo.Pecho, Musculo.Triceps }));
			dao.Crea(new Ejercicio(0, "Sentadillas", new List<Musculo> { Musculo.Cuadriceps, Musculo.Gluteos }));

			var dto = dao.ObtenEjerciciosPorMusculo(Musculo.Pecho);

			Assert.Equal(Musculo.Pecho, dto.Musculo);
			Assert.Equal(2, dto.NombresEjercicios.Count);
			Assert.Contains("Press de Banca", dto.NombresEjercicios);
			Assert.Contains("Flexiones", dto.NombresEjercicios);
			Assert.DoesNotContain("Sentadillas", dto.NombresEjercicios);
		}
	}
}
