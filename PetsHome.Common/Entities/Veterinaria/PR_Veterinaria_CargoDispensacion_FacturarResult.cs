namespace PetsHome.Common.Entities
{
    /// <summary>Resultado del SP FacturarCliente: id de la factura creada + estado.</summary>
    public class PR_Veterinaria_CargoDispensacion_FacturarResult
    {
        public int? fact_Id { get; set; }
        public int CodeStatus { get; set; }
        public string MessageStatus { get; set; }
    }
}
