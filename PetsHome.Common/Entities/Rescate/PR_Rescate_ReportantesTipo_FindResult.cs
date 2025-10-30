using System;

namespace PetsHome.Common.Entities
{
    public class PR_Rescate_ReportantesTipo_FindResult
    {
        public int reptip_Id { get; set; }
        public string reptip_Descripcion { get; set; }
        public bool reptip_EsActivo { get; set; }
        public int reptip_UsuarioCrea { get; set; }
        public string usuarioCrea { get; set; }
        public DateTime reptip_FechaCrea { get; set; }
        public int? reptip_UsuarioModifica { get; set; }
        public string usuarioModifica { get; set; }
        public DateTime? reptip_FechaModifica { get; set; }
    }
}
