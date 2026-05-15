namespace PetsHome.Common.Entities
{
    public class PR_General_DashboardAdmin_TendenciasResult
    {
        public string  EtiquetaMes { get; set; }
        public int     NumMes      { get; set; }
        public int     NumAnio     { get; set; }
        public int     Ingresos    { get; set; }
        public int     Adopciones  { get; set; }
        public decimal Donaciones  { get; set; }
    }
}
