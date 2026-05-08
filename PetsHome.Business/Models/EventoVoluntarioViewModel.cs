using System;

namespace PetsHome.Business.Models
{
    public class EventoVoluntarioViewModel
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
