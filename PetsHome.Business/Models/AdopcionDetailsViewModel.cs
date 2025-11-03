using System;
using System.Collections.Generic;
using System.Text;

namespace PetsHome.Business.Models
{
    public class AdopcionDetailsViewModel
    {
        // Datos de la Solicitud
        public int sol_Id { get; set; }
        public string sol_Identidad { get; set; }
        public string sol_Nombres { get; set; }
        public string sol_Apellidos { get; set; }
        public string sol_Telefono { get; set; }
        public string sol_Correo { get; set; }
        public string sol_NombreUsuarioCrea { get; set; }
        public DateTime sol_FechaCrea { get; set; }
        public string sol_NombreUsuarioModifica { get; set; }
        public DateTime? sol_FechaModifica { get; set; }

        // Datos de la Mascota
        public int masc_Id { get; set; }
        public System.Byte[] masc_Imagen { get; set; }
        public string masc_Nombre { get; set; }
        public bool masc_EsAdoptado { get; set; }

        // Datos de Raza y Refugio
        public string raza_Descripcion { get; set; }
        public string refg_Nombre { get; set; }

        // Estado de Adopción
        public string adop_Estado { get; set; }
    }
}
