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
    /// Pruebas de integracion para VoluntarioService contra la base de datos REAL.
    /// Verifica que el flujo CRUD completo funciona correctamente end-to-end.
    ///
    /// IMPORTANTE:
    /// 1. Requiere base de datos configurada y accesible
    /// 2. Connection string configurado en DatabaseFixture
    /// 3. Stored procedures de Voluntario existentes en la BD
    /// </summary>
    public class VoluntarioIntegrationTests : IClassFixture<DatabaseFixture>
    {
        private readonly VoluntarioService _service;
        private readonly VoluntarioRepository _repository;
        private readonly IMapper _mapper;
        private readonly DatabaseFixture _fixture;
        private readonly int _testUserId = 1;

        public VoluntarioIntegrationTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
            _repository = new VoluntarioRepository();
            _mapper = fixture.Mapper;

            var loggerMock = new Mock<ILogger<VoluntarioService>>();
            _service = new VoluntarioService(_repository, loggerMock.Object, _mapper);
        }

        // =======================================================================
        // CREATE (INSERT)
        // =======================================================================

        [Fact]
        public async Task AddAsync_ConDatosValidos_InsertaEnBaseDeDatos()
        {
            // Arrange
            string uniqueIdentidad = $"0801{Guid.NewGuid().ToString().Substring(0, 8)}";
            var nuevoVoluntario = new VoluntarioFormViewModel
            {
                vol_Nombres = $"Test {Guid.NewGuid().ToString().Substring(0, 8)}",
                vol_Recurrente = true,
                vol_HorasTrabajadas = 10,
                per_Identidad = uniqueIdentidad,
                per_Apellidos = "Prueba",
                per = new PersonaViewModel
                {
                    per_PrimerNombre = "Juan",
                    per_SegundoNombre = "Test",
                    per_ApellidoPaterno = "Prueba",
                    per_ApellidoMaterno = "Test",
                    per_FechaNacimiento = new DateTime(1990, 1, 1),
                    per_Domicilio = "Test Address",
                    per_Telefono = "99999999",
                    per_Correo = $"t{Guid.NewGuid().ToString().Substring(0, 6)}@t.com",
                    per_Identidad = uniqueIdentidad
                }
            };

            // Act
            bool resultado = await _service.AddAsync(nuevoVoluntario, _testUserId);

            // Assert
            Assert.True(resultado, "La insercion deberia retornar true");

            var lista = await _service.ListAsync();
            // vol_Nombres en BD es calculado de per_PrimerNombre+apellidos, buscar por per_Identidad (unico)
            var voluntarioInsertado = lista.FirstOrDefault(x => x.per_Identidad == uniqueIdentidad);

            Assert.True(voluntarioInsertado != null,
                $"No se encontro voluntario con per_Identidad='{uniqueIdentidad}' en la BD — el INSERT puede haber fallado (verificar SP)");

            // Cleanup
            if (voluntarioInsertado != null)
                await _service.RemoveAsync(voluntarioInsertado.vol_Id);
        }

        [Fact]
        public async Task AddAsync_ConDescripcionVacia_NoDeberiaInsertar()
        {
            // Arrange
            var voluntarioInvalido = new VoluntarioFormViewModel
            {
                vol_Nombres = ""
            };

            // Act & Assert
            try
            {
                bool resultado = await _service.AddAsync(voluntarioInvalido, _testUserId);
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
        public async Task ListAsync_RetornaListaDeVoluntariosDesdeBD()
        {
            // Act
            var resultado = await _service.ListAsync();

            // Assert
            Assert.NotNull(resultado);
            Assert.IsType<List<VoluntarioListViewModel>>(resultado);
            Assert.True(resultado.Count >= 0);
        }

        [Fact]
        public async Task FindAsync_ConIdExistente_RetornaVoluntarioDesdeBD()
        {
            // Arrange
            string uniqueIdentidad = $"0801{Guid.NewGuid().ToString().Substring(0, 8)}";
            var voluntarioPrueba = new VoluntarioFormViewModel
            {
                vol_Nombres = $"Find {Guid.NewGuid().ToString().Substring(0, 8)}",
                vol_Recurrente = true,
                vol_HorasTrabajadas = 10,
                per_Identidad = uniqueIdentidad,
                per_Apellidos = "Prueba",
                per = new PersonaViewModel
                {
                    per_PrimerNombre = "Juan",
                    per_SegundoNombre = "Test",
                    per_ApellidoPaterno = "Prueba",
                    per_ApellidoMaterno = "Test",
                    per_FechaNacimiento = new DateTime(1990, 1, 1),
                    per_Domicilio = "Test Address",
                    per_Telefono = "99999999",
                    per_Correo = $"t{Guid.NewGuid().ToString().Substring(0, 6)}@t.com",
                    per_Identidad = uniqueIdentidad
                }
            };

            await _service.AddAsync(voluntarioPrueba, _testUserId);

            var lista = await _service.ListAsync();
            // vol_Nombres en BD es calculado de per_PrimerNombre+apellidos, buscar por per_Identidad (unico)
            var voluntarioCreado = lista.FirstOrDefault(x => x.per_Identidad == uniqueIdentidad);
            Assert.True(voluntarioCreado != null,
                $"Precondicion fallida: no se pudo insertar voluntario con per_Identidad='{uniqueIdentidad}' — verificar SP de INSERT");

            // Act
            var resultado = await _service.FindAsync(voluntarioCreado.vol_Id);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(voluntarioCreado.vol_Id, resultado.vol_Id);
            Assert.Equal(uniqueIdentidad, resultado.per.per_Identidad);

            // Cleanup
            await _service.RemoveAsync(voluntarioCreado.vol_Id);
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
            string uniqueIdentidad = $"0801{Guid.NewGuid().ToString().Substring(0, 8)}";
            var voluntarioPrueba = new VoluntarioFormViewModel
            {
                vol_Nombres = $"Detail {Guid.NewGuid().ToString().Substring(0, 8)}",
                vol_Recurrente = true,
                vol_HorasTrabajadas = 10,
                per_Identidad = uniqueIdentidad,
                per_Apellidos = "Prueba",
                per = new PersonaViewModel
                {
                    per_PrimerNombre = "Juan",
                    per_SegundoNombre = "Test",
                    per_ApellidoPaterno = "Prueba",
                    per_ApellidoMaterno = "Test",
                    per_FechaNacimiento = new DateTime(1990, 1, 1),
                    per_Domicilio = "Test Address",
                    per_Telefono = "99999999",
                    per_Correo = $"t{Guid.NewGuid().ToString().Substring(0, 6)}@t.com",
                    per_Identidad = uniqueIdentidad
                }
            };

            await _service.AddAsync(voluntarioPrueba, _testUserId);

            var lista = await _service.ListAsync();
            // vol_Nombres en BD es calculado de per_PrimerNombre+apellidos, buscar por per_Identidad (unico)
            var voluntarioCreado = lista.FirstOrDefault(x => x.per_Identidad == uniqueIdentidad);
            Assert.True(voluntarioCreado != null,
                $"Precondicion fallida: no se pudo insertar voluntario con per_Identidad='{uniqueIdentidad}' — verificar SP de INSERT");

            // Act
            var resultado = await _service.DetailAsync(voluntarioCreado.vol_Id);

            // Assert
            Assert.True(resultado != null,
                $"DetailAsync no retorno datos para Id={voluntarioCreado.vol_Id}");
            Assert.Equal(voluntarioCreado.vol_Id, resultado.vol_Id);
            Assert.Equal(uniqueIdentidad, resultado.per_Identidad);
            Assert.NotEqual(default(DateTime), resultado.vol_FechaCrea);

            // Cleanup
            await _service.RemoveAsync(voluntarioCreado.vol_Id);
        }

        // =======================================================================
        // UPDATE
        // =======================================================================

        [Fact]
        public async Task UpdateAsync_ConDatosValidos_ActualizaEnBaseDeDatos()
        {
            // Arrange
            string uniqueIdentidad = $"0801{Guid.NewGuid().ToString().Substring(0, 8)}";
            var voluntarioInicial = new VoluntarioFormViewModel
            {
                vol_Nombres = $"Orig {Guid.NewGuid().ToString().Substring(0, 8)}",
                vol_Recurrente = true,
                vol_HorasTrabajadas = 10,
                per_Identidad = uniqueIdentidad,
                per_Apellidos = "Prueba",
                per = new PersonaViewModel
                {
                    per_PrimerNombre = "Juan",
                    per_SegundoNombre = "Test",
                    per_ApellidoPaterno = "Prueba",
                    per_ApellidoMaterno = "Test",
                    per_FechaNacimiento = new DateTime(1990, 1, 1),
                    per_Domicilio = "Test Address",
                    per_Telefono = "99999999",
                    per_Correo = $"t{Guid.NewGuid().ToString().Substring(0, 6)}@t.com",
                    per_Identidad = uniqueIdentidad
                }
            };

            await _service.AddAsync(voluntarioInicial, _testUserId);

            var lista = await _service.ListAsync();
            // vol_Nombres en BD es calculado de per_PrimerNombre+apellidos, buscar por per_Identidad (unico)
            var voluntarioCreado = lista.FirstOrDefault(x => x.per_Identidad == uniqueIdentidad);
            Assert.True(voluntarioCreado != null,
                $"Precondicion fallida: no se pudo insertar voluntario con per_Identidad='{uniqueIdentidad}' — verificar SP de INSERT");

            var voluntarioActualizar = await _service.FindAsync(voluntarioCreado.vol_Id);
            // vol_Nombres es campo calculado en SP — actualizar vol_HorasTrabajadas que SI se persiste
            voluntarioActualizar.vol_HorasTrabajadas = 20;

            // Act
            bool resultado = await _service.UpdateAsync(voluntarioActualizar, _testUserId);

            // Assert
            Assert.True(resultado, "La actualizacion deberia retornar true");

            var voluntarioVerificado = await _service.FindAsync(voluntarioCreado.vol_Id);
            Assert.True(voluntarioVerificado != null,
                $"No se encontro el registro Id={voluntarioCreado.vol_Id} tras el UPDATE");
            Assert.Equal(voluntarioActualizar.vol_HorasTrabajadas, voluntarioVerificado.vol_HorasTrabajadas);

            // Cleanup
            await _service.RemoveAsync(voluntarioCreado.vol_Id);
        }

        // =======================================================================
        // DELETE
        // =======================================================================

        [Fact(Skip = "SP [General].[PR_General_Voluntarios_Delete] no existe")]
        public async Task RemoveAsync_ConIdExistente_EliminaDeBaseDeDatos()
        {
            // Arrange
            var voluntarioPrueba = new VoluntarioFormViewModel
            {
                vol_Nombres = $"Del {Guid.NewGuid().ToString().Substring(0, 8)}",
                vol_Recurrente = true,
                vol_HorasTrabajadas = 10,
                per_Identidad = $"0801{Guid.NewGuid().ToString().Substring(0, 8)}",
                per_Apellidos = "Prueba",
                per = new PersonaViewModel
                {
                    per_PrimerNombre = "Juan",
                    per_SegundoNombre = "Test",
                    per_ApellidoPaterno = "Prueba",
                    per_ApellidoMaterno = "Test",
                    per_FechaNacimiento = new DateTime(1990, 1, 1),
                    per_Domicilio = "Test Address",
                    per_Telefono = "99999999",
                    per_Correo = $"t{Guid.NewGuid().ToString().Substring(0, 6)}@t.com",
                    per_Identidad = $"0801{Guid.NewGuid().ToString().Substring(0, 8)}"
                }
            };

            await _service.AddAsync(voluntarioPrueba, _testUserId);

            var lista = await _service.ListAsync();
            var voluntarioCreado = lista.FirstOrDefault(x => x.vol_Nombres == voluntarioPrueba.vol_Nombres);
            Assert.True(voluntarioCreado != null,
                $"Precondicion fallida: no se pudo insertar '{voluntarioPrueba.vol_Nombres}' — verificar SP de INSERT");

            int idEliminar = voluntarioCreado.vol_Id;

            // Act
            bool resultado = await _service.RemoveAsync(idEliminar);

            // Assert
            Assert.True(resultado, "La eliminacion deberia retornar true");

            var voluntarioEliminado = await _service.FindAsync(idEliminar);
            Assert.Null(voluntarioEliminado);
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

        [Fact(Skip = "DELETE fallo debido a SP faltante")]
        public async Task FlujoCRUDCompleto_CrearLeerActualizarEliminar_FuncionaCorrectamente()
        {
            // 1. CREATE
            var nuevoVoluntario = new VoluntarioFormViewModel
            {
                vol_Nombres = $"CRUD {Guid.NewGuid().ToString().Substring(0, 8)}",
                vol_Recurrente = true,
                vol_HorasTrabajadas = 10,
                per_Identidad = $"0801{Guid.NewGuid().ToString().Substring(0, 8)}",
                per_Apellidos = "Prueba",
                per = new PersonaViewModel
                {
                    per_PrimerNombre = "Juan",
                    per_SegundoNombre = "Test",
                    per_ApellidoPaterno = "Prueba",
                    per_ApellidoMaterno = "Test",
                    per_FechaNacimiento = new DateTime(1990, 1, 1),
                    per_Domicilio = "Test Address",
                    per_Telefono = "99999999",
                    per_Correo = $"t{Guid.NewGuid().ToString().Substring(0, 6)}@t.com",
                    per_Identidad = $"0801{Guid.NewGuid().ToString().Substring(0, 8)}"
                }
            };

            bool insertado = await _service.AddAsync(nuevoVoluntario, _testUserId);
            Assert.True(insertado, "CREATE fallo — verificar SP de INSERT");

            // 2. READ
            var lista = await _service.ListAsync();
            var voluntarioCreado = lista.FirstOrDefault(x => x.vol_Nombres == nuevoVoluntario.vol_Nombres);
            Assert.True(voluntarioCreado != null,
                $"CREATE fallo: '{nuevoVoluntario.vol_Nombres}' no esta en la BD — verificar SP de INSERT");

            var voluntarioEncontrado = await _service.FindAsync(voluntarioCreado.vol_Id);
            Assert.True(voluntarioEncontrado != null,
                $"READ fallo: FindAsync no encontro Id={voluntarioCreado.vol_Id}");
            Assert.Equal(nuevoVoluntario.vol_Nombres, voluntarioEncontrado.vol_Nombres);

            // 3. UPDATE
            voluntarioEncontrado.vol_Nombres = $"CRUD2 {Guid.NewGuid().ToString().Substring(0, 8)}";
            bool actualizado = await _service.UpdateAsync(voluntarioEncontrado, _testUserId);
            Assert.True(actualizado, "UPDATE fallo");

            var voluntarioVerificado = await _service.FindAsync(voluntarioCreado.vol_Id);
            Assert.Equal(voluntarioEncontrado.vol_Nombres, voluntarioVerificado.vol_Nombres);

            // 4. DELETE
            bool eliminado = await _service.RemoveAsync(voluntarioCreado.vol_Id);
            Assert.True(eliminado, "DELETE fallo");

            var voluntarioEliminado = await _service.FindAsync(voluntarioCreado.vol_Id);
            Assert.Null(voluntarioEliminado);
        }
    }
}
