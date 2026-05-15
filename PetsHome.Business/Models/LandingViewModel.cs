using System.Collections.Generic;

namespace PetsHome.Business.Models
{
    public class LandingViewModel
    {
        public List<MascotaListViewModel> Mascotas { get; set; } = new List<MascotaListViewModel>();
        public List<EventoViewModel> Eventos { get; set; } = new List<EventoViewModel>();
        public int TotalMascotas { get; set; }
        public int TotalAdoptados { get; set; }
        public int TotalDisponibles { get; set; }
    }
}
