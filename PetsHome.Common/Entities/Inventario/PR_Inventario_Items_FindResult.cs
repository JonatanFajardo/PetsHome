﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetsHome.Common.Entities
{
    public partial class PR_Inventario_Items_FindResult
    {
        public int itm_Id { get; set; }
        public string itm_Codigo { get; set; }
        public string itm_Descripcion { get; set; }
        public int cat_Id { get; set; }
        public string cat_Descripcion { get; set; }
        public decimal itm_Precio { get; set; }
        public int itm_UsuarioCrea { get; set; }
        public string usuarioCrea { get; set; }
        public DateTime itm_FechaCrea { get; set; }
        public int? itm_UsuarioModifica { get; set; }
        public string usuarioModifica { get; set; }
        public DateTime? itm_FechaModifica { get; set; }
    }
}
