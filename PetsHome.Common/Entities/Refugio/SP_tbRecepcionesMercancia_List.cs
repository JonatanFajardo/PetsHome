using System;
using System.Collections.Generic;
using System.Text;

namespace PetsHome.Common.Entities
{
    public class SP_tbRecepcionesMercancia_List
    {
        public int recep_Id { get; set; }
        public string recep_Descripcion { get; set; }
        public DateTime recep_Fecha { get; set; }
        public string recep_TipoRecepcion { get; set; }
        public string TipoRecepcionDescripcion { get; set; }
        public string recep_NumeroDocumento { get; set; }
        public int refg_Id { get; set; }
        public string refg_Nombre { get; set; }
        public int CantidadItems { get; set; }
        public decimal TotalProductos { get; set; }
    }
}
