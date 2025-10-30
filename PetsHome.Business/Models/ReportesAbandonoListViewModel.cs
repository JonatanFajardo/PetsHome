using System;

namespace PetsHome.Business.Models
{
    /// <summary>
    /// View model para listados de reportes de abandono.
    /// </summary>
    public class ReportesAbandonoListViewModel
    {
        public long? Fila { get; set; }

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
    }
}
