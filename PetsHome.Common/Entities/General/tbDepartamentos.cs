﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace PetsHome.Common.Entities
{
    public partial class tbDepartamentos
    {
        public tbDepartamentos()
        {
            tbMunicipios = new HashSet<tbMunicipios>();
        }

        /// <summary>
        /// Identificador único de la tabla Departamentos.
        /// </summary>
        public int depto_Id { get; set; }
        /// <summary>
        /// Código administrativo único del departamento.
        /// </summary>
        public string depto_Codigo { get; set; }
        /// <summary>
        /// Nombre del departamento.
        /// </summary>
        public string depto_Descripcion { get; set; }
        /// <summary>
        /// Indica si el registro está desactivado permanentemente.
        /// </summary>
        public bool depto_EsEliminado { get; set; }
        /// <summary>
        /// Indica el identificador del usuario que creó el registro.
        /// </summary>
        public int depto_UsuarioCrea { get; set; }
        /// <summary>
        /// Registra la fecha en que se creó el registro.
        /// </summary>
        public DateTime depto_FechaCrea { get; set; }
        /// <summary>
        /// Indica el identificador del último usuario que modificó el registro.
        /// </summary>
        public int? depto_UsuarioModifica { get; set; }
        /// <summary>
        /// Registra la última fecha en que se modificó el registro.
        /// </summary>
        public DateTime? depto_FechaModifica { get; set; }

        public virtual tbUsuarios depto_UsuarioCreaNavigation { get; set; }
        public virtual ICollection<tbMunicipios> tbMunicipios { get; set; }
    }
}