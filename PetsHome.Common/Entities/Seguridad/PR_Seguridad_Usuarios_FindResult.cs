namespace PetsHome.Common.Entities
{
    public class PR_Seguridad_Usuarios_FindResult
    {
        public int usu_Id { get; set; }
        public string Usu_Nombre { get; set; }
        public int Emp_Id { get; set; }
        public int Rol_Id { get; set; }
        public bool Usu_EsActivo { get; set; }
    }
}
