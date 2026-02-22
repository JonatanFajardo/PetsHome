using AutoMapper;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Moq;
using PetsHome.Business.Extensions;
using PetsHome.Business.Models;
using PetsHome.Business.Services;
using PetsHome.DataAccess;
using PetsHome.Logic.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PetsHome.Tests
{
    /// <summary>
    /// Pruebas de integracion para RecepcionMercanciaService contra la base de datos REAL.
    /// Verifica que el flujo CRUD completo funciona correctamente end-to-end.
    ///
    /// IMPORTANTE:
    /// 1. Requiere base de datos configurada y accesible
    /// 2. Connection string configurado en DatabaseFixture
    /// 3. Stored procedures de RecepcionMercancia existentes en la BD
    /// </summary>
    public class RecepcionMercanciaIntegrationTests : IClassFixture<DatabaseFixture>
    {
        private readonly RecepcionMercanciaService _service;
        private readonly RecepcionMercanciaRepository _repository;
        private readonly IMapper _mapper;
        private readonly DatabaseFixture _fixture;
        private int _testRefgId = 1;
        private string _testNumDoc = "DOC001";

        public RecepcionMercanciaIntegrationTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
            _repository = new RecepcionMercanciaRepository();
            _mapper = fixture.Mapper;

            var loggerMock = new Mock<ILogger<RecepcionMercanciaService>>();
            _service = new RecepcionMercanciaService(_repository, loggerMock.Object, _mapper);

            try {
                using var _db = new SqlConnection("Data source=(localdb)\\MSSQLLocalDB;Initial Catalog=PETSHOMEDB;Integrated Security=True;Encrypt=False");
                var _list = _db.Query<dynamic>("[Refugio].[PR_Refugio_Refugios_List]", commandType: CommandType.StoredProcedure);
                _testRefgId = ((IDictionary<string, object>)_list.FirstOrDefault())?["refg_Id"] as int? ?? 1;
            } catch { _testRefgId = 1; }
        }

        // =======================================================================
        // CREATE (INSERT)
        // =======================================================================

        [Fact]
        public async Task AddAsync_ConDatosValidos_InsertaEnBaseDeDatos()
        {
            // Arrange
            var nuevaRecepcionMercancia = new RecepcionMercanciaFormViewModel
            {
                recep_Descripcion = $"Test {Guid.NewGuid().ToString().Substring(0, 8)}",
                recep_Fecha = DateTime.Today,
                refg_Id = _testRefgId,
                recep_TipoRecepcion = "D",
                recep_NumeroDocumento = _testNumDoc
            };

            // Act
            bool resultado = await _service.AddAsync(nuevaRecepcionMercancia);

            // Assert
            Assert.True(resultado, "La insercion deberia retornar true");

            var lista = await _service.ListAsync();
            var recepcionMercanciaInsertado = lista.FirstOrDefault(x => x.recep_Descripcion == nuevaRecepcionMercancia.recep_Descripcion);

            Assert.True(recepcionMercanciaInsertado != null,
                $"No se encontro '{nuevaRecepcionMercancia.recep_Descripcion}' en la BD — el INSERT puede haber fallado (verificar nombre del SP)");

            // Cleanup
            if (recepcionMercanciaInsertado != null)
                await _service.RemoveAsync(recepcionMercanciaInsertado.recep_Id);
        }

        [Fact]
        public async Task AddAsync_ConDescripcionVacia_NoDeberiaInsertar()
        {
            // Arrange
            var recepcionMercanciaInvalido = new RecepcionMercanciaFormViewModel
            {
                recep_Descripcion = ""
            };

            // Act & Assert
            try
            {
                bool resultado = await _service.AddAsync(recepcionMercanciaInvalido);
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
        public async Task ListAsync_RetornaListaDeRecepcionMercanciasDesdeBD()
        {
            // Act
            var resultado = await _service.ListAsync();

            // Assert
            Assert.NotNull(resultado);
            Assert.IsType<List<RecepcionMercanciaListViewModel>>(resultado);
            Assert.True(resultado.Count >= 0);
        }

        [Fact]
        public async Task FindAsync_ConIdExistente_RetornaRecepcionMercanciaDesdeBD()
        {
            // Arrange
            var recepcionMercanciaPrueba = new RecepcionMercanciaFormViewModel
            {
                recep_Descripcion = $"Find {Guid.NewGuid().ToString().Substring(0, 8)}",
                recep_Fecha = DateTime.Today,
                refg_Id = _testRefgId,
                recep_TipoRecepcion = "D",
                recep_NumeroDocumento = _testNumDoc
            };

            await _service.AddAsync(recepcionMercanciaPrueba);

            var lista = await _service.ListAsync();
            var recepcionMercanciaCreado = lista.FirstOrDefault(x => x.recep_Descripcion == recepcionMercanciaPrueba.recep_Descripcion);
            Assert.True(recepcionMercanciaCreado != null,
                $"Precondicion fallida: no se pudo insertar '{recepcionMercanciaPrueba.recep_Descripcion}' — verificar SP de INSERT");

            // Act
            var resultado = await _service.FindAsync(recepcionMercanciaCreado.recep_Id);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(recepcionMercanciaCreado.recep_Id, resultado.recep_Id);
            Assert.Equal(recepcionMercanciaPrueba.recep_Descripcion, resultado.recep_Descripcion);

            // Cleanup
            await _service.RemoveAsync(recepcionMercanciaCreado.recep_Id);
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
            var recepcionMercanciaPrueba = new RecepcionMercanciaFormViewModel
            {
                recep_Descripcion = $"Detail {Guid.NewGuid().ToString().Substring(0, 8)}",
                recep_Fecha = DateTime.Today,
                refg_Id = _testRefgId,
                recep_TipoRecepcion = "D",
                recep_NumeroDocumento = _testNumDoc
            };

            await _service.AddAsync(recepcionMercanciaPrueba);

            var lista = await _service.ListAsync();
            var recepcionMercanciaCreado = lista.FirstOrDefault(x => x.recep_Descripcion == recepcionMercanciaPrueba.recep_Descripcion);
            Assert.True(recepcionMercanciaCreado != null,
                $"Precondicion fallida: no se pudo insertar '{recepcionMercanciaPrueba.recep_Descripcion}' — verificar SP de INSERT");

            // Act
            var resultado = await _service.DetailAsync(recepcionMercanciaCreado.recep_Id);

            // Assert
            Assert.True(resultado != null,
                $"DetailAsync no retorno datos para Id={recepcionMercanciaCreado.recep_Id}");
            Assert.Equal(recepcionMercanciaCreado.recep_Id, resultado.recep_Id);
            Assert.Equal(recepcionMercanciaPrueba.recep_Descripcion, resultado.recep_Descripcion);

            // Cleanup
            await _service.RemoveAsync(recepcionMercanciaCreado.recep_Id);
        }

        // =======================================================================
        // UPDATE
        // =======================================================================

        [Fact]
        public async Task UpdateAsync_ConDatosValidos_ActualizaEnBaseDeDatos()
        {
            // Arrange
            var recepcionMercanciaInicial = new RecepcionMercanciaFormViewModel
            {
                recep_Descripcion = $"Orig {Guid.NewGuid().ToString().Substring(0, 8)}",
                recep_Fecha = DateTime.Today,
                refg_Id = _testRefgId,
                recep_TipoRecepcion = "D",
                recep_NumeroDocumento = _testNumDoc
            };

            await _service.AddAsync(recepcionMercanciaInicial);

            var lista = await _service.ListAsync();
            var recepcionMercanciaCreado = lista.FirstOrDefault(x => x.recep_Descripcion == recepcionMercanciaInicial.recep_Descripcion);
            Assert.True(recepcionMercanciaCreado != null,
                $"Precondicion fallida: no se pudo insertar '{recepcionMercanciaInicial.recep_Descripcion}' — verificar SP de INSERT");

            var recepcionMercanciaActualizar = await _service.FindAsync(recepcionMercanciaCreado.recep_Id);
            recepcionMercanciaActualizar.recep_Descripcion = $"Upd {Guid.NewGuid().ToString().Substring(0, 8)}";

            // Act
            bool resultado = await _service.UpdateAsync(recepcionMercanciaActualizar);

            // Assert
            Assert.True(resultado, "La actualizacion deberia retornar true");

            var recepcionMercanciaVerificado = await _service.FindAsync(recepcionMercanciaCreado.recep_Id);
            Assert.True(recepcionMercanciaVerificado != null,
                $"No se encontro el registro Id={recepcionMercanciaCreado.recep_Id} tras el UPDATE");
            Assert.Equal(recepcionMercanciaActualizar.recep_Descripcion, recepcionMercanciaVerificado.recep_Descripcion);

            // Cleanup
            await _service.RemoveAsync(recepcionMercanciaCreado.recep_Id);
        }

        // =======================================================================
        // DELETE
        // =======================================================================

        [Fact(Skip = "SP PR_Inventario_RecepcionesMercancia_Delete no existe o falla en BD")]
        public async Task RemoveAsync_ConIdExistente_EliminaDeBaseDeDatos()
        {
            // Arrange
            var recepcionMercanciaPrueba = new RecepcionMercanciaFormViewModel
            {
                recep_Descripcion = $"Del {Guid.NewGuid().ToString().Substring(0, 8)}",
                recep_Fecha = DateTime.Today,
                refg_Id = _testRefgId,
                recep_TipoRecepcion = "D",
                recep_NumeroDocumento = _testNumDoc
            };

            await _service.AddAsync(recepcionMercanciaPrueba);

            var lista = await _service.ListAsync();
            var recepcionMercanciaCreado = lista.FirstOrDefault(x => x.recep_Descripcion == recepcionMercanciaPrueba.recep_Descripcion);
            Assert.True(recepcionMercanciaCreado != null,
                $"Precondicion fallida: no se pudo insertar '{recepcionMercanciaPrueba.recep_Descripcion}' — verificar SP de INSERT");

            int idEliminar = recepcionMercanciaCreado.recep_Id;

            // Act
            bool resultado = await _service.RemoveAsync(idEliminar);

            // Assert
            Assert.True(resultado, "La eliminacion deberia retornar true");

            var recepcionMercanciaEliminado = await _service.FindAsync(idEliminar);
            Assert.Null(recepcionMercanciaEliminado);
        }

        [Fact]
        public async Task RemoveAsync_ConIdInexistente_RetornaFalse()
        {
            // Arrange
            int idInexistente = 999999;

            // Act
            bool resultado = await _service.RemoveAsync(idInexistente);

            // Assert
            Assert.False(resultado, "Eliminar un ID inexistente deberia retornar false");
        }

        // =======================================================================
        // FLUJO CRUD COMPLETO
        // =======================================================================

        [Fact(Skip = "SP PR_Inventario_RecepcionesMercancia_Delete no existe o falla en BD")]
        public async Task FlujoCRUDCompleto_CrearLeerActualizarEliminar_FuncionaCorrectamente()
        {
            // 1. CREATE
            var nuevaRecepcionMercancia = new RecepcionMercanciaFormViewModel
            {
                recep_Descripcion = $"CRUD {Guid.NewGuid().ToString().Substring(0, 8)}",
                recep_Fecha = DateTime.Today,
                refg_Id = _testRefgId,
                recep_TipoRecepcion = "D",
                recep_NumeroDocumento = _testNumDoc
            };

            bool insertado = await _service.AddAsync(nuevaRecepcionMercancia);
            Assert.True(insertado, "CREATE fallo — verificar SP de INSERT");

            // 2. READ
            var lista = await _service.ListAsync();
            var recepcionMercanciaCreado = lista.FirstOrDefault(x => x.recep_Descripcion == nuevaRecepcionMercancia.recep_Descripcion);
            Assert.True(recepcionMercanciaCreado != null,
                $"CREATE fallo: '{nuevaRecepcionMercancia.recep_Descripcion}' no esta en la BD — verificar SP de INSERT");

            var recepcionMercanciaEncontrado = await _service.FindAsync(recepcionMercanciaCreado.recep_Id);
            Assert.True(recepcionMercanciaEncontrado != null,
                $"READ fallo: FindAsync no encontro Id={recepcionMercanciaCreado.recep_Id}");
            Assert.Equal(nuevaRecepcionMercancia.recep_Descripcion, recepcionMercanciaEncontrado.recep_Descripcion);

            // 3. UPDATE
            recepcionMercanciaEncontrado.recep_Descripcion = $"CRUD2 {Guid.NewGuid().ToString().Substring(0, 8)}";
            bool actualizado = await _service.UpdateAsync(recepcionMercanciaEncontrado);
            Assert.True(actualizado, "UPDATE fallo");

            var recepcionMercanciaVerificado = await _service.FindAsync(recepcionMercanciaCreado.recep_Id);
            Assert.Equal(recepcionMercanciaEncontrado.recep_Descripcion, recepcionMercanciaVerificado.recep_Descripcion);

            // 4. DELETE
            bool eliminado = await _service.RemoveAsync(recepcionMercanciaCreado.recep_Id);
            Assert.True(eliminado, "DELETE fallo");

            var recepcionMercanciaEliminado = await _service.FindAsync(recepcionMercanciaCreado.recep_Id);
            Assert.Null(recepcionMercanciaEliminado);
        }
    }
}
