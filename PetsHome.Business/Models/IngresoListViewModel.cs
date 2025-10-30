using System;

namespace PetsHome.Business.Models
{
    /// <summary>
    /// View model para listados de ingresos.
    /// </summary>
    public class IngresoListViewModel
    {
        public long? Fila { get; set; }

        public int ingr_Id { get; set; }

        public int? repa_Id { get; set; }

        public int refg_Id { get; set; }

        public string refg_Nombre { get; set; }

        public DateTime ingr_FechaIngreso { get; set; }

        public string ingr_LugarRescate { get; set; }

        public string ingr_CondicionInicial { get; set; }

        public string ingr_PersonaRescatista { get; set; }

        public string ingr_MedioTransporte { get; set; }

        public string ingr_Observaciones { get; set; }

        public bool ingr_EsEmergencia { get; set; }

        public string LugarReporte { get; set; }

        public string repa_DescripcionAnimal { get; set; }

        public int TieneMascota { get; set; }

        public int? MascotaId { get; set; }

        public string MascotaNombre { get; set; }
    }
}
