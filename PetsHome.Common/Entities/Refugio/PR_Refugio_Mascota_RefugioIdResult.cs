namespace PetsHome.Common.Entities
{
    /// <summary>
    /// Resultado de [Refugio].[PR_Refugio_Mascota_RefugioId]:
    /// resuelve el refugio (y nombre) de una mascota para operaciones de inventario.
    /// </summary>
    public class PR_Refugio_Mascota_RefugioIdResult
    {
        public int refg_Id { get; set; }
        public string masc_Nombre { get; set; }

        /// <summary>NULL = paciente interno (refugio) | con valor = paciente de cliente externo.</summary>
        public int? cli_Id { get; set; }
    }
}
