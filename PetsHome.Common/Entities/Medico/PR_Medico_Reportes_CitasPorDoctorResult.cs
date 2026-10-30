namespace PetsHome.Common.Entities
{
    /// <summary>
    /// Fila del desglose de citas por doctor en el periodo para /Reportes.
    /// (Doctor, Cantidad de citas).
    /// </summary>
    public class PR_Medico_Reportes_CitasPorDoctorResult
    {
        public string Doctor { get; set; }
        public int Cantidad { get; set; }
    }
}
