using System;
using System.Collections.Generic;

namespace PetsHome.Business.Models
{
    // ─── Listado de presupuestos ──────────────────────────
    public class EstimadoListViewModel
    {
        public int Fila { get; set; }
        public int est_Id { get; set; }
        public int? cli_Id { get; set; }
        public string Cliente { get; set; }
        public int? masc_Id { get; set; }
        public string Mascota { get; set; }
        public DateTime est_Fecha { get; set; }
        public DateTime? est_Vigencia { get; set; }
        public string est_Estado { get; set; }
        public decimal est_Total { get; set; }
        public int? est_FacturaId { get; set; }
        public int Lineas { get; set; }
    }

    // ─── Encabezado (Find) ────────────────────────────────
    public class EstimadoFindViewModel
    {
        public int est_Id { get; set; }
        public int? cli_Id { get; set; }
        public string Cliente { get; set; }
        public string cli_Identidad { get; set; }
        public int? masc_Id { get; set; }
        public string Mascota { get; set; }
        public DateTime est_Fecha { get; set; }
        public DateTime? est_Vigencia { get; set; }
        public string est_Estado { get; set; }
        public decimal est_Total { get; set; }
        public string est_Notas { get; set; }
        public int? est_FacturaId { get; set; }
    }

    // ─── Linea del presupuesto (detalle) ──────────────────
    public class EstimadoDetalleViewModel
    {
        public int estd_Id { get; set; }
        public int est_Id { get; set; }
        public string estd_Tipo { get; set; }
        public int? serv_Id { get; set; }
        public int? itm_Id { get; set; }
        public string estd_Descripcion { get; set; }
        public decimal estd_Cantidad { get; set; }
        public decimal estd_PrecioUnit { get; set; }
        public decimal estd_Subtotal { get; set; }
    }

    // ─── Detalle completo (encabezado + lineas) ───────────
    public class EstimadoDetailViewModel
    {
        public EstimadoFindViewModel Header { get; set; }
        public List<EstimadoDetalleViewModel> Lineas { get; set; } = new List<EstimadoDetalleViewModel>();
    }

    // ─── Payload de creacion/edicion (Form) ───────────────
    public class EstimadoFormViewModel
    {
        public int? estId { get; set; }                 // null = crear, set = editar
        public int? cliId { get; set; }
        public int? mascId { get; set; }
        public DateTime? vigencia { get; set; }
        public string notas { get; set; }
        public List<EstimadoLineaViewModel> lineas { get; set; } = new List<EstimadoLineaViewModel>();
    }

    public class EstimadoLineaViewModel
    {
        public string tipo { get; set; }        // "Servicio" | "Producto"
        public int? servId { get; set; }
        public int? itmId { get; set; }
        public string descripcion { get; set; }
        public decimal cantidad { get; set; }
        public decimal precioUnit { get; set; }
    }

    // ─── Resultado de operaciones ─────────────────────────
    public class EstimadoResultViewModel
    {
        public bool success { get; set; }
        public int? estId { get; set; }
        public int? factId { get; set; }
        public string message { get; set; }
    }
}
