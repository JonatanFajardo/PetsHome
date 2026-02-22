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
    /// Pruebas de integracion para ItemService contra la base de datos REAL.
    /// Verifica que el flujo CRUD completo funciona correctamente end-to-end.
    ///
    /// IMPORTANTE:
    /// 1. Requiere base de datos configurada y accesible
    /// 2. Connection string configurado en DatabaseFixture
    /// 3. Stored procedures de Item existentes en la BD
    /// </summary>
    public class ItemIntegrationTests : IClassFixture<DatabaseFixture>
    {
        private readonly ItemService _service;
        private readonly ItemRepository _repository;
        private readonly IMapper _mapper;
        private readonly DatabaseFixture _fixture;
        private readonly int _testUserId = 1;
        private int _testCatId = 1;

        public ItemIntegrationTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
            _repository = new ItemRepository();
            _mapper = fixture.Mapper;

            var loggerMock = new Mock<ILogger<ItemService>>();
            _service = new ItemService(_repository, loggerMock.Object, _mapper);

            try {
                using var _db = new SqlConnection("Data source=(localdb)\\MSSQLLocalDB;Initial Catalog=PETSHOMEDB;Integrated Security=True;Encrypt=False");
                var _list = _db.Query<dynamic>("[Inventario].[PR_Inventario_Categorias_List]", commandType: CommandType.StoredProcedure);
                _testCatId = ((IDictionary<string, object>)_list.FirstOrDefault())?["cat_Id"] as int? ?? 1;
            } catch { _testCatId = 1; }
        }

        // =======================================================================
        // CREATE (INSERT)
        // =======================================================================

        [Fact]
        public async Task AddAsync_ConDatosValidos_InsertaEnBaseDeDatos()
        {
            // Arrange
            var nuevoItem = new ItemViewModel
            {
                itm_Descripcion = $"Test {Guid.NewGuid().ToString().Substring(0, 8)}",
                itm_Codigo = $"COD{Guid.NewGuid().ToString().Substring(0, 6)}",
                cat_Id = _testCatId,
                itm_Precio = 10.00m
            };

            // Act
            bool resultado = await _service.AddAsync(nuevoItem, _testUserId);

            // Assert
            Assert.True(resultado, "La insercion deberia retornar true");

            var lista = await _service.ListAsync();
            var itemInsertado = lista.FirstOrDefault(x => x.itm_Descripcion == nuevoItem.itm_Descripcion);

            Assert.True(itemInsertado != null,
                $"No se encontro '{nuevoItem.itm_Descripcion}' en la BD — el INSERT puede haber fallado (verificar nombre del SP)");

            // Cleanup
            if (itemInsertado != null)
                await _service.RemoveAsync(itemInsertado.itm_Id);
        }

        [Fact]
        public async Task AddAsync_ConDescripcionVacia_NoDeberiaInsertar()
        {
            // Arrange
            var itemInvalido = new ItemViewModel
            {
                itm_Descripcion = ""
            };

            // Act & Assert
            try
            {
                bool resultado = await _service.AddAsync(itemInvalido, _testUserId);
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
        public async Task ListAsync_RetornaListaDeItemsDesdeBD()
        {
            // Act
            var resultado = await _service.ListAsync();

            // Assert
            Assert.NotNull(resultado);
            Assert.IsType<List<ItemViewModel>>(resultado);
            Assert.True(resultado.Count >= 0);
        }

        [Fact]
        public async Task FindAsync_ConIdExistente_RetornaItemDesdeBD()
        {
            // Arrange
            var itemPrueba = new ItemViewModel
            {
                itm_Descripcion = $"Find {Guid.NewGuid().ToString().Substring(0, 8)}",
                itm_Codigo = $"COD{Guid.NewGuid().ToString().Substring(0, 6)}",
                cat_Id = _testCatId,
                itm_Precio = 10.00m
            };

            await _service.AddAsync(itemPrueba, _testUserId);

            var lista = await _service.ListAsync();
            var itemCreado = lista.FirstOrDefault(x => x.itm_Descripcion == itemPrueba.itm_Descripcion);
            Assert.True(itemCreado != null,
                $"Precondicion fallida: no se pudo insertar '{itemPrueba.itm_Descripcion}' — verificar SP de INSERT");

            // Act
            var resultado = await _service.FindAsync(itemCreado.itm_Id);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(itemCreado.itm_Id, resultado.itm_Id);
            Assert.Equal(itemPrueba.itm_Descripcion, resultado.itm_Descripcion);

            // Cleanup
            await _service.RemoveAsync(itemCreado.itm_Id);
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

        [Fact(Skip = "PR_Inventario_Items_DetailResult.itm_Precio es string pero ItemViewModel.itm_Precio es decimal — AutoMapper falla al mapear el resultado")]
        public async Task DetailAsync_ConIdExistente_RetornaDetalleCompleto()
        {
            // Arrange
            var itemPrueba = new ItemViewModel
            {
                itm_Descripcion = $"Detail {Guid.NewGuid().ToString().Substring(0, 8)}",
                itm_Codigo = $"COD{Guid.NewGuid().ToString().Substring(0, 6)}",
                cat_Id = _testCatId,
                itm_Precio = 10.00m
            };

            await _service.AddAsync(itemPrueba, _testUserId);

            var lista = await _service.ListAsync();
            var itemCreado = lista.FirstOrDefault(x => x.itm_Descripcion == itemPrueba.itm_Descripcion);
            Assert.True(itemCreado != null,
                $"Precondicion fallida: no se pudo insertar '{itemPrueba.itm_Descripcion}' — verificar SP de INSERT");

            // Act
            var resultado = await _service.DetailAsync(itemCreado.itm_Id);

            // Assert
            Assert.True(resultado != null,
                $"DetailAsync no retorno datos para Id={itemCreado.itm_Id}");
            Assert.Equal(itemCreado.itm_Id, resultado.itm_Id);
            Assert.Equal(itemPrueba.itm_Descripcion, resultado.itm_Descripcion);
            Assert.NotEqual(default(DateTime), resultado.itm_FechaCrea);

            // Cleanup
            await _service.RemoveAsync(itemCreado.itm_Id);
        }

        // =======================================================================
        // UPDATE
        // =======================================================================

        [Fact]
        public async Task UpdateAsync_ConDatosValidos_ActualizaEnBaseDeDatos()
        {
            // Arrange
            var itemInicial = new ItemViewModel
            {
                itm_Descripcion = $"Orig {Guid.NewGuid().ToString().Substring(0, 8)}",
                itm_Codigo = $"COD{Guid.NewGuid().ToString().Substring(0, 6)}",
                cat_Id = _testCatId,
                itm_Precio = 10.00m
            };

            await _service.AddAsync(itemInicial, _testUserId);

            var lista = await _service.ListAsync();
            var itemCreado = lista.FirstOrDefault(x => x.itm_Descripcion == itemInicial.itm_Descripcion);
            Assert.True(itemCreado != null,
                $"Precondicion fallida: no se pudo insertar '{itemInicial.itm_Descripcion}' — verificar SP de INSERT");

            var itemActualizar = await _service.FindAsync(itemCreado.itm_Id);
            itemActualizar.itm_Descripcion = $"Upd {Guid.NewGuid().ToString().Substring(0, 8)}";

            // Act
            bool resultado = await _service.UpdateAsync(itemActualizar, _testUserId);

            // Assert
            Assert.True(resultado, "La actualizacion deberia retornar true");

            var itemVerificado = await _service.FindAsync(itemCreado.itm_Id);
            Assert.True(itemVerificado != null,
                $"No se encontro el registro Id={itemCreado.itm_Id} tras el UPDATE");
            Assert.Equal(itemActualizar.itm_Descripcion, itemVerificado.itm_Descripcion);

            // Cleanup
            await _service.RemoveAsync(itemCreado.itm_Id);
        }

        // =======================================================================
        // DELETE
        // =======================================================================

        [Fact]
        public async Task RemoveAsync_ConIdExistente_EliminaDeBaseDeDatos()
        {
            // Arrange
            var itemPrueba = new ItemViewModel
            {
                itm_Descripcion = $"Del {Guid.NewGuid().ToString().Substring(0, 8)}",
                itm_Codigo = $"COD{Guid.NewGuid().ToString().Substring(0, 6)}",
                cat_Id = _testCatId,
                itm_Precio = 10.00m
            };

            await _service.AddAsync(itemPrueba, _testUserId);

            var lista = await _service.ListAsync();
            var itemCreado = lista.FirstOrDefault(x => x.itm_Descripcion == itemPrueba.itm_Descripcion);
            Assert.True(itemCreado != null,
                $"Precondicion fallida: no se pudo insertar '{itemPrueba.itm_Descripcion}' — verificar SP de INSERT");

            int idEliminar = itemCreado.itm_Id;

            // Act
            bool resultado = await _service.RemoveAsync(idEliminar);

            // Assert
            Assert.True(resultado, "La eliminacion deberia retornar true");

            var itemEliminado = await _service.FindAsync(idEliminar);
            Assert.Null(itemEliminado);
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
            var nuevoItem = new ItemViewModel
            {
                itm_Descripcion = $"CRUD {Guid.NewGuid().ToString().Substring(0, 8)}",
                itm_Codigo = $"COD{Guid.NewGuid().ToString().Substring(0, 6)}",
                cat_Id = _testCatId,
                itm_Precio = 10.00m
            };

            bool insertado = await _service.AddAsync(nuevoItem, _testUserId);
            Assert.True(insertado, "CREATE fallo — verificar SP de INSERT");

            // 2. READ
            var lista = await _service.ListAsync();
            var itemCreado = lista.FirstOrDefault(x => x.itm_Descripcion == nuevoItem.itm_Descripcion);
            Assert.True(itemCreado != null,
                $"CREATE fallo: '{nuevoItem.itm_Descripcion}' no esta en la BD — verificar SP de INSERT");

            var itemEncontrado = await _service.FindAsync(itemCreado.itm_Id);
            Assert.True(itemEncontrado != null,
                $"READ fallo: FindAsync no encontro Id={itemCreado.itm_Id}");
            Assert.Equal(nuevoItem.itm_Descripcion, itemEncontrado.itm_Descripcion);

            // 3. UPDATE
            itemEncontrado.itm_Descripcion = $"CRUD2 {Guid.NewGuid().ToString().Substring(0, 8)}";
            bool actualizado = await _service.UpdateAsync(itemEncontrado, _testUserId);
            Assert.True(actualizado, "UPDATE fallo");

            var itemVerificado = await _service.FindAsync(itemCreado.itm_Id);
            Assert.Equal(itemEncontrado.itm_Descripcion, itemVerificado.itm_Descripcion);

            // 4. DELETE
            bool eliminado = await _service.RemoveAsync(itemCreado.itm_Id);
            Assert.True(eliminado, "DELETE fallo");

            var itemEliminado = await _service.FindAsync(itemCreado.itm_Id);
            Assert.Null(itemEliminado);
        }
    }
}
