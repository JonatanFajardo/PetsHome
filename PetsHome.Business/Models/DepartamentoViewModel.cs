using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace PetsHome.Business.Models
{
    //public class DepartamentoViewModel
    //{
    //    public EditarDepartamento EditarDepartamento { get; set; }
    //    public MunicipioViewModel EditarMunicipio { get; set; }
    //    public DepartamentoViewModel()
    //    {
    //        EditarDepartamento = new EditarDepartamento();
    //        EditarMunicipio = new MunicipioViewModel();
    //    }
    //}


    public class DepartamentoViewModel
    {
        [Key]
        [Display(Name = "Id Departamentos")]
        public int depto_Id { get; set; }

        [Display(Name = "Código")]
        [Required(ErrorMessage = "El campo código es requerido")]
        [StringLength(2, MinimumLength = 2, ErrorMessage = "El código debe tener {1} caracteres")]
        [RegularExpression(@"^\d*$", ErrorMessage = "Solo se permiten números")]
        [Remote(action: "ExistDepartamento", controller: "Departamentos", AdditionalFields = nameof(depto_Id))]
        public string depto_Codigo { get; set; }

        [Display(Name = "Departamento")]
        [Required(ErrorMessage = "El campo departamento es requerido")]
        [StringLength(100, ErrorMessage = "El departamento debe tener un máximo de {1} caracteres")]
        [Remote(action: "DepartamentosDescripcionExist", controller: "Departamentos", AdditionalFields = nameof(depto_Id))]
        public string depto_Descripcion { get; set; }
        [Display(Name = "Usuario creación")]
        public int? depto_UsuarioCrea { get; set; }
        [Display(Name = "Usuario creación")]
        public string UsuarioCreacion { get; set; }
        [Display(Name = "Fecha creación")]
        public DateTime? depto_FechaCrea { get; set; }
        [Display(Name = "Usuario modificación")]
        public int? depto_UsuarioModifica { get; set; }
        [Display(Name = "Usuario modificación")]
        public string UsuarioModificacion { get; set; }
        [Display(Name = "Fecha modificación")]
        public DateTime? depto_FechaModifica { get; set; }

        public List<MunicipioViewModel> ListadoMunicipios { get; set; }


        //Indica si el formulario se esta editando.
        public Boolean isEdit
        {
            get
            {
                if (this.depto_Id == 0)
                    return false;
                else
                    return true;
            }
        }
    }




    //public partial class MunicipioViewModel{}


}
