using System;

namespace PetsHome.Business.Models
{
    // ─── Lista (tabla con filtros) ─────────────────
    public partial class RecordatorioListViewModel
    {
        public int rec_Id { get; set; }
        public int masc_Id { get; set; }
        public string Mascota { get; set; }
        public int? cli_Id { get; set; }
        public string Responsable { get; set; }
        public string Telefono { get; set; }
        public string rec_Tipo { get; set; }
        public DateTime rec_FechaObjetivo { get; set; }
        public string rec_Estado { get; set; }
        public string rec_Nota { get; set; }
        public string rec_Origen { get; set; }
        public DateTime? rec_FechaCompletado { get; set; }
        public int DiasRestantes { get; set; }
        public bool EsVencido { get; set; }
        public DateTime rec_FechaCrea { get; set; }
    }

    // ─── Detalle (read-only) ───────────────────────
    public partial class RecordatorioDetailViewModel
    {
        public int rec_Id { get; set; }
        public int masc_Id { get; set; }
        public string Mascota { get; set; }
        public int? cli_Id { get; set; }
        public string Responsable { get; set; }
        public string ResponsableContacto { get; set; }
        public string rec_Tipo { get; set; }
        public DateTime rec_FechaObjetivo { get; set; }
        public string rec_Estado { get; set; }
        public string rec_Nota { get; set; }
        public string rec_Origen { get; set; }
        public DateTime? rec_FechaCompletado { get; set; }
        public int DiasRestantes { get; set; }
        public string UsuarioCreacion { get; set; }
        public DateTime rec_FechaCrea { get; set; }
        public string UsuarioModificacion { get; set; }
        public DateTime? rec_FechaModifica { get; set; }
    }
}
