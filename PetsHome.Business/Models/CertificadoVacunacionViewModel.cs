using System;
using System.Collections.Generic;

namespace PetsHome.Business.Models
{
    /// <summary>
    /// Modelo del certificado de vacunacion imprimible de una mascota.
    /// Contrato consumido por Views/PerfilMedico/Certificado.cshtml.
    /// </summary>
    public class CertificadoVacunacionViewModel
    {
        public string MascotaNombre { get; set; }
        public string DuenoNombre { get; set; }
        public string Especie { get; set; }
        public string Raza { get; set; }
        public string Sexo { get; set; }
        public DateTime FechaEmision { get; set; } = DateTime.Now;

        public List<VacunaCertificadoLinea> Vacunas { get; set; }
            = new List<VacunaCertificadoLinea>();
    }

    /// <summary>
    /// Una fila del historial de vacunacion dentro del certificado.
    /// </summary>
    public class VacunaCertificadoLinea
    {
        public string VacunaNombre { get; set; }
        public DateTime? FechaAplicada { get; set; }
        public DateTime? ProximoRefuerzo { get; set; }
        // Valores: "Vigente"/"Al dia", "Proxima", "Vencida".
        public string Estado { get; set; }
    }
}
