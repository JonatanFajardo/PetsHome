using System;
using System.ComponentModel.DataAnnotations;

namespace PetsHome.Business.Models
{
    /// <summary>
    /// View model utilizado para mostrar el detalle de un empleado.
    /// </summary>
    public class EmpleadoDetailsViewModel
    {
        [Key]
        public int emp_Id { get; set; }

        public string emp_Codigo { get; set; }

        public string per_PrimerNombre { get; set; }

        public string per_SegundoNombre { get; set; }

        public string per_ApellidoPaterno { get; set; }

        public string per_ApellidoMaterno { get; set; }

        public string per_Identidad { get; set; }

        public DateTime per_FechaNacimiento { get; set; }

        public string per_Domicilio { get; set; }

        public string per_Telefono { get; set; }

        public string per_Correo { get; set; }

        public string cag_Descripcion { get; set; }

        public string refg_Nombre { get; set; }

        public string esActivo { get; set; }

        public string UsuarioCreacion { get; set; }

        public DateTime per_FechaCrea { get; set; }

        public string UsuarioModificacion { get; set; }

        public DateTime? per_FechaModifica { get; set; }

        public int emp_UsuarioCrea { get; set; }

        public int? emp_UsuarioModifica { get; set; }

        public DateTime emp_FechaCrea { get; set; }

        public DateTime? emp_FechaModifica { get; set; }

        public bool emp_EsActivo => string.Equals(esActivo, "Activo", StringComparison.OrdinalIgnoreCase);

        public string emp_Nombres
        {
            get
            {
                return string.Join(" ", new[]
                {
                    per_PrimerNombre,
                    per_SegundoNombre,
                    per_ApellidoPaterno,
                    per_ApellidoMaterno
                }).Replace("  ", " ").Trim();
            }
        }
    }
}
