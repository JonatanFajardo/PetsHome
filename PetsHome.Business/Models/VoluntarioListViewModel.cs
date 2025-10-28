namespace PetsHome.Business.Models
{
    /// <summary>
    /// View model utilizado para mostrar voluntarios en listados.
    /// </summary>
    public class VoluntarioListViewModel
    {
        public int vol_Id { get; set; }

        public int vol_HorasTrabajadas { get; set; }

        public string vol_Nombres { get; set; }

        public string per_Identidad { get; set; }
    }
}
