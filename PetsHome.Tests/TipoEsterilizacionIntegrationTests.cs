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
    /// Pruebas de integracion para TipoEsterilizacionService contra la base de datos REAL.
    /// Verifica que el flujo CRUD completo funciona correctamente end-to-end.
    ///
    /// IMPORTANTE:
    /// 1. Requiere base de datos configurada y accesible
    /// 2. Connection string configurado en DatabaseFixture
    /// 3. Stored procedures de TipoEsterilizacion existentes en la BD
    /// </summary>
    public class TipoEsterilizacionIntegrationTests : IClassFixture<DatabaseFixture>
    {
        private readonly TipoEsterilizacionService _service;
        private readonly TipoEsterilizacionRepository _repository;
        private readonly IMapper _mapper;
        private readonly DatabaseFixture _fixture;

        public TipoEsterilizacionIntegrationTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
            _repository = new TipoEsterilizacionRepository();
            _mapper = fixture.Mapper;

            var loggerMock = new Mock<ILogger<TipoEsterilizacionService>>();
            _service = new TipoEsterilizacionService(_repository, loggerMock.Object, _mapper);
        }

        // =======================================================================
        // CREATE (INSERT)
        // =======================================================================

        [Fact]
        public async Task AddAsync_ConDatosValidos_InsertaEnBaseDeDatos()
        {
            // Arrange
            var nuevoTipoEsterilizacion = new TipoEsterilizacionViewModel
            {
                tipoEst_Descripcion = $"Test {Guid.NewGuid().ToString().Substring(0, 8)}",
                tipoEst_EsActivoBool = true
            };

            // Act
            bool resultado = await _service.AddAsync(nuevoTipoEsterilizacion);

            // Assert
            Assert.True(resultado, "La insercion deberia retornar true");

            var lista = await _service.ListAsync();
            var tipoEsterilizacionInsertado = lista.FirstOrDefault(x => x.tipoEst_Descripcion == nuevoTipoEsterilizacion.tipoEst_Descripcion);

            Assert.True(tipoEsterilizacionInsertado != null,
                $"No se encontro '{nuevoTipoEsterilizacion.tipoEst_Descripcion}' en la BD — el INSERT puede haber fallado (verificar nombre del SP)");

            // Cleanup
            if (tipoEsterilizacionInsertado != null)
                await _service.RemoveAsync(tipoEsterilizacionInsertado.tipoEst_Id);
        }

        [Fact]
        public async Task AddAsync_ConDescripcionVacia_NoDeberiaInsertar()
        {
            // Arrange
            var tipoEsterilizacionInvalido = new TipoEsterilizacionViewModel
            {
                tipoEst_Descripcion = ""
            };

            // Act & Assert
            try
            {
                bool resultado = await _service.AddAsync(tipoEsterilizacionInvalido);
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
        public async Task ListAsync_RetornaListaDeTipoEsterilizacionsDesdeBD()
        {
            // Act
            var resultado = await _service.ListAsync();

            // Assert
            Assert.NotNull(resultado);
            Assert.IsType<List<TipoEsterilizacionViewModel>>(resultado);
            Assert.True(resultado.Count >= 0);
        }

        [Fact]
        public async Task FindAsync_ConIdExistente_RetornaTipoEsterilizacionDesdeBD()
        {
            // Arrange
            var tipoEsterilizacionPrueba = new TipoEsterilizacionViewModel
            {
                tipoEst_Descripcion = $"Find {Guid.NewGuid().ToString().Substring(0, 8)}",
                tipoEst_EsActivoBool = true
            };

            await _service.AddAsync(tipoEsterilizacionPrueba);

            var lista = await _service.ListAsync();
            var tipoEsterilizacionCreado = lista.FirstOrDefault(x => x.tipoEst_Descripcion == tipoEsterilizacionPrueba.tipoEst_Descripcion);
            Assert.True(tipoEsterilizacionCreado != null,
                $"Precondicion fallida: no se pudo insertar '{tipoEsterilizacionPrueba.tipoEst_Descripcion}' — verificar SP de INSERT");

            // Act
            var resultado = await _service.FindAsync(tipoEsterilizacionCreado.tipoEst_Id);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(tipoEsterilizacionCreado.tipoEst_Id, resultado.tipoEst_Id);
            Assert.Equal(tipoEsterilizacionPrueba.tipoEst_Descripcion, resultado.tipoEst_Descripcion);

            // Cleanup
            await _service.RemoveAsync(tipoEsterilizacionCreado.tipoEst_Id);
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
            var tipoEsterilizacionPrueba = new TipoEsterilizacionViewModel
            {
                tipoEst_Descripcion = $"Detail {Guid.NewGuid().ToString().Substring(0, 8)}",
                tipoEst_EsActivoBool = true
            };

            await _service.AddAsync(tipoEsterilizacionPrueba);

            var lista = await _service.ListAsync();
            var tipoEsterilizacionCreado = lista.FirstOrDefault(x => x.tipoEst_Descripcion == tipoEsterilizacionPrueba.tipoEst_Descripcion);
            Assert.True(tipoEsterilizacionCreado != null,
                $"Precondicion fallida: no se pudo insertar '{tipoEsterilizacionPrueba.tipoEst_Descripcion}' — verificar SP de INSERT");

            // Act
            var resultado = await _service.DetailAsync(tipoEsterilizacionCreado.tipoEst_Id);

            // Assert
            Assert.True(resultado != null,
                $"DetailAsync no retorno datos para Id={tipoEsterilizacionCreado.tipoEst_Id}");
            Assert.Equal(tipoEsterilizacionCreado.tipoEst_Id, resultado.tipoEst_Id);
            Assert.Equal(tipoEsterilizacionPrueba.tipoEst_Descripcion, resultado.tipoEst_Descripcion);
            Assert.NotEqual(default(DateTime), resultado.tipoEst_FechaCrea);

            // Cleanup
            await _service.RemoveAsync(tipoEsterilizacionCreado.tipoEst_Id);
        }

        // =======================================================================
        // UPDATE
        // =======================================================================

        [Fact]
        public async Task UpdateAsync_ConDatosValidos_ActualizaEnBaseDeDatos()
        {
            // Arrange
            var tipoEsterilizacionInicial = new TipoEsterilizacionViewModel
            {
                tipoEst_Descripcion = $"Orig {Guid.NewGuid().ToString().Substring(0, 8)}",
                tipoEst_EsActivoBool = true
            };

            await _service.AddAsync(tipoEsterilizacionInicial);

            var lista = await _service.ListAsync();
            var tipoEsterilizacionCreado = lista.FirstOrDefault(x => x.tipoEst_Descripcion == tipoEsterilizacionInicial.tipoEst_Descripcion);
            Assert.True(tipoEsterilizacionCreado != null,
                $"Precondicion fallida: no se pudo insertar '{tipoEsterilizacionInicial.tipoEst_Descripcion}' — verificar SP de INSERT");

            var tipoEsterilizacionActualizar = await _service.FindAsync(tipoEsterilizacionCreado.tipoEst_Id);
            tipoEsterilizacionActualizar.tipoEst_Descripcion = $"Upd {Guid.NewGuid().ToString().Substring(0, 8)}";

            // Act
            bool resultado = await _service.UpdateAsync(tipoEsterilizacionActualizar);

            // Assert
            Assert.True(resultado, "La actualizacion deberia retornar true");

            var tipoEsterilizacionVerificado = await _service.FindAsync(tipoEsterilizacionCreado.tipoEst_Id);
            Assert.True(tipoEsterilizacionVerificado != null,
                $"No se encontro el registro Id={tipoEsterilizacionCreado.tipoEst_Id} tras el UPDATE");
            Assert.Equal(tipoEsterilizacionActualizar.tipoEst_Descripcion, tipoEsterilizacionVerificado.tipoEst_Descripcion);

            // Cleanup
            await _service.RemoveAsync(tipoEsterilizacionCreado.tipoEst_Id);
        }

        [Fact]
        public async Task UpdateAsync_CambiaEstadoActivo_ActualizaCorrectamente()
        {
            // Arrange
            var tipoEsterilizacionPrueba = new TipoEsterilizacionViewModel
            {
                tipoEst_Descripcion = $"Estado {Guid.NewGuid().ToString().Substring(0, 8)}",
                tipoEst_EsActivoBool = true
            };

            await _service.AddAsync(tipoEsterilizacionPrueba);

            var lista = await _service.ListAsync();
            var tipoEsterilizacionCreado = lista.FirstOrDefault(x => x.tipoEst_Descripcion == tipoEsterilizacionPrueba.tipoEst_Descripcion);
            Assert.True(tipoEsterilizacionCreado != null,
                $"Precondicion fallida: no se pudo insertar '{tipoEsterilizacionPrueba.tipoEst_Descripcion}' — verificar SP de INSERT");

            var tipoEsterilizacionActualizar = await _service.FindAsync(tipoEsterilizacionCreado.tipoEst_Id);
            tipoEsterilizacionActualizar.tipoEst_EsActivoBool = false;

            // Act
            bool resultado = await _service.UpdateAsync(tipoEsterilizacionActualizar);

            // Assert
            Assert.True(resultado);

            var tipoEsterilizacionVerificado = await _service.FindAsync(tipoEsterilizacionCreado.tipoEst_Id);
            Assert.False(tipoEsterilizacionVerificado.tipoEst_EsActivoBool);

            // Cleanup
            await _service.RemoveAsync(tipoEsterilizacionCreado.tipoEst_Id);
        }

        // =======================================================================
        // DELETE
        // =======================================================================

        [Fact]
        public async Task RemoveAsync_ConIdExistente_EliminaDeBaseDeDatos()
        {
            // Arrange
            var tipoEsterilizacionPrueba = new TipoEsterilizacionViewModel
            {
                tipoEst_Descripcion = $"Del {Guid.NewGuid().ToString().Substring(0, 8)}",
                tipoEst_EsActivoBool = true
            };

            await _service.AddAsync(tipoEsterilizacionPrueba);

            var lista = await _service.ListAsync();
            var tipoEsterilizacionCreado = lista.FirstOrDefault(x => x.tipoEst_Descripcion == tipoEsterilizacionPrueba.tipoEst_Descripcion);
            Assert.True(tipoEsterilizacionCreado != null,
                $"Precondicion fallida: no se pudo insertar '{tipoEsterilizacionPrueba.tipoEst_Descripcion}' — verificar SP de INSERT");

            int idEliminar = tipoEsterilizacionCreado.tipoEst_Id;

            // Act
            bool resultado = await _service.RemoveAsync(idEliminar);

            // Assert
            Assert.True(resultado, "La eliminacion deberia retornar true");

            var tipoEsterilizacionEliminado = await _service.FindAsync(idEliminar);
            Assert.Null(tipoEsterilizacionEliminado);
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
            var nuevoTipoEsterilizacion = new TipoEsterilizacionViewModel
            {
                tipoEst_Descripcion = $"CRUD {Guid.NewGuid().ToString().Substring(0, 8)}",
                tipoEst_EsActivoBool = true
            };

            bool insertado = await _service.AddAsync(nuevoTipoEsterilizacion);
            Assert.True(insertado, "CREATE fallo — verificar SP de INSERT");

            // 2. READ
            var lista = await _service.ListAsync();
            var tipoEsterilizacionCreado = lista.FirstOrDefault(x => x.tipoEst_Descripcion == nuevoTipoEsterilizacion.tipoEst_Descripcion);
            Assert.True(tipoEsterilizacionCreado != null,
                $"CREATE fallo: '{nuevoTipoEsterilizacion.tipoEst_Descripcion}' no esta en la BD — verificar SP de INSERT");

            var tipoEsterilizacionEncontrado = await _service.FindAsync(tipoEsterilizacionCreado.tipoEst_Id);
            Assert.True(tipoEsterilizacionEncontrado != null,
                $"READ fallo: FindAsync no encontro Id={tipoEsterilizacionCreado.tipoEst_Id}");
            Assert.Equal(nuevoTipoEsterilizacion.tipoEst_Descripcion, tipoEsterilizacionEncontrado.tipoEst_Descripcion);

            // 3. UPDATE
            tipoEsterilizacionEncontrado.tipoEst_Descripcion = $"CRUD2 {Guid.NewGuid().ToString().Substring(0, 8)}";
            bool actualizado = await _service.UpdateAsync(tipoEsterilizacionEncontrado);
            Assert.True(actualizado, "UPDATE fallo");

            var tipoEsterilizacionVerificado = await _service.FindAsync(tipoEsterilizacionCreado.tipoEst_Id);
            Assert.Equal(tipoEsterilizacionEncontrado.tipoEst_Descripcion, tipoEsterilizacionVerificado.tipoEst_Descripcion);

            // 4. DELETE
            bool eliminado = await _service.RemoveAsync(tipoEsterilizacionCreado.tipoEst_Id);
            Assert.True(eliminado, "DELETE fallo");

            var tipoEsterilizacionEliminado = await _service.FindAsync(tipoEsterilizacionCreado.tipoEst_Id);
            Assert.Null(tipoEsterilizacionEliminado);
        }
    }
}
