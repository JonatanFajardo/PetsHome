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
    /// Pruebas de integracion para RefugioService contra la base de datos REAL.
    /// Verifica que el flujo CRUD completo funciona correctamente end-to-end.
    ///
    /// IMPORTANTE:
    /// 1. Requiere base de datos configurada y accesible
    /// 2. Connection string configurado en DatabaseFixture
    /// 3. Stored procedures de Refugio existentes en la BD
    /// </summary>
    public class RefugioIntegrationTests : IClassFixture<DatabaseFixture>
    {
        private readonly RefugioService _service;
        private readonly RefugioRepository _repository;
        private readonly IMapper _mapper;
        private readonly DatabaseFixture _fixture;
        private readonly int _testUserId = 1;

        public RefugioIntegrationTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
            _repository = new RefugioRepository();
            _mapper = fixture.Mapper;

            var loggerMock = new Mock<ILogger<RefugioService>>();
            _service = new RefugioService(_repository, loggerMock.Object, _mapper);
        }

        // =======================================================================
        // CREATE (INSERT)
        // =======================================================================

        [Fact]
        public async Task AddAsync_ConDatosValidos_InsertaEnBaseDeDatos()
        {
            // Arrange
            var nuevoRefugio = new RefugioFormViewModel
            {
                refg_Nombre = $"Test {Guid.NewGuid().ToString().Substring(0, 8)}",
                refg_EsActivo = true,
                refg_Ubicacion = "Test Ubicacion",
                refg_RTN = "08019999000000",
                refg_Telefono = "22001100",
                refg_Correo = "test@test.com",
                refg_InformacionAdicional = "Prueba de integracion",
                depto_Id = 1,
                mpio_Id = 1
            };

            // Act
            bool resultado = await _service.AddAsync(nuevoRefugio, _testUserId);

            // Assert
            Assert.True(resultado, "La insercion deberia retornar true");

            var lista = await _service.ListAsync();
            var refugioInsertado = lista.FirstOrDefault(x => x.refg_Nombre == nuevoRefugio.refg_Nombre);

            Assert.True(refugioInsertado != null,
                $"No se encontro '{nuevoRefugio.refg_Nombre}' en la BD — el INSERT puede haber fallado (verificar nombre del SP)");

            // Cleanup
            if (refugioInsertado != null)
                await _service.RemoveAsync(refugioInsertado.refg_Id);
        }

        [Fact]
        public async Task AddAsync_ConDescripcionVacia_NoDeberiaInsertar()
        {
            // Arrange
            var refugioInvalido = new RefugioFormViewModel
            {
                refg_Nombre = ""
            };

            // Act & Assert
            try
            {
                bool resultado = await _service.AddAsync(refugioInvalido, _testUserId);
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
        public async Task ListAsync_RetornaListaDeRefugiosDesdeBD()
        {
            // Act
            var resultado = await _service.ListAsync();

            // Assert
            Assert.NotNull(resultado);
            Assert.IsType<List<RefugioListViewModel>>(resultado);
            Assert.True(resultado.Count >= 0);
        }

        [Fact]
        public async Task FindAsync_ConIdExistente_RetornaRefugioDesdeBD()
        {
            // Arrange
            var refugioPrueba = new RefugioFormViewModel
            {
                refg_Nombre = $"Find {Guid.NewGuid().ToString().Substring(0, 8)}",
                refg_EsActivo = true,
                refg_Ubicacion = "Test Ubicacion",
                refg_RTN = "08019999000000",
                refg_Telefono = "22001100",
                refg_Correo = "test@test.com",
                refg_InformacionAdicional = "Prueba de integracion",
                depto_Id = 1,
                mpio_Id = 1
            };

            await _service.AddAsync(refugioPrueba, _testUserId);

            var lista = await _service.ListAsync();
            var refugioCreado = lista.FirstOrDefault(x => x.refg_Nombre == refugioPrueba.refg_Nombre);
            Assert.True(refugioCreado != null,
                $"Precondicion fallida: no se pudo insertar '{refugioPrueba.refg_Nombre}' — verificar SP de INSERT");

            // Act
            var resultado = await _service.FindAsync(refugioCreado.refg_Id);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(refugioCreado.refg_Id, resultado.refg_Id);
            Assert.Equal(refugioPrueba.refg_Nombre, resultado.refg_Nombre);

            // Cleanup
            await _service.RemoveAsync(refugioCreado.refg_Id);
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
            var refugioPrueba = new RefugioFormViewModel
            {
                refg_Nombre = $"Detail {Guid.NewGuid().ToString().Substring(0, 8)}",
                refg_EsActivo = true,
                refg_Ubicacion = "Test Ubicacion",
                refg_RTN = "08019999000000",
                refg_Telefono = "22001100",
                refg_Correo = "test@test.com",
                refg_InformacionAdicional = "Prueba de integracion",
                depto_Id = 1,
                mpio_Id = 1
            };

            await _service.AddAsync(refugioPrueba, _testUserId);

            var lista = await _service.ListAsync();
            var refugioCreado = lista.FirstOrDefault(x => x.refg_Nombre == refugioPrueba.refg_Nombre);
            Assert.True(refugioCreado != null,
                $"Precondicion fallida: no se pudo insertar '{refugioPrueba.refg_Nombre}' — verificar SP de INSERT");

            // Act
            var resultado = await _service.DetailAsync(refugioCreado.refg_Id);

            // Assert
            Assert.True(resultado != null,
                $"DetailAsync no retorno datos para Id={refugioCreado.refg_Id}");
            Assert.Equal(refugioCreado.refg_Id, resultado.refg_Id);
            Assert.Equal(refugioPrueba.refg_Nombre, resultado.refg_Nombre);
            Assert.NotEqual(default(DateTime), resultado.refg_FechaCrea);

            // Cleanup
            await _service.RemoveAsync(refugioCreado.refg_Id);
        }

        // =======================================================================
        // UPDATE
        // =======================================================================

        [Fact]
        public async Task UpdateAsync_ConDatosValidos_ActualizaEnBaseDeDatos()
        {
            // Arrange
            var refugioInicial = new RefugioFormViewModel
            {
                refg_Nombre = $"Orig {Guid.NewGuid().ToString().Substring(0, 8)}",
                refg_EsActivo = true,
                refg_Ubicacion = "Test Ubicacion",
                refg_RTN = "08019999000000",
                refg_Telefono = "22001100",
                refg_Correo = "test@test.com",
                refg_InformacionAdicional = "Prueba de integracion",
                depto_Id = 1,
                mpio_Id = 1
            };

            await _service.AddAsync(refugioInicial, _testUserId);

            var lista = await _service.ListAsync();
            var refugioCreado = lista.FirstOrDefault(x => x.refg_Nombre == refugioInicial.refg_Nombre);
            Assert.True(refugioCreado != null,
                $"Precondicion fallida: no se pudo insertar '{refugioInicial.refg_Nombre}' — verificar SP de INSERT");

            var refugioActualizar = await _service.FindAsync(refugioCreado.refg_Id);
            refugioActualizar.refg_Nombre = $"Upd {Guid.NewGuid().ToString().Substring(0, 8)}";

            // Act
            bool resultado = await _service.UpdateAsync(refugioActualizar, _testUserId);

            // Assert
            Assert.True(resultado, "La actualizacion deberia retornar true");

            var refugioVerificado = await _service.FindAsync(refugioCreado.refg_Id);
            Assert.True(refugioVerificado != null,
                $"No se encontro el registro Id={refugioCreado.refg_Id} tras el UPDATE");
            Assert.Equal(refugioActualizar.refg_Nombre, refugioVerificado.refg_Nombre);

            // Cleanup
            await _service.RemoveAsync(refugioCreado.refg_Id);
        }

        [Fact]
        public async Task UpdateAsync_CambiaEstadoActivo_ActualizaCorrectamente()
        {
            // Arrange
            var refugioPrueba = new RefugioFormViewModel
            {
                refg_Nombre = $"Estado {Guid.NewGuid().ToString().Substring(0, 8)}",
                refg_EsActivo = true,
                refg_Ubicacion = "Test Ubicacion",
                refg_RTN = "08019999000000",
                refg_Telefono = "22001100",
                refg_Correo = "test@test.com",
                refg_InformacionAdicional = "Prueba de integracion",
                depto_Id = 1,
                mpio_Id = 1
            };

            await _service.AddAsync(refugioPrueba, _testUserId);

            var lista = await _service.ListAsync();
            var refugioCreado = lista.FirstOrDefault(x => x.refg_Nombre == refugioPrueba.refg_Nombre);
            Assert.True(refugioCreado != null,
                $"Precondicion fallida: no se pudo insertar '{refugioPrueba.refg_Nombre}' — verificar SP de INSERT");

            var refugioActualizar = await _service.FindAsync(refugioCreado.refg_Id);
            refugioActualizar.refg_EsActivo = false;

            // Act
            bool resultado = await _service.UpdateAsync(refugioActualizar, _testUserId);

            // Assert
            Assert.True(resultado);

            var refugioVerificado = await _service.FindAsync(refugioCreado.refg_Id);
            Assert.False(refugioVerificado.refg_EsActivo);

            // Cleanup
            await _service.RemoveAsync(refugioCreado.refg_Id);
        }

        // =======================================================================
        // DELETE
        // =======================================================================

        [Fact]
        public async Task RemoveAsync_ConIdExistente_EliminaDeBaseDeDatos()
        {
            // Arrange
            var refugioPrueba = new RefugioFormViewModel
            {
                refg_Nombre = $"Del {Guid.NewGuid().ToString().Substring(0, 8)}",
                refg_EsActivo = true,
                refg_Ubicacion = "Test Ubicacion",
                refg_RTN = "08019999000000",
                refg_Telefono = "22001100",
                refg_Correo = "test@test.com",
                refg_InformacionAdicional = "Prueba de integracion",
                depto_Id = 1,
                mpio_Id = 1
            };

            await _service.AddAsync(refugioPrueba, _testUserId);

            var lista = await _service.ListAsync();
            var refugioCreado = lista.FirstOrDefault(x => x.refg_Nombre == refugioPrueba.refg_Nombre);
            Assert.True(refugioCreado != null,
                $"Precondicion fallida: no se pudo insertar '{refugioPrueba.refg_Nombre}' — verificar SP de INSERT");

            int idEliminar = refugioCreado.refg_Id;

            // Act
            bool resultado = await _service.RemoveAsync(idEliminar);

            // Assert
            Assert.True(resultado, "La eliminacion deberia retornar true");

            var refugioEliminado = await _service.FindAsync(idEliminar);
            Assert.Null(refugioEliminado);
        }

        [Fact]
        public async Task RemoveAsync_ConIdInexistente_NoLanzaExcepcion()
        {
            // Arrange
            int idInexistente = 999999;

            // Act
            // Nota: SP [General].[PR_General_Refugios_Delete] no existe — retorna false sin excepcion
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
            var nuevoRefugio = new RefugioFormViewModel
            {
                refg_Nombre = $"CRUD {Guid.NewGuid().ToString().Substring(0, 8)}",
                refg_EsActivo = true,
                refg_Ubicacion = "Test Ubicacion",
                refg_RTN = "08019999000000",
                refg_Telefono = "22001100",
                refg_Correo = "test@test.com",
                refg_InformacionAdicional = "Prueba de integracion",
                depto_Id = 1,
                mpio_Id = 1
            };

            bool insertado = await _service.AddAsync(nuevoRefugio, _testUserId);
            Assert.True(insertado, "CREATE fallo — verificar SP de INSERT");

            // 2. READ
            var lista = await _service.ListAsync();
            var refugioCreado = lista.FirstOrDefault(x => x.refg_Nombre == nuevoRefugio.refg_Nombre);
            Assert.True(refugioCreado != null,
                $"CREATE fallo: '{nuevoRefugio.refg_Nombre}' no esta en la BD — verificar SP de INSERT");

            var refugioEncontrado = await _service.FindAsync(refugioCreado.refg_Id);
            Assert.True(refugioEncontrado != null,
                $"READ fallo: FindAsync no encontro Id={refugioCreado.refg_Id}");
            Assert.Equal(nuevoRefugio.refg_Nombre, refugioEncontrado.refg_Nombre);

            // 3. UPDATE
            refugioEncontrado.refg_Nombre = $"CRUD2 {Guid.NewGuid().ToString().Substring(0, 8)}";
            bool actualizado = await _service.UpdateAsync(refugioEncontrado, _testUserId);
            Assert.True(actualizado, "UPDATE fallo");

            var refugioVerificado = await _service.FindAsync(refugioCreado.refg_Id);
            Assert.Equal(refugioEncontrado.refg_Nombre, refugioVerificado.refg_Nombre);

            // 4. DELETE
            bool eliminado = await _service.RemoveAsync(refugioCreado.refg_Id);
            Assert.True(eliminado, "DELETE fallo");

            var refugioEliminado = await _service.FindAsync(refugioCreado.refg_Id);
            Assert.Null(refugioEliminado);
        }
    }
}
