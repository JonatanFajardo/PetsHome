using System;

namespace PetsHome.Common.Entities
{
    public partial class PR_Inventario_Proveedores_FindResult
    {
        public int prov_Id { get; set; }
        public string prov_Nombre { get; set; }
        public string prov_Email { get; set; }
        public string prov_Telefono { get; set; }
        public string prov_Direccion { get; set; }
        public string prov_Notas { get; set; }
        public bool prov_EsActivo { get; set; }
        public int prov_UsuarioCrea { get; set; }
        public string usuarioCrea { get; set; }
        public DateTime prov_FechaCrea { get; set; }
        public int? prov_UsuarioModifica { get; set; }
        public string usuarioModifica { get; set; }
        public DateTime? prov_FechaModifica { get; set; }
    }
}
