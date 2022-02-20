﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace PetsHome.Common.Entities
{
    public partial class tbEventos
    {
        public tbEventos()
        {
            tbEventos_tbVoluntarios = new HashSet<tbEventos_tbVoluntarios>();
        }

        /// <summary>
        /// Identificador único de la tabla Eventos.
        /// </summary>
        public int eve_Id { get; set; }
        public string eve_Descripcion { get; set; }
        public int refg_Id { get; set; }
        public TimeSpan eve_HoraInicio { get; set; }
        public TimeSpan eve_HoraFinal { get; set; }
        public DateTime eve_Fecha { get; set; }
        /// <summary>
        /// Indica si el registro está desactivado permanentemente.
        /// </summary>
        public bool eve_EsEliminado { get; set; }
        /// <summary>
        /// Indica el identificador del usuario que creó el registro.
        /// </summary>
        public int eve_UsuarioCrea { get; set; }
        /// <summary>
        /// Registra la fecha en que se creó el registro.
        /// </summary>
        public DateTime eve_FechaCrea { get; set; }
        /// <summary>
        /// Indica el identificador del último usuario que modificó el registro.
        /// </summary>
        public int? eve_UsuarioModifica { get; set; }
        /// <summary>
        /// Registra la última fecha en que se modificó el registro.
        /// </summary>
        public DateTime? eve_FechaModifica { get; set; }

        public virtual tbUsuarios eve_UsuarioCreaNavigation { get; set; }
        public virtual tbRefugios refg { get; set; }
        public virtual ICollection<tbEventos_tbVoluntarios> tbEventos_tbVoluntarios { get; set; }
    }
}