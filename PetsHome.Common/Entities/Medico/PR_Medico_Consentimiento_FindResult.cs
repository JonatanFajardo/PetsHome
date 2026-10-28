namespace PetsHome.Common.Entities
{
    public partial class PR_Medico_Consentimiento_FindResult
    {
        public int cons_Id { get; set; }
        public int masc_Id { get; set; }
        public int? cli_Id { get; set; }
        public int? cita_Id { get; set; }
        public string cons_Tipo { get; set; }
        public string cons_Estado { get; set; }
        public string cons_Texto { get; set; }
        public string cons_Notas { get; set; }
    }
}
