using System;

namespace PetsHome.Contracts.DTOs
{
    /// <summary>
    /// ado del procedimiento almacenado UDP_Acce_tbUsuarios_Login
    /// </summary>
    public class SeguridadUsuarios_Login
    {
        public int usu_Id { get; set; }
        public int Emp_Id { get; set; }
        public string usu_NombreUsuario { get; set; }
        public string usu_NombreCompleto { get; set; }
        public string per_PrimerNombre { get; set; }
        public string per_ApellidoPaterno { get; set; }
        public int? rol_Id { get; set; }
        public string rol_Descripcion { get; set; }
        public bool usu_Estado { get; set; }
        public string usu_ImagenPerfil { get; set; }
        public bool? usu_Logueado { get; set; }
    }
}
