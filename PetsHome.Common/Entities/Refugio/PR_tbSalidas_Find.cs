using System;
using System.Collections.Generic;
using System.Text;

namespace PetsHome.Common.Entities
{
    public class PR_tbSalidas_Find
    {
        public int sal_Id { get; set; }
        public string sal_Descripcion { get; set; }
        public DateTime sal_Fecha { get; set; }
        public string sal_TipoSalida { get; set; }
        public string refg_Nombre { get; set; }
    }
}
