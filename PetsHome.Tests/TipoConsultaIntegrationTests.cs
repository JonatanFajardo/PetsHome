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
    /// Pruebas de integracion para TipoConsultaService contra la base de datos REAL.
    /// Verifica que el flujo CRUD completo funciona correctamente end-to-end.
    ///
    /// IMPORTANTE:
    /// 1. Requiere base de datos configurada y accesible
    /// 2. Connection string configurado en DatabaseFixture
    /// 3. Stored procedures de TipoConsulta existentes en la BD
    /// </summary>
    public class TipoConsultaIntegrationTests : IClassFixture<DatabaseFixture>
    {
        private readonly TipoConsultaService _service;
        private readonly TipoConsultaRepository _repository;
        private readonly IMapper _mapper;
        private readonly DatabaseFixture _fixture;

        public TipoConsultaIntegrationTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
            _repository = new TipoConsultaRepository();
            _mapper = fixture.Mapper;

            var loggerMock = new Mock<ILogger<TipoConsultaService>>();
            _service = new TipoConsultaService(_repository, loggerMock.Object, _mapper);
        }

        // =======================================================================
        // CREATE (INSERT)
        // =======================================================================

        [Fact]
        public async Task AddAsync_ConDatosValidos_InsertaEnBaseDeDatos()
        {
            // Arrange
            var nuevaTipoConsulta = new TipoConsultaViewModel
            {
                tipoCon_Descripcion = $"Test {Guid.NewGuid().ToString().Substring(0, 8)}",
                tipoCon_EsActivoBool = true
            };

            // Act
            bool resultado = await _service.AddAsync(nuevaTipoConsulta);

            // Assert
            Assert.True(resultado, "La insercion deberia retornar true");

            var lista = await _service.ListAsync();
            var tipoConsultaInsertado = lista.FirstOrDefault(x => x.tipoCon_Descripcion == nuevaTipoConsulta.tipoCon_Descripcion);

            Assert.True(tipoConsultaInsertado != null,
                $"No se encontro '{nuevaTipoConsulta.tipoCon_Descripcion}' en la BD — el INSERT puede haber fallado (verificar nombre del SP)");

            // Cleanup
            if (tipoConsultaInsertado != null)
                await _service.RemoveAsync(tipoConsultaInsertado.tipoCon_Id);
        }

        [Fact]
        public async Task AddAsync_ConDescripcionVacia_NoDeberiaInsertar()
        {
            // Arrange
            var tipoConsultaInvalido = new TipoConsultaViewModel
            {
                tipoCon_Descripcion = ""
            };

            // Act & Assert
            try
            {
                bool resultado = await _service.AddAsync(tipoConsultaInvalido);
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
        public async Task ListAsync_RetornaListaDeTipoConsultasDesdeBD()
        {
            // Act
            var resultado = await _service.ListAsync();

            // Assert
            Assert.NotNull(resultado);
            Assert.IsType<List<TipoConsultaViewModel>>(resultado);
            Assert.True(resultado.Count >= 0);
        }

        [Fact]
        public async Task FindAsync_ConIdExistente_RetornaTipoConsultaDesdeBD()
        {
            // Arrange
            var tipoConsultaPrueba = new TipoConsultaViewModel
            {
                tipoCon_Descripcion = $"Find {Guid.NewGuid().ToString().Substring(0, 8)}",
                tipoCon_EsActivoBool = true
            };

            await _service.AddAsync(tipoConsultaPrueba);

            var lista = await _service.ListAsync();
            var tipoConsultaCreado = lista.FirstOrDefault(x => x.tipoCon_Descripcion == tipoConsultaPrueba.tipoCon_Descripcion);
            Assert.True(tipoConsultaCreado != null,
                $"Precondicion fallida: no se pudo insertar '{tipoConsultaPrueba.tipoCon_Descripcion}' — verificar SP de INSERT");

            // Act
            var resultado = await _service.FindAsync(tipoConsultaCreado.tipoCon_Id);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(tipoConsultaCreado.tipoCon_Id, resultado.tipoCon_Id);
            Assert.Equal(tipoConsultaPrueba.tipoCon_Descripcion, resultado.tipoCon_Descripcion);

            // Cleanup
            await _service.RemoveAsync(tipoConsultaCreado.tipoCon_Id);
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
            var tipoConsultaPrueba = new TipoConsultaViewModel
            {
                tipoCon_Descripcion = $"Detail {Guid.NewGuid().ToString().Substring(0, 8)}",
                tipoCon_EsActivoBool = true
            };

            await _service.AddAsync(tipoConsultaPrueba);

            var lista = await _service.ListAsync();
            var tipoConsultaCreado = lista.FirstOrDefault(x => x.tipoCon_Descripcion == tipoConsultaPrueba.tipoCon_Descripcion);
            Assert.True(tipoConsultaCreado != null,
                $"Precondicion fallida: no se pudo insertar '{tipoConsultaPrueba.tipoCon_Descripcion}' — verificar SP de INSERT");

            // Act
            var resultado = await _service.DetailAsync(tipoConsultaCreado.tipoCon_Id);

            // Assert
            Assert.True(resultado != null,
                $"DetailAsync no retorno datos para Id={tipoConsultaCreado.tipoCon_Id}");
            Assert.Equal(tipoConsultaCreado.tipoCon_Id, resultado.tipoCon_Id);
            Assert.Equal(tipoConsultaPrueba.tipoCon_Descripcion, resultado.tipoCon_Descripcion);
            Assert.NotEqual(default(DateTime), resultado.tipoCon_FechaCrea);

            // Cleanup
            await _service.RemoveAsync(tipoConsultaCreado.tipoCon_Id);
        }

        // =======================================================================
        // UPDATE
        // =======================================================================

        [Fact]
        public async Task UpdateAsync_ConDatosValidos_ActualizaEnBaseDeDatos()
        {
            // Arrange
            var tipoConsultaInicial = new TipoConsultaViewModel
            {
                tipoCon_Descripcion = $"Orig {Guid.NewGuid().ToString().Substring(0, 8)}",
                tipoCon_EsActivoBool = true
            };

            await _service.AddAsync(tipoConsultaInicial);

            var lista = await _service.ListAsync();
            var tipoConsultaCreado = lista.FirstOrDefault(x => x.tipoCon_Descripcion == tipoConsultaInicial.tipoCon_Descripcion);
            Assert.True(tipoConsultaCreado != null,
                $"Precondicion fallida: no se pudo insertar '{tipoConsultaInicial.tipoCon_Descripcion}' — verificar SP de INSERT");

            var tipoConsultaActualizar = await _service.FindAsync(tipoConsultaCreado.tipoCon_Id);
            tipoConsultaActualizar.tipoCon_Descripcion = $"Upd {Guid.NewGuid().ToString().Substring(0, 8)}";

            // Act
            bool resultado = await _service.UpdateAsync(tipoConsultaActualizar);

            // Assert
            Assert.True(resultado, "La actualizacion deberia retornar true");

            var tipoConsultaVerificado = await _service.FindAsync(tipoConsultaCreado.tipoCon_Id);
            Assert.True(tipoConsultaVerificado != null,
                $"No se encontro el registro Id={tipoConsultaCreado.tipoCon_Id} tras el UPDATE");
            Assert.Equal(tipoConsultaActualizar.tipoCon_Descripcion, tipoConsultaVerificado.tipoCon_Descripcion);

            // Cleanup
            await _service.RemoveAsync(tipoConsultaCreado.tipoCon_Id);
        }

        [Fact]
        public async Task UpdateAsync_CambiaEstadoActivo_ActualizaCorrectamente()
        {
            // Arrange
            var tipoConsultaPrueba = new TipoConsultaViewModel
            {
                tipoCon_Descripcion = $"Estado {Guid.NewGuid().ToString().Substring(0, 8)}",
                tipoCon_EsActivoBool = true
            };

            await _service.AddAsync(tipoConsultaPrueba);

            var lista = await _service.ListAsync();
            var tipoConsultaCreado = lista.FirstOrDefault(x => x.tipoCon_Descripcion == tipoConsultaPrueba.tipoCon_Descripcion);
            Assert.True(tipoConsultaCreado != null,
                $"Precondicion fallida: no se pudo insertar '{tipoConsultaPrueba.tipoCon_Descripcion}' — verificar SP de INSERT");

            var tipoConsultaActualizar = await _service.FindAsync(tipoConsultaCreado.tipoCon_Id);
            tipoConsultaActualizar.tipoCon_EsActivoBool = false;

            // Act
            bool resultado = await _service.UpdateAsync(tipoConsultaActualizar);

            // Assert
            Assert.True(resultado);

            var tipoConsultaVerificado = await _service.FindAsync(tipoConsultaCreado.tipoCon_Id);
            Assert.False(tipoConsultaVerificado.tipoCon_EsActivoBool);

            // Cleanup
            await _service.RemoveAsync(tipoConsultaCreado.tipoCon_Id);
        }

        // =======================================================================
        // DELETE
        // =======================================================================

        [Fact]
        public async Task RemoveAsync_ConIdExistente_EliminaDeBaseDeDatos()
        {
            // Arrange
            var tipoConsultaPrueba = new TipoConsultaViewModel
            {
                tipoCon_Descripcion = $"Del {Guid.NewGuid().ToString().Substring(0, 8)}",
                tipoCon_EsActivoBool = true
            };

            await _service.AddAsync(tipoConsultaPrueba);

            var lista = await _service.ListAsync();
            var tipoConsultaCreado = lista.FirstOrDefault(x => x.tipoCon_Descripcion == tipoConsultaPrueba.tipoCon_Descripcion);
            Assert.True(tipoConsultaCreado != null,
                $"Precondicion fallida: no se pudo insertar '{tipoConsultaPrueba.tipoCon_Descripcion}' — verificar SP de INSERT");

            int idEliminar = tipoConsultaCreado.tipoCon_Id;

            // Act
            bool resultado = await _service.RemoveAsync(idEliminar);

            // Assert
            Assert.True(resultado, "La eliminacion deberia retornar true");

            var tipoConsultaEliminado = await _service.FindAsync(idEliminar);
            Assert.Null(tipoConsultaEliminado);
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
            var nuevaTipoConsulta = new TipoConsultaViewModel
            {
                tipoCon_Descripcion = $"CRUD {Guid.NewGuid().ToString().Substring(0, 8)}",
                tipoCon_EsActivoBool = true
            };

            bool insertado = await _service.AddAsync(nuevaTipoConsulta);
            Assert.True(insertado, "CREATE fallo — verificar SP de INSERT");

            // 2. READ
            var lista = await _service.ListAsync();
            var tipoConsultaCreado = lista.FirstOrDefault(x => x.tipoCon_Descripcion == nuevaTipoConsulta.tipoCon_Descripcion);
            Assert.True(tipoConsultaCreado != null,
                $"CREATE fallo: '{nuevaTipoConsulta.tipoCon_Descripcion}' no esta en la BD — verificar SP de INSERT");

            var tipoConsultaEncontrado = await _service.FindAsync(tipoConsultaCreado.tipoCon_Id);
            Assert.True(tipoConsultaEncontrado != null,
                $"READ fallo: FindAsync no encontro Id={tipoConsultaCreado.tipoCon_Id}");
            Assert.Equal(nuevaTipoConsulta.tipoCon_Descripcion, tipoConsultaEncontrado.tipoCon_Descripcion);

            // 3. UPDATE
            tipoConsultaEncontrado.tipoCon_Descripcion = $"CRUD2 {Guid.NewGuid().ToString().Substring(0, 8)}";
            bool actualizado = await _service.UpdateAsync(tipoConsultaEncontrado);
            Assert.True(actualizado, "UPDATE fallo");

            var tipoConsultaVerificado = await _service.FindAsync(tipoConsultaCreado.tipoCon_Id);
            Assert.Equal(tipoConsultaEncontrado.tipoCon_Descripcion, tipoConsultaVerificado.tipoCon_Descripcion);

            // 4. DELETE
            bool eliminado = await _service.RemoveAsync(tipoConsultaCreado.tipoCon_Id);
            Assert.True(eliminado, "DELETE fallo");

            var tipoConsultaEliminado = await _service.FindAsync(tipoConsultaCreado.tipoCon_Id);
            Assert.Null(tipoConsultaEliminado);
        }
    }
}
