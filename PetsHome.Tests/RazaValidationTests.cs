using PetsHome.Business.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Xunit;

namespace PetsHome.Tests
{
    /// <summary>
    /// Pruebas unitarias para las validaciones del modelo RazaFormViewModel.
    /// Verifica que las reglas de negocio del formulario funcionen correctamente.
    /// </summary>
    public class RazaValidationTests
    {
        // -----------------------------------------------------------------------
        // EspaciosAttribute
        // -----------------------------------------------------------------------

        [Fact]
        public void EspaciosAttribute_DescripcionConEspacioInicial_EsInvalido()
        {
            var atributo = new EspaciosAttribute();
            bool resultado = atributo.IsValid(" Labrador");
            Assert.False(resultado);
        }

        [Fact]
        public void EspaciosAttribute_DescripcionSinEspacioInicial_EsValido()
        {
            var atributo = new EspaciosAttribute();
            bool resultado = atributo.IsValid("Labrador");
            Assert.True(resultado);
        }

        [Fact]
        public void EspaciosAttribute_CadenaVacia_EsValido()
        {
            // Cadena vacía se considera válida (Required se encarga de eso)
            var atributo = new EspaciosAttribute();
            bool resultado = atributo.IsValid("");
            Assert.True(resultado);
        }

        [Fact]
        public void EspaciosAttribute_ValorNull_EsValido()
        {
            var atributo = new EspaciosAttribute();
            bool resultado = atributo.IsValid(null);
            Assert.True(resultado);
        }

        [Fact]
        public void EspaciosAttribute_SoloEspacios_EsInvalido()
        {
            var atributo = new EspaciosAttribute();
            bool resultado = atributo.IsValid("   ");
            Assert.False(resultado);
        }

        // -----------------------------------------------------------------------
        // isEdit - propiedad calculada
        // -----------------------------------------------------------------------

        [Fact]
        public void IsEdit_CuandoIdEsCero_RetornaFalse()
        {
            var modelo = new RazaFormViewModel { raza_Id = 0 };
            Assert.False(modelo.isEdit);
        }

        [Fact]
        public void IsEdit_CuandoIdEsPositivo_RetornaTrue()
        {
            var modelo = new RazaFormViewModel { raza_Id = 5 };
            Assert.True(modelo.isEdit);
        }

        // -----------------------------------------------------------------------
        // Validación completa del modelo con DataAnnotations
        // -----------------------------------------------------------------------

        [Fact]
        public void ModeloRaza_DescripcionVacia_FallaValidacion()
        {
            var modelo = new RazaFormViewModel
            {
                raza_Descripcion = ""  // campo requerido
            };

            var errores = ValidarModelo(modelo);

            Assert.Contains(errores, e => e.MemberNames.Contains(nameof(RazaFormViewModel.raza_Descripcion)));
        }

        [Fact]
        public void ModeloRaza_DescripcionValida_PasaValidacion()
        {
            var modelo = new RazaFormViewModel
            {
                raza_Descripcion = "Golden Retriever"
            };

            var errores = ValidarModelo(modelo);

            Assert.DoesNotContain(errores, e => e.MemberNames.Contains(nameof(RazaFormViewModel.raza_Descripcion)));
        }

        [Fact]
        public void ModeloRaza_DescripcionExcede50Caracteres_FallaValidacion()
        {
            var modelo = new RazaFormViewModel
            {
                raza_Descripcion = new string('A', 51)
            };

            var errores = ValidarModelo(modelo);

            Assert.Contains(errores, e => e.MemberNames.Contains(nameof(RazaFormViewModel.raza_Descripcion)));
        }

        [Fact]
        public void ModeloRaza_DescripcionConEspacioInicial_FallaValidacion()
        {
            var modelo = new RazaFormViewModel
            {
                raza_Descripcion = " Labrador"
            };

            var errores = ValidarModelo(modelo);

            Assert.Contains(errores, e => e.MemberNames.Contains(nameof(RazaFormViewModel.raza_Descripcion)));
        }

        [Fact]
        public void ModeloRaza_TamanoExcede20Caracteres_FallaValidacion()
        {
            var modelo = new RazaFormViewModel
            {
                raza_Descripcion = "Labrador",
                raza_Tamano = new string('X', 21)
            };

            var errores = ValidarModelo(modelo);

            Assert.Contains(errores, e => e.MemberNames.Contains(nameof(RazaFormViewModel.raza_Tamano)));
        }

        // -----------------------------------------------------------------------
        // Helper
        // -----------------------------------------------------------------------

        private static List<ValidationResult> ValidarModelo(object modelo)
        {
            var resultados = new List<ValidationResult>();
            var contexto = new ValidationContext(modelo);
            Validator.TryValidateObject(modelo, contexto, resultados, validateAllProperties: true);
            return resultados;
        }
    }
}
