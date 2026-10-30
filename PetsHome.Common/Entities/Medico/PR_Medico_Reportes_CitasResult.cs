namespace PetsHome.Common.Entities
{
    /// <summary>
    /// Resumen (una sola fila) del reporte de citas del periodo para /Reportes.
    /// Totales por estado: Total, Atendidas, NoShow (no asistio), Canceladas.
    /// </summary>
    public class PR_Medico_Reportes_CitasResult
    {
        public int Total { get; set; }
        public int Atendidas { get; set; }
        public int NoShow { get; set; }
        public int Canceladas { get; set; }
    }
}
