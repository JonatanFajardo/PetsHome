namespace PetsHome.Business.Models
{
    /// <summary>
    /// View model utilizado para mostrar recetas médicas en listados.
    /// </summary>
    public class RecetaListViewModel
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

        public string receta_Estado { get; set; }
    }
}
