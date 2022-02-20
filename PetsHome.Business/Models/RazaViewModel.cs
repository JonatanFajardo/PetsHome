using System;
using System.ComponentModel.DataAnnotations;
namespace PetsHome.Business.Models
{
    public class RazaViewModel
    {
        public long? Fila { get; set; }
        [Display(Name = "Id raza")]
        public int? raza_Id { get; set; }

        [Display(Name = "Descripción")]
        //[Required(AllowEmptyStrings = false, ErrorMessage = "El campo descripción es requerido")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(50)]
        [Espacios(ErrorMessage = "bebesita")]
        //[RegularExpression(@"[^s]+", ErrorMessage = "El campo {0} no admite espacios")]
        //[RazaExist(ErrorMessage = "Ya existe")]
        public string raza_Descripcion { get; set; }

        public int? raza_UsuarioCrea { get; set; }

        [Display(Name = "Usuario creación")]
        public string? raza_NombreUsuarioCrea { get; set; }
        public DateTime? raza_FechaCrea { get; set; }
        public int? raza_UsuarioModifica { get; set; }

        [Display(Name = "Usuario modificación")]
        public string? raza_NombreUsuarioModifica { get; set; }

        [Display(Name = "Fecha modificación")]
        public DateTime? raza_FechaModifica { get; set; }

        //Indica si el formulario se esta editando.
        public Boolean isEdit
        {
            get
            {
                if (this.raza_Id == 0)
                    return false;
                else
                    return true;
            }
        }
    }

    public class EspaciosAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {

            //string cc = value.ToString();
            //char chaar = cc[0];
            //if (chaar == ' ')
            //{
            //return new ValidationResult("golaaa");
            //}
            //    return ValidationResult.Success;

            string cc = value.ToString();
            char chaar = cc[0];
            if (chaar == ' ')
            {
                return false;
            }
            return true;
        }
    }

    //public class RazaExistAttribute : ValidationAttribute
    //{
    //    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    //    {
    //        RazaRepository razaRepository = new RazaRepository();
    //        //int priod_Id = (int)value;
    //        string raza_Descripcion = (string)value;
    //        var result = razaRepository.Validation(raza_Descripcion);
    //        if (result != null)
    //        {
    //            //if (result.priod_Id == priod_Id)
    //            //{
    //            //    return ValidationResult.Success;
    //            //}
    //            //else
    //            //{
    //            return new ValidationResult("Registro existente");
    //            //}
    //        }
    //        return ValidationResult.Success;
    //    }
    //}

    //public class RazaExistAttribute : ValidationAttribute
    //{
    //    public override bool IsValid(object value/*, ValidationContext validationContext*/)
    //    {
    //        RazaRepository razaRepository = new RazaRepository();
    //        //int priod_Id = (int)value;
    //        string raza_Descripcion = (string)value;
    //        var result = razaRepository.Validation(raza_Descripcion);
    //        if (result != null)
    //        {
    //            //if (result.priod_Id == priod_Id)
    //            //{
    //            //    return ValidationResult.Success;
    //            //}
    //            //else
    //            //{
    //            return false;
    //            //return new ValidationResult("Registro existente");
    //            //}
    //        }
    //        return true;
    //        //return ValidationResult.Success;
    //    }
    //}
}