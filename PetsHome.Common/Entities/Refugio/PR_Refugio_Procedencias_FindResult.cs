﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetsHome.Common.Entities
{
    public partial class PR_Refugio_Procedencias_FindResult
    {
        public int proc_Id { get; set; }
        public string proc_Descripcion { get; set; }
        public int proc_UsuarioCrea { get; set; }
        public string usuarioCrea { get; set; }
        public DateTime proc_FechaCrea { get; set; }
        public int? proc_UsuarioModifica { get; set; }
        public string usuarioModifica { get; set; }
        public DateTime? proc_FechaModifica { get; set; }
    }
}
