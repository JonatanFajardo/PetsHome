namespace PetsHome.Common.Entities
{
    /// <summary>
    /// Servicios para el POS de facturacion. Lleva el flag de exento para que
    /// la pantalla sepa que lineas NO causan impuesto.
    /// </summary>
    public class PR_Veterinaria_Servicio_DropdownFacturaResult
    {
        public int serv_Id { get; set; }
        public string serv_Nombre { get; set; }
        public decimal serv_Precio { get; set; }
        public string serv_Categoria { get; set; }
        public bool serv_Exento { get; set; }
    }
}
