using System.Collections.Generic;

namespace PetsHome.Business.Models
{
    public class LandingAnimalesViewModel
    {
        public List<MascotaListViewModel> Mascotas { get; set; } = new List<MascotaListViewModel>();
        public string FiltroActivo { get; set; } = "todos";
    }
}
