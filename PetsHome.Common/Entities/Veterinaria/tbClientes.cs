using System;

namespace PetsHome.Common.Entities
{
    public partial class tbClientes
    {
        public int cli_Id { get; set; }
public string cli_Identidad { get; set; }
public string cli_Nombres { get; set; }
public string cli_Apellidos { get; set; }
public string cli_Telefono { get; set; }
public string cli_Correo { get; set; }
public string cli_Direccion { get; set; }
public bool cli_EsConsumidorFinal { get; set; }
public bool cli_EsEliminado { get; set; }
public int cli_UsuarioCrea { get; set; }
public DateTime cli_FechaCrea { get; set; }
public int? cli_UsuarioModifica { get; set; }
public DateTime? cli_FechaModifica { get; set; }
    }
}
