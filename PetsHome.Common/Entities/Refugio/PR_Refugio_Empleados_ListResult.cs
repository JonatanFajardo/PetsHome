﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetsHome.Common.Entities
{
    public partial class PR_Refugio_Empleados_ListResult
    {
        public int emp_Id { get; set; }
        public string emp_Codigo { get; set; }
        public string Nombres { get; set; }
        public string per_Identidad { get; set; }
        public DateTime per_FechaNacimiento { get; set; }
        public string per_Domicilio { get; set; }
        public string per_Telefono { get; set; }
        public string per_Correo { get; set; }
        public string cag_Descripcion { get; set; }
        public string refg_Nombre { get; set; }
        public string EsActivo { get; set; }
    }
}
