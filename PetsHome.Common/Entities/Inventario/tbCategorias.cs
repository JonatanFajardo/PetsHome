﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace PetsHome.Common.Entities
{
    public partial class tbCategorias
    {
        public tbCategorias()
        {
            tbItems = new HashSet<tbItems>();
        }

        /// <summary>
        /// Identificador único de la tabla Categorias.
        /// </summary>
        public int cat_Id { get; set; }
        public string cat_Descripcion { get; set; }
        /// <summary>
        /// Indica si el registro está desactivado permanentemente.
        /// </summary>
        public bool cat_EsEliminado { get; set; }
        /// <summary>
        /// Indica el identificador del usuario que creó el registro.
        /// </summary>
        public int cat_UsuarioCrea { get; set; }
        /// <summary>
        /// Registra la fecha en que se creó el registro.
        /// </summary>
        public DateTime cat_FechaCrea { get; set; }
        /// <summary>
        /// Indica el identificador del último usuario que modificó el registro.
        /// </summary>
        public int? cat_UsuarioModifica { get; set; }
        /// <summary>
        /// Registra la última fecha en que se modificó el registro.
        /// </summary>
        public DateTime? cat_FechaModifica { get; set; }

        public virtual tbUsuarios cat_UsuarioCreaNavigation { get; set; }
        public virtual ICollection<tbItems> tbItems { get; set; }
    }
}