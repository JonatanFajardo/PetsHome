namespace PetsHome.Business.Models
{
    public class ModuloViewModel
    {
        public int modu_Id { get; set; }
        public string modu_Codigo { get; set; }
        public string modu_Nombre { get; set; }
        public string modu_Area { get; set; }
        public bool modu_EsNucleo { get; set; }
        public bool modu_EsActivo { get; set; }
        public string modu_RequiereModulos { get; set; }
        public int modu_Orden { get; set; }
    }
}
