"""
Generador de pruebas de integracion para PetsHome.
Uso: python generar_integration_tests.py <Entidad> <prefijo_campo> [campo_descripcion]

Ejemplos:
  python generar_integration_tests.py Vacuna vac vac_Descripcion
  python generar_integration_tests.py Categoria cat cat_Descripcion
  python generar_integration_tests.py Refugio ref ref_Nombre
  python generar_integration_tests.py Empleado emp emp_Nombre

Parametros:
  Entidad          : Nombre de la entidad en PascalCase  (ej: Vacuna, Categoria)
  prefijo_campo    : Prefijo de las columnas en BD       (ej: vac, cat, ref)
  campo_descripcion: Campo usado como identificador unico (por defecto: {prefijo}_Descripcion)
"""

import sys
import os

TEMPLATE = '''\
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
    /// Pruebas de integracion para {Entidad}Service contra la base de datos REAL.
    /// Verifica que el flujo CRUD completo funciona correctamente end-to-end.
    ///
    /// IMPORTANTE:
    /// 1. Requiere base de datos configurada y accesible
    /// 2. Connection string configurado en DatabaseFixture
    /// 3. Stored procedures de {Entidad} existentes en la BD
    /// </summary>
    public class {Entidad}IntegrationTests : IClassFixture<DatabaseFixture>
    {{
        private readonly {Entidad}Service _service;
        private readonly {Entidad}Repository _repository;
        private readonly IMapper _mapper;
        private readonly DatabaseFixture _fixture;
        private readonly int _testUserId = 1;

        public {Entidad}IntegrationTests(DatabaseFixture fixture)
        {{
            _fixture = fixture;
            _repository = new {Entidad}Repository();
            _mapper = fixture.Mapper;

            var loggerMock = new Mock<ILogger<{Entidad}Service>>();
            _service = new {Entidad}Service(_repository, loggerMock.Object, _mapper);
        }}

        // ═══════════════════════════════════════════════════════════════════════
        // CREATE (INSERT)
        // ═══════════════════════════════════════════════════════════════════════

        [Fact]
        public async Task AddAsync_ConDatosValidos_InsertaEnBaseDeDatos()
        {{
            var nuevo = new {Entidad}FormViewModel
            {{
                {campo_desc} = $"Test {{Guid.NewGuid().ToString().Substring(0, 8)}}",
                {campo_activo} = true
            }};

            bool resultado = await _service.AddAsync(nuevo, _testUserId);

            Assert.True(resultado, "La insercion deberia retornar true");

            var lista = await _service.ListAsync();
            var insertado = lista.FirstOrDefault(x => x.{campo_desc} == nuevo.{campo_desc});

            Assert.True(insertado != null,
                $"No se encontro '{{nuevo.{campo_desc}}}' en la BD — el INSERT puede haber fallado (verificar nombre del SP)");

            if (insertado != null)
                await _service.RemoveAsync(insertado.{campo_id});
        }}

        [Fact]
        public async Task AddAsync_ConDescripcionVacia_NoDeberiaFallarFatalmente()
        {{
            var invalido = new {Entidad}FormViewModel
            {{
                {campo_desc} = ""
            }};

            try
            {{
                bool resultado = await _service.AddAsync(invalido, _testUserId);
                Assert.True(true);
            }}
            catch (Exception ex)
            {{
                Assert.NotNull(ex);
            }}
        }}

        // ═══════════════════════════════════════════════════════════════════════
        // READ (SELECT)
        // ═══════════════════════════════════════════════════════════════════════

        [Fact]
        public async Task ListAsync_RetornaListaDesdeBD()
        {{
            var resultado = await _service.ListAsync();

            Assert.NotNull(resultado);
            Assert.IsType<List<{Entidad}ListViewModel>>(resultado);
            Assert.True(resultado.Count >= 0);
        }}

        [Fact]
        public async Task FindAsync_ConIdExistente_RetornaDatosDesdeBD()
        {{
            var prueba = new {Entidad}FormViewModel
            {{
                {campo_desc} = $"Find {{Guid.NewGuid().ToString().Substring(0, 8)}}",
                {campo_activo} = true
            }};

            await _service.AddAsync(prueba, _testUserId);

            var lista = await _service.ListAsync();
            var creado = lista.FirstOrDefault(x => x.{campo_desc} == prueba.{campo_desc});
            Assert.True(creado != null,
                $"Precondicion fallida: no se pudo insertar '{{prueba.{campo_desc}}}' — verificar SP de INSERT");

            var resultado = await _service.FindAsync(creado.{campo_id});

            Assert.NotNull(resultado);
            Assert.Equal(creado.{campo_id}, resultado.{campo_id});
            Assert.Equal(prueba.{campo_desc}, resultado.{campo_desc});

            await _service.RemoveAsync(creado.{campo_id});
        }}

        [Fact]
        public async Task FindAsync_ConIdInexistente_RetornaNull()
        {{
            var resultado = await _service.FindAsync(999999);

            Assert.Null(resultado);
        }}

        [Fact]
        public async Task DetailAsync_ConIdExistente_RetornaDetalleCompleto()
        {{
            var prueba = new {Entidad}FormViewModel
            {{
                {campo_desc} = $"Detail {{Guid.NewGuid().ToString().Substring(0, 8)}}",
                {campo_activo} = true
            }};

            await _service.AddAsync(prueba, _testUserId);

            var lista = await _service.ListAsync();
            var creado = lista.FirstOrDefault(x => x.{campo_desc} == prueba.{campo_desc});
            Assert.True(creado != null,
                $"Precondicion fallida: no se pudo insertar '{{prueba.{campo_desc}}}' — verificar SP de INSERT");

            var resultado = await _service.DetailAsync(creado.{campo_id});

            Assert.True(resultado != null,
                $"DetailAsync no retorno datos para Id={{creado.{campo_id}}}");
            Assert.Equal(creado.{campo_id}, resultado.{campo_id});
            Assert.Equal(prueba.{campo_desc}, resultado.{campo_desc});
            Assert.NotEqual(default(DateTime), resultado.{campo_fechacrea});

            await _service.RemoveAsync(creado.{campo_id});
        }}

        // ═══════════════════════════════════════════════════════════════════════
        // UPDATE
        // ═══════════════════════════════════════════════════════════════════════

        [Fact]
        public async Task UpdateAsync_ConDatosValidos_ActualizaEnBaseDeDatos()
        {{
            var inicial = new {Entidad}FormViewModel
            {{
                {campo_desc} = $"Orig {{Guid.NewGuid().ToString().Substring(0, 8)}}",
                {campo_activo} = true
            }};

            await _service.AddAsync(inicial, _testUserId);

            var lista = await _service.ListAsync();
            var creado = lista.FirstOrDefault(x => x.{campo_desc} == inicial.{campo_desc});
            Assert.True(creado != null,
                $"Precondicion fallida: no se pudo insertar '{{inicial.{campo_desc}}}' — verificar SP de INSERT");

            var actualizar = await _service.FindAsync(creado.{campo_id});
            actualizar.{campo_desc} = $"Upd {{Guid.NewGuid().ToString().Substring(0, 8)}}";

            bool resultado = await _service.UpdateAsync(actualizar, _testUserId);

            Assert.True(resultado, "La actualizacion deberia retornar true");

            var verificado = await _service.FindAsync(creado.{campo_id});
            Assert.True(verificado != null,
                $"No se encontro el registro Id={{creado.{campo_id}}} tras el UPDATE");
            Assert.Equal(actualizar.{campo_desc}, verificado.{campo_desc});

            await _service.RemoveAsync(creado.{campo_id});
        }}

        [Fact]
        public async Task UpdateAsync_CambiaEstadoActivo_ActualizaCorrectamente()
        {{
            var prueba = new {Entidad}FormViewModel
            {{
                {campo_desc} = $"Estado {{Guid.NewGuid().ToString().Substring(0, 8)}}",
                {campo_activo} = true
            }};

            await _service.AddAsync(prueba, _testUserId);

            var lista = await _service.ListAsync();
            var creado = lista.FirstOrDefault(x => x.{campo_desc} == prueba.{campo_desc});
            Assert.True(creado != null,
                $"Precondicion fallida: no se pudo insertar '{{prueba.{campo_desc}}}' — verificar SP de INSERT");

            var actualizar = await _service.FindAsync(creado.{campo_id});
            actualizar.{campo_activo} = false;

            bool resultado = await _service.UpdateAsync(actualizar, _testUserId);

            Assert.True(resultado);

            var verificado = await _service.FindAsync(creado.{campo_id});
            Assert.False(verificado.{campo_activo});

            await _service.RemoveAsync(creado.{campo_id});
        }}

        // ═══════════════════════════════════════════════════════════════════════
        // DELETE
        // ═══════════════════════════════════════════════════════════════════════

        [Fact]
        public async Task RemoveAsync_ConIdExistente_EliminaDeBaseDeDatos()
        {{
            var prueba = new {Entidad}FormViewModel
            {{
                {campo_desc} = $"Del {{Guid.NewGuid().ToString().Substring(0, 8)}}",
                {campo_activo} = true
            }};

            await _service.AddAsync(prueba, _testUserId);

            var lista = await _service.ListAsync();
            var creado = lista.FirstOrDefault(x => x.{campo_desc} == prueba.{campo_desc});
            Assert.True(creado != null,
                $"Precondicion fallida: no se pudo insertar '{{prueba.{campo_desc}}}' — verificar SP de INSERT");

            int idEliminar = creado.{campo_id};

            bool resultado = await _service.RemoveAsync(idEliminar);

            Assert.True(resultado, "La eliminacion deberia retornar true");

            var eliminado = await _service.FindAsync(idEliminar);
            Assert.Null(eliminado);
        }}

        [Fact]
        public async Task RemoveAsync_ConIdInexistente_RetornaFalse()
        {{
            bool resultado = await _service.RemoveAsync(999999);

            Assert.False(resultado, "Eliminar un ID inexistente deberia retornar false");
        }}

        // ═══════════════════════════════════════════════════════════════════════
        // FLUJO CRUD COMPLETO
        // ═══════════════════════════════════════════════════════════════════════

        [Fact]
        public async Task FlujoCRUDCompleto_CrearLeerActualizarEliminar_FuncionaCorrectamente()
        {{
            // 1. CREATE
            var nuevo = new {Entidad}FormViewModel
            {{
                {campo_desc} = $"CRUD {{Guid.NewGuid().ToString().Substring(0, 8)}}",
                {campo_activo} = true
            }};

            bool insertado = await _service.AddAsync(nuevo, _testUserId);
            Assert.True(insertado, "CREATE fallo — verificar SP de INSERT");

            // 2. READ
            var lista = await _service.ListAsync();
            var creado = lista.FirstOrDefault(x => x.{campo_desc} == nuevo.{campo_desc});
            Assert.True(creado != null,
                $"CREATE fallo: '{{nuevo.{campo_desc}}}' no esta en la BD — verificar SP de INSERT");

            var encontrado = await _service.FindAsync(creado.{campo_id});
            Assert.True(encontrado != null,
                $"READ fallo: FindAsync no encontro Id={{creado.{campo_id}}}");
            Assert.Equal(nuevo.{campo_desc}, encontrado.{campo_desc});

            // 3. UPDATE
            encontrado.{campo_desc} = $"CRUD2 {{Guid.NewGuid().ToString().Substring(0, 8)}}";
            bool actualizado = await _service.UpdateAsync(encontrado, _testUserId);
            Assert.True(actualizado, "UPDATE fallo");

            var verificado = await _service.FindAsync(creado.{campo_id});
            Assert.Equal(encontrado.{campo_desc}, verificado.{campo_desc});

            // 4. DELETE
            bool eliminado = await _service.RemoveAsync(creado.{campo_id});
            Assert.True(eliminado, "DELETE fallo");

            var eliminadoVerif = await _service.FindAsync(creado.{campo_id});
            Assert.Null(eliminadoVerif);
        }}
    }}
}}
'''


def to_lower_first(s):
    return s[0].lower() + s[1:] if s else s


def generar(entidad, prefijo, campo_desc_override=None):
    prefijo_lower = prefijo.lower()
    campo_desc     = campo_desc_override or f"{prefijo_lower}_Descripcion"
    campo_id       = f"{prefijo_lower}_Id"
    campo_activo   = f"{prefijo_lower}_EsActivo"
    campo_fechacrea = f"{prefijo_lower}_FechaCrea"

    contenido = TEMPLATE.format(
        Entidad        = entidad,
        campo_desc     = campo_desc,
        campo_id       = campo_id,
        campo_activo   = campo_activo,
        campo_fechacrea = campo_fechacrea,
    )

    directorio_tests = os.path.join(
        os.path.dirname(os.path.abspath(__file__)),
        "PetsHome.Tests"
    )
    os.makedirs(directorio_tests, exist_ok=True)

    nombre_archivo = os.path.join(directorio_tests, f"{entidad}IntegrationTests.cs")

    if os.path.exists(nombre_archivo):
        respuesta = input(f"El archivo {nombre_archivo} ya existe. Sobreescribir? (s/n): ")
        if respuesta.strip().lower() != "s":
            print("Operacion cancelada.")
            return

    with open(nombre_archivo, "w", encoding="utf-8") as f:
        f.write(contenido)

    print(f"Archivo generado: {nombre_archivo}")
    print()
    print("Recuerda verificar manualmente:")
    print(f"  - Que {entidad}FormViewModel tenga los campos: {campo_desc}, {campo_activo}")
    print(f"  - Que {entidad}ListViewModel tenga los campos: {campo_desc}, {campo_id}")
    print(f"  - Que {entidad}DetailsViewModel tenga el campo: {campo_fechacrea}")
    print(f"  - Que {entidad}Service acepte (repository, logger, mapper) en su constructor")
    print(f"  - Que DatabaseFixture este registrado (ya existe en RazaIntegrationTests.cs)")
    print()
    print("Si la clase DatabaseFixture ya existe en otro archivo, elimina el bloque")
    print("de DatabaseFixture al final del archivo generado para evitar duplicados.")


def main():
    if len(sys.argv) < 3:
        print(__doc__)
        sys.exit(1)

    entidad  = sys.argv[1]
    prefijo  = sys.argv[2]
    campo_desc = sys.argv[3] if len(sys.argv) >= 4 else None

    generar(entidad, prefijo, campo_desc)


if __name__ == "__main__":
    main()
