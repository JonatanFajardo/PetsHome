using System;

namespace PetsHome.Common.Entities
{
    public class PR_Seguridad_Configuracion_GetResult
    {
        public int cfg_Id { get; set; }
        public string cfg_PerfilPrincipal { get; set; }
        public bool cfg_WizardCompletado { get; set; }
        public string cfg_NombreOrganizacion { get; set; }
        public DateTime? cfg_FechaConfig { get; set; }
        public DateTime? cfg_FechaModifica { get; set; }
    }
}
