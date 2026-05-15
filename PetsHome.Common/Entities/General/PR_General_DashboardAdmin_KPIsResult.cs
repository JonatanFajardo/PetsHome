namespace PetsHome.Common.Entities
{
    public class PR_General_DashboardAdmin_KPIsResult
    {
        public int     TotalMascotas                  { get; set; }
        public int     MascotasMesActual              { get; set; }
        public int     MascotasMesAnterior            { get; set; }
        public int     AdopcionesPendientes           { get; set; }
        public int     AdopcionesPendientesAnterior   { get; set; }
        public int     CitasHoy                       { get; set; }
        public int     AlertasActivas                 { get; set; }
        public decimal DonacionesMesActual            { get; set; }
        public decimal DonacionesMesAnterior          { get; set; }

        public int DeltaMascotas =>
            MascotasMesActual - MascotasMesAnterior;

        public int DeltaAdopciones =>
            AdopcionesPendientes - AdopcionesPendientesAnterior;

        public decimal DeltaDonacionesPct =>
            DonacionesMesAnterior == 0 ? 0
            : ((DonacionesMesActual - DonacionesMesAnterior) / DonacionesMesAnterior) * 100;
    }
}
