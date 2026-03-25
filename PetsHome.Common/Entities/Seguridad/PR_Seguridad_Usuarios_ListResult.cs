namespace PetsHome.Common.Entities
{
    public class PR_Seguridad_Usuarios_ListResult
    {
        public int Fila { get; set; }
        public int usu_Id { get; set; }
        public string Usu_Nombre { get; set; }
        public string rol_Descripcion { get; set; }
        public bool Usu_EsActivo { get; set; }
    }
}
