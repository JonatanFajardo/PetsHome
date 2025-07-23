using System;
using System.Collections.Generic;
using System.Text;

namespace PetsHome.Common.Entities
{
    public class PR_tbSalidas_Detail
    {
        public int sal_Id { get; set; }
        public string sal_Descripcion { get; set; }
        public string sal_TipoSalida { get; set; }
        public int refg_Id { get; set; }
        public string refg_Nombre { get; set; }
        public DateTime sal_Fecha { get; set; }
        public bool sal_EsEliminado { get; set; }
        public int sal_UsuarioCrea { get; set; }
        public string UsuarioCreacion { get; set; }
        public DateTime sal_FechaCrea { get; set; }
        public int? sal_UsuarioModifica { get; set; }
        public string? UsuarioModificacion { get; set; }
        public DateTime? sal_FechaModifica { get; set; }
    }
}
