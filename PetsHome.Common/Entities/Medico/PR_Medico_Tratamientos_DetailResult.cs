using System;

namespace PetsHome.Common.Entities
{
    public partial class PR_Medico_Tratamientos_DetailResult
    {
        public int trat_Id { get; set; }
        public int masc_Id { get; set; }
        public string Mascota { get; set; }
        public int? cita_Id { get; set; }
        public int? receta_Id { get; set; }
        public int? tipoPar_Id { get; set; }
        public string TipoParasito { get; set; }
        public string CategoriaParasito { get; set; }
        public string trat_ParasitoDetectado { get; set; }
        public string trat_Medicamento { get; set; }
        public int? tipoMed_Id { get; set; }
        public string TipoMedicamento { get; set; }
        public int? viaAdmin_Id { get; set; }
        public string ViaAdministracion { get; set; }
        public DateTime trat_FechaAplicacion { get; set; }
        public string trat_AplicadoPor { get; set; }
        public DateTime? trat_ProximaDosis { get; set; }
        public string trat_Estado { get; set; }
        public string trat_Observaciones { get; set; }
        public string UsuarioCreacion { get; set; }
        public DateTime trat_FechaCrea { get; set; }
        public string UsuarioModificacion { get; set; }
        public DateTime? trat_FechaModifica { get; set; }
    }
}
