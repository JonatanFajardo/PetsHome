using System;
using System.Collections.Generic;
using System.Text;

namespace PetsHome.Business.Models
{
    public class CitaMedicaDetailViewModel
    {
        public int medic_Id { get; set; }
        public int masc_Id { get; set; }
        public string masc_Nombre { get; set; }
        public string raza_Descripcion { get; set; }
        public int com_Id { get; set; }
        public string com_Descripcion { get; set; }
        public DateTime medic_FechaConsulta { get; set; }
        public string medic_TipoConsulta { get; set; }
        public string medic_MotivoConsulta { get; set; }
        public string medic_Diagnostico { get; set; }
        public decimal? medic_Peso { get; set; }
        public decimal? medic_Temperatura { get; set; }
        public int? medic_FrecuenciaCardiaca { get; set; }
        public int? medic_FrecuenciaRespiratoria { get; set; }
        public int? vac_Id { get; set; }
        public string medic_MedicamentosRecetados { get; set; }
        public string medic_Dosificacion { get; set; }
        public string medic_ProcedimientosRealizados { get; set; }
        public string medic_ResultadosExamenes { get; set; }
        public DateTime? medic_ProximaCita { get; set; }
        public string medic_MotivoProximaCita { get; set; }
        public bool medic_EsEliminado { get; set; }
        public int medic_UsuarioCrea { get; set; }
        public string UsuarioCreacion { get; set; }
        public DateTime medic_FechaCrea { get; set; }
        public int? medic_UsuarioModifica { get; set; }
        public string UsuarioModificacion { get; set; }
        public DateTime? medic_FechaModifica { get; set; }
    }
}
