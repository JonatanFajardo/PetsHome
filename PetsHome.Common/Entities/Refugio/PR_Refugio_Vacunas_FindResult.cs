﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetsHome.Common.Entities
{
    public partial class PR_Refugio_Vacunas_FindResult
    {
        public int vac_Id { get; set; }
        public string vac_Descripcion { get; set; }
        public bool vac_EsActivo { get; set; }
        public int vac_UsuarioCrea { get; set; }
        public DateTime vac_FechaCrea { get; set; }
        public int? vac_UsuarioModifica { get; set; }
        public DateTime? vac_FechaModifica { get; set; }
    }
}