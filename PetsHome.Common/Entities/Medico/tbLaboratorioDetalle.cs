using System;

namespace PetsHome.Common.Entities
{
    public partial class tbLaboratorioDetalle
    {
        public int labd_Id { get; set; }
        public int lab_Id { get; set; }
        public string labd_Parametro { get; set; }
        public string labd_Valor { get; set; }
        public string labd_Unidad { get; set; }
        public decimal? labd_RangoMin { get; set; }
        public decimal? labd_RangoMax { get; set; }
        public bool labd_FueraRango { get; set; }
        public bool labd_EsEliminado { get; set; }
        public int labd_UsuarioCrea { get; set; }
        public DateTime labd_FechaCrea { get; set; }
        public int? labd_UsuarioModifica { get; set; }
        public DateTime? labd_FechaModifica { get; set; }
    }
}
