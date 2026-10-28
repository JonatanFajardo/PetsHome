using System;

namespace PetsHome.Common.Entities
{
    /// <summary>
    /// Movimiento de la bitacora de sustancias controladas.
    /// Tipos (sin tildes): Recibido | Administrado | Desechado | Devuelto.
    /// </summary>
    public partial class tbSustanciaMovimiento
    {
        public int scm_Id { get; set; }
        public int itm_Id { get; set; }
        public string scm_Tipo { get; set; }
        public decimal scm_Cantidad { get; set; }
        public int? masc_Id { get; set; }
        public int? scm_TestigoId { get; set; }
        public string scm_Lote { get; set; }
        public string scm_DeaSchedule { get; set; }
        public string scm_Notas { get; set; }
        public DateTime scm_Fecha { get; set; }
        public bool scm_EsEliminado { get; set; }
        public int scm_UsuarioCrea { get; set; }
        public DateTime scm_FechaCrea { get; set; }
        public int? scm_UsuarioModifica { get; set; }
        public DateTime? scm_FechaModifica { get; set; }
    }
}
