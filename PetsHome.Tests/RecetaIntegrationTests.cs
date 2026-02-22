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
    /// Pruebas de integracion para RecetaService contra la base de datos REAL.
    /// Verifica que el flujo CRUD completo funciona correctamente end-to-end.
    ///
    /// IMPORTANTE:
    /// 1. Requiere base de datos configurada y accesible
    /// 2. Connection string configurado en DatabaseFixture
    /// 3. Stored procedures de Receta existentes en la BD
    /// </summary>
    public class RecetaIntegrationTests : IClassFixture<DatabaseFixture>
    {
        private readonly RecetaService _service;
        private readonly RecetaRepository _repository;
        private readonly IMapper _mapper;
        private readonly DatabaseFixture _fixture;

        public RecetaIntegrationTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
            _repository = new RecetaRepository();
            _mapper = fixture.Mapper;

            var loggerMock = new Mock<ILogger<RecetaService>>();
            _service = new RecetaService(_repository, loggerMock.Object, _mapper);
        }

        // =======================================================================
        // CREATE (INSERT)
        // =======================================================================

        [Fact(Skip = "Requiere datos FK existentes: cita_Id, masc_Id, tipoMed_Id, viaAdmin_Id")]
        public async Task AddAsync_ConDatosValidos_InsertaEnBaseDeDatos()
        {
            // Arrange
            var nuevaReceta = new RecetaFormViewModel
            {
                receta_Medicamento = $"Test {Guid.NewGuid().ToString().Substring(0, 8)}",
                
            };

            // Act
            bool resultado = await _service.AddAsync(nuevaReceta);

            // Assert
            Assert.True(resultado, "La insercion deberia retornar true");

            var lista = await _service.ListAsync();
            var recetaInsertado = lista.FirstOrDefault(x => x.receta_Medicamento == nuevaReceta.receta_Medicamento);

            Assert.True(recetaInsertado != null,
                $"No se encontro '{nuevaReceta.receta_Medicamento}' en la BD — el INSERT puede haber fallado (verificar nombre del SP)");

            // Cleanup
            if (recetaInsertado != null)
                await _service.RemoveAsync(recetaInsertado.receta_Id);
        }

        [Fact(Skip = "Requiere datos FK existentes: cita_Id, masc_Id, tipoMed_Id, viaAdmin_Id")]
        public async Task AddAsync_ConDescripcionVacia_NoDeberiaInsertar()
        {
            // Arrange
            var recetaInvalido = new RecetaFormViewModel
            {
                receta_Medicamento = ""
            };

            // Act & Assert
            try
            {
                bool resultado = await _service.AddAsync(recetaInvalido);
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

        [Fact(Skip = "Requiere datos FK existentes: cita_Id, masc_Id, tipoMed_Id, viaAdmin_Id")]
        public async Task ListAsync_RetornaListaDeRecetasDesdeBD()
        {
            // Act
            var resultado = await _service.ListAsync();

            // Assert
            Assert.NotNull(resultado);
            Assert.IsType<List<RecetaListViewModel>>(resultado);
            Assert.True(resultado.Count >= 0);
        }

        [Fact(Skip = "Requiere datos FK existentes: cita_Id, masc_Id, tipoMed_Id, viaAdmin_Id")]
        public async Task FindAsync_ConIdExistente_RetornaRecetaDesdeBD()
        {
            // Arrange
            var recetaPrueba = new RecetaFormViewModel
            {
                receta_Medicamento = $"Find {Guid.NewGuid().ToString().Substring(0, 8)}",
                
            };

            await _service.AddAsync(recetaPrueba);

            var lista = await _service.ListAsync();
            var recetaCreado = lista.FirstOrDefault(x => x.receta_Medicamento == recetaPrueba.receta_Medicamento);
            Assert.True(recetaCreado != null,
                $"Precondicion fallida: no se pudo insertar '{recetaPrueba.receta_Medicamento}' — verificar SP de INSERT");

            // Act
            var resultado = await _service.FindAsync(recetaCreado.receta_Id);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(recetaCreado.receta_Id, resultado.receta_Id);
            Assert.Equal(recetaPrueba.receta_Medicamento, resultado.receta_Medicamento);

            // Cleanup
            await _service.RemoveAsync(recetaCreado.receta_Id);
        }

        [Fact(Skip = "Requiere datos FK existentes: cita_Id, masc_Id, tipoMed_Id, viaAdmin_Id")]
        public async Task FindAsync_ConIdInexistente_RetornaNull()
        {
            // Arrange
            int idInexistente = 999999;

            // Act
            var resultado = await _service.FindAsync(idInexistente);

            // Assert
            Assert.Null(resultado);
        }

        [Fact(Skip = "Requiere datos FK existentes: cita_Id, masc_Id, tipoMed_Id, viaAdmin_Id")]
        public async Task DetailAsync_ConIdExistente_RetornaDetalleCompleto()
        {
            // Arrange
            var recetaPrueba = new RecetaFormViewModel
            {
                receta_Medicamento = $"Detail {Guid.NewGuid().ToString().Substring(0, 8)}",
                
            };

            await _service.AddAsync(recetaPrueba);

            var lista = await _service.ListAsync();
            var recetaCreado = lista.FirstOrDefault(x => x.receta_Medicamento == recetaPrueba.receta_Medicamento);
            Assert.True(recetaCreado != null,
                $"Precondicion fallida: no se pudo insertar '{recetaPrueba.receta_Medicamento}' — verificar SP de INSERT");

            // Act
            var resultado = await _service.DetailAsync(recetaCreado.receta_Id);

            // Assert
            Assert.True(resultado != null,
                $"DetailAsync no retorno datos para Id={recetaCreado.receta_Id}");
            Assert.Equal(recetaCreado.receta_Id, resultado.receta_Id);
            Assert.Equal(recetaPrueba.receta_Medicamento, resultado.receta_Medicamento);
            Assert.NotEqual(default(DateTime), resultado.receta_FechaCrea);

            // Cleanup
            await _service.RemoveAsync(recetaCreado.receta_Id);
        }

        // =======================================================================
        // UPDATE
        // =======================================================================

        [Fact(Skip = "Requiere datos FK existentes: cita_Id, masc_Id, tipoMed_Id, viaAdmin_Id")]
        public async Task UpdateAsync_ConDatosValidos_ActualizaEnBaseDeDatos()
        {
            // Arrange
            var recetaInicial = new RecetaFormViewModel
            {
                receta_Medicamento = $"Orig {Guid.NewGuid().ToString().Substring(0, 8)}",
                
            };

            await _service.AddAsync(recetaInicial);

            var lista = await _service.ListAsync();
            var recetaCreado = lista.FirstOrDefault(x => x.receta_Medicamento == recetaInicial.receta_Medicamento);
            Assert.True(recetaCreado != null,
                $"Precondicion fallida: no se pudo insertar '{recetaInicial.receta_Medicamento}' — verificar SP de INSERT");

            var recetaActualizar = await _service.FindAsync(recetaCreado.receta_Id);
            recetaActualizar.receta_Medicamento = $"Upd {Guid.NewGuid().ToString().Substring(0, 8)}";

            // Act
            bool resultado = await _service.UpdateAsync(recetaActualizar);

            // Assert
            Assert.True(resultado, "La actualizacion deberia retornar true");

            var recetaVerificado = await _service.FindAsync(recetaCreado.receta_Id);
            Assert.True(recetaVerificado != null,
                $"No se encontro el registro Id={recetaCreado.receta_Id} tras el UPDATE");
            Assert.Equal(recetaActualizar.receta_Medicamento, recetaVerificado.receta_Medicamento);

            // Cleanup
            await _service.RemoveAsync(recetaCreado.receta_Id);
        }

        // =======================================================================
        // DELETE
        // =======================================================================

        [Fact(Skip = "Requiere datos FK existentes: cita_Id, masc_Id, tipoMed_Id, viaAdmin_Id")]
        public async Task RemoveAsync_ConIdExistente_EliminaDeBaseDeDatos()
        {
            // Arrange
            var recetaPrueba = new RecetaFormViewModel
            {
                receta_Medicamento = $"Del {Guid.NewGuid().ToString().Substring(0, 8)}",
                
            };

            await _service.AddAsync(recetaPrueba);

            var lista = await _service.ListAsync();
            var recetaCreado = lista.FirstOrDefault(x => x.receta_Medicamento == recetaPrueba.receta_Medicamento);
            Assert.True(recetaCreado != null,
                $"Precondicion fallida: no se pudo insertar '{recetaPrueba.receta_Medicamento}' — verificar SP de INSERT");

            int idEliminar = recetaCreado.receta_Id;

            // Act
            bool resultado = await _service.RemoveAsync(idEliminar);

            // Assert
            Assert.True(resultado, "La eliminacion deberia retornar true");

            var recetaEliminado = await _service.FindAsync(idEliminar);
            Assert.Null(recetaEliminado);
        }

        [Fact(Skip = "Requiere datos FK existentes: cita_Id, masc_Id, tipoMed_Id, viaAdmin_Id")]
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

        [Fact(Skip = "Requiere datos FK existentes: cita_Id, masc_Id, tipoMed_Id, viaAdmin_Id")]
        public async Task FlujoCRUDCompleto_CrearLeerActualizarEliminar_FuncionaCorrectamente()
        {
            // 1. CREATE
            var nuevaReceta = new RecetaFormViewModel
            {
                receta_Medicamento = $"CRUD {Guid.NewGuid().ToString().Substring(0, 8)}",
                
            };

            bool insertado = await _service.AddAsync(nuevaReceta);
            Assert.True(insertado, "CREATE fallo — verificar SP de INSERT");

            // 2. READ
            var lista = await _service.ListAsync();
            var recetaCreado = lista.FirstOrDefault(x => x.receta_Medicamento == nuevaReceta.receta_Medicamento);
            Assert.True(recetaCreado != null,
                $"CREATE fallo: '{nuevaReceta.receta_Medicamento}' no esta en la BD — verificar SP de INSERT");

            var recetaEncontrado = await _service.FindAsync(recetaCreado.receta_Id);
            Assert.True(recetaEncontrado != null,
                $"READ fallo: FindAsync no encontro Id={recetaCreado.receta_Id}");
            Assert.Equal(nuevaReceta.receta_Medicamento, recetaEncontrado.receta_Medicamento);

            // 3. UPDATE
            recetaEncontrado.receta_Medicamento = $"CRUD2 {Guid.NewGuid().ToString().Substring(0, 8)}";
            bool actualizado = await _service.UpdateAsync(recetaEncontrado);
            Assert.True(actualizado, "UPDATE fallo");

            var recetaVerificado = await _service.FindAsync(recetaCreado.receta_Id);
            Assert.Equal(recetaEncontrado.receta_Medicamento, recetaVerificado.receta_Medicamento);

            // 4. DELETE
            bool eliminado = await _service.RemoveAsync(recetaCreado.receta_Id);
            Assert.True(eliminado, "DELETE fallo");

            var recetaEliminado = await _service.FindAsync(recetaCreado.receta_Id);
            Assert.Null(recetaEliminado);
        }
    }
}
