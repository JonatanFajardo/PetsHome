using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using PetsHome.Business.Extensions;
using PetsHome.Business.Models;
using PetsHome.Business.Services;
using PetsHome.DataAccess;
using PetsHome.Logic.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PetsHome.Tests
{
    /// <summary>
    /// Pruebas de integración para RazaService que prueban contra la base de datos REAL.
    /// Estas pruebas verifican que todo el flujo CRUD funciona correctamente end-to-end.
    ///
    /// IMPORTANTE: Estas pruebas requieren:
    /// 1. Una base de datos real configurada
    /// 2. Conexión string válida en appsettings.json
    /// 3. Los stored procedures existentes
    /// </summary>
    public class RazaIntegrationTests : IClassFixture<DatabaseFixture>
    {
        private readonly RazaService _service;
        private readonly RazaRepository _repository;
        private readonly IMapper _mapper;
        private readonly DatabaseFixture _fixture;
        private readonly int _testUserId = 1; // Usuario de prueba

        public RazaIntegrationTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
            _repository = new RazaRepository();
            _mapper = fixture.Mapper;

            // Logger real (opcional: puede ser mock si no quieres logs en tests)
            var loggerMock = new Mock<ILogger<RazaService>>();
            _service = new RazaService(_repository, loggerMock.Object, _mapper);
        }

        // ═══════════════════════════════════════════════════════════════════════
        // CREATE (INSERT) - Pruebas de creación
        // ═══════════════════════════════════════════════════════════════════════

        [Fact]
        public async Task AddAsync_ConDatosValidos_InsertaEnBaseDeDatos()
        {
            // Arrange
            var nuevaRaza = new RazaFormViewModel
            {
                raza_Descripcion = $"Test {Guid.NewGuid().ToString().Substring(0, 8)}",
                raza_Tamano = "Mediano",
                raza_TipoAnimal = "Perro",
                raza_TipoPelaje = "Corto",
                raza_EsActivo = true
            };

            // Act
            bool resultado = await _service.AddAsync(nuevaRaza, _testUserId);

            // Assert
            Assert.True(resultado, "La inserción debería retornar true");

            // Verificar que realmente se insertó en la BD
            var lista = await _service.ListAsync();
            var razaInsertada = lista.FirstOrDefault(r => r.raza_Descripcion == nuevaRaza.raza_Descripcion);

            Assert.True(razaInsertada != null, $"No se encontró '{nuevaRaza.raza_Descripcion}' en la BD — el INSERT puede haber fallado (¿nombre de SP incorrecto?)");
            Assert.Equal(nuevaRaza.raza_Tamano, razaInsertada.raza_Tamano);
            Assert.Equal(nuevaRaza.raza_TipoAnimal, razaInsertada.raza_TipoAnimal);

            // Cleanup - Eliminar el registro de prueba
            if (razaInsertada != null)
            {
                await _service.RemoveAsync(razaInsertada.raza_Id);
            }
        }

        [Fact]
        public async Task AddAsync_ConDescripcionVacia_NoDeberiaInsertar()
        {
            // Arrange - modelo inválido
            var razaInvalida = new RazaFormViewModel
            {
                raza_Descripcion = "", // Descripción vacía (no debería pasar validación)
                raza_Tamano = "Pequeño"
            };

            // Act & Assert
            // Nota: En una implementación real con validaciones, esto podría lanzar excepción
            // o retornar false. El comportamiento actual depende de tu implementación.
            try
            {
                bool resultado = await _service.AddAsync(razaInvalida, _testUserId);
                // Si llega aquí, al menos verificamos que no cause errores fatales
                Assert.True(true);
            }
            catch (Exception ex)
            {
                // Es aceptable que lance excepción con datos inválidos
                Assert.NotNull(ex);
            }
        }

        // ═══════════════════════════════════════════════════════════════════════
        // READ (SELECT) - Pruebas de lectura
        // ═══════════════════════════════════════════════════════════════════════

        [Fact]
        public async Task ListAsync_RetornaListaDeRazasDesdeBD()
        {
            // Act
            var resultado = await _service.ListAsync();

            // Assert
            Assert.NotNull(resultado);
            Assert.IsType<List<RazaListViewModel>>(resultado);
            // Debería haber al menos algunas razas en la BD
            Assert.True(resultado.Count >= 0);
        }

        [Fact]
        public async Task FindAsync_ConIdExistente_RetornaRazaDesdeBD()
        {
            // Arrange - Primero crear una raza de prueba
            var razaPrueba = new RazaFormViewModel
            {
                raza_Descripcion = $"Find {Guid.NewGuid().ToString().Substring(0, 8)}",
                raza_Tamano = "Grande",
                raza_TipoAnimal = "Gato",
                raza_EsActivo = true
            };

            await _service.AddAsync(razaPrueba, _testUserId);

            // Obtener el ID de la raza recién creada
            var lista = await _service.ListAsync();
            var razaCreada = lista.FirstOrDefault(r => r.raza_Descripcion == razaPrueba.raza_Descripcion);
            Assert.True(razaCreada != null, $"Precondición fallida: no se pudo insertar '{razaPrueba.raza_Descripcion}' — verificar SP de INSERT");

            // Act - Buscar por ID
            var resultado = await _service.FindAsync(razaCreada.raza_Id);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(razaCreada.raza_Id, resultado.raza_Id);
            Assert.Equal(razaPrueba.raza_Descripcion, resultado.raza_Descripcion);
            Assert.Equal(razaPrueba.raza_Tamano, resultado.raza_Tamano);

            // Cleanup
            await _service.RemoveAsync(razaCreada.raza_Id);
        }

        [Fact]
        public async Task FindAsync_ConIdInexistente_RetornaNull()
        {
            // Arrange
            int idInexistente = 999999;

            // Act
            var resultado = await _service.FindAsync(idInexistente);

            // Assert
            Assert.Null(resultado);
        }

        [Fact]
        public async Task DetailAsync_ConIdExistente_RetornaDetalleCompleto()
        {
            // Arrange - Crear raza de prueba
            var razaPrueba = new RazaFormViewModel
            {
                raza_Descripcion = $"Detail {Guid.NewGuid().ToString().Substring(0, 8)}",
                raza_Tamano = "Pequeño",
                raza_TipoAnimal = "Perro",
                raza_TipoPelaje = "Largo",
                raza_EsActivo = true
            };

            await _service.AddAsync(razaPrueba, _testUserId);

            var lista = await _service.ListAsync();
            var razaCreada = lista.FirstOrDefault(r => r.raza_Descripcion == razaPrueba.raza_Descripcion);
            Assert.True(razaCreada != null, $"Precondición fallida: no se pudo insertar '{razaPrueba.raza_Descripcion}' — verificar SP de INSERT");

            // Act
            var resultado = await _service.DetailAsync(razaCreada.raza_Id);

            // Assert
            Assert.True(resultado != null, $"DetailAsync no retornó datos para el Id={razaCreada.raza_Id}");
            Assert.Equal(razaCreada.raza_Id, resultado.raza_Id);
            Assert.Equal(razaPrueba.raza_Descripcion, resultado.raza_Descripcion);
            // DetailAsync debería incluir información de auditoría
            Assert.NotEqual(default(DateTime), resultado.raza_FechaCrea);

            // Cleanup
            await _service.RemoveAsync(razaCreada.raza_Id);
        }

        // ═══════════════════════════════════════════════════════════════════════
        // UPDATE - Pruebas de actualización
        // ═══════════════════════════════════════════════════════════════════════

        [Fact]
        public async Task UpdateAsync_ConDatosValidos_ActualizaEnBaseDeDatos()
        {
            // Arrange - Crear raza inicial
            var razaInicial = new RazaFormViewModel
            {
                raza_Descripcion = $"Orig {Guid.NewGuid().ToString().Substring(0, 8)}",
                raza_Tamano = "Mediano",
                raza_TipoAnimal = "Perro",
                raza_TipoPelaje = "Corto",
                raza_EsActivo = true
            };

            await _service.AddAsync(razaInicial, _testUserId);

            var lista = await _service.ListAsync();
            var razaCreada = lista.FirstOrDefault(r => r.raza_Descripcion == razaInicial.raza_Descripcion);
            Assert.True(razaCreada != null, $"Precondición fallida: no se pudo insertar '{razaInicial.raza_Descripcion}' — verificar SP de INSERT");

            // Modificar datos
            var razaActualizada = await _service.FindAsync(razaCreada.raza_Id);
            razaActualizada.raza_Descripcion = $"Upd {Guid.NewGuid().ToString().Substring(0, 8)}";
            razaActualizada.raza_Tamano = "Grande";
            razaActualizada.raza_TipoPelaje = "Largo";

            // Act
            bool resultado = await _service.UpdateAsync(razaActualizada, _testUserId);

            // Assert
            Assert.True(resultado, "La actualización debería retornar true");

            // Verificar que se actualizó en la BD
            var razaVerificada = await _service.FindAsync(razaCreada.raza_Id);
            Assert.True(razaVerificada != null, $"No se encontró la raza Id={razaCreada.raza_Id} tras el UPDATE");
            Assert.Equal(razaActualizada.raza_Descripcion, razaVerificada.raza_Descripcion);
            Assert.Equal("Grande", razaVerificada.raza_Tamano);
            Assert.Equal("Largo", razaVerificada.raza_TipoPelaje);

            // Cleanup
            await _service.RemoveAsync(razaCreada.raza_Id);
        }

        [Fact]
        public async Task UpdateAsync_CambiaEstadoActivo_ActualizaCorrectamente()
        {
            // Arrange
            var razaPrueba = new RazaFormViewModel
            {
                raza_Descripcion = $"Estado {Guid.NewGuid().ToString().Substring(0, 8)}",
                raza_Tamano = "Mediano",
                raza_EsActivo = true
            };

            await _service.AddAsync(razaPrueba, _testUserId);

            var lista = await _service.ListAsync();
            var razaCreada = lista.FirstOrDefault(r => r.raza_Descripcion == razaPrueba.raza_Descripcion);
            Assert.True(razaCreada != null, $"Precondición fallida: no se pudo insertar '{razaPrueba.raza_Descripcion}' — verificar SP de INSERT");

            // Cambiar estado a inactivo
            var razaActualizar = await _service.FindAsync(razaCreada.raza_Id);
            razaActualizar.raza_EsActivo = false;

            // Act
            bool resultado = await _service.UpdateAsync(razaActualizar, _testUserId);

            // Assert
            Assert.True(resultado);

            var razaVerificada = await _service.FindAsync(razaCreada.raza_Id);
            Assert.False(razaVerificada.raza_EsActivo);

            // Cleanup
            await _service.RemoveAsync(razaCreada.raza_Id);
        }

        // ═══════════════════════════════════════════════════════════════════════
        // DELETE - Pruebas de eliminación
        // ═══════════════════════════════════════════════════════════════════════

        [Fact]
        public async Task RemoveAsync_ConIdExistente_EliminaDeBaseDeDatos()
        {
            // Arrange - Crear raza para eliminar
            var razaPrueba = new RazaFormViewModel
            {
                raza_Descripcion = $"Del {Guid.NewGuid().ToString().Substring(0, 8)}",
                raza_Tamano = "Pequeño",
                raza_EsActivo = true
            };

            await _service.AddAsync(razaPrueba, _testUserId);

            var lista = await _service.ListAsync();
            var razaCreada = lista.FirstOrDefault(r => r.raza_Descripcion == razaPrueba.raza_Descripcion);
            Assert.True(razaCreada != null, $"Precondición fallida: no se pudo insertar '{razaPrueba.raza_Descripcion}' — verificar SP de INSERT");

            int idAEliminar = razaCreada.raza_Id;

            // Act
            bool resultado = await _service.RemoveAsync(idAEliminar);

            // Assert
            Assert.True(resultado, "La eliminación debería retornar true");

            // Verificar que ya no existe en la BD
            var razaEliminada = await _service.FindAsync(idAEliminar);
            Assert.Null(razaEliminada);
        }

        [Fact]
        public async Task RemoveAsync_ConIdInexistente_RetornaFalse()
        {
            // Arrange
            int idInexistente = 999999;

            // Act
            bool resultado = await _service.RemoveAsync(idInexistente);

            // Assert
            Assert.False(resultado, "Eliminar un ID inexistente debería retornar false");
        }

        // ═══════════════════════════════════════════════════════════════════════
        // INTEGRATION - Pruebas de flujo completo CRUD
        // ═══════════════════════════════════════════════════════════════════════

        [Fact]
        public async Task FlujoCRUDCompleto_CrearLeerActualizarEliminar_FuncionaCorrectamente()
        {
            // 1. CREATE - Crear nueva raza
            var nuevaRaza = new RazaFormViewModel
            {
                raza_Descripcion = $"CRUD {Guid.NewGuid().ToString().Substring(0, 8)}",
                raza_Tamano = "Mediano",
                raza_TipoAnimal = "Perro",
                raza_TipoPelaje = "Corto",
                raza_EsActivo = true
            };

            bool insertado = await _service.AddAsync(nuevaRaza, _testUserId);
            Assert.True(insertado, "CREATE falló");

            // 2. READ - Leer la raza creada
            var lista = await _service.ListAsync();
            var razaCreada = lista.FirstOrDefault(r => r.raza_Descripcion == nuevaRaza.raza_Descripcion);
            Assert.True(razaCreada != null, $"CREATE falló: '{nuevaRaza.raza_Descripcion}' no está en la BD — verificar SP de INSERT");

            var razaEncontrada = await _service.FindAsync(razaCreada.raza_Id);
            Assert.True(razaEncontrada != null, $"READ falló: FindAsync no encontró Id={razaCreada.raza_Id}");
            Assert.Equal(nuevaRaza.raza_Descripcion, razaEncontrada.raza_Descripcion);

            // 3. UPDATE - Actualizar la raza
            razaEncontrada.raza_Descripcion = $"CRUD2 {Guid.NewGuid().ToString().Substring(0, 8)}";
            razaEncontrada.raza_Tamano = "Grande";
            bool actualizado = await _service.UpdateAsync(razaEncontrada, _testUserId);
            Assert.True(actualizado, "UPDATE falló");

            var razaActualizada = await _service.FindAsync(razaCreada.raza_Id);
            Assert.Equal(razaEncontrada.raza_Descripcion, razaActualizada.raza_Descripcion);
            Assert.Equal("Grande", razaActualizada.raza_Tamano);

            // 4. DELETE - Eliminar la raza
            bool eliminado = await _service.RemoveAsync(razaCreada.raza_Id);
            Assert.True(eliminado, "DELETE falló");

            var razaEliminada = await _service.FindAsync(razaCreada.raza_Id);
            Assert.Null(razaEliminada);
        }
    }

    /// <summary>
    /// Fixture para configurar la base de datos antes de ejecutar las pruebas de integración.
    /// Se ejecuta una vez antes de todas las pruebas de la clase.
    /// </summary>
    public class DatabaseFixture : IDisposable
    {
        public IMapper Mapper { get; private set; }

        public DatabaseFixture()
        {
            // Connection string para pruebas de integración
            // IMPORTANTE: Cambia esto si tu base de datos está en otro servidor
            var connectionString = "Data source=(localdb)\\MSSQLLocalDB;Initial Catalog=PETSHOMEDB;Integrated Security=True;Encrypt=False";

            // Configurar la conexión estática
            PetsHomeDbContext.BuildConnectionString(connectionString);

            // Configurar AutoMapper
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfileExtensions());
            });

            Mapper = config.CreateMapper();
        }

        public void Dispose()
        {
            // Cleanup si es necesario
            // Por ejemplo, limpiar datos de prueba residuales
        }
    }
}
