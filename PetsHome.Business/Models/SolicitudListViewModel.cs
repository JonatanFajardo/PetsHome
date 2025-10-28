namespace PetsHome.Business.Models
{
    /// <summary>
    /// View model utilizado para mostrar solicitudes en listados.
    /// </summary>
    public class SolicitudListViewModel
    {
        public int sol_Id { get; set; }

        public string sol_Identidad { get; set; }

        public string sol_Nombres { get; set; }

        public string masc_Nombre { get; set; }

        public string sol_Correo { get; set; }
    }
}
