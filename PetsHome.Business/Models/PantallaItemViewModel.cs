namespace PetsHome.Business.Models
{
    public class PantallaItemViewModel
    {
        public int pan_Id { get; set; }
        public string pan_Descripcion { get; set; }
        public string pan_Grupo { get; set; }
    }

    public class PantallaIdViewModel
    {
        public int pan_Id { get; set; }
    }

    public class PantallaPermisoViewModel
    {
        public int pan_Id { get; set; }
        public bool ropan_Consultar { get; set; }
        public bool ropan_Insertar { get; set; }
        public bool ropan_Editar { get; set; }
        public bool ropan_Eliminar { get; set; }
    }
}
