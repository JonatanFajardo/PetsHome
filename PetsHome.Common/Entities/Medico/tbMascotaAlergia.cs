using System;

namespace PetsHome.Common.Entities
{
    /// <summary>
    /// Alergia de una mascota (bandera roja clínica). Modelo adaptado de
    /// OpenVPM patient_allergies. "Corregir" = soft-delete (alrg_EsEliminado).
    /// </summary>
    public partial class tbMascotaAlergia
    {
        public int alrg_Id { get; set; }
        public int masc_Id { get; set; }
        public string alrg_Sustancia { get; set; }
        public string alrg_Reaccion { get; set; }
        public string alrg_Gravedad { get; set; }   // Leve | Media | Alta

        public bool alrg_EsEliminado { get; set; }
        public int alrg_UsuarioCrea { get; set; }
        public DateTime alrg_FechaCrea { get; set; }
        public int? alrg_UsuarioModifica { get; set; }
        public DateTime? alrg_FechaModifica { get; set; }
    }
}
