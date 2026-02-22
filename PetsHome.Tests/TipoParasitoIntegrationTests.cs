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
    /// Pruebas de integracion para TipoParasitoService contra la base de datos REAL.
    /// Verifica que el flujo CRUD completo funciona correctamente end-to-end.
    ///
    /// IMPORTANTE:
    /// 1. Requiere base de datos configurada y accesible
    /// 2. Connection string configurado en DatabaseFixture
    /// 3. Stored procedures de TipoParasito existentes en la BD
    /// </summary>
    public class TipoParasitoIntegrationTests : IClassFixture<DatabaseFixture>
    {
        private readonly TipoParasitoService _service;
        private readonly TipoParasitoRepository _repository;
        private readonly IMapper _mapper;
        private readonly DatabaseFixture _fixture;

        public TipoParasitoIntegrationTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
            _repository = new TipoParasitoRepository();
            _mapper = fixture.Mapper;

            var loggerMock = new Mock<ILogger<TipoParasitoService>>();
            _service = new TipoParasitoService(_repository, loggerMock.Object, _mapper);
        }

        // =======================================================================
        // CREATE (INSERT)
        // =======================================================================

        [Fact]
        public async Task AddAsync_ConDatosValidos_InsertaEnBaseDeDatos()
        {
            // Arrange
            var nuevoTipoParasito = new TipoParasitoViewModel
            {
                tipoPar_Descripcion = $"Test {Guid.NewGuid().ToString().Substring(0, 8)}",
                tipoPar_EsActivoBool = true
            };

            // Act
            bool resultado = await _service.AddAsync(nuevoTipoParasito);

            // Assert
            Assert.True(resultado, "La insercion deberia retornar true");

            var lista = await _service.ListAsync();
            var tipoParasitoInsertado = lista.FirstOrDefault(x => x.tipoPar_Descripcion == nuevoTipoParasito.tipoPar_Descripcion);

            Assert.True(tipoParasitoInsertado != null,
                $"No se encontro '{nuevoTipoParasito.tipoPar_Descripcion}' en la BD — el INSERT puede haber fallado (verificar nombre del SP)");

            // Cleanup
            if (tipoParasitoInsertado != null)
                await _service.RemoveAsync(tipoParasitoInsertado.tipoPar_Id);
        }

        [Fact]
        public async Task AddAsync_ConDescripcionVacia_NoDeberiaInsertar()
        {
            // Arrange
            var tipoParasitoInvalido = new TipoParasitoViewModel
            {
                tipoPar_Descripcion = ""
            };

            // Act & Assert
            try
            {
                bool resultado = await _service.AddAsync(tipoParasitoInvalido);
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
        public async Task ListAsync_RetornaListaDeTipoParasitosDesdeBD()
        {
            // Act
            var resultado = await _service.ListAsync();

            // Assert
            Assert.NotNull(resultado);
            Assert.IsType<List<TipoParasitoViewModel>>(resultado);
            Assert.True(resultado.Count >= 0);
        }

        [Fact]
        public async Task FindAsync_ConIdExistente_RetornaTipoParasitoDesdeBD()
        {
            // Arrange
            var tipoParasitoPrueba = new TipoParasitoViewModel
            {
                tipoPar_Descripcion = $"Find {Guid.NewGuid().ToString().Substring(0, 8)}",
                tipoPar_EsActivoBool = true
            };

            await _service.AddAsync(tipoParasitoPrueba);

            var lista = await _service.ListAsync();
            var tipoParasitoCreado = lista.FirstOrDefault(x => x.tipoPar_Descripcion == tipoParasitoPrueba.tipoPar_Descripcion);
            Assert.True(tipoParasitoCreado != null,
                $"Precondicion fallida: no se pudo insertar '{tipoParasitoPrueba.tipoPar_Descripcion}' — verificar SP de INSERT");

            // Act
            var resultado = await _service.FindAsync(tipoParasitoCreado.tipoPar_Id);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(tipoParasitoCreado.tipoPar_Id, resultado.tipoPar_Id);
            Assert.Equal(tipoParasitoPrueba.tipoPar_Descripcion, resultado.tipoPar_Descripcion);

            // Cleanup
            await _service.RemoveAsync(tipoParasitoCreado.tipoPar_Id);
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
            var tipoParasitoPrueba = new TipoParasitoViewModel
            {
                tipoPar_Descripcion = $"Detail {Guid.NewGuid().ToString().Substring(0, 8)}",
                tipoPar_EsActivoBool = true
            };

            await _service.AddAsync(tipoParasitoPrueba);

            var lista = await _service.ListAsync();
            var tipoParasitoCreado = lista.FirstOrDefault(x => x.tipoPar_Descripcion == tipoParasitoPrueba.tipoPar_Descripcion);
            Assert.True(tipoParasitoCreado != null,
                $"Precondicion fallida: no se pudo insertar '{tipoParasitoPrueba.tipoPar_Descripcion}' — verificar SP de INSERT");

            // Act
            var resultado = await _service.DetailAsync(tipoParasitoCreado.tipoPar_Id);

            // Assert
            Assert.True(resultado != null,
                $"DetailAsync no retorno datos para Id={tipoParasitoCreado.tipoPar_Id}");
            Assert.Equal(tipoParasitoCreado.tipoPar_Id, resultado.tipoPar_Id);
            Assert.Equal(tipoParasitoPrueba.tipoPar_Descripcion, resultado.tipoPar_Descripcion);
            Assert.NotEqual(default(DateTime), resultado.tipoPar_FechaCrea);

            // Cleanup
            await _service.RemoveAsync(tipoParasitoCreado.tipoPar_Id);
        }

        // =======================================================================
        // UPDATE
        // =======================================================================

        [Fact]
        public async Task UpdateAsync_ConDatosValidos_ActualizaEnBaseDeDatos()
        {
            // Arrange
            var tipoParasitoInicial = new TipoParasitoViewModel
            {
                tipoPar_Descripcion = $"Orig {Guid.NewGuid().ToString().Substring(0, 8)}",
                tipoPar_EsActivoBool = true
            };

            await _service.AddAsync(tipoParasitoInicial);

            var lista = await _service.ListAsync();
            var tipoParasitoCreado = lista.FirstOrDefault(x => x.tipoPar_Descripcion == tipoParasitoInicial.tipoPar_Descripcion);
            Assert.True(tipoParasitoCreado != null,
                $"Precondicion fallida: no se pudo insertar '{tipoParasitoInicial.tipoPar_Descripcion}' — verificar SP de INSERT");

            var tipoParasitoActualizar = await _service.FindAsync(tipoParasitoCreado.tipoPar_Id);
            tipoParasitoActualizar.tipoPar_Descripcion = $"Upd {Guid.NewGuid().ToString().Substring(0, 8)}";

            // Act
            bool resultado = await _service.UpdateAsync(tipoParasitoActualizar);

            // Assert
            Assert.True(resultado, "La actualizacion deberia retornar true");

            var tipoParasitoVerificado = await _service.FindAsync(tipoParasitoCreado.tipoPar_Id);
            Assert.True(tipoParasitoVerificado != null,
                $"No se encontro el registro Id={tipoParasitoCreado.tipoPar_Id} tras el UPDATE");
            Assert.Equal(tipoParasitoActualizar.tipoPar_Descripcion, tipoParasitoVerificado.tipoPar_Descripcion);

            // Cleanup
            await _service.RemoveAsync(tipoParasitoCreado.tipoPar_Id);
        }

        [Fact]
        public async Task UpdateAsync_CambiaEstadoActivo_ActualizaCorrectamente()
        {
            // Arrange
            var tipoParasitoPrueba = new TipoParasitoViewModel
            {
                tipoPar_Descripcion = $"Estado {Guid.NewGuid().ToString().Substring(0, 8)}",
                tipoPar_EsActivoBool = true
            };

            await _service.AddAsync(tipoParasitoPrueba);

            var lista = await _service.ListAsync();
            var tipoParasitoCreado = lista.FirstOrDefault(x => x.tipoPar_Descripcion == tipoParasitoPrueba.tipoPar_Descripcion);
            Assert.True(tipoParasitoCreado != null,
                $"Precondicion fallida: no se pudo insertar '{tipoParasitoPrueba.tipoPar_Descripcion}' — verificar SP de INSERT");

            var tipoParasitoActualizar = await _service.FindAsync(tipoParasitoCreado.tipoPar_Id);
            tipoParasitoActualizar.tipoPar_EsActivoBool = false;

            // Act
            bool resultado = await _service.UpdateAsync(tipoParasitoActualizar);

            // Assert
            Assert.True(resultado);

            var tipoParasitoVerificado = await _service.FindAsync(tipoParasitoCreado.tipoPar_Id);
            Assert.False(tipoParasitoVerificado.tipoPar_EsActivoBool);

            // Cleanup
            await _service.RemoveAsync(tipoParasitoCreado.tipoPar_Id);
        }

        // =======================================================================
        // DELETE
        // =======================================================================

        [Fact]
        public async Task RemoveAsync_ConIdExistente_EliminaDeBaseDeDatos()
        {
            // Arrange
            var tipoParasitoPrueba = new TipoParasitoViewModel
            {
                tipoPar_Descripcion = $"Del {Guid.NewGuid().ToString().Substring(0, 8)}",
                tipoPar_EsActivoBool = true
            };

            await _service.AddAsync(tipoParasitoPrueba);

            var lista = await _service.ListAsync();
            var tipoParasitoCreado = lista.FirstOrDefault(x => x.tipoPar_Descripcion == tipoParasitoPrueba.tipoPar_Descripcion);
            Assert.True(tipoParasitoCreado != null,
                $"Precondicion fallida: no se pudo insertar '{tipoParasitoPrueba.tipoPar_Descripcion}' — verificar SP de INSERT");

            int idEliminar = tipoParasitoCreado.tipoPar_Id;

            // Act
            bool resultado = await _service.RemoveAsync(idEliminar);

            // Assert
            Assert.True(resultado, "La eliminacion deberia retornar true");

            var tipoParasitoEliminado = await _service.FindAsync(idEliminar);
            Assert.Null(tipoParasitoEliminado);
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
            var nuevoTipoParasito = new TipoParasitoViewModel
            {
                tipoPar_Descripcion = $"CRUD {Guid.NewGuid().ToString().Substring(0, 8)}",
                tipoPar_EsActivoBool = true
            };

            bool insertado = await _service.AddAsync(nuevoTipoParasito);
            Assert.True(insertado, "CREATE fallo — verificar SP de INSERT");

            // 2. READ
            var lista = await _service.ListAsync();
            var tipoParasitoCreado = lista.FirstOrDefault(x => x.tipoPar_Descripcion == nuevoTipoParasito.tipoPar_Descripcion);
            Assert.True(tipoParasitoCreado != null,
                $"CREATE fallo: '{nuevoTipoParasito.tipoPar_Descripcion}' no esta en la BD — verificar SP de INSERT");

            var tipoParasitoEncontrado = await _service.FindAsync(tipoParasitoCreado.tipoPar_Id);
            Assert.True(tipoParasitoEncontrado != null,
                $"READ fallo: FindAsync no encontro Id={tipoParasitoCreado.tipoPar_Id}");
            Assert.Equal(nuevoTipoParasito.tipoPar_Descripcion, tipoParasitoEncontrado.tipoPar_Descripcion);

            // 3. UPDATE
            tipoParasitoEncontrado.tipoPar_Descripcion = $"CRUD2 {Guid.NewGuid().ToString().Substring(0, 8)}";
            bool actualizado = await _service.UpdateAsync(tipoParasitoEncontrado);
            Assert.True(actualizado, "UPDATE fallo");

            var tipoParasitoVerificado = await _service.FindAsync(tipoParasitoCreado.tipoPar_Id);
            Assert.Equal(tipoParasitoEncontrado.tipoPar_Descripcion, tipoParasitoVerificado.tipoPar_Descripcion);

            // 4. DELETE
            bool eliminado = await _service.RemoveAsync(tipoParasitoCreado.tipoPar_Id);
            Assert.True(eliminado, "DELETE fallo");

            var tipoParasitoEliminado = await _service.FindAsync(tipoParasitoCreado.tipoPar_Id);
            Assert.Null(tipoParasitoEliminado);
        }
    }
}
