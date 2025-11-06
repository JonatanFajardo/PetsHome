using System;

namespace PetsHome.Common.Entities
{
    public partial class tbTratamientos
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
        public bool trat_EsEliminado { get; set; }
        public int trat_UsuarioCrea { get; set; }
        public DateTime trat_FechaCrea { get; set; }
        public int? trat_UsuarioModifica { get; set; }
        public DateTime? trat_FechaModifica { get; set; }

        // Navegación
        public virtual tbMascotas Mascota { get; set; }
        public virtual tbCitaMedica CitaMedica { get; set; }
        public virtual tbRecetas Receta { get; set; }
        public virtual tbTiposParasito TipoParasito { get; set; }
        public virtual tbTiposMedicamento TipoMedicamento { get; set; }
        public virtual tbViasAdministracion ViaAdministracion { get; set; }
        public virtual tbUsuarios UsuarioCrea { get; set; }
        public virtual tbUsuarios UsuarioModifica { get; set; }
    }
}
