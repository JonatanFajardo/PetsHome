namespace PetsHome.Common.Entities
{
    /// <summary>
    /// Resultado del SP [Medico].[PR_Medico_Sugerencia_PorTexto].
    /// Regla clinica cuya palabra clave aparece en el texto del diagnostico.
    /// </summary>
    public class PR_Medico_Sugerencia_PorTextoResult
    {
        /// <summary>'medicamento' | 'vacuna'.</summary>
        public string sug_TipoSugerencia { get; set; }

        /// <summary>Nombre a sugerir (se resuelve contra inventario / catalogo de vacunas).</summary>
        public string sug_Termino { get; set; }
    }
}
