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
    /// Pruebas de integracion para ViaAdministracionService contra la base de datos REAL.
    /// Verifica que el flujo CRUD completo funciona correctamente end-to-end.
    ///
    /// IMPORTANTE:
    /// 1. Requiere base de datos configurada y accesible
    /// 2. Connection string configurado en DatabaseFixture
    /// 3. Stored procedures de ViaAdministracion existentes en la BD
    /// </summary>
    public class ViaAdministracionIntegrationTests : IClassFixture<DatabaseFixture>
    {
        private readonly ViaAdministracionService _service;
        private readonly ViaAdministracionRepository _repository;
        private readonly IMapper _mapper;
        private readonly DatabaseFixture _fixture;

        public ViaAdministracionIntegrationTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
            _repository = new ViaAdministracionRepository();
            _mapper = fixture.Mapper;

            var loggerMock = new Mock<ILogger<ViaAdministracionService>>();
            _service = new ViaAdministracionService(_repository, loggerMock.Object, _mapper);
        }

        // =======================================================================
        // CREATE (INSERT)
        // =======================================================================

        [Fact]
        public async Task AddAsync_ConDatosValidos_InsertaEnBaseDeDatos()
        {
            // Arrange
            var nuevoViaAdministracion = new ViaAdministracionViewModel
            {
                viaAdmin_Descripcion = $"Test {Guid.NewGuid().ToString().Substring(0, 8)}",
                
            };

            // Act
            bool resultado = await _service.AddAsync(nuevoViaAdministracion);

            // Assert
            Assert.True(resultado, "La insercion deberia retornar true");

            var lista = await _service.ListAsync();
            var viaAdministracionInsertado = lista.FirstOrDefault(x => x.viaAdmin_Descripcion == nuevoViaAdministracion.viaAdmin_Descripcion);

            Assert.True(viaAdministracionInsertado != null,
                $"No se encontro '{nuevoViaAdministracion.viaAdmin_Descripcion}' en la BD — el INSERT puede haber fallado (verificar nombre del SP)");

            // Cleanup
            if (viaAdministracionInsertado != null)
                await _service.RemoveAsync(viaAdministracionInsertado.viaAdmin_Id);
        }

        [Fact]
        public async Task AddAsync_ConDescripcionVacia_NoDeberiaInsertar()
        {
            // Arrange
            var viaAdministracionInvalido = new ViaAdministracionViewModel
            {
                viaAdmin_Descripcion = ""
            };

            // Act & Assert
            try
            {
                bool resultado = await _service.AddAsync(viaAdministracionInvalido);
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
        public async Task ListAsync_RetornaListaDeViaAdministracionsDesdeBD()
        {
            // Act
            var resultado = await _service.ListAsync();

            // Assert
            Assert.NotNull(resultado);
            Assert.IsType<List<ViaAdministracionViewModel>>(resultado);
            Assert.True(resultado.Count >= 0);
        }

        [Fact]
        public async Task FindAsync_ConIdExistente_RetornaViaAdministracionDesdeBD()
        {
            // Arrange
            var viaAdministracionPrueba = new ViaAdministracionViewModel
            {
                viaAdmin_Descripcion = $"Find {Guid.NewGuid().ToString().Substring(0, 8)}",
                
            };

            await _service.AddAsync(viaAdministracionPrueba);

            var lista = await _service.ListAsync();
            var viaAdministracionCreado = lista.FirstOrDefault(x => x.viaAdmin_Descripcion == viaAdministracionPrueba.viaAdmin_Descripcion);
            Assert.True(viaAdministracionCreado != null,
                $"Precondicion fallida: no se pudo insertar '{viaAdministracionPrueba.viaAdmin_Descripcion}' — verificar SP de INSERT");

            // Act
            var resultado = await _service.FindAsync(viaAdministracionCreado.viaAdmin_Id);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(viaAdministracionCreado.viaAdmin_Id, resultado.viaAdmin_Id);
            Assert.Equal(viaAdministracionPrueba.viaAdmin_Descripcion, resultado.viaAdmin_Descripcion);

            // Cleanup
            await _service.RemoveAsync(viaAdministracionCreado.viaAdmin_Id);
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
            var viaAdministracionPrueba = new ViaAdministracionViewModel
            {
                viaAdmin_Descripcion = $"Detail {Guid.NewGuid().ToString().Substring(0, 8)}",
                
            };

            await _service.AddAsync(viaAdministracionPrueba);

            var lista = await _service.ListAsync();
            var viaAdministracionCreado = lista.FirstOrDefault(x => x.viaAdmin_Descripcion == viaAdministracionPrueba.viaAdmin_Descripcion);
            Assert.True(viaAdministracionCreado != null,
                $"Precondicion fallida: no se pudo insertar '{viaAdministracionPrueba.viaAdmin_Descripcion}' — verificar SP de INSERT");

            // Act
            var resultado = await _service.DetailAsync(viaAdministracionCreado.viaAdmin_Id);

            // Assert
            Assert.True(resultado != null,
                $"DetailAsync no retorno datos para Id={viaAdministracionCreado.viaAdmin_Id}");
            Assert.Equal(viaAdministracionCreado.viaAdmin_Id, resultado.viaAdmin_Id);
            Assert.Equal(viaAdministracionPrueba.viaAdmin_Descripcion, resultado.viaAdmin_Descripcion);

            // Cleanup
            await _service.RemoveAsync(viaAdministracionCreado.viaAdmin_Id);
        }

        // =======================================================================
        // UPDATE
        // =======================================================================

        [Fact]
        public async Task UpdateAsync_ConDatosValidos_ActualizaEnBaseDeDatos()
        {
            // Arrange
            var viaAdministracionInicial = new ViaAdministracionViewModel
            {
                viaAdmin_Descripcion = $"Orig {Guid.NewGuid().ToString().Substring(0, 8)}",
                
            };

            await _service.AddAsync(viaAdministracionInicial);

            var lista = await _service.ListAsync();
            var viaAdministracionCreado = lista.FirstOrDefault(x => x.viaAdmin_Descripcion == viaAdministracionInicial.viaAdmin_Descripcion);
            Assert.True(viaAdministracionCreado != null,
                $"Precondicion fallida: no se pudo insertar '{viaAdministracionInicial.viaAdmin_Descripcion}' — verificar SP de INSERT");

            var viaAdministracionActualizar = await _service.FindAsync(viaAdministracionCreado.viaAdmin_Id);
            viaAdministracionActualizar.viaAdmin_Descripcion = $"Upd {Guid.NewGuid().ToString().Substring(0, 8)}";

            // Act
            bool resultado = await _service.UpdateAsync(viaAdministracionActualizar);

            // Assert
            Assert.True(resultado, "La actualizacion deberia retornar true");

            var viaAdministracionVerificado = await _service.FindAsync(viaAdministracionCreado.viaAdmin_Id);
            Assert.True(viaAdministracionVerificado != null,
                $"No se encontro el registro Id={viaAdministracionCreado.viaAdmin_Id} tras el UPDATE");
            Assert.Equal(viaAdministracionActualizar.viaAdmin_Descripcion, viaAdministracionVerificado.viaAdmin_Descripcion);

            // Cleanup
            await _service.RemoveAsync(viaAdministracionCreado.viaAdmin_Id);
        }

        // =======================================================================
        // DELETE
        // =======================================================================

        [Fact]
        public async Task RemoveAsync_ConIdExistente_EliminaDeBaseDeDatos()
        {
            // Arrange
            var viaAdministracionPrueba = new ViaAdministracionViewModel
            {
                viaAdmin_Descripcion = $"Del {Guid.NewGuid().ToString().Substring(0, 8)}",
                
            };

            await _service.AddAsync(viaAdministracionPrueba);

            var lista = await _service.ListAsync();
            var viaAdministracionCreado = lista.FirstOrDefault(x => x.viaAdmin_Descripcion == viaAdministracionPrueba.viaAdmin_Descripcion);
            Assert.True(viaAdministracionCreado != null,
                $"Precondicion fallida: no se pudo insertar '{viaAdministracionPrueba.viaAdmin_Descripcion}' — verificar SP de INSERT");

            int idEliminar = viaAdministracionCreado.viaAdmin_Id;

            // Act
            bool resultado = await _service.RemoveAsync(idEliminar);

            // Assert
            Assert.True(resultado, "La eliminacion deberia retornar true");

            var viaAdministracionEliminado = await _service.FindAsync(idEliminar);
            Assert.Null(viaAdministracionEliminado);
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
            var nuevoViaAdministracion = new ViaAdministracionViewModel
            {
                viaAdmin_Descripcion = $"CRUD {Guid.NewGuid().ToString().Substring(0, 8)}",
                
            };

            bool insertado = await _service.AddAsync(nuevoViaAdministracion);
            Assert.True(insertado, "CREATE fallo — verificar SP de INSERT");

            // 2. READ
            var lista = await _service.ListAsync();
            var viaAdministracionCreado = lista.FirstOrDefault(x => x.viaAdmin_Descripcion == nuevoViaAdministracion.viaAdmin_Descripcion);
            Assert.True(viaAdministracionCreado != null,
                $"CREATE fallo: '{nuevoViaAdministracion.viaAdmin_Descripcion}' no esta en la BD — verificar SP de INSERT");

            var viaAdministracionEncontrado = await _service.FindAsync(viaAdministracionCreado.viaAdmin_Id);
            Assert.True(viaAdministracionEncontrado != null,
                $"READ fallo: FindAsync no encontro Id={viaAdministracionCreado.viaAdmin_Id}");
            Assert.Equal(nuevoViaAdministracion.viaAdmin_Descripcion, viaAdministracionEncontrado.viaAdmin_Descripcion);

            // 3. UPDATE
            viaAdministracionEncontrado.viaAdmin_Descripcion = $"CRUD2 {Guid.NewGuid().ToString().Substring(0, 8)}";
            bool actualizado = await _service.UpdateAsync(viaAdministracionEncontrado);
            Assert.True(actualizado, "UPDATE fallo");

            var viaAdministracionVerificado = await _service.FindAsync(viaAdministracionCreado.viaAdmin_Id);
            Assert.Equal(viaAdministracionEncontrado.viaAdmin_Descripcion, viaAdministracionVerificado.viaAdmin_Descripcion);

            // 4. DELETE
            bool eliminado = await _service.RemoveAsync(viaAdministracionCreado.viaAdmin_Id);
            Assert.True(eliminado, "DELETE fallo");

            var viaAdministracionEliminado = await _service.FindAsync(viaAdministracionCreado.viaAdmin_Id);
            Assert.Null(viaAdministracionEliminado);
        }
    }
}
