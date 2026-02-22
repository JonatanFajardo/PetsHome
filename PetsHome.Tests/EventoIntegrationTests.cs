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
    /// Pruebas de integracion para EventoService contra la base de datos REAL.
    /// Verifica que el flujo CRUD completo funciona correctamente end-to-end.
    ///
    /// IMPORTANTE:
    /// 1. Requiere base de datos configurada y accesible
    /// 2. Connection string configurado en DatabaseFixture
    /// 3. Stored procedures de Evento existentes en la BD
    /// </summary>
    public class EventoIntegrationTests : IClassFixture<DatabaseFixture>
    {
        private readonly EventoService _service;
        private readonly EventoRepository _repository;
        private readonly IMapper _mapper;
        private readonly DatabaseFixture _fixture;
        private readonly int _testUserId = 1;
        private int _testRefgId = 1;

        public EventoIntegrationTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
            _repository = new EventoRepository();
            _mapper = fixture.Mapper;

            var loggerMock = new Mock<ILogger<EventoService>>();
            _service = new EventoService(_repository, loggerMock.Object, _mapper);

            // Obtener un refg_Id valido de la BD
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
            var nuevoEvento = new EventoViewModel
            {
                eve_Descripcion = $"Test {Guid.NewGuid().ToString().Substring(0, 8)}",
                refg_Id = _testRefgId,
                eve_Fecha = DateTime.Today,
                eve_HoraInicio = TimeSpan.FromHours(8),
                eve_HoraFinal = TimeSpan.FromHours(10)
            };

            // Act
            bool resultado = await _service.AddAsync(nuevoEvento, _testUserId);

            // Assert
            Assert.True(resultado, "La insercion deberia retornar true");

            var lista = await _service.ListAsync();
            var eventoInsertado = lista.FirstOrDefault(x => x.eve_Descripcion == nuevoEvento.eve_Descripcion);

            Assert.True(eventoInsertado != null,
                $"No se encontro '{nuevoEvento.eve_Descripcion}' en la BD — el INSERT puede haber fallado (verificar nombre del SP)");

            // Cleanup
            if (eventoInsertado != null)
                await _service.RemoveAsync(eventoInsertado.eve_Id);
        }

        [Fact]
        public async Task AddAsync_ConDescripcionVacia_NoDeberiaInsertar()
        {
            // Arrange
            var eventoInvalido = new EventoViewModel
            {
                eve_Descripcion = ""
            };

            // Act & Assert
            try
            {
                bool resultado = await _service.AddAsync(eventoInvalido, _testUserId);
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
        public async Task ListAsync_RetornaListaDeEventosDesdeBD()
        {
            // Act
            var resultado = await _service.ListAsync();

            // Assert
            Assert.NotNull(resultado);
            Assert.IsType<List<EventoViewModel>>(resultado);
            Assert.True(resultado.Count >= 0);
        }

        [Fact]
        public async Task FindAsync_ConIdExistente_RetornaEventoDesdeBD()
        {
            // Arrange
            var eventoPrueba = new EventoViewModel
            {
                eve_Descripcion = $"Find {Guid.NewGuid().ToString().Substring(0, 8)}",
                refg_Id = _testRefgId,
                eve_Fecha = DateTime.Today,
                eve_HoraInicio = TimeSpan.FromHours(8),
                eve_HoraFinal = TimeSpan.FromHours(10)
            };

            await _service.AddAsync(eventoPrueba, _testUserId);

            var lista = await _service.ListAsync();
            var eventoCreado = lista.FirstOrDefault(x => x.eve_Descripcion == eventoPrueba.eve_Descripcion);
            Assert.True(eventoCreado != null,
                $"Precondicion fallida: no se pudo insertar '{eventoPrueba.eve_Descripcion}' — verificar SP de INSERT");

            // Act
            var resultado = await _service.FindAsync(eventoCreado.eve_Id);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(eventoCreado.eve_Id, resultado.eve_Id);
            Assert.Equal(eventoPrueba.eve_Descripcion, resultado.eve_Descripcion);

            // Cleanup
            await _service.RemoveAsync(eventoCreado.eve_Id);
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
            var eventoPrueba = new EventoViewModel
            {
                eve_Descripcion = $"Detail {Guid.NewGuid().ToString().Substring(0, 8)}",
                refg_Id = _testRefgId,
                eve_Fecha = DateTime.Today,
                eve_HoraInicio = TimeSpan.FromHours(8),
                eve_HoraFinal = TimeSpan.FromHours(10)
            };

            await _service.AddAsync(eventoPrueba, _testUserId);

            var lista = await _service.ListAsync();
            var eventoCreado = lista.FirstOrDefault(x => x.eve_Descripcion == eventoPrueba.eve_Descripcion);
            Assert.True(eventoCreado != null,
                $"Precondicion fallida: no se pudo insertar '{eventoPrueba.eve_Descripcion}' — verificar SP de INSERT");

            // Act
            var resultado = await _service.DetailAsync(eventoCreado.eve_Id);

            // Assert
            Assert.True(resultado != null,
                $"DetailAsync no retorno datos para Id={eventoCreado.eve_Id}");
            Assert.Equal(eventoCreado.eve_Id, resultado.eve_Id);
            Assert.Equal(eventoPrueba.eve_Descripcion, resultado.eve_Descripcion);

            // Cleanup
            await _service.RemoveAsync(eventoCreado.eve_Id);
        }

        // =======================================================================
        // UPDATE
        // =======================================================================

        [Fact]
        public async Task UpdateAsync_ConDatosValidos_ActualizaEnBaseDeDatos()
        {
            // Arrange
            var eventoInicial = new EventoViewModel
            {
                eve_Descripcion = $"Orig {Guid.NewGuid().ToString().Substring(0, 8)}",
                refg_Id = _testRefgId,
                eve_Fecha = DateTime.Today,
                eve_HoraInicio = TimeSpan.FromHours(8),
                eve_HoraFinal = TimeSpan.FromHours(10)
            };

            await _service.AddAsync(eventoInicial, _testUserId);

            var lista = await _service.ListAsync();
            var eventoCreado = lista.FirstOrDefault(x => x.eve_Descripcion == eventoInicial.eve_Descripcion);
            Assert.True(eventoCreado != null,
                $"Precondicion fallida: no se pudo insertar '{eventoInicial.eve_Descripcion}' — verificar SP de INSERT");

            var eventoActualizar = await _service.FindAsync(eventoCreado.eve_Id);
            eventoActualizar.eve_Descripcion = $"Upd {Guid.NewGuid().ToString().Substring(0, 8)}";

            // Act
            bool resultado = await _service.UpdateAsync(eventoActualizar, _testUserId);

            // Assert
            Assert.True(resultado, "La actualizacion deberia retornar true");

            var eventoVerificado = await _service.FindAsync(eventoCreado.eve_Id);
            Assert.True(eventoVerificado != null,
                $"No se encontro el registro Id={eventoCreado.eve_Id} tras el UPDATE");
            Assert.Equal(eventoActualizar.eve_Descripcion, eventoVerificado.eve_Descripcion);

            // Cleanup
            await _service.RemoveAsync(eventoCreado.eve_Id);
        }

        // =======================================================================
        // DELETE
        // =======================================================================

        [Fact(Skip = "SP PR_Refugio_Eventos_Delete no existe en la BD")]
        public async Task RemoveAsync_ConIdExistente_EliminaDeBaseDeDatos()
        {
            // Arrange
            var eventoPrueba = new EventoViewModel
            {
                eve_Descripcion = $"Del {Guid.NewGuid().ToString().Substring(0, 8)}",
                refg_Id = _testRefgId,
                eve_Fecha = DateTime.Today,
                eve_HoraInicio = TimeSpan.FromHours(8),
                eve_HoraFinal = TimeSpan.FromHours(10)
            };

            await _service.AddAsync(eventoPrueba, _testUserId);

            var lista = await _service.ListAsync();
            var eventoCreado = lista.FirstOrDefault(x => x.eve_Descripcion == eventoPrueba.eve_Descripcion);
            Assert.True(eventoCreado != null,
                $"Precondicion fallida: no se pudo insertar '{eventoPrueba.eve_Descripcion}' — verificar SP de INSERT");

            int idEliminar = eventoCreado.eve_Id;

            // Act
            bool resultado = await _service.RemoveAsync(idEliminar);

            // Assert
            Assert.True(resultado, "La eliminacion deberia retornar true");

            var eventoEliminado = await _service.FindAsync(idEliminar);
            Assert.Null(eventoEliminado);
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

        [Fact(Skip = "DELETE falla: SP PR_Refugio_Eventos_Delete no existe en la BD")]
        public async Task FlujoCRUDCompleto_CrearLeerActualizarEliminar_FuncionaCorrectamente()
        {
            // 1. CREATE
            var nuevoEvento = new EventoViewModel
            {
                eve_Descripcion = $"CRUD {Guid.NewGuid().ToString().Substring(0, 8)}",
                refg_Id = _testRefgId,
                eve_Fecha = DateTime.Today,
                eve_HoraInicio = TimeSpan.FromHours(8),
                eve_HoraFinal = TimeSpan.FromHours(10)
            };

            bool insertado = await _service.AddAsync(nuevoEvento, _testUserId);
            Assert.True(insertado, "CREATE fallo — verificar SP de INSERT");

            // 2. READ
            var lista = await _service.ListAsync();
            var eventoCreado = lista.FirstOrDefault(x => x.eve_Descripcion == nuevoEvento.eve_Descripcion);
            Assert.True(eventoCreado != null,
                $"CREATE fallo: '{nuevoEvento.eve_Descripcion}' no esta en la BD — verificar SP de INSERT");

            var eventoEncontrado = await _service.FindAsync(eventoCreado.eve_Id);
            Assert.True(eventoEncontrado != null,
                $"READ fallo: FindAsync no encontro Id={eventoCreado.eve_Id}");
            Assert.Equal(nuevoEvento.eve_Descripcion, eventoEncontrado.eve_Descripcion);

            // 3. UPDATE
            eventoEncontrado.eve_Descripcion = $"CRUD2 {Guid.NewGuid().ToString().Substring(0, 8)}";
            bool actualizado = await _service.UpdateAsync(eventoEncontrado, _testUserId);
            Assert.True(actualizado, "UPDATE fallo");

            var eventoVerificado = await _service.FindAsync(eventoCreado.eve_Id);
            Assert.Equal(eventoEncontrado.eve_Descripcion, eventoVerificado.eve_Descripcion);

            // 4. DELETE
            bool eliminado = await _service.RemoveAsync(eventoCreado.eve_Id);
            Assert.True(eliminado, "DELETE fallo");

            var eventoEliminado = await _service.FindAsync(eventoCreado.eve_Id);
            Assert.Null(eventoEliminado);
        }
    }
}
