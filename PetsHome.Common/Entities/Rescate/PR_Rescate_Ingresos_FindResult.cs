using System;

namespace PetsHome.Common.Entities
{
    public class PR_Rescate_Ingresos_FindResult
    {
        public int ingr_Id { get; set; }
        public int? repa_Id { get; set; }
        public int refg_Id { get; set; }
        public string NombreRefugio { get; set; }
        public DateTime ingr_FechaIngreso { get; set; }
        public string ingr_LugarRescate { get; set; }
        public string ingr_CondicionInicial { get; set; }
        public string ingr_PersonaRescatista { get; set; }
        public string ingr_MedioTransporte { get; set; }
        public string ingr_Observaciones { get; set; }
        public bool ingr_EsEmergencia { get; set; }
        public string LugarReporte { get; set; }
        public string repa_DescripcionAnimal { get; set; }
        public string repa_EstadoAtencion { get; set; }
        public string repa_NombreReportante { get; set; }
        public string TelefonoReportante { get; set; }
        public int ingr_UsuarioCrea { get; set; }
        public string usuarioCrea { get; set; }
        public DateTime ingr_FechaCrea { get; set; }
        public int? ingr_UsuarioModifica { get; set; }
        public string usuarioModifica { get; set; }
        public DateTime? ingr_FechaModifica { get; set; }
        public int TieneMascota { get; set; }
        public int? MascotaId { get; set; }
        public string MascotaNombre { get; set; }
    }
}
