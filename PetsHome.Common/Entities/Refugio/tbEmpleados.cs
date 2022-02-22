﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PetsHome.Common.Entities
{
    public partial class tbEmpleados
    {
        /// <summary>
        /// Identificador único de la tabla Empleados.
        /// </summary>
        /// 
        public int emp_Id { get; set; }
        public string emp_Codigo { get; set; }
        public int per_Id { get; set; }
        public int refg_Id { get; set; }
        public int cag_Id { get; set; }
        public bool? emp_EsActivo { get; set; }

        public virtual tbEmpleadosCargos cag { get; set; }
        public virtual tbPersonas per { get; set; }
        public virtual tbRefugios refg { get; set; }
    }
}