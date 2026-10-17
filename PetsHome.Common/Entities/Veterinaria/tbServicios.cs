using System;

namespace PetsHome.Common.Entities
{
    public partial class tbServicios
    {
        public int serv_Id { get; set; }
public string serv_Codigo { get; set; }
public string serv_Nombre { get; set; }
public string serv_Descripcion { get; set; }
public decimal serv_Precio { get; set; }
public bool serv_EsEliminado { get; set; }
public int serv_UsuarioCrea { get; set; }
public DateTime serv_FechaCrea { get; set; }
public int? serv_UsuarioModifica { get; set; }
public DateTime? serv_FechaModifica { get; set; }
    }
}
