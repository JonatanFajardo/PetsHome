﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace PetsHome.Common.Entities
{
    public partial class tbEntradasDetalles
    {
        /// <summary>
        /// Identificador único de la tabla tbEntradasDetalles.
        /// </summary>
        public int entdet_Id { get; set; }
        /// <summary>
        /// Este es el ID del departamento que hace referencia al primary key de la tabla tbEntradas.
        /// </summary>
        public int ent_Id { get; set; }
        /// <summary>
        /// Este es el ID del departamento que hace referencia al primary key de la tabla tbItems.
        /// </summary>
        public int itm_Id { get; set; }
        /// <summary>
        /// Suma del numero de producto ingresado.
        /// </summary>
        public int entdet_Cantidad { get; set; }
        /// <summary>
        /// Indica si el registro está desactivado permanentemente.
        /// </summary>
        public bool entdet_EsEliminado { get; set; }
        /// <summary>
        /// Indica el identificador del usuario que creó el registro.
        /// </summary>
        public int entdet_UsuarioCrea { get; set; }
        /// <summary>
        /// Registra la fecha en que se creó el registro.
        /// </summary>
        public DateTime entdet_FechaCrea { get; set; }
        /// <summary>
        /// Indica el identificador del último usuario que modificó el registro.
        /// </summary>
        public int? entdet_UsuarioModifica { get; set; }
        /// <summary>
        /// Registra la última fecha en que se modificó el registro.
        /// </summary>
        public DateTime? entdet_FechaModifica { get; set; }

        public virtual tbEntradas ent { get; set; }
        public virtual tbUsuarios entdet_UsuarioCreaNavigation { get; set; }
        public virtual tbItems itm { get; set; }
    }
}