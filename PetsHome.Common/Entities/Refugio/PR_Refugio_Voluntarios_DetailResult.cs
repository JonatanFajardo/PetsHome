﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetsHome.Common.Entities
{
    public partial class PR_Refugio_Voluntarios_DetailResult
    {
        public int vol_Id { get; set; }
        public int vol_HorasTrabajadas { get; set; }
        public string Nombres { get; set; }
        public string per_Identidad { get; set; }
        public DateTime per_FechaNacimiento { get; set; }
        public string per_Domicilio { get; set; }
        public string per_Telefono { get; set; }
        public string per_Correo { get; set; }
        public int? vol_Recurrente { get; set; }
        public int vol_UsuarioCrea { get; set; }
        public DateTime vol_FechaCrea { get; set; }
        public int? vol_UsuarioModifica { get; set; }
        public DateTime? vol_FechaModifica { get; set; }
    }
}
