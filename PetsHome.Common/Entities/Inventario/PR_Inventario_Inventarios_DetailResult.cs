﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetsHome.Common.Entities
{
    public partial class PR_Inventario_Inventarios_DetailResult
    {
        public int inv_Id { get; set; }
        public DateTime inv_Fecha { get; set; }
        public int refg_Id { get; set; }
        public string UsuarioCreacion { get; set; }
        public DateTime inv_FechaCrea { get; set; }
        public string UsuarioModificacion { get; set; }
        public DateTime? inv_FechaModifica { get; set; }
    }
}