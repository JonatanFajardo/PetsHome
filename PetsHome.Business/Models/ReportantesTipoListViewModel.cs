namespace PetsHome.Business.Models
{
    /// <summary>
    /// View model para listados de tipos de reportantes.
    /// </summary>
    public class ReportantesTipoListViewModel
    {
        public long? Fila { get; set; }

        public int reptip_Id { get; set; }

        public string reptip_Descripcion { get; set; }

        public bool reptip_EsActivo { get; set; }
    }
}
