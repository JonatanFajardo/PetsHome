using System;

namespace PetsHome.Common.Entities
{
    public class PR_Rescate_ReportantesTipo_DetailResult
    {
        public int reptip_Id { get; set; }
        public string reptip_Descripcion { get; set; }
        public bool reptip_EsActivo { get; set; }
        public string UsuarioCreacion { get; set; }
        public DateTime reptip_FechaCrea { get; set; }
        public string UsuarioModificacion { get; set; }
        public DateTime? reptip_FechaModifica { get; set; }
    }
}
