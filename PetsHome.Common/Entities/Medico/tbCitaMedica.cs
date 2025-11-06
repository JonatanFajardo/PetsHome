using System;
using System.Collections.Generic;

namespace PetsHome.Common.Entities
{
    public partial class tbCitaMedica
    {
        public tbCitaMedica()
        {
            tbRecetas = new HashSet<tbRecetas>();
            tbTratamientos = new HashSet<tbTratamientos>();
        }

        public int cita_Id { get; set; }
        public int masc_Id { get; set; }
        public DateTime cita_FechaConsulta { get; set; }
        public int? tipoCon_Id { get; set; }
        public string cita_MotivoConsulta { get; set; }
        public string cita_Diagnostico { get; set; }
        public int? grav_Id { get; set; }
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
        public bool cita_EsEliminado { get; set; }
        public int cita_UsuarioCrea { get; set; }
        public DateTime cita_FechaCrea { get; set; }
        public int? cita_UsuarioModifica { get; set; }
        public DateTime? cita_FechaModifica { get; set; }

        // Navegación
        public virtual tbMascotas Mascota { get; set; }
        public virtual tbTiposConsulta TipoConsulta { get; set; }
        public virtual tbGravedades Gravedad { get; set; }
        public virtual tbComportamientos Comportamiento { get; set; }
        public virtual tbUsuarios UsuarioCrea { get; set; }
        public virtual tbUsuarios UsuarioModifica { get; set; }
        public virtual ICollection<tbRecetas> tbRecetas { get; set; }
        public virtual ICollection<tbTratamientos> tbTratamientos { get; set; }
    }
}
