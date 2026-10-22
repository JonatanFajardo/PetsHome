using System;

namespace PetsHome.Common.Entities
{
    public partial class PR_Veterinaria_CargoDispensacion_ListPendientesResult
    {
        public int Fila { get; set; }
        public int cargo_Id { get; set; }
        public int receta_Id { get; set; }
        public int masc_Id { get; set; }
        public int cli_Id { get; set; }
        public int? cita_Id { get; set; }
        public int itm_Id { get; set; }
        public int cargo_Cantidad { get; set; }
        public string cargo_DescripcionSnap { get; set; }
        public decimal cargo_PrecioUnitSnap { get; set; }
        public decimal cargo_Total { get; set; }
        public string cargo_Estado { get; set; }
        public DateTime cargo_FechaCrea { get; set; }
        public string MascotaNombre { get; set; }
    }
}
