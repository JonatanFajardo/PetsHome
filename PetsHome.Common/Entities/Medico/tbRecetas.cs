using System;

namespace PetsHome.Common.Entities
{
    public partial class tbRecetas
    {
        public int receta_Id { get; set; }
        public int cita_Id { get; set; }
        public int masc_Id { get; set; }
        public string receta_Medicamento { get; set; }
        public int? tipoMed_Id { get; set; }
        public int? viaAdmin_Id { get; set; }
        public string receta_Dosis { get; set; }
        public string receta_Frecuencia { get; set; }
        public string receta_Duracion { get; set; }
        public string receta_Instrucciones { get; set; }
        public DateTime? receta_FechaInicio { get; set; }
        public DateTime? receta_FechaFin { get; set; }
        public string receta_Estado { get; set; }

        /// <summary>Item de inventario del que proviene el medicamento (NULL = texto libre).</summary>
        public int? itm_Id { get; set; }

        /// <summary>Resurtidos autorizados por el veterinario (0 = sin resurtidos).</summary>
        public int receta_ResurtidosAutorizados { get; set; }

        /// <summary>Resurtidos ya consumidos.</summary>
        public int receta_ResurtidosUsados { get; set; }

        public bool receta_EsEliminado { get; set; }
        public int receta_UsuarioCrea { get; set; }
        public DateTime receta_FechaCrea { get; set; }
        public int? receta_UsuarioModifica { get; set; }
        public DateTime? receta_FechaModifica { get; set; }

        // Navegación
        public virtual tbCitaMedica CitaMedica { get; set; }
        public virtual tbMascotas Mascota { get; set; }
        public virtual tbTiposMedicamento TipoMedicamento { get; set; }
        public virtual tbViasAdministracion ViaAdministracion { get; set; }
        public virtual tbUsuarios UsuarioCrea { get; set; }
        public virtual tbUsuarios UsuarioModifica { get; set; }
    }
}
