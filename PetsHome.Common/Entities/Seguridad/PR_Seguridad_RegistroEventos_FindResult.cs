﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetsHome.Common.Entities
{
    public partial class PR_Seguridad_RegistroEventos_FindResult
    {
        public long Evt_Id { get; set; }
        public string Tipo_Evento { get; set; }
        public int? Evt_Usu_Id { get; set; }
        public string Evt_Detalles { get; set; }
        public string Evt_UserAgent { get; set; }
        public string Evt_DireccionIP { get; set; }
        public string Evt_EstadoAnterior { get; set; }
        public string Evt_NuevoEstado { get; set; }
        public DateTime? Evt_FechaCreacion { get; set; }
    }
}