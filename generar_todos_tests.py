"""
Genera archivos IntegrationTests.cs para TODOS los servicios de PetsHome.
Uso: python generar_todos_tests.py
"""

import os

TESTS_DIR = os.path.join(os.path.dirname(os.path.abspath(__file__)), "PetsHome.Tests")

# ---------------------------------------------------------------------------
# PLANTILLA CON USERID
# ---------------------------------------------------------------------------
TEMPLATE_CON_USERID = '''\
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
{{
    /// <summary>
    /// Pruebas de integracion para {servicio} contra la base de datos REAL.
    /// Verifica que el flujo CRUD completo funciona correctamente end-to-end.
    ///
    /// IMPORTANTE:
    /// 1. Requiere base de datos configurada y accesible
    /// 2. Connection string configurado en DatabaseFixture
    /// 3. Stored procedures de {entidad} existentes en la BD
    /// </summary>
    public class {entidad}IntegrationTests : IClassFixture<DatabaseFixture>
    {{
        private readonly {servicio} _service;
        private readonly {repositorio} _repository;
        private readonly IMapper _mapper;
        private readonly DatabaseFixture _fixture;
        private readonly int _testUserId = 1;

        public {entidad}IntegrationTests(DatabaseFixture fixture)
        {{
            _fixture = fixture;
            _repository = new {repositorio}();
            _mapper = fixture.Mapper;

            var loggerMock = new Mock<ILogger<{servicio}>>();
            _service = new {servicio}(_repository, loggerMock.Object, _mapper);
        }}

        // =======================================================================
        // CREATE (INSERT)
        // =======================================================================

        [Fact]
        public async Task AddAsync_ConDatosValidos_InsertaEnBaseDeDatos()
        {{
            // Arrange
            var {var_nuevo} = new {form_vm}
            {{
                {campo_desc} = $"Test {{Guid.NewGuid().ToString().Substring(0, 8)}}",
                {campo_activo} = true{campos_extra_create}
            }};

            // Act
            bool resultado = await _service.AddAsync({var_nuevo}, _testUserId);

            // Assert
            Assert.True(resultado, "La insercion deberia retornar true");

            var lista = await _service.ListAsync();
            var {var_insertado} = lista.FirstOrDefault(x => x.{campo_desc} == {var_nuevo}.{campo_desc});

            Assert.True({var_insertado} != null,
                $"No se encontro '{{{var_nuevo}.{campo_desc}}}' en la BD — el INSERT puede haber fallado (verificar nombre del SP)");

            // Cleanup
            if ({var_insertado} != null)
                await _service.RemoveAsync({var_insertado}.{campo_id});
        }}

        [Fact]
        public async Task AddAsync_ConDescripcionVacia_NoDeberiaInsertar()
        {{
            // Arrange
            var {var_invalido} = new {form_vm}
            {{
                {campo_desc} = ""
            }};

            // Act & Assert
            try
            {{
                bool resultado = await _service.AddAsync({var_invalido}, _testUserId);
                Assert.True(true);
            }}
            catch (Exception ex)
            {{
                Assert.NotNull(ex);
            }}
        }}

        // =======================================================================
        // READ (SELECT)
        // =======================================================================

        [Fact]
        public async Task ListAsync_RetornaListaDe{entidad}sDesdeBD()
        {{
            // Act
            var resultado = await _service.ListAsync();

            // Assert
            Assert.NotNull(resultado);
            Assert.IsType<List<{list_vm}>>(resultado);
            Assert.True(resultado.Count >= 0);
        }}

        [Fact]
        public async Task FindAsync_ConIdExistente_Retorna{entidad}DesdeBD()
        {{
            // Arrange
            var {var_prueba} = new {form_vm}
            {{
                {campo_desc} = $"Find {{Guid.NewGuid().ToString().Substring(0, 8)}}",
                {campo_activo} = true{campos_extra_create}
            }};

            await _service.AddAsync({var_prueba}, _testUserId);

            var lista = await _service.ListAsync();
            var {var_creado} = lista.FirstOrDefault(x => x.{campo_desc} == {var_prueba}.{campo_desc});
            Assert.True({var_creado} != null,
                $"Precondicion fallida: no se pudo insertar '{{{var_prueba}.{campo_desc}}}' — verificar SP de INSERT");

            // Act
            var resultado = await _service.FindAsync({var_creado}.{campo_id});

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal({var_creado}.{campo_id}, resultado.{campo_id});
            Assert.Equal({var_prueba}.{campo_desc}, resultado.{campo_desc});

            // Cleanup
            await _service.RemoveAsync({var_creado}.{campo_id});
        }}

        [Fact]
        public async Task FindAsync_ConIdInexistente_RetornaNull()
        {{
            // Arrange
            int idInexistente = 999999;

            // Act
            var resultado = await _service.FindAsync(idInexistente);

            // Assert
            Assert.Null(resultado);
        }}

        [Fact]
        public async Task DetailAsync_ConIdExistente_RetornaDetalleCompleto()
        {{
            // Arrange
            var {var_prueba} = new {form_vm}
            {{
                {campo_desc} = $"Detail {{Guid.NewGuid().ToString().Substring(0, 8)}}",
                {campo_activo} = true{campos_extra_create}
            }};

            await _service.AddAsync({var_prueba}, _testUserId);

            var lista = await _service.ListAsync();
            var {var_creado} = lista.FirstOrDefault(x => x.{campo_desc} == {var_prueba}.{campo_desc});
            Assert.True({var_creado} != null,
                $"Precondicion fallida: no se pudo insertar '{{{var_prueba}.{campo_desc}}}' — verificar SP de INSERT");

            // Act
            var resultado = await _service.DetailAsync({var_creado}.{campo_id});

            // Assert
            Assert.True(resultado != null,
                $"DetailAsync no retorno datos para Id={{{var_creado}.{campo_id}}}");
            Assert.Equal({var_creado}.{campo_id}, resultado.{campo_id});
            Assert.Equal({var_prueba}.{campo_desc}, resultado.{campo_desc});
{assert_fechacrea}
            // Cleanup
            await _service.RemoveAsync({var_creado}.{campo_id});
        }}

        // =======================================================================
        // UPDATE
        // =======================================================================

        [Fact]
        public async Task UpdateAsync_ConDatosValidos_ActualizaEnBaseDeDatos()
        {{
            // Arrange
            var {var_inicial} = new {form_vm}
            {{
                {campo_desc} = $"Orig {{Guid.NewGuid().ToString().Substring(0, 8)}}",
                {campo_activo} = true{campos_extra_create}
            }};

            await _service.AddAsync({var_inicial}, _testUserId);

            var lista = await _service.ListAsync();
            var {var_creado} = lista.FirstOrDefault(x => x.{campo_desc} == {var_inicial}.{campo_desc});
            Assert.True({var_creado} != null,
                $"Precondicion fallida: no se pudo insertar '{{{var_inicial}.{campo_desc}}}' — verificar SP de INSERT");

            var {var_actualizar} = await _service.FindAsync({var_creado}.{campo_id});
            {var_actualizar}.{campo_desc} = $"Upd {{Guid.NewGuid().ToString().Substring(0, 8)}}";

            // Act
            bool resultado = await _service.UpdateAsync({var_actualizar}, _testUserId);

            // Assert
            Assert.True(resultado, "La actualizacion deberia retornar true");

            var {var_verificado} = await _service.FindAsync({var_creado}.{campo_id});
            Assert.True({var_verificado} != null,
                $"No se encontro el registro Id={{{var_creado}.{campo_id}}} tras el UPDATE");
            Assert.Equal({var_actualizar}.{campo_desc}, {var_verificado}.{campo_desc});

            // Cleanup
            await _service.RemoveAsync({var_creado}.{campo_id});
        }}
{test_activo}
        // =======================================================================
        // DELETE
        // =======================================================================

        [Fact]
        public async Task RemoveAsync_ConIdExistente_EliminaDeBaseDeDatos()
        {{
            // Arrange
            var {var_prueba} = new {form_vm}
            {{
                {campo_desc} = $"Del {{Guid.NewGuid().ToString().Substring(0, 8)}}",
                {campo_activo} = true{campos_extra_create}
            }};

            await _service.AddAsync({var_prueba}, _testUserId);

            var lista = await _service.ListAsync();
            var {var_creado} = lista.FirstOrDefault(x => x.{campo_desc} == {var_prueba}.{campo_desc});
            Assert.True({var_creado} != null,
                $"Precondicion fallida: no se pudo insertar '{{{var_prueba}.{campo_desc}}}' — verificar SP de INSERT");

            int idEliminar = {var_creado}.{campo_id};

            // Act
            bool resultado = await _service.RemoveAsync(idEliminar);

            // Assert
            Assert.True(resultado, "La eliminacion deberia retornar true");

            var {var_eliminado} = await _service.FindAsync(idEliminar);
            Assert.Null({var_eliminado});
        }}

        [Fact]
        public async Task RemoveAsync_ConIdInexistente_RetornaFalse()
        {{
            // Arrange
            int idInexistente = 999999;

            // Act
            bool resultado = await _service.RemoveAsync(idInexistente);

            // Assert
            Assert.False(resultado, "Eliminar un ID inexistente deberia retornar false");
        }}

        // =======================================================================
        // FLUJO CRUD COMPLETO
        // =======================================================================

        [Fact]
        public async Task FlujoCRUDCompleto_CrearLeerActualizarEliminar_FuncionaCorrectamente()
        {{
            // 1. CREATE
            var {var_nuevo} = new {form_vm}
            {{
                {campo_desc} = $"CRUD {{Guid.NewGuid().ToString().Substring(0, 8)}}",
                {campo_activo} = true{campos_extra_create}
            }};

            bool insertado = await _service.AddAsync({var_nuevo}, _testUserId);
            Assert.True(insertado, "CREATE fallo — verificar SP de INSERT");

            // 2. READ
            var lista = await _service.ListAsync();
            var {var_creado} = lista.FirstOrDefault(x => x.{campo_desc} == {var_nuevo}.{campo_desc});
            Assert.True({var_creado} != null,
                $"CREATE fallo: '{{{var_nuevo}.{campo_desc}}}' no esta en la BD — verificar SP de INSERT");

            var {var_encontrado} = await _service.FindAsync({var_creado}.{campo_id});
            Assert.True({var_encontrado} != null,
                $"READ fallo: FindAsync no encontro Id={{{var_creado}.{campo_id}}}");
            Assert.Equal({var_nuevo}.{campo_desc}, {var_encontrado}.{campo_desc});

            // 3. UPDATE
            {var_encontrado}.{campo_desc} = $"CRUD2 {{Guid.NewGuid().ToString().Substring(0, 8)}}";
            bool actualizado = await _service.UpdateAsync({var_encontrado}, _testUserId);
            Assert.True(actualizado, "UPDATE fallo");

            var {var_verificado} = await _service.FindAsync({var_creado}.{campo_id});
            Assert.Equal({var_encontrado}.{campo_desc}, {var_verificado}.{campo_desc});

            // 4. DELETE
            bool eliminado = await _service.RemoveAsync({var_creado}.{campo_id});
            Assert.True(eliminado, "DELETE fallo");

            var {var_eliminado} = await _service.FindAsync({var_creado}.{campo_id});
            Assert.Null({var_eliminado});
        }}
    }}
}}
'''

# ---------------------------------------------------------------------------
# PLANTILLA SIN USERID (AddAsync/UpdateAsync sin parametro userId)
# ---------------------------------------------------------------------------
TEMPLATE_SIN_USERID = '''\
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
{{
    /// <summary>
    /// Pruebas de integracion para {servicio} contra la base de datos REAL.
    /// Verifica que el flujo CRUD completo funciona correctamente end-to-end.
    ///
    /// IMPORTANTE:
    /// 1. Requiere base de datos configurada y accesible
    /// 2. Connection string configurado en DatabaseFixture
    /// 3. Stored procedures de {entidad} existentes en la BD
    /// </summary>
    public class {entidad}IntegrationTests : IClassFixture<DatabaseFixture>
    {{
        private readonly {servicio} _service;
        private readonly {repositorio} _repository;
        private readonly IMapper _mapper;
        private readonly DatabaseFixture _fixture;

        public {entidad}IntegrationTests(DatabaseFixture fixture)
        {{
            _fixture = fixture;
            _repository = new {repositorio}();
            _mapper = fixture.Mapper;

            var loggerMock = new Mock<ILogger<{servicio}>>();
            _service = new {servicio}(_repository, loggerMock.Object, _mapper);
        }}

        // =======================================================================
        // CREATE (INSERT)
        // =======================================================================

        [Fact]
        public async Task AddAsync_ConDatosValidos_InsertaEnBaseDeDatos()
        {{
            // Arrange
            var {var_nuevo} = new {form_vm}
            {{
                {campo_desc} = $"Test {{Guid.NewGuid().ToString().Substring(0, 8)}}",
                {campo_activo} = true{campos_extra_create}
            }};

            // Act
            bool resultado = await _service.AddAsync({var_nuevo});

            // Assert
            Assert.True(resultado, "La insercion deberia retornar true");

            var lista = await _service.ListAsync();
            var {var_insertado} = lista.FirstOrDefault(x => x.{campo_desc} == {var_nuevo}.{campo_desc});

            Assert.True({var_insertado} != null,
                $"No se encontro '{{{var_nuevo}.{campo_desc}}}' en la BD — el INSERT puede haber fallado (verificar nombre del SP)");

            // Cleanup
            if ({var_insertado} != null)
                await _service.RemoveAsync({var_insertado}.{campo_id});
        }}

        [Fact]
        public async Task AddAsync_ConDescripcionVacia_NoDeberiaInsertar()
        {{
            // Arrange
            var {var_invalido} = new {form_vm}
            {{
                {campo_desc} = ""
            }};

            // Act & Assert
            try
            {{
                bool resultado = await _service.AddAsync({var_invalido});
                Assert.True(true);
            }}
            catch (Exception ex)
            {{
                Assert.NotNull(ex);
            }}
        }}

        // =======================================================================
        // READ (SELECT)
        // =======================================================================

        [Fact]
        public async Task ListAsync_RetornaListaDe{entidad}sDesdeBD()
        {{
            // Act
            var resultado = await _service.ListAsync();

            // Assert
            Assert.NotNull(resultado);
            Assert.IsType<List<{list_vm}>>(resultado);
            Assert.True(resultado.Count >= 0);
        }}

        [Fact]
        public async Task FindAsync_ConIdExistente_Retorna{entidad}DesdeBD()
        {{
            // Arrange
            var {var_prueba} = new {form_vm}
            {{
                {campo_desc} = $"Find {{Guid.NewGuid().ToString().Substring(0, 8)}}",
                {campo_activo} = true{campos_extra_create}
            }};

            await _service.AddAsync({var_prueba});

            var lista = await _service.ListAsync();
            var {var_creado} = lista.FirstOrDefault(x => x.{campo_desc} == {var_prueba}.{campo_desc});
            Assert.True({var_creado} != null,
                $"Precondicion fallida: no se pudo insertar '{{{var_prueba}.{campo_desc}}}' — verificar SP de INSERT");

            // Act
            var resultado = await _service.FindAsync({var_creado}.{campo_id});

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal({var_creado}.{campo_id}, resultado.{campo_id});
            Assert.Equal({var_prueba}.{campo_desc}, resultado.{campo_desc});

            // Cleanup
            await _service.RemoveAsync({var_creado}.{campo_id});
        }}

        [Fact]
        public async Task FindAsync_ConIdInexistente_RetornaNull()
        {{
            // Arrange
            int idInexistente = 999999;

            // Act
            var resultado = await _service.FindAsync(idInexistente);

            // Assert
            Assert.Null(resultado);
        }}

        [Fact]
        public async Task DetailAsync_ConIdExistente_RetornaDetalleCompleto()
        {{
            // Arrange
            var {var_prueba} = new {form_vm}
            {{
                {campo_desc} = $"Detail {{Guid.NewGuid().ToString().Substring(0, 8)}}",
                {campo_activo} = true{campos_extra_create}
            }};

            await _service.AddAsync({var_prueba});

            var lista = await _service.ListAsync();
            var {var_creado} = lista.FirstOrDefault(x => x.{campo_desc} == {var_prueba}.{campo_desc});
            Assert.True({var_creado} != null,
                $"Precondicion fallida: no se pudo insertar '{{{var_prueba}.{campo_desc}}}' — verificar SP de INSERT");

            // Act
            var resultado = await _service.DetailAsync({var_creado}.{campo_id});

            // Assert
            Assert.True(resultado != null,
                $"DetailAsync no retorno datos para Id={{{var_creado}.{campo_id}}}");
            Assert.Equal({var_creado}.{campo_id}, resultado.{campo_id});
            Assert.Equal({var_prueba}.{campo_desc}, resultado.{campo_desc});
{assert_fechacrea}
            // Cleanup
            await _service.RemoveAsync({var_creado}.{campo_id});
        }}

        // =======================================================================
        // UPDATE
        // =======================================================================

        [Fact]
        public async Task UpdateAsync_ConDatosValidos_ActualizaEnBaseDeDatos()
        {{
            // Arrange
            var {var_inicial} = new {form_vm}
            {{
                {campo_desc} = $"Orig {{Guid.NewGuid().ToString().Substring(0, 8)}}",
                {campo_activo} = true{campos_extra_create}
            }};

            await _service.AddAsync({var_inicial});

            var lista = await _service.ListAsync();
            var {var_creado} = lista.FirstOrDefault(x => x.{campo_desc} == {var_inicial}.{campo_desc});
            Assert.True({var_creado} != null,
                $"Precondicion fallida: no se pudo insertar '{{{var_inicial}.{campo_desc}}}' — verificar SP de INSERT");

            var {var_actualizar} = await _service.FindAsync({var_creado}.{campo_id});
            {var_actualizar}.{campo_desc} = $"Upd {{Guid.NewGuid().ToString().Substring(0, 8)}}";

            // Act
            bool resultado = await _service.UpdateAsync({var_actualizar});

            // Assert
            Assert.True(resultado, "La actualizacion deberia retornar true");

            var {var_verificado} = await _service.FindAsync({var_creado}.{campo_id});
            Assert.True({var_verificado} != null,
                $"No se encontro el registro Id={{{var_creado}.{campo_id}}} tras el UPDATE");
            Assert.Equal({var_actualizar}.{campo_desc}, {var_verificado}.{campo_desc});

            // Cleanup
            await _service.RemoveAsync({var_creado}.{campo_id});
        }}
{test_activo}
        // =======================================================================
        // DELETE
        // =======================================================================

        [Fact]
        public async Task RemoveAsync_ConIdExistente_EliminaDeBaseDeDatos()
        {{
            // Arrange
            var {var_prueba} = new {form_vm}
            {{
                {campo_desc} = $"Del {{Guid.NewGuid().ToString().Substring(0, 8)}}",
                {campo_activo} = true{campos_extra_create}
            }};

            await _service.AddAsync({var_prueba});

            var lista = await _service.ListAsync();
            var {var_creado} = lista.FirstOrDefault(x => x.{campo_desc} == {var_prueba}.{campo_desc});
            Assert.True({var_creado} != null,
                $"Precondicion fallida: no se pudo insertar '{{{var_prueba}.{campo_desc}}}' — verificar SP de INSERT");

            int idEliminar = {var_creado}.{campo_id};

            // Act
            bool resultado = await _service.RemoveAsync(idEliminar);

            // Assert
            Assert.True(resultado, "La eliminacion deberia retornar true");

            var {var_eliminado} = await _service.FindAsync(idEliminar);
            Assert.Null({var_eliminado});
        }}

        [Fact]
        public async Task RemoveAsync_ConIdInexistente_RetornaFalse()
        {{
            // Arrange
            int idInexistente = 999999;

            // Act
            bool resultado = await _service.RemoveAsync(idInexistente);

            // Assert
            Assert.False(resultado, "Eliminar un ID inexistente deberia retornar false");
        }}

        // =======================================================================
        // FLUJO CRUD COMPLETO
        // =======================================================================

        [Fact]
        public async Task FlujoCRUDCompleto_CrearLeerActualizarEliminar_FuncionaCorrectamente()
        {{
            // 1. CREATE
            var {var_nuevo} = new {form_vm}
            {{
                {campo_desc} = $"CRUD {{Guid.NewGuid().ToString().Substring(0, 8)}}",
                {campo_activo} = true{campos_extra_create}
            }};

            bool insertado = await _service.AddAsync({var_nuevo});
            Assert.True(insertado, "CREATE fallo — verificar SP de INSERT");

            // 2. READ
            var lista = await _service.ListAsync();
            var {var_creado} = lista.FirstOrDefault(x => x.{campo_desc} == {var_nuevo}.{campo_desc});
            Assert.True({var_creado} != null,
                $"CREATE fallo: '{{{var_nuevo}.{campo_desc}}}' no esta en la BD — verificar SP de INSERT");

            var {var_encontrado} = await _service.FindAsync({var_creado}.{campo_id});
            Assert.True({var_encontrado} != null,
                $"READ fallo: FindAsync no encontro Id={{{var_creado}.{campo_id}}}");
            Assert.Equal({var_nuevo}.{campo_desc}, {var_encontrado}.{campo_desc});

            // 3. UPDATE
            {var_encontrado}.{campo_desc} = $"CRUD2 {{Guid.NewGuid().ToString().Substring(0, 8)}}";
            bool actualizado = await _service.UpdateAsync({var_encontrado});
            Assert.True(actualizado, "UPDATE fallo");

            var {var_verificado} = await _service.FindAsync({var_creado}.{campo_id});
            Assert.Equal({var_encontrado}.{campo_desc}, {var_verificado}.{campo_desc});

            // 4. DELETE
            bool eliminado = await _service.RemoveAsync({var_creado}.{campo_id});
            Assert.True(eliminado, "DELETE fallo");

            var {var_eliminado} = await _service.FindAsync({var_creado}.{campo_id});
            Assert.Null({var_eliminado});
        }}
    }}
}}
'''

# ---------------------------------------------------------------------------
# BLOQUE: test de cambio de estado activo (se inyecta cuando tiene_activo=True)
# Con userId
# ---------------------------------------------------------------------------
TEST_ACTIVO_CON_USERID = '''
        [Fact]
        public async Task UpdateAsync_CambiaEstadoActivo_ActualizaCorrectamente()
        {{
            // Arrange
            var {var_prueba} = new {form_vm}
            {{
                {campo_desc} = $"Estado {{Guid.NewGuid().ToString().Substring(0, 8)}}",
                {campo_activo} = true{campos_extra_create}
            }};

            await _service.AddAsync({var_prueba}, _testUserId);

            var lista = await _service.ListAsync();
            var {var_creado} = lista.FirstOrDefault(x => x.{campo_desc} == {var_prueba}.{campo_desc});
            Assert.True({var_creado} != null,
                $"Precondicion fallida: no se pudo insertar '{{{var_prueba}.{campo_desc}}}' — verificar SP de INSERT");

            var {var_actualizar} = await _service.FindAsync({var_creado}.{campo_id});
            {var_actualizar}.{campo_activo} = false;

            // Act
            bool resultado = await _service.UpdateAsync({var_actualizar}, _testUserId);

            // Assert
            Assert.True(resultado);

            var {var_verificado} = await _service.FindAsync({var_creado}.{campo_id});
            Assert.False({var_verificado}.{campo_activo});

            // Cleanup
            await _service.RemoveAsync({var_creado}.{campo_id});
        }}
'''

# ---------------------------------------------------------------------------
# BLOQUE: test de cambio de estado activo SIN userId
# ---------------------------------------------------------------------------
TEST_ACTIVO_SIN_USERID = '''
        [Fact]
        public async Task UpdateAsync_CambiaEstadoActivo_ActualizaCorrectamente()
        {{
            // Arrange
            var {var_prueba} = new {form_vm}
            {{
                {campo_desc} = $"Estado {{Guid.NewGuid().ToString().Substring(0, 8)}}",
                {campo_activo} = true{campos_extra_create}
            }};

            await _service.AddAsync({var_prueba});

            var lista = await _service.ListAsync();
            var {var_creado} = lista.FirstOrDefault(x => x.{campo_desc} == {var_prueba}.{campo_desc});
            Assert.True({var_creado} != null,
                $"Precondicion fallida: no se pudo insertar '{{{var_prueba}.{campo_desc}}}' — verificar SP de INSERT");

            var {var_actualizar} = await _service.FindAsync({var_creado}.{campo_id});
            {var_actualizar}.{campo_activo} = false;

            // Act
            bool resultado = await _service.UpdateAsync({var_actualizar});

            // Assert
            Assert.True(resultado);

            var {var_verificado} = await _service.FindAsync({var_creado}.{campo_id});
            Assert.False({var_verificado}.{campo_activo});

            // Cleanup
            await _service.RemoveAsync({var_creado}.{campo_id});
        }}
'''

# ---------------------------------------------------------------------------
# CONFIGURACION DE ENTIDADES
# Campos obligatorios:
#   entidad          nombre de la clase de prueba
#   servicio         nombre del Service
#   repositorio      nombre del Repository
#   form_vm          ViewModel para Add/Find/Update
#   list_vm          ViewModel retornado por ListAsync (dentro de List<>)
#   campo_id         campo PK
#   campo_desc       campo descripcion/nombre principal
#   campo_activo     campo bool activo (dejar "" si no existe)
#   campo_fechacrea  campo fechaCrea en detail_vm (dejar "" si no existe)
#   tiene_userid     True si AddAsync/UpdateAsync reciben (model, userId)
#   campos_extra     campos adicionales para el constructor del form_vm
# ---------------------------------------------------------------------------
ENTIDADES = [
    # ------------------------------------------------------------------
    # CATALOGOS SIMPLES
    # ------------------------------------------------------------------
    {
        "entidad":        "Vacuna",
        "servicio":       "VacunaService",
        "repositorio":    "VacunaRepository",
        "form_vm":        "VacunaFormViewModel",
        "list_vm":        "VacunaListViewModel",
        "campo_id":       "vac_Id",
        "campo_desc":     "vac_Descripcion",
        "campo_activo":   "vac_EsActivo",
        "campo_fechacrea":"vac_FechaCrea",
        "tiene_userid":   True,
        "campos_extra":   "",
    },
    {
        "entidad":        "Categoria",
        "servicio":       "CategoriaService",
        "repositorio":    "CategoriaRepository",
        "form_vm":        "CategoriaViewModel",
        "list_vm":        "CategoriaViewModel",
        "campo_id":       "cat_Id",
        "campo_desc":     "cat_Descripcion",
        "campo_activo":   "cat_EsActivoBool",
        "campo_fechacrea":"cat_FechaCrea",
        "tiene_userid":   True,
        "campos_extra":   "",
    },
    {
        "entidad":        "Procedencia",
        "servicio":       "ProcedenciaService",
        "repositorio":    "ProcedenciaRepository",
        "form_vm":        "ProcedenciaViewModel",
        "list_vm":        "ProcedenciaViewModel",
        "campo_id":       "proc_Id",
        "campo_desc":     "proc_Descripcion",
        "campo_activo":   "proc_EsActivoBool",
        "campo_fechacrea":"proc_FechaCrea",
        "tiene_userid":   True,
        "campos_extra":   "",
    },
    {
        "entidad":        "EmpleadosCargo",
        "servicio":       "EmpleadosCargoService",
        "repositorio":    "EmpleadosCargoRepository",
        "form_vm":        "EmpleadoCargoViewModel",
        "list_vm":        "EmpleadoCargoViewModel",
        "campo_id":       "cag_Id",
        "campo_desc":     "cag_Descripcion",
        "campo_activo":   "cag_EsActivo",
        "campo_fechacrea":"cag_FechaCrea",
        "tiene_userid":   True,
        "campos_extra":   ',\n                cag_Salario = 5000m',
    },
    {
        "entidad":        "Refugio",
        "servicio":       "RefugioService",
        "repositorio":    "RefugioRepository",
        "form_vm":        "RefugioFormViewModel",
        "list_vm":        "RefugioListViewModel",
        "campo_id":       "refg_Id",
        "campo_desc":     "refg_Nombre",
        "campo_activo":   "refg_EsActivo",
        "campo_fechacrea":"refg_FechaCrea",
        "tiene_userid":   True,
        "campos_extra":   (
            ',\n                refg_Ubicacion = "Test Ubicacion"'
            ',\n                refg_RTN = "08019999000000"'
            ',\n                refg_Telefono = "22001100"'
            ',\n                refg_Correo = "test@test.com"'
            ',\n                refg_InformacionAdicional = "Prueba de integracion"'
            ',\n                depto_Id = 1'
            ',\n                mpio_Id = 1'
        ),
    },
    # ------------------------------------------------------------------
    # CATALOGOS MEDICOS (sin userId)
    # ------------------------------------------------------------------
    {
        "entidad":        "Gravedad",
        "servicio":       "GravedadService",
        "repositorio":    "GravedadRepository",
        "form_vm":        "GravedadViewModel",
        "list_vm":        "GravedadViewModel",
        "campo_id":       "grav_Id",
        "campo_desc":     "grav_Descripcion",
        "campo_activo":   "grav_EsActivoBool",
        "campo_fechacrea":"grav_FechaCrea",
        "tiene_userid":   False,
        "campos_extra":   "",
    },
    {
        "entidad":        "TipoConsulta",
        "servicio":       "TipoConsultaService",
        "repositorio":    "TipoConsultaRepository",
        "form_vm":        "TipoConsultaViewModel",
        "list_vm":        "TipoConsultaViewModel",
        "campo_id":       "tipoCon_Id",
        "campo_desc":     "tipoCon_Descripcion",
        "campo_activo":   "tipoCon_EsActivoBool",
        "campo_fechacrea":"tipoCon_FechaCrea",
        "tiene_userid":   False,
        "campos_extra":   "",
    },
    {
        "entidad":        "TipoEsterilizacion",
        "servicio":       "TipoEsterilizacionService",
        "repositorio":    "TipoEsterilizacionRepository",
        "form_vm":        "TipoEsterilizacionViewModel",
        "list_vm":        "TipoEsterilizacionViewModel",
        "campo_id":       "tipoEst_Id",
        "campo_desc":     "tipoEst_Descripcion",
        "campo_activo":   "tipoEst_EsActivoBool",
        "campo_fechacrea":"tipoEst_FechaCrea",
        "tiene_userid":   False,
        "campos_extra":   "",
    },
    {
        "entidad":        "TipoMedicamento",
        "servicio":       "TipoMedicamentoService",
        "repositorio":    "TipoMedicamentoRepository",
        "form_vm":        "TipoMedicamentoViewModel",
        "list_vm":        "TipoMedicamentoViewModel",
        "campo_id":       "tipoMed_Id",
        "campo_desc":     "tipoMed_Descripcion",
        "campo_activo":   "tipoMed_EsActivoBool",
        "campo_fechacrea":"tipoMed_FechaCrea",
        "tiene_userid":   False,
        "campos_extra":   "",
    },
    {
        "entidad":        "TipoParasito",
        "servicio":       "TipoParasitoService",
        "repositorio":    "TipoParasitoRepository",
        "form_vm":        "TipoParasitoViewModel",
        "list_vm":        "TipoParasitoViewModel",
        "campo_id":       "tipoPar_Id",
        "campo_desc":     "tipoPar_Descripcion",
        "campo_activo":   "tipoPar_EsActivoBool",
        "campo_fechacrea":"tipoPar_FechaCrea",
        "tiene_userid":   False,
        "campos_extra":   "",
    },
    # ------------------------------------------------------------------
    # ENTIDADES PRINCIPALES (con userId, sin EsActivo bool dedicado)
    # ------------------------------------------------------------------
    {
        "entidad":        "Item",
        "servicio":       "ItemService",
        "repositorio":    "ItemRepository",
        "form_vm":        "ItemViewModel",
        "list_vm":        "ItemViewModel",
        "campo_id":       "itm_Id",
        "campo_desc":     "itm_Descripcion",
        "campo_activo":   "",
        "campo_fechacrea":"itm_FechaCrea",
        "tiene_userid":   True,
        "campos_extra":   "",
    },
    {
        "entidad":        "Evento",
        "servicio":       "EventoService",
        "repositorio":    "EventoRepository",
        "form_vm":        "EventoViewModel",
        "list_vm":        "EventoViewModel",
        "campo_id":       "eve_Id",
        "campo_desc":     "eve_Descripcion",
        "campo_activo":   "",
        "campo_fechacrea":"",
        "tiene_userid":   True,
        "campos_extra":   '',
    },
    # Departamento excluido: constructor requiere 2 repositorios (LocalidadRepository + MunicipioRepository)
    # Municipio excluido: no implementa ListAsync() ni DetailAsync() estandar
    {
        "entidad":        "Solicitud",
        "servicio":       "SolicitudService",
        "repositorio":    "SolicitudRepository",
        "form_vm":        "SolicitudFormViewModel",
        "list_vm":        "SolicitudListViewModel",
        "campo_id":       "sol_Id",
        "campo_desc":     "sol_Nombres",
        "campo_activo":   "",
        "campo_fechacrea":"sol_FechaCrea",
        "tiene_userid":   True,
        "campos_extra":   (
            ',\n                sol_Apellidos = "ApellidoTest"'
            ',\n                sol_Identidad = "0101199900001"'
            ',\n                sol_Telefono = "99001100"'
            ',\n                sol_Correo = "sol@test.com"'
        ),
    },
    {
        "entidad":        "Voluntario",
        "servicio":       "VoluntarioService",
        "repositorio":    "VoluntarioRepository",
        "form_vm":        "VoluntarioFormViewModel",
        "list_vm":        "VoluntarioListViewModel",
        "campo_id":       "vol_Id",
        "campo_desc":     "vol_Nombres",
        "campo_activo":   "",
        "campo_fechacrea":"vol_FechaCrea",
        "tiene_userid":   True,
        "campos_extra":   "",
    },
    {
        "entidad":        "RecepcionMercancia",
        "servicio":       "RecepcionMercanciaService",
        "repositorio":    "RecepcionMercanciaRepository",
        "form_vm":        "RecepcionMercanciaFormViewModel",
        "list_vm":        "RecepcionMercanciaListViewModel",
        "campo_id":       "recep_Id",
        "campo_desc":     "recep_Descripcion",
        "campo_activo":   "",
        "campo_fechacrea":"",
        "tiene_userid":   False,
        "campos_extra":   "",
    },
    {
        "entidad":        "Receta",
        "servicio":       "RecetaService",
        "repositorio":    "RecetaRepository",
        "form_vm":        "RecetaFormViewModel",
        "list_vm":        "RecetaListViewModel",
        "campo_id":       "receta_Id",
        "campo_desc":     "receta_Medicamento",
        "campo_activo":   "",
        "campo_fechacrea":"receta_FechaCrea",
        "tiene_userid":   False,
        "campos_extra":   "",
    },
    {
        "entidad":        "ViaAdministracion",
        "servicio":       "ViaAdministracionService",
        "repositorio":    "ViaAdministracionRepository",
        "form_vm":        "ViaAdministracionViewModel",
        "list_vm":        "ViaAdministracionViewModel",
        "campo_id":       "viaAdmin_Id",
        "campo_desc":     "viaAdmin_Descripcion",
        "campo_activo":   "",
        "campo_fechacrea":"",
        "tiene_userid":   False,
        "campos_extra":   "",
    },
]


def _nombres_variables(entidad):
    e = entidad[0].lower() + entidad[1:]
    return {
        "var_nuevo":     f"nueva{entidad}"    if entidad.endswith(('a','A')) else f"nuevo{entidad}",
        "var_insertado": f"{e}Insertado",
        "var_invalido":  f"{e}Invalido",
        "var_prueba":    f"{e}Prueba",
        "var_creado":    f"{e}Creado",
        "var_inicial":   f"{e}Inicial",
        "var_actualizar":f"{e}Actualizar",
        "var_verificado":f"{e}Verificado",
        "var_encontrado":f"{e}Encontrado",
        "var_eliminado": f"{e}Eliminado",
    }


def _assert_fechacrea(campo_fechacrea, var_resultado="resultado"):
    if not campo_fechacrea:
        return ""
    return (
        f"            Assert.NotEqual(default(DateTime), resultado.{campo_fechacrea});\n"
    )


def _test_activo(cfg, vars_):
    if not cfg["campo_activo"]:
        return ""
    bloque = TEST_ACTIVO_CON_USERID if cfg["tiene_userid"] else TEST_ACTIVO_SIN_USERID
    return bloque.format(
        form_vm             = cfg["form_vm"],
        campo_id            = cfg["campo_id"],
        campo_desc          = cfg["campo_desc"],
        campo_activo        = cfg["campo_activo"],
        campos_extra_create = cfg["campos_extra"],
        **vars_,
    )


def _activo_placeholder(cfg):
    """Devuelve la linea del campo activo en el constructor, o una descripcion extra."""
    if cfg["campo_activo"]:
        return cfg["campo_activo"]
    # Si no hay campo activo, usamos solo el campo desc (ya esta en la plantilla)
    # La linea '{campo_activo} = true' quedaria mal; ponemos un comentario vacio
    return "// sin_campo_activo"


def generar(cfg):
    entidad = cfg["entidad"]
    vars_ = _nombres_variables(entidad)

    # Si no hay campo activo, hay que ajustar la plantilla para no poner
    # '{campo_activo} = true' en el constructor
    campo_activo_render = cfg["campo_activo"] if cfg["campo_activo"] else "// (sin campo EsActivo)"

    extra_create = cfg["campos_extra"]

    # Si campo_activo esta vacio, eliminamos esa linea del bloque de inicializacion
    if not cfg["campo_activo"]:
        # Quitamos la linea "{campo_activo} = true," del template
        # Lo hacemos reemplazando la linea post-proceso
        activo_linea = "                // (sin campo EsActivo) = true,"
        activo_inline = "// (sin campo EsActivo)"
    else:
        activo_linea = f"                {cfg['campo_activo']} = true,"
        activo_inline = cfg["campo_activo"]

    plantilla = TEMPLATE_CON_USERID if cfg["tiene_userid"] else TEMPLATE_SIN_USERID

    contenido = plantilla.format(
        entidad              = entidad,
        servicio             = cfg["servicio"],
        repositorio          = cfg["repositorio"],
        form_vm              = cfg["form_vm"],
        list_vm              = cfg["list_vm"],
        campo_id             = cfg["campo_id"],
        campo_desc           = cfg["campo_desc"],
        campo_activo         = campo_activo_render,
        campos_extra_create  = extra_create,
        assert_fechacrea     = _assert_fechacrea(cfg["campo_fechacrea"]),
        test_activo          = _test_activo(cfg, vars_),
        **vars_,
    )

    # Post-proceso: si no hay campo activo, limpiamos la linea sucia
    if not cfg["campo_activo"]:
        contenido = contenido.replace(
            "                // (sin campo EsActivo) = true,",
            ""
        )
        contenido = contenido.replace("// (sin campo EsActivo) = true", "")

    os.makedirs(TESTS_DIR, exist_ok=True)
    archivo = os.path.join(TESTS_DIR, f"{entidad}IntegrationTests.cs")

    with open(archivo, "w", encoding="utf-8") as f:
        f.write(contenido)

    userid_str = "con userId" if cfg["tiene_userid"] else "sin userId"
    activo_str = f"activo={cfg['campo_activo']}" if cfg["campo_activo"] else "sin EsActivo"
    print(f"  OK  {entidad}IntegrationTests.cs  ({userid_str}, {activo_str})")


def main():
    print(f"Directorio destino: {TESTS_DIR}")
    print(f"Entidades a generar: {len(ENTIDADES)}")
    print()

    for cfg in ENTIDADES:
        generar(cfg)

    print()
    print("Listo. Notas:")
    print("  - DatabaseFixture esta en RazaIntegrationTests.cs, no se duplica")
    print("  - Departamento usa LocalidadRepository (verificar nombre en BD)")
    print("  - Municipio: depto_Id=1 debe existir en BD")
    print("  - Refugio: depto_Id=1 y mpio_Id=1 deben existir en BD")
    print("  - Solicitud/Voluntario: datos de identidad son ficticios")
    print("  - Entidades sin EsActivo no incluyen test UpdateAsync_CambiaEstadoActivo")
    print("  - Entidades sin FechaCrea no incluyen Assert de auditoria en DetailAsync")
    print()
    print("Servicios NO incluidos (sin CRUD estandar):")
    print("  - RazaService           (ya tiene RazaIntegrationTests.cs)")
    print("  - HomeService           (sin CRUD)")
    print("  - UsuarioService        (solo autenticacion)")
    print("  - ComportamientosService(solo Dropdown)")
    print("  - InventariosDetalleService (metodos comentados)")
    print("  - PersonaService        (metodos comentados)")
    print("  - MascotaService        (procesamiento de imagen, requiere config especial)")
    print("  - AdopcionService       (sin campo descripcion unico)")
    print("  - HistorialMedicoService(sin campo descripcion unico)")
    print("  - CitaMedicaService     (sin campo descripcion unico)")
    print("  - TratamientoService    (sin campo descripcion unico)")
    print("  - RecepcionDetalleService(sin campo descripcion unico)")
    print("  - DepartamentoService   (constructor con 2 repositorios, requiere ajuste manual)")
    print("  - MunicipioService      (no tiene ListAsync/DetailAsync estandar)")


if __name__ == "__main__":
    main()
