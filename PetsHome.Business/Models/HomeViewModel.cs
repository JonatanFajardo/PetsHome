using System.Collections.Generic;

namespace PetsHome.Business.Models
{
    /// <summary>
    /// Modelo de vista para la página de inicio (Home).
    /// </summary>
    public class HomeViewModel
    {
        /// <summary>
        /// Obtiene o establece el total de mascotas registradas.
        /// </summary>
        public int TotalMascotasRegistradas { get; set; }

        /// <summary>
        /// Obtiene o establece el total de próximas citas médicas programadas.
        /// </summary>
        public int ProximasCitasMedicas { get; set; }

        /// <summary>
        /// Obtiene o establece el total de donaciones recibidas.
        /// </summary>
        public int DonacionesRecibidas { get; set; }

        /// <summary>
        /// Obtiene o establece el total de adopciones pendientes.
        /// </summary>
        public int AdopcionesPendientes { get; set; }

        /// <summary>
        /// Obtiene o establece la lista de últimas adopciones.
        /// </summary>
        public List<AdopcionViewModel> UltimasAdopciones { get; set; }

        public HomeViewModel()
        {
            UltimasAdopciones = new List<AdopcionViewModel>();
        }
    }
}
