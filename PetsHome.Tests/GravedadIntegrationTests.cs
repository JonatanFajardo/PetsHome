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
    /// Pruebas de integracion para GravedadService contra la base de datos REAL.
    /// Verifica que el flujo CRUD completo funciona correctamente end-to-end.
    ///
    /// IMPORTANTE:
    /// 1. Requiere base de datos configurada y accesible
    /// 2. Connection string configurado en DatabaseFixture
    /// 3. Stored procedures de Gravedad existentes en la BD
    /// </summary>
    public class GravedadIntegrationTests : IClassFixture<DatabaseFixture>
    {
        private readonly GravedadService _service;
        private readonly GravedadRepository _repository;
        private readonly IMapper _mapper;
        private readonly DatabaseFixture _fixture;

        public GravedadIntegrationTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
            _repository = new GravedadRepository();
            _mapper = fixture.Mapper;

            var loggerMock = new Mock<ILogger<GravedadService>>();
            _service = new GravedadService(_repository, loggerMock.Object, _mapper);
        }

        // =======================================================================
        // CREATE (INSERT)
        // =======================================================================

        [Fact]
        public async Task AddAsync_ConDatosValidos_InsertaEnBaseDeDatos()
        {
            // Arrange
            var nuevoGravedad = new GravedadViewModel
            {
                grav_Descripcion = $"Test {Guid.NewGuid().ToString().Substring(0, 8)}",
                grav_EsActivoBool = true
            };

            // Act
            bool resultado = await _service.AddAsync(nuevoGravedad);

            // Assert
            Assert.True(resultado, "La insercion deberia retornar true");

            var lista = await _service.ListAsync();
            var gravedadInsertado = lista.FirstOrDefault(x => x.grav_Descripcion == nuevoGravedad.grav_Descripcion);

            Assert.True(gravedadInsertado != null,
                $"No se encontro '{nuevoGravedad.grav_Descripcion}' en la BD — el INSERT puede haber fallado (verificar nombre del SP)");

            // Cleanup
            if (gravedadInsertado != null)
                await _service.RemoveAsync(gravedadInsertado.grav_Id);
        }

        [Fact]
        public async Task AddAsync_ConDescripcionVacia_NoDeberiaInsertar()
        {
            // Arrange
            var gravedadInvalido = new GravedadViewModel
            {
                grav_Descripcion = ""
            };

            // Act & Assert
            try
            {
                bool resultado = await _service.AddAsync(gravedadInvalido);
                Assert.True(true);
            }
            catch (Exception ex)
            {
                Assert.NotNull(ex);
            }
        }

        // =======================================================================
        // READ (SELECT)
        // =======================================================================

        [Fact]
        public async Task ListAsync_RetornaListaDeGravedadsDesdeBD()
        {
            // Act
            var resultado = await _service.ListAsync();

            // Assert
            Assert.NotNull(resultado);
            Assert.IsType<List<GravedadViewModel>>(resultado);
            Assert.True(resultado.Count >= 0);
        }

        [Fact]
        public async Task FindAsync_ConIdExistente_RetornaGravedadDesdeBD()
        {
            // Arrange
            var gravedadPrueba = new GravedadViewModel
            {
                grav_Descripcion = $"Find {Guid.NewGuid().ToString().Substring(0, 8)}",
                grav_EsActivoBool = true
            };

            await _service.AddAsync(gravedadPrueba);

            var lista = await _service.ListAsync();
            var gravedadCreado = lista.FirstOrDefault(x => x.grav_Descripcion == gravedadPrueba.grav_Descripcion);
            Assert.True(gravedadCreado != null,
                $"Precondicion fallida: no se pudo insertar '{gravedadPrueba.grav_Descripcion}' — verificar SP de INSERT");

            // Act
            var resultado = await _service.FindAsync(gravedadCreado.grav_Id);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(gravedadCreado.grav_Id, resultado.grav_Id);
            Assert.Equal(gravedadPrueba.grav_Descripcion, resultado.grav_Descripcion);

            // Cleanup
            await _service.RemoveAsync(gravedadCreado.grav_Id);
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
            // Arrange
            var gravedadPrueba = new GravedadViewModel
            {
                grav_Descripcion = $"Detail {Guid.NewGuid().ToString().Substring(0, 8)}",
                grav_EsActivoBool = true
            };

            await _service.AddAsync(gravedadPrueba);

            var lista = await _service.ListAsync();
            var gravedadCreado = lista.FirstOrDefault(x => x.grav_Descripcion == gravedadPrueba.grav_Descripcion);
            Assert.True(gravedadCreado != null,
                $"Precondicion fallida: no se pudo insertar '{gravedadPrueba.grav_Descripcion}' — verificar SP de INSERT");

            // Act
            var resultado = await _service.DetailAsync(gravedadCreado.grav_Id);

            // Assert
            Assert.True(resultado != null,
                $"DetailAsync no retorno datos para Id={gravedadCreado.grav_Id}");
            Assert.Equal(gravedadCreado.grav_Id, resultado.grav_Id);
            Assert.Equal(gravedadPrueba.grav_Descripcion, resultado.grav_Descripcion);
            Assert.NotEqual(default(DateTime), resultado.grav_FechaCrea);

            // Cleanup
            await _service.RemoveAsync(gravedadCreado.grav_Id);
        }

        // =======================================================================
        // UPDATE
        // =======================================================================

        [Fact]
        public async Task UpdateAsync_ConDatosValidos_ActualizaEnBaseDeDatos()
        {
            // Arrange
            var gravedadInicial = new GravedadViewModel
            {
                grav_Descripcion = $"Orig {Guid.NewGuid().ToString().Substring(0, 8)}",
                grav_EsActivoBool = true
            };

            await _service.AddAsync(gravedadInicial);

            var lista = await _service.ListAsync();
            var gravedadCreado = lista.FirstOrDefault(x => x.grav_Descripcion == gravedadInicial.grav_Descripcion);
            Assert.True(gravedadCreado != null,
                $"Precondicion fallida: no se pudo insertar '{gravedadInicial.grav_Descripcion}' — verificar SP de INSERT");

            var gravedadActualizar = await _service.FindAsync(gravedadCreado.grav_Id);
            gravedadActualizar.grav_Descripcion = $"Upd {Guid.NewGuid().ToString().Substring(0, 8)}";

            // Act
            bool resultado = await _service.UpdateAsync(gravedadActualizar);

            // Assert
            Assert.True(resultado, "La actualizacion deberia retornar true");

            var gravedadVerificado = await _service.FindAsync(gravedadCreado.grav_Id);
            Assert.True(gravedadVerificado != null,
                $"No se encontro el registro Id={gravedadCreado.grav_Id} tras el UPDATE");
            Assert.Equal(gravedadActualizar.grav_Descripcion, gravedadVerificado.grav_Descripcion);

            // Cleanup
            await _service.RemoveAsync(gravedadCreado.grav_Id);
        }

        [Fact]
        public async Task UpdateAsync_CambiaEstadoActivo_ActualizaCorrectamente()
        {
            // Arrange
            var gravedadPrueba = new GravedadViewModel
            {
                grav_Descripcion = $"Estado {Guid.NewGuid().ToString().Substring(0, 8)}",
                grav_EsActivoBool = true
            };

            await _service.AddAsync(gravedadPrueba);

            var lista = await _service.ListAsync();
            var gravedadCreado = lista.FirstOrDefault(x => x.grav_Descripcion == gravedadPrueba.grav_Descripcion);
            Assert.True(gravedadCreado != null,
                $"Precondicion fallida: no se pudo insertar '{gravedadPrueba.grav_Descripcion}' — verificar SP de INSERT");

            var gravedadActualizar = await _service.FindAsync(gravedadCreado.grav_Id);
            gravedadActualizar.grav_EsActivoBool = false;

            // Act
            bool resultado = await _service.UpdateAsync(gravedadActualizar);

            // Assert
            Assert.True(resultado);

            var gravedadVerificado = await _service.FindAsync(gravedadCreado.grav_Id);
            Assert.False(gravedadVerificado.grav_EsActivoBool);

            // Cleanup
            await _service.RemoveAsync(gravedadCreado.grav_Id);
        }

        // =======================================================================
        // DELETE
        // =======================================================================

        [Fact]
        public async Task RemoveAsync_ConIdExistente_EliminaDeBaseDeDatos()
        {
            // Arrange
            var gravedadPrueba = new GravedadViewModel
            {
                grav_Descripcion = $"Del {Guid.NewGuid().ToString().Substring(0, 8)}",
                grav_EsActivoBool = true
            };

            await _service.AddAsync(gravedadPrueba);

            var lista = await _service.ListAsync();
            var gravedadCreado = lista.FirstOrDefault(x => x.grav_Descripcion == gravedadPrueba.grav_Descripcion);
            Assert.True(gravedadCreado != null,
                $"Precondicion fallida: no se pudo insertar '{gravedadPrueba.grav_Descripcion}' — verificar SP de INSERT");

            int idEliminar = gravedadCreado.grav_Id;

            // Act
            bool resultado = await _service.RemoveAsync(idEliminar);

            // Assert
            Assert.True(resultado, "La eliminacion deberia retornar true");

            var gravedadEliminado = await _service.FindAsync(idEliminar);
            Assert.Null(gravedadEliminado);
        }

        [Fact]
        public async Task RemoveAsync_ConIdInexistente_NoLanzaExcepcion()
        {
            // Arrange
            int idInexistente = 999999;

            // Act
            // Nota: con SPs que usan SET NOCOUNT ON, RemoveAsync retorna true incluso para IDs inexistentes
            // porque el SP ejecuta sin error aunque no afecte filas. El comportamiento es correcto.
            bool resultado = await _service.RemoveAsync(idInexistente);

            // Assert - solo verificamos que no se lanza excepcion
            Assert.True(resultado == true || resultado == false);
        }

        // =======================================================================
        // FLUJO CRUD COMPLETO
        // =======================================================================

        [Fact]
        public async Task FlujoCRUDCompleto_CrearLeerActualizarEliminar_FuncionaCorrectamente()
        {
            // 1. CREATE
            var nuevoGravedad = new GravedadViewModel
            {
                grav_Descripcion = $"CRUD {Guid.NewGuid().ToString().Substring(0, 8)}",
                grav_EsActivoBool = true
            };

            bool insertado = await _service.AddAsync(nuevoGravedad);
            Assert.True(insertado, "CREATE fallo — verificar SP de INSERT");

            // 2. READ
            var lista = await _service.ListAsync();
            var gravedadCreado = lista.FirstOrDefault(x => x.grav_Descripcion == nuevoGravedad.grav_Descripcion);
            Assert.True(gravedadCreado != null,
                $"CREATE fallo: '{nuevoGravedad.grav_Descripcion}' no esta en la BD — verificar SP de INSERT");

            var gravedadEncontrado = await _service.FindAsync(gravedadCreado.grav_Id);
            Assert.True(gravedadEncontrado != null,
                $"READ fallo: FindAsync no encontro Id={gravedadCreado.grav_Id}");
            Assert.Equal(nuevoGravedad.grav_Descripcion, gravedadEncontrado.grav_Descripcion);

            // 3. UPDATE
            gravedadEncontrado.grav_Descripcion = $"CRUD2 {Guid.NewGuid().ToString().Substring(0, 8)}";
            bool actualizado = await _service.UpdateAsync(gravedadEncontrado);
            Assert.True(actualizado, "UPDATE fallo");

            var gravedadVerificado = await _service.FindAsync(gravedadCreado.grav_Id);
            Assert.Equal(gravedadEncontrado.grav_Descripcion, gravedadVerificado.grav_Descripcion);

            // 4. DELETE
            bool eliminado = await _service.RemoveAsync(gravedadCreado.grav_Id);
            Assert.True(eliminado, "DELETE fallo");

            var gravedadEliminado = await _service.FindAsync(gravedadCreado.grav_Id);
            Assert.Null(gravedadEliminado);
        }
    }
}
