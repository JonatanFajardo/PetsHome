using System;

namespace PetsHome.Common.Entities
{
    public class PR_Supervisor_Dashboard_SolicitudesResult
    {
        public int sol_Id { get; set; }
        public string sol_NombreCompleto { get; set; }
        public string sol_Iniciales { get; set; }
        public string sol_Correo { get; set; }
        public string masc_Nombre { get; set; }
        public string masc_Especie { get; set; }
        public string masc_Raza { get; set; }
        public string sol_Estado { get; set; }
        public DateTime sol_Fecha { get; set; }
    }
}
