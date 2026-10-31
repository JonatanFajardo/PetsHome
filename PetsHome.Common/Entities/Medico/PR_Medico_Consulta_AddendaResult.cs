using System;

namespace PetsHome.Common.Entities
{
    /// <summary>
    /// Resultado del SP [Medico].[PR_Medico_Consulta_ListAddenda].
    /// Una addenda = correccion posterior a la nota clinica (append-only).
    /// </summary>
    public class PR_Medico_Consulta_AddendaResult
    {
        public int add_Id { get; set; }
        public int cita_Id { get; set; }
        public string add_Texto { get; set; }
        public DateTime add_Fecha { get; set; }
        public int? add_Usuario { get; set; }
        public string AutorNombre { get; set; }
    }
}
