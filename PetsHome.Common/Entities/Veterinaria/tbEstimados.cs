using System;

namespace PetsHome.Common.Entities
{
    public partial class tbEstimados
    {
        public int est_Id { get; set; }
        public int? cli_Id { get; set; }
        public int? masc_Id { get; set; }
        public DateTime est_Fecha { get; set; }
        public DateTime? est_Vigencia { get; set; }
        public string est_Estado { get; set; }
        public decimal est_Total { get; set; }
        public string est_Notas { get; set; }
        public int? est_FacturaId { get; set; }
        public bool est_EsEliminado { get; set; }
        public int est_UsuarioCrea { get; set; }
        public DateTime est_FechaCrea { get; set; }
        public int? est_UsuarioModifica { get; set; }
        public DateTime? est_FechaModifica { get; set; }
    }
}
