using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PetsHome.Business.Models
{
    // ─── Lista (DataTable) ─────────────────────────
    public partial class ClientesListViewModel
    {
        public int cli_Id { get; set; }
public string cli_Identidad { get; set; }
public string cli_Nombres { get; set; }
public string cli_Apellidos { get; set; }
public string cli_Telefono { get; set; }
public string cli_Correo { get; set; }
public string cli_Direccion { get; set; }
public bool cli_EsConsumidorFinal { get; set; }
    }

    // ─── Formulario Create/Edit ────────────────────
    public partial class ClientesFormViewModel
    {
        public int cli_Id { get; set; }
public string cli_Identidad { get; set; }
public string cli_Nombres { get; set; }
public string cli_Apellidos { get; set; }
public string cli_Telefono { get; set; }
public string cli_Correo { get; set; }
public string cli_Direccion { get; set; }
public bool cli_EsConsumidorFinal { get; set; }

        public bool isEdit => cli_Id > 0;
    }

    // ─── Busqueda (para pre-llenar el form en edicion) ─
    public partial class ClientesFindViewModel
    {
        public int cli_Id { get; set; }
public string cli_Identidad { get; set; }
public string cli_Nombres { get; set; }
public string cli_Apellidos { get; set; }
public string cli_Telefono { get; set; }
public string cli_Correo { get; set; }
public string cli_Direccion { get; set; }
public bool cli_EsConsumidorFinal { get; set; }
    }

    // ─── Detalle (read-only) ───────────────────────
    public partial class ClientesDetailViewModel
    {
        public int cli_Id { get; set; }
public string cli_Identidad { get; set; }
public string cli_Nombres { get; set; }
public string cli_Apellidos { get; set; }
public string cli_Telefono { get; set; }
public string cli_Correo { get; set; }
public string cli_Direccion { get; set; }
public bool cli_EsConsumidorFinal { get; set; }

        public string UsuarioCreacion      { get; set; }
        public DateTime FechaCrea          { get; set; }
        public string UsuarioModificacion  { get; set; }
        public DateTime? FechaModifica     { get; set; }
    }

    // ─── Dropdown ─────────────────────────────────
    public partial class ClientesDropdownViewModel
    {
        public int cli_Id { get; set; }
        public string Descripcion { get; set; }
    }
}
