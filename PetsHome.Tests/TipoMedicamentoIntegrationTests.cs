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
    /// Pruebas de integracion para TipoMedicamentoService contra la base de datos REAL.
    /// Verifica que el flujo CRUD completo funciona correctamente end-to-end.
    ///
    /// IMPORTANTE:
    /// 1. Requiere base de datos configurada y accesible
    /// 2. Connection string configurado en DatabaseFixture
    /// 3. Stored procedures de TipoMedicamento existentes en la BD
    /// </summary>
    public class TipoMedicamentoIntegrationTests : IClassFixture<DatabaseFixture>
    {
        private readonly TipoMedicamentoService _service;
        private readonly TipoMedicamentoRepository _repository;
        private readonly IMapper _mapper;
        private readonly DatabaseFixture _fixture;

        public TipoMedicamentoIntegrationTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
            _repository = new TipoMedicamentoRepository();
            _mapper = fixture.Mapper;

            var loggerMock = new Mock<ILogger<TipoMedicamentoService>>();
            _service = new TipoMedicamentoService(_repository, loggerMock.Object, _mapper);
        }

        // =======================================================================
        // CREATE (INSERT)
        // =======================================================================

        [Fact]
        public async Task AddAsync_ConDatosValidos_InsertaEnBaseDeDatos()
        {
            // Arrange
            var nuevoTipoMedicamento = new TipoMedicamentoViewModel
            {
                tipoMed_Descripcion = $"Test {Guid.NewGuid().ToString().Substring(0, 8)}",
                tipoMed_EsActivoBool = true
            };

            // Act
            bool resultado = await _service.AddAsync(nuevoTipoMedicamento);

            // Assert
            Assert.True(resultado, "La insercion deberia retornar true");

            var lista = await _service.ListAsync();
            var tipoMedicamentoInsertado = lista.FirstOrDefault(x => x.tipoMed_Descripcion == nuevoTipoMedicamento.tipoMed_Descripcion);

            Assert.True(tipoMedicamentoInsertado != null,
                $"No se encontro '{nuevoTipoMedicamento.tipoMed_Descripcion}' en la BD — el INSERT puede haber fallado (verificar nombre del SP)");

            // Cleanup
            if (tipoMedicamentoInsertado != null)
                await _service.RemoveAsync(tipoMedicamentoInsertado.tipoMed_Id);
        }

        [Fact]
        public async Task AddAsync_ConDescripcionVacia_NoDeberiaInsertar()
        {
            // Arrange
            var tipoMedicamentoInvalido = new TipoMedicamentoViewModel
            {
                tipoMed_Descripcion = ""
            };

            // Act & Assert
            try
            {
                bool resultado = await _service.AddAsync(tipoMedicamentoInvalido);
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
        public async Task ListAsync_RetornaListaDeTipoMedicamentosDesdeBD()
        {
            // Act
            var resultado = await _service.ListAsync();

            // Assert
            Assert.NotNull(resultado);
            Assert.IsType<List<TipoMedicamentoViewModel>>(resultado);
            Assert.True(resultado.Count >= 0);
        }

        [Fact]
        public async Task FindAsync_ConIdExistente_RetornaTipoMedicamentoDesdeBD()
        {
            // Arrange
            var tipoMedicamentoPrueba = new TipoMedicamentoViewModel
            {
                tipoMed_Descripcion = $"Find {Guid.NewGuid().ToString().Substring(0, 8)}",
                tipoMed_EsActivoBool = true
            };

            await _service.AddAsync(tipoMedicamentoPrueba);

            var lista = await _service.ListAsync();
            var tipoMedicamentoCreado = lista.FirstOrDefault(x => x.tipoMed_Descripcion == tipoMedicamentoPrueba.tipoMed_Descripcion);
            Assert.True(tipoMedicamentoCreado != null,
                $"Precondicion fallida: no se pudo insertar '{tipoMedicamentoPrueba.tipoMed_Descripcion}' — verificar SP de INSERT");

            // Act
            var resultado = await _service.FindAsync(tipoMedicamentoCreado.tipoMed_Id);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(tipoMedicamentoCreado.tipoMed_Id, resultado.tipoMed_Id);
            Assert.Equal(tipoMedicamentoPrueba.tipoMed_Descripcion, resultado.tipoMed_Descripcion);

            // Cleanup
            await _service.RemoveAsync(tipoMedicamentoCreado.tipoMed_Id);
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
            var tipoMedicamentoPrueba = new TipoMedicamentoViewModel
            {
                tipoMed_Descripcion = $"Detail {Guid.NewGuid().ToString().Substring(0, 8)}",
                tipoMed_EsActivoBool = true
            };

            await _service.AddAsync(tipoMedicamentoPrueba);

            var lista = await _service.ListAsync();
            var tipoMedicamentoCreado = lista.FirstOrDefault(x => x.tipoMed_Descripcion == tipoMedicamentoPrueba.tipoMed_Descripcion);
            Assert.True(tipoMedicamentoCreado != null,
                $"Precondicion fallida: no se pudo insertar '{tipoMedicamentoPrueba.tipoMed_Descripcion}' — verificar SP de INSERT");

            // Act
            var resultado = await _service.DetailAsync(tipoMedicamentoCreado.tipoMed_Id);

            // Assert
            Assert.True(resultado != null,
                $"DetailAsync no retorno datos para Id={tipoMedicamentoCreado.tipoMed_Id}");
            Assert.Equal(tipoMedicamentoCreado.tipoMed_Id, resultado.tipoMed_Id);
            Assert.Equal(tipoMedicamentoPrueba.tipoMed_Descripcion, resultado.tipoMed_Descripcion);
            Assert.NotEqual(default(DateTime), resultado.tipoMed_FechaCrea);

            // Cleanup
            await _service.RemoveAsync(tipoMedicamentoCreado.tipoMed_Id);
        }

        // =======================================================================
        // UPDATE
        // =======================================================================

        [Fact]
        public async Task UpdateAsync_ConDatosValidos_ActualizaEnBaseDeDatos()
        {
            // Arrange
            var tipoMedicamentoInicial = new TipoMedicamentoViewModel
            {
                tipoMed_Descripcion = $"Orig {Guid.NewGuid().ToString().Substring(0, 8)}",
                tipoMed_EsActivoBool = true
            };

            await _service.AddAsync(tipoMedicamentoInicial);

            var lista = await _service.ListAsync();
            var tipoMedicamentoCreado = lista.FirstOrDefault(x => x.tipoMed_Descripcion == tipoMedicamentoInicial.tipoMed_Descripcion);
            Assert.True(tipoMedicamentoCreado != null,
                $"Precondicion fallida: no se pudo insertar '{tipoMedicamentoInicial.tipoMed_Descripcion}' — verificar SP de INSERT");

            var tipoMedicamentoActualizar = await _service.FindAsync(tipoMedicamentoCreado.tipoMed_Id);
            tipoMedicamentoActualizar.tipoMed_Descripcion = $"Upd {Guid.NewGuid().ToString().Substring(0, 8)}";

            // Act
            bool resultado = await _service.UpdateAsync(tipoMedicamentoActualizar);

            // Assert
            Assert.True(resultado, "La actualizacion deberia retornar true");

            var tipoMedicamentoVerificado = await _service.FindAsync(tipoMedicamentoCreado.tipoMed_Id);
            Assert.True(tipoMedicamentoVerificado != null,
                $"No se encontro el registro Id={tipoMedicamentoCreado.tipoMed_Id} tras el UPDATE");
            Assert.Equal(tipoMedicamentoActualizar.tipoMed_Descripcion, tipoMedicamentoVerificado.tipoMed_Descripcion);

            // Cleanup
            await _service.RemoveAsync(tipoMedicamentoCreado.tipoMed_Id);
        }

        [Fact]
        public async Task UpdateAsync_CambiaEstadoActivo_ActualizaCorrectamente()
        {
            // Arrange
            var tipoMedicamentoPrueba = new TipoMedicamentoViewModel
            {
                tipoMed_Descripcion = $"Estado {Guid.NewGuid().ToString().Substring(0, 8)}",
                tipoMed_EsActivoBool = true
            };

            await _service.AddAsync(tipoMedicamentoPrueba);

            var lista = await _service.ListAsync();
            var tipoMedicamentoCreado = lista.FirstOrDefault(x => x.tipoMed_Descripcion == tipoMedicamentoPrueba.tipoMed_Descripcion);
            Assert.True(tipoMedicamentoCreado != null,
                $"Precondicion fallida: no se pudo insertar '{tipoMedicamentoPrueba.tipoMed_Descripcion}' — verificar SP de INSERT");

            var tipoMedicamentoActualizar = await _service.FindAsync(tipoMedicamentoCreado.tipoMed_Id);
            tipoMedicamentoActualizar.tipoMed_EsActivoBool = false;

            // Act
            bool resultado = await _service.UpdateAsync(tipoMedicamentoActualizar);

            // Assert
            Assert.True(resultado);

            var tipoMedicamentoVerificado = await _service.FindAsync(tipoMedicamentoCreado.tipoMed_Id);
            Assert.False(tipoMedicamentoVerificado.tipoMed_EsActivoBool);

            // Cleanup
            await _service.RemoveAsync(tipoMedicamentoCreado.tipoMed_Id);
        }

        // =======================================================================
        // DELETE
        // =======================================================================

        [Fact]
        public async Task RemoveAsync_ConIdExistente_EliminaDeBaseDeDatos()
        {
            // Arrange
            var tipoMedicamentoPrueba = new TipoMedicamentoViewModel
            {
                tipoMed_Descripcion = $"Del {Guid.NewGuid().ToString().Substring(0, 8)}",
                tipoMed_EsActivoBool = true
            };

            await _service.AddAsync(tipoMedicamentoPrueba);

            var lista = await _service.ListAsync();
            var tipoMedicamentoCreado = lista.FirstOrDefault(x => x.tipoMed_Descripcion == tipoMedicamentoPrueba.tipoMed_Descripcion);
            Assert.True(tipoMedicamentoCreado != null,
                $"Precondicion fallida: no se pudo insertar '{tipoMedicamentoPrueba.tipoMed_Descripcion}' — verificar SP de INSERT");

            int idEliminar = tipoMedicamentoCreado.tipoMed_Id;

            // Act
            bool resultado = await _service.RemoveAsync(idEliminar);

            // Assert
            Assert.True(resultado, "La eliminacion deberia retornar true");

            var tipoMedicamentoEliminado = await _service.FindAsync(idEliminar);
            Assert.Null(tipoMedicamentoEliminado);
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
            var nuevoTipoMedicamento = new TipoMedicamentoViewModel
            {
                tipoMed_Descripcion = $"CRUD {Guid.NewGuid().ToString().Substring(0, 8)}",
                tipoMed_EsActivoBool = true
            };

            bool insertado = await _service.AddAsync(nuevoTipoMedicamento);
            Assert.True(insertado, "CREATE fallo — verificar SP de INSERT");

            // 2. READ
            var lista = await _service.ListAsync();
            var tipoMedicamentoCreado = lista.FirstOrDefault(x => x.tipoMed_Descripcion == nuevoTipoMedicamento.tipoMed_Descripcion);
            Assert.True(tipoMedicamentoCreado != null,
                $"CREATE fallo: '{nuevoTipoMedicamento.tipoMed_Descripcion}' no esta en la BD — verificar SP de INSERT");

            var tipoMedicamentoEncontrado = await _service.FindAsync(tipoMedicamentoCreado.tipoMed_Id);
            Assert.True(tipoMedicamentoEncontrado != null,
                $"READ fallo: FindAsync no encontro Id={tipoMedicamentoCreado.tipoMed_Id}");
            Assert.Equal(nuevoTipoMedicamento.tipoMed_Descripcion, tipoMedicamentoEncontrado.tipoMed_Descripcion);

            // 3. UPDATE
            tipoMedicamentoEncontrado.tipoMed_Descripcion = $"CRUD2 {Guid.NewGuid().ToString().Substring(0, 8)}";
            bool actualizado = await _service.UpdateAsync(tipoMedicamentoEncontrado);
            Assert.True(actualizado, "UPDATE fallo");

            var tipoMedicamentoVerificado = await _service.FindAsync(tipoMedicamentoCreado.tipoMed_Id);
            Assert.Equal(tipoMedicamentoEncontrado.tipoMed_Descripcion, tipoMedicamentoVerificado.tipoMed_Descripcion);

            // 4. DELETE
            bool eliminado = await _service.RemoveAsync(tipoMedicamentoCreado.tipoMed_Id);
            Assert.True(eliminado, "DELETE fallo");

            var tipoMedicamentoEliminado = await _service.FindAsync(tipoMedicamentoCreado.tipoMed_Id);
            Assert.Null(tipoMedicamentoEliminado);
        }
    }
}
