﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace PetsHome.Common.Entities
{
    public partial class tbComportamientos
    {
        public tbComportamientos()
        {
            tbHistorialMedico = new HashSet<tbHistorialMedico>();
        }

        public int com_Id { get; set; }
        public string com_Descripcion { get; set; }
        public bool com_EsEliminado { get; set; }
        public int com_UsuarioCrea { get; set; }
        public DateTime com_FechaCrea { get; set; }
        public int? com_UsuarioModifica { get; set; }
        public DateTime? com_FechaModifica { get; set; }

        public virtual ICollection<tbHistorialMedico> tbHistorialMedico { get; set; }
    }
}