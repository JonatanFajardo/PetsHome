namespace PetsHome.Common.Entities
{
    /// <summary>
    /// Fila plana del vademecum (farmaco + dosis por especie) para la
    /// calculadora de dosis por peso. Resultado de PR_Medico_Vademecum_List.
    /// </summary>
    public partial class PR_Medico_Vademecum_ListResult
    {
        public int vade_Id { get; set; }
        public string vade_Farmaco { get; set; }
        public string vade_Categoria { get; set; }
        public string vdos_Especie { get; set; }
        public decimal vdos_MgKgMin { get; set; }
        public decimal vdos_MgKgMax { get; set; }
        public string vdos_Via { get; set; }
        public string vdos_Frecuencia { get; set; }
        public decimal? vdos_MaxDosisUnicaMg { get; set; }
        public string vade_PresentacionesTabletaMg { get; set; }
        public string vade_ConcentracionesMgMl { get; set; }
        public string vade_Advertencias { get; set; }
        public string vdos_Notas { get; set; }
    }
}
