﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetsHome.Common.Entities
{
    /// <summary>
    /// Entidad que representa la tabla.
    /// </summary>
    public partial class PR_Refugio_Vacunas_FindResult
    {
        
        /// <summary>
        /// Obtiene o establece el valor de la propiedad.
        /// </summary>
        public int vac_Id { get; set; }
        
        /// <summary>
        /// Obtiene o establece el valor de la propiedad.
        /// </summary>
        public string vac_Descripcion { get; set; }
        
        /// <summary>
        /// Obtiene o establece el valor de la propiedad.
        /// </summary>
        public bool vac_EsActivo { get; set; }
        
        /// <summary>
        /// Obtiene o establece el valor de la propiedad.
        /// </summary>
        public int vac_UsuarioCrea { get; set; }
        
        /// <summary>
        /// Obtiene o establece el valor de la propiedad.
        /// </summary>
        public string usuarioCrea { get; set; }
        
        /// <summary>
        /// Obtiene o establece el valor de la propiedad.
        /// </summary>
        public DateTime vac_FechaCrea { get; set; }
        
        /// <summary>
        /// Obtiene o establece el valor de la propiedad.
        /// </summary>
        public int? vac_UsuarioModifica { get; set; }
        
        /// <summary>
        /// Obtiene o establece el valor de la propiedad.
        /// </summary>
        public string usuarioModifica { get; set; }
        
        /// <summary>
        /// Obtiene o establece el valor de la propiedad.
        /// </summary>
        public DateTime? vac_FechaModifica { get; set; }
    }
}
