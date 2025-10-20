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
        /// Obtiene o establece el total de citas programadas para hoy.
        /// </summary>
        public int CitasHoy { get; set; }

        /// <summary>
        /// Obtiene o establece el total de medicamentos por vencer.
        /// </summary>
        public int MedicamentosPorVencer { get; set; }

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
