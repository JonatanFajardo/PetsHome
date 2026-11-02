using System;

namespace PetsHome.Common.Entities
{
    public partial class PR_Inventario_Proveedores_DetailResult
    {
        public int prov_Id { get; set; }
        public string prov_Nombre { get; set; }
        public string prov_Email { get; set; }
        public string prov_Telefono { get; set; }
        public string prov_Direccion { get; set; }
        public string prov_Notas { get; set; }
        public string prov_EsActivo { get; set; }
        public string UsuarioCreacion { get; set; }
        public DateTime prov_FechaCrea { get; set; }
        public string UsuarioModificacion { get; set; }
        public DateTime? prov_FechaModifica { get; set; }
    }
}
