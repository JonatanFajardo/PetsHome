﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetsHome.Common.Entities
{
    public partial class PR_Refugio_Solicitudes_DetailResult
    {
        public int sol_Id { get; set; }
        public string sol_Identidad { get; set; }
        public string sol_Nombres { get; set; }
        public string sol_Apellidos { get; set; }
        public string sol_Telefono { get; set; }
        public string sol_Correo { get; set; }
        public int masc_Id { get; set; }
        public byte[] masc_Imagen { get; set; }
        public string masc_Nombre { get; set; }
        public bool masc_EsAdoptado { get; set; }
        public string raza_Descripcion { get; set; }
        public string refg_Nombre { get; set; }
        public string UsuarioCreacion { get; set; }
        public DateTime sol_FechaCrea { get; set; }
        public string UsuarioModificacion { get; set; }
        public DateTime? sol_FechaModifica { get; set; }
    }
}
