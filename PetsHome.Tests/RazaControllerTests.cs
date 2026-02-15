using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using PetsHome.Business.Models;
using PetsHome.Business.Services;
using PetsHome.Logic.Repositories;
using PetsHome.UI.Controllers;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace PetsHome.Tests
{
    /// <summary>
    /// Pruebas unitarias para RazaController.
    /// Se usa un mock de RazaService para no depender de base de datos.
    /// </summary>
    public class RazaControllerTests
    {
        // -----------------------------------------------------------------------
        // Helpers
        // -----------------------------------------------------------------------

        /// <summary>
        /// Crea un controlador con un usuario autenticado (userId = 1).
        /// </summary>
        private static RazaController CrearControladorAutenticado(Mock<RazaService> serviceMock)
        {
            var controller = new RazaController(serviceMock.Object);

            // Simular usuario autenticado con claim de NameIdentifier
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Name, "testuser")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);

            var httpContext = new DefaultHttpContext { User = principal };

            // Configurar IUrlHelperFactory (requerido por RedirectToAction)
            var urlHelperFactory = new Mock<IUrlHelperFactory>();
            urlHelperFactory.Setup(f => f.GetUrlHelper(It.IsAny<ActionContext>()))
                            .Returns(Mock.Of<IUrlHelper>());

            var serviceProvider = new Mock<System.IServiceProvider>();
            serviceProvider.Setup(sp => sp.GetService(typeof(IUrlHelperFactory)))
                           .Returns(urlHelperFactory.Object);
            httpContext.RequestServices = serviceProvider.Object;

            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
            controller.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());

            return controller;
        }

        private static Mock<RazaService> CrearServicioMock()
            // Moq necesita los argumentos del constructor de RazaService
            => new Mock<RazaService>(
                new Mock<RazaRepository>().Object,
                Mock.Of<ILogger<RazaService>>(),
                Mock.Of<IMapper>());

        // -----------------------------------------------------------------------
        // Index
        // -----------------------------------------------------------------------

        [Fact]
        public void Index_RetornaViewResult()
        {
            var serviceMock = CrearServicioMock();
            var controller = CrearControladorAutenticado(serviceMock);

            IActionResult resultado = controller.Index();

            Assert.IsType<ViewResult>(resultado);
        }

        // -----------------------------------------------------------------------
        // List
        // -----------------------------------------------------------------------

        [Fact]
        public async Task List_CuandoServicioRetornaLista_RetornaJsonConData()
        {
            var serviceMock = CrearServicioMock();
            serviceMock.Setup(s => s.ListAsync()).ReturnsAsync(new List<RazaListViewModel>
            {
                new RazaListViewModel { raza_Id = 1, raza_Descripcion = "Labrador" }
            });

            var controller = CrearControladorAutenticado(serviceMock);

            IActionResult resultado = await controller.List();

            var json = Assert.IsType<JsonResult>(resultado);
            Assert.NotNull(json.Value);
        }

        [Fact]
        public async Task List_CuandoServicioRetornaNull_RedireccionaAIndex()
        {
            var serviceMock = CrearServicioMock();
            serviceMock.Setup(s => s.ListAsync()).ReturnsAsync((List<RazaListViewModel>)null);

            var controller = CrearControladorAutenticado(serviceMock);

            IActionResult resultado = await controller.List();

            var redirect = Assert.IsType<RedirectToActionResult>(resultado);
            Assert.Equal("Index", redirect.ActionName);
        }

        // -----------------------------------------------------------------------
        // Find
        // -----------------------------------------------------------------------

        [Fact]
        public async Task Find_CuandoRazaExiste_RetornaJsonConItem()
        {
            var serviceMock = CrearServicioMock();
            serviceMock.Setup(s => s.FindAsync(2))
                       .ReturnsAsync(new RazaFormViewModel { raza_Id = 2, raza_Descripcion = "Beagle" });

            var controller = CrearControladorAutenticado(serviceMock);

            IActionResult resultado = await controller.Find(2);

            var json = Assert.IsType<JsonResult>(resultado);
            Assert.NotNull(json.Value);
        }

        // -----------------------------------------------------------------------
        // Detail
        // -----------------------------------------------------------------------

        [Fact]
        public async Task Detail_CuandoIdCero_RedireccionaAIndex()
        {
            var serviceMock = CrearServicioMock();
            var controller = CrearControladorAutenticado(serviceMock);

            IActionResult resultado = await controller.Detail(0);

            var redirect = Assert.IsType<RedirectToActionResult>(resultado);
            Assert.Equal("Index", redirect.ActionName);
        }

        [Fact]
        public async Task Detail_CuandoRazaNoEncontrada_RedireccionaAIndex()
        {
            var serviceMock = CrearServicioMock();
            serviceMock.Setup(s => s.DetailAsync(99)).ReturnsAsync((RazaDetailsViewModel)null);

            var controller = CrearControladorAutenticado(serviceMock);

            IActionResult resultado = await controller.Detail(99);

            var redirect = Assert.IsType<RedirectToActionResult>(resultado);
            Assert.Equal("Index", redirect.ActionName);
        }

        [Fact]
        public async Task Detail_CuandoRazaExiste_RetornaView()
        {
            var serviceMock = CrearServicioMock();
            serviceMock.Setup(s => s.DetailAsync(1))
                       .ReturnsAsync(new RazaDetailsViewModel { raza_Id = 1, raza_Descripcion = "Labrador" });

            var controller = CrearControladorAutenticado(serviceMock);

            IActionResult resultado = await controller.Detail(1);

            var view = Assert.IsType<ViewResult>(resultado);
            Assert.IsType<RazaDetailsViewModel>(view.Model);
        }

        // -----------------------------------------------------------------------
        // Add — creación (isEdit = false)
        // -----------------------------------------------------------------------

        [Fact]
        public async Task Add_NuevaRaza_LlamaAddAsyncYRetornaJson()
        {
            var serviceMock = CrearServicioMock();
            serviceMock.Setup(s => s.AddAsync(It.IsAny<RazaFormViewModel>(), 1)).ReturnsAsync(true);

            var controller = CrearControladorAutenticado(serviceMock);
            var modelo = new RazaFormViewModel { raza_Id = 0, raza_Descripcion = "Dálmata" }; // isEdit = false

            IActionResult resultado = await controller.Add(modelo);

            var json = Assert.IsType<JsonResult>(resultado);
            serviceMock.Verify(s => s.AddAsync(modelo, 1), Times.Once);
        }

        // -----------------------------------------------------------------------
        // Add — edición (isEdit = true)
        // -----------------------------------------------------------------------

        [Fact]
        public async Task Add_EditarRaza_LlamaUpdateAsyncYRetornaJson()
        {
            var serviceMock = CrearServicioMock();
            serviceMock.Setup(s => s.UpdateAsync(It.IsAny<RazaFormViewModel>(), 1)).ReturnsAsync(true);

            var controller = CrearControladorAutenticado(serviceMock);
            var modelo = new RazaFormViewModel { raza_Id = 3, raza_Descripcion = "Poodle" }; // isEdit = true

            IActionResult resultado = await controller.Add(modelo);

            var json = Assert.IsType<JsonResult>(resultado);
            serviceMock.Verify(s => s.UpdateAsync(modelo, 1), Times.Once);
        }

        // -----------------------------------------------------------------------
        // Remove
        // -----------------------------------------------------------------------

        [Fact]
        public async Task Remove_LlamaRemoveAsyncConIdCorrecto()
        {
            var serviceMock = CrearServicioMock();
            serviceMock.Setup(s => s.RemoveAsync(5)).ReturnsAsync(true);

            var controller = CrearControladorAutenticado(serviceMock);

            IActionResult resultado = await controller.Remove(5);

            var json = Assert.IsType<JsonResult>(resultado);
            serviceMock.Verify(s => s.RemoveAsync(5), Times.Once);
        }
    }

}
