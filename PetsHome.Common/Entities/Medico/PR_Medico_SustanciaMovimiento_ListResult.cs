using System;

namespace PetsHome.Common.Entities
{
    public partial class PR_Medico_SustanciaMovimiento_ListResult
    {
        public int Fila { get; set; }
        public int scm_Id { get; set; }
        public int itm_Id { get; set; }
        public string itm_Codigo { get; set; }
        public string Farmaco { get; set; }
        public string scm_Tipo { get; set; }
        public decimal scm_Cantidad { get; set; }
        public int? masc_Id { get; set; }
        public string Mascota { get; set; }
        public int? scm_TestigoId { get; set; }
        public string Testigo { get; set; }
        public string scm_Lote { get; set; }
        public string scm_DeaSchedule { get; set; }
        public string scm_Notas { get; set; }
        public DateTime scm_Fecha { get; set; }
        public int scm_UsuarioCrea { get; set; }
        public string RegistradoPor { get; set; }
    }
}
