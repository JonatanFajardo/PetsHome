using System;

namespace PetsHome.Business.Models
{
    public class LotePorItemViewModel
    {
        public int lot_Id { get; set; }
        public int refg_Id { get; set; }
        public string refg_Nombre { get; set; }
        public string lot_Numero { get; set; }
        public DateTime? lot_Vencimiento { get; set; }
        public int lot_CantidadActual { get; set; }
        public int? DiasRestantes { get; set; }
    }
}
