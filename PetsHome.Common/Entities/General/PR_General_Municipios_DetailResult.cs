﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetsHome.Common.Entities
{
    public partial class PR_General_Municipios_DetailResult
    {
        public int mpio_Id { get; set; }
        public string mpio_Codigo { get; set; }
        public string mpio_Descripcion { get; set; }
        public string codigoDept { get; set; }
        public string codigo { get; set; }
        public int mpio_UsuarioCrea { get; set; }
        public DateTime mpio_FechaCrea { get; set; }
        public int? mpio_UsuarioModifica { get; set; }
        public DateTime? mpio_FechaModifica { get; set; }
    }
}