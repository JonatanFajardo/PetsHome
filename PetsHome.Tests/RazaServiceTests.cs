using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using PetsHome.Business.Models;
using PetsHome.Business.Services;
using PetsHome.Common.Entities;
using PetsHome.Logic.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PetsHome.Tests
{
    /// <summary>
    /// Pruebas unitarias para RazaService.
    /// Se usan dobles (mocks) para el repositorio, el mapper y el logger,
    /// de modo que las pruebas corren sin base de datos.
    /// </summary>
    public class RazaServiceTests
    {
        // -----------------------------------------------------------------------
        // Infraestructura compartida entre pruebas
        // -----------------------------------------------------------------------

        private readonly Mock<RazaRepository> _repoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<RazaService>> _loggerMock;
        private readonly RazaService _servicio;

        public RazaServiceTests()
        {
            // callBase: false → ningún método del repositorio llama a la BD real
            _repoMock = new Mock<RazaRepository>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<RazaService>>();

            _servicio = new RazaService(_repoMock.Object, _loggerMock.Object, _mapperMock.Object);
        }

        // -----------------------------------------------------------------------
        // ListAsync
        // -----------------------------------------------------------------------

        [Fact]
        public async Task ListAsync_CuandoExistenRazas_RetornaLista()
        {
            // Arrange
            var resultadoBD = new List<PR_Refugio_Razas_ListResult>
            {
                new PR_Refugio_Razas_ListResult { raza_Id = 1, raza_Descripcion = "Labrador" },
                new PR_Refugio_Razas_ListResult { raza_Id = 2, raza_Descripcion = "Beagle"   }
            };
            var resultadoEsperado = new List<RazaListViewModel>
            {
                new RazaListViewModel { raza_Id = 1, raza_Descripcion = "Labrador" },
                new RazaListViewModel { raza_Id = 2, raza_Descripcion = "Beagle"   }
            };

            _repoMock.Setup(r => r.ListAsync()).ReturnsAsync(resultadoBD);
            _mapperMock.Setup(m => m.Map<List<RazaListViewModel>>(It.IsAny<List<PR_Refugio_Razas_ListResult>>()))
                       .Returns(resultadoEsperado);

            // Act
            var resultado = await _servicio.ListAsync();

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.Count);
            Assert.Equal("Labrador", resultado[0].raza_Descripcion);
        }

        [Fact]
        public async Task ListAsync_CuandoRepoLanzaExcepcion_RetornaNull()
        {
            // Arrange
            _repoMock.Setup(r => r.ListAsync()).ThrowsAsync(new Exception("Error de BD"));

            // Act
            var resultado = await _servicio.ListAsync();

            // Assert — el servicio captura la excepción y retorna null en lugar de propagar
            Assert.Null(resultado);
        }

        // -----------------------------------------------------------------------
        // FindAsync
        // -----------------------------------------------------------------------

        [Fact]
        public async Task FindAsync_CuandoRazaExiste_RetornaViewModel()
        {
            // Arrange
            var resultadoBD = new PR_Refugio_Razas_FindResult { raza_Id = 3, raza_Descripcion = "Poodle" };
            var viewModel = new RazaFormViewModel { raza_Id = 3, raza_Descripcion = "Poodle" };

            _repoMock.Setup(r => r.FindAsync(3)).ReturnsAsync(resultadoBD);
            _mapperMock.Setup(m => m.Map<RazaFormViewModel>(resultadoBD)).Returns(viewModel);

            // Act
            var resultado = await _servicio.FindAsync(3);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(3, resultado.raza_Id);
            Assert.Equal("Poodle", resultado.raza_Descripcion);
        }

        [Fact]
        public async Task FindAsync_CuandoRepoLanzaExcepcion_RetornaNull()
        {
            // Arrange
            _repoMock.Setup(r => r.FindAsync(It.IsAny<int>())).ThrowsAsync(new Exception("No encontrado"));

            // Act
            var resultado = await _servicio.FindAsync(99);

            // Assert
            Assert.Null(resultado);
        }

        // -----------------------------------------------------------------------
        // AddAsync
        // -----------------------------------------------------------------------

        [Fact]
        public async Task AddAsync_ConDatosValidos_LlamaRepoConUsuarioAuditoria()
        {
            // Arrange
            var modelo = new RazaFormViewModel { raza_Descripcion = "Dálmata", raza_Tamano = "Grande" };
            var entidad = new tbRazas();
            int idUsuario = 7;

            _mapperMock.Setup(m => m.Map<tbRazas>(modelo)).Returns(entidad);
            _repoMock.Setup(r => r.AddAsync(entidad)).ReturnsAsync(true);

            // Act
            bool resultado = await _servicio.AddAsync(modelo, idUsuario);

            // Assert — retorna el valor del repositorio
            Assert.True(resultado);

            // Verifica que se asignó el usuario y la fecha de creación antes de insertar
            Assert.Equal(idUsuario, entidad.raza_UsuarioCrea);
            Assert.NotNull(entidad.raza_FechaCrea);
        }

        [Fact]
        public async Task AddAsync_CuandoRepoRetornaFalse_RetornaFalse()
        {
            // Arrange
            var modelo = new RazaFormViewModel { raza_Descripcion = "Chihuahua" };
            var entidad = new tbRazas();

            _mapperMock.Setup(m => m.Map<tbRazas>(modelo)).Returns(entidad);
            _repoMock.Setup(r => r.AddAsync(entidad)).ReturnsAsync(false);

            // Act
            bool resultado = await _servicio.AddAsync(modelo, 1);

            // Assert
            Assert.False(resultado);
        }

        // -----------------------------------------------------------------------
        // UpdateAsync
        // -----------------------------------------------------------------------

        [Fact]
        public async Task UpdateAsync_ConDatosValidos_LlamaRepoConUsuarioModificacion()
        {
            // Arrange
            var modelo = new RazaFormViewModel { raza_Id = 5, raza_Descripcion = "Bulldog" };
            var entidad = new tbRazas();
            int idUsuario = 10;

            _mapperMock.Setup(m => m.Map<tbRazas>(modelo)).Returns(entidad);
            _repoMock.Setup(r => r.EditAsync(entidad)).ReturnsAsync(true);

            // Act
            bool resultado = await _servicio.UpdateAsync(modelo, idUsuario);

            // Assert
            Assert.True(resultado);
            Assert.Equal(idUsuario, entidad.raza_UsuarioModifica);
            Assert.NotNull(entidad.raza_FechaModifica);
        }

        [Fact]
        public async Task UpdateAsync_CuandoRepoLanzaExcepcion_RetornaTrue()
        {
            // Nota: el servicio retorna `true` cuando hay excepción en Update
            // (comportamiento actual del código — la prueba documenta eso)
            var modelo = new RazaFormViewModel { raza_Id = 5, raza_Descripcion = "Bulldog" };
            var entidad = new tbRazas();

            _mapperMock.Setup(m => m.Map<tbRazas>(modelo)).Returns(entidad);
            _repoMock.Setup(r => r.EditAsync(entidad)).ThrowsAsync(new Exception("Error"));

            bool resultado = await _servicio.UpdateAsync(modelo, 1);

            Assert.True(resultado);
        }

        // -----------------------------------------------------------------------
        // RemoveAsync
        // -----------------------------------------------------------------------

        [Fact]
        public async Task RemoveAsync_CuandoEliminacionExitosa_RetornaTrue()
        {
            // Arrange
            _repoMock.Setup(r => r.RemoveAsync(4)).ReturnsAsync(true);

            // Act
            bool resultado = await _servicio.RemoveAsync(4);

            // Assert
            Assert.True(resultado);
        }

        [Fact]
        public async Task RemoveAsync_CuandoEliminacionFalla_RetornaFalse()
        {
            // Arrange
            _repoMock.Setup(r => r.RemoveAsync(4)).ReturnsAsync(false);

            // Act
            bool resultado = await _servicio.RemoveAsync(4);

            // Assert
            Assert.False(resultado);
        }

        [Fact]
        public async Task RemoveAsync_LlamaAlRepositorioConElIdCorrecto()
        {
            // Arrange
            _repoMock.Setup(r => r.RemoveAsync(It.IsAny<int>())).ReturnsAsync(true);

            // Act
            await _servicio.RemoveAsync(8);

            // Assert — verificamos que el repositorio se llamó exactamente una vez con id=8
            _repoMock.Verify(r => r.RemoveAsync(8), Times.Once);
        }
    }
}
