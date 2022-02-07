﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace PetsHome.Common.Entities
{
    public partial class tbItems
    {
        public tbItems()
        {
            tbEntradasDetalles = new HashSet<tbEntradasDetalles>();
        }

        /// <summary>
        /// Identificador único de la tabla Items.
        /// </summary>
        public int itm_Id { get; set; }
        /// <summary>
        /// Codigo que identifica al producto.
        /// </summary>
        public string itm_Codigo { get; set; }
        /// <summary>
        /// Nombre del producto.
        /// </summary>
        public string itm_Descripcion { get; set; }
        /// <summary>
        /// Este es el ID del departamento que hace referencia al primary key de la tabla tbCategorias.
        /// </summary>
        public int cat_Id { get; set; }
        /// <summary>
        /// Precio unitario del producto.
        /// </summary>
        public decimal itm_Precio { get; set; }
        /// <summary>
        /// Indica si el registro está desactivado permanentemente.
        /// </summary>
        public bool itm_EsEliminado { get; set; }
        public int? itm_UsuarioCrea { get; set; }
        /// <summary>
        /// Registra la fecha en que se creó el registro.
        /// </summary>
        public DateTime? itm_FechaCrea { get; set; }
        /// <summary>
        /// Indica el identificador del último usuario que modificó el registro.
        /// </summary>
        public int? itm_UsuarioModifica { get; set; }
        /// <summary>
        /// Registra la última fecha en que se modificó el registro.
        /// </summary>
        public DateTime? itm_FechaModifica { get; set; }

        public virtual tbCategorias cat { get; set; }
        public virtual ICollection<tbEntradasDetalles> tbEntradasDetalles { get; set; }
    }
}