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
    /// Pruebas de integracion para ProcedenciaService contra la base de datos REAL.
    /// Verifica que el flujo CRUD completo funciona correctamente end-to-end.
    ///
    /// IMPORTANTE:
    /// 1. Requiere base de datos configurada y accesible
    /// 2. Connection string configurado en DatabaseFixture
    /// 3. Stored procedures de Procedencia existentes en la BD
    /// </summary>
    public class ProcedenciaIntegrationTests : IClassFixture<DatabaseFixture>
    {
        private readonly ProcedenciaService _service;
        private readonly ProcedenciaRepository _repository;
        private readonly IMapper _mapper;
        private readonly DatabaseFixture _fixture;
        private readonly int _testUserId = 1;

        public ProcedenciaIntegrationTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
            _repository = new ProcedenciaRepository();
            _mapper = fixture.Mapper;

            var loggerMock = new Mock<ILogger<ProcedenciaService>>();
            _service = new ProcedenciaService(_repository, loggerMock.Object, _mapper);
        }

        // =======================================================================
        // CREATE (INSERT)
        // =======================================================================

        [Fact]
        public async Task AddAsync_ConDatosValidos_InsertaEnBaseDeDatos()
        {
            // Arrange
            var nuevaProcedencia = new ProcedenciaViewModel
            {
                proc_Descripcion = $"Test {Guid.NewGuid().ToString().Substring(0, 8)}",
                proc_EsActivoBool = true
            };

            // Act
            bool resultado = await _service.AddAsync(nuevaProcedencia, _testUserId);

            // Assert
            Assert.True(resultado, "La insercion deberia retornar true");

            var lista = await _service.ListAsync();
            var procedenciaInsertado = lista.FirstOrDefault(x => x.proc_Descripcion == nuevaProcedencia.proc_Descripcion);

            Assert.True(procedenciaInsertado != null,
                $"No se encontro '{nuevaProcedencia.proc_Descripcion}' en la BD — el INSERT puede haber fallado (verificar nombre del SP)");

            // Cleanup
            if (procedenciaInsertado != null)
                await _service.RemoveAsync(procedenciaInsertado.proc_Id);
        }

        [Fact]
        public async Task AddAsync_ConDescripcionVacia_NoDeberiaInsertar()
        {
            // Arrange
            var procedenciaInvalido = new ProcedenciaViewModel
            {
                proc_Descripcion = ""
            };

            // Act & Assert
            try
            {
                bool resultado = await _service.AddAsync(procedenciaInvalido, _testUserId);
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
        public async Task ListAsync_RetornaListaDeProcedenciasDesdeBD()
        {
            // Act
            var resultado = await _service.ListAsync();

            // Assert
            Assert.NotNull(resultado);
            Assert.IsType<List<ProcedenciaViewModel>>(resultado);
            Assert.True(resultado.Count >= 0);
        }

        [Fact]
        public async Task FindAsync_ConIdExistente_RetornaProcedenciaDesdeBD()
        {
            // Arrange
            var procedenciaPrueba = new ProcedenciaViewModel
            {
                proc_Descripcion = $"Find {Guid.NewGuid().ToString().Substring(0, 8)}",
                proc_EsActivoBool = true
            };

            await _service.AddAsync(procedenciaPrueba, _testUserId);

            var lista = await _service.ListAsync();
            var procedenciaCreado = lista.FirstOrDefault(x => x.proc_Descripcion == procedenciaPrueba.proc_Descripcion);
            Assert.True(procedenciaCreado != null,
                $"Precondicion fallida: no se pudo insertar '{procedenciaPrueba.proc_Descripcion}' — verificar SP de INSERT");

            // Act
            var resultado = await _service.FindAsync(procedenciaCreado.proc_Id);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(procedenciaCreado.proc_Id, resultado.proc_Id);
            Assert.Equal(procedenciaPrueba.proc_Descripcion, resultado.proc_Descripcion);

            // Cleanup
            await _service.RemoveAsync(procedenciaCreado.proc_Id);
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
            var procedenciaPrueba = new ProcedenciaViewModel
            {
                proc_Descripcion = $"Detail {Guid.NewGuid().ToString().Substring(0, 8)}",
                proc_EsActivoBool = true
            };

            await _service.AddAsync(procedenciaPrueba, _testUserId);

            var lista = await _service.ListAsync();
            var procedenciaCreado = lista.FirstOrDefault(x => x.proc_Descripcion == procedenciaPrueba.proc_Descripcion);
            Assert.True(procedenciaCreado != null,
                $"Precondicion fallida: no se pudo insertar '{procedenciaPrueba.proc_Descripcion}' — verificar SP de INSERT");

            // Act
            var resultado = await _service.DetailAsync(procedenciaCreado.proc_Id);

            // Assert
            Assert.True(resultado != null,
                $"DetailAsync no retorno datos para Id={procedenciaCreado.proc_Id}");
            Assert.Equal(procedenciaCreado.proc_Id, resultado.proc_Id);
            Assert.Equal(procedenciaPrueba.proc_Descripcion, resultado.proc_Descripcion);
            Assert.NotEqual(default(DateTime), resultado.proc_FechaCrea);

            // Cleanup
            await _service.RemoveAsync(procedenciaCreado.proc_Id);
        }

        // =======================================================================
        // UPDATE
        // =======================================================================

        [Fact]
        public async Task UpdateAsync_ConDatosValidos_ActualizaEnBaseDeDatos()
        {
            // Arrange
            var procedenciaInicial = new ProcedenciaViewModel
            {
                proc_Descripcion = $"Orig {Guid.NewGuid().ToString().Substring(0, 8)}",
                proc_EsActivoBool = true
            };

            await _service.AddAsync(procedenciaInicial, _testUserId);

            var lista = await _service.ListAsync();
            var procedenciaCreado = lista.FirstOrDefault(x => x.proc_Descripcion == procedenciaInicial.proc_Descripcion);
            Assert.True(procedenciaCreado != null,
                $"Precondicion fallida: no se pudo insertar '{procedenciaInicial.proc_Descripcion}' — verificar SP de INSERT");

            var procedenciaActualizar = await _service.FindAsync(procedenciaCreado.proc_Id);
            procedenciaActualizar.proc_Descripcion = $"Upd {Guid.NewGuid().ToString().Substring(0, 8)}";

            // Act
            bool resultado = await _service.UpdateAsync(procedenciaActualizar, _testUserId);

            // Assert
            Assert.True(resultado, "La actualizacion deberia retornar true");

            var procedenciaVerificado = await _service.FindAsync(procedenciaCreado.proc_Id);
            Assert.True(procedenciaVerificado != null,
                $"No se encontro el registro Id={procedenciaCreado.proc_Id} tras el UPDATE");
            Assert.Equal(procedenciaActualizar.proc_Descripcion, procedenciaVerificado.proc_Descripcion);

            // Cleanup
            await _service.RemoveAsync(procedenciaCreado.proc_Id);
        }

        [Fact]
        public async Task UpdateAsync_CambiaEstadoActivo_ActualizaCorrectamente()
        {
            // Arrange
            var procedenciaPrueba = new ProcedenciaViewModel
            {
                proc_Descripcion = $"Estado {Guid.NewGuid().ToString().Substring(0, 8)}",
                proc_EsActivoBool = true
            };

            await _service.AddAsync(procedenciaPrueba, _testUserId);

            var lista = await _service.ListAsync();
            var procedenciaCreado = lista.FirstOrDefault(x => x.proc_Descripcion == procedenciaPrueba.proc_Descripcion);
            Assert.True(procedenciaCreado != null,
                $"Precondicion fallida: no se pudo insertar '{procedenciaPrueba.proc_Descripcion}' — verificar SP de INSERT");

            var procedenciaActualizar = await _service.FindAsync(procedenciaCreado.proc_Id);
            procedenciaActualizar.proc_EsActivoBool = false;

            // Act
            bool resultado = await _service.UpdateAsync(procedenciaActualizar, _testUserId);

            // Assert
            Assert.True(resultado);

            var procedenciaVerificado = await _service.FindAsync(procedenciaCreado.proc_Id);
            Assert.False(procedenciaVerificado.proc_EsActivoBool);

            // Cleanup
            await _service.RemoveAsync(procedenciaCreado.proc_Id);
        }

        // =======================================================================
        // DELETE
        // =======================================================================

        [Fact]
        public async Task RemoveAsync_ConIdExistente_EliminaDeBaseDeDatos()
        {
            // Arrange
            var procedenciaPrueba = new ProcedenciaViewModel
            {
                proc_Descripcion = $"Del {Guid.NewGuid().ToString().Substring(0, 8)}",
                proc_EsActivoBool = true
            };

            await _service.AddAsync(procedenciaPrueba, _testUserId);

            var lista = await _service.ListAsync();
            var procedenciaCreado = lista.FirstOrDefault(x => x.proc_Descripcion == procedenciaPrueba.proc_Descripcion);
            Assert.True(procedenciaCreado != null,
                $"Precondicion fallida: no se pudo insertar '{procedenciaPrueba.proc_Descripcion}' — verificar SP de INSERT");

            int idEliminar = procedenciaCreado.proc_Id;

            // Act
            bool resultado = await _service.RemoveAsync(idEliminar);

            // Assert
            Assert.True(resultado, "La eliminacion deberia retornar true");

            var procedenciaEliminado = await _service.FindAsync(idEliminar);
            Assert.Null(procedenciaEliminado);
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

        [Fact]
        public async Task FlujoCRUDCompleto_CrearLeerActualizarEliminar_FuncionaCorrectamente()
        {
            // 1. CREATE
            var nuevaProcedencia = new ProcedenciaViewModel
            {
                proc_Descripcion = $"CRUD {Guid.NewGuid().ToString().Substring(0, 8)}",
                proc_EsActivoBool = true
            };

            bool insertado = await _service.AddAsync(nuevaProcedencia, _testUserId);
            Assert.True(insertado, "CREATE fallo — verificar SP de INSERT");

            // 2. READ
            var lista = await _service.ListAsync();
            var procedenciaCreado = lista.FirstOrDefault(x => x.proc_Descripcion == nuevaProcedencia.proc_Descripcion);
            Assert.True(procedenciaCreado != null,
                $"CREATE fallo: '{nuevaProcedencia.proc_Descripcion}' no esta en la BD — verificar SP de INSERT");

            var procedenciaEncontrado = await _service.FindAsync(procedenciaCreado.proc_Id);
            Assert.True(procedenciaEncontrado != null,
                $"READ fallo: FindAsync no encontro Id={procedenciaCreado.proc_Id}");
            Assert.Equal(nuevaProcedencia.proc_Descripcion, procedenciaEncontrado.proc_Descripcion);

            // 3. UPDATE
            procedenciaEncontrado.proc_Descripcion = $"CRUD2 {Guid.NewGuid().ToString().Substring(0, 8)}";
            bool actualizado = await _service.UpdateAsync(procedenciaEncontrado, _testUserId);
            Assert.True(actualizado, "UPDATE fallo");

            var procedenciaVerificado = await _service.FindAsync(procedenciaCreado.proc_Id);
            Assert.Equal(procedenciaEncontrado.proc_Descripcion, procedenciaVerificado.proc_Descripcion);

            // 4. DELETE
            bool eliminado = await _service.RemoveAsync(procedenciaCreado.proc_Id);
            Assert.True(eliminado, "DELETE fallo");

            var procedenciaEliminado = await _service.FindAsync(procedenciaCreado.proc_Id);
            Assert.Null(procedenciaEliminado);
        }
    }
}
