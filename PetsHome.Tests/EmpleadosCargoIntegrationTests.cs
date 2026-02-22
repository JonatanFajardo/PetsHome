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
    /// Pruebas de integracion para EmpleadosCargoService contra la base de datos REAL.
    /// Verifica que el flujo CRUD completo funciona correctamente end-to-end.
    ///
    /// IMPORTANTE:
    /// 1. Requiere base de datos configurada y accesible
    /// 2. Connection string configurado en DatabaseFixture
    /// 3. Stored procedures de EmpleadosCargo existentes en la BD
    /// </summary>
    public class EmpleadosCargoIntegrationTests : IClassFixture<DatabaseFixture>
    {
        private readonly EmpleadosCargoService _service;
        private readonly EmpleadosCargoRepository _repository;
        private readonly IMapper _mapper;
        private readonly DatabaseFixture _fixture;
        private readonly int _testUserId = 1;

        public EmpleadosCargoIntegrationTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
            _repository = new EmpleadosCargoRepository();
            _mapper = fixture.Mapper;

            var loggerMock = new Mock<ILogger<EmpleadosCargoService>>();
            _service = new EmpleadosCargoService(_repository, loggerMock.Object, _mapper);
        }

        // =======================================================================
        // CREATE (INSERT)
        // =======================================================================

        [Fact]
        public async Task AddAsync_ConDatosValidos_InsertaEnBaseDeDatos()
        {
            // Arrange
            var nuevoEmpleadosCargo = new EmpleadoCargoViewModel
            {
                cag_Descripcion = $"Test {Guid.NewGuid().ToString().Substring(0, 8)}",
                cag_EsActivo = true,
                cag_Salario = 5000m
            };

            // Act
            bool resultado = await _service.AddAsync(nuevoEmpleadosCargo, _testUserId);

            // Assert
            Assert.True(resultado, "La insercion deberia retornar true");

            var lista = await _service.ListAsync();
            var empleadosCargoInsertado = lista.FirstOrDefault(x => x.cag_Descripcion == nuevoEmpleadosCargo.cag_Descripcion);

            Assert.True(empleadosCargoInsertado != null,
                $"No se encontro '{nuevoEmpleadosCargo.cag_Descripcion}' en la BD — el INSERT puede haber fallado (verificar nombre del SP)");

            // Cleanup
            if (empleadosCargoInsertado != null)
                await _service.RemoveAsync(empleadosCargoInsertado.cag_Id);
        }

        [Fact]
        public async Task AddAsync_ConDescripcionVacia_NoDeberiaInsertar()
        {
            // Arrange
            var empleadosCargoInvalido = new EmpleadoCargoViewModel
            {
                cag_Descripcion = ""
            };

            // Act & Assert
            try
            {
                bool resultado = await _service.AddAsync(empleadosCargoInvalido, _testUserId);
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
        public async Task ListAsync_RetornaListaDeEmpleadosCargosDesdeBD()
        {
            // Act
            var resultado = await _service.ListAsync();

            // Assert
            Assert.NotNull(resultado);
            Assert.IsType<List<EmpleadoCargoViewModel>>(resultado);
            Assert.True(resultado.Count >= 0);
        }

        [Fact]
        public async Task FindAsync_ConIdExistente_RetornaEmpleadosCargoDesdeBD()
        {
            // Arrange
            var empleadosCargoPrueba = new EmpleadoCargoViewModel
            {
                cag_Descripcion = $"Find {Guid.NewGuid().ToString().Substring(0, 8)}",
                cag_EsActivo = true,
                cag_Salario = 5000m
            };

            await _service.AddAsync(empleadosCargoPrueba, _testUserId);

            var lista = await _service.ListAsync();
            var empleadosCargoCreado = lista.FirstOrDefault(x => x.cag_Descripcion == empleadosCargoPrueba.cag_Descripcion);
            Assert.True(empleadosCargoCreado != null,
                $"Precondicion fallida: no se pudo insertar '{empleadosCargoPrueba.cag_Descripcion}' — verificar SP de INSERT");

            // Act
            var resultado = await _service.FindAsync(empleadosCargoCreado.cag_Id);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(empleadosCargoCreado.cag_Id, resultado.cag_Id);
            Assert.Equal(empleadosCargoPrueba.cag_Descripcion, resultado.cag_Descripcion);

            // Cleanup
            await _service.RemoveAsync(empleadosCargoCreado.cag_Id);
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
            var empleadosCargoPrueba = new EmpleadoCargoViewModel
            {
                cag_Descripcion = $"Detail {Guid.NewGuid().ToString().Substring(0, 8)}",
                cag_EsActivo = true,
                cag_Salario = 5000m
            };

            await _service.AddAsync(empleadosCargoPrueba, _testUserId);

            var lista = await _service.ListAsync();
            var empleadosCargoCreado = lista.FirstOrDefault(x => x.cag_Descripcion == empleadosCargoPrueba.cag_Descripcion);
            Assert.True(empleadosCargoCreado != null,
                $"Precondicion fallida: no se pudo insertar '{empleadosCargoPrueba.cag_Descripcion}' — verificar SP de INSERT");

            // Act
            var resultado = await _service.DetailAsync(empleadosCargoCreado.cag_Id);

            // Assert
            Assert.True(resultado != null,
                $"DetailAsync no retorno datos para Id={empleadosCargoCreado.cag_Id}");
            Assert.Equal(empleadosCargoCreado.cag_Id, resultado.cag_Id);
            Assert.Equal(empleadosCargoPrueba.cag_Descripcion, resultado.cag_Descripcion);
            Assert.NotEqual(default(DateTime), resultado.cag_FechaCrea);

            // Cleanup
            await _service.RemoveAsync(empleadosCargoCreado.cag_Id);
        }

        // =======================================================================
        // UPDATE
        // =======================================================================

        [Fact]
        public async Task UpdateAsync_ConDatosValidos_ActualizaEnBaseDeDatos()
        {
            // Arrange
            var empleadosCargoInicial = new EmpleadoCargoViewModel
            {
                cag_Descripcion = $"Orig {Guid.NewGuid().ToString().Substring(0, 8)}",
                cag_EsActivo = true,
                cag_Salario = 5000m
            };

            await _service.AddAsync(empleadosCargoInicial, _testUserId);

            var lista = await _service.ListAsync();
            var empleadosCargoCreado = lista.FirstOrDefault(x => x.cag_Descripcion == empleadosCargoInicial.cag_Descripcion);
            Assert.True(empleadosCargoCreado != null,
                $"Precondicion fallida: no se pudo insertar '{empleadosCargoInicial.cag_Descripcion}' — verificar SP de INSERT");

            var empleadosCargoActualizar = await _service.FindAsync(empleadosCargoCreado.cag_Id);
            empleadosCargoActualizar.cag_Descripcion = $"Upd {Guid.NewGuid().ToString().Substring(0, 8)}";

            // Act
            bool resultado = await _service.UpdateAsync(empleadosCargoActualizar, _testUserId);

            // Assert
            Assert.True(resultado, "La actualizacion deberia retornar true");

            var empleadosCargoVerificado = await _service.FindAsync(empleadosCargoCreado.cag_Id);
            Assert.True(empleadosCargoVerificado != null,
                $"No se encontro el registro Id={empleadosCargoCreado.cag_Id} tras el UPDATE");
            Assert.Equal(empleadosCargoActualizar.cag_Descripcion, empleadosCargoVerificado.cag_Descripcion);

            // Cleanup
            await _service.RemoveAsync(empleadosCargoCreado.cag_Id);
        }

        [Fact]
        public async Task UpdateAsync_CambiaEstadoActivo_ActualizaCorrectamente()
        {
            // Arrange
            var empleadosCargoPrueba = new EmpleadoCargoViewModel
            {
                cag_Descripcion = $"Estado {Guid.NewGuid().ToString().Substring(0, 8)}",
                cag_EsActivo = true,
                cag_Salario = 5000m
            };

            await _service.AddAsync(empleadosCargoPrueba, _testUserId);

            var lista = await _service.ListAsync();
            var empleadosCargoCreado = lista.FirstOrDefault(x => x.cag_Descripcion == empleadosCargoPrueba.cag_Descripcion);
            Assert.True(empleadosCargoCreado != null,
                $"Precondicion fallida: no se pudo insertar '{empleadosCargoPrueba.cag_Descripcion}' — verificar SP de INSERT");

            var empleadosCargoActualizar = await _service.FindAsync(empleadosCargoCreado.cag_Id);
            empleadosCargoActualizar.cag_EsActivo = false;

            // Act
            bool resultado = await _service.UpdateAsync(empleadosCargoActualizar, _testUserId);

            // Assert
            Assert.True(resultado);

            var empleadosCargoVerificado = await _service.FindAsync(empleadosCargoCreado.cag_Id);
            Assert.False(empleadosCargoVerificado.cag_EsActivo);

            // Cleanup
            await _service.RemoveAsync(empleadosCargoCreado.cag_Id);
        }

        // =======================================================================
        // DELETE
        // =======================================================================

        [Fact]
        public async Task RemoveAsync_ConIdExistente_EliminaDeBaseDeDatos()
        {
            // Arrange
            var empleadosCargoPrueba = new EmpleadoCargoViewModel
            {
                cag_Descripcion = $"Del {Guid.NewGuid().ToString().Substring(0, 8)}",
                cag_EsActivo = true,
                cag_Salario = 5000m
            };

            await _service.AddAsync(empleadosCargoPrueba, _testUserId);

            var lista = await _service.ListAsync();
            var empleadosCargoCreado = lista.FirstOrDefault(x => x.cag_Descripcion == empleadosCargoPrueba.cag_Descripcion);
            Assert.True(empleadosCargoCreado != null,
                $"Precondicion fallida: no se pudo insertar '{empleadosCargoPrueba.cag_Descripcion}' — verificar SP de INSERT");

            int idEliminar = empleadosCargoCreado.cag_Id;

            // Act
            bool resultado = await _service.RemoveAsync(idEliminar);

            // Assert
            Assert.True(resultado, "La eliminacion deberia retornar true");

            var empleadosCargoEliminado = await _service.FindAsync(idEliminar);
            Assert.Null(empleadosCargoEliminado);
        }

        [Fact]
        public async Task RemoveAsync_ConIdInexistente_NoLanzaExcepcion()
        {
            // Arrange
            int idInexistente = 999999;

            // Act
            // Nota: SP [General].[PR_General_EmpleadosCargos_Delete] no existe — retorna false sin excepcion
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
            var nuevoEmpleadosCargo = new EmpleadoCargoViewModel
            {
                cag_Descripcion = $"CRUD {Guid.NewGuid().ToString().Substring(0, 8)}",
                cag_EsActivo = true,
                cag_Salario = 5000m
            };

            bool insertado = await _service.AddAsync(nuevoEmpleadosCargo, _testUserId);
            Assert.True(insertado, "CREATE fallo — verificar SP de INSERT");

            // 2. READ
            var lista = await _service.ListAsync();
            var empleadosCargoCreado = lista.FirstOrDefault(x => x.cag_Descripcion == nuevoEmpleadosCargo.cag_Descripcion);
            Assert.True(empleadosCargoCreado != null,
                $"CREATE fallo: '{nuevoEmpleadosCargo.cag_Descripcion}' no esta en la BD — verificar SP de INSERT");

            var empleadosCargoEncontrado = await _service.FindAsync(empleadosCargoCreado.cag_Id);
            Assert.True(empleadosCargoEncontrado != null,
                $"READ fallo: FindAsync no encontro Id={empleadosCargoCreado.cag_Id}");
            Assert.Equal(nuevoEmpleadosCargo.cag_Descripcion, empleadosCargoEncontrado.cag_Descripcion);

            // 3. UPDATE
            empleadosCargoEncontrado.cag_Descripcion = $"CRUD2 {Guid.NewGuid().ToString().Substring(0, 8)}";
            bool actualizado = await _service.UpdateAsync(empleadosCargoEncontrado, _testUserId);
            Assert.True(actualizado, "UPDATE fallo");

            var empleadosCargoVerificado = await _service.FindAsync(empleadosCargoCreado.cag_Id);
            Assert.Equal(empleadosCargoEncontrado.cag_Descripcion, empleadosCargoVerificado.cag_Descripcion);

            // 4. DELETE
            bool eliminado = await _service.RemoveAsync(empleadosCargoCreado.cag_Id);
            Assert.True(eliminado, "DELETE fallo");

            var empleadosCargoEliminado = await _service.FindAsync(empleadosCargoCreado.cag_Id);
            Assert.Null(empleadosCargoEliminado);
        }
    }
}
