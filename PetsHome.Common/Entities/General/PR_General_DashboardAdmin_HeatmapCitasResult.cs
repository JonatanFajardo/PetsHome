namespace PetsHome.Common.Entities
{
    public class PR_General_DashboardAdmin_HeatmapCitasResult
    {
        public int DiaSemana { get; set; }  // 1=Dom … 7=Sáb (DATEPART WEEKDAY SQL Server)
        public int Hora      { get; set; }
        public int Cantidad  { get; set; }
    }
}
