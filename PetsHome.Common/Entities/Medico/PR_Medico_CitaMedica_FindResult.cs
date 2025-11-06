using System;

namespace PetsHome.Common.Entities
{
    public partial class PR_Medico_CitaMedica_FindResult
    {
        public int cita_Id { get; set; }
        public int masc_Id { get; set; }
        public DateTime cita_FechaConsulta { get; set; }
        public int? tipoCon_Id { get; set; }
        public int? grav_Id { get; set; }
        public string cita_MotivoConsulta { get; set; }
        public string cita_Diagnostico { get; set; }
        public decimal? cita_Peso { get; set; }
        public decimal? cita_Temperatura { get; set; }
        public int? cita_FrecuenciaCardiaca { get; set; }
        public int? cita_FrecuenciaRespiratoria { get; set; }
        public int? com_Id { get; set; }
        public int? vac_Id { get; set; }
        public string cita_ProcedimientosRealizados { get; set; }
        public string cita_ResultadosExamenes { get; set; }
        public DateTime? cita_ProximaCita { get; set; }
        public string cita_MotivoProximaCita { get; set; }
        public int cita_UsuarioCrea { get; set; }
        public string usuarioCrea { get; set; }
        public DateTime cita_FechaCrea { get; set; }
        public int? cita_UsuarioModifica { get; set; }
        public string usuarioModifica { get; set; }
        public DateTime? cita_FechaModifica { get; set; }
    }
}
