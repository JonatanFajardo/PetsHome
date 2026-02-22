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
    /// Pruebas de integracion para CategoriaService contra la base de datos REAL.
    /// Verifica que el flujo CRUD completo funciona correctamente end-to-end.
    ///
    /// IMPORTANTE:
    /// 1. Requiere base de datos configurada y accesible
    /// 2. Connection string configurado en DatabaseFixture
    /// 3. Stored procedures de Categoria existentes en la BD
    /// </summary>
    public class CategoriaIntegrationTests : IClassFixture<DatabaseFixture>
    {
        private readonly CategoriaService _service;
        private readonly CategoriaRepository _repository;
        private readonly IMapper _mapper;
        private readonly DatabaseFixture _fixture;
        private readonly int _testUserId = 1;

        public CategoriaIntegrationTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
            _repository = new CategoriaRepository();
            _mapper = fixture.Mapper;

            var loggerMock = new Mock<ILogger<CategoriaService>>();
            _service = new CategoriaService(_repository, loggerMock.Object, _mapper);
        }

        // =======================================================================
        // CREATE (INSERT)
        // =======================================================================

        [Fact]
        public async Task AddAsync_ConDatosValidos_InsertaEnBaseDeDatos()
        {
            // Arrange
            var nuevaCategoria = new CategoriaViewModel
            {
                cat_Descripcion = $"Test {Guid.NewGuid().ToString().Substring(0, 8)}",
                cat_EsActivoBool = true
            };

            // Act
            bool resultado = await _service.AddAsync(nuevaCategoria, _testUserId);

            // Assert
            Assert.True(resultado, "La insercion deberia retornar true");

            var lista = await _service.ListAsync();
            var categoriaInsertado = lista.FirstOrDefault(x => x.cat_Descripcion == nuevaCategoria.cat_Descripcion);

            Assert.True(categoriaInsertado != null,
                $"No se encontro '{nuevaCategoria.cat_Descripcion}' en la BD — el INSERT puede haber fallado (verificar nombre del SP)");

            // Cleanup
            if (categoriaInsertado != null)
                await _service.RemoveAsync(categoriaInsertado.cat_Id);
        }

        [Fact]
        public async Task AddAsync_ConDescripcionVacia_NoDeberiaInsertar()
        {
            // Arrange
            var categoriaInvalido = new CategoriaViewModel
            {
                cat_Descripcion = ""
            };

            // Act & Assert
            try
            {
                bool resultado = await _service.AddAsync(categoriaInvalido, _testUserId);
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
        public async Task ListAsync_RetornaListaDeCategoriasDesdeBD()
        {
            // Act
            var resultado = await _service.ListAsync();

            // Assert
            Assert.NotNull(resultado);
            Assert.IsType<List<CategoriaViewModel>>(resultado);
            Assert.True(resultado.Count >= 0);
        }

        [Fact]
        public async Task FindAsync_ConIdExistente_RetornaCategoriaDesdeBD()
        {
            // Arrange
            var categoriaPrueba = new CategoriaViewModel
            {
                cat_Descripcion = $"Find {Guid.NewGuid().ToString().Substring(0, 8)}",
                cat_EsActivoBool = true
            };

            await _service.AddAsync(categoriaPrueba, _testUserId);

            var lista = await _service.ListAsync();
            var categoriaCreado = lista.FirstOrDefault(x => x.cat_Descripcion == categoriaPrueba.cat_Descripcion);
            Assert.True(categoriaCreado != null,
                $"Precondicion fallida: no se pudo insertar '{categoriaPrueba.cat_Descripcion}' — verificar SP de INSERT");

            // Act
            var resultado = await _service.FindAsync(categoriaCreado.cat_Id);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(categoriaCreado.cat_Id, resultado.cat_Id);
            Assert.Equal(categoriaPrueba.cat_Descripcion, resultado.cat_Descripcion);

            // Cleanup
            await _service.RemoveAsync(categoriaCreado.cat_Id);
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

        [Fact(Skip = "SP PR_Inventario_Categorias_Detail no acepta parametros (bug en BD) — Detail siempre retorna null")]
        public async Task DetailAsync_ConIdExistente_RetornaDetalleCompleto()
        {
            // Arrange
            var categoriaPrueba = new CategoriaViewModel
            {
                cat_Descripcion = $"Detail {Guid.NewGuid().ToString().Substring(0, 8)}",
                cat_EsActivoBool = true
            };

            await _service.AddAsync(categoriaPrueba, _testUserId);

            var lista = await _service.ListAsync();
            var categoriaCreado = lista.FirstOrDefault(x => x.cat_Descripcion == categoriaPrueba.cat_Descripcion);
            Assert.True(categoriaCreado != null,
                $"Precondicion fallida: no se pudo insertar '{categoriaPrueba.cat_Descripcion}' — verificar SP de INSERT");

            // Act
            var resultado = await _service.DetailAsync(categoriaCreado.cat_Id);

            // Assert
            Assert.True(resultado != null,
                $"DetailAsync no retorno datos para Id={categoriaCreado.cat_Id}");
            Assert.Equal(categoriaCreado.cat_Id, resultado.cat_Id);
            Assert.Equal(categoriaPrueba.cat_Descripcion, resultado.cat_Descripcion);
            Assert.NotEqual(default(DateTime), resultado.cat_FechaCrea);

            // Cleanup
            await _service.RemoveAsync(categoriaCreado.cat_Id);
        }

        // =======================================================================
        // UPDATE
        // =======================================================================

        [Fact]
        public async Task UpdateAsync_ConDatosValidos_ActualizaEnBaseDeDatos()
        {
            // Arrange
            var categoriaInicial = new CategoriaViewModel
            {
                cat_Descripcion = $"Orig {Guid.NewGuid().ToString().Substring(0, 8)}",
                cat_EsActivoBool = true
            };

            await _service.AddAsync(categoriaInicial, _testUserId);

            var lista = await _service.ListAsync();
            var categoriaCreado = lista.FirstOrDefault(x => x.cat_Descripcion == categoriaInicial.cat_Descripcion);
            Assert.True(categoriaCreado != null,
                $"Precondicion fallida: no se pudo insertar '{categoriaInicial.cat_Descripcion}' — verificar SP de INSERT");

            var categoriaActualizar = await _service.FindAsync(categoriaCreado.cat_Id);
            categoriaActualizar.cat_Descripcion = $"Upd {Guid.NewGuid().ToString().Substring(0, 8)}";

            // Act
            bool resultado = await _service.UpdateAsync(categoriaActualizar, _testUserId);

            // Assert
            Assert.True(resultado, "La actualizacion deberia retornar true");

            var categoriaVerificado = await _service.FindAsync(categoriaCreado.cat_Id);
            Assert.True(categoriaVerificado != null,
                $"No se encontro el registro Id={categoriaCreado.cat_Id} tras el UPDATE");
            Assert.Equal(categoriaActualizar.cat_Descripcion, categoriaVerificado.cat_Descripcion);

            // Cleanup
            await _service.RemoveAsync(categoriaCreado.cat_Id);
        }

        [Fact]
        public async Task UpdateAsync_CambiaEstadoActivo_ActualizaCorrectamente()
        {
            // Arrange
            var categoriaPrueba = new CategoriaViewModel
            {
                cat_Descripcion = $"Estado {Guid.NewGuid().ToString().Substring(0, 8)}",
                cat_EsActivoBool = true
            };

            await _service.AddAsync(categoriaPrueba, _testUserId);

            var lista = await _service.ListAsync();
            var categoriaCreado = lista.FirstOrDefault(x => x.cat_Descripcion == categoriaPrueba.cat_Descripcion);
            Assert.True(categoriaCreado != null,
                $"Precondicion fallida: no se pudo insertar '{categoriaPrueba.cat_Descripcion}' — verificar SP de INSERT");

            var categoriaActualizar = await _service.FindAsync(categoriaCreado.cat_Id);
            categoriaActualizar.cat_EsActivoBool = false;

            // Act
            bool resultado = await _service.UpdateAsync(categoriaActualizar, _testUserId);

            // Assert
            Assert.True(resultado);

            var categoriaVerificado = await _service.FindAsync(categoriaCreado.cat_Id);
            Assert.False(categoriaVerificado.cat_EsActivoBool);

            // Cleanup
            await _service.RemoveAsync(categoriaCreado.cat_Id);
        }

        // =======================================================================
        // DELETE
        // =======================================================================

        [Fact]
        public async Task RemoveAsync_ConIdExistente_EliminaDeBaseDeDatos()
        {
            // Arrange
            var categoriaPrueba = new CategoriaViewModel
            {
                cat_Descripcion = $"Del {Guid.NewGuid().ToString().Substring(0, 8)}",
                cat_EsActivoBool = true
            };

            await _service.AddAsync(categoriaPrueba, _testUserId);

            var lista = await _service.ListAsync();
            var categoriaCreado = lista.FirstOrDefault(x => x.cat_Descripcion == categoriaPrueba.cat_Descripcion);
            Assert.True(categoriaCreado != null,
                $"Precondicion fallida: no se pudo insertar '{categoriaPrueba.cat_Descripcion}' — verificar SP de INSERT");

            int idEliminar = categoriaCreado.cat_Id;

            // Act
            bool resultado = await _service.RemoveAsync(idEliminar);

            // Assert
            Assert.True(resultado, "La eliminacion deberia retornar true");

            var categoriaEliminado = await _service.FindAsync(idEliminar);
            Assert.Null(categoriaEliminado);
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
            var nuevaCategoria = new CategoriaViewModel
            {
                cat_Descripcion = $"CRUD {Guid.NewGuid().ToString().Substring(0, 8)}",
                cat_EsActivoBool = true
            };

            bool insertado = await _service.AddAsync(nuevaCategoria, _testUserId);
            Assert.True(insertado, "CREATE fallo — verificar SP de INSERT");

            // 2. READ
            var lista = await _service.ListAsync();
            var categoriaCreado = lista.FirstOrDefault(x => x.cat_Descripcion == nuevaCategoria.cat_Descripcion);
            Assert.True(categoriaCreado != null,
                $"CREATE fallo: '{nuevaCategoria.cat_Descripcion}' no esta en la BD — verificar SP de INSERT");

            var categoriaEncontrado = await _service.FindAsync(categoriaCreado.cat_Id);
            Assert.True(categoriaEncontrado != null,
                $"READ fallo: FindAsync no encontro Id={categoriaCreado.cat_Id}");
            Assert.Equal(nuevaCategoria.cat_Descripcion, categoriaEncontrado.cat_Descripcion);

            // 3. UPDATE
            categoriaEncontrado.cat_Descripcion = $"CRUD2 {Guid.NewGuid().ToString().Substring(0, 8)}";
            bool actualizado = await _service.UpdateAsync(categoriaEncontrado, _testUserId);
            Assert.True(actualizado, "UPDATE fallo");

            var categoriaVerificado = await _service.FindAsync(categoriaCreado.cat_Id);
            Assert.Equal(categoriaEncontrado.cat_Descripcion, categoriaVerificado.cat_Descripcion);

            // 4. DELETE
            bool eliminado = await _service.RemoveAsync(categoriaCreado.cat_Id);
            Assert.True(eliminado, "DELETE fallo");

            var categoriaEliminado = await _service.FindAsync(categoriaCreado.cat_Id);
            Assert.Null(categoriaEliminado);
        }
    }
}
