using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PetsHome.Business.Models
{
    public class VoluntarioDetailViewModel
    {
        /// <summary>
        /// Obtiene o establece el valor de la propiedad.
        /// </summary>
        public int vol_Id { get; set; }

        /// <summary>
        /// Obtiene o establece el valor de la propiedad.
        /// </summary>
        public int vol_HorasTrabajadas { get; set; }

        /// <summary>
        /// Obtiene o establece el valor de la propiedad.
        /// </summary>
        public bool vol_Recurrente { get; set; }

        /// <summary>
        /// Obtiene o establece el valor de la propiedad.
        /// </summary>
        public string per_PrimerNombre { get; set; }

        /// <summary>
        /// Obtiene o establece el valor de la propiedad.
        /// </summary>
        public string per_SegundoNombre { get; set; }

        /// <summary>
        /// Obtiene o establece el valor de la propiedad.
        /// </summary>
        public string per_ApellidoPaterno { get; set; }

        /// <summary>
        /// Obtiene o establece el valor de la propiedad.
        /// </summary>
        public string per_ApellidoMaterno { get; set; }

        /// <summary>
        /// Obtiene o establece el valor de la propiedad.
        /// </summary>
        public string per_Identidad { get; set; }

        /// <summary>
        /// Obtiene o establece el valor de la propiedad.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime per_FechaNacimiento { get; set; }

        /// <summary>
        /// Obtiene o establece el valor de la propiedad.
        /// </summary>
        public string per_Domicilio { get; set; }

        /// <summary>
        /// Obtiene o establece el valor de la propiedad.
        /// </summary>
        public string per_Telefono { get; set; }

        /// <summary>
        /// Obtiene o establece el valor de la propiedad.
        /// </summary>
        public string per_Correo { get; set; }

        /// <summary>
        /// Obtiene o establece el valor de la propiedad.
        /// </summary>
        public int per_UsuarioCrea { get; set; }

        /// <summary>
        /// Obtiene o establece el valor de la propiedad.
        /// </summary>
        public string UsuarioCreacion { get; set; }

        /// <summary>
        /// Obtiene o establece el valor de la propiedad.
        /// </summary>
        public DateTime per_FechaCrea { get; set; }

        /// <summary>
        /// Obtiene o establece el valor de la propiedad.
        /// </summary>
        public int? per_UsuarioModifica { get; set; }

        /// <summary>
        /// Obtiene o establece el valor de la propiedad.
        /// </summary>
        public string UsuarioModificacion { get; set; }

        /// <summary>
        /// Obtiene o establece el valor de la propiedad.
        /// </summary>
        public DateTime? per_FechaModifica { get; set; }
    }
}
