using System;

namespace PetsHome.Common.Entities
{
    public partial class PR_Medico_Tratamientos_FindResult
    {
        public int trat_Id { get; set; }
        public int masc_Id { get; set; }
        public int? cita_Id { get; set; }
        public int? receta_Id { get; set; }
        public int? tipoPar_Id { get; set; }
        public string trat_ParasitoDetectado { get; set; }
        public string trat_Medicamento { get; set; }
        public int? tipoMed_Id { get; set; }
        public int? viaAdmin_Id { get; set; }
        public DateTime trat_FechaAplicacion { get; set; }
        public string trat_AplicadoPor { get; set; }
        public DateTime? trat_ProximaDosis { get; set; }
        public string trat_Estado { get; set; }
        public string trat_Observaciones { get; set; }
        public int trat_UsuarioCrea { get; set; }
        public string usuarioCrea { get; set; }
        public DateTime trat_FechaCrea { get; set; }
        public int? trat_UsuarioModifica { get; set; }
        public string usuarioModifica { get; set; }
        public DateTime? trat_FechaModifica { get; set; }
    }
}
