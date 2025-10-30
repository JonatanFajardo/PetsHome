using System;

namespace PetsHome.Common.Entities
{
    public class PR_Rescate_ReportesAbandono_FindResult
    {
        public int repa_Id { get; set; }
        public int reptip_Id { get; set; }
        public string TipoReportante { get; set; }
        public string repa_NombreReportante { get; set; }
        public string repa_TelefonoContacto { get; set; }
        public string repa_Email { get; set; }
        public DateTime repa_FechaReporte { get; set; }
        public string repa_UbicacionIncidente { get; set; }
        public string repa_DescripcionAnimal { get; set; }
        public string repa_EstadoAtencion { get; set; }
        public string repa_Observaciones { get; set; }
        public bool repa_EsAnonimo { get; set; }
        public int refg_Id { get; set; }
        public string NombreRefugio { get; set; }
        public int repa_UsuarioCrea { get; set; }
        public string usuarioCrea { get; set; }
        public DateTime repa_FechaCrea { get; set; }
        public int? repa_UsuarioModifica { get; set; }
        public string usuarioModifica { get; set; }
        public DateTime? repa_FechaModifica { get; set; }
    }
}
