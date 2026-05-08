using System;

namespace PetsHome.Common.Entities
{
    public partial class PR_Refugio_EventoVoluntarios_ListResult
    {
        public int evevol_Id { get; set; }
        public int eve_Id { get; set; }
        public int vol_Id { get; set; }
        public string vol_NombreCompleto { get; set; }
        public string per_Telefono { get; set; }
        public string evevol_Estado { get; set; }
        public DateTime? evevol_FechaConfirmacion { get; set; }
    }
}
