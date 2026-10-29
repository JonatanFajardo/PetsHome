using System;

namespace PetsHome.Common.Entities
{
    /// <summary>
    /// Cargo pendiente de cobro por dispensación en consulta (cola dispensar→cobrar).
    /// Adaptado de OpenVPM dispense_charge_queue. NO toca inventario (stock ya descontado).
    /// Estados: Pendiente | Facturado | Condonado.
    /// </summary>
    public partial class tbCargoDispensacion
    {
        public int cargo_Id { get; set; }
        public int? receta_Id { get; set; }
        public int masc_Id { get; set; }
        public int cli_Id { get; set; }
        public int? cita_Id { get; set; }
        public int? itm_Id { get; set; }
        public int cargo_Cantidad { get; set; }
        public string cargo_DescripcionSnap { get; set; }
        public decimal cargo_PrecioUnitSnap { get; set; }
        public string cargo_Estado { get; set; }

        /// <summary>'Medicamento' | 'Servicio'. Discrimina el origen del cargo.</summary>
        public string cargo_TipoOrigen { get; set; }

        /// <summary>receta_Id (medicamento) o serv_Id (servicio).</summary>
        public int? cargo_OrigenId { get; set; }
        public int? fact_Id { get; set; }
        public int? fdet_Id { get; set; }
        public int? cargo_ResueltoPor { get; set; }
        public DateTime? cargo_FechaResuelto { get; set; }
        public string cargo_MotivoResolucion { get; set; }

        public bool cargo_EsEliminado { get; set; }
        public int cargo_UsuarioCrea { get; set; }
        public DateTime cargo_FechaCrea { get; set; }
        public int? cargo_UsuarioModifica { get; set; }
        public DateTime? cargo_FechaModifica { get; set; }
    }
}
