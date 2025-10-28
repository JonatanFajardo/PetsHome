using System;
using System.ComponentModel.DataAnnotations;

namespace PetsHome.Business.Models
{
    /// <summary>
    /// View model utilizado para mostrar los detalles de una solicitud.
    /// </summary>
    public class SolicitudDetailsViewModel
    {
        [Display(Name = "Id solicitud")]
        public int sol_Id { get; set; }

        public int sol_UsuarioCrea { get; set; }

        public string sol_Identidad { get; set; }

        public string sol_Nombres { get; set; }

        public string sol_Apellidos { get; set; }

        public string sol_Telefono { get; set; }

        public string sol_Correo { get; set; }

        public DateTime sol_Fecha { get; set; }

        public int masc_Id { get; set; }

        public byte[] masc_Imagen { get; set; }

        public string masc_Nombre { get; set; }

        public bool masc_EsAdoptado { get; set; }

        public string raza_Descripcion { get; set; }

        public string refg_Nombre { get; set; }

        public string sol_NombreUsuarioCrea { get; set; }

        public DateTime sol_FechaCrea { get; set; }

        public int? sol_UsuarioModifica { get; set; }

        public string sol_NombreUsuarioModifica { get; set; }

        public DateTime? sol_FechaModifica { get; set; }
    }
}
