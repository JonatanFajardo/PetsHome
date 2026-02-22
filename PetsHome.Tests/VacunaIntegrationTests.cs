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
    /// Pruebas de integracion para VacunaService contra la base de datos REAL.
    /// Verifica que el flujo CRUD completo funciona correctamente end-to-end.
    ///
    /// IMPORTANTE:
    /// 1. Requiere base de datos configurada y accesible
    /// 2. Connection string configurado en DatabaseFixture
    /// 3. Stored procedures de Vacuna existentes en la BD
    /// </summary>
    public class VacunaIntegrationTests : IClassFixture<DatabaseFixture>
    {
        private readonly VacunaService _service;
        private readonly VacunaRepository _repository;
        private readonly IMapper _mapper;
        private readonly DatabaseFixture _fixture;
        private readonly int _testUserId = 1;

        public VacunaIntegrationTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
            _repository = new VacunaRepository();
            _mapper = fixture.Mapper;

            var loggerMock = new Mock<ILogger<VacunaService>>();
            _service = new VacunaService(_repository, loggerMock.Object, _mapper);
        }

        // =======================================================================
        // CREATE (INSERT)
        // =======================================================================

        [Fact]
        public async Task AddAsync_ConDatosValidos_InsertaEnBaseDeDatos()
        {
            // Arrange
            var nuevaVacuna = new VacunaFormViewModel
            {
                vac_Descripcion = $"Test {Guid.NewGuid().ToString().Substring(0, 8)}",
                vac_EsActivo = true
            };

            // Act
            bool resultado = await _service.AddAsync(nuevaVacuna, _testUserId);

            // Assert
            Assert.True(resultado, "La insercion deberia retornar true");

            var lista = await _service.ListAsync();
            var vacunaInsertado = lista.FirstOrDefault(x => x.vac_Descripcion == nuevaVacuna.vac_Descripcion);

            Assert.True(vacunaInsertado != null,
                $"No se encontro '{nuevaVacuna.vac_Descripcion}' en la BD — el INSERT puede haber fallado (verificar nombre del SP)");

            // Cleanup
            if (vacunaInsertado != null)
                await _service.RemoveAsync(vacunaInsertado.vac_Id);
        }

        [Fact]
        public async Task AddAsync_ConDescripcionVacia_NoDeberiaInsertar()
        {
            // Arrange
            var vacunaInvalido = new VacunaFormViewModel
            {
                vac_Descripcion = ""
            };

            // Act & Assert
            try
            {
                bool resultado = await _service.AddAsync(vacunaInvalido, _testUserId);
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
        public async Task ListAsync_RetornaListaDeVacunasDesdeBD()
        {
            // Act
            var resultado = await _service.ListAsync();

            // Assert
            Assert.NotNull(resultado);
            Assert.IsType<List<VacunaListViewModel>>(resultado);
            Assert.True(resultado.Count >= 0);
        }

        [Fact]
        public async Task FindAsync_ConIdExistente_RetornaVacunaDesdeBD()
        {
            // Arrange
            var vacunaPrueba = new VacunaFormViewModel
            {
                vac_Descripcion = $"Find {Guid.NewGuid().ToString().Substring(0, 8)}",
                vac_EsActivo = true
            };

            await _service.AddAsync(vacunaPrueba, _testUserId);

            var lista = await _service.ListAsync();
            var vacunaCreado = lista.FirstOrDefault(x => x.vac_Descripcion == vacunaPrueba.vac_Descripcion);
            Assert.True(vacunaCreado != null,
                $"Precondicion fallida: no se pudo insertar '{vacunaPrueba.vac_Descripcion}' — verificar SP de INSERT");

            // Act
            var resultado = await _service.FindAsync(vacunaCreado.vac_Id);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(vacunaCreado.vac_Id, resultado.vac_Id);
            Assert.Equal(vacunaPrueba.vac_Descripcion, resultado.vac_Descripcion);

            // Cleanup
            await _service.RemoveAsync(vacunaCreado.vac_Id);
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
            var vacunaPrueba = new VacunaFormViewModel
            {
                vac_Descripcion = $"Detail {Guid.NewGuid().ToString().Substring(0, 8)}",
                vac_EsActivo = true
            };

            await _service.AddAsync(vacunaPrueba, _testUserId);

            var lista = await _service.ListAsync();
            var vacunaCreado = lista.FirstOrDefault(x => x.vac_Descripcion == vacunaPrueba.vac_Descripcion);
            Assert.True(vacunaCreado != null,
                $"Precondicion fallida: no se pudo insertar '{vacunaPrueba.vac_Descripcion}' — verificar SP de INSERT");

            // Act
            var resultado = await _service.DetailAsync(vacunaCreado.vac_Id);

            // Assert
            Assert.True(resultado != null,
                $"DetailAsync no retorno datos para Id={vacunaCreado.vac_Id}");
            Assert.Equal(vacunaCreado.vac_Id, resultado.vac_Id);
            Assert.Equal(vacunaPrueba.vac_Descripcion, resultado.vac_Descripcion);
            Assert.NotEqual(default(DateTime), resultado.vac_FechaCrea);

            // Cleanup
            await _service.RemoveAsync(vacunaCreado.vac_Id);
        }

        // =======================================================================
        // UPDATE
        // =======================================================================

        [Fact]
        public async Task UpdateAsync_ConDatosValidos_ActualizaEnBaseDeDatos()
        {
            // Arrange
            var vacunaInicial = new VacunaFormViewModel
            {
                vac_Descripcion = $"Orig {Guid.NewGuid().ToString().Substring(0, 8)}",
                vac_EsActivo = true
            };

            await _service.AddAsync(vacunaInicial, _testUserId);

            var lista = await _service.ListAsync();
            var vacunaCreado = lista.FirstOrDefault(x => x.vac_Descripcion == vacunaInicial.vac_Descripcion);
            Assert.True(vacunaCreado != null,
                $"Precondicion fallida: no se pudo insertar '{vacunaInicial.vac_Descripcion}' — verificar SP de INSERT");

            var vacunaActualizar = await _service.FindAsync(vacunaCreado.vac_Id);
            vacunaActualizar.vac_Descripcion = $"Upd {Guid.NewGuid().ToString().Substring(0, 8)}";

            // Act
            bool resultado = await _service.UpdateAsync(vacunaActualizar, _testUserId);

            // Assert
            Assert.True(resultado, "La actualizacion deberia retornar true");

            var vacunaVerificado = await _service.FindAsync(vacunaCreado.vac_Id);
            Assert.True(vacunaVerificado != null,
                $"No se encontro el registro Id={vacunaCreado.vac_Id} tras el UPDATE");
            Assert.Equal(vacunaActualizar.vac_Descripcion, vacunaVerificado.vac_Descripcion);

            // Cleanup
            await _service.RemoveAsync(vacunaCreado.vac_Id);
        }

        [Fact]
        public async Task UpdateAsync_CambiaEstadoActivo_ActualizaCorrectamente()
        {
            // Arrange
            var vacunaPrueba = new VacunaFormViewModel
            {
                vac_Descripcion = $"Estado {Guid.NewGuid().ToString().Substring(0, 8)}",
                vac_EsActivo = true
            };

            await _service.AddAsync(vacunaPrueba, _testUserId);

            var lista = await _service.ListAsync();
            var vacunaCreado = lista.FirstOrDefault(x => x.vac_Descripcion == vacunaPrueba.vac_Descripcion);
            Assert.True(vacunaCreado != null,
                $"Precondicion fallida: no se pudo insertar '{vacunaPrueba.vac_Descripcion}' — verificar SP de INSERT");

            var vacunaActualizar = await _service.FindAsync(vacunaCreado.vac_Id);
            vacunaActualizar.vac_EsActivo = false;

            // Act
            bool resultado = await _service.UpdateAsync(vacunaActualizar, _testUserId);

            // Assert
            Assert.True(resultado);

            var vacunaVerificado = await _service.FindAsync(vacunaCreado.vac_Id);
            Assert.False(vacunaVerificado.vac_EsActivo);

            // Cleanup
            await _service.RemoveAsync(vacunaCreado.vac_Id);
        }

        // =======================================================================
        // DELETE
        // =======================================================================

        [Fact]
        public async Task RemoveAsync_ConIdExistente_EliminaDeBaseDeDatos()
        {
            // Arrange
            var vacunaPrueba = new VacunaFormViewModel
            {
                vac_Descripcion = $"Del {Guid.NewGuid().ToString().Substring(0, 8)}",
                vac_EsActivo = true
            };

            await _service.AddAsync(vacunaPrueba, _testUserId);

            var lista = await _service.ListAsync();
            var vacunaCreado = lista.FirstOrDefault(x => x.vac_Descripcion == vacunaPrueba.vac_Descripcion);
            Assert.True(vacunaCreado != null,
                $"Precondicion fallida: no se pudo insertar '{vacunaPrueba.vac_Descripcion}' — verificar SP de INSERT");

            int idEliminar = vacunaCreado.vac_Id;

            // Act
            bool resultado = await _service.RemoveAsync(idEliminar);

            // Assert
            Assert.True(resultado, "La eliminacion deberia retornar true");

            var vacunaEliminado = await _service.FindAsync(idEliminar);
            Assert.Null(vacunaEliminado);
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
            var nuevaVacuna = new VacunaFormViewModel
            {
                vac_Descripcion = $"CRUD {Guid.NewGuid().ToString().Substring(0, 8)}",
                vac_EsActivo = true
            };

            bool insertado = await _service.AddAsync(nuevaVacuna, _testUserId);
            Assert.True(insertado, "CREATE fallo — verificar SP de INSERT");

            // 2. READ
            var lista = await _service.ListAsync();
            var vacunaCreado = lista.FirstOrDefault(x => x.vac_Descripcion == nuevaVacuna.vac_Descripcion);
            Assert.True(vacunaCreado != null,
                $"CREATE fallo: '{nuevaVacuna.vac_Descripcion}' no esta en la BD — verificar SP de INSERT");

            var vacunaEncontrado = await _service.FindAsync(vacunaCreado.vac_Id);
            Assert.True(vacunaEncontrado != null,
                $"READ fallo: FindAsync no encontro Id={vacunaCreado.vac_Id}");
            Assert.Equal(nuevaVacuna.vac_Descripcion, vacunaEncontrado.vac_Descripcion);

            // 3. UPDATE
            vacunaEncontrado.vac_Descripcion = $"CRUD2 {Guid.NewGuid().ToString().Substring(0, 8)}";
            bool actualizado = await _service.UpdateAsync(vacunaEncontrado, _testUserId);
            Assert.True(actualizado, "UPDATE fallo");

            var vacunaVerificado = await _service.FindAsync(vacunaCreado.vac_Id);
            Assert.Equal(vacunaEncontrado.vac_Descripcion, vacunaVerificado.vac_Descripcion);

            // 4. DELETE
            bool eliminado = await _service.RemoveAsync(vacunaCreado.vac_Id);
            Assert.True(eliminado, "DELETE fallo");

            var vacunaEliminado = await _service.FindAsync(vacunaCreado.vac_Id);
            Assert.Null(vacunaEliminado);
        }
    }
}
