using System;

namespace PetsHome.Business.Models
{
    /// <summary>
    /// View model utilizado para mostrar los detalles de un voluntario.
    /// </summary>
    public class VoluntarioDetailsViewModel
    {
        public int vol_Id { get; set; }

        public int? vol_HorasTrabajadas { get; set; }

        public bool vol_Recurrente { get; set; }

        public string vol_Nombres { get; set; }

        public string per_Apellidos { get; set; }

        public string per_Identidad { get; set; }

        public DateTime per_FechaNacimiento { get; set; }

        public string per_Domicilio { get; set; }

        public string per_Telefono { get; set; }

        public string per_Correo { get; set; }

        public int per_UsuarioCrea { get; set; }

        public string? vol_NombreUsuarioCrea { get; set; }

        public DateTime vol_FechaCrea { get; set; }

        public int? vol_UsuarioModifica { get; set; }

        public string? vol_NombreUsuarioModifica { get; set; }

        public DateTime? vol_FechaModifica { get; set; }

        public PersonaViewModel per { get; set; } = new PersonaViewModel();
    }
}
