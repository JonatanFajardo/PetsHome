﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetsHome.Common.Entities
{
    public partial class PR_Refugio_HistorialMedico_DetailResult
    {
        public int medic_Id { get; set; }
        public string masc_Nombre { get; set; }
        public string raza_Descripcion { get; set; }
        public bool medic_Esterilizacion { get; set; }
        public int com_Id { get; set; }
        public string com_Descripcion { get; set; }
        public string medic_SaludCuidado { get; set; }
        public string medic_InformacionAdicional { get; set; }
        public bool medic_EsEliminado { get; set; }
        public string UsuarioCreacion { get; set; }
        public DateTime medic_FechaCrea { get; set; }
        public string UsuarioModificacion { get; set; }
        public DateTime? medic_FechaModifica { get; set; }
    }
}