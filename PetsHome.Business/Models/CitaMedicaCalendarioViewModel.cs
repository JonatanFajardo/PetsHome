namespace PetsHome.Business.Models
{
    /// <summary>
    /// Datos de una cita médica para el calendario (JSON API).
    /// Los nombres de propiedad coinciden con los que espera el JS del calendario.
    /// </summary>
    public class CitaMedicaCalendarioViewModel
    {
        public int id { get; set; }
        /// <summary>Fecha en formato yyyy-MM-dd</summary>
        public string date { get; set; }
        /// <summary>Hora en formato HH:mm</summary>
        public string time { get; set; }
        /// <summary>Duración en minutos (30/60/90 según tipo)</summary>
        public int dur { get; set; }
        /// <summary>Nombre de la mascota</summary>
        public string pet { get; set; }
        /// <summary>Motivo de consulta (línea descriptiva en tooltip)</summary>
        public string owner { get; set; }
        /// <summary>Tipo de consulta (Emergencia, Vacunación, Control, Cirugía, Consulta General)</summary>
        public string type { get; set; }
    }
}
