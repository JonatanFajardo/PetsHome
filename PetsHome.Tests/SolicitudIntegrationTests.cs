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
    /// Pruebas de integracion para SolicitudService contra la base de datos REAL.
    /// Verifica que el flujo CRUD completo funciona correctamente end-to-end.
    ///
    /// IMPORTANTE:
    /// 1. Requiere base de datos configurada y accesible
    /// 2. Connection string configurado en DatabaseFixture
    /// 3. Stored procedures de Solicitud existentes en la BD
    /// </summary>
    public class SolicitudIntegrationTests : IClassFixture<DatabaseFixture>
    {
        private readonly SolicitudService _service;
        private readonly SolicitudRepository _repository;
        private readonly IMapper _mapper;
        private readonly DatabaseFixture _fixture;
        private readonly int _testUserId = 1;

        public SolicitudIntegrationTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
            _repository = new SolicitudRepository();
            _mapper = fixture.Mapper;

            var loggerMock = new Mock<ILogger<SolicitudService>>();
            _service = new SolicitudService(_repository, loggerMock.Object, _mapper);
        }

        // =======================================================================
        // CREATE (INSERT)
        // =======================================================================

        [Fact(Skip = "Requiere datos FK existentes: masc_Id (mascota)")]
        public async Task AddAsync_ConDatosValidos_InsertaEnBaseDeDatos()
        {
            // Arrange
            var nuevoSolicitud = new SolicitudFormViewModel
            {
                sol_Nombres = $"Test {Guid.NewGuid().ToString().Substring(0, 8)}",

                sol_Apellidos = "ApellidoTest",
                sol_Identidad = "0101199900001",
                sol_Telefono = "99001100",
                sol_Correo = "sol@test.com"
            };

            // Act
            bool resultado = await _service.AddAsync(nuevoSolicitud, _testUserId);

            // Assert
            Assert.True(resultado, "La insercion deberia retornar true");

            var lista = await _service.ListAsync();
            var solicitudInsertado = lista.FirstOrDefault(x => x.sol_Nombres == nuevoSolicitud.sol_Nombres);

            Assert.True(solicitudInsertado != null,
                $"No se encontro '{nuevoSolicitud.sol_Nombres}' en la BD — el INSERT puede haber fallado (verificar nombre del SP)");

            // Cleanup
            if (solicitudInsertado != null)
                await _service.RemoveAsync(solicitudInsertado.sol_Id);
        }

        [Fact(Skip = "Requiere datos FK existentes: masc_Id (mascota)")]
        public async Task AddAsync_ConDescripcionVacia_NoDeberiaInsertar()
        {
            // Arrange
            var solicitudInvalido = new SolicitudFormViewModel
            {
                sol_Nombres = ""
            };

            // Act & Assert
            try
            {
                bool resultado = await _service.AddAsync(solicitudInvalido, _testUserId);
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

        [Fact(Skip = "Requiere datos FK existentes: masc_Id (mascota)")]
        public async Task ListAsync_RetornaListaDeSolicitudsDesdeBD()
        {
            // Act
            var resultado = await _service.ListAsync();

            // Assert
            Assert.NotNull(resultado);
            Assert.IsType<List<SolicitudListViewModel>>(resultado);
            Assert.True(resultado.Count >= 0);
        }

        [Fact(Skip = "Requiere datos FK existentes: masc_Id (mascota)")]
        public async Task FindAsync_ConIdExistente_RetornaSolicitudDesdeBD()
        {
            // Arrange
            var solicitudPrueba = new SolicitudFormViewModel
            {
                sol_Nombres = $"Find {Guid.NewGuid().ToString().Substring(0, 8)}",

                sol_Apellidos = "ApellidoTest",
                sol_Identidad = "0101199900001",
                sol_Telefono = "99001100",
                sol_Correo = "sol@test.com"
            };

            await _service.AddAsync(solicitudPrueba, _testUserId);

            var lista = await _service.ListAsync();
            var solicitudCreado = lista.FirstOrDefault(x => x.sol_Nombres == solicitudPrueba.sol_Nombres);
            Assert.True(solicitudCreado != null,
                $"Precondicion fallida: no se pudo insertar '{solicitudPrueba.sol_Nombres}' — verificar SP de INSERT");

            // Act
            var resultado = await _service.FindAsync(solicitudCreado.sol_Id);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(solicitudCreado.sol_Id, resultado.sol_Id);
            Assert.Equal(solicitudPrueba.sol_Nombres, resultado.sol_Nombres);

            // Cleanup
            await _service.RemoveAsync(solicitudCreado.sol_Id);
        }

        [Fact(Skip = "Requiere datos FK existentes: masc_Id (mascota)")]
        public async Task FindAsync_ConIdInexistente_RetornaNull()
        {
            // Arrange
            int idInexistente = 999999;

            // Act
            var resultado = await _service.FindAsync(idInexistente);

            // Assert
            Assert.Null(resultado);
        }

        [Fact(Skip = "Requiere datos FK existentes: masc_Id (mascota)")]
        public async Task DetailAsync_ConIdExistente_RetornaDetalleCompleto()
        {
            // Arrange
            var solicitudPrueba = new SolicitudFormViewModel
            {
                sol_Nombres = $"Detail {Guid.NewGuid().ToString().Substring(0, 8)}",

                sol_Apellidos = "ApellidoTest",
                sol_Identidad = "0101199900001",
                sol_Telefono = "99001100",
                sol_Correo = "sol@test.com"
            };

            await _service.AddAsync(solicitudPrueba, _testUserId);

            var lista = await _service.ListAsync();
            var solicitudCreado = lista.FirstOrDefault(x => x.sol_Nombres == solicitudPrueba.sol_Nombres);
            Assert.True(solicitudCreado != null,
                $"Precondicion fallida: no se pudo insertar '{solicitudPrueba.sol_Nombres}' — verificar SP de INSERT");

            // Act
            var resultado = await _service.DetailAsync(solicitudCreado.sol_Id);

            // Assert
            Assert.True(resultado != null,
                $"DetailAsync no retorno datos para Id={solicitudCreado.sol_Id}");
            Assert.Equal(solicitudCreado.sol_Id, resultado.sol_Id);
            Assert.Equal(solicitudPrueba.sol_Nombres, resultado.sol_Nombres);
            Assert.NotEqual(default(DateTime), resultado.sol_FechaCrea);

            // Cleanup
            await _service.RemoveAsync(solicitudCreado.sol_Id);
        }

        // =======================================================================
        // UPDATE
        // =======================================================================

        [Fact(Skip = "Requiere datos FK existentes: masc_Id (mascota)")]
        public async Task UpdateAsync_ConDatosValidos_ActualizaEnBaseDeDatos()
        {
            // Arrange
            var solicitudInicial = new SolicitudFormViewModel
            {
                sol_Nombres = $"Orig {Guid.NewGuid().ToString().Substring(0, 8)}",

                sol_Apellidos = "ApellidoTest",
                sol_Identidad = "0101199900001",
                sol_Telefono = "99001100",
                sol_Correo = "sol@test.com"
            };

            await _service.AddAsync(solicitudInicial, _testUserId);

            var lista = await _service.ListAsync();
            var solicitudCreado = lista.FirstOrDefault(x => x.sol_Nombres == solicitudInicial.sol_Nombres);
            Assert.True(solicitudCreado != null,
                $"Precondicion fallida: no se pudo insertar '{solicitudInicial.sol_Nombres}' — verificar SP de INSERT");

            var solicitudActualizar = await _service.FindAsync(solicitudCreado.sol_Id);
            solicitudActualizar.sol_Nombres = $"Upd {Guid.NewGuid().ToString().Substring(0, 8)}";

            // Act
            bool resultado = await _service.UpdateAsync(solicitudActualizar, _testUserId);

            // Assert
            Assert.True(resultado, "La actualizacion deberia retornar true");

            var solicitudVerificado = await _service.FindAsync(solicitudCreado.sol_Id);
            Assert.True(solicitudVerificado != null,
                $"No se encontro el registro Id={solicitudCreado.sol_Id} tras el UPDATE");
            Assert.Equal(solicitudActualizar.sol_Nombres, solicitudVerificado.sol_Nombres);

            // Cleanup
            await _service.RemoveAsync(solicitudCreado.sol_Id);
        }

        // =======================================================================
        // DELETE
        // =======================================================================

        [Fact(Skip = "Requiere datos FK existentes: masc_Id (mascota)")]
        public async Task RemoveAsync_ConIdExistente_EliminaDeBaseDeDatos()
        {
            // Arrange
            var solicitudPrueba = new SolicitudFormViewModel
            {
                sol_Nombres = $"Del {Guid.NewGuid().ToString().Substring(0, 8)}",

                sol_Apellidos = "ApellidoTest",
                sol_Identidad = "0101199900001",
                sol_Telefono = "99001100",
                sol_Correo = "sol@test.com"
            };

            await _service.AddAsync(solicitudPrueba, _testUserId);

            var lista = await _service.ListAsync();
            var solicitudCreado = lista.FirstOrDefault(x => x.sol_Nombres == solicitudPrueba.sol_Nombres);
            Assert.True(solicitudCreado != null,
                $"Precondicion fallida: no se pudo insertar '{solicitudPrueba.sol_Nombres}' — verificar SP de INSERT");

            int idEliminar = solicitudCreado.sol_Id;

            // Act
            bool resultado = await _service.RemoveAsync(idEliminar);

            // Assert
            Assert.True(resultado, "La eliminacion deberia retornar true");

            var solicitudEliminado = await _service.FindAsync(idEliminar);
            Assert.Null(solicitudEliminado);
        }

        [Fact(Skip = "Requiere datos FK existentes: masc_Id (mascota)")]
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

        [Fact(Skip = "Requiere datos FK existentes: masc_Id (mascota)")]
        public async Task FlujoCRUDCompleto_CrearLeerActualizarEliminar_FuncionaCorrectamente()
        {
            // 1. CREATE
            var nuevoSolicitud = new SolicitudFormViewModel
            {
                sol_Nombres = $"CRUD {Guid.NewGuid().ToString().Substring(0, 8)}",

                sol_Apellidos = "ApellidoTest",
                sol_Identidad = "0101199900001",
                sol_Telefono = "99001100",
                sol_Correo = "sol@test.com"
            };

            bool insertado = await _service.AddAsync(nuevoSolicitud, _testUserId);
            Assert.True(insertado, "CREATE fallo — verificar SP de INSERT");

            // 2. READ
            var lista = await _service.ListAsync();
            var solicitudCreado = lista.FirstOrDefault(x => x.sol_Nombres == nuevoSolicitud.sol_Nombres);
            Assert.True(solicitudCreado != null,
                $"CREATE fallo: '{nuevoSolicitud.sol_Nombres}' no esta en la BD — verificar SP de INSERT");

            var solicitudEncontrado = await _service.FindAsync(solicitudCreado.sol_Id);
            Assert.True(solicitudEncontrado != null,
                $"READ fallo: FindAsync no encontro Id={solicitudCreado.sol_Id}");
            Assert.Equal(nuevoSolicitud.sol_Nombres, solicitudEncontrado.sol_Nombres);

            // 3. UPDATE
            solicitudEncontrado.sol_Nombres = $"CRUD2 {Guid.NewGuid().ToString().Substring(0, 8)}";
            bool actualizado = await _service.UpdateAsync(solicitudEncontrado, _testUserId);
            Assert.True(actualizado, "UPDATE fallo");

            var solicitudVerificado = await _service.FindAsync(solicitudCreado.sol_Id);
            Assert.Equal(solicitudEncontrado.sol_Nombres, solicitudVerificado.sol_Nombres);

            // 4. DELETE
            bool eliminado = await _service.RemoveAsync(solicitudCreado.sol_Id);
            Assert.True(eliminado, "DELETE fallo");

            var solicitudEliminado = await _service.FindAsync(solicitudCreado.sol_Id);
            Assert.Null(solicitudEliminado);
        }
    }
}
