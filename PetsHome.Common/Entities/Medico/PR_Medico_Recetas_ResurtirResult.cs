namespace PetsHome.Common.Entities
{
    /// <summary>
    /// Resultado de PR_Medico_Recetas_Resurtir: patron CodeStatus/MessageStatus
    /// mas el conteo de resurtidos usados/autorizados tras la operacion.
    /// </summary>
    public partial class PR_Medico_Recetas_ResurtirResult
    {
        public int CodeStatus { get; set; }
        public string MessageStatus { get; set; }
        public int receta_ResurtidosUsados { get; set; }
        public int receta_ResurtidosAutorizados { get; set; }
    }
}
