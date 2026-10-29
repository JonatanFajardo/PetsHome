namespace PetsHome.Common.Entities
{
    /// <summary>Resultado de registrar un pago: id del pago, nuevo saldo y estado de la factura.</summary>
    public class PR_Veterinaria_Pago_RegistrarResult
    {
        public int? pago_Id { get; set; }
        public decimal nuevo_saldo { get; set; }
        public string fact_Estado { get; set; }
        public int CodeStatus { get; set; }
        public string MessageStatus { get; set; }
    }
}
