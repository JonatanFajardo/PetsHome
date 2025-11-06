using System;

namespace PetsHome.Business.Models
{
    /// <summary>
    /// Modelo utilizado para representar los registros del listado de tratamientos.
    /// </summary>
    public class TratamientoListViewModel
    {
        public int Fila { get; set; }
        public int trat_Id { get; set; }
        public int masc_Id { get; set; }
        public string Mascota { get; set; }
        public int? tipoPar_Id { get; set; }
        public string TipoParasito { get; set; }
        public string CategoriaParasito { get; set; }
        public string trat_ParasitoDetectado { get; set; }
        public string trat_Medicamento { get; set; }
        public int? tipoMed_Id { get; set; }
        public string TipoMedicamento { get; set; }
        public int? viaAdmin_Id { get; set; }
        public string ViaAdministracion { get; set; }
        public DateTime trat_FechaAplicacion { get; set; }
        public DateTime? trat_ProximaDosis { get; set; }
        public string trat_Estado { get; set; }
    }
}
