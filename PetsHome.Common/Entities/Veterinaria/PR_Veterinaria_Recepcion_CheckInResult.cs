namespace PetsHome.Common.Entities
{
    /// <summary>
    /// Resultado de [Veterinaria].[PR_Veterinaria_Recepcion_CheckIn].
    /// En exito el SP devuelve solo cita_Id; en error devuelve CodeStatus/MessageStatus
    /// (cita_Id null).
    /// </summary>
    public class PR_Veterinaria_Recepcion_CheckInResult
    {
        public int? cita_Id { get; set; }
        public int CodeStatus { get; set; }
        public string MessageStatus { get; set; }
    }
}
