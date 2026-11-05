using System;

namespace PetsHome.Common.Entities
{
    /// <summary>Detalle de un procedimiento con nombres resueltos (mascota, vet, servicio, auditoria).</summary>
    public partial class PR_Medico_Procedimiento_DetailResult
    {
        public int proc_Id { get; set; }
        public int masc_Id { get; set; }
        public string Mascota { get; set; }
        public int? cita_Id { get; set; }
        public string proc_Nombre { get; set; }
        public string proc_Descripcion { get; set; }
        public int? proc_RealizadoPor { get; set; }
        public string RealizadoPorNombre { get; set; }
        public string proc_Anestesia { get; set; }
        public int? proc_DuracionMinutos { get; set; }
        public string proc_Notas { get; set; }
        public DateTime proc_Fecha { get; set; }
        public int? serv_Id { get; set; }
        public string ServicioNombre { get; set; }
        public decimal? ServicioPrecio { get; set; }
        public string UsuarioCreacion { get; set; }
        public DateTime proc_FechaCrea { get; set; }
        public string UsuarioModificacion { get; set; }
        public DateTime? proc_FechaModifica { get; set; }
    }
}
