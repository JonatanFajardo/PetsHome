using System;

namespace PetsHome.Common.Entities
{
    public partial class PR_Medico_Recetas_DetailResult
    {
        public int receta_Id { get; set; }
        public int cita_Id { get; set; }
        public int masc_Id { get; set; }
        public string masc_Nombre { get; set; }
        public string receta_Medicamento { get; set; }
        public int? tipoMed_Id { get; set; }
        public string TipoMedicamento { get; set; }
        public int? viaAdmin_Id { get; set; }
        public string ViaAdministracion { get; set; }
        public string receta_Dosis { get; set; }
        public string receta_Frecuencia { get; set; }
        public string receta_Duracion { get; set; }
        public string receta_Instrucciones { get; set; }
        public DateTime? receta_FechaInicio { get; set; }
        public DateTime? receta_FechaFin { get; set; }
        public string receta_Estado { get; set; }
        public string UsuarioCreacion { get; set; }
        public DateTime receta_FechaCrea { get; set; }
        public string UsuarioModificacion { get; set; }
        public DateTime? receta_FechaModifica { get; set; }
    }
}
