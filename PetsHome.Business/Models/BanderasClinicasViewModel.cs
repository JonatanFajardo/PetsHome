using System.Collections.Generic;

namespace PetsHome.Business.Models
{
    /// <summary>
    /// Contenedor de las banderas rojas clínicas de una mascota
    /// (alergias + padecimientos) para el alert bar reutilizable.
    /// </summary>
    public class BanderasClinicasViewModel
    {
        public int masc_Id { get; set; }
        public List<MascotaAlergiaViewModel> Alergias { get; set; } = new List<MascotaAlergiaViewModel>();
        public List<MascotaPadecimientoViewModel> Padecimientos { get; set; } = new List<MascotaPadecimientoViewModel>();
    }
}
