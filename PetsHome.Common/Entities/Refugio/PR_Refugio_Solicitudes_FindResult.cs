// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetsHome.Common.Entities
{
    public partial class PR_Refugio_Solicitudes_FindResult
    {
        public int sol_Id { get; set; }
        public string sol_Identidad { get; set; }
        public string sol_Nombres { get; set; }
        public string sol_Apellidos { get; set; }
        public string sol_Telefono { get; set; }
        public string sol_Correo { get; set; }
        public DateTime sol_Fecha { get; set; }
        public int masc_Id { get; set; }
        public string masc_Nombre { get; set; }
        public int sol_UsuarioCrea { get; set; }
        public string usuarioCrea { get; set; }
        public DateTime sol_FechaCrea { get; set; }
        public int? sol_UsuarioModifica { get; set; }
        public string usuarioModifica { get; set; }
        public DateTime? sol_FechaModifica { get; set; }
    }
}
