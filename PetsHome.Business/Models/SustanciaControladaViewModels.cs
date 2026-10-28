using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace PetsHome.Business.Models
{
    // ─── Fila de la bitacora (DataTable) ───────────
    public partial class SustanciaMovimientoListViewModel
    {
        public int scm_Id { get; set; }
        public int itm_Id { get; set; }
        public string itm_Codigo { get; set; }
        public string Farmaco { get; set; }
        public string scm_Tipo { get; set; }
        public decimal scm_Cantidad { get; set; }
        public int? masc_Id { get; set; }
        public string Mascota { get; set; }
        public int? scm_TestigoId { get; set; }
        public string Testigo { get; set; }
        public string scm_Lote { get; set; }
        public string scm_DeaSchedule { get; set; }
        public string scm_Notas { get; set; }
        public DateTime scm_Fecha { get; set; }
        public string RegistradoPor { get; set; }

        /// <summary>Tipo con tilde para mostrar en UI (los datos se guardan sin tildes).</summary>
        public string TipoDisplay => TipoConTilde(scm_Tipo);

        internal static string TipoConTilde(string tipo) => tipo;
    }

    // ─── Formulario de registrar movimiento ────────
    public partial class SustanciaMovimientoFormViewModel
    {
        [Required(ErrorMessage = "Seleccione el farmaco controlado.")]
        public int itm_Id { get; set; }

        [Required(ErrorMessage = "Indique el tipo de movimiento.")]
        public string scm_Tipo { get; set; }

        [Required(ErrorMessage = "Indique la cantidad.")]
        [Range(0.001, double.MaxValue, ErrorMessage = "La cantidad debe ser mayor que cero.")]
        public decimal scm_Cantidad { get; set; }

        public int? masc_Id { get; set; }
        public int? scm_TestigoId { get; set; }
        public string scm_Lote { get; set; }
        public string scm_DeaSchedule { get; set; }
        public string scm_Notas { get; set; }

        // Dropdowns de apoyo (no se persisten)
        public List<FarmacoControladoOpcion> FarmacoList { get; set; } = new List<FarmacoControladoOpcion>();
        public List<MascotaDropdownViewModel> MascotaList { get; set; } = new List<MascotaDropdownViewModel>();
        public List<TestigoOpcion> TestigoList { get; set; } = new List<TestigoOpcion>();

        public List<SelectListItem> TipoList { get; set; } = new List<SelectListItem>
        {
            new SelectListItem("Recibido (entra al inventario)", "Recibido"),
            new SelectListItem("Administrado (a un paciente)", "Administrado"),
            new SelectListItem("Desechado (requiere testigo)", "Desechado"),
            new SelectListItem("Devuelto (regresa al inventario)", "Devuelto"),
        };

        public List<SelectListItem> DeaScheduleList { get; set; } = new List<SelectListItem>
        {
            new SelectListItem("II", "II"),
            new SelectListItem("III", "III"),
            new SelectListItem("IV", "IV"),
            new SelectListItem("V", "V"),
        };
    }

    // ─── Detalle (read-only) ───────────────────────
    public partial class SustanciaMovimientoDetailViewModel
    {
        public int scm_Id { get; set; }
        public int itm_Id { get; set; }
        public string itm_Codigo { get; set; }
        public string Farmaco { get; set; }
        public string scm_Tipo { get; set; }
        public decimal scm_Cantidad { get; set; }
        public int? masc_Id { get; set; }
        public string Mascota { get; set; }
        public int? scm_TestigoId { get; set; }
        public string Testigo { get; set; }
        public string scm_Lote { get; set; }
        public string scm_DeaSchedule { get; set; }
        public string scm_Notas { get; set; }
        public DateTime scm_Fecha { get; set; }
        public string RegistradoPor { get; set; }
        public DateTime scm_FechaCrea { get; set; }
    }

    // ─── Resumen de saldo por farmaco ──────────────
    public partial class SustanciaSaldoViewModel
    {
        public int itm_Id { get; set; }
        public string itm_Codigo { get; set; }
        public string Farmaco { get; set; }
        public decimal TotalRecibido { get; set; }
        public decimal TotalDevuelto { get; set; }
        public decimal TotalAdministrado { get; set; }
        public decimal TotalDesechado { get; set; }
        public decimal Saldo { get; set; }
        public string DeaSchedule { get; set; }
        public int TotalMovimientos { get; set; }
    }

    // ─── Modelo de la pantalla Index (saldos + form embebido) ─
    public partial class SustanciaControladaIndexViewModel
    {
        public List<SustanciaSaldoViewModel> Saldos { get; set; } = new List<SustanciaSaldoViewModel>();
    }

    // ─── Opciones de dropdown propias del modulo ───
    public class FarmacoControladoOpcion
    {
        public int itm_Id { get; set; }
        public string itm_Codigo { get; set; }
        public string itm_Descripcion { get; set; }
    }

    public class TestigoOpcion
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
    }
}
