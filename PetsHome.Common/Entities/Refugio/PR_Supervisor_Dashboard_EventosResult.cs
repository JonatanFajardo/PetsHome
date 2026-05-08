using System;

namespace PetsHome.Common.Entities
{
    public class PR_Supervisor_Dashboard_EventosResult
    {
        public int eve_Id { get; set; }
        public string eve_Descripcion { get; set; }
        public DateTime eve_Fecha { get; set; }
        public TimeSpan? eve_HoraInicio { get; set; }
        public string eve_Lugar { get; set; }
        public int eve_CantidadVoluntarios { get; set; }
    }
}
