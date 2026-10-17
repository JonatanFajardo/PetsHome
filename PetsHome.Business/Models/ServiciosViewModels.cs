using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PetsHome.Business.Models
{
    // ─── Lista (DataTable) ─────────────────────────
    public partial class ServiciosListViewModel
    {
        public int serv_Id { get; set; }
public string serv_Codigo { get; set; }
public string serv_Nombre { get; set; }
public string serv_Descripcion { get; set; }
public decimal serv_Precio { get; set; }
    }

    // ─── Formulario Create/Edit ────────────────────
    public partial class ServiciosFormViewModel
    {
        public int serv_Id { get; set; }
public string serv_Codigo { get; set; }
public string serv_Nombre { get; set; }
public string serv_Descripcion { get; set; }
public decimal serv_Precio { get; set; }

        public bool isEdit => serv_Id > 0;
    }

    // ─── Busqueda (para pre-llenar el form en edicion) ─
    public partial class ServiciosFindViewModel
    {
        public int serv_Id { get; set; }
public string serv_Codigo { get; set; }
public string serv_Nombre { get; set; }
public string serv_Descripcion { get; set; }
public decimal serv_Precio { get; set; }
    }

    // ─── Detalle (read-only) ───────────────────────
    public partial class ServiciosDetailViewModel
    {
        public int serv_Id { get; set; }
public string serv_Codigo { get; set; }
public string serv_Nombre { get; set; }
public string serv_Descripcion { get; set; }
public decimal serv_Precio { get; set; }

        public string UsuarioCreacion      { get; set; }
        public DateTime FechaCrea          { get; set; }
        public string UsuarioModificacion  { get; set; }
        public DateTime? FechaModifica     { get; set; }
    }

    // ─── Dropdown ─────────────────────────────────
    public partial class ServiciosDropdownViewModel
    {
        public int serv_Id { get; set; }
        public string Descripcion { get; set; }
    }
}
