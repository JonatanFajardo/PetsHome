using System;

namespace PetsHome.Common.Entities
{
    /// <summary>
    /// Padecimiento / enfermedad crónica de una mascota (bandera roja clínica).
    /// Modelo adaptado de OpenVPM problem_list. Estado almacenado SIN tilde
    /// ('Cronico') para evitar mojibake; la UI muestra "Crónico".
    /// </summary>
    public partial class tbMascotaPadecimiento
    {
        public int pade_Id { get; set; }
        public int masc_Id { get; set; }
        public string pade_Nombre { get; set; }
        public string pade_Estado { get; set; }   // Vigente | Superado | Cronico
        public DateTime? pade_Desde { get; set; }
        public DateTime? pade_Hasta { get; set; }

        public bool pade_EsEliminado { get; set; }
        public int pade_UsuarioCrea { get; set; }
        public DateTime pade_FechaCrea { get; set; }
        public int? pade_UsuarioModifica { get; set; }
        public DateTime? pade_FechaModifica { get; set; }
    }
}
