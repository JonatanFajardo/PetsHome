using System;

namespace PetsHome.Common.Entities
{
    public partial class PR_Medico_CitaMedica_DetailResult
    {
        public int cita_Id { get; set; }
        public int masc_Id { get; set; }
        public string Mascota { get; set; }
        public DateTime cita_FechaConsulta { get; set; }
        public int? tipoCon_Id { get; set; }
        public string TipoConsulta { get; set; }
        public int? grav_Id { get; set; }
        public string Gravedad { get; set; }
        public string cita_MotivoConsulta { get; set; }
        public string cita_Diagnostico { get; set; }
        public decimal? cita_Peso { get; set; }
        public decimal? cita_Temperatura { get; set; }
        public int? cita_FrecuenciaCardiaca { get; set; }
        public int? cita_FrecuenciaRespiratoria { get; set; }
        public int? com_Id { get; set; }
        public string Comportamiento { get; set; }
        public string cita_ProcedimientosRealizados { get; set; }
        public string cita_ResultadosExamenes { get; set; }
        public DateTime? cita_ProximaCita { get; set; }
        public string cita_MotivoProximaCita { get; set; }
        public string UsuarioCreacion { get; set; }
        public DateTime cita_FechaCrea { get; set; }
        public string UsuarioModificacion { get; set; }
        public DateTime? cita_FechaModifica { get; set; }
    }
}
