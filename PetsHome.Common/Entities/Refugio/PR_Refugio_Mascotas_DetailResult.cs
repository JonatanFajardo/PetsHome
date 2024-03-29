﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetsHome.Common.Entities
{
    public partial class PR_Refugio_Mascotas_DetailResult
    {
        public int masc_Id { get; set; }
        public byte[] masc_Imagen { get; set; }
        public string masc_Nombre { get; set; }
        public string raza_Descripcion { get; set; }
        public int? masc_Edad { get; set; }
        public string masc_Sexo { get; set; }
        public decimal masc_Peso { get; set; }
        public string masc_Color { get; set; }
        public string masc_Historia { get; set; }
        public string refg_Nombre { get; set; }
        public string proc_Descripcion { get; set; }
        public bool masc_EsAdoptado { get; set; }
        public bool masc_EsReservado { get; set; }
        public string UsuarioCreacion { get; set; }
        public DateTime masc_FechaCrea { get; set; }
        public string UsuarioModificacion { get; set; }
        public DateTime? masc_FechaModifica { get; set; }
    }
}
