namespace PetsHome.Business.Models
{
    /// <summary>
    /// View model utilizado para mostrar empleados en listados.
    /// </summary>
    public class EmpleadoListViewModel
    {
        public int emp_Id { get; set; }

        public string emp_Codigo { get; set; }

        public string emp_Nombres { get; set; }

        public string cag_Descripcion { get; set; }

        public string refg_Nombre { get; set; }

        public string esActivo { get; set; }

        public bool emp_EsActivo => string.Equals(esActivo, "Activo", System.StringComparison.OrdinalIgnoreCase);
    }
}
