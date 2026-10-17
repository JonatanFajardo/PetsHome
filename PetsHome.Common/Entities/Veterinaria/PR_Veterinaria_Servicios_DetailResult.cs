    using System;

    namespace PetsHome.Common.Entities
    {
        public partial class PR_Veterinaria_Servicios_DetailResult
        {
        public int serv_Id { get; set; }
public string serv_Codigo { get; set; }
public string serv_Nombre { get; set; }
public string serv_Descripcion { get; set; }
public decimal serv_Precio { get; set; }
public string UsuarioCreacion { get; set; }
public DateTime serv_FechaCrea { get; set; }
public string UsuarioModificacion { get; set; }
public DateTime? serv_FechaModifica { get; set; }
        }
    }
