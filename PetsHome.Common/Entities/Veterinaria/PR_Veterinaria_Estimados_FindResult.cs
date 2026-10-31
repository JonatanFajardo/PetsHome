using System;

namespace PetsHome.Common.Entities
{
    public class PR_Veterinaria_Estimados_FindResult
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
}
