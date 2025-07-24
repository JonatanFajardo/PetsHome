using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PetsHome.Business.Models
{
    public class RolPermisosViewModel
    {
        public int Rol_Id { get; set; }

        [Display(Name = "Rol")]
        public string Rol_Descripcion { get; set; }

        [Display(Name = "Módulos y Permisos")]
        public List<ModuloViewModel> Modulos { get; set; }

        public RolPermisosViewModel()
        {
            Modulos = new List<ModuloViewModel>();
        }
    }
}