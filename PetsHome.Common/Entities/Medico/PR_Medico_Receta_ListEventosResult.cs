using System;

namespace PetsHome.Common.Entities
{
    /// <summary>
    /// Resultado de PR_Medico_Receta_ListEventos: una fila por evento de la
    /// bitacora del ciclo de vida de la receta (creada, resurtidos, completada,
    /// cancelada, vencida), ordenada por fecha.
    /// </summary>
    public partial class PR_Medico_Receta_ListEventosResult
    {
        public int evento_Id { get; set; }
        public int receta_Id { get; set; }
        public string evento_Tipo { get; set; }
        public string evento_Nota { get; set; }
        public DateTime evento_Fecha { get; set; }
        public int evento_Usuario { get; set; }
        public string UsuarioNombre { get; set; }
    }
}
