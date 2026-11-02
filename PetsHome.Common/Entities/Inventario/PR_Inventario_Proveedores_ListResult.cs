using System;

namespace PetsHome.Common.Entities
{
    public partial class PR_Inventario_Proveedores_ListResult
    {
        public int Fila { get; set; }
        public int prov_Id { get; set; }
        public string prov_Nombre { get; set; }
        public string prov_Email { get; set; }
        public string prov_Telefono { get; set; }
        public string prov_EsActivo { get; set; }
    }
}
