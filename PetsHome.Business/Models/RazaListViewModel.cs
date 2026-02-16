namespace PetsHome.Business.Models
{
    /// <summary>
    /// View model para listados de razas.
    /// </summary>
    public class RazaListViewModel
    {
        public long? Fila { get; set; }

        public int raza_Id { get; set; }

        public string raza_Descripcion { get; set; }

        public string raza_Tamano { get; set; }

        public string raza_TipoAnimal { get; set; }

        public string raza_TipoPelaje { get; set; }

        public string raza_ImagenUrl { get; set; }

        public string raza_EsActivo { get; set; }
    }
}
