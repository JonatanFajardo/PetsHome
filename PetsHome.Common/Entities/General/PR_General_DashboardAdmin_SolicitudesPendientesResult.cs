using System;

namespace PetsHome.Common.Entities
{
    public class PR_General_DashboardAdmin_SolicitudesPendientesResult
    {
        public string   sol_NombreCompleto { get; set; }
        public string   sol_Estado         { get; set; }
        public DateTime sol_Fecha          { get; set; }
        public string   masc_Nombre        { get; set; }
        public int      DiasAntiguedad     { get; set; }
    }
}
