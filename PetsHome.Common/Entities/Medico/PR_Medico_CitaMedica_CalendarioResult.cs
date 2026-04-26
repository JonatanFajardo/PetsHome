using System;

namespace PetsHome.Common.Entities
{
    public partial class PR_Medico_CitaMedica_CalendarioResult
    {
        public int cita_Id { get; set; }
        public string Mascota { get; set; }
        public DateTime cita_FechaConsulta { get; set; }
        public string TipoConsulta { get; set; }
        public string cita_MotivoConsulta { get; set; }
        public string Gravedad { get; set; }
        public int Duracion { get; set; }
    }
}
