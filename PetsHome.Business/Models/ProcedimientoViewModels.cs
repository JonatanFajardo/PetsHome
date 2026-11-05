using System;

namespace PetsHome.Business.Models
{
    /// <summary>Fila del historial de procedimientos (ficha del paciente y panel de la consulta).</summary>
    public class ProcedimientoListViewModel
    {
        public int Fila { get; set; }
        public int proc_Id { get; set; }
        public int masc_Id { get; set; }
        public string Mascota { get; set; }
        public int? cita_Id { get; set; }
        public string proc_Nombre { get; set; }
        public string proc_Descripcion { get; set; }
        public int? proc_RealizadoPor { get; set; }
        public string RealizadoPorNombre { get; set; }
        public string proc_Anestesia { get; set; }
        public int? proc_DuracionMinutos { get; set; }
        public string proc_Notas { get; set; }
        public DateTime proc_Fecha { get; set; }
        public int? serv_Id { get; set; }
        public string ServicioNombre { get; set; }
        public decimal? ServicioPrecio { get; set; }
    }

    public class ProcedimientoFindViewModel
    {
        public int proc_Id { get; set; }
        public int masc_Id { get; set; }
        public int? cita_Id { get; set; }
        public string proc_Nombre { get; set; }
        public string proc_Descripcion { get; set; }
        public int? proc_RealizadoPor { get; set; }
        public string proc_Anestesia { get; set; }
        public int? proc_DuracionMinutos { get; set; }
        public string proc_Notas { get; set; }
        public DateTime proc_Fecha { get; set; }
        public int? serv_Id { get; set; }
    }

    public class ProcedimientoDetailViewModel
    {
        public int proc_Id { get; set; }
        public int masc_Id { get; set; }
        public string Mascota { get; set; }
        public int? cita_Id { get; set; }
        public string proc_Nombre { get; set; }
        public string proc_Descripcion { get; set; }
        public int? proc_RealizadoPor { get; set; }
        public string RealizadoPorNombre { get; set; }
        public string proc_Anestesia { get; set; }
        public int? proc_DuracionMinutos { get; set; }
        public string proc_Notas { get; set; }
        public DateTime proc_Fecha { get; set; }
        public int? serv_Id { get; set; }
        public string ServicioNombre { get; set; }
        public decimal? ServicioPrecio { get; set; }
        public string UsuarioCreacion { get; set; }
        public DateTime proc_FechaCrea { get; set; }
        public string UsuarioModificacion { get; set; }
        public DateTime? proc_FechaModifica { get; set; }
    }

    /// <summary>
    /// Datos que envia el frontend (bloque de la consulta / ficha) para guardar
    /// un procedimiento estructurado. proc_Id &gt; 0 = edicion; 0 = alta nueva.
    /// </summary>
    public class ProcedimientoGuardarDto
    {
        public int ProcId { get; set; }
        public int MascId { get; set; }
        public int? CitaId { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Anestesia { get; set; }
        public int? DuracionMinutos { get; set; }
        public string Notas { get; set; }
        public int? ServId { get; set; }
    }

    /// <summary>Resultado del guardado de un procedimiento (para la respuesta JSON).</summary>
    public class ProcedimientoGuardarResultViewModel
    {
        public bool Ok { get; set; }
        public int ProcId { get; set; }
        public string Mensaje { get; set; }
        /// <summary>true si se encolo un cargo por el servicio ligado (cliente externo).</summary>
        public bool CargoEncolado { get; set; }
    }
}
