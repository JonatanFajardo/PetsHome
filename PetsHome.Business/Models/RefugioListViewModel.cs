namespace PetsHome.Business.Models
{
    /// <summary>
    /// View model para listados de refugios.
    /// </summary>
    public class RefugioListViewModel
    {
        public int refg_Id { get; set; }

        public string refg_Nombre { get; set; }

        public string refg_RTN { get; set; }

        public string refg_Ubicacion { get; set; }

        public string EsActivo { get; set; }
    }
}
