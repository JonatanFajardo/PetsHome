﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetsHome.Common.Entities
{
    public partial class PR_Inventario_Items_DetailResult
    {
        public int itm_Id { get; set; }
        public string itm_Codigo { get; set; }
        public string itm_Descripcion { get; set; }
        public string itm_Precio { get; set; }
        public string UsuarioCreacion { get; set; }
        public DateTime itm_FechaCrea { get; set; }
        public string UsuarioModificacion { get; set; }
        public DateTime? itm_FechaModifica { get; set; }
    }
}